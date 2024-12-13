using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{
    public class MouseBypass
    {
        private static MouseBypass _instance = new MouseBypass();
        private Driver driver;
        public MouseBypass()
        {
            _instance = this;
            driver = new Driver();
            driver.InitDriver();
        }


        public static void MouseEvent(int x, int y, int buttons)
        {
            _instance.driver.MouseEvent(x, y, buttons);
        }
    }
}
