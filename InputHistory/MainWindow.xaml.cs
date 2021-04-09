using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		RawInputHandler RawInputHandler;
		public MainWindow() {
			InitializeComponent();
			CompositionTarget.Rendering += CompositionTarget_Rendering;
			//FontFamily = new FontFamily((string)Properties.Settings.Default.Properties["FontName"].DefaultValue);
			//FontSize = double.Parse((string)Properties.Settings.Default.Properties["FontSize"].DefaultValue);
			Background = new SolidColorBrush((Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromInvariantString((string)Properties.Settings.Default.Properties["BgColor"].DefaultValue));
			Foreground = new SolidColorBrush((Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromInvariantString((string)Properties.Settings.Default.Properties["FontColor"].DefaultValue));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			RawInputHandler = new(this);
			RawInputHandler.MouseDown += RawInputHandler_MouseDown;
			RawInputHandler.MouseUp += RawInputHandler_MouseUp;
			RawInputHandler.MouseScrolled += RawInputHandler_MouseScrolled;
		}

		private void CompositionTarget_Rendering(object? sender, EventArgs e) {
			if(LiveEvents.Count == 0) {
				LiveEvents.Add(new(Key.None, Array.Empty<EventCode>(), HistoryContainer, this));
			}
			foreach(var ev in LiveEvents)	ev.Update();
		}
		private IEnumerable<EventCode> CurrentlyActiveCodes => LiveEvents.Select(e => e.Code);

		private void AddEvent(EventCode code) {
			FinalizeEvent(Key.None);
			if(!CurrentlyActiveCodes.Where(c => c == code).Any())
				LiveEvents.Add(new HistoryEntry(code, CurrentlyActiveCodes, HistoryContainer, this));
		}
		private void FinalizeEvent(EventCode code) => LiveEvents.RemoveAll(e => e.Code == code);

		public void KListenerKeyDown(object sender, RawKeyEventArgs args) => AddEvent(args.Key);
		public void KListenerKeyUp(object sender, RawKeyEventArgs args) => FinalizeEvent(args.Key);
		private void RawInputHandler_MouseUp(MouseButton released) => FinalizeEvent(released);
		private void RawInputHandler_MouseDown(MouseButton pressed) => AddEvent(pressed);
		private void RawInputHandler_MouseScrolled(short dx, short dy) {
			FinalizeEvent(Key.None);
			if(dy < 0) _ = new HistoryEntry(EventCode.ScrollDown,  CurrentlyActiveCodes, HistoryContainer, this);
			if(dy > 0) _ = new HistoryEntry(EventCode.ScrollUp,    CurrentlyActiveCodes, HistoryContainer, this);
			if(dx < 0) _ = new HistoryEntry(EventCode.ScrollLeft,  CurrentlyActiveCodes, HistoryContainer, this);
			if(dx > 0) _ = new HistoryEntry(EventCode.ScrollRight, CurrentlyActiveCodes, HistoryContainer, this);
		}
	}
}
