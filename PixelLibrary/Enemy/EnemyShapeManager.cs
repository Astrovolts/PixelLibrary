using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Numerics;

namespace PixelLibrary.Enemy
{
    public class EnemyShapeDetector
    {
        // Debugging flag
        public static bool Debugging { get; set; } = false;

        public EnemyShape[] FindEnemiesWithPurple(Bitmap bitmap)
        {
            // Convert Bitmap to Mat
            Mat image = bitmap.ToMat();

            // Convert the image to HSV and apply color filtering for purple
            Mat hsvImage = new Mat();
            Mat mask = new Mat();

            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

            ScalarArray lowerPurple = new ScalarArray(new MCvScalar(140, 50, 120));
            ScalarArray upperPurple = new ScalarArray(new MCvScalar(170, 255, 255));

            CvInvoke.InRange(hsvImage, lowerPurple, upperPurple, mask);

            // Blur and sharpen for better detection
            Mat blurred = new Mat();
            CvInvoke.GaussianBlur(image, blurred, new Size(1, 1), 2);
            Mat sharpened = new Mat();
            CvInvoke.AddWeighted(image, 3.5, blurred, -3.5, 0, sharpened);

            // Find contours
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                Mat hierarchy = new Mat();
                CvInvoke.FindContours(mask, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                List<EnemyShape> enemies = new List<EnemyShape>();
                List<Rectangle> boundingBoxes = new List<Rectangle>();

                for (int i = 0; i < contours.Size; i++)
                {
                    VectorOfPoint contour = contours[i];

                    // Compute bounding box and add to the list
                    Rectangle boundingBox = CvInvoke.BoundingRectangle(contour);
                    boundingBoxes.Add(boundingBox);

                    // Compute center and area
                    Point center = new Point(
                        boundingBox.X + boundingBox.Width / 2,
                        boundingBox.Y + boundingBox.Height / 2
                    );
                    double area = CvInvoke.ContourArea(contour);

                    // Add enemy data to the list
                    enemies.Add(new EnemyShape
                    {
                        BoundingBox = boundingBox,
                        Center = new Vector2(center.X, center.Y),
                        Contour = contour,
                        Area = area
                    });
                }

                // Combine overlapping/close bounding boxes
                List<Rectangle> mergedBoundingBoxes = MergeBoundingBoxes(boundingBoxes);

                // Remove nested bounding boxes
                mergedBoundingBoxes = RemoveNestedBoundingBoxes(mergedBoundingBoxes);

                // Debugging: Draw the merged bounding boxes
                if (Debugging)
                {
                    foreach (var mergedBox in mergedBoundingBoxes)
                    {
                        CvInvoke.Rectangle(image, mergedBox, new MCvScalar(255, 0, 0), 2); // Blue bounding box
                    }

                    using (var debugBitmap = image.ToBitmap())
                    {
                        DebugForm.Instance.UpdatePicture(debugBitmap);
                    }
                }

                return mergedBoundingBoxes.Select(bbox =>
                {
                    int highestY = int.MaxValue;
                    int highestX = bbox.X + bbox.Width / 2; // Default to bottom-center X

                    for (int i = 0; i < contours.Size; i++)
                    {
                        VectorOfPoint contour = contours[i];
                        Point[] points = contour.ToArray();

                        foreach (var point in points)
                        {
                            if (bbox.Contains(point))
                            {
                                if (point.Y < highestY)
                                {
                                    // Found a new highest point
                                    highestY = point.Y;
                                    highestX = point.X;
                                }
                                else if (point.Y == highestY)
                                {
                                    // Prioritize the leftmost point
                                    if (point.X < highestX) // Correct comparison for leftmost
                                    {
                                        highestX = point.X;
                                    }
                                }
                            }
                        }
                    }

                    // If no specific high point is found, default to bottom-center
                    if (highestY == int.MaxValue)
                    {
                        highestY = bbox.Y + bbox.Height;
                    }

                    return new EnemyShape
                    {
                        BoundingBox = bbox,
                        Center = new Vector2(highestX, highestY), // Adjust center to leftmost highest point
                        Contour = null, // Original contours are not combined
                        Area = bbox.Width * bbox.Height
                    };
                }).ToArray();





            }
        }

        /// <summary>
        /// Merges overlapping or nearby bounding boxes into a single box.
        /// </summary>
        private List<Rectangle> MergeBoundingBoxes(List<Rectangle> boundingBoxes)
        {
            List<Rectangle> merged = new List<Rectangle>();

            foreach (var box in boundingBoxes)
            {
                bool mergedWithExisting = false;

                for (int i = 0; i < merged.Count; i++)
                {
                    Rectangle existingBox = merged[i];

                    // Check for overlap or proximity
                    if (IsOverlappingOrClose(existingBox, box))
                    {
                        // Merge the boxes
                        merged[i] = Rectangle.Union(existingBox, box);
                        mergedWithExisting = true;
                        break;
                    }
                }

                if (!mergedWithExisting)
                {
                    merged.Add(box);
                }
            }

            return merged;
        }

        /// <summary>
        /// Determines if two rectangles are overlapping or close to each other.
        /// </summary>
        private bool IsOverlappingOrClose(Rectangle a, Rectangle b)
        {
            // Define a proximity threshold
            int proximityThreshold = 10;

            // Expand rectangles slightly to check for proximity
            Rectangle expandedA = new Rectangle(a.X - proximityThreshold, a.Y - proximityThreshold,
                a.Width + 2 * proximityThreshold, a.Height + 2 * proximityThreshold);

            return expandedA.IntersectsWith(b);
        }

        /// <summary>
        /// Removes nested rectangles from the list.
        /// A rectangle is considered nested if it is completely enclosed within another rectangle.
        /// </summary>
        private List<Rectangle> RemoveNestedBoundingBoxes(List<Rectangle> boundingBoxes)
        {
            List<Rectangle> filtered = new List<Rectangle>();

            for (int i = 0; i < boundingBoxes.Count; i++)
            {
                bool isNested = false;
                for (int j = 0; j < boundingBoxes.Count; j++)
                {
                    if (i != j && IsNested(boundingBoxes[i], boundingBoxes[j]))
                    {
                        isNested = true;
                        break;
                    }
                }
                if (!isNested)
                {
                    filtered.Add(boundingBoxes[i]);
                }
            }

            return filtered;
        }

        /// <summary>
        /// Checks if rectangle a is nested inside rectangle b.
        /// </summary>
        private bool IsNested(Rectangle a, Rectangle b)
        {
            return b.Contains(a);
        }

    }
}