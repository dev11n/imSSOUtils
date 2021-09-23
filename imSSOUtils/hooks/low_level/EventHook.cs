using System.Threading;
using imSSOUtils.adapters.low_level;
using static imSSOUtils.linker.EventLinker;

namespace imSSOUtils.hooks.low_level
{
    /// <summary>
    /// Reads code coming from events like OnPlayerTPNetLevelUp
    /// </summary>
    internal readonly struct EventHook
    {
        /// <summary>
        /// Enable the hook.
        /// </summary>
        public static void plug() => new Thread(() =>
        {
            while (true)
            {
                if (!CVar.hasCachedAll) continue;
                switch (CVar.read_cvar01_string())
                {
                    case "OP CS ACTI LEVELUP":
                        on_player_level_up();
                        CVar.write_cvar01("0", "String");
                        break;
                }

                Thread.Sleep(500);
            }
        }).Start();
    }
}