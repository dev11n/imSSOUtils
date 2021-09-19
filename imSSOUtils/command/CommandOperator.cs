using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vortice.DXGI;

namespace imSSOUtils.command
{
    /// <summary>
    /// Basically the command manger.
    /// </summary>
    internal struct CommandOperator
    {
        #region Variables
        /// <summary>
        /// A list of all commands.
        /// </summary>
        private static readonly IEnumerable<Type> cmds = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => typeof(ICommand).IsAssignableFrom(t) && t != typeof(ICommand));
        #endregion

        /// <summary>
        /// Trigger a command.
        /// </summary>
        public static void push_command(string[] args)
        {
            var name = string.Empty;
            try
            {
                const string find = "command";
                foreach (var cmd in cmds)
                {
                    name = cmd.Name.ToLower();
                    // Traditional Substring is faster than "[..(object)]"
                    if (args[0].ToLower() != name.Substring(0, name.IndexOf(find, StringComparison.Ordinal))) continue;
                    var core = Activator.CreateInstance(cmd);
                    ((ICommand) core)?.push(args);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.WriteLine($"[Fatal Error] >> A command isn't following the correct name-scheme! In: {name}");
            }
        }
    }
}