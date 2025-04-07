using PixelLibrary.Enemy;
using PixelLibrary.Processing;
using PixelLibrary.Settings;
using PixelLibrary.Util;
using System.Numerics;

namespace PixelLibrary.Components
{
    public class Aimbot : Module
    {
        AimbotSettings _settings;
        private int screenWidth;
        private int screenHeight;   
        public Aimbot(AimbotSettings settings)
        {
            _settings = settings;
            screenHeight = PixelSearcher.Screen.Bounds.Height;
            screenWidth = PixelSearcher.Screen.Bounds.Width;
        }

        public bool IsADS()
        {
            return Native.GetAsyncKeyState((ushort)VirtualKeys.RightButton) != 0;

        }

        public bool IsShooting()
        {
            return Native.GetAsyncKeyState((ushort)VirtualKeys.LeftButton) != 0;

        }

        public bool IsCrouch()
        {
            return Native.GetAsyncKeyState((ushort)VirtualKeys.LeftControl) != 0;

        }

        public bool IsWalk()
        {
            return Native.GetAsyncKeyState((ushort)VirtualKeys.LeftShift) != 0;

        }

        Benchmarker _benchmarker = new Benchmarker(showLogs:false);
        private float _timeDeltaMs;
        public override void Loop()
        {
            _timeDeltaMs = _benchmarker.Stop() / 400;
            _benchmarker.Start();
        }

        public override void OnPlayerFound(EnemyShape player)
        {
            foreach (var key in _settings.hotKeys)
            { 
                if (Native.GetAsyncKeyState((ushort)key) != 0)
                {
                    Vector2 move = MouseFeature.GetSmoothMovement(player.AimTarget, _settings.gameSensitivity);
                    move *= _timeDeltaMs;

                    move = GetShutterVector(move);
                    if (move.X > 1 || move.X < -1 || move.Y > 1 || move.Y < -1)
                        MouseBypass.MouseEvent((int)move.X, (int)move.Y);
                }
            }
        }

        public DateTime lastDecision = DateTime.Now;
        bool _targetHead;
        private Random random = new Random();
        public bool TargetHead()
        {
            if ((DateTime.Now - lastDecision).TotalSeconds > 1)
            {
                _targetHead = random.Next(0, 100) > 70;
                lastDecision = DateTime.Now;
            }

            return _targetHead;
        }

        Vector2 GetShutterVector(Vector2 move) 
        {
            if (IsShutteringX(move))
                move.X = 0;
            if (IsShutteringY(move))
                move.Y = 0;

            return move;
        }

        Vector2 _lastDirectionX;
        DateTime _lastDirectionTimeX;
        Vector2 _lastDirectionY;
        DateTime _lastDirectionTimeY;
        public bool IsShutteringX(Vector2 newMove)
        {
            if (_lastDirectionX.X < 0 && newMove.X > 0 || _lastDirectionX.X > 0 && newMove.X < 0)
            {
                if ((DateTime.Now - _lastDirectionTimeX).TotalMilliseconds > 125)
                {
                    return false;
                }
                return true;
            }
            else
            {
                _lastDirectionX = newMove;
                _lastDirectionTimeX = DateTime.Now;

                return false;
            }
        }

        public bool IsShutteringY(Vector2 newMove)
        {
            if (_lastDirectionY.Y < 0 && newMove.Y > 0 || _lastDirectionY.Y > 0 && newMove.Y < 0)
            {
                if ((DateTime.Now - _lastDirectionTimeY).TotalMilliseconds > 125)
                {
                    return false;
                }
                return true;
            }
            else
            {
                _lastDirectionY = newMove;
                _lastDirectionTimeY = DateTime.Now;

                return false;
            }
        }
    }
}
