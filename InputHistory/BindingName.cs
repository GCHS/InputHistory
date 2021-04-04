using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputHistory {
	class BindingName {
		public class Override {
			readonly public string Name;
			readonly public Key[] Keys;
			readonly public MouseButton[] MouseButtons;

			public Override(string name, Key[] keys, MouseButton[] mouseButtons) {
				Name = name;
				Keys = keys;
				MouseButtons = mouseButtons;
			}
			public Override(string name, Key[] keys) {
				Name = name;
				Keys = keys;
				MouseButtons = Array.Empty<MouseButton>();
			}
			public Override(string name, MouseButton[] mouseButtons) {
				Name = name;
				Keys = Array.Empty<Key>();
				MouseButtons = mouseButtons;
			}
		}
		private readonly string Name;
		private readonly Override[] Overrides;

		public BindingName(string name, Override[] overrides) {//Overrides with lower indices take precedence over higher-indexed Overrides
			Name = name;
			Overrides = overrides;
		}
		public BindingName(string name) {
			Name = name;
			Overrides = Array.Empty<Override>();
		}

		public string GetName(Key[] pressedKeys, MouseButton[] pressedButtons) =>
			Overrides.Where(o => o.Keys.Intersect(pressedKeys).Any() || o.MouseButtons.Intersect(pressedButtons).Any()).FirstOrDefault()?.Name ?? Name;
	}
}
