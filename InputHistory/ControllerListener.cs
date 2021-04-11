using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace InputHistory {
	class ControllerListener {
		readonly Controller Controller = new(UserIndex.One);
		readonly Dispatcher dispatcher;
		readonly Task updater;
		State lastState;
		EventCode lastLStickOctant = EventCode.None, lastRStickOctant = EventCode.None;
		EventCode lastDPadCode = EventCode.None;
		public delegate void OnInputPress(EventCode code);
		public delegate void OnInputRelease(EventCode code);
		public event OnInputPress? InputPress;
		public event OnInputRelease? InputRelease;
		private readonly static EventCode[] LeftStickOctantCodes = new[] {
			EventCode.XInputLStickLeft,
			EventCode.XInputLStickDownLeft,
			EventCode.XInputLStickDown,
			EventCode.XInputLStickDownRight,
			EventCode.XInputLStickRight,
			EventCode.XInputLStickUpRight,
			EventCode.XInputLStickUp,
			EventCode.XInputLStickUpLeft,
			EventCode.XInputLStickLeft,
		};
		private readonly static EventCode[] RightStickOctantCodes = new[] {
			EventCode.XInputRStickLeft,
			EventCode.XInputRStickDownLeft,
			EventCode.XInputRStickDown,
			EventCode.XInputRStickDownRight,
			EventCode.XInputRStickRight,
			EventCode.XInputRStickUpRight,
			EventCode.XInputRStickUp,
			EventCode.XInputRStickUpLeft,
			EventCode.XInputRStickLeft,
		};
		private readonly static EventCode[] DPadCodes = new[] {//names for the bitfield of dpad directions
			/*0*/EventCode.None,
			/*1*/EventCode.XInputDPadUp,
			/*2*/EventCode.XInputDPadDown,
			/*3*/EventCode.None, // Up | Down
			/*4*/EventCode.XInputDPadLeft,
			/*5*/EventCode.XInputDPadUpLeft,
			/*6*/EventCode.XInputDPadDownLeft,
			/*7*/EventCode.XInputDPadLeft, // Up | Down | Left
			/*8*/EventCode.XInputDPadRight,
			/*9*/EventCode.XInputDPadUpRight,
			/*A*/EventCode.XInputDPadDownRight,
			/*B*/EventCode.XInputDPadRight, // Up | Down | Right
			/*C*/EventCode.None, // Left | Right
			/*D*/EventCode.Up, // Up | Left | Right
			/*E*/EventCode.Down, // Down | Left | Right
			/*F*/EventCode.None, // Up | Down | Left | Right
		};
		readonly EdgeTrigger<byte> LeftTriggerWatcher, RightTriggerWatcher;
		private readonly bool SeparateOutDiagonalDPadInputs;
		public ControllerListener(bool separateOutDiagonalDPadInputs = true) {
			dispatcher = Dispatcher.CurrentDispatcher;
			LeftTriggerWatcher = new(
				b => b > Gamepad.TriggerThreshold,
				() => dispatcher.Invoke(() => InputPress?.Invoke(EventCode.XInputLT)),
				() => dispatcher.Invoke(() => InputRelease?.Invoke(EventCode.XInputLT))
			);
			RightTriggerWatcher = new(
				b => b > Gamepad.TriggerThreshold,
				() => dispatcher.Invoke(() => InputPress?.Invoke(EventCode.XInputRT)),
				() => dispatcher.Invoke(() => InputRelease?.Invoke(EventCode.XInputRT))
			);
			SeparateOutDiagonalDPadInputs = separateOutDiagonalDPadInputs;
			updater = BackgroundUpdate();
		}
		private static double Hypot(double a, double b) => Math.Sqrt(a * a + b * b);

		const GamepadButtonFlags DPadMask =
			GamepadButtonFlags.DPadUp | GamepadButtonFlags.DPadDown | GamepadButtonFlags.DPadLeft | GamepadButtonFlags.DPadRight;
		public void Update() {
			var state = Controller.GetState();

			#region nondirectional digital buttons, d-pad if diagonals are not separate
			var changed = state.Gamepad.Buttons ^ lastState.Gamepad.Buttons;
			for(var f = SeparateOutDiagonalDPadInputs ? GamepadButtonFlags.Start : GamepadButtonFlags.DPadUp; f != GamepadButtonFlags.None; f = (GamepadButtonFlags)((ushort)f << 1)) {
				if((f & changed) != GamepadButtonFlags.None) {
					if((f & state.Gamepad.Buttons) == GamepadButtonFlags.None) {
						dispatcher.Invoke(() => InputRelease?.Invoke(EventEncoder.Encode(f)));
					} else {
						dispatcher.Invoke(() => InputPress?.Invoke(EventEncoder.Encode(f)));
					}
				}
			}
			#endregion

			#region d-pad if diagonals are separate
			if(SeparateOutDiagonalDPadInputs) {
				var dPadCode = DPadCodes[(ushort)(state.Gamepad.Buttons & DPadMask)];
				if(dPadCode != lastDPadCode) {
					if(lastDPadCode != EventCode.None) dispatcher.Invoke(() => InputRelease?.Invoke(lastDPadCode));
					if(dPadCode != EventCode.None) dispatcher.Invoke(() => InputPress?.Invoke(dPadCode));
				}
				lastDPadCode = dPadCode;
			}
			#endregion

			#region left stick
			var lStickOctant = Hypot(state.Gamepad.LeftThumbX, state.Gamepad.LeftThumbY) > Gamepad.LeftThumbDeadZone ?
				LeftStickOctantCodes[4 + (int)Math.Round(Math.Atan2(state.Gamepad.LeftThumbY, state.Gamepad.LeftThumbX) / (Math.PI / 4))]
				: EventCode.None;
			if(lStickOctant != lastLStickOctant) {
				if(lastLStickOctant != EventCode.None) dispatcher.Invoke(() => InputRelease?.Invoke(lastLStickOctant));
				if(lStickOctant != EventCode.None) dispatcher.Invoke(() => InputPress?.Invoke(lStickOctant));
			}
			#endregion

			#region right stick
			var rStickOctant = Hypot(state.Gamepad.RightThumbX, state.Gamepad.RightThumbY) > Gamepad.RightThumbDeadZone ?
				RightStickOctantCodes[4 + (int)Math.Round(Math.Atan2(state.Gamepad.RightThumbY, state.Gamepad.RightThumbX) / (Math.PI / 4))]
				: EventCode.None;
			if(rStickOctant != lastRStickOctant) {
				if(lastRStickOctant != EventCode.None) dispatcher.Invoke(() => InputRelease?.Invoke(lastRStickOctant));
				if(rStickOctant != EventCode.None) dispatcher.Invoke(() => InputPress?.Invoke(rStickOctant));
			}
			#endregion

			#region triggers
			LeftTriggerWatcher.Update(state.Gamepad.LeftTrigger);
			RightTriggerWatcher.Update(state.Gamepad.RightTrigger);
			#endregion

			#region update state
			lastState = state;
			lastLStickOctant = lStickOctant;
			lastRStickOctant = rStickOctant;
			#endregion
		}
		private Task BackgroundUpdate() => Task.Run(async () => {
			while(true) {
				Update();
				await Task.Delay(1);
			}
		});
	}
}
