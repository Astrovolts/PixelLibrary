using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{
    public class Benchmarker
    {
        private Stopwatch _stopwatch;
        private bool _isRunning;
        private bool _showLogs = false;
        public Benchmarker(bool showLogs = true)
        {
            _showLogs = showLogs;
            _stopwatch = new Stopwatch();
            _isRunning = false;
        }

        /// <summary>
        /// Starts the benchmark timer.
        /// </summary>
        public void Start()
        {
            if (_isRunning && _showLogs)
            {
                Console.WriteLine("Benchmarker is already running.");
                return;
            }

            _stopwatch.Start();
            _isRunning = true;

            if (_showLogs)
            Console.WriteLine("Benchmarking started.");
        }

        /// <summary>
        /// Stops the benchmark timer and prints the elapsed time in milliseconds.
        /// </summary>
        public float Stop()
        {
            if (!_isRunning)
            {
                if (_showLogs)
                Console.WriteLine("Benchmarker is not running.");

                return 0;
            }

            _stopwatch.Stop();
            _isRunning = false;

            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (_showLogs)
            Console.WriteLine($"Benchmarking stopped. Elapsed time: {elapsedMilliseconds} ms.");

            // Reset the stopwatch for potential reuse
            _stopwatch.Reset();

            return elapsedMilliseconds;
        }

        /// <summary>
        /// Gets the elapsed time in milliseconds without stopping the timer.
        /// </summary>
        /// <returns>Elapsed time in milliseconds.</returns>
        public long GetElapsedMilliseconds()
        {
            return _stopwatch.ElapsedMilliseconds;
        }
    }
}
