
using PixelLibrary.Core;
using PixelLibrary.Core.ScreenCapture;
using PixelLibrary.Core.Settings;
using System.Numerics;

namespace PixelLibrary
{
    public class PixelSearcher
    {
        private int Monitor { get; set; }
        private Rectangle FovBox { get; set; }

        private Vector2 Center { get; set; }
        public static int FovX { get; set; }
        public static int FovY { get; set; }

        EnemyShapeManager enemyShapeManager = new EnemyShapeManager();

        public static Screen Screen;

        private IScreenCapture _screenCapture { get; set; }

        Benchmarker benchmarker;
        public EnemySorter enemySorter = new EnemySorter();

        private float targetBonePercent;

        private SettingsConfig _settings;

        public PixelSearcher(SettingsConfig _settings)
        {
            targetBonePercent = _settings.targetBonePercentage;

            benchmarker = new Benchmarker();

            Monitor = _settings.monitor;
            FovY = _settings.fovY;
            FovX = _settings.fovX;

            Init();
        }

        private void Init()
        {
            Screen = Screen.AllScreens[Monitor];

            Center = new Vector2(Screen.Bounds.Width / 2, Screen.Bounds.Height / 2);
            FovBox = new Rectangle((int)Center.X - (FovX / 2), (int)(Center.Y - (FovY / 2)), FovX, FovY); // Adjusted FovBox

            if (_settings.screenCaptureMethod == ScreenCaptureMethod.GPU)
                _screenCapture = new GPUScreenCapture(FovBox);

            if (_settings.screenCaptureMethod == ScreenCaptureMethod.CPU)
                _screenCapture = new CPUScreenCapture(FovBox);
        }

        public unsafe Core.EnemyShape[] PixelSearch()
        {
            Core.EnemyShape[] enemies = null;

            try
            {
                using (var bitmap = _screenCapture.GetNextFrame())
                {

                    enemies = enemyShapeManager.FindEnemiesWithPurple(bitmap);

                    return enemies;
                }
            }
            catch (Exception e)
            {
                // Handle or log the exception appropriately
                Console.WriteLine($"Exception in PixelSearch: {e.Message}");
            }
            finally
            {
                if (enemies != null)
                {
                    int offsetX = (int)(FovBox.X - Center.X);
                    int offsetY = (int)(FovBox.Y - Center.Y);

                    for (int i = 0; i < enemies.Length; i++)
                    {
                        // Apply offsets
                        enemies[i].ApplyOffset(offsetX, offsetY);

                        // Calculate the distance from the enemy's center to Vector2.Zero
                        enemies[i].Distance = Vector2.Distance(enemies[i].Center, Vector2.Zero);

                        // Calculate the AimTarget and AimDistance
                        float centerX = enemies[i].BoundingBox.X + enemies[i].BoundingBox.Width / 2f;
                        float centerY = enemies[i].BoundingBox.Y + (enemies[i].BoundingBox.Height * targetBonePercent);

                        enemies[i].AimTarget = new Vector2(centerX, centerY);
                        enemies[i].AimTargetDistance = Vector2.Distance(enemies[i].AimTarget, Vector2.Zero);
                    }
                }

            }

            return [];
        }
        public PixelSearchResult Search()
        {
            var enemies = PixelSearch();

            var result = new PixelSearchResult();
            if (enemies == null)
                enemies = new Core.EnemyShape[0];

            result.foundPlayer = enemies.Length > 0;
            result.enemies = enemies;

            return enemySorter.OptimizeEnemySorting(result, enemies.Length, Vector2.Zero);
        }
    }
}
