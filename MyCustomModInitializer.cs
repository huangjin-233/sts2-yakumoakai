using Godot.Bridge;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using YakumoAkai.character;
using YakumoAkai.character.card.ancient;
using YakumoAkai.character.card.basic;

namespace YakumoAkai
{
	[ModInitializer(nameof(Initialize))]
	public static class MyCustomModInitializer
	{
		public static void Initialize()
		{
			RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<GodGungnir, Gungnir>();

			ScriptManagerBridge.LookupScriptsInAssembly(typeof(Akai).Assembly);
			Log.Info("YakumoAkai - 加载成功!");
		}
	}
}
