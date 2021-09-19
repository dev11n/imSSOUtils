using System.Diagnostics;
using System.Numerics;
using imClickable;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.coatings;
using imSSOUtils.registers;
using static imSSOUtils.Program;

namespace imSSOUtils.window.windows
{
    /// <summary>
    /// Essentially the main window.
    /// </summary>
    internal class IntroWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// "Show (Window)" button size.
        /// </summary>
        private readonly Vector2 showButtonSize = new Vector2(200, 25);
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            var isRunning = true;
            if (!ImGui.Begin(identifier, ref isRunning,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize))
            {
                Overlay.Close = !isRunning;
                ImGui.End();
                return;
            }

            HexaCoating.plug();
            Overlay.Close = !isRunning;
            ImTools.CentreText("Welcome to SSOUtils!");
            ImTools.CentreText("To view the latest changes, write \"changelog\" in Console");
            if (ImGui.Button("Discord", new Vector2(500, 25)))
                Process.Start("cmd.exe", "/c start https://discord.gg/G8ADsuSBcg");
            ImGui.NewLine();
            if (ImGui.CollapsingHeader("Credits"))
            {
                ImGui.Bullet();
                ImGui.Text("qqt - Developer");
                ImGui.Bullet();
                ImGui.Text("Optimusik - Bypasses");
                ImGui.Bullet();
                ImGui.Text("mellinoe - C# Wrapper of ImGui");
                ImGui.Bullet();
                ImGui.Text("zaafar - ImGui.NET Utils");
                ImGui.Bullet();
                ImGui.Text("ocornut - ImGui");
                ImGui.Bullet();
                ImGui.Text("xo1337 - Styling");
                ImGui.Bullet();
                ImGui.Text("erfg12 - Memory.dll");
            }

            if (ImGui.CollapsingHeader("Experiments"))
                ImGui.Checkbox("Use an experimental version of injecting code - Not recommended", ref BaseRegister.useV2AlpineInject);

            if (ImGui.CollapsingHeader("Window States"))
            {
                if (ImGui.Button("Toggle Extra Information", showButtonSize)) showExtraInfo = !showExtraInfo;
                if (ImGui.Button("Show Save Manager", showButtonSize)) get_by_name("Save Manager").shouldDisplay = true;
                if (ImGui.Button("Show Visual", showButtonSize)) get_by_name("Visual").shouldDisplay = true;
                if (ImGui.Button("Show Movement", showButtonSize)) get_by_name("Movement").shouldDisplay = true;
                if (ImGui.Button("Show Misc", showButtonSize)) get_by_name("Misc").shouldDisplay = true;
                if (ImGui.Button("Show Cinematica", showButtonSize)) get_by_name("Cinematica").shouldDisplay = true;
                if (ImGui.Button("Show Console", showButtonSize)) get_by_name("Console").shouldDisplay = true;
                if (ImGui.Button("Show Alpine Editor", showButtonSize))
                    get_by_name("Alpine Editor").shouldDisplay = true;
                if (ImGui.Button("Show Custom Mods", showButtonSize)) get_by_name("Custom Mods").shouldDisplay = true;
            }

            if (MemoryAdapter.is_enabled()) ImGui.Checkbox("Show extra information", ref showExtraInfo);
            ImGui.End();
        }

        protected internal override void initialize() => identifier = "SSOUtils";
    }
}