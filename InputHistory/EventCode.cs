using System.Windows.Input;

namespace InputHistory {
	public struct EventCode {
		private enum ScrollDirection {
			Up = 1, Right = 2, Down = 3, Left = 4
		}
		uint value;
		public static bool operator ==(EventCode l, EventCode r) => l.value == r.value;
		public static bool operator !=(EventCode l, EventCode r) => l.value != r.value;
		public static implicit operator EventCode(Key k) => new() { value = (uint)k};
		public static implicit operator EventCode(MouseButton m) => new() { value = (uint)(m+1) << 16};//add one to m because it indexes the left mouse button to 0 and 0 is taken by Key.None
		public static EventCode ScrollUp    => new() { value = (uint)ScrollDirection.Up << 24 };
		public static EventCode ScrollRight => new() { value = (uint)ScrollDirection.Right << 24 };
		public static EventCode ScrollDown  => new() { value = (uint)ScrollDirection.Down << 24 };
		public static EventCode ScrollLeft  => new() { value = (uint)ScrollDirection.Left << 24 };
	}
}
