
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitscoreCounter {
	internal class Config {
		public static Config Instance { get; set; }


		public virtual bool verticalLayout { get; set; } = true;

		[NonNullable, UseConverter]
		public virtual List<int> splits { get; set; } = new List<int>() { 115, 114, 110, 100, 0 };

		public static IEnumerable<int> FilterSplitsList(IEnumerable<int> splits) {
			var x = splits.Where(x => x <= 115 && x >= 0).Distinct().OrderByDescending(x => x);

			if(!x.Any())
				return new[] { 115 };

			return x;
		}

		public virtual void Changed() {
			var x = FilterSplitsList(splits);

			if(!x.SequenceEqual(splits))
				splits = x.ToList();
		}
	}
}
