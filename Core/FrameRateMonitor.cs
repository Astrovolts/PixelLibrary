using System.Diagnostics;


namespace PixelLibrary.Core
{
    using System;
    using System.Diagnostics;

    public class FrameRateMonitor
    {
        private readonly Stopwatch _stopwatch;
        private long _lastElapsedTicks;
        private float _smoothedFrameRate;
        private float _smoothedFrameTimeMs;
        private readonly float _smoothingFactor;

        public FrameRateMonitor(float smoothingFactor = 0.1f)
        {
            if (smoothingFactor <= 0 || smoothingFactor > 1)
                throw new ArgumentException("Smoothing factor must be in the range (0, 1].");

            _stopwatch = Stopwatch.StartNew();
            _lastElapsedTicks = 0;
            _smoothedFrameRate = 0f;
            _smoothedFrameTimeMs = 0f;
            _smoothingFactor = smoothingFactor;
        }

        /// <summary>
        /// Struct to hold average frame rate and frame time.
        /// </summary>
        public struct FrameRateData
        {
            public float AverageFPS { get; }
            public float AverageFrameTimeMs { get; }

            public FrameRateData(float averageFPS, float averageFrameTimeMs)
            {
                AverageFPS = averageFPS;
                AverageFrameTimeMs = averageFrameTimeMs;
            }
        }

        /// <summary>
        /// Call this method once per frame to calculate and return average FPS and ms/frame.
        /// </summary>
        /// <returns>A FrameRateData struct containing average FPS and average ms/frame.</returns>
        public FrameRateData Update()
        {
            long currentElapsedTicks = _stopwatch.ElapsedTicks;

            if (_lastElapsedTicks > 0)
            {
                long deltaTicks = currentElapsedTicks - _lastElapsedTicks;
                double deltaTimeSeconds = (double)deltaTicks / Stopwatch.Frequency;
                float deltaTimeMs = (float)(deltaTimeSeconds * 1000.0); // Convert to milliseconds

                if (deltaTimeSeconds > 0)
                {
                    float instantaneousFrameRate = (float)(1.0 / deltaTimeSeconds);

                    // Update smoothed frame rate
                    if (_smoothedFrameRate == 0)
                    {
                        // First frame initializes smoothing
                        _smoothedFrameRate = instantaneousFrameRate;
                        _smoothedFrameTimeMs = deltaTimeMs;
                    }
                    else
                    {
                        _smoothedFrameRate = (_smoothingFactor * instantaneousFrameRate) +
                                             ((1 - _smoothingFactor) * _smoothedFrameRate);

                        _smoothedFrameTimeMs = (_smoothingFactor * deltaTimeMs) +
                                               ((1 - _smoothingFactor) * _smoothedFrameTimeMs);
                    }
                }
            }

            _lastElapsedTicks = currentElapsedTicks;
            return new FrameRateData(_smoothedFrameRate, _smoothedFrameTimeMs);
        }
    }

}
