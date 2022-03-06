using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace HitscoreCounter {
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin {
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }

		internal static string Name => "HitscoreCounter";

		[Init]
		public void Init(IPALogger logger, IPA.Config.Config config) {
			Config.Instance = config.Generated<Config>();
			Instance = this;
			Log = logger;
		}
	}
}
