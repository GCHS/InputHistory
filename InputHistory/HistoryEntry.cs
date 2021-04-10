using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static InputHistory.BindingName;

namespace InputHistory {
	partial class HistoryEntry {
		public readonly EventCode Code;
		public readonly Grid Entry = new() { Margin = new Thickness(0) };
		private readonly List<(Stopwatch, long)> timersAndStarts = new();
		private readonly Label Name = new();
		public readonly Override? Override = null;
		private readonly Label Count = new();
		private readonly Label DurationMillis = new();
		private readonly double WidestCharWidth;
		public double AverageDurationMillis { get; private set; }

		public static Dictionary<EventCode, BindingName> Names = new() {
			{EventCode.None, new("")},
			{EventCode.W, new("Forward")},
			{EventCode.A, new("Leftward")},
			{EventCode.S, new("Backward")},
			{EventCode.D, new("Rightward")},
			{EventCode.R, new("Lock On")},
			{EventCode.E, new("Change Target")},
			{EventCode.Q, new("Change Situation Command")},
			{EventCode.LeftCtrl,  new("Change Target")},
			{EventCode.LeftShift, new("Open Shortcut Menu")},
			{EventCode.M,  new("Pause")},
			{EventCode.D4, new("Pause")},
			{EventCode.N,  new("Open Camera")},
			{EventCode.D3, new("Open Camera")},
			{EventCode.Escape,  new("Open PC Menu")},
			{EventCode.Space,   new("Jump", new Override[] {new("Shortcut 4", new EventCode[]{EventCode.LeftShift})})},
			{EventCode.LeftAlt, new("Situation Command", new Override[] {new("Shortcut 2", new EventCode[]{EventCode.LeftShift})})},
			{EventCode.LeftMouseButton,   new("Attack", new Override[] {new("Shortcut 1", new EventCode[]{EventCode.LeftShift})})},
			{EventCode.RightMouseButton,  new("Block",  new Override[] {new("Shortcut 3", new EventCode[]{EventCode.LeftShift}), new("Dodge", new EventCode[]{EventCode.W, EventCode.A, EventCode.S, EventCode.D})})},
			{EventCode.MiddleMouseButton, new("Lock On")},
			{EventCode.XButton1, new("Equip Right Keyblade")},
			{EventCode.XButton2, new("Equip Left Keyblade")},
			{EventCode.ScrollDown, new("Menu Down")},
			{EventCode.ScrollUp,   new("Menu Up")},
		};
		
		private HistoryEntry() {
			timersAndStarts.Add((Stopwatch.StartNew(), Stopwatch.GetTimestamp()));
			Entry.RowDefinitions.Add(new());
			Entry.RowDefinitions.Add(new());
			Entry.ColumnDefinitions.Add(new());
			Entry.ColumnDefinitions.Add(new());
		}

		public static Override? GetOverride(EventCode code, IEnumerable<EventCode> liveEvents) =>
			(Names.TryGetValue(code, out var binding) ? binding.GetNameAndOverride(liveEvents) : (null, null)).Item2;

		public HistoryEntry(in EventCode code, in IEnumerable<EventCode> liveEvents, Panel container, Control copySettingsFrom, double widestCharWidth) : this() {
			Code = code;
			var (name, whichOverride) = Names.TryGetValue(Code, out var bindingName) ? bindingName.GetNameAndOverride(liveEvents) : ("Fatfinger", null);
			Override = whichOverride;
			ConfigureUI(name, container, copySettingsFrom);
			WidestCharWidth = widestCharWidth;
			Update();
		}

		private void ConfigureUI(string eventName, Panel container, Control copySettingsFrom) {
			#region configure name
			Name.FontFamily = copySettingsFrom.FontFamily;
			Name.FontSize = copySettingsFrom.FontSize;
			Name.Foreground = copySettingsFrom.Foreground;
			Name.Content = eventName;
			Name.Padding = new Thickness(0);
			Name.Margin = new Thickness(8, 0, 0, 0);
			Name.ClipToBounds = false;
			
			Name.SetValue(Grid.RowProperty, 0);
			Name.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Right);
			
			Entry.Children.Add(Name);
			#endregion

			#region configure durationMillis
			DurationMillis.FontFamily = copySettingsFrom.FontFamily;
			DurationMillis.FontSize   = copySettingsFrom.FontSize;
			DurationMillis.Foreground = copySettingsFrom.Foreground;
			DurationMillis.Padding = new Thickness(0);
			DurationMillis.Margin = new Thickness(8, 0, 8, 0);
			DurationMillis.SetValue(Grid.RowProperty, 1);
			DurationMillis.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
			DurationMillis.ClipToBounds = false;

			Entry.Children.Add(DurationMillis);
			#endregion

			#region configure count
			var superscript = new TransformGroup() { };
			superscript.Children.Add(new ScaleTransform(0.75, 0.75));

			Count.FontFamily = DurationMillis.FontFamily;
			Count.FontSize = DurationMillis.FontSize;
			Count.Foreground = DurationMillis.Foreground;
			Count.Content = "";
			Count.Padding = new Thickness(0);
			Count.Margin = new Thickness(0, 0, 8, 0);
			Count.ClipToBounds = false;
			Count.RenderTransform = superscript;

			Count.SetValue(Grid.RowProperty, 0);
			Count.SetValue(Grid.ColumnProperty, 1);
			Count.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);

			Entry.Children.Add(Count);
			#endregion

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
			DurationMillis.Content = message;
			DurationMillis.Width = WidestCharWidth * message.Length;//make the label wide enough to accomommodate every char within being max width to keep the label from bouncing around in size as it counts up
		}
		public void Stop() {
			foreach(var t in timersAndStarts) {
				t.Item1.Stop();
			}
		}
		public void Restart() {
			timersAndStarts.Add((Stopwatch.StartNew(), Stopwatch.GetTimestamp()));
			Count.Content = timersAndStarts.Count;
		}
	}
}
