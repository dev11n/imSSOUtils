using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using static imClickable.ImGuiController;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows.modding.low_level
{
    internal class LL_PlayerStatsWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Sizes.
        /// </summary>
        private readonly Vector2 size = new(486, 298);

        /// <summary>
        /// Variables.
        /// </summary>
        private static string playerName = "PlayerName";

        /// <summary>
        /// Variables.
        /// </summary>
        private static int playerLevel;
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin(identifier, ref shouldDisplay, NoResize | NoScrollbar | NoScrollWithMouse))
            {
                ImGui.End();
                return;
            }

            ImGui.SetWindowSize(size);
            if (!CVar.hasCachedAll) return;
            write_top();
            ImGui.Separator();
        }

        /// <summary>
        /// Write with a font.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="centre"></param>
        /// <param name="font"></param>
        private void write_font(string text, bool centre, ImFontPtr font)
        {
            ImGui.PushFont(font);
            if (centre) ImTools.CentreText(text);
            else ImGui.Text(text);
            ImGui.PopFont();
        }

        /// <summary>
        /// Write top content.
        /// </summary>
        private void write_top()
        {
            write_font(playerName, true, comfortaa_SemiBold_Large);
            write_font($"Level {playerLevel.ToString()}", true, comfortaa_SemiBold_Main);
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void initialize() => identifier = "LOW_LEVEL_PLAYER_INFO";

        /// <summary>
        /// Fetch all the player information.
        /// </summary>
        public static void fetch_updates()
        {
            CVar.write_cvar01("CurrentPlayerName::GetDataString()", "String");
            playerName = CVar.read_cvar01_string();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetLevel()", "Int");
            playerLevel = CVar.read_cvar01_int();
        }
    }
}