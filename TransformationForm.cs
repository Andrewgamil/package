using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace package
{
    public partial class TransformationForm : Form
    {
        private Bitmap originalImage;
        private Bitmap transformedImage = new Bitmap(1, 1); // Example initialization

        public TransformationForm()
        {
            InitializeComponent();
            // Load the original image
            originalImage = new Bitmap("C:\\Users\\andre\\Downloads\\sunflower.jpg");
            pictureBox.Image = originalImage;
        }

        ///////// Translation ///////////////////////////////////////////////////////////////////
        private Bitmap TranslateImage(Bitmap image, int tx, int ty)
        {
            // Create a new bitmap for the transformed image
            Bitmap transformedBitmap = new Bitmap(image.Width, image.Height);

            // Loop through each pixel of the original image
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Translate each pixel
                    int newX = x + tx;
                    int newY = y + ty;

                    // Ensure the new coordinates are within the image bounds
                    if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                    {
                        // Set the pixel color in the transformed image
                        transformedBitmap.SetPixel(newX, newY, image.GetPixel(x, y));
                    }
                }
            }

            return transformedBitmap;
        }
        private void btnTranslate_Click(object sender, EventArgs e)
        {
            if (originalImage != null && ValidateTranslationInput())
            {
                // Get the translation values from the textboxes
                int tx, ty;
                if (int.TryParse(txtTranslateX.Text, out tx) && int.TryParse(txtTranslateY.Text, out ty))
                {
                    // Perform translation
                    transformedImage = TranslateImage(originalImage, tx, ty);

                    // Display the transformed image
                    pictureBox.Image = transformedImage;
                }
                else
                {
                    MessageBox.Show("Invalid translation values. Please enter integers.");
                }
            }
        }

        ///////// End Of Translation ///////////////////////////////////////////////////////////////////

        ///////// Scaling ///////////////////////////////////////////////////////////////////
        private Bitmap ScaleImage(Bitmap image, float sx, float sy)
        {
            // Create a new bitmap for the transformed image
            Bitmap transformedBitmap = new Bitmap(image.Width, image.Height);

            // Loop through each pixel of the original image
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Calculate the corresponding coordinates in the transformed image
                    int newX = (int)(x * sx);
                    int newY = (int)(y * sy);

                    // Ensure the new coordinates are within the bounds of the transformed image
                    if (newX >= 0 && newX < transformedBitmap.Width && newY >= 0 && newY < transformedBitmap.Height)
                    {
                        // Set the pixel color in the transformed image
                        transformedBitmap.SetPixel(newX, newY, image.GetPixel(x, y));
                    }
                }
            }

            return transformedBitmap;
        }
        private void btnScale_Click(object sender, EventArgs e)
        {
            if (originalImage != null && ValidateScaleInput())
            {
                // Get the scaling factors from the textboxes
                float sx, sy;
                if (float.TryParse(txtScaleX.Text, out sx) && float.TryParse(txtScaleY.Text, out sy))
                {
                    // Perform scaling
                    transformedImage = ScaleImage(originalImage, sx, sy);

                    // Display the transformed image
                    pictureBox.Image = transformedImage;
                }
                else
                {
                    MessageBox.Show("Invalid scaling values. Please enter numeric values.");
                }
            }
        }

        ///////// End Of Scaling  ///////////////////////////////////////////////////////////////////

        ///////// Rotatation ///////////////////////////////////////////////////////////////////
        private Bitmap RotateImage(Bitmap image, float angle)
        {
            // Calculate the rotation angle in radians
            float angleRad = (float)(angle * Math.PI / 180.0);

            // Calculate the center of the image
            float centerX = image.Width / 2.0f;
            float centerY = image.Height / 2.0f;

            // Calculate the bounding box of the rotated image
            float cosTheta = (float)Math.Abs(Math.Cos(angleRad));
            float sinTheta = (float)Math.Abs(Math.Sin(angleRad));
            int newWidth = (int)(image.Width * cosTheta + image.Height * sinTheta);
            int newHeight = (int)(image.Width * sinTheta + image.Height * cosTheta);

            // Create a new bitmap for the rotated image
            Bitmap rotatedBitmap = new Bitmap(newWidth, newHeight);

            // Loop through each pixel of the rotated image
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    // Calculate the coordinates relative to the center
                    float newX = (x - newWidth / 2.0f) * (float)Math.Cos(angleRad) - (y - newHeight / 2.0f) * (float)Math.Sin(angleRad) + centerX;
                    float newY = (x - newWidth / 2.0f) * (float)Math.Sin(angleRad) + (y - newHeight / 2.0f) * (float)Math.Cos(angleRad) + centerY;

                    // Check if the calculated coordinates are within the bounds of the original image
                    if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                    {
                        // Perform bilinear interpolation to get the color of the rotated pixel
                        Color color = BilinearInterpolation(image, newX, newY);

                        // Set the pixel color in the rotated image
                        rotatedBitmap.SetPixel(x, y, color);
                    }
                }
            }

            return rotatedBitmap;
        }
        private void btnRotate_Click(object sender, EventArgs e)
        {
            if (originalImage != null && ValidateRotationInput())
            {
                // Get the rotation angle from the textbox
                float angle;
                if (float.TryParse(txtRotationAngle.Text, out angle))
                {
                    // Perform rotation
                    transformedImage = RotateImage(originalImage, angle);

                    // Display the transformed image
                    pictureBox.Image = transformedImage;
                }
                else
                {
                    MessageBox.Show("Invalid rotation angle. Please enter a numeric value.");
                }
            }
        }

        ///////// End Of Rotatation ///////////////////////////////////////////////////////////////////

        ///////// Reflectation ///////////////////////////////////////////////////////////////////
        private Bitmap ReflectImage(Bitmap image, int reflectionX, int reflectionY)
            {
                // Create a new bitmap for the transformed image
                Bitmap transformedBitmap = new Bitmap(image.Width, image.Height);

                // Loop through each pixel of the transformed image
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // Calculate the reflected coordinates
                        int newX = x;
                        int newY = y;

                        if (reflectionX == 1)
                        {
                            newX = image.Width - x - 1;
                        }

                        if (reflectionY == 1)
                        {
                            newY = image.Height - y - 1;
                        }

                        // Set the pixel color in the transformed image
                        transformedBitmap.SetPixel(newX, newY, image.GetPixel(x, y));
                    }
                }

                return transformedBitmap;
            }
        private void btnReflect_Click(object sender, EventArgs e)
        {
            if (originalImage != null && ValidateReflectionInput())
            {
                // Get the reflection axis coordinates from the textboxes
                int reflectionX, reflectionY;
                if (int.TryParse(txtReflectionX.Text, out reflectionX) && int.TryParse(txtReflectionY.Text, out reflectionY))
                {
                    // Perform reflection
                    transformedImage = ReflectImage(originalImage, reflectionX, reflectionY);

                    // Display the transformed image
                    pictureBox.Image = transformedImage;
                }
                else
                {
                    MessageBox.Show("Invalid reflection values. Please enter integers (0 or 1).");
                }
            }
        }
        private Color BilinearInterpolation(Bitmap image, float x, float y)
        {
            int x1 = (int)Math.Floor(x);
            int x2 = (int)Math.Ceiling(x);
            int y1 = (int)Math.Floor(y);
            int y2 = (int)Math.Ceiling(y);

            if (x2 >= image.Width) x2 = image.Width - 1;
            if (y2 >= image.Height) y2 = image.Height - 1;

            Color c00 = image.GetPixel(x1, y1);
            Color c01 = image.GetPixel(x1, y2);
            Color c10 = image.GetPixel(x2, y1);
            Color c11 = image.GetPixel(x2, y2);

            float fx = x - x1;
            float fy = y - y1;

            float r = (1 - fx) * (1 - fy) * c00.R + fx * (1 - fy) * c10.R + (1 - fx) * fy * c01.R + fx * fy * c11.R;
            float g = (1 - fx) * (1 - fy) * c00.G + fx * (1 - fy) * c10.G + (1 - fx) * fy * c01.G + fx * fy * c11.G;
            float b = (1 - fx) * (1 - fy) * c00.B + fx * (1 - fy) * c10.B + (1 - fx) * fy * c01.B + fx * fy * c11.B;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        ///////// End Of Reflectation ///////////////////////////////////////////////////////////////////

        ///////// Shearing ///////////////////////////////////////////////////////////////////
        private Bitmap ShearImage(Bitmap image, float shearX, float shearY)
        {
 
                int width = image.Width;
                int height = image.Height;
                Bitmap transformedBitmap = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(transformedBitmap))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    PointF[] destPoints = { new PointF(0, 0), new PointF(width, shearY * width), new PointF(shearX * height, height) };
                    g.DrawImage(image, destPoints);
                }

            return transformedBitmap;
        }
        private void btnShear_Click(object sender, EventArgs e)
        {
            if (originalImage != null && ValidateShearInput())
            {
                // Get the shearing factors from the textboxes
                float shearX, shearY;
                if (float.TryParse(txtShearX.Text, out shearX) && float.TryParse(txtShearY.Text, out shearY))
                {
                    // Perform shearing
                    transformedImage = ShearImage(originalImage, shearX, shearY);

                    // Display the transformed image
                    pictureBox.Image = transformedImage;
                }
                else
                {
                    MessageBox.Show("Invalid shearing values. Please enter valid floating-point numbers.");
                }
            }
        }


        ///////// End Of Shearing ///////////////////////////////////////////////////////////////////

        ///////// Reset Button to reset changes happened in the image and to clear all the textboxes
        private void btnReset_Click(object sender, EventArgs e)
        {
            // Reset the image
            pictureBox.Image = originalImage;

            // Clear textboxes
            ClearTextBoxes();
        }

        ///////// ClearTextBoxes to clear all the textboxes
        private void ClearTextBoxes()
        {
            txtTranslateX.Clear();
            txtTranslateY.Clear();
            txtScaleX.Clear();
            txtScaleY.Clear();
            txtRotationAngle.Clear();
            txtReflectionX.Clear();
            txtReflectionY.Clear();
            txtShearX.Clear();
            txtShearY.Clear();
        }

        ///////// Validation ( To make sure the user entered values and it must be a number ) ///////////////////////////////////////////////////////////////////
        private bool ValidateTranslationInput()
        {
            return !string.IsNullOrWhiteSpace(txtTranslateX.Text) && !string.IsNullOrWhiteSpace(txtTranslateY.Text);
        }

        private bool ValidateScaleInput()
        {
            return !string.IsNullOrWhiteSpace(txtScaleX.Text) && !string.IsNullOrWhiteSpace(txtScaleY.Text);
        }

        private bool ValidateRotationInput()
        {
            return !string.IsNullOrWhiteSpace(txtRotationAngle.Text);
        }

        private bool ValidateReflectionInput()
        {
            return !string.IsNullOrWhiteSpace(txtReflectionX.Text) && !string.IsNullOrWhiteSpace(txtReflectionY.Text);
        }

        private bool ValidateShearInput()
        {
            return !string.IsNullOrWhiteSpace(txtShearX.Text) && !string.IsNullOrWhiteSpace(txtShearY.Text);
        }

        ///////// End Of Validation ///////////////////////////////////////////////////////////////////
       

    }
}
        
