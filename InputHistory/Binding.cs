using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputHistory {
	public class Binding {
		private static object AsUriIfContainsADot(string s) => //returns the Uri representation of s if it contains a period
				s.Contains('.') ? new Uri(s) : s;
		public class Override {
			
			[JsonInclude]
			public readonly string Representation;
			[JsonInclude]
			public readonly EventCode[] Codes;

			public Override(string representation, EventCode[] codes) {
				Representation = representation;
				Codes = codes;
			}
		}
		[JsonInclude]
		public readonly string DefaultRepresentation;
		[JsonInclude]
		public readonly Override[] Overrides;
		[JsonConstructor]
		public Binding(string defaultRepresentation, Override[] overrides) {//Overrides with lower indices take precedence over higher-indexed Overrides
			DefaultRepresentation = defaultRepresentation;
			Overrides = overrides;
		}
		public Binding(string defaultRepresentation) {
			DefaultRepresentation = defaultRepresentation;
			Overrides = Array.Empty<Override>();
		}

		public object GetRepresentation(IEnumerable<EventCode> pressed) =>
			AsUriIfContainsADot(Overrides.Where(o => o.Codes.Intersect(pressed).Any()).FirstOrDefault()?.Representation ?? DefaultRepresentation);
		public (object, Override?) GetRepresentationAndOverride(IEnumerable<EventCode> pressed) {
			var o = Overrides.Where(o => o.Codes.Intersect(pressed).Any()).FirstOrDefault();
			return (AsUriIfContainsADot(o?.Representation ?? DefaultRepresentation), o);
		}
	}
}
