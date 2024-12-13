using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{
    public class ClockwisePointComparer : IComparer<Vector2>
    {
        public int Compare(Vector2 v1, Vector2 v2)
        {
            if (v1.X >= 0)
            {
                if (v2.X < 0)
                {
                    return -1;
                }
                return -Comparer<float>.Default.Compare(v1.Y, v2.Y);
            }
            else
            {
                if (v2.X >= 0)
                {
                    return 1;
                }
                return Comparer<float>.Default.Compare(v1.Y, v2.Y);
            }
        }
    }
}
