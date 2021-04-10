using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static InputHistory.BindingName;

namespace InputHistory {
	partial class HistoryEntry {
		public readonly EventCode Code;
		private readonly Label durationMillis;
		public readonly Grid Entry;
		private readonly List<(Stopwatch, long)> timersAndStarts = new();
		private readonly List<Label> names = new();
		private readonly List<Label> counts = new();
		private readonly double WidestCharWidth;
		public double AverageDurationMillis { get; private set; }

		private static readonly Dictionary<EventCode, BindingName> Names = new() {
			{Key.None, new("")},
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
			{Key.Space,   new("Jump", new Override[] {new("Shortcut 4", new EventCode[]{Key.LeftShift})})},
			{Key.LeftAlt, new("Situation Command", new Override[] {new("Shortcut 2", new EventCode[]{Key.LeftShift})})},
			{MouseButton.Left,     new("Attack", new Override[] {new("Shortcut 1", new EventCode[]{Key.LeftShift})})},
			{MouseButton.Right,    new("Block",  new Override[] {new("Shortcut 3", new EventCode[]{Key.LeftShift}), new("Dodge", new EventCode[]{Key.W, Key.A, Key.S, Key.D})})},
			{MouseButton.Middle,   new("Lock On")},
			{MouseButton.XButton1, new("Equip Right Keyblade")},
			{MouseButton.XButton2, new("Equip Left Keyblade")},
			{EventCode.ScrollDown, new("Menu Down")},
			{EventCode.ScrollUp,   new("Menu Up")},
		};
		
		private HistoryEntry() {
			timersAndStarts.Add((Stopwatch.StartNew(), Stopwatch.GetTimestamp()));
			Entry = new() { Margin = new Thickness(0) };
			Entry.RowDefinitions.Add(new());
			Entry.RowDefinitions.Add(new());
			Entry.ColumnDefinitions.Add(new());
			Entry.ColumnDefinitions.Add(new());
			durationMillis = new();
		}

		public HistoryEntry(in EventCode code, in IEnumerable<EventCode> liveEvents, Panel container, Control copySettingsFrom, double widestCharWidth) : this() {
			Code = code;
			var name = Names.TryGetValue(Code, out var bindingName) ? bindingName.GetName(liveEvents) : "Fatfinger";
			ConfigureUI(name, container, copySettingsFrom);
			WidestCharWidth = widestCharWidth;
			Update();
		}

		private void ConfigureUI(string eventName, Panel container, Control copySettingsFrom) {
			Label name = new() {
				FontFamily = copySettingsFrom.FontFamily,
				FontSize = copySettingsFrom.FontSize,
				Foreground = copySettingsFrom.Foreground,
				Content = eventName,
				Padding = new Thickness(0),
				Margin = new Thickness(8, 0, 0, 0),
				ClipToBounds = false
			};
			name.SetValue(Grid.RowProperty, 0);
			name.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Right);
			names.Add(name);
			
			Entry.Children.Add(name);

			durationMillis.FontFamily = copySettingsFrom.FontFamily;
			durationMillis.FontSize   = copySettingsFrom.FontSize;
			durationMillis.Foreground = copySettingsFrom.Foreground;
			durationMillis.Padding = new Thickness(0);
			durationMillis.Margin = new Thickness(8, 0, 8, 0);
			durationMillis.SetValue(Grid.RowProperty, 1);
			durationMillis.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
			durationMillis.ClipToBounds = false;
			Entry.Children.Add(durationMillis);


			var superscript = new TransformGroup() { };
			superscript.Children.Add(new ScaleTransform(0.75, 0.75));
			//superscript.Children.Add(new TranslateTransform(0, -durationMillis.FontSize / 0.25));

			

			Label count = new() {
				FontFamily = durationMillis.FontFamily,
				FontSize = durationMillis.FontSize,
				Foreground = durationMillis.Foreground,
				Content = "",
				Padding = new Thickness(0),
				Margin = new Thickness(0, 0, 8, 0),
				ClipToBounds = false,
				RenderTransform = superscript
			};

			count.SetValue(Grid.RowProperty, 0);
			count.SetValue(Grid.ColumnProperty, 1);
			count.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);

			counts.Add(count);
			Entry.Children.Add(count);

			container.Children.Add(Entry);
		}
		public void Update() {
			string? message;
			if(timersAndStarts.Count == 1) { //duration mode
				message = $"{timersAndStarts.Last().Item1.ElapsedMilliseconds}ms";
			} else { //frequency mode
				var risingEdgeFreq = Stopwatch.Frequency / timersAndStarts.SkipLast(1).Zip(timersAndStarts.Skip(1)).Select(ts => ts.Second.Item2 - ts.First.Item2).Average();
				var fallingEdgeFreq = Stopwatch.Frequency / timersAndStarts.SkipLast(1).Zip(timersAndStarts.Skip(1)).Select(ts => ts.Second.Item2 + ts.Second.Item1.ElapsedTicks - ts.First.Item2 - ts.First.Item1.ElapsedTicks).Average();
				message = $"{(risingEdgeFreq + fallingEdgeFreq) / 2:F3}Hz";
			}
			durationMillis.Content = message;
			durationMillis.Width = WidestCharWidth * message.Length;//make the label wide enough to accomommodate every char within being max width to keep the label from bouncing around in size as it counts up
		}
		public void Stop() {
			foreach(var t in timersAndStarts) {
				t.Item1.Stop();
			}
		}
		public void Restart() {
			timersAndStarts.Add((Stopwatch.StartNew(), Stopwatch.GetTimestamp()));
			counts[0].Content = timersAndStarts.Count;
		}
	}
}
