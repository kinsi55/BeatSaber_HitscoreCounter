using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HitscoreCounter {
	class SettingsHandler : INotifyPropertyChanged {
		Config config => Config.Instance;

		string splits {
			get => string.Join(",", config.splits);
			set {
				config.splits = value
					.Split(new[] { ",", ";", " " }, System.StringSplitOptions.RemoveEmptyEntries)
					.Select(x => {
						if(int.TryParse(x, out var l))
							return l;

						return -1;
					}).ToList();

				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(splits)));
			}
		}

		private readonly string version = $"Version {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)} by Kinsi55\nCommissioned by zachakaquack";

		public event PropertyChangedEventHandler PropertyChanged;
	}
}