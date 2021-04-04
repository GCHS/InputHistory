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
			foreach(var ev in LiveEvents)	ev.Update();
		}

		public void KListenerKeyDown(object sender, RawKeyEventArgs args) {
			LiveEvents.Add(new(args, HistoryContainer, LiveEvents.Select(e => e.KeyEvent.Key).ToArray(), Array.Empty<MouseButton>()));
		}
		public void KListenerKeyUp(object sender, RawKeyEventArgs args) {
			LiveEvents = LiveEvents.Where(e => e.KeyEvent.Key != args.Key).ToList();
		}
	}
}
