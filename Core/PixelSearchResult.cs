using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{

    public enum EnemyDistance 
    {
        Close,
        Medium,
        Far,
        VeryFar,
    }
    public struct PixelSearchResult
    {
        public bool foundPlayer;
        public EnemyShape[] enemies;
    }
}
