using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImgOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ControlPanel cp = new ControlPanel();
        private static readonly RotateTransform myRotateTransform = new RotateTransform();
        private static readonly ScaleTransform myScaleTransform = new ScaleTransform();
        private static readonly TranslateTransform myTranslateTransform = new TranslateTransform();

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();

            this.Left = 624;
            this.Top = 80;
            this.Width = 1424 - 624;
            this.Height = 880 - 80;
        }

        public bool LoadImage(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                MessageBox.Show("Cannot open folders.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("The selected image file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var img = new BitmapImage();
            try
            {
                img.BeginInit();
                img.UriSource = new Uri(path);
                img.EndInit();
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading image. Perhaps its format is unsupported?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            DisplayImage.Source = img;
            cp.UpdateOffsetSliders(img.PixelWidth, img.PixelHeight);
            return true;
        }

        public void ChangeOpacity(float opacity)
        {
            DisplayImage.Opacity = opacity;
        }

        public void ChangeRotation(float angle)
        {
            // Set the rotation of the transform.
            myRotateTransform.Angle = angle;

            UpdateTransform();
        }

        public void ChangeScale(float scale)
        {
            // Set the scale of the transform.
            myScaleTransform.ScaleX = scale;
            myScaleTransform.ScaleY = scale;

            UpdateTransform();
        }

        public void ChangeOffsetX(float offset)
        {
            myTranslateTransform.X = offset;
            UpdateTransform();
        }

        public void ChangeOffsetY(float offset)
        {
            myTranslateTransform.Y = offset;
            UpdateTransform();
        }

        public void UpdateImagePositionAndSize(double x1, double y1, double x2, double y2)
        {
            this.Left = x1;
            this.Top = y1;
            this.Width = x2 - x1;
            this.Height = y2 - y1;
        }

        private void UpdateTransform()
        {
            // Create a TransformGroup to contain the transforms
            // and add the transforms to it.
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myScaleTransform);
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslateTransform);

            DisplayImage.RenderTransformOrigin = new Point(0.5, 0.5);
            // Associate the transforms to the button.
            DisplayImage.RenderTransform = myTransformGroup;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cp.Owner = this;
            cp.Show();
            cp.Closed += (o, ev) =>
            {
                this.Close();
            };

            this.LocationChanged += MainWindow_LocationChanged;
            this.SizeChanged += MainWindow_SizeChanged;
            UpdateCoordinateControls();
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            UpdateCoordinateControls();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCoordinateControls();
        }

        private void UpdateCoordinateControls()
        {
            cp.UpdateCoordinateUpDowns(this.Left, this.Top, this.Left + this.Width, this.Top + this.Height);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
    }
}
