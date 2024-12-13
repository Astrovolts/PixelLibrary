using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Numerics;
namespace PixelLibrary.Core
{

    public class EnemyShapeManager
    {
        // Debugging flag
        public bool Debugging { get; set; } = false;

        public EnemyShape[] FindEnemiesWithPurple(Bitmap bitmap)
        {
            // Convert Bitmap to Mat
            Mat image = bitmap.ToMat();

            Mat hsvImage = new Mat();
            Mat mask = new Mat();
            Mat blurredImage = new Mat();

            // Convert the image to HSV
            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

            // Define the range for purple color in HSV
            ScalarArray lowerPurple = new ScalarArray(new MCvScalar(130, 50, 50)); // Adjust as needed
            ScalarArray upperPurple = new ScalarArray(new MCvScalar(160, 255, 255)); // Adjust as needed

            // Create a mask for purple
            CvInvoke.InRange(hsvImage, lowerPurple, upperPurple, mask);

            // Enhance the purple outlines by dilating the mask
            Mat dilatedMask = new Mat();
            CvInvoke.Dilate(mask, dilatedMask, null, new Point(-1, -1), 2, BorderType.Default, new MCvScalar());

            // Apply Gaussian blur to the entire image
            CvInvoke.GaussianBlur(image, blurredImage, new Size(15, 15), 0);

            // Create inverted mask
            Mat invertedMask = new Mat();
            CvInvoke.BitwiseNot(dilatedMask, invertedMask);

            // Prepare result image with sharp purple regions and blurred background
            Mat result = new Mat();
            blurredImage.CopyTo(result); // Start with blurred image
            image.CopyTo(result, dilatedMask); // Overlay original image where purple is detected

            // Find contours on the dilated mask
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                Mat hierarchy = new Mat();
                CvInvoke.FindContours(dilatedMask, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

                List<EnemyShape> enemies = new List<EnemyShape>();

                for (int i = 0; i < contours.Size; i++)
                {
                    VectorOfPoint contour = contours[i];

                    // Compute bounding box
                    Rectangle boundingBox = CvInvoke.BoundingRectangle(contour);

                    // Compute center position
                    Point center = new Point(
                        boundingBox.X + boundingBox.Width / 2,
                        boundingBox.Y + boundingBox.Height / 2
                    );

                    // Compute area
                    double area = CvInvoke.ContourArea(contour);

                    if (area < 20)
                        continue; // Ignore small contours

                    // Add enemy data to the list
                    enemies.Add(new EnemyShape
                    {
                        BoundingBox = boundingBox,
                        Center = new Vector2(center.X, center.Y),
                        Contour = contour,
                        Area = area
                    });

                    // If debugging, draw the bounding box
                    if (Debugging)
                    {
                        // Draw the bounding box on the result image
                        CvInvoke.Rectangle(result, boundingBox, new MCvScalar(0, 255, 0), 2); // Green bounding box
                    }
                }

                // If debugging, display the image with bounding boxes
                if (Debugging)
                {
                    using (var debugBitmap = result.ToBitmap())
                    {
                        DebugForm.Instance.UpdatePicture(debugBitmap);
                    }
                }

                return enemies.ToArray();
            }
        }

    }
}