using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coroutine;
using imClickable;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.command;
using imSSOUtils.command.commands;
using imSSOUtils.mod;
using imSSOUtils.mod.option.dynamic;
using imSSOUtils.window;
using Veldrid;

namespace imSSOUtils
{
    internal struct Program
    {
        /// <summary>
        /// All structures / classes that inherits from <see cref="IWindow"/>.
        /// </summary>
        private static readonly IEnumerable<Type> windows = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => typeof(IWindow).IsAssignableFrom(t) && t != typeof(IWindow));

        /// <summary>
        /// All cached windows which can be called directly.
        /// </summary>
        private static readonly IWindow[] wnd = new IWindow[windows.Count()];

        /// <summary>
        /// Show extra info, such as horse position etc?
        /// </summary>
        internal static bool showExtraInfo = true;

        /// <summary>
        /// Show the clickable UI?
        /// </summary>
        public static bool showUI = true;

        /// <summary>
        /// Startup logic
        /// </summary>
        private static async Task Main()
        {
            if (!MemoryAdapter.is_elevated() && !Debugger.IsAttached) return;
            await initialize();
        }

        /// <summary>
        /// Initialize everything
        /// </summary>
        private static async Task initialize()
        {
            try
            {
                cache_instances();
                ModOperator.cache_mods();
                FileAdapter.initialize();
                Alpine.initialize_ascript();
                await WebAdapter.cache_api();
                if (!Debugger.IsAttached) await verify_version();
                CoroutineHandler.Start(SubmitRenderLogic());
                //if (!Debugger.IsAttached) 
                    patch();
                PXOverlay.begin_check();
                // ! KeyboardHook.start(); -- This has major performance issues and should be rewritten!
                Overlay.RunInfiniteLoop();
                dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                write_crash(e);
            }
        }

        /// <summary>
        /// Print a new crash file.
        /// </summary>
        /// <param name="e"></param>
        public static void write_crash(Exception e)
        {
            dispose();
            if (Debugger.IsAttached) return;
            var crash = $"crashlog_{DateTime.Now}.log";
            File.Create(crash).Close();
            File.WriteAllText(crash, $"-- EXCEPTION: {e.GetType()}\n-- MESSAGE: {e.Message}\n-- TRACE:\n{e}");
            Environment.Exit(-1);
        }

        /// <summary>
        /// Dispose and clean up everything when the program closes normally.
        /// </summary>
        private static void dispose()
        {
            ModOperator.dispose_timers();
            WebAdapter.dispose_web();
            CModOption.Dispose();
            PXOverlay.checkWindows?.Dispose();
            MemoryAdapter.syncPosition?.Dispose();
        }

        /// <summary>
        /// Verify the version.
        /// </summary>
        private static async Task verify_version()
        {
            var version = await WebAdapter.DownloadContent("https://meadowriders.com/burst/cuver.pli");
            if (version is not InformationCommand.version)
            {
                WebAdapter.dispose_web();
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Patch the game as soon as the <see cref="Overlay.OnRender"/> event is called.
        /// </summary>
        private static void patch() => CommandOperator.push_command(new[] {"ready"});

        /// <summary>
        /// Cache all <see cref="windows"/> instances to <see cref="wnd"/>.
        /// </summary>
        private static void cache_instances()
        {
            for (var i = 0; i < windows.Count(); i++)
                wnd[i] = Activator.CreateInstance(windows.ElementAt(i)) as IWindow;
        }

        /// <summary>
        /// Basic render logic.
        /// </summary>
        private static IEnumerator<Wait> SubmitRenderLogic()
        {
            for (var i = 0; i < wnd.Length; i++) wnd[i].initialize();
            var wait = new Wait(Overlay.OnRender);
            while (true)
            {
                yield return wait;
                if (!showUI || !PXOverlay.inSSO) continue;
                for (var i = 0; i < wnd.Length; i++)
                {
                    var window = wnd[i];
                    if (window.shouldDisplay)
                    {
                        ImGui.PushFont(ImGuiController.comfortaa_SemiBold_Main);
                        window.draw();
                    }
                }
            }
        }

        /// <summary>
        /// Get a window by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IWindow get_by_name(string name)
        {
            for (var i = 0; i < wnd.Length; i++)
                if (wnd[i].identifier == name)
                    return wnd[i];
            return null;
        }

        /// <summary>
        /// Change the main window size.
        /// </summary>
        public static void set_size(Point xy, Point widthHeight)
        {
            Overlay.Position = xy;
            Overlay.Size = widthHeight;
        }
    }
}