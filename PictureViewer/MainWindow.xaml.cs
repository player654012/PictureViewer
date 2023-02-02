using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace PictureViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point startPoint, endPoint;
        private Rectangle rect;
        private bool isDrawing = false;
        private SolidColorBrush selectedColor = Brushes.Red;
        private double rightBoundary;
        private double bottomBoundary;


        public MainWindow()
        {
            InitializeComponent();
        }


        // Load image from File Dialog
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // If the user selected a file in the OpenFileDialog and clicked the "OK" button
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the path of selected file
                image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                image.Stretch = Stretch.None;

                // Clean the previous rectangles and thumbs
                Thumbs_Cleaner();
                foreach (var item in canvas.Children.OfType<Rectangle>().ToList())
                {
                    canvas.Children.Remove(item);
                }

                canvas.Width = image.ActualWidth;
                canvas.Height = image.ActualHeight;
                canvas.UpdateLayout();
            }
            
        }


        // Save image with rectangles to File Dialog
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG (.jpg)|.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Clean all resize thumbs
                Thumbs_Cleaner();

                // Get the size of the canvas and create a RenderTargetBitmap of the same size
                int width = (int)image.ActualWidth;
                int height = (int)image.ActualHeight;
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);

                // Render the canvas and its elements to the RenderTargetBitmap
                canvas.Measure(new System.Windows.Size(width, height));
                canvas.Arrange(new Rect(0, 0, width, height));
                renderBitmap.Render(canvas);

                // Encode the RenderTargetBitmap as a JPEG and save it to the specified file
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                // New image saved
                canvas.Width = image.ActualWidth;
                canvas.Height = image.ActualHeight;
                canvas.UpdateLayout();
                MessageBox.Show("Image saved!");
            }
        }


        // Start to drag a new rectangle on image
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if the left mouse button is pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Get the position of the mouse cursor when the mouse button is pressed
                startPoint = e.GetPosition(image);

                // Create a new rectangle
                Generate_Rectangle();

                // Set the isDrawing flag to true
                isDrawing = true;
            }
            Thumbs_Cleaner();
        }


        // During dragging a rectangle on image
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            // Update rectangle when dragging
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                // Get the current position of the mouse cursor
                endPoint = e.GetPosition(image);

                // Set the width and height of the rectangle based on the difference between the start and end points
                rect.Width = Math.Abs(startPoint.X - endPoint.X);
                rect.Height = Math.Abs(startPoint.Y - endPoint.Y);

                // Set the position of the rectangle on the canvas
                Canvas.SetLeft(rect, Math.Min(startPoint.X, endPoint.X));
                Canvas.SetTop(rect, Math.Min(startPoint.Y, endPoint.Y));
            }
        }


        // Dragging finished on image
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            foreach (var item in canvas.Children.OfType<Rectangle>().ToList())
            {
                if (item.Width == 0 && item.Height == 0)
                {
                    canvas.Children.Remove(item);
                    return;
                }
            }
            if (isDrawing)
            {
                Thumb_Generater(rect);
            }

            // Set the isDrawing flag to false
            isDrawing = false;
        }


        // Drag a new rectangle on previous rectangle
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                return;
            }

            isDrawing = true;
            startPoint = e.GetPosition(canvas);
            Generate_Rectangle();
        }

        // During dragging a rectangle on existing rectangle
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(canvas);
                rect.Width = Math.Abs(currentPoint.X - startPoint.X);
                rect.Height = Math.Abs(currentPoint.Y - startPoint.Y);

                Canvas.SetLeft(rect, Math.Min(startPoint.X, currentPoint.X));
                Canvas.SetTop(rect, Math.Min(startPoint.Y, currentPoint.Y));
            }
        }

        // Dragging finished on the existing rectangle
        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rect = sender as Rectangle;
            foreach (var item in canvas.Children.OfType<Rectangle>().ToList())
            {
                if(item.Width == double.NaN&& item.Height == double.NaN)
                {
                    canvas.Children.Remove(item);
                }
            }
            if (isDrawing)
            {
                Thumb_Generater(rect);
            }
            isDrawing = false;
        }

        // Delete the chosen rectangle
        private void Rectangle_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            rect = sender as Rectangle;
            // Clean the chosen rectangle element
            canvas.Children.Remove(rect);

            // Clean the surrounding thumbs
            Thumbs_Cleaner();
        }


        private void Generate_Rectangle()
        {
            Thumbs_Cleaner();
            rect = new Rectangle();

            // Add event handlers to the rectangle for mouse down, move, right button up, and left button up events
            rect.MouseDown += Rectangle_MouseDown;
            rect.MouseMove += Rectangle_MouseMove;
            rect.MouseLeftButtonUp += Rectangle_MouseLeftButtonUp;
            rect.MouseRightButtonUp += Rectangle_MouseRightButtonUp;

            // Set the stroke and fill properties of the rectangle
            rect.Stroke = selectedColor;
            rect.Fill = Brushes.Transparent;
            rect.StrokeThickness = 2;

            rect.Width = 0;
            rect.Height = 0;

            // Add the rectangle to the canvas
            canvas.Children.Add(rect);
        }

        private void Thumbs_Cleaner()
        {
            foreach (var item in canvas.Children.OfType<Thumb>().ToList())
            {
                canvas.Children.Remove(item);
            }
        }

        // Creat resize thumbs on the four corners of rectangles
        private void Thumb_Generater(object sender)
        {
            if (sender == null) { return; }
            // Clean thumbs of all other rectangles
            Thumbs_Cleaner();

            rect = sender as Rectangle;
            
            // Creat a thumbs on the bottom-left corner
            Thumb centerThumb = new Thumb();
            centerThumb.Name = "CenterThumb";
            centerThumb.Width = 5;
            centerThumb.Height = 5;
            Canvas.SetLeft(centerThumb, Canvas.GetLeft(rect) + rect.Width / 2 - centerThumb.Width / 2);
            Canvas.SetTop(centerThumb, Canvas.GetTop(rect) + rect.Height / 2 - centerThumb.Height / 2);
            centerThumb.Cursor = Cursors.SizeAll;
            centerThumb.DragDelta += Thumb_DragDelta;
            centerThumb.DragCompleted += Thumb_DragComplete;
            canvas.Children.Add(centerThumb);

            // Creat a thumbs on the bottom-right corner
            Thumb bottomRight = new Thumb();
            bottomRight.Name = "BottomRight";
            bottomRight.Width = 5;
            bottomRight.Height = 5;
            Canvas.SetLeft(bottomRight, Canvas.GetLeft(rect) + rect.Width - bottomRight.Width / 2);
            Canvas.SetTop(bottomRight, Canvas.GetTop(rect) + rect.Height - bottomRight.Height / 2);
            bottomRight.Cursor = Cursors.SizeNWSE;
            bottomRight.DragDelta += Thumb_DragDelta;
            bottomRight.DragCompleted += Thumb_DragComplete;
            canvas.Children.Add(bottomRight);

            // Creat a thumbs on the top-left corner
            Thumb topLeft = new Thumb();
            topLeft.Name = "TopLeft";
            topLeft.Width = 5;
            topLeft.Height = 5;
            Canvas.SetLeft(topLeft, Canvas.GetLeft(rect));
            Canvas.SetTop(topLeft, Canvas.GetTop(rect));
            topLeft.Cursor = Cursors.SizeNWSE;
            topLeft.DragDelta += Thumb_DragDelta;
            topLeft.DragCompleted += Thumb_DragComplete;
            canvas.Children.Add(topLeft);

            // Creat a thumbs on the top-right corner
            Thumb topRight = new Thumb();
            topRight.Name = "TopRight";
            topRight.Width = 5;
            topRight.Height = 5;
            Canvas.SetLeft(topRight, Canvas.GetLeft(rect) + rect.Width - topRight.Width / 2);
            Canvas.SetTop(topRight, Canvas.GetTop(rect));
            topRight.Cursor = Cursors.SizeNESW;
            topRight.DragDelta += Thumb_DragDelta;
            topRight.DragCompleted += Thumb_DragComplete;
            canvas.Children.Add(topRight);

            // Creat a thumbs on the bottom-left corner
            Thumb bottomLeft = new Thumb();
            bottomLeft.Name = "BottomLeft";
            bottomLeft.Width = 5;
            bottomLeft.Height = 5;
            Canvas.SetLeft(bottomLeft, Canvas.GetLeft(rect));
            Canvas.SetTop(bottomLeft, Canvas.GetTop(rect) + rect.Height - bottomLeft.Height / 2);
            bottomLeft.Cursor = Cursors.SizeNESW;
            bottomLeft.DragDelta += Thumb_DragDelta;
            bottomLeft.DragCompleted += Thumb_DragComplete;
            canvas.Children.Add(bottomLeft);
        }

        // Resize the rectangle when dragging the thumb
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;

            double deltaVertical =  e.VerticalChange / 25;
            double deltaHorizontal = e.HorizontalChange / 25;
            rightBoundary =  image.ActualWidth;
            bottomBoundary = image.ActualHeight;

            switch (thumb.Name)
            {
                case "CenterThumb":
                    if (Canvas.GetLeft(rect) + deltaHorizontal >= 0 && Canvas.GetTop(rect) + deltaVertical >= 0
                        && Canvas.GetLeft(rect) + deltaHorizontal + rect.Width <= rightBoundary
                        && Canvas.GetTop(rect) + deltaVertical + rect.Height <= bottomBoundary)
                    {
                        var newLeft = Canvas.GetLeft(rect) + deltaHorizontal;
                        var newTop = Canvas.GetTop(rect) + deltaVertical;

                        Canvas.SetLeft(rect, newLeft);
                        Canvas.SetTop(rect, newTop);
                    }
                    break;
                case "TopLeft":
                    if (rect.Width - deltaHorizontal >= 0 && rect.Height - deltaVertical >= 0)
                    {
                        var newLeft = Canvas.GetLeft(rect) + deltaHorizontal;
                        var newTop = Canvas.GetTop(rect) + deltaVertical;
                        if(newLeft >= 0 && newTop >= 0)
                        {
                            Canvas.SetLeft(rect, newLeft);
                            Canvas.SetTop(rect, newTop);
                            rect.Width = rect.Width - deltaHorizontal;
                            rect.Height = rect.Height - deltaVertical;
                        }
                    }
                    break;
                case "TopRight":
                    if (rect.Width + deltaHorizontal >= 0 && rect.Height - deltaVertical >= 0)
                    {
                        var newLeft = Canvas.GetLeft(rect);
                        var newTop = Canvas.GetTop(rect) + deltaVertical;
                        if (newTop >= 0 && newLeft + rect.Width + deltaHorizontal <= rightBoundary)
                        {
                            Canvas.SetTop(rect, newTop);
                            rect.Width = rect.Width + deltaHorizontal;
                            rect.Height = rect.Height - deltaVertical;
                        }
                    }
                    break;
                case "BottomLeft":
                    if (rect.Width - deltaHorizontal >= 0 && rect.Height + deltaVertical >= 0)
                    {
                        var newLeft = Canvas.GetLeft(rect) + deltaHorizontal;
                        var newTop = Canvas.GetTop(rect);
                        if (newLeft >= 0 && newTop + rect.Height + deltaVertical <= bottomBoundary)
                        {
                            Canvas.SetLeft(rect, newLeft);
                            rect.Width = rect.Width - deltaHorizontal;
                            rect.Height = rect.Height + deltaVertical;
                        }
                    }
                    break;
                case "BottomRight":
                    if (rect.Width + deltaHorizontal >= 0 && rect.Height + deltaVertical >= 0)
                    {
                        var newLeft = Canvas.GetLeft(rect);
                        var newTop = Canvas.GetTop(rect);
                        if (newLeft + rect.Width + deltaHorizontal <= rightBoundary  && newTop + rect.Height + deltaVertical <= bottomBoundary)
                        {
                            rect.Width = rect.Width + deltaHorizontal;
                            rect.Height = rect.Height + deltaVertical;
                        }
                    }
                    break;
            }
        }


        // Update thumbs of rectangles when dragging completed
        private void Thumb_DragComplete(object sender, DragCompletedEventArgs e)
        {
            Thumb_Generater(rect);
        }


        // Switch the color of rectangles
        private void Color_Checked(object sender, RoutedEventArgs e)
        {
            if (redRadioButton.IsChecked == true)
            {
                selectedColor = Brushes.Red;
            }
            else if (blackRadioButton.IsChecked == true)
            {
                selectedColor = Brushes.Black;
            }
            else if (blueRadioButton.IsChecked == true)
            {
                selectedColor = Brushes.Blue;
            }
            else if (greenRadioButton.IsChecked == true)
            {
                selectedColor = Brushes.Green;
            }
            if (rect != null)
            {
                //set color for chosen rectangle
                rect.Stroke = selectedColor;
            }
        }

    }
}
