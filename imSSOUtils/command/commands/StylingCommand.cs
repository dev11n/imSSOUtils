using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using imSSOUtils.adapters;
using imSSOUtils.customs;
using imSSOUtils.window.windows;
using static imSSOUtils.mod.option.dynamic.CModOption;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Command for styling custom mods with controls.
    /// </summary>
    internal struct StylingCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            if (args is {Length: < 3})
            {
                ConsoleWindow.send_input("not enough arguments, adding controls:", "[dynamic styling]",
                    Color.OrangeRed);
                ConsoleWindow.send_input("Buttons: styling add [mod_name] button [name] [code]", "[dynamic styling]",
                    Color.White);
                ConsoleWindow.send_input("Input Text: styling add [mod_name] inputText [control_name]",
                    "[dynamic styling]",
                    Color.White);
                ConsoleWindow.send_input("Checkbox: styling add [mod_name] checkbox [control_name]",
                    "[dynamic styling]",
                    Color.White);
                ConsoleWindow.send_input("removing controls:", "[dynamic styling]", Color.OrangeRed);
                ConsoleWindow.send_input("styling remove [mod_name] [controlType_control_name]", "[dynamic styling]",
                    Color.White);
                return;
            }

            var mod = Alpine.get_cmod(args[2]);
            if (mod is null)
            {
                ConsoleWindow.send_input("couldn't find the specified mod!", "[dynamic styling]", Color.OrangeRed);
                return;
            }

            switch (args[1])
            {
                case "add":
                    add(args, mod);
                    break;
                case "remove":
                    if (args is not {Length: 4}) return;
                    Debug.WriteLine(args[3]);
                    mod.raw.Remove(args[3]);
                    FileAdapter.create_cmod(mod.raw);
                    ConsoleWindow.send_input("removed!", "[dynamic styling]", Color.Lime);
                    break;
                default:
                    ConsoleWindow.send_input("invalid mode!", "[dynamic styling]", Color.OrangeRed);
                    break;
            }
        }

        /// <summary>
        /// Adds a new control.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="mod"></param>
        private void add(IReadOnlyList<string> args, CMod mod)
        {
            switch (args[3])
            {
                case "button":
                    process_value(mod, buttonStart, args[4], args[5]);
                    break;
                case "inputText":
                    process_value(mod, inputTextStart, args[4], string.Empty);
                    break;
                case "checkbox":
                    process_value(mod, checkboxStart, args[4], string.Empty);
                    break;
            }
        }

        /// <summary>
        /// Add / Modify a JSON entry in a specific mod, then save it.
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void process_value(CMod mod, string prefix, string name, string value)
        {
            var key = prefix + name;
            if (mod.raw.ContainsKey(key)) mod.raw[key] = value;
            else mod.raw.Add(key, value);
            FileAdapter.create_cmod(mod.raw);
            ConsoleWindow.send_input("saved and loaded changes!", "[dynamic styling]", Color.Lime);
        }
    }
}