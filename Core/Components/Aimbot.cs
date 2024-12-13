using PixelLibrary.Core.Components;
using PixelLibrary.Core.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PixelLibrary.Core
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
            _timeDeltaMs = _benchmarker.Stop();
            _benchmarker.Start();
        }

        public override void OnPlayerFound(EnemyShape player)
        {
            foreach (var key in _settings.hotKeys)
            { 
                if ((Native.GetAsyncKeyState((ushort)key) != 0))
                {
                    

                    if (player.Distance > 5)
                    {
                        var move = CalculateMouseMovement(player.AimTarget);
                        //Console.WriteLine($"{player.headLocation} / {move}");
                        move = GetShutterVector(move);

                        //move *= (_timeDeltaMs * .1f);

                        MouseBypass.MouseEvent((int)move.X, (int)move.Y, 0);
                    }

                }
            }
        }

        /// <summary>
        /// Calculates the mouse movement in dots required to move the crosshair to a target location on the screen.
        /// </summary>
        /// <param name="targetLocation">Target location as a Vector2 (X, Y) in screen space (e.g., pixels or normalized [0-1]).</param>
        /// <param name="currentLocation">Current location of the crosshair as a Vector2 (X, Y) in the same space as the targetLocation.</param>
        /// <returns>Required mouse movement in dots as a Vector2 (X, Y).</returns>
        /// <summary>
        /// Calculates the mouse movement in dots required to move the crosshair to a target location relative to the center of the screen.
        /// </summary>
        /// <param name="targetLocation">
        /// Target location as a Vector2 (X, Y), representing the offset from the center of the screen.
        /// Positive X moves to the right, negative X to the left.
        /// Positive Y moves upwards, negative Y downwards.
        /// </param>
        /// <returns>Required mouse movement in dots as a Vector2 (X, Y).</returns>
        Vector2 CalculateMouseMovement(Vector2 targetLocation)
        {
            // Precompute values if they are constant
            double aspectRatio = screenHeight / (double)screenWidth;
            double anglePerPixelX = _settings.gameFov / screenWidth; // Horizontal angle per pixel
            double anglePerPixelY = (_settings.gameFov * aspectRatio) / screenHeight; // Vertical angle per pixel

            // Smoothing parameters
            float smoothingFactor = 10.0f; // Adjust for the desired smoothness

            // Calculate the angular rotation required for the given pixel offset (targetLocation - currentLocation)
            Vector2 offset = targetLocation - Vector2.Zero;
            double requiredAngleX = anglePerPixelX * offset.X;
            double requiredAngleY = anglePerPixelY * offset.Y;

            // Calculate desired mouse movement based on sensitivity
            double desiredMovementX = requiredAngleX / _settings.gameSensitivity * _timeDeltaMs;
            double desiredMovementY = requiredAngleY / _settings.gameSensitivity * _timeDeltaMs;

            // Smoothly interpolate the movement
            float smoothMovementX = (float)(desiredMovementX * smoothingFactor);
            float smoothMovementY = (float)(desiredMovementY * smoothingFactor);

            // Prevent overshooting by clamping the movement to the remaining offset
            smoothMovementX = Math.Abs(smoothMovementX) > Math.Abs(offset.X) ? (float)offset.X : smoothMovementX;
            smoothMovementY = Math.Abs(smoothMovementY) > Math.Abs(offset.Y) ? (float)offset.Y : smoothMovementY;

            return new Vector2(smoothMovementX, smoothMovementY);
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
                if ((DateTime.Now - _lastDirectionTimeX).TotalMilliseconds > 50)
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
                if ((DateTime.Now - _lastDirectionTimeY).TotalMilliseconds > 50)
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
