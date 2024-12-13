using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PixelLibrary.Core.ScreenCapture
{
    public class CPUScreenCapture : IDisposable, IScreenCapture
    {
        private bool _disposed;
        private IntPtr _hdcScreen; // Persistent screen DC
        private IntPtr _hdcMem; // Persistent memory DC
        private IntPtr _hBitmap; // Persistent compatible bitmap handle
        private Rectangle _captureRectangle;
        private Bitmap _capturedImage;

        public CPUScreenCapture(Rectangle captureRectangle)
        {
            _captureRectangle = captureRectangle;

            // Initialize resources
            _capturedImage = new Bitmap(_captureRectangle.Width, _captureRectangle.Height);

            _hdcScreen = GetDC(IntPtr.Zero);
            _hdcMem = CreateCompatibleDC(_hdcScreen);
            _hBitmap = CreateCompatibleBitmap(_hdcScreen, _captureRectangle.Width, _captureRectangle.Height);
            SelectObject(_hdcMem, _hBitmap);
        }

        public Bitmap GetNextFrame()
        {
            // Capture the specified rectangle of the screen into the memory DC
            bool bltSuccess = BitBlt(
                _hdcMem,
                0,
                0,
                _captureRectangle.Width,
                _captureRectangle.Height,
                _hdcScreen,
                _captureRectangle.Left,
                _captureRectangle.Top,
                CopyPixelOperation.SourceCopy);

            if (!bltSuccess)
            {
                throw new InvalidOperationException("BitBlt failed during screen capture.");
            }

            // Copy from memory DC to the bitmap
            using (Graphics g = Graphics.FromImage(_capturedImage))
            {
                IntPtr hdcDest = g.GetHdc();
                try
                {
                    bltSuccess = BitBlt(
                        hdcDest,
                        0,
                        0,
                        _captureRectangle.Width,
                        _captureRectangle.Height,
                        _hdcMem,
                        0,
                        0,
                        CopyPixelOperation.SourceCopy);

                    if (!bltSuccess)
                    {
                        throw new InvalidOperationException("BitBlt failed while copying to the bitmap.");
                    }
                }
                finally
                {
                    g.ReleaseHdc(hdcDest);
                }
            }

            // Return a clone of the captured image to ensure thread safety
            return (Bitmap)_capturedImage.Clone();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _capturedImage.Dispose();
                DeleteObject(_hBitmap);
                DeleteDC(_hdcMem);
                ReleaseDC(IntPtr.Zero, _hdcScreen);

                _disposed = true;
            }
        }

        #region WinAPI Functions

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation rop);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #endregion
    }
}
