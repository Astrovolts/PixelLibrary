using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core.Settings
{
    public struct SettingsConfig
    {
        public bool _ignoreDefaults;
        public int monitor; 
        public int fovX;
        public int fovY;
        /// <summary>
        /// Bone percentage, 0 being bottom of target, 100 being the top
        /// </summary>
        public float targetBonePercentage;
        public AimbotSettings aimbotSettings;
        public TriggerbotSettings triggerbotSettings;
        public ScreenCaptureMethod screenCaptureMethod;
        public bool ShowFPS;
    }

    public enum ScreenCaptureMethod 
    {
        GPU,
        CPU,
    }
}
