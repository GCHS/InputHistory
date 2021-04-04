using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static InputHistory.BindingName;

namespace InputHistory {
	class HistoryEntry {
		public readonly RawKeyEventArgs KeyEvent;
		private readonly Label durationMillis;
		private readonly Stopwatch timer;

		private static readonly Dictionary<Key, BindingName> KbNames = new() {
			{Key.W, new("Forward")},
			{Key.A, new("Leftward")},
			{Key.S, new("Backward")},
			{Key.D, new("Rightward")},
			{Key.R, new("Lock On")},
			{Key.E, new("Change Target")},
			{Key.Q, new("Change Situation Command")},
			{Key.LeftCtrl,  new("Change Target")},
			{Key.LeftShift, new("Open Shortcut Menu")},
			{Key.M,  new("Pause")},
			{Key.D4, new("Pause")},
			{Key.N,  new("Open Camera")},
			{Key.D3, new("Open Camera")},
			{Key.Escape,  new("Open PC Menu")},
			{Key.Space,   new("Jump", new Override[] {new("Shortcut 4", new[]{Key.LeftShift})})},
			{Key.LeftAlt, new("Situation Command", new Override[] {new("Shortcut 2", new[]{Key.LeftShift})})},
		};
		private static readonly Dictionary<MouseButton, BindingName> MbNames = new() {
			{MouseButton.Left,     new("Attack", new Override[] {new("Shortcut 1", new[]{Key.LeftShift})})},
			{MouseButton.Right,    new("Block",  new Override[] {new("Shortcut 3", new[]{Key.LeftShift}), new("Dodge", new[]{Key.W, Key.A, Key.S, Key.D})})},
			{MouseButton.Middle,   new("Lock On")},
			{MouseButton.XButton1, new("Equip Right Keyblade")},
			{MouseButton.XButton2, new("Equip Left Keyblade")}
		};
		
		public HistoryEntry(in RawKeyEventArgs args, Panel container, in Key[] pressedKeys, in MouseButton[] pressedButtons) {
			timer = Stopwatch.StartNew();
			KeyEvent = args;
			Grid entryContainer = new();
			entryContainer.RowDefinitions.Add(new());
			entryContainer.RowDefinitions.Add(new());

			Label name = new();
			name.SetValue(Grid.RowProperty, 0);
			name.Content = KbNames.TryGetValue(args.Key, out var bindingName) ? bindingName.GetName(pressedKeys, pressedButtons) : "";
			name.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
			entryContainer.Children.Add(name);

			durationMillis = new();
			durationMillis.SetValue(Grid.RowProperty, 1);
			durationMillis.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
			entryContainer.Children.Add(durationMillis);

			container.Children.Add(entryContainer);

			Update();
		}
		public void Update() {
			durationMillis.Content = $"{timer.ElapsedMilliseconds}ms";
		}
	}
}
