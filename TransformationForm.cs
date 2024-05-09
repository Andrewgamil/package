using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace package
{
    public partial class TransformationForm : Form
    {
        private Bitmap originalImage;
        private Bitmap transformedImage;
        public TransformationForm()
        {
            InitializeComponent();
            // Load the original image
            originalImage = new Bitmap("C:\\Users\\andre\\Downloads\\sunflower.jpg");
            pictureBox.Image = originalImage;
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
        private Bitmap RotateImage(Bitmap image, float angle)
            {
                // Calculate the rotation angle in radians
                float angleRad = (float)(angle * Math.PI / 180.0);

                // Create a new bitmap for the transformed image
                Bitmap transformedBitmap = new Bitmap(image.Width, image.Height);

                // Calculate the center of the image
                float centerX = image.Width / 2.0f;
                float centerY = image.Height / 2.0f;

                // Loop through each pixel of the transformed image
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // Calculate the coordinates relative to the center
                        float newX = (x - centerX) * (float)Math.Cos(angleRad) - (y - centerY) * (float)Math.Sin(angleRad) + centerX;
                        float newY = (x - centerX) * (float)Math.Sin(angleRad) + (y - centerY) * (float)Math.Cos(angleRad) + centerY;

                        // Ensure the new coordinates are within the image bounds
                        if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                        {
                            // Set the pixel color in the transformed image
                            transformedBitmap.SetPixel((int)newX, (int)newY, image.GetPixel(x, y));
                        }
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
                    MessageBox.Show("Invalid shearing values. Please enter numeric values.");
                }
            }
        }


        private Bitmap ShearImage(Bitmap image, float shearX, float shearY)
            {
                // Create a new bitmap for the transformed image
                Bitmap transformedBitmap = new Bitmap(image.Width, image.Height);

                // Loop through each pixel of the transformed image
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // Calculate the sheared coordinates
                        int newX = (int)(x + shearX * y);
                        int newY = (int)(y + shearY * x);

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
        private void btnReset_Click(object sender, EventArgs e)
        {
            // Reset the image
            pictureBox.Image = originalImage;

            // Clear textboxes
            ClearTextBoxes();
        }

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

    }
}
        
