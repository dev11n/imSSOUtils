using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using CNLibrary;
using imSSOUtils.window.windows;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Quick access to game internal stuff.
    /// </summary>
    internal struct PXInternal
    {
        #region Variables
        /// <summary>
        /// Web Bytes
        /// </summary>
        public static string dc01_bytes, dc02_bytes, dc03_bytes, bypass_bytes;

        public static readonly List<string> directAddresses = new();

        /// <summary>
        /// Web Addresses
        /// </summary>
        public static string dc01, dc02, dc03, bypass;

        /// <summary>
        /// Pointer addresses converted into real ones.
        /// </summary>
        private static UIntPtr xReal, yReal, zReal;

        /// <summary>
        /// Address for accessing a variables data.
        /// </summary>
        public static string directAddress = string.Empty;

        /// <summary>
        /// Determines whether the value of <see cref="directAddress"/> has been cached or not.
        /// </summary>
        public static bool hasCachedDirect;

        /// <summary>
        /// Web Addresses
        /// </summary>
        private static string x, y, z;

        /// <summary>
        /// Web Addresses
        /// </summary>
        public static string state;
        #endregion
        #region Enums
        /// <summary>
        /// Horse states
        /// </summary>
        public enum HorseState
        {
            JUMPING = 7
        }
        #endregion

        /// <summary>
        /// Get the current horse position.
        /// </summary>
        /// <returns>A vector for the X, Y and Z coordinates.</returns>
        public static Vector3 get_horse_position()
        {
            var mem = MemoryAdapter.head.get_consult().Memory;
            return new Vector3(mem.read_float(xReal), mem.read_float(yReal), mem.read_float(zReal));
        }

        /// <summary>
        /// Cache everything that's needed.
        /// </summary>
        public static void cache_pointers()
        {
            // ? Web
            x = WebAdapter.GetAPIValue("pos_x").ToString();
            y = WebAdapter.GetAPIValue("pos_y").ToString();
            z = WebAdapter.GetAPIValue("pos_z").ToString();
            dc01 = WebAdapter.GetAPIValue("disconnect1").ToString();
            dc01_bytes = WebAdapter.GetAPIValue("disconnect1a").ToString();
            dc02 = WebAdapter.GetAPIValue("disconnect2").ToString();
            dc02_bytes = WebAdapter.GetAPIValue("disconnect2a").ToString();
            dc03 = WebAdapter.GetAPIValue("disconnect3").ToString();
            dc03_bytes = WebAdapter.GetAPIValue("disconnect3a").ToString();
            bypass = WebAdapter.GetAPIValue("bypass").ToString();
            bypass_bytes = WebAdapter.GetAPIValue("bypassa").ToString();
            state = WebAdapter.GetAPIValue("trigger").ToString();
            ConsoleWindow.send_input("cached pointers", "[system]", Color.White);
        }

        /// <summary>
        /// Convert X Y Z to an unsigned integer pointer.
        /// </summary>
        public static void convert()
        {
            // ? Convert
            xReal = Utils.get_real_address(x);
            yReal = Utils.get_real_address(y);
            zReal = Utils.get_real_address(z);
        }

        /// <summary>
        /// Change the horses position.
        /// </summary>
        /// <param name="pos">The new position.</param>
        public static void set_horse_position(Vector3 pos)
        {
            var mem = MemoryAdapter.head.get_consult().Memory;
            mem.write_float(x, pos.X);
            mem.write_float(y, pos.Y);
            mem.write_float(z, pos.Z);
        }

        /// <summary>
        /// Show a generic text window.
        /// </summary>
        /// <param name="headline"></param>
        /// <param name="text"></param>
        public static void show_generic_window(string headline, string text) => MemoryAdapter.direct_call(
            $"\nGame->GenericRequestWindow2->Headline::SetViewText(\"{headline}\");\n" +
            $"\nGame->GenericRequestWindow2->Message::SetViewText(\"{text}\");\n" +
            "Game->GenericRequestWindow2::Start();\n" +
            "Game->ReportWindow::SetScaleX(0);");

        /// <summary>
        /// Get the current horse state.
        /// </summary>
        /// <returns></returns>
        public static HorseState get_horse_state() =>
            (HorseState) MemoryAdapter.head.get_consult().Memory.read_int(state);

        /// <summary>
        /// Displays a basic middle-aligned text message.
        /// </summary>
        /// <param name="text"></param>
        public static void show_white_message(string text) => MemoryAdapter.direct_call(
            $"Game->GUI_RescueRanch_InfoText::SetViewText( \"{text}\" );\n" +
            "Game->GUI_RescueRanch_InfoText::Start();\n" +
            "Game->GUI_RescueRanch_InfoText->Duration::Start();\n" +
            "Game->ReportWindow::SetScaleX(0);");

        /// <summary>
        /// Displays a basic middle-aligned text message.
        /// </summary>
        /// <param name="identifier"></param>
        public static void show_white_message_identifier(string identifier) => MemoryAdapter.direct_call(
            $"Game->GUI_RescueRanch_InfoText::SetViewText( {identifier} );\n" +
            "Game->GUI_RescueRanch_InfoText::Start();\n" +
            "Game->GUI_RescueRanch_InfoText->Duration::Start();\n" +
            "Game->ReportWindow::SetScaleX(0);");

        /// <summary>
        /// Retrieves the code in order to show a basic white text message.
        /// </summary>
        /// <param name="text"></param>
        public static string get_white_text_message(string text) =>
            $"Game->GUI_RescueRanch_InfoText::SetViewText( \"{text}\" );\n" +
            "Game->GUI_RescueRanch_InfoText::Start();\n" +
            "Game->GUI_RescueRanch_InfoText->Duration::Start();";

        /// <summary>
        /// Displays a basic middle-aligned text message.
        /// </summary>
        /// <param name="value"></param>
        public static void show_white_message(int value) => MemoryAdapter.direct_call(
            $"Game->GUI_RescueRanch_InfoText::SetViewTextInt({value});\n" +
            "Game->GUI_RescueRanch_InfoText::Start();\n" +
            "Game->GUI_RescueRanch_InfoText->Duration::Start();");

        /// <summary>
        /// Retrieves the code in order to show a basic white int message.
        /// </summary>
        /// <param name="value"></param>
        public static string get_white_int_message(string value) =>
            $"Game->GUI_RescueRanch_InfoText::SetViewTextInt( {value} );\n" +
            "Game->GUI_RescueRanch_InfoText::Start();\n" +
            "Game->GUI_RescueRanch_InfoText->Duration::Start();";

        /// <summary>
        /// Draws the current location.
        /// </summary>
        /// <returns></returns>
        public static void draw_current_location()
        {
            var code = PXShort.p_if(
                           "Game->GlobalTempStringData::GetDataString() != Game->LocationNameMiniMap::GetViewText()",
                           "Game->InfoTextWindow3::SetViewText(Game->LocationNameMiniMap::GetViewText());\n" +
                           "Game->InfoTextWindow3::SetViewTextColor(1, 1, 1, 1);\n" +
                           "Game->InfoTextWindow3::Start();") +
                       "Game->GlobalTempStringData::SetDataString(Game->LocationNameMiniMap::GetViewText());";
            MemoryAdapter.direct_call(code);
        }
    }
}