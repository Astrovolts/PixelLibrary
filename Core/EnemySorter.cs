using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{
    public class EnemySorter
    {
        // Custom comparer for sorting by squared distance and Y value
        private class EnemyCompositeComparer : IComparer<EnemyShape>
        {
            private readonly Vector2 _center;

            public EnemyCompositeComparer(Vector2 center)
            {
                _center = center;
            }

            public int Compare(EnemyShape enemy1, EnemyShape enemy2)
            {
                float dx1 = enemy1.Center.X - _center.X;
                float dy1 = enemy1.Center.Y - _center.Y;
                float dx2 = enemy2.Center.X - _center.X;
                float dy2 = enemy2.Center.Y - _center.Y;

                // Compare squared distances
                float distanceSquared1 = dx1 * dx1 + dy1 * dy1;
                float distanceSquared2 = dx2 * dx2 + dy2 * dy2;
                int distanceComparison = distanceSquared1.CompareTo(distanceSquared2);

                if (distanceComparison != 0)
                {
                    return distanceComparison;
                }

                // If distances are equal, compare by Y value (ascending)
                return enemy1.Center.Y.CompareTo(enemy2.Center.Y);
            }
        }

        public PixelSearchResult OptimizeEnemySorting(PixelSearchResult result, int enemyCount, Vector2 center)
        {
            if (result.enemies == null || result.enemies.Length <= 1 || enemyCount <= 0)
                return result;

            // Sort enemies by composite comparer (distance and Y value)
            Array.Sort(result.enemies, new EnemyCompositeComparer(center));

            // Resize array to keep only the closest `enemyCount` enemies
            if (enemyCount < result.enemies.Length)
            {
                Array.Resize(ref result.enemies, enemyCount);
            }

            return result;
        }
    }

}
