using AForge.Video.DirectShow;
using Ild_PictureRecognizer_lib.Detectors;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ild_PictureDetector
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path;
        private string url;

        private FilterInfoCollection collection;
        private VideoCaptureDevice device;

        private IEnumerable<string> labelssDetectedEntities = new string[] { "Faces", "Eyes", "Full Body", "Upper Body", "Lower Body", "Licences", "Licences_RUS"};

        public MainWindow()
        {
            InitializeComponent();
            cmbDetect.IsEnabled = true;
            btnDetect.IsEnabled = false;
            btnCamera.IsEnabled = false;
            imgMain.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Image Sources/wait_image.png"));
            imgDetected.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Image Sources/detector.png"));
            imgDetectedGray.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Image Sources/detector.png"));
            cmbDetect.ItemsSource = this.labelssDetectedEntities;

        }

        private void btnDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();

                if (dialog.ShowDialog() == true) 
                {
                    imgMain.Source = new BitmapImage(new Uri(dialog.FileName));
                    this.path = dialog.FileName;
                    cmbDetect.IsEnabled = true;
                    btnDetect.IsEnabled = true;
                    imgDetected.IsEnabled = true;
                    imgDetectedGray.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDetect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FaceDetector detector;
                FaceDetector.detectedEntities entities = FaceDetector.detectedEntities.Face;

                switch (cmbDetect.SelectedIndex)
                {
                    case 0:
                        entities = FaceDetector.detectedEntities.Face;
                        break;
                    case 1:
                        entities = FaceDetector.detectedEntities.Eyes;
                        break;
                    case 2:
                        entities = FaceDetector.detectedEntities.FullBody;
                        break;
                    case 3:
                        entities = FaceDetector.detectedEntities.UpperBody;
                        break;
                    case 4:
                        entities = FaceDetector.detectedEntities.LowerBody;
                        break;
                    case 5:
                        entities = FaceDetector.detectedEntities.LicensePlate;
                        break;
                    case 6:
                        entities = FaceDetector.detectedEntities.LicensePlate_RUS;
                        break;
                    default:
                        break;
                }

                if (path != null)
                {
                    var snapImage = new Bitmap(this.path);
                    detector = new FaceDetector(snapImage);
                }
                else
                {
                    detector = new FaceDetector(new FaceDetector().ConvertFromSource((BitmapImage)imgMain.Source));
                }

                detector.PrepareCascade(entities);
                detector.ProccessPicture();
                imgDetected.Source = detector.GetResult();
                imgDetectedGray.Source = detector.GetGrayResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            device = new VideoCaptureDevice(collection[lsCams.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        private void Device_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                var image = (Bitmap)eventArgs.Frame.Clone();
                var frame = new FaceDetector().ConvertToSource(image);
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    imgMain.Source = frame;
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            collection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo item in collection)
            {
                lsCams.Items.Add(item.Name.ToString());
            }
        }

        private void lsCams_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            cmbDetect.IsEnabled = true;
            btnDetect.IsEnabled = true;
            imgDetected.IsEnabled = true;
            imgDetectedGray.IsEnabled = true;
            btnCamera.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var saveFile = new SaveFileDialog();
            FileStream fileStream = null;
            Bitmap saveImage = new FaceDetector().ConvertFromSource((BitmapImage)imgDetected.Source);

            if (saveFile.ShowDialog() == true)
            {
                saveFile.FileName = saveFile.FileName + ".Jpeg";
                fileStream = (FileStream)saveFile.OpenFile();
                saveImage.Save(fileStream, ImageFormat.Jpeg);
                fileStream.Close();
            }
        }

        private void btnVideoDirectory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();

                if (dialog.ShowDialog() == true)
                {
                    imgMain.Source = new BitmapImage(new Uri(dialog.FileName));
                    this.path = dialog.FileName;
                    cmbDetect.IsEnabled = true;
                    btnDetect.IsEnabled = true;
                    imgDetected.IsEnabled = true;
                    imgDetectedGray.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
