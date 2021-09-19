using System;
using System.Numerics;
using Coroutine;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace imClickable
{
    /// <summary>
    /// A class to create clickable transparent overlay.
    /// </summary>
    public static class Overlay
    {
        private static readonly Sdl2Window window;
        private static readonly GraphicsDevice graphicsDevice;
        private static readonly CommandList commandList;
        private static readonly ImGuiController imController;
        private static readonly Vector4 clearColor;
        private static bool terminal = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Overlay"/> class.
        /// </summary>
        static Overlay()
        {
            clearColor = Vector4.Zero;
            window = new Sdl2Window("Overlay", 0, 0, 2560, 1440,
                SDL_WindowFlags.Borderless | SDL_WindowFlags.AlwaysOnTop | SDL_WindowFlags.SkipTaskbar, false);
            graphicsDevice =
                VeldridStartup.CreateDefaultD3D11GraphicsDevice(new GraphicsDeviceOptions(false, null, true), window);
            commandList = graphicsDevice.ResourceFactory.CreateCommandList();
            imController = new ImGuiController(graphicsDevice,
                graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height);
            window.Resized += () =>
            {
                graphicsDevice.MainSwapchain.Resize((uint) window.Width, (uint) window.Height);
                imController.WindowResized(window.Width, window.Height);
            };

            NativeMethods.InitTransparency(window.Handle);
            NativeMethods.SetOverlayClickable(window.Handle, false);
        }

        /// <summary>
        /// Infinitely renders the over (and execute co-routines) till it's closed.
        /// </summary>
        public static void RunInfiniteLoop()
        {
            var previous = DateTime.Now;
            while (window.Exists && !Close)
            {
                var snapshot = window.PumpEvents();
                if (!window.Exists) break;
                var current = DateTime.Now;
                var interval = current - previous;
                var sec = (float) interval.TotalSeconds;
                previous = current;
                imController.Update(sec > 0 ? sec : .001f, snapshot, window.Handle);
                CoroutineHandler.Tick(interval.TotalSeconds);
                if (Visible) CoroutineHandler.RaiseEvent(OnRender);
                commandList.Begin();
                commandList.SetFramebuffer(graphicsDevice.MainSwapchain.Framebuffer);
                commandList.ClearColorTarget(0, new RgbaFloat(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W));
                imController.Render(graphicsDevice, commandList);
                commandList.End();
                graphicsDevice.SubmitCommands(commandList);
                graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);
            }

            Dispose();
        }

        /// <summary>
        /// To submit ImGui code for generating the UI.
        /// </summary>
        public static readonly Event OnRender = new();

        /// <summary>
        /// Safely Closes the Overlay.
        /// Doesn't matter if you set it to true multiple times.
        /// </summary>
        public static bool Close { get; set; }

        /// <summary>
        /// Makes the overlay visible or invisible. Invisible Overlay
        /// will not call OnRender coroutines, however time based
        /// coroutines are still called.
        /// </summary>
        private static bool Visible => true;

        /// <summary>
        /// Gets or sets a value indicating whether to hide the terminal window.
        /// </summary>
        public static bool TerminalWindow
        {
            get => terminal;
            set
            {
                if (value != terminal) NativeMethods.SetConsoleWindow(value);
                terminal = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the overlay window.
        /// </summary>
        public static Point Position
        {
            set => Sdl2Native.SDL_SetWindowPosition(window.SdlWindowHandle, value.X, value.Y);
        }

        /// <summary>
        /// Gets or sets the size of the overlay window.
        /// </summary>
        public static Point Size
        {
            set => Sdl2Native.SDL_SetWindowSize(window.SdlWindowHandle, value.X, value.Y);
        }

        /// <summary>
        /// Free all resources acquired by the overlay.
        /// </summary>
        private static void Dispose()
        {
            window.Close();
            graphicsDevice.WaitForIdle();
            imController.Dispose();
            commandList.Dispose();
            graphicsDevice.WaitForIdle();
            graphicsDevice.Dispose();
            NativeMethods.SetConsoleWindow(true);
            Environment.Exit(0);
        }
    }
}