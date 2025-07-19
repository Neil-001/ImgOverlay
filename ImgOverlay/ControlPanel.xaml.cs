using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace ImgOverlay
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Window
    {
        public ControlPanel() => InitializeComponent();

        private void DragButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner == null)
                return;

            var opaque = (sender as ToggleButton).IsChecked.Value;
            Owner.IsHitTestVisible = opaque;

            var hwnd = new WindowInteropHelper(Owner).Handle;
            if (opaque)
            {
                WindowsServices.SetWindowExOpaque(hwnd);
            }
            else
            {
                WindowsServices.SetWindowExTransparent(hwnd);
            }

            e.Handled = true;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                if ((Owner as MainWindow)?.LoadImage(openDialog.FileName) == true)
                {
                    DragButton.IsEnabled = true;
                }
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (Owner as MainWindow)?.ChangeOpacity((float)e.NewValue);
        }

        private void RotateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (Owner as MainWindow)?.ChangeRotation((float)e.NewValue);
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (Owner as MainWindow)?.ChangeScale((float)e.NewValue);
        }

        private void OffsetXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (Owner as MainWindow)?.ChangeOffsetX((float)e.NewValue);
        }

        private void OffsetYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (Owner as MainWindow)?.ChangeOffsetY((float)e.NewValue);
        }

        public void UpdateOffsetSliders(double width, double height)
        {
            OffsetXSlider.Minimum = -width;
            OffsetXSlider.Maximum = width;
            OffsetYSlider.Minimum = -height;
            OffsetYSlider.Maximum = height;
        }

        private bool _isUpdatingFromMainWindow = false;
        public void UpdateCoordinateUpDowns(double x1, double y1, double x2, double y2)
        {
            _isUpdatingFromMainWindow = true;
            X1UpDown.Value = (int)x1;
            Y1UpDown.Value = (int)y1;
            X2UpDown.Value = (int)x2;
            Y2UpDown.Value = (int)y2;
            _isUpdatingFromMainWindow = false;
        }

        private void ResetOpacityButton_Click(object sender, RoutedEventArgs e)
        {
            OpacitySlider.Value = 1;
        }

        private void ResetScaleButton_Click(object sender, RoutedEventArgs e)
        {
            ScaleSlider.Value = 1;
        }

        private void ResetRotateButton_Click(object sender, RoutedEventArgs e)
        {
            RotateSlider.Value = 0;
        }

        private void ResetOffsetXButton_Click(object sender, RoutedEventArgs e)
        {
            OffsetXSlider.Value = 0;
        }

        private void ResetOffsetYButton_Click(object sender, RoutedEventArgs e)
        {
            OffsetYSlider.Value = 0;
        }

        private void NearestNeighborCheckBox_Click(object sender, RoutedEventArgs e)
        {
            (Owner as MainWindow)?.SetScalingMode(NearestNeighborCheckBox.IsChecked ?? false);
        }

        private void ControlPanel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] s = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
                if (s.Length == 1)
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void ControlPanel_Drop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s.Length == 1)
            {
                if ((Owner as MainWindow)?.LoadImage(s[0]) == true)
                {
                    DragButton.IsEnabled = true;
                }
            }
        }

        private void CoordinateUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_isUpdatingFromMainWindow) return;

            if (X1UpDown.Value.HasValue && Y1UpDown.Value.HasValue && X2UpDown.Value.HasValue && Y2UpDown.Value.HasValue)
            {
                (Owner as MainWindow)?.UpdateImagePositionAndSize(
                    X1UpDown.Value.Value,
                    Y1UpDown.Value.Value,
                    X2UpDown.Value.Value,
                    Y2UpDown.Value.Value);
            }
        }
    }
}
