using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InputHistory {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		readonly List<HistoryEntry> LiveEvents = new();
		readonly List<HistoryEntry> FinalizedEvents = new();
		readonly double WidestCharInDurationTextWidth;
		readonly int MaxEntries;
		RawInputHandler RawInputHandler;
		readonly ControllerListener ControllerListener;
		readonly bool DoCoalesce, ShowFatfingers;
		public MainWindow() {
			InitializeComponent();
			CompositionTarget.Rendering += CompositionTarget_Rendering;

			if(Properties.Settings.Default.SettingsMigrationRequired) {
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.SettingsMigrationRequired = false;
				Properties.Settings.Default.Save();
			}

			try {
				JsonSerializerOptions options = new() { WriteIndented = true };
				options.Converters.Add(new JsonStringEnumConverter());
				var bindNames = JsonSerializer.Deserialize(Properties.Settings.Default.BindingRepresentations, HistoryEntry.Bindings.GetType(), options) as Dictionary<EventCode, Binding>;
				if(bindNames is not null) {
					HistoryEntry.Bindings = bindNames;
				}
			} catch(Exception e) {
				Title = e.Message;
			}

			Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.BgColor.R, Properties.Settings.Default.BgColor.G, Properties.Settings.Default.BgColor.B));
			Foreground = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.FontColor.R, Properties.Settings.Default.FontColor.G, Properties.Settings.Default.FontColor.B));

			MaxEntries = Properties.Settings.Default.MaxEntries;
			
			Width = Properties.Settings.Default.Width;
			Height = Properties.Settings.Default.Height;
			
			DoCoalesce = Properties.Settings.Default.CoalesceMashing;
			ShowFatfingers = Properties.Settings.Default.ShowFatfingers;
			
			var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
			typeface.TryGetGlyphTypeface(out var glyphTypeface);
			WidestCharInDurationTextWidth = "0123456789msMS.".Select(c => glyphTypeface.CharacterToGlyphMap[c])
				.Max(g => glyphTypeface.AdvanceWidths[g]) * FontSize;

			ControllerListener = new(Properties.Settings.Default.SeparateOutDiagonalDPadInputs);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			RawInputHandler = new(this);
			RawInputHandler.MouseDown += RawInputHandler_MouseDown;
			RawInputHandler.MouseUp += RawInputHandler_MouseUp;
			RawInputHandler.MouseScrolled += RawInputHandler_MouseScrolled;

			ControllerListener.InputPress += ControllerListener_InputPress;
			ControllerListener.InputRelease += ControllerListener_InputRelease;
		}

		private void ControllerListener_InputPress(EventCode code) =>	AddEvent(code);
		private void ControllerListener_InputRelease(EventCode code) => FinalizeEvent(code);

		private void CompositionTarget_Rendering(object? sender, EventArgs e) {
			if(LiveEvents.Count == 0 && HistoryEntry.IsNamed(EventCode.None)) {
				LiveEvents.Add(new(EventCode.None, Array.Empty<EventCode>(), HistoryContainer, this, WidestCharInDurationTextWidth));
			}
			foreach(var ev in LiveEvents) ev.Update();
			if(HistoryContainer.Children.Count > MaxEntries) {
				HistoryContainer.Children.RemoveRange(0, HistoryContainer.Children.Count - MaxEntries);
			}
			if(FinalizedEvents.Count + LiveEvents.Count > MaxEntries &&
				/*ensure there's actually excess FinalizedEvents (just in case we somehow have a humongous amount of LiveEvents)*/
				FinalizedEvents.Count > LiveEvents.Count + MaxEntries) {
				FinalizedEvents.RemoveRange(0, FinalizedEvents.Count - LiveEvents.Count - MaxEntries);
			}
		}
		private IEnumerable<EventCode> CurrentlyActiveCodes => LiveEvents.Select(e => e.Code);

		private void AddEvent(EventCode code) {
			if(ShowFatfingers || HistoryEntry.IsNamed(code)) {
				FinalizeEvent(EventCode.None);
				var over = HistoryEntry.GetOverride(code, CurrentlyActiveCodes);
				if(!LiveEvents.Where(e => e.Code == code && e.Override == over).Any()) {
					if(DoCoalesce) {
						if(FinalizedEvents.Last().Code == code && FinalizedEvents.Last().Override == over) {
							var toRestart = FinalizedEvents.Last();
							toRestart.Restart();
							LiveEvents.Add(toRestart);
							FinalizedEvents.RemoveAt(FinalizedEvents.Count - 1);
							return;
						}
						if(FinalizedEvents.Count >= 2 && FinalizedEvents.Last().Code == EventCode.None) {
							var toRestart = FinalizedEvents.TakeLast(2).First();
							if(toRestart.Code == code && toRestart.Override == over) {
								if(HistoryContainer.Children[^1] == FinalizedEvents.Last().Entry) {
									HistoryContainer.Children.RemoveAt(HistoryContainer.Children.Count - 1);
								}

								toRestart.Restart();
								toRestart.Update();

								LiveEvents.Add(toRestart);
								LiveEvents.Add(FinalizedEvents.Last());
								FinalizedEvents.RemoveRange(FinalizedEvents.Count - 2, 2);
								return;
							}
						}
					}
					LiveEvents.Add(new HistoryEntry(code, CurrentlyActiveCodes, HistoryContainer, this, WidestCharInDurationTextWidth));
				}
			}
		}
		private void FinalizeEvent(EventCode code) {
			var toFinalize = LiveEvents.Where(e => e.Code == code);
			foreach(var f in toFinalize) f.Stop();
			FinalizedEvents.AddRange(toFinalize);
			LiveEvents.RemoveAll(e => e.Code == code);
		}

		public void KListenerKeyDown(object sender, RawKeyEventArgs args) => AddEvent(EventEncoder.Encode(args.Key));
		public void KListenerKeyUp(object sender, RawKeyEventArgs args) => FinalizeEvent(EventEncoder.Encode(args.Key));
		private void RawInputHandler_MouseUp(MouseButton released) => FinalizeEvent(EventEncoder.Encode(released));
		private void RawInputHandler_MouseDown(MouseButton pressed) => AddEvent(EventEncoder.Encode(pressed));
		private void RawInputHandler_MouseScrolled(short dx, short dy) {
			if(dy < 0){AddEvent(EventCode.ScrollDown ); FinalizeEvent(EventCode.ScrollDown );}
			if(dy > 0){AddEvent(EventCode.ScrollUp 	 ); FinalizeEvent(EventCode.ScrollUp   );}
			if(dx < 0){AddEvent(EventCode.ScrollLeft ); FinalizeEvent(EventCode.ScrollLeft );}
			if(dx > 0){AddEvent(EventCode.ScrollRight); FinalizeEvent(EventCode.ScrollRight);}
		}

		private void Window_Closing(object sender, CancelEventArgs e) {
			Properties.Settings.Default.Width = Width;
			Properties.Settings.Default.Height = Height;
			JsonSerializerOptions options = new() { WriteIndented = true };
			options.Converters.Add(new JsonStringEnumConverter());
			Properties.Settings.Default.BindingRepresentations = JsonSerializer.Serialize(HistoryEntry.Bindings, options);
			Properties.Settings.Default.Save();
		}
	}
}
