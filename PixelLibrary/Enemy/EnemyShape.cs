using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Enemy
{
    using Emgu.CV.Util;
    using System.Numerics;

    public struct EnemyShape
    {
        public int Id { get; set; }
        public float Confidence { get; set; } // Added Confidence property
        public Rectangle BoundingBox { get; set; } // The bounding box of the shape
        public Vector2 Center { get; set; }          // The center position of the shape
        public VectorOfPoint Contour { get; set; } // The contour of the shape
        public string ShapeType { get; set; }      // Type of the shape (optional, e.g., rectangle, circle)
        public double Area { get; set; }           // The area of the shape
        public float Distance { get; set; }
        public Vector2 AimTarget { get; set; }
        public float AimTargetDistance { get; set; }
        public string Label { get; set; }
        // Internal fields to track the applied offset
        private int offsetX;
        private int offsetY;

        /// <summary>
        /// Applies an offset to all position-related properties of the shape.
        /// </summary>
        /// <param name="offsetX">The X offset to apply.</param>
        /// <param name="offsetY">The Y offset to apply.</param>
        public void ApplyOffset(int offsetX, int offsetY)
        {
            // Store the applied offset
            this.offsetX += offsetX;
            this.offsetY += offsetY;

            // Offset the bounding box
            BoundingBox = new Rectangle(
                BoundingBox.X + offsetX,
                BoundingBox.Y + offsetY,
                BoundingBox.Width,
                BoundingBox.Height
            );

            // Offset the center
            Center = new Vector2(Center.X + offsetX, Center.Y + offsetY);
        }

        /// <summary>
        /// Removes the previously applied offset from the shape.
        /// </summary>
        public void RemoveOffset()
        {
            // Reverse the previously applied offset
            BoundingBox = new Rectangle(
                BoundingBox.X - offsetX,
                BoundingBox.Y - offsetY,
                BoundingBox.Width,
                BoundingBox.Height
            );

            Center = new Vector2(Center.X - offsetX, Center.Y - offsetY);

            // Reset offsets to zero
            offsetX = 0;
            offsetY = 0;
        }
    }


}
