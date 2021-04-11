using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputHistory {
	public class Binding {
		public class Override {
			[JsonInclude]
			public readonly string Representation;
			[JsonInclude]
			public readonly EventCode[] Codes;
			[JsonIgnore]
			public readonly Uri? uri;

			[JsonIgnore]
			public object PreferredRepresentation => (object?)uri ?? Representation;

			public Override(string representation, EventCode[] codes) {
				Representation = representation;
				Codes = codes;
				if(Representation.Contains('.')) {
					uri = new Uri(Path.GetFullPath(Representation));
				}
			}
		}
		[JsonInclude]
		public readonly string DefaultRepresentation;
		[JsonInclude]
		public readonly Override[] Overrides;
		[JsonIgnore]
		public readonly Uri? uri;
		[JsonIgnore]
		public object PreferredRepresentation => (object?)uri ?? DefaultRepresentation;

		[JsonConstructor]
		public Binding(string defaultRepresentation, Override[] overrides) {//Overrides with lower indices take precedence over higher-indexed Overrides
			DefaultRepresentation = defaultRepresentation;
			Overrides = overrides;
			if(DefaultRepresentation.Contains('.')) {
				uri = new Uri(Path.GetFullPath(DefaultRepresentation));
			}
		}
		public Binding(string defaultRepresentation) : this(defaultRepresentation, Array.Empty<Override>()){}

		public object GetRepresentation(IEnumerable<EventCode> pressed) =>
			Overrides.Where(o => o.Codes.Intersect(pressed).Any()).FirstOrDefault()?.PreferredRepresentation ?? DefaultRepresentation;
		public (object, Override?) GetRepresentationAndOverride(IEnumerable<EventCode> pressed) {
			var o = Overrides.Where(o => o.Codes.Intersect(pressed).Any()).FirstOrDefault();
			return (o?.PreferredRepresentation ?? PreferredRepresentation, o);
		}
	}
}
