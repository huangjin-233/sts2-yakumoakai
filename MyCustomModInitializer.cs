using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using Godot.Bridge;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace YakumoAkai
{
    [ModInitializer(nameof(Initialize))]
    public static class MyCustomModInitializer
    {
        public const string ModId = "yakumoakai";
        public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
        public static readonly ConcurrentDictionary<string, PackedScene> ModSceneCache = new();

        public static void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
            ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
            LoadScenes();
        }
        static void LoadScenes() {
            //你的场景字符串列表
            var paths = new List<string> {
                "res://scenes/vfx/ironwheel/ironwheel.tscn",
                "res://scenes/vfx/time/time.tscn",
            };
            foreach (var path in paths) {
                if (ModSceneCache.ContainsKey(path)) continue;
                var scene = ResourceLoader.Load<PackedScene>(path, null, ResourceLoader.CacheMode.Reuse);
                if (scene != null) {
                    ModSceneCache[path] = scene;
                }
            }
        }
        
    }
}
