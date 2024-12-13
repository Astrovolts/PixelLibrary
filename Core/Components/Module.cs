using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PixelLibrary.Core.Components
{
    public abstract class Module : ModuleBase
    {
        public int id;

        public abstract void OnPlayerFound(EnemyShape player);
    }
}
