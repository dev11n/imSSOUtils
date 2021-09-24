using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using imSSOUtils.registers;

namespace imSSOUtils.cache.entities
{
    internal readonly struct Player
    {
        /// <summary>
        /// Have we already initialized everything?
        /// </summary>
        private static bool hasLoaded;

        /// <summary>
        /// Delayed between each CVar operation.
        /// </summary>
        private const int delay = 70;

        /// <summary>
        /// The players username.
        /// </summary>
        public static string username { get; private set; }

        /// <summary>
        /// The players level.
        /// </summary>
        public static int level { get; private set; }

        /// <summary>
        /// Riding skill value.
        /// </summary>
        public static uint riding { get; private set; }

        /// <summary>
        /// Caring skill value.
        /// </summary>
        public static uint caring { get; private set; }

        /// <summary>
        /// Command skill value.
        /// </summary>
        public static uint command { get; private set; }

        /// <summary>
        /// Jumping skill value.
        /// </summary>
        public static uint jumping { get; private set; }

        /// <summary>
        /// Star Coins amount.
        /// </summary>
        public static int starCoins { get; private set; }

        /// <summary>
        /// Jorvik Shillings amount.
        /// </summary>
        public static int jorvikShillings { get; private set; }

        /// <summary>
        /// Initialize everything.
        /// </summary>
        public static void initialize()
        {
            if (hasLoaded || !MemoryAdapter.is_enabled() || !CVar.hasCachedAll) return;
            CVar.write_cvar01("CurrentPlayerName::GetDataString()", "String", delay);
            username = CVar.read_cvar01_string();
            update_level();
            update_skills();
            update_currencies();
            ready_events();
            hasLoaded = true;
        }

        /// <summary>
        /// Make certain events ready for code execution.
        /// </summary>
        private static void ready_events()
        {
            // Event -> OnPlayerTPNetLevelUp
            MemoryAdapter.replace_all(LowLevelRegister.on_player_level_up,
                "global/QuestCollectCompleteWindow/Script/sText.SetDataString(\"OP CS ACTI LEVELUP\");");
        }

        /// <summary>
        /// Updates the player level.
        /// </summary>
        public static void update_level()
        {
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetLevel()", "Int", delay);
            level = CVar.read_cvar01_int();
        }

        /// <summary>
        /// Updates all skill values.
        /// </summary>
        public static void update_skills()
        {
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetRidingSkillTotalValue()", "UInt", delay);
            riding = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetCaringSkillTotalValue()", "UInt", delay);
            caring = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetCommandSkillTotalValue()", "UInt", delay);
            command = CVar.read_cvar01_uint();
            CVar.write_cvar01("CurrentPlayer::GetPlayerTPNetJumpingSkillTotalValue()", "UInt", delay);
            jumping = CVar.read_cvar01_uint();
        }

        /// <summary>
        /// Updates all currencies.
        /// </summary>
        public static void update_currencies()
        {
            CVar.write_cvar01("CurrentPlayerStarMoney::GetDataInt()", "Int", delay);
            starCoins = CVar.read_cvar01_int();
            CVar.write_cvar01("CurrentPlayerMoney::GetDataInt()", "Int", delay);
            jorvikShillings = CVar.read_cvar01_int();
        }
    }
}