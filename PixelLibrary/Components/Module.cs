using PixelLibrary.Enemy;

namespace PixelLibrary.Components
{
    public abstract class Module : ModuleBase
    {
        public int id;

        public abstract void OnPlayerFound(EnemyShape player);
    }
}
