﻿using System;
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
		private readonly Stopwatch timer;
		private readonly double WidestCharWidth;

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
			timer = Stopwatch.StartNew();
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
			Grid entryContainer = new() { Margin = new Thickness(0) };
			entryContainer.RowDefinitions.Add(new());
			entryContainer.RowDefinitions.Add(new());

			Label name = new() {
				FontFamily = copySettingsFrom.FontFamily,
				FontSize = copySettingsFrom.FontSize,
				Foreground = copySettingsFrom.Foreground,
				Content = eventName,
				Padding = new Thickness(0),
				Margin = new Thickness(8, 0, 8, 0),
				ClipToBounds = false
			};
			name.SetValue(Grid.RowProperty, 0);
			name.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
			
			entryContainer.Children.Add(name);

			durationMillis.FontFamily = copySettingsFrom.FontFamily;
			durationMillis.FontSize   = copySettingsFrom.FontSize;
			durationMillis.Foreground = copySettingsFrom.Foreground;
			durationMillis.Padding = new Thickness(0);
			durationMillis.Margin = new Thickness(8, 0, 8, 0);
			durationMillis.SetValue(Grid.RowProperty, 1);
			durationMillis.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
			durationMillis.ClipToBounds = false;
			entryContainer.Children.Add(durationMillis);

			container.Children.Add(entryContainer);
		}
		public void Update() {
			var ms = timer.ElapsedMilliseconds;
			var duration = $"{ms}ms";
			durationMillis.Content = duration;
			durationMillis.Width = WidestCharWidth * duration.Length;//make the label wide enough to accomommodate every char within being max width to keep the label from bouncing around in size as it counts up
		}
	}
}
