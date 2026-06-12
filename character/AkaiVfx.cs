using System.Collections.Concurrent;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;
using YakumoAkai.character.power;

namespace YakumoAkai.character;

public static class AkaiVfx {
	// Mod 独立的场景缓存（避免被 PreloadManager 清理）

	public static Node2D GenVFXNode(string scenePath) {
		if (MyCustomModInitializer.ModSceneCache.TryGetValue(scenePath, out var modScene)) {
			return modScene.Instantiate<Node2D>();
		}
		return PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>();
	}

	public static T GenVFXNode<T>(string scenePath) where T : Node2D {
		if (MyCustomModInitializer.ModSceneCache.TryGetValue(scenePath, out var modScene)) {
			return modScene.Instantiate<T>();
		}
		return PreloadManager.Cache.GetScene(scenePath).Instantiate<T>();
	}
	
	public static Node2D? PlaySimple(string scenePath, Vector2 position, float lifetime = 2f) {
		if (!TestMode.IsOn && NCombatRoom.Instance != null) {
			Node2D node2D = GenVFXNode(scenePath);
			NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(node2D);
			node2D.GlobalPosition = position;
			
			// 创建定时器，超时后销毁
			SceneTreeTimer timer = node2D.GetTree().CreateTimer(lifetime);
			timer.Timeout += () => {
				if (GodotObject.IsInstanceValid(node2D)) {
					node2D.QueueFreeSafely();
				}
			};
			return node2D;
		}
		return null;
	}
	public static Node2D? Playback(string scenePath, Vector2 position) {
		if (!TestMode.IsOn && NCombatRoom.Instance != null) {
			Node2D node2D = GenVFXNode(scenePath);
			NCombatRoom.Instance.BackCombatVfxContainer.AddChildSafely(node2D);
			node2D.GlobalPosition = position;
			return node2D;
		}
		return null;
	}
	public static Node2D? Play(string scenePath, Vector2 position) {
		if (!TestMode.IsOn && NCombatRoom.Instance != null) {
			Node2D node2D = GenVFXNode(scenePath);
			NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(node2D);
			node2D.GlobalPosition = position;
			return node2D;
		}
		return null;
	}
}
