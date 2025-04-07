using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Util
{
    public static class MouseFeature
    {
        private static readonly Random _random = new Random();

        public static Vector2 GetFlickShot(Vector2 target, float sensitivity) 
        {
            var sens = (float)CalculateValSensitivity(sensitivity);

            target.X *= sens;
            target.Y *= sens;

            return target;

        }

        public static Vector2 GetSmoothMovement(Vector2 target, float sensitivity)
        {
            if (Math.Abs(target.X) < 1) target.X = 0;
            if (Math.Abs(target.Y) < 1) target.Y = 0;

            var sens =(float) CalculateValSensitivity(sensitivity);

            target.X *= sens;
            target.Y *= sens;

            return target;

        }
        /// <summary>
        /// Calculates Valorant sensitivity based on a given input sensitivity using a specific formula.
        /// </summary>
        /// <param name="sensitivity">The input sensitivity to convert.</param>
        /// <returns>The calculated Valorant sensitivity.</returns>
        public static double CalculateValSensitivity(float sensitivity)
        {
            // Coefficients might need adjustment based on specific conversion needs or empirical testing.
            const double coefficient = 1.07437623;
            const double exponent = -0.9936827126;

            return coefficient * Math.Pow(sensitivity, exponent);
        }
        private static float GetPoint(float start, float end, float percentage)
        {
            return start + (end - start) * percentage;
        }

        private static int GetRandom(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static Vector2 MoveGaussian(Vector2 target)
        {
            var initial = new Vector2(0, 0);
            var lastMove = new Vector2(0, 0);
            var final = new Vector2(target.X, target.Y);

            int randomFactor1 = GetRandom(4, 8);
            int randomFactor2 = GetRandom(3, 6);

            var midPoint1 = new Vector2(
                initial.X + (final.X - initial.X) / randomFactor1,
                initial.Y + (final.Y >= initial.Y ? final.X - initial.X : -(final.X - initial.X)) / randomFactor1
            );

            var midPoint2 = new Vector2(
                initial.X + (final.X - initial.X) / randomFactor2,
                initial.Y + (final.Y >= initial.Y ? final.X - initial.X : -(final.X - initial.X)) / randomFactor2
            );

            // Adjust the loop to return only after all calculations if necessary.
            Vector2 result = new Vector2();
            for (float i = 0; i <= 1.01; i += 0.01f)
            {
                float xa = GetPoint(initial.X, midPoint1.X, i);
                float ya = GetPoint(initial.Y, midPoint1.Y, i);
                float xb = GetPoint(midPoint1.X, midPoint2.X, i);
                float yb = GetPoint(midPoint1.Y, midPoint2.Y, i);
                float xc = GetPoint(midPoint2.X, final.X, i);
                float yc = GetPoint(midPoint2.Y, final.Y, i);

                float xm = GetPoint(xa, xb, i);
                float ym = GetPoint(ya, yb, i);
                float xn = GetPoint(xb, xc, i);
                float yn = GetPoint(yb, yc, i);

                float x = GetPoint(xm, xn, i);
                float y = GetPoint(ym, yn, i);

                var move = new Vector2(x - lastMove.X, y - lastMove.Y); // Relative movement
                lastMove = new Vector2(x, y);
                result = move;
            }

            // Assuming that the loop should return the final calculated vector.
            return result;
        }
    }

}
