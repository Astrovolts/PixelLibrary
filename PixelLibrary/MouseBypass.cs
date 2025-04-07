using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary
{
    public class MouseBypass
    {
        private static MouseBypass _instance = new MouseBypass();
        public MouseBypass()
        {
            _instance = this;
        }

        public static void MouseEvent(int x, int y)
        {
            //_instance.driver.MouseEvent(x, y);
        }
        public static void MouseEvent(int x, int y, int buttons)
        {
            //_instance.driver.MouseEvent(x, y, buttons);
        }
    }
}
