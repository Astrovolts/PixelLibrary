using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.ScreenCapture
{
    public interface IScreenCapture
    {
        Bitmap GetNextFrame();
    }
}
