using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace package
{
    public partial class Form1 : Form
    {
        private Bitmap canvas; // Bitmap to draw on
        private Graphics g; // Graphics object to draw on the bitmap

        public Form1()
        {
            InitializeComponent();
            canvas = new Bitmap(panel1.Width, panel1.Height);
            g = Graphics.FromImage(canvas);
            panel1.Paint += panel1_Paint; // Register the event handler
        }

        private void buttonDDA_Click(object sender, EventArgs e)
        {
            if (IsInputValid("DDA Line", textBox_X0.Text, textBox_Y0.Text, textBox_XEnd.Text, textBox_YEnd.Text))
            {
                // Parse user input from textboxes
                int x0 = int.Parse(textBox_X0.Text) + panel1.Width / 2; // Adjust x0 to be relative to the center
                int y0 = panel1.Height / 2 - int.Parse(textBox_Y0.Text); // Adjust y0 to be relative to the center
                int xEnd = int.Parse(textBox_XEnd.Text) + panel1.Width / 2; // Adjust xEnd to be relative to the center
                int yEnd = panel1.Height / 2 - int.Parse(textBox_YEnd.Text); // Adjust yEnd to be relative to the center

                // Generate coordinates using DDA algorithm
                List<Point> points = lineDDA(x0, y0, xEnd, yEnd);

                // Clear the canvas
                g.Clear(Color.White);

                // Draw the Cartesian plane
                DrawCartesianPlane(g);

                // Draw a line using DDA algorithm
                foreach (Point point in points)
                {
                    // Draw a pixel on the bitmap
                    if ((int)point.X >= 0 && (int)point.X < canvas.Width && (int)point.Y >= 0 && (int)point.Y < canvas.Height)
                        canvas.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }

                // Prompt the user to select a file location
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "HTML File|*.html";
                saveFileDialog1.Title = "Save DDA Line Output";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string, open it for saving
                if (saveFileDialog1.FileName != "")
                {
                    SaveToHTML(x0, y0, xEnd, yEnd, points, saveFileDialog1.FileName);
                }

                // Redraw the panel
                panel1.Invalidate();
            }
        }

        private void SaveToHTML(int x0, int y0, int xEnd, int yEnd, List<Point> points, string fileName)
{
    // Create a new HTML document
    using (StreamWriter sw = new StreamWriter(fileName))
    {
        // Write HTML header
        sw.WriteLine("<!DOCTYPE html>");
        sw.WriteLine("<html>");
        sw.WriteLine("<head>");
        sw.WriteLine("<title>DDA Line Output</title>");
        sw.WriteLine("<style>");
        sw.WriteLine("table { width: 100%; border-collapse: collapse; }");
        sw.WriteLine("table, th, td { border: 1px solid black; padding: 8px; }");
        sw.WriteLine("</style>");
        sw.WriteLine("</head>");
        sw.WriteLine("<body>");
        sw.WriteLine("<h2>DDA Line from (" + x0 + "," + y0 + ") to (" + xEnd + "," + yEnd + ")</h2>");
        sw.WriteLine("<table>");
        sw.WriteLine("<tr><th>k</th><th>x</th><th>y</th><th>(x,y)</th></tr>");

        // Write data rows
        for (int i = 0; i < points.Count; i++)
        {
            Point point = points[i];
            sw.WriteLine("<tr><td>" + i + "</td><td>" + point.X + "</td><td>" + point.Y + "</td><td>(" + point.X + "," + point.Y + ")</td></tr>");
        }

        // Write HTML footer
        sw.WriteLine("</table>");
        sw.WriteLine("</body>");
        sw.WriteLine("</html>");
    }

    MessageBox.Show("HTML file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
}


        private List<Point> lineBresenham(int x0, int y0, int xEnd, int yEnd, out List<int> pkValues, out List<Point> nextPoints)
        {
            List<Point> points = new List<Point>();
            pkValues = new List<int>();
            nextPoints = new List<Point>();

            // Adjust input coordinates to be relative to the center
            x0 += panel1.Width / 2;
            y0 = panel1.Height / 2 - y0;
            xEnd += panel1.Width / 2;
            yEnd = panel1.Height / 2 - yEnd;

            int dx = Math.Abs(xEnd - x0), dy = Math.Abs(yEnd - y0);
            int p = 2 * dy - dx;
            int twoDy = 2 * dy, twoDyMinusDx = 2 * (dy - dx);
            int x, y;
            if (x0 > xEnd)
            {
                x = xEnd; y = yEnd; xEnd = x0;
            }
            else
            {
                x = x0; y = y0;
            }
            points.Add(new Point(x, y));
            pkValues.Add(p); // Add initial PK value
            nextPoints.Add(new Point(x0, y0)); // Add initial point

            while (x < xEnd)
            {
                x++;
                if (p < 0)
                    p += twoDy;
                else
                {
                    y++;
                    p += twoDyMinusDx;
                }
                points.Add(new Point(x, y));
                pkValues.Add(p); // Add PK value for each step
                nextPoints.Add(new Point(x, y)); // Add next point (xk+1, yk+1)
            }

            return points;
        }


        private void SaveBresenhamToHTML(int x0, int y0, int xEnd, int yEnd, List<int> pkValues, List<Point> points, string fileName)
        {
            // Create a new HTML document
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                // Write HTML header
                sw.WriteLine("<!DOCTYPE html>");
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("<title>Bresenham Line Output</title>");
                sw.WriteLine("<style>");
                sw.WriteLine("table { width: 100%; border-collapse: collapse; }");
                sw.WriteLine("table, th, td { border: 1px solid black; padding: 8px; }");
                sw.WriteLine("</style>");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                sw.WriteLine("<h2>Bresenham Line from (" + x0 + "," + y0 + ") to (" + xEnd + "," + yEnd + ")</h2>");
                sw.WriteLine("<table>");
                sw.WriteLine("<tr><th>k</th><th>pk</th><th>(xk+1, yk+1)</th></tr>");

                // Write data rows
                for (int i = 0; i < pkValues.Count; i++)
                {
                    int pk = pkValues[i];
                    Point point = points[i];
                    sw.WriteLine("<tr><td>" + i + "</td><td>" + pk + "</td><td>(" + point.X + "," + point.Y + ")</td></tr>");
                }

                // Write HTML footer
                sw.WriteLine("</table>");
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }

            MessageBox.Show("HTML file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }





        private void buttonBresenham_Click(object sender, EventArgs e)
        {
            if (IsInputValid("Bresenham Line", textBox_X0.Text, textBox_Y0.Text, textBox_XEnd.Text, textBox_YEnd.Text))
            {
                // Parse user input from textboxes
                int x0 = int.Parse(textBox_X0.Text);
                int y0 = int.Parse(textBox_Y0.Text);
                int xEnd = int.Parse(textBox_XEnd.Text);
                int yEnd = int.Parse(textBox_YEnd.Text);

                // Generate coordinates using Bresenham's algorithm
                List<int> pkValues;
                List<Point> nextPoints;
                List<Point> points = lineBresenham(x0, y0, xEnd, yEnd, out pkValues, out nextPoints);

                // Clear the canvas
                g.Clear(Color.White);

                // Draw the Cartesian plane
                DrawCartesianPlane(g);

                // Draw the line using Bresenham's algorithm
                for (int i = 0; i < pkValues.Count; i++)
                {
                    // Draw a pixel on the bitmap
                    int adjustedX = nextPoints[i].X + panel1.Width / 2; // Adjust x-coordinate to be relative to the center
                    int adjustedY = panel1.Height / 2 - nextPoints[i].Y; // Adjust y-coordinate to be relative to the center
                    if (adjustedX >= 0 && adjustedX < canvas.Width && adjustedY >= 0 && adjustedY < canvas.Height)
                    {
                        // Set the pixel on the canvas
                        canvas.SetPixel(adjustedX, adjustedY, Color.Black);
                    }

                    // Output the desired format to the console
                    Console.WriteLine($"{i}\t{pkValues[i]}\t({nextPoints[i].X},{nextPoints[i].Y})");
                }

                // Prompt the user to select a file location
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "HTML File|*.html";
                saveFileDialog1.Title = "Save Bresenham Line Output";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string, open it for saving
                if (saveFileDialog1.FileName != "")
                {
                    SaveBresenhamToHTML(x0, y0, xEnd, yEnd, pkValues, nextPoints, saveFileDialog1.FileName);
                }

                // Redraw the panel
                panel1.Invalidate();
            }


        }















        private void buttonCircle_Click(object sender, EventArgs e)
        {
            if (IsInputValid("Circle", textBox_XCenter.Text, textBox_YCenter.Text, textBox_Radius.Text))
            {
                // Proceed with drawing circle
                // Parse user input from textboxes
                int xCenter = int.Parse(textBox_XCenter.Text) + panel1.Width / 2; // Adjust xCenter to be relative to the center
                int yCenter = panel1.Height / 2 - int.Parse(textBox_YCenter.Text); // Adjust yCenter to be relative to the center
                int radius = int.Parse(textBox_Radius.Text);

                // Generate coordinates using Midpoint Circle algorithm
                List<Point> points = circleMidpoint(xCenter, yCenter, radius);

                // Clear the canvas
                g.Clear(Color.White);

                // Draw the Cartesian plane
                DrawCartesianPlane(g);

                // Draw a circle using Midpoint Circle algorithm
                foreach (Point point in points)
                {
                    // Draw a pixel on the bitmap
                    if ((int)point.X >= 0 && (int)point.X < canvas.Width && (int)point.Y >= 0 && (int)point.Y < canvas.Height)
                        canvas.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }

                // Redraw the panel
                panel1.Invalidate();


            }
        }


        private List<Point> lineDDA(int x0, int y0, int xEnd, int yEnd)
        {
            List<Point> points = new List<Point>();

            int dx = xEnd - x0, dy = yEnd - y0, steps, k;
            float xIncrement, yIncrement, x = x0, y = y0;
            if (Math.Abs(dx) > Math.Abs(dy))
                steps = Math.Abs(dx);
            else
                steps = Math.Abs(dy);
            xIncrement = (float)dx / (float)steps;
            yIncrement = (float)dy / (float)steps;

            points.Add(new Point(x0, y0)); // Add the starting point

            for (k = 0; k < steps; k++)
            {
                x += xIncrement;
                y += yIncrement;
                points.Add(new Point(round(x), round(y))); // Add each point along the line
            }

            return points;
        }





        private List<Point> circleMidpoint(int xCenter, int yCenter, int radius)
        {
            List<Point> points = new List<Point>();

            // Adjust center to be relative to the center of the panel
            xCenter += panel1.Width / 2;
            yCenter = panel1.Height / 2 - yCenter;

            int x = 0;
            int y = radius;
            int p = 1 - radius;

            circlePlotPoints(xCenter, yCenter, x, y, points);

            while (x < y)
            {
                x++;
                if (p < 0)
                    p += 2 * x + 1;
                else
                {
                    y--;
                    p += 2 * (x - y) + 1;
                }
                circlePlotPoints(xCenter, yCenter, x, y, points);
            }

            return points;
        }

        private void circlePlotPoints(int xCenter, int yCenter, int x, int y, List<Point> points)
        {
            points.Add(new Point(xCenter + x, yCenter + y));
            points.Add(new Point(xCenter - x, yCenter + y));
            points.Add(new Point(xCenter + x, yCenter - y));
            points.Add(new Point(xCenter - x, yCenter - y));
            points.Add(new Point(xCenter + y, yCenter + x));
            points.Add(new Point(xCenter - y, yCenter + x));
            points.Add(new Point(xCenter + y, yCenter - x));
            points.Add(new Point(xCenter - y, yCenter - x));
        }



        private int round(float a)
        {
            return (int)(a + 0.5);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the Cartesian plane
            DrawCartesianPlane(e.Graphics);

            // Draw the bitmap on the panel
            e.Graphics.DrawImage(canvas, 0, 0);
        }

        private void DrawCartesianPlane(Graphics g)
        {
            // Get panel dimensions
            int panelWidth = panel1.Width;
            int panelHeight = panel1.Height;

            // Draw x-axis
            g.DrawLine(Pens.Black, 0, panelHeight / 2, panelWidth, panelHeight / 2);

            // Draw y-axis
            g.DrawLine(Pens.Black, panelWidth / 2, 0, panelWidth / 2, panelHeight);

            // Draw x-axis ticks
            for (int i = -panelWidth / 2; i <= panelWidth / 2; i += 10)
            {
                g.DrawLine(Pens.Black, panelWidth / 2 + i, panelHeight / 2 - 3, panelWidth / 2 + i, panelHeight / 2 + 3);
            }

            // Draw y-axis ticks
            for (int i = -panelHeight / 2; i <= panelHeight / 2; i += 10)
            {
                g.DrawLine(Pens.Black, panelWidth / 2 - 3, panelHeight / 2 + i, panelWidth / 2 + 3, panelHeight / 2 + i);
            }
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            // Clear the canvas
            g.Clear(Color.White);

            // Redraw the Cartesian plane
            DrawCartesianPlane(g);

            // Redraw the panel
            panel1.Invalidate();
        }

        private bool IsInputValid(string algorithmName, params string[] inputs)
        {
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show($"Please enter values for all input fields to draw {algorithmName}.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        private void buttonEllipse_Click(object sender, EventArgs e)
        {
            if (IsInputValid("Ellipse", textBox_XCenter_Ellipse.Text, textBox_YCenter_Ellipse.Text, textBox_SemiMajorAxis.Text, textBox_SemiMinorAxis.Text))
            {
                // Proceed with drawing ellipse
                // Parse user input from textboxes
                int xCenter = int.Parse(textBox_XCenter_Ellipse.Text) + panel1.Width / 2; // Adjust xCenter to be relative to the center
                int yCenter = panel1.Height / 2 - int.Parse(textBox_YCenter_Ellipse.Text); // Adjust yCenter to be relative to the center
                int semiMajorAxis = int.Parse(textBox_SemiMajorAxis.Text);
                int semiMinorAxis = int.Parse(textBox_SemiMinorAxis.Text);

                // Clear the canvas
                g.Clear(Color.White);

                // Draw the Cartesian plane
                DrawCartesianPlane(g);

                // Draw the ellipse using Mid-Point Ellipse algorithm
                midptellipse(semiMajorAxis, semiMinorAxis, xCenter, yCenter);

                // Redraw the panel
                panel1.Invalidate();
            }
        }

        private void midptellipse(double rx, double ry, double xc, double yc)
        {
            // Adjust center to be relative to the center of the panel
            xc += panel1.Width / 2;
            yc = panel1.Height / 2 - yc;

            double dx, dy, d1, d2, x, y;
            x = 0;
            y = ry;

            // Initial decision parameter of region 1
            d1 = (ry * ry) - (rx * rx * ry) + (0.25 * rx * rx);
            dx = 2 * ry * ry * x;
            dy = 2 * rx * rx * y;

            // For region 1
            while (dx < dy)
            {
                // Draw points based on 4-way symmetry
                DrawEllipsePoints(xc, yc, (int)x, (int)y);

                // Checking and updating value of decision parameter based on algorithm
                if (d1 < 0)
                {
                    x++;
                    dx = dx + (2 * ry * ry);
                    d1 = d1 + dx + (ry * ry);
                }
                else
                {
                    x++;
                    y--;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d1 = d1 + dx - dy + (ry * ry);
                }
            }

            // Decision parameter of region 2
            d2 = ((ry * ry) * ((x + 0.5) * (x + 0.5))) + ((rx * rx) * ((y - 1) * (y - 1))) - (rx * rx * ry * ry);

            // Plotting points of region 2
            while (y >= 0)
            {
                // Draw points based on 4-way symmetry
                DrawEllipsePoints(xc, yc, (int)x, (int)y);

                // Checking and updating parameter value based on algorithm
                if (d2 > 0)
                {
                    y--;
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + (rx * rx) - dy;
                }
                else
                {
                    y--;
                    x++;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + dx - dy + (rx * rx);
                }
            }
        }

        private void DrawEllipsePoints(double xc, double yc, int x, int y)
        {
            // Draw points based on 4-way symmetry
            canvas.SetPixel((int)(x + xc), (int)(y + yc), Color.Black);
            canvas.SetPixel((int)(-x + xc), (int)(y + yc), Color.Black);
            canvas.SetPixel((int)(x + xc), (int)(-y + yc), Color.Black);
            canvas.SetPixel((int)(-x + xc), (int)(-y + yc), Color.Black);
        }

        private void btnOpenTransformationForm_Click(object sender, EventArgs e)
        {
            // Open the TransformationForm when the button is clicked
            TransformationForm transformationForm = new TransformationForm();
            transformationForm.Show();
        }

    }
}
