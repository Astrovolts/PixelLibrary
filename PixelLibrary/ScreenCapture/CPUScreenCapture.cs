using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PixelLibrary.ScreenCapture
{
    public class CPUScreenCapture : IDisposable, IScreenCapture
    {
        private bool _disposed;
        private nint _hdcScreen; // Persistent screen DC
        private nint _hdcMem; // Persistent memory DC
        private nint _hBitmap; // Persistent compatible bitmap handle
        private Rectangle _captureRectangle;
        private Bitmap _capturedImage;

        public CPUScreenCapture(Rectangle captureRectangle)
        {
            _captureRectangle = captureRectangle;

            // Initialize resources
            _capturedImage = new Bitmap(_captureRectangle.Width, _captureRectangle.Height);

            _hdcScreen = GetDC(nint.Zero);
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
                nint hdcDest = g.GetHdc();
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
                ReleaseDC(nint.Zero, _hdcScreen);

                _disposed = true;
            }
        }

        #region WinAPI Functions

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern bool BitBlt(nint hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            nint hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation rop);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern nint CreateCompatibleDC(nint hdc);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern nint CreateCompatibleBitmap(nint hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern nint SelectObject(nint hdc, nint hgdiobj);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern bool DeleteObject(nint hObject);

        [DllImport("gdi32.dll", SetLastError = false)]
        private static extern bool DeleteDC(nint hdc);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern nint GetDC(nint hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern int ReleaseDC(nint hWnd, nint hDC);

        #endregion
    }
}
