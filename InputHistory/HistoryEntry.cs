using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static InputHistory.Binding;

namespace InputHistory {
	partial class HistoryEntry {
		public readonly EventCode Code;
		public readonly Grid Entry = new() { Margin = new Thickness(0) };
		private readonly List<(Stopwatch, long)> timersAndStarts = new();
		private readonly Label Representation = new();
		public readonly Override? Override = null;
		private readonly Label Count = new();
		private readonly Label DurationMillis = new();
		private readonly double WidestCharWidth;
		public double AverageDurationMillis { get; private set; }

		public static Dictionary<EventCode, Binding> Bindings = new() {
			{EventCode.XInputDPadUp,    new("up.png")},
			{EventCode.XInputDPadDown,  new("down.png")},
			{EventCode.XInputDPadLeft,  new("left.png")},
			{EventCode.XInputDPadRight, new("right.png")},
			{EventCode.XInputDPadUpLeft, new("up left.png")},
			{EventCode.XInputDPadUpRight, new("up right.png")},
			{EventCode.XInputDPadDownLeft, new("down left.png")},
			{EventCode.XInputDPadDownRight, new("down right.png")},
			{EventCode.XInputStart, new("Start")},
			{EventCode.XInputBack,  new("Back")},
			{EventCode.XInputLeftThumb,  new("Pad L")},
			{EventCode.XInputRightThumb, new("Pad R")},
			{EventCode.XInputLeftShoulder,  new("p4.png")},
			{EventCode.XInputRightShoulder, new("p3.png")},
			{EventCode.XInputA, new("k1.png")},
			{EventCode.XInputB, new("k2.png")},
			{EventCode.XInputX, new("p1.png")},
			{EventCode.XInputY, new("p2.png") },
			{EventCode.XInputLStickUp,        new("up.png")},
			{EventCode.XInputLStickUpRight,   new("up right.png")},
			{EventCode.XInputLStickRight,     new("right.png")},
			{EventCode.XInputLStickDownRight, new("down right.png")},
			{EventCode.XInputLStickDown,      new("down.png")},
			{EventCode.XInputLStickDownLeft,  new("down left.png")},
			{EventCode.XInputLStickLeft,      new("left.png")},
			{EventCode.XInputLStickUpLeft,    new("up left.png")},
			{EventCode.XInputRStickUp,        new("up.png")},
			{EventCode.XInputRStickUpRight,   new("up right.png")},
			{EventCode.XInputRStickRight,     new("right.png")},
			{EventCode.XInputRStickDownRight, new("down right.png")},
			{EventCode.XInputRStickDown,      new("down.png")},
			{EventCode.XInputRStickDownLeft,  new("down left.png")},
			{EventCode.XInputRStickLeft,      new("left.png")},
			{EventCode.XInputRStickUpLeft,    new("up left.png")},
			{EventCode.XInputRT, new("k3.png")},
			{EventCode.XInputLT, new("k4.png")},
		};
		
		private HistoryEntry() {
			timersAndStarts.Add((Stopwatch.StartNew(), Stopwatch.GetTimestamp()));
			Entry.RowDefinitions.Add(new());
			Entry.RowDefinitions.Add(new());
			Entry.ColumnDefinitions.Add(new());
			Entry.ColumnDefinitions.Add(new());
		}

		public static Override? GetOverride(EventCode code, IEnumerable<EventCode> liveEvents) =>
			(Bindings.TryGetValue(code, out var binding) ? binding.GetRepresentationAndOverride(liveEvents) : (null, null)).Item2;

		public HistoryEntry(in EventCode code, in IEnumerable<EventCode> liveEvents, Panel container, Control copySettingsFrom, double widestCharWidth) : this() {
			Code = code;
			var (representation, whichOverride) = Bindings.TryGetValue(Code, out var bindingName) ? bindingName.GetRepresentationAndOverride(liveEvents) : ("Fatfinger", null);
			Override = whichOverride;
			ConfigureUI(representation, container, copySettingsFrom);
			WidestCharWidth = widestCharWidth;
			Update();
		}

		public static bool IsNamed(EventCode code) => Bindings.ContainsKey(code);

		private void ConfigureUI(object representation, Panel container, Control copySettingsFrom) {
			#region configure Representation
			Representation.FontFamily = copySettingsFrom.FontFamily;
			Representation.FontSize = copySettingsFrom.FontSize;
			Representation.Foreground = copySettingsFrom.Foreground;
			Representation.Content = representation switch {
				Uri u => new Image() { Source = new BitmapImage(u) },
				_ => representation,
			};
			Representation.Padding = new Thickness(0);
			Representation.Margin = new Thickness(8, 0, 0, 0);
			Representation.ClipToBounds = false;
			
			Representation.SetValue(Grid.RowProperty, 0);
			Representation.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Right);
			
			Entry.Children.Add(Representation);
			#endregion

			#region configure DurationMillis
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

			#region configure Count
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
