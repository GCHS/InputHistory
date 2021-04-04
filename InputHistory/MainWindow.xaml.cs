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
		List<HistoryEntry> LiveEvents = new();
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
			if(!LiveEvents.Where(e => e.Key == args.Key).Any())
				LiveEvents.Add(new(args.Key, HistoryContainer, LiveEvents.Select(e => e.Key).ToArray(), Array.Empty<MouseButton>()));
		}
		public void KListenerKeyUp(object sender, RawKeyEventArgs args) {
			LiveEvents.RemoveAll(e => e.Key == args.Key);
		}
	}
}
