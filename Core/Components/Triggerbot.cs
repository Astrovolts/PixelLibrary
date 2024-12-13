using PixelLibrary.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PixelLibrary.Core.Components
{
    public class Triggerbot : Module
    {
        private TriggerbotSettings _settings;
        public Triggerbot(TriggerbotSettings settings)
        {
            _settings = settings;
        }

        DateTime _lastShot;


        DateTime _timeSinceAds;
        public bool IsADS()
        {
            
            bool ads = Native.GetAsyncKeyState((ushort)VirtualKeys.RightButton) != 0;
            if (ads)
                _timeSinceAds = DateTime.Now;
            return ads;
        }

        public bool IsCrouch()
        {
            return Native.GetAsyncKeyState((ushort)VirtualKeys.LeftControl) != 0;

        }

        bool antiR = false;
        DateTime timeSinceStart = DateTime.Now;
        public override void OnPlayerFound(EnemyShape player)
        {
            foreach (var key in _settings.hotKeys)
            {
                if ((Native.GetAsyncKeyState((ushort)key) != 0))
                {
                    if (player.AimTargetDistance < 10)
                    {
                        MouseBypass.MouseEvent(0, 0, 1);
                        MouseBypass.MouseEvent(0, 0, 2);
                        MouseBypass.MouseEvent(0, 0, 0);

                        _lastShot = DateTime.Now;
                    }

                    break;
                }
            }
        }
    }
}
