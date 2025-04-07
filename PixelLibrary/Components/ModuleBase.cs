namespace PixelLibrary.Components
{
    public abstract class ModuleBase
    {
        public virtual void OnEnable() { }
        public virtual void Loop() { }

        public virtual void Dispose() { }
    }
}
