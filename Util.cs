using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;

namespace SimpleHeels;

public static unsafe class Utils {
    public static GameObject* GetGameObjectById(uint objectId) {
        for (var i = 0; i < Constants.ObjectLimit; i++) {
            var o = GameObjectManager.Instance()->Objects.IndexSorted[i].Value;
            if (o == null || o->EntityId != objectId) continue;
            return o;
        }

        return null;
    }
    
    public static Lazy<HashSet<uint>> StaticMinions = new(() => {
        try {
            return PluginService.Data.GetExcelSheet<Companion>()?.Where(c => c.Behavior.RowId == 3).Select(c => c.RowId)?.ToHashSet() ?? [];
        } catch {
            return [];
        }
    });

    public static bool IsPlayerWorld(this World world) {
        if (world.Name.Data.IsEmpty) return false;
        if (world.DataCenter.RowId == 0) return false;
        if (world.RowId is > 1000 and < 2000 and not 1200) return true;
        if (world.IsPublic) return true;
        return char.IsUpper((char)world.Name.Data.Span[0]);
    }

    public static string OrIfWhitespace(this string? s, string replacement) {
        return string.IsNullOrWhiteSpace(s) ? replacement : s;
    }

    public static string GetGitHash() {
        try {
            var assembly = Assembly.GetExecutingAssembly();
            var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (!string.IsNullOrEmpty(informationalVersion)) {
                var plusIndex = informationalVersion.IndexOf('+');
                if (plusIndex >= 0 && plusIndex < informationalVersion.Length - 1) {
                    return informationalVersion.Substring(plusIndex + 1);
                }
            }
        } catch {
            // 忽略错误
        }
        return "unknown";
    }
}
