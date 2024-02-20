using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.GeneratedSheets2;
using Newtonsoft.Json;

namespace SimpleHeels;

public unsafe record EmoteIdentifier([property: JsonProperty("e")] uint EmoteModeId, [property: JsonProperty("c")] byte CPoseState) {
    public virtual bool Equals(EmoteIdentifier? other) => other != null && EmoteModeId == other.EmoteModeId && CPoseState == other.CPoseState;
    public override int GetHashCode() => HashCode.Combine(EmoteModeId, CPoseState);

    public static Lazy<List<EmoteIdentifier>> EmoteList = new(() => {
        var l = new List<EmoteIdentifier>();
        foreach (var emoteMode in PluginService.Data.GetExcelSheet<EmoteMode>()!) {
            if (emoteMode.RowId == 0 || emoteMode.StartEmote.Row == 0) continue;
            for (byte i = 0; i < emoteMode.RowId switch { 1 => 4, 2 => 5, 3 => 3, _ => 1 }; i++) l.Add(new EmoteIdentifier(emoteMode.RowId, i));
        }

        return l;
    });

    public static IReadOnlyList<EmoteIdentifier> List => EmoteList.Value;

    private static readonly Dictionary<uint, string> Names = new();

    private static readonly Dictionary<uint, uint> Icons = new() {
        [3] = 64013 // Sleep -> Doze
    };

    private static string FetchName(uint emoteModeId) {
        var emoteMode = PluginService.Data.GetExcelSheet<EmoteMode>()?.GetRow(emoteModeId);
        if (emoteMode == null) return $"EmoteMode#{emoteModeId}";
        var emote = emoteMode.StartEmote.Value;
        if (emote == null) return $"EmoteMode#{emoteModeId}";
        return emote.Name.ToDalamudString().TextValue;
    }

    private static uint FetchIcon(uint emoteModeId) {
        var emoteMode = PluginService.Data.GetExcelSheet<EmoteMode>()?.GetRow(emoteModeId);
        if (emoteMode == null) return 0;
        var emote = emoteMode.StartEmote.Value;
        if (emote == null) return 0;

        return emote.Icon;
    }

    [Newtonsoft.Json.JsonIgnore]
    public string EmoteName {
        get {
            if (Names.TryGetValue(EmoteModeId, out var name)) return name;

            name = FetchName(EmoteModeId);
            Names.TryAdd(EmoteModeId, name);
            return name;
        }
    }

    [Newtonsoft.Json.JsonIgnore] public string Name => EmoteModeId is 1 or 2 or 3 ? $"{EmoteName} Pose {CPoseState + 1}" : EmoteName;

    [Newtonsoft.Json.JsonIgnore]
    public uint Icon {
        get {
            if (Icons.TryGetValue(EmoteModeId, out var icon)) return icon;

            icon = FetchIcon(EmoteModeId);
            Icons.TryAdd(EmoteModeId, icon);

            return icon;
        }
    }

    public static EmoteIdentifier? Get(Character* character) {
        if (character == null) return null;
        if (character->Mode is not (Character.CharacterModes.InPositionLoop or Character.CharacterModes.EmoteLoop)) return null;
        return new EmoteIdentifier(character->ModeParam, character->EmoteController.CPoseState);
    }

    public static bool TryGet(Character* character, [NotNullWhen(true)] out EmoteIdentifier? emoteIdentifier) {
        emoteIdentifier = Get(character);
        return emoteIdentifier != null;
    }
}
