using System;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using imSSOUtils.cache;
using imSSOUtils.cache.entities;
using static imClickable.ImGuiController;
using static ImGuiNET.ImGuiWindowFlags;
using static imSSOUtils.cache.entities.Player;

namespace imSSOUtils.window.windows.modding.low_level
{
    /// <summary>
    /// Low-level player stats window.
    /// </summary>
    internal class LL_PlayerStatsWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Sizes.
        /// </summary>
        private readonly Vector2 size = new(486, 195);

        /// <summary>
        /// Integers.
        /// </summary>
        private static int retry;
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
            write_font($"Star Coins: {starCoins}", true, comfortaa_SemiBold_Main);
            write_font($"Jorvik Shillings: {jorvikShillings}", true, comfortaa_SemiBold_Main);
        }

        /// <summary>
        /// Write top content.
        /// </summary>
        private void write_top()
        {
            write_font(username, true, comfortaa_SemiBold_Large);
            write_font($"Level {level.ToString()}", true, comfortaa_SemiBold_Main);
        }

        /// <summary>
        /// Write middle content.
        /// </summary>
        private void write_middle()
        {
            write_font($"Riding: {riding.ToString()}", true, comfortaa_SemiBold_Main);
            write_font($"Caring: {caring.ToString()}", true, comfortaa_SemiBold_Main);
            write_font($"Command: {Player.command}", true, comfortaa_SemiBold_Main);
            write_font($"Jumping: {jumping.ToString()}", true, comfortaa_SemiBold_Main);
        }

        /// <summary>
        /// Open the window.
        /// </summary>
        public void open_window()
        {
            MemoryAdapter.direct_call(
                "Game->CharacterSheet::GlobalAccessShortcut(\"CSheet\");\n" +
                "Game->CSheet->TabView::GlobalAccessShortcut(\"CTab\");\n" +
                "Game->CTab->Me->Info::SetScaleX(0);\n" +
                "Game->CTab->Me->line::SetScaleX(0);\n" +
                "Game->CTab->Me->Background::SetScaleX(0);\n" +
                "Game->CTab->Me->Currency::SetScaleX(0);\n" +
                "Game->CTab->Me->Skills::SetScaleX(0);\n" +
                "Game->CTab::SetViewBorderColor(0, 0, 0, 0);\n" +
                "Game->CTab::SetViewTextColor(0, 0, 0, 0);\n" +
                "Game->CTab::SetScale(1, 1, 1);\n" +
                "Game->CTab->Me::SetPosition(-178, -115, 1);\n" +
                "Game->CTab->Me::SetScale(1, 1, 1);\n" +
                "Game->CSheet::SetScale(1, 1, 1);\n" +
                "Game->CSheet::SetViewBorderColor(1, 1, 1, 1);\n" +
                "Game->CSheet::SetViewHeight(335);\n" +
                "Game->CSheet::SetViewWidth(190);");
            Thread.Sleep(550);
            fetch_updates();
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
            if (shouldDisplay) return;
            retry = 0;
            fetch_e();
            shouldDisplay = true;
        }).Start();

        /// <summary>
        /// Fetch everything.
        /// </summary>
        private void fetch_e()
        {
            try
            {
                update_skills();
                update_currencies();
                update_level();
            }
            catch (Exception)
            {
                retry++;
                if (retry < 5) fetch_e();
                // BUG: This doesn't work well, so uncomment it when a fix has been implemented.
                // else Environment.Exit(-1);
            }
        }
    }
}