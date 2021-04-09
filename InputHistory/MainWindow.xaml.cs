using System;
using System.Collections.Generic;
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
		}

		private void CompositionTarget_Rendering(object? sender, EventArgs e) {
			if(LiveEvents.Count == 0) {
				LiveEvents.Add(new(Key.None, HistoryContainer, Array.Empty<Key>(), Array.Empty<MouseButton>()));
			}
			foreach(var ev in LiveEvents)	ev.Update();
		}

		public void KListenerKeyDown(object sender, RawKeyEventArgs args) {
			LiveEvents.RemoveAll(e => e.Key == Key.None);
			if(!CurrentlyPressedKeys.Where(k => k == args.Key).Any())
				LiveEvents.Add(new(args.Key, HistoryContainer, CurrentlyPressedKeys, CurrentlyPressedMouseButtons));
		}
		public void KListenerKeyUp(object sender, RawKeyEventArgs args) {
			LiveEvents.RemoveAll(e => e.IsKey == true && e.Key == args.Key);
		}

		private IEnumerable<Key> CurrentlyPressedKeys => LiveEvents.Where(e => e.IsKey).Select(e => e.Key);
		private IEnumerable<MouseButton> CurrentlyPressedMouseButtons => LiveEvents.Where(e => !e.IsKey).Select(e => e.Button);

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			RawInputHandler = new(this);
			RawInputHandler.MouseDown += RawInputHandler_MouseDown;
			RawInputHandler.MouseUp += RawInputHandler_MouseUp;
		}

		private void RawInputHandler_MouseUp(MouseButton released) {
			LiveEvents.RemoveAll(e => e.IsKey == false && e.Button == released);
		}

		private void RawInputHandler_MouseDown(MouseButton pressed) {
			LiveEvents.RemoveAll(e => e.Key == Key.None);
			if(!CurrentlyPressedMouseButtons.Where(b => b == pressed).Any())
				LiveEvents.Add(new HistoryEntry(pressed, HistoryContainer, CurrentlyPressedKeys, CurrentlyPressedMouseButtons));
		}
	}
}
