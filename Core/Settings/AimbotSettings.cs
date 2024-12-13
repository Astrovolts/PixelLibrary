using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core.Settings
{
    public struct AimbotSettings
    {
        public float gameFov;
        public float gameSensitivity;
        public bool enabled;
        public List<VirtualKeys> hotKeys;
    }
}
