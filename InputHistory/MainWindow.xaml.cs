using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
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
		readonly double WidestCharWidth;
		readonly int MaxEntries;
		RawInputHandler RawInputHandler;
		readonly bool DoCoalesce;
		public MainWindow() {
			InitializeComponent();
			CompositionTarget.Rendering += CompositionTarget_Rendering;
			//FontFamily = new FontFamily((string)Properties.Settings.Default.Properties["FontName"].DefaultValue);
			//FontSize = double.Parse((string)Properties.Settings.Default.Properties["FontSize"].DefaultValue);
			Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.BgColor.R, Properties.Settings.Default.BgColor.G, Properties.Settings.Default.BgColor.B));
			Foreground = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.FontColor.R, Properties.Settings.Default.FontColor.G, Properties.Settings.Default.FontColor.B));
			MaxEntries = Properties.Settings.Default.MaxEntries;
			Width = Properties.Settings.Default.Width;
			Height = Properties.Settings.Default.Height;
			DoCoalesce = Properties.Settings.Default.CoalesceMashing;
			var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
			typeface.TryGetGlyphTypeface(out var glyphTypeface);
			WidestCharWidth = glyphTypeface.CharacterToGlyphMap.Max(cg => glyphTypeface.AdvanceWidths[cg.Value]) * FontSize;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			RawInputHandler = new(this);
			RawInputHandler.MouseDown += RawInputHandler_MouseDown;
			RawInputHandler.MouseUp += RawInputHandler_MouseUp;
			RawInputHandler.MouseScrolled += RawInputHandler_MouseScrolled;
		}

		private void CompositionTarget_Rendering(object? sender, EventArgs e) {
			if(LiveEvents.Count == 0) {
				LiveEvents.Add(new(Key.None, Array.Empty<EventCode>(), HistoryContainer, this, WidestCharWidth));
			}
			foreach(var ev in LiveEvents) ev.Update();
			if(HistoryContainer.Children.Count > MaxEntries) {
				HistoryContainer.Children.RemoveRange(0, HistoryContainer.Children.Count - MaxEntries);
			}
		}
		private IEnumerable<EventCode> CurrentlyActiveCodes => LiveEvents.Select(e => e.Code);

		private void AddEvent(EventCode code) {
			FinalizeEvent(Key.None);
			if(!CurrentlyActiveCodes.Where(c => c == code).Any()) {
				if(DoCoalesce && FinalizedEvents.Last().Code == code) {
					var toRestart = FinalizedEvents.Last();
					toRestart.Restart();
					LiveEvents.Add(toRestart);
					FinalizedEvents.RemoveAt(FinalizedEvents.Count - 1);
				} else if(DoCoalesce && FinalizedEvents.Count >= 2 && FinalizedEvents.Last().Code == Key.None && FinalizedEvents.TakeLast(2).First().Code == code) {
					var toRestart = FinalizedEvents.TakeLast(2);
					foreach(var r in toRestart) {
						r.Restart();
						r.Update();
					}
					LiveEvents.AddRange(toRestart);
					FinalizedEvents.RemoveRange(FinalizedEvents.Count - 2, 2);
				} else {
					LiveEvents.Add(new HistoryEntry(code, CurrentlyActiveCodes, HistoryContainer, this, WidestCharWidth));
				}
			}
		}
		private void FinalizeEvent(EventCode code) {
			var toFinalize = LiveEvents.Where(e => e.Code == code);
			foreach(var f in toFinalize) f.Stop();
			FinalizedEvents.AddRange(toFinalize);
			LiveEvents.RemoveAll(e => e.Code == code);
		}

		public void KListenerKeyDown(object sender, RawKeyEventArgs args) => AddEvent(args.Key);
		public void KListenerKeyUp(object sender, RawKeyEventArgs args) => FinalizeEvent(args.Key);
		private void RawInputHandler_MouseUp(MouseButton released) => FinalizeEvent(released);
		private void RawInputHandler_MouseDown(MouseButton pressed) => AddEvent(pressed);
		private void RawInputHandler_MouseScrolled(short dx, short dy) {
			if(dy < 0){AddEvent(EventCode.ScrollDown ); FinalizeEvent(EventCode.ScrollDown );}
			if(dy > 0){AddEvent(EventCode.ScrollUp 	 ); FinalizeEvent(EventCode.ScrollUp   );}
			if(dx < 0){AddEvent(EventCode.ScrollLeft ); FinalizeEvent(EventCode.ScrollLeft );}
			if(dx > 0){AddEvent(EventCode.ScrollRight); FinalizeEvent(EventCode.ScrollRight);}
		}

		private void Window_Closing(object sender, CancelEventArgs e) {
			Properties.Settings.Default.Width = Width;
			Properties.Settings.Default.Height = Height;
			Properties.Settings.Default.Save();
		}
	}
}
