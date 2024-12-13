using PixelLibrary.Core.Components;
using PixelLibrary.Core.ScreenCapture;
using PixelLibrary.Core.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{
    public class Core : IDisposable
    {
        private List<Module> _modules = new List<Module>();
        public static SettingsConfig _settings;
        IScreenCapture screenCapture;
        PixelSearcher searcher;
        FrameRateMonitor frameRateMonitor;

        public bool Enabled = true;

        public Core(SettingsConfig settings) 
        {
            _settings = settings;

            searcher = new PixelSearcher(settings);

            frameRateMonitor = new FrameRateMonitor();
        }

        public void Loop() 
        {
            var result = searcher.Search();

            if (_settings.ShowFPS)
            {
                var frameRate = frameRateMonitor.Update();
                Console.Title = $"FPS: {frameRate.AverageFPS:F2} | Delta: {frameRate.AverageFrameTimeMs}";
            }

            for (int i = 0; i < _modules.Count; i++)
            {
                _modules[i].Loop();

                if (result.foundPlayer)
                    _modules[i].OnPlayerFound(result.enemies[0]);
            }
        }

        public void AddModule(Module module) 
        {
            module.id = _modules.Count;
            _modules.Add(module);
            module.OnEnable();
        }

        public void Dispose()
        {
            for (int i = 0; i < _modules.Count; i++) 
            {
                _modules[i].Dispose();
            }
        }
    }
}
