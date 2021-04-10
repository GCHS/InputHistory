using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputHistory {
	public class BindingName {
		public class Override {
			[JsonInclude]
			public readonly string Name;
			[JsonInclude]
			public readonly EventCode[] Codes;

			public Override(string name, EventCode[] codes) {
				Name = name;
				Codes = codes;
			}
		}
		[JsonInclude]
		public readonly string Name;
		[JsonInclude]
		public readonly Override[] Overrides;
		[JsonConstructor]
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
		public (string, Override?) GetNameAndOverride(IEnumerable<EventCode> pressed) {
			var o = Overrides.Where(o => o.Codes.Intersect(pressed).Any()).FirstOrDefault();
			return (o?.Name ?? Name, o);
		}
	}
}
