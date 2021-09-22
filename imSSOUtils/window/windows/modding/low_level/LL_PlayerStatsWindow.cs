using System.Numerics;
using System.Threading;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using static imClickable.ImGuiController;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows.modding.low_level
{
    /// <summary>
    /// Low-level player stats window.
    /// </summary>
    internal class LL_PlayerStatsWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Player stats.
        /// </summary>
        private static uint riding, caring, command, jumping;

        /// <summary>
        /// Sizes.
        /// </summary>
        private readonly Vector2 size = new(486, 195);

        /// <summary>
        /// Player strings.
        /// </summary>
        private static string playerName = "PlayerName";

        /// <summary>
        /// Integers.
        /// </summary>
        private static int playerLevel, js, sc;
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin(identifier, ref shouldDisplay, NoResize | NoScrollbar | NoScrollWithMouse | NoCollapse))
            {
                ImGui.End();
                return;
            }

            ImGui.SetWindowSize(size);
            if (!CVar.hasCachedAll) return;
            write_top();
            ImGui.Separator();
            write_currencies();
            ImGui.Separator();
            write_middle();
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
        /// Write all the currencies.
        /// </summary>
        private void write_currencies()
        {
            write_font($"Star Coins: {sc}", true, comfortaa_SemiBold_Main);
            write_font($"Jorvik Shillings: {js}", true, comfortaa_SemiBold_Main);
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
        /// Write middle content.
        /// </summary>
        private void write_middle()
        {
            write_font($"Riding: {riding.ToString()}", true, comfortaa_SemiBold_Main);
            write_font($"Caring: {caring.ToString()}", true, comfortaa_SemiBold_Main);
            write_font($"Command: {command.ToString()}", true, comfortaa_SemiBold_Main);
            write_font($"Jumping: {jumping.ToString()}", true, comfortaa_SemiBold_Main);
        }

        /// <summary>
        /// Open the window.
        /// </summary>
        public void open_window()
        {
            MemoryAdapter.direct_call("Game->CharacterSheet::Stop();");
            fetch_updates();
            shouldDisplay = true;
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void initialize()
        {
            identifier = "Player";
            shouldDisplay = false;
        }

        /// <summary>
        /// Fetch all the player information.
        /// </summary>
        private void fetch_updates() => new Thread(() =>
        {
            CVar.write_cvar01("CurrentPlayerName::GetDataString()", "String");
            playerName = CVar.read_cvar01_string();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetLevel()", "Int");
            playerLevel = CVar.read_cvar01_int();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetRidingSkillTotalValue()", "UInt");
            riding = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetCaringSkillTotalValue()", "UInt");
            caring = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetCommandSkillTotalValue()", "UInt");
            command = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetJumpingSkillTotalValue()", "UInt");
            jumping = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayerStarMoney::GetDataInt()", "Int");
            sc = CVar.read_cvar01_int();
            CVar.write_cvar01("CurrentPlayerMoney::GetDataInt()", "Int");
            js = CVar.read_cvar01_int();
        }).Start();
    }
}