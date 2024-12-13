namespace PixelLibrary.Core.ScreenCapture
{
    using SharpDX;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;

    public class GPUScreenCapture : IDisposable, IScreenCapture
    {
        private SharpDX.Direct3D11.Device _device;
        private DeviceContext _context;
        private OutputDuplication _outputDuplication;
        private Texture2D _stagingTexture;

        private readonly Rectangle _captureRectangle;
        private Bitmap _capturedImage;

        public GPUScreenCapture(Rectangle captureRectangle)
        {
            _captureRectangle = captureRectangle;

            // Create device and context
            var factory = new Factory1();
            var adapter = factory.GetAdapter1(0);

            // Create device with BGRA support for Desktop Duplication
            _device = new SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.None);
            _context = _device.ImmediateContext;

            using (var output = adapter.GetOutput(0))
            using (var output1 = output.QueryInterface<Output1>())
            {
                var desktopBounds = output.Description.DesktopBounds;
                var fullWidth = desktopBounds.Right - desktopBounds.Left;
                var fullHeight = desktopBounds.Bottom - desktopBounds.Top;

                // Validate that the rectangle is within screen bounds
                if (_captureRectangle.Right > fullWidth || _captureRectangle.Bottom > fullHeight)
                    throw new ArgumentException("The capture rectangle exceeds screen bounds.");

                // Create duplication
                _outputDuplication = output1.DuplicateOutput(_device);

                // Create a staging texture for the rectangle area
                var texDesc = new Texture2DDescription
                {
                    Width = _captureRectangle.Width,
                    Height = _captureRectangle.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Staging,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    OptionFlags = ResourceOptionFlags.None
                };

                _stagingTexture = new Texture2D(_device, texDesc);
                _capturedImage = new Bitmap(_captureRectangle.Width, _captureRectangle.Height, PixelFormat.Format32bppArgb);
            }
        }

        public Bitmap GetNextFrame()
        {
            // Acquire next frame
            var frameInfo = new OutputDuplicateFrameInformation();
            Texture2D desktopTexture = null;

            try
            {
                _outputDuplication.TryAcquireNextFrame(500, out frameInfo, out var resource);
                if (resource == null)
                    return null;

                desktopTexture = resource.QueryInterface<Texture2D>();

                // Define the region of the full desktop texture to copy
                var region = new ResourceRegion
                {
                    Left = _captureRectangle.Left,
                    Top = _captureRectangle.Top,
                    Front = 0,
                    Right = _captureRectangle.Right,
                    Bottom = _captureRectangle.Bottom,
                    Back = 1
                };

                // Copy from the full desktop texture into the staging texture
                _context.CopySubresourceRegion(
                    desktopTexture,
                    0,
                    region,
                    _stagingTexture,
                    0,
                    0,
                    0,
                    0
                );

                // Map the staging texture and copy pixels into a bitmap
                var dataBox = _context.MapSubresource(_stagingTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                try
                {
                    BitmapData bmpData = _capturedImage.LockBits(
                        new System.Drawing.Rectangle(0, 0, _captureRectangle.Width, _captureRectangle.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb
                    );

                    try
                    {
                        int bytesPerPixel = 4;
                        for (int y = 0; y < _captureRectangle.Height; y++)
                        {
                            IntPtr srcPtr = dataBox.DataPointer + y * dataBox.RowPitch;
                            IntPtr destPtr = bmpData.Scan0 + y * bmpData.Stride;
                            Utilities.CopyMemory(destPtr, srcPtr, _captureRectangle.Width * bytesPerPixel);
                        }
                    }
                    finally
                    {
                        _capturedImage.UnlockBits(bmpData);
                    }
                }
                finally
                {
                    _context.UnmapSubresource(_stagingTexture, 0);
                }

                // Return a clone of the captured image
                return (Bitmap)_capturedImage.Clone();
            }
            finally
            {
                if (desktopTexture != null) desktopTexture.Dispose();
                _outputDuplication.ReleaseFrame();
            }
        }

        public void Dispose()
        {
            if (_stagingTexture != null) _stagingTexture.Dispose();
            if (_outputDuplication != null) _outputDuplication.Dispose();
            if (_context != null) _context.Dispose();
            if (_device != null) _device.Dispose();
        }
    }
}
