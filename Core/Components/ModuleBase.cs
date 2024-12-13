using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelLibrary.Core.Components
{
    public abstract class ModuleBase
    {
        public virtual void OnEnable() { }
        public virtual void Loop() { }

        public virtual void Dispose() { }
    }
}
