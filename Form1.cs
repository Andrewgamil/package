using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

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

        ///////// lineDDA ///////////////////////////////////////////////////////////////////
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
            // Create HTML content
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("<title>DDA Line Output</title>");
            htmlContent.AppendLine("<style>");
            htmlContent.AppendLine("table { width: 100%; border-collapse: collapse; }");
            htmlContent.AppendLine("table, th, td { border: 1px solid black; padding: 8px; }");
            htmlContent.AppendLine("</style>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");
            htmlContent.AppendLine("<h2>DDA Line from (" + x0 + ", " + y0 + ") to (" + xEnd + ", " + yEnd + ")</h2>");
            htmlContent.AppendLine("<table>");
            htmlContent.AppendLine("<tr><th>k</th><th>x</th><th>y</th><th>(x,y)</th></tr>");

            // Add data rows
            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                htmlContent.AppendLine("<tr><td>" + i + "</td><td>" + point.X + "</td><td>" + point.Y + "</td><td>(" + point.X + "," + point.Y + ")</td></tr>");
            }

            htmlContent.AppendLine("</table>");
            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");

            // Write content to file
            File.WriteAllText(fileName, htmlContent.ToString());

            MessageBox.Show("HTML file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        ///////// End Of lineDDA ///////////////////////////////////////////////////////////////////

        ///////// lineBresenham ///////////////////////////////////////////////////////////////////
        private List<Point> lineBresenham(int x0, int y0, int xEnd, int yEnd)
        {
            List<Point> points = new List<Point>();

            int dx = Math.Abs(xEnd - x0), dy = Math.Abs(yEnd - y0);
            int p = 2 * dy - dx;
            int twoDx = 2 * dx, twoDy = 2 * dy;
            int x, y;

            // Determine the direction of the line
            int xStep, yStep;
            if (x0 < xEnd)
                xStep = 1;
            else
                xStep = -1;

            if (y0 < yEnd)
                yStep = 1;
            else
                yStep = -1;

            x = x0;
            y = y0;

            points.Add(new Point(x, y));

            while (x != xEnd || y != yEnd)
            {
                if (p < 0)
                {
                    p += twoDy;
                }
                else
                {
                    p += twoDy - twoDx;
                    y += yStep; // Move upwards
                }

                x += xStep;
                points.Add(new Point(x, y));
            }

            return points;
        }
        private void buttonBresenham_Click(object sender, EventArgs e)
        {
            if (IsInputValid("Bresenham Line", textBox_X0.Text, textBox_Y0.Text, textBox_XEnd.Text, textBox_YEnd.Text))
            {
                // Proceed with drawing Bresenham line
                // Parse user input from textboxes
                int x0 = int.Parse(textBox_X0.Text) + panel1.Width / 2; // Adjust x0 to be relative to the center
                int y0 = panel1.Height / 2 - int.Parse(textBox_Y0.Text); // Adjust y0 to be relative to the center
                int xEnd = int.Parse(textBox_XEnd.Text) + panel1.Width / 2; // Adjust xEnd to be relative to the center
                int yEnd = panel1.Height / 2 - int.Parse(textBox_YEnd.Text); // Adjust yEnd to be relative to the center

                // Generate coordinates and pk values using Bresenham's algorithm
                List<Point> points = lineBresenham(x0, y0, xEnd, yEnd);
                List<int> pkValues = CalculatePkValues(x0, y0, xEnd, yEnd);

                // Clear the canvas
                g.Clear(Color.White);

                // Draw the Cartesian plane
                DrawCartesianPlane(g);

                // Draw a line using Bresenham's algorithm
                foreach (Point point in points)
                {
                    // Draw a pixel on the bitmap
                    if ((int)point.X >= 0 && (int)point.X < canvas.Width && (int)point.Y >= 0 && (int)point.Y < canvas.Height)
                        canvas.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }

                // Prompt the user to select a file location
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "HTML File|*.html";
                saveFileDialog1.Title = "Save Bresenham Line Output";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // Save the HTML table to the selected file location
                    string fileName = saveFileDialog1.FileName;
                    SaveToHTML(x0, y0, xEnd, yEnd, points, pkValues, fileName);
                }

                // Redraw the panel
                panel1.Invalidate();
            }
        }

        private List<int> CalculatePkValues(int x0, int y0, int xEnd, int yEnd)
        {
            List<int> pkValues = new List<int>();

            int dx = Math.Abs(xEnd - x0);
            int dy = Math.Abs(yEnd - y0);
            int p = 2 * dy - dx;

            int twoDy = 2 * dy;
            int twoDyMinusDx = 2 * (dy - dx);

            int x = x0;
            int y = y0;

            if (x0 > xEnd)
            {
                x = xEnd;
                y = yEnd;
                xEnd = x0;
            }
            else
            {
                x = x0;
                y = y0;
            }

            pkValues.Add(p); // Add initial p value

            while (x < xEnd)
            {
                x++;

                if (p < 0)
                {
                    p += twoDy;
                }
                else
                {
                    y++;
                    p += twoDyMinusDx;
                }

                pkValues.Add(p); // Add the updated p value
            }

            return pkValues;
        }


        private void SaveToHTML(int x0, int y0, int xEnd, int yEnd, List<Point> points, List<int> pkValues, string fileName)
        {
            // Create HTML content
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("<title>Bresenham Line Output</title>");
            htmlContent.AppendLine("<style>");
            htmlContent.AppendLine("table { width: 100%; border-collapse: collapse; }");
            htmlContent.AppendLine("table, th, td { border: 1px solid black; padding: 8px; }");
            htmlContent.AppendLine("</style>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");
            htmlContent.AppendLine("<h2>Bresenham Line from (" + x0 + ", " + y0 + ") to (" + xEnd + ", " + yEnd + ")</h2>");
            htmlContent.AppendLine("<table>");
            htmlContent.AppendLine("<tr><th>k</th><th>x</th><th>y</th><th>(x,y)</th><th>pk</th></tr>");

            // Add data rows
            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                htmlContent.AppendLine("<tr><td>" + i + "</td><td>" + point.X + "</td><td>" + point.Y + "</td><td>(" + point.X + "," + point.Y + ")</td><td>" + pkValues[i] + "</td></tr>");
            }

            htmlContent.AppendLine("</table>");
            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");

            // Write content to file
            File.WriteAllText(fileName, htmlContent.ToString());

            MessageBox.Show("HTML file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }




        ///////// End Of lineBresenham ///////////////////////////////////////////////////////////////////

        ///////// CircleBresenham ///////////////////////////////////////////////////////////////////
        private List<Point> circleMidpoint(int xCenter, int yCenter, int radius)
        {
            List<Point> points = new List<Point>();

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

                // Generate HTML table and save to file
                SaveHTMLTable(xCenter, yCenter, radius, points);
            }
        }

        private void SaveHTMLTable(int xCenter, int yCenter, int radius, List<Point> points)
        {
            // Prompt the user to select a file location
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "HTML Files|*.html";
            saveFileDialog.Title = "Save HTML Table";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;

                // Determine the octant
                int octant = DetermineOctant(xCenter, yCenter, points[0].X, points[0].Y);

                // Create HTML content
                StringBuilder htmlContent = new StringBuilder();
                htmlContent.AppendLine("<!DOCTYPE html>");
                htmlContent.AppendLine("<html>");
                htmlContent.AppendLine("<head>");
                htmlContent.AppendLine("<title>Bresenham Circle Output</title>");
                htmlContent.AppendLine("</head>");
                htmlContent.AppendLine("<body>");
                htmlContent.AppendLine("<h2>Bresenham Circle with center (" + xCenter + ", " + yCenter + ") and radius " + radius + " - " + GetOctantName(octant) + " octant</h2>");
                htmlContent.AppendLine("<table border='1' width='100%'>");
                htmlContent.AppendLine("<tr><th>k</th><th>x</th><th>y</th><th>(x,y)</th></tr>");

                // Add data rows
                for (int i = 0; i < points.Count; i++)
                {
                    Point point = points[i];
                    htmlContent.AppendLine("<tr><td>" + i + "</td><td>" + point.X + "</td><td>" + point.Y + "</td><td>(" + point.X + "," + point.Y + ")</td></tr>");
                }

                htmlContent.AppendLine("</table>");
                htmlContent.AppendLine("</body>");
                htmlContent.AppendLine("</html>");

                // Write content to file
                File.WriteAllText(fileName, htmlContent.ToString());

                MessageBox.Show("HTML file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private int DetermineOctant(int xCenter, int yCenter, int x, int y)
        {
            int dx = x - xCenter;
            int dy = y - yCenter;

            if (dx >= 0 && dy >= 0)
            {
                if (dx > dy)
                    return 1;
                else
                    return 2;
            }
            else if (dx < 0 && dy >= 0)
            {
                if (-dx < dy)
                    return 3;
                else
                    return 4;
            }
            else if (dx < 0 && dy < 0)
            {
                if (-dx > -dy)
                    return 5;
                else
                    return 6;
            }
            else // (dx >= 0 && dy < 0)
            {
                if (dx < -dy)
                    return 7;
                else
                    return 8;
            }
        }

        private string GetOctantName(int octant)
        {
            switch (octant)
            {
                case 1: return "first";
                case 2: return "second";
                case 3: return "third";
                case 4: return "fourth";
                case 5: return "fifth";
                case 6: return "sixth";
                case 7: return "seventh";
                case 8: return "eighth";
                default: return "";
            }
        }



        ///////// End Of  CircleBresenham ///////////////////////////////////////////////////////////////////

        ///////// Ellipse ///////////////////////////////////////////////////////////////////
       

        private List<Point> MidpointEllipse(int rx, int ry, int xc, int yc)
        {
            List<Point> points = new List<Point>();
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
                // Add points based on 4-way symmetry
                points.Add(new Point(xc + (int)x, yc + (int)y));
                points.Add(new Point(xc - (int)x, yc + (int)y));
                points.Add(new Point(xc + (int)x, yc - (int)y));
                points.Add(new Point(xc - (int)x, yc - (int)y));

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
                // Add points based on 4-way symmetry
                points.Add(new Point(xc + (int)x, yc + (int)y));
                points.Add(new Point(xc - (int)x, yc + (int)y));
                points.Add(new Point(xc + (int)x, yc - (int)y));
                points.Add(new Point(xc - (int)x, yc - (int)y));

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

            return points;
        }

        private void DrawEllipse(Graphics g, List<Point> points)
        {
            foreach (Point point in points)
            {
                // Draw a pixel on the bitmap
                if (point.X >= 0 && point.X < canvas.Width && point.Y >= 0 && point.Y < canvas.Height)
                    canvas.SetPixel(point.X, point.Y, Color.Black);
            }
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

                // Generate coordinates using Mid-Point Ellipse algorithm
                List<Point> points = MidpointEllipse(semiMajorAxis, semiMinorAxis, xCenter, yCenter);

                // Clear the canvas
                g.Clear(Color.White);

                // Draw the Cartesian plane
                DrawCartesianPlane(g);

                // Draw the ellipse using Mid-Point Ellipse algorithm
                DrawEllipse(g, points);

                // Prompt the user to select a file location
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "HTML Files|*.html";
                saveFileDialog.Title = "Save Ellipse Table";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;

                    // Save the HTML table to the selected file location
                    SaveHTMLTable(xCenter, yCenter, semiMajorAxis, semiMinorAxis, points, fileName);
                }

                // Redraw the panel
                panel1.Invalidate();
            }
        }
        private void SaveHTMLTable(int xCenter, int yCenter, int semiMajorAxis, int semiMinorAxis, List<Point> points, string fileName)
        {
            // Create HTML content
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("<title>Ellipse Output</title>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");
            htmlContent.AppendLine("<h2>Ellipse with center (" + xCenter + ", " + yCenter + ") and xradius " + semiMajorAxis + " and yradius " + semiMinorAxis + "</h2>");
            htmlContent.AppendLine("<table border='1' width='100%'>");
            htmlContent.AppendLine("<tr><th>k</th><th>x</th><th>y</th><th>(x,y)</th></tr>");

            // Add data rows
            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                htmlContent.AppendLine("<tr><td>" + i + "</td><td>" + point.X + "</td><td>" + point.Y + "</td><td>(" + point.X + "," + point.Y + ")</td></tr>");
            }

            htmlContent.AppendLine("</table>");
            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");

            // Write content to file
            File.WriteAllText(fileName, htmlContent.ToString());

            MessageBox.Show("HTML file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        ///////// End Of  Ellipse ///////////////////////////////////////////////////////////////////

        ///////// TransformationForm button to open the TransformationForm ///////////////////////////////////////////////////////////////////
        private void btnOpenTransformationForm_Click(object sender, EventArgs e)
        {
            // Open the TransformationForm when the button is clicked
            TransformationForm transformationForm = new TransformationForm();
            transformationForm.Show();
        }


        ///////// clear button to clear the drawings  ///////////////////////////////////////////////////////////////////
        private void button_Clear_Click(object sender, EventArgs e)
        {
            // Clear the canvas
            g.Clear(Color.White);

            // Redraw the Cartesian plane
            DrawCartesianPlane(g);

            // Redraw the panel
            panel1.Invalidate();
        }

        ///////// validation to ensure that  the user entered all the values of the drawings ///////////////////////////////////////////////////////////////////
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


        ///////// kjkhk ///////////////////////////////////////////////////////////////////
        private void panel1_Paint(object? sender, PaintEventArgs e)
        {
            // Draw the Cartesian plane
            DrawCartesianPlane(e.Graphics);

            // Draw the bitmap on the panel
            e.Graphics.DrawImage(canvas, 0, 0);
        }

        private int round(float a)
        {
            return (int)(a + 0.5);
        }

        ///////// Draw the Cartesian Plane ///////////////////////////////////////////////////////////////////
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

    }
}
