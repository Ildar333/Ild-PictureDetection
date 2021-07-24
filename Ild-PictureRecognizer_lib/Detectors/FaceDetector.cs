using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System;

namespace Ild_PictureRecognizer_lib.Detectors
{
    public class FaceDetector
    {
        public enum detectedEntities { Face, Eyes, FullBody, UpperBody, LowerBody, LicensePlate, LicensePlate_RUS}

        Image<Bgr, byte> image = null;
        Image<Gray, byte> grayImage = null;
        Bitmap outpuImage = null;
        Bitmap outputGrayImage = null;
        CascadeClassifier cascade = null;

        public FaceDetector()
        {

        }

        public FaceDetector(Bitmap inputImage)
        {
            this.image = new Image<Bgr, byte>(inputImage);
            grayImage = image.Convert<Gray, byte>();
            CvInvoke.EqualizeHist(grayImage,grayImage);
        }

        public void PrepareCascade(detectedEntities entity)
        {
            switch (entity)
            {
                case detectedEntities.Face:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_frontalface_alt.xml");
                    break;
                case detectedEntities.Eyes:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_eye.xml");
                    break;
                case detectedEntities.FullBody:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_fullbody.xml");
                    break;
                case detectedEntities.UpperBody:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_upperbody.xml");
                    break;
                case detectedEntities.LowerBody:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_lowerbody.xml");
                    break;
                case detectedEntities.LicensePlate:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_licence_plate_rus_16stages.xml");
                    break;
                case detectedEntities.LicensePlate_RUS:
                    this.cascade = new CascadeClassifier(Environment.CurrentDirectory + @"\Resources\haarcascade_russian_plate_number.xml");
                    break;
                default:
                    break;
            }
        }

        public void ProccessPicture()
        {
            Rectangle[] faces = cascade.DetectMultiScale(grayImage, 1.1, 3, Size.Empty, Size.Empty);

            foreach (Rectangle face in faces) 
            {
                CvInvoke.Rectangle(image, face, new MCvScalar(02, 0, 255), 3, Emgu.CV.CvEnum.LineType.Filled);

                Image<Bgr, byte> resultImage = image.Convert<Bgr, byte>();
                resultImage.ROI = face;
                resultImage.Resize(158,158, Inter.Cubic);
                outpuImage = resultImage.ToBitmap();
                outputGrayImage = resultImage.Convert<Gray, byte>().Bitmap;
            }
        }

        public BitmapImage ConvertToSource(Bitmap img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public Bitmap ConvertFromSource(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public BitmapImage GetResult() 
        {
            return ConvertToSource(outpuImage);
        }

        public BitmapImage GetGrayResult() 
        {
            return ConvertToSource(outputGrayImage);
        }
    }
}
