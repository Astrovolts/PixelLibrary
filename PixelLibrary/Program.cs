using PixelLibrary.Settings;
using PixelLibrary;
using System.Runtime.InteropServices;
using PixelLibrary.Components;
using PixelLibrary.Util;
class Program
{
    public void Start()
    {
        var settings = new SettingsConfig
        {
            targetBonePercentage = .21f,
            aimbotSettings = new AimbotSettings
            {
                hotKeys = new List<VirtualKeys> { VirtualKeys.RightButton, VirtualKeys.Control },
                enabled = true,
                gameSensitivity = 2f,
                gameFov = 103f,
            },
            triggerbotSettings = new TriggerbotSettings
            {
                hotKeys = new List<VirtualKeys> { VirtualKeys.RightButton },
                fireDelay = 100,
                enabled = true,
            },
            fovX = 640,
            fovY = 640,
            monitor = 2,
            ShowFPS = true,
            screenCaptureMethod = ScreenCaptureMethod.GPU,
        };

        using (var core = new PixelSystem(settings))
        {

            var aimbot = new Aimbot(settings.aimbotSettings);
            core.AddModule(aimbot);
            //var triggerbot = new Triggerbot(settings.triggerbotSettings);
            //core.AddModule(triggerbot);

            while (core.Enabled) 
            {
                core.Loop();
            }
        }
    }

    static void Main()
    {
        _handler += new EventHandler(Handler);
        SetConsoleCtrlHandler(_handler, true);

        //start your multi threaded program here
        Program p = new Program();
        p.Start();

        //hold the console so it doesn’t run off the end
        while (!exitSystem)
        {
            Thread.Sleep(500);
        }
    }

    static bool exitSystem = false;

    #region Trap application termination
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    private delegate bool EventHandler(CtrlType sig);
    static EventHandler _handler;

    enum CtrlType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }

    private static bool Handler(CtrlType sig)
    {
        Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

        //do your cleanup here
        Thread.Sleep(5000); //simulate some cleanup delay

        Console.WriteLine("Cleanup complete");

        //allow main to run off
        exitSystem = true;

        //shutdown right away so there are no lingering threads
        Environment.Exit(-1);

        return true;
    }
    #endregion
}