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
			readonly public IEnumerable<EventCode> Codes;

			public Override(string name, IEnumerable<EventCode> codes) {
				Name = name;
				Codes = codes;
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

		public string GetName(IEnumerable<EventCode> pressed) =>
			Overrides.Where(o => o.Codes.Intersect(pressed).Any()).FirstOrDefault()?.Name ?? Name;
	}
}
