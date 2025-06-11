using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace SimpleHeels;

public static class Changelog {
    private static bool _displayedTitle;
    private static float _latestChangelog = 1;
    private static PluginConfig? _config;
    private static bool _showAll;

    private static int _configIndex;
    private static bool _isOldExpanded;

    private static void Changelogs() {
        ChangelogFor(10.73f, "0.10.7.3", "Added manual save button.");
        ChangelogFor(10.72f, "0.10.7.2", () => {
            C("Added optional 'delay' parameter to emotesync command.");
            C("/heels emotesync delay [seconds]", 0, ImGuiColors.DalamudViolet);
        });
        ChangelogFor(10.7f, "0.10.7.0", () => {
            C("Added ability to change selected 'identity'.");
            C("When using another 'identity' configs for heels will be used for the configured character instead of the default.");
            C("/heels identity set [name]", 0, ImGuiColors.DalamudViolet);
            C("/heels identity reset", 0, ImGuiColors.DalamudViolet);
            C("or Right click a character in the honorific config window and select 'Identity as'");
        });
        ChangelogFor(10.6f, "0.10.6.0", () => {
            C("新增同步情感动作的指令，使所有玩家的情感动作保持一致。");
            C("/heels emotesync", 1, ImGuiColors.DalamudViolet);
            C("我不喜欢反复重绘，直到游戏凑巧让所有人动作同步。", 1, ImGuiColors.DalamudGrey2);
            C("新增用于调整临时偏移量的指令。");
            C("/heels temp set [...]", 1, ImGuiColors.DalamudViolet);
            C("/heels temp add [...]", 1, ImGuiColors.DalamudViolet);
            C("/heels temp reset", 1, ImGuiColors.DalamudViolet);
            C("使用指令时不附加额外参数可获取更多信息。", 2);
        });
        ChangelogFor(10.5f, "0.10.5.0", () => {
            C("添加了宠物坐标轴工具。");
            using (ImRaii.PushIndent(2)) {
                using (ImRaii.PushColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled))) {
                    ImGui.Checkbox("启用宠物坐标轴", ref Plugin.Config.MinionGizmo);
                }

                if (!Plugin.Config.MinionGizmo) return;
                ImGui.TextDisabled("当临时偏移窗口开启时，按住");
                ImGui.SameLine();
                if (HotkeyHelper.DrawHotkeyConfigEditor("##MinionGizmoHotkeyInChangelog", Plugin.Config.MinionGizmoHotkey, out var newKeys)) {
                    Plugin.Config.TempOffsetGizmoHotkey = newKeys;
                }
                ImGui.SameLine();
                ImGui.TextDisabled("来移动你的宠物。");
            }
        });
        ChangelogFor(10.4f, "0.10.4.0", "添加了在骑乘时调整临时偏移的功能。");
        ChangelogFor(10.3f, "0.10.3.0", () => {
            C("为临时偏移坐标轴添加了可配置的快捷键。");
            C("新安装默认设置为 ALT。", 2);
            C("对于在 0.10.3 版本之前安装插件的用户，默认设置将保持为 SHIFT。", 2);
            using (ImRaii.PushIndent(2)) {
                ImGui.TextDisabled("您的快捷键是");
                ImGui.SameLine();
                if (HotkeyHelper.DrawHotkeyConfigEditor("##TempOffsetGizmoHotkeyInChangelog", Plugin.Config.TempOffsetGizmoHotkey, out var newKeys, true)) {
                    Plugin.Config.TempOffsetGizmoHotkey = newKeys;
                }
            }
            
        });
        ChangelogFor(10.2f, "0.10.2.1", ()=> {
            C("修复了导致主角色无法在集体动作中被其他工具旋转的问题。");
        });
        ChangelogFor(10.2f, "0.10.2.0", ()=> {
            C("添加了可以在临时偏移中使用坐标轴工具的选项。");
            C("为临时偏移添加了俯仰和横滚支持。");
        });
        ChangelogFor(10.11f, "0.10.1.1", "修复了通过月海同步器应用相对情感动作偏移时未生效的问题。");
        ChangelogFor(10.10f, "0.10.1.0", () => {
            C("新增了使情感动作偏移相对于装备偏移生效的功能。");
            C("例如，这将允许情感动作偏移应用旋转，但保持原始鞋子偏移。", 2);
        });
        ChangelogFor(10.06f, "0.10.0.6", "新增了重新应用临时偏移的功能。");
        ChangelogFor(10.05f, "0.10.0.5", () => {
            C("新增了一个按钮，允许模组开发者将偏移属性复制到剪贴板。");
            C("禁用了 Simple Heels 模型属性编辑器。");
        });
        ChangelogFor(10.0f, "0.10.0.0", "更新以支持「金曦之遗辉」。");
        ChangelogFor(9.35f, "0.9.3.5", "修复了对新世界的支持。");
        ChangelogFor(9.34f, "0.9.3.4", "修复了使用精确定位情感动作时导致其他玩家短暂瞬移的问题。");
        ChangelogFor(9.31f, "0.9.3.1", () => {
            C("新增了右键点击偏移输入返回零值的选项。");
            C("为情感动作偏移新增了重置所有值为零的按钮。");
            C("增加了精确定位的范围。");
        });
        ChangelogFor(9.3f, "0.9.3.0", "添加了可选的精确定位共享功能，当执行循环情感动作时生效。");
        ChangelogFor(9.2f, "0.9.2.0", "增加了同步静态宠物位置的选项。");
        ChangelogFor(9.13f, "0.9.1.3", "修复单击加号/减号按钮时会连击的问题。");
        ChangelogFor(9.12f, "0.9.1.2", "将来自同步玩家的临时偏移应用到集体动作模式。");
        ChangelogFor(9.1f, "0.9.1.0", () => {
            C("添加临时偏移：");
            C("允许设置不会保存到配置中的偏移。", 1);
            C("可覆盖所有其他激活的偏移。", 1);
            C("从叠加层窗口编辑。", 1);
            C("/heels temp", 2);
            C("可以在插件设置中锁定和自定义叠加层窗口。", 2);
        });
        ChangelogFor(9.08f, "0.9.0.8", "修正了向其他插件报告情感动作偏移时出现的问题。");
        ChangelogFor(9.07f, "0.9.0.7", () => {
            C("在设置里添加了一个选项，允许在组分配中将偏移应用于宠物。");
            C("已删除用于同步偏移的旧数据。");
        });
        ChangelogFor(9.06f, "0.9.0.6", "Attempt to fix the positioning of other players in GPose.");
        ChangelogFor(9.05f, "0.9.0.5", () => {
            C("Dimmed character and group names in config window when disabled.");
            C("Fixed some UI not functioning correctly");
        });
        ChangelogFor(9.02f, "0.9.0.2", "Fixed applying offsets to GPose actors.");
        ChangelogFor(9.0f, "0.9.0.0", () => {
            C("Major rework of internals");
            C("Added 'Emote Offsets'");
            C("Allows Full 3D positioning while performing emotes.", 1);
            C("Allows rotation while performing emotes.", 1);
            C("Different Sitting and Sleeping poses can be assigned individual offsets.", 1);
            C("");
        });
        ChangelogFor(8.50f, "0.8.5.0", () => {
            C("Added ability to disable processing of config groups.");
            C("Added ability to toggle visibility of the 'Move/Copy' UI in character configs.");
            C("Fixed issue causing config window to be larger than intended on higher resolution screens.");
            C("The 'Create Group' option from an existing character config now requires holding SHIFT.");
            C("Improved 'active offset' displays for groups.");
            C("Added option to prefer model paths over equipment ID when adding new entries.");
        });
        ChangelogFor(8.42f, "0.8.4.2", "Fixed an issue causing synced ground sitting offset from not appearing when chair sitting offsets are at zero.");
        ChangelogFor(8.41f, "0.8.4.1", () => { C("Improved support for baked in model offsets, allowing mod developers to define offsets in TexTools."); });
        ChangelogFor(8.4f, "0.8.4.0", () => {
            C("Added a 'Default Offset' to apply to all unconfigured footwear.");
            C("Offset will no longer be applied while crafting.");
        });
        ChangelogFor(8.3f, "0.8.3.0", "Reapers will no longer have their offset changed while under the effect of Enshroud.");
        ChangelogFor(8.2f, "0.8.2.0", () => {
            C("Added name filtering for groups.");
            C("A character can appear in multiple groups, the top group will be applied first.", 1);
        });
        ChangelogFor(8.11f, "0.8.1.1", "Fixed offsets not applying in gpose and cutscenes.");
        ChangelogFor(8f, "0.8.0.0", () => {
            C("Added an option to create a group from a configured character.");
            C("Added an option to ignore Dalamud's 'Hide UI while GPose is active' option.");
            C("Added options to assign an offset for ground sitting and sleeping.");
        });
        ChangelogFor(7f, "0.7.0.0", () => {
            C("Defaulted new heel entries to enabled");
            C("Improved UX");
            C("Made it more clear when no config is enabled", 1);
            C("Made it more clear which entry is currently active.", 1);
            C("Added a note explaining conflicts when wearing multiple items that are configured", 1);
            C("Entries can now be enabled or disabled when locked.");
        });
        ChangelogFor(6.3f, "0.6.3.0", () => {
            C("Improved ordering method a bit");
            C("Added a lock to entries");
            C("Added method of renaming or copying character configs.");
        });
        ChangelogFor(6.2f, "0.6.2.0", () => { C("Added a way to reorder heel config entries."); });
        ChangelogFor(6.12f, "0.6.1.3", "Another attempt to fix offset getting stuck for some people.");
        ChangelogFor(6.12f, "0.6.1.2", "Fixed plugin breaking when character is redrawn by Penumbra or Glamourer.");
        ChangelogFor(6.11f, "0.6.1.1", "Fixed 0 offset not being reported correctly to other plugins.");
        ChangelogFor(6.10f, "0.6.1.0", "Allow NPC characters to have their offsets assigned by groups.");
        ChangelogFor(6.00f, "0.6.0.0", () => {
            C("Added Groups (");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text($"{(char)FontAwesomeIcon.PeopleGroup}");
            ImGui.PopFont();
            ImGui.SameLine();
            ImGui.Text(") to allow setting offsets to a range of characters");
        });
        ChangelogFor(5.11f, "0.5.1.1", "Increased maximum offset value.");
        ChangelogFor(5.1f, "0.5.1.0", () => {
            C("Now allows assigning sitting offset to characters that have only their standing offset assigned by IPC.");
            C("Now applies offsets to GPose and Cutscene actors.");
            C("Due to the way the game handles cutscenes, cutscenes featuring non-standing poses will be incorrect.", 1, ImGuiColors.DalamudGrey3);
        });
        ChangelogFor(5, "0.5.0.0", () => {
            C("Added support for assigning an offset when sitting in a chair.");
            C("This will not be synced until support is added through Mare Synchronos", 1, ImGuiColors.DalamudGrey3);
        });

        ChangelogFor(4, "0.4.0.0", "Added support for Body and Legs equipment that hide shoes.");
    }

    private static void Title() {
        if (_displayedTitle) return;
        _displayedTitle = true;
        ImGui.Text("更新日志");

        if (!_showAll && _config != null) {
            ImGui.SameLine();
            if (ImGui.SmallButton("忽略")) _config.DismissedChangelog = _latestChangelog;
        }

        ImGuiExt.Separator();
    }

    private static void ChangelogFor(float version, string label, Action draw) {
        _configIndex++;
        if (version > _latestChangelog) _latestChangelog = version;
        if (!_showAll && _config != null && _config.DismissedChangelog >= version) return;
        if (_configIndex == 4) _isOldExpanded = ImGui.TreeNodeEx("旧版本", ImGuiTreeNodeFlags.NoTreePushOnOpen);
        if (_configIndex >= 4 && _isOldExpanded == false) return;
        Title();
        ImGui.Text($"{label}:");
        ImGui.Indent();
        draw();
        ImGui.Unindent();
    }

    private static void ChangelogFor(float version, string label, string singleLineChangelog) {
        ChangelogFor(version, label, () => { C(singleLineChangelog); });
    }

    private static void C(string text, int indent = 0, Vector4? color = null) {
        for (var i = 0; i < indent; i++) ImGui.Indent();
        if (color != null)
            ImGui.TextColored(color.Value, $"- {text}");
        else
            ImGui.Text($"- {text}");

        for (var i = 0; i < indent; i++) ImGui.Unindent();
    }

    public static bool Show(PluginConfig config, bool showAll = false) {
        _displayedTitle = false;
        _config = config;
        _showAll = showAll;
        _configIndex = 0;
        Changelogs();
        if (_displayedTitle) {
            ImGui.Spacing();
            ImGui.Spacing();
            ImGuiExt.Separator();
            return true;
        }

        return false;
    }
}
