using CameraSystem;
using CameraTestSpace;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DynaTouch.CameraSystem
{
    public class Camera
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Camera));
        public EigenFaceRecognizer faceRecogntion { get; set; }
        public CascadeClassifier faceDetection { get; set; }
        public CascadeClassifier eyeDetection { get; set; }
        public CascadeClassifier faceRecognizer { get; set; }
        public Mat frameCapture { get; set; }
        public Mat recognitionFrame { get; set; }
        public bool faceSquare { get; set; } = true;
        public bool eyeSquare { get; set; } = true;

        public List<Image<Gray, byte>> faces { get; set; }
        public List<int> ID { get; set; }

        public int processedImageWidth { get; set; } = 128;
        public int processedImageHeight { get; set; } = 150;

        public int timerCounter { get; set; } = 0;
        public int scanLimit { get; set; } = 40;

        public int faceRecognitionAttempts = 0;
        public int maxFaceAttempts = 30;

        
        public int camOrientation = 0;
        public bool scanInProgress = false;
         
         

        public bool isInFocus = false;
        public string xmlPath { get; set; } = @"FacialData/trainingData.yml";
        public Bitmap SnapShot { get; private set; }

        public bool TrainingInProgress = false;
        public bool faceTrain = false;
        string saltDirectory = "FacialData/SaltImages";

        string paymentImagesAddress = "Images/PaymentImages";

        public Bitmap displayImage;

        public bool facesPresent = false;

        public int predictionLabel;
        public double predictionDistance;

        public Thread predictionThread;
        public bool approved = false;
        public bool rejected = false;

        public VideoCapture webCam { get; set; }

        public VideoWriter videoWriter = null;
        public DispatcherTimer autoFocusOff;
        public bool facialRec = false;
        public Guid userGuid;

        public CapPropData capPropDefault;
        public CapPropData capPropCurrent;
         

        public Camera()
        { 
            faceRecogntion = new EigenFaceRecognizer(30, double.PositiveInfinity);

            faceDetection = new CascadeClassifier((@"FacialData/haarcascade_frontalface_default.xml"));
            eyeDetection = new CascadeClassifier(("FacialData/haarcascade_eye.xml"));
            faces = new List<Image<Gray, byte>>();
            ID = new List<int>();

            capPropCurrent = new CapPropData();

            webCam = new VideoCapture(0, VideoCapture.API.DShow);// creation of a VideoCapture Object
            
            SaveDefaultSettings();
            
            SetSettingsValues();

            if (File.Exists("FacialData/PredictionLog.txt"))
            {
                File.Delete("FacialData/PredictionLog.txt");
            }
            userGuid = Guid.Empty;
            try
            {
                frameCapture = new Mat();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            TrainingInProgress = false;

        }  
        private void WebCamBase_ImageGrabbed(object sender, EventArgs e)
        {
            #region CapturePropertiesForBaseCamera             
            /*SetCameraSize(450, 800);*/
            #endregion 

            /* Any Changes to captures properties have to be made before webCam.Retrive Is Called*/
            webCam.Retrieve(frameCapture);
            if (frameCapture != null)
            {

                Image<Bgr, Byte> img = frameCapture.ToImage<Bgr, byte>();

                var imageFrame = frameCapture.ToImage<Bgr, byte>();

                if (imageFrame != null)
                {
                    var bitmapImage = imageFrame.ToBitmap(imageFrame.Width, imageFrame.Height);
                    switch (camOrientation)
                    {

                        case 0:
                            /*nothing to do, webcam is set correctly*/
                            break;
                        case 90:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 180:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 270:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                            break;
                        default:
                            break;

                    }

                    imageFrame = bitmapImage.ToImage<Bgr, byte>();
                    displayImage = bitmapImage;
                    /*Random rand = new Random();*/
                    /*bitmapImage.Save(rand.Next(1, 90000).ToString() + ".png");*/
                }
            }

        }

        public void SaveDefaultSettings()
        {
            try
            {
                capPropDefault = new CapPropData();
                capPropDefault.Focus = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus); 
                capPropDefault.Brightness = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness); 
                capPropDefault.Contrast = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast);
                capPropDefault.Gamma = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma);
                capPropDefault.Tilt = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt);
                capPropDefault.Roll = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll);
                capPropDefault.Pan = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan);
                capPropDefault.Backlight = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight);
                capPropDefault.Zoom = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom);
                capPropDefault.Sharpness = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness);
                capPropDefault.Saturation = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation);
                capPropDefault.Hue = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue);
                capPropDefault.Gain = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain);
                capPropDefault.Exposure = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure);
                capPropDefault.Temperature = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Temperature);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }

        }

        public void SaveDefaultSetting(string name, double value)
        {
            try
            {
                capPropDefault = new CapPropData();
                switch (name)
                {
                    case "Focus":
                        capPropCurrent.Focus = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus);
                        break;
                    case "Brightness":
                        capPropCurrent.Brightness = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness); 
                        break;
                    case "Contrast":
                        capPropCurrent.Contrast = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast);
                        break;
                    case "Gamma":
                        capPropCurrent.Gamma = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma);
                        break;
                    case "Tilt":
                        capPropCurrent.Tilt = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt);
                        break;
                    case "Roll":
                        capPropCurrent.Roll = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll);
                        break;
                    case "Pan":
                        capPropCurrent.Pan = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan);
                        break;
                    case "Backlight":
                        capPropCurrent.Backlight = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight);
                        break;
                    case "Zoom":
                        capPropCurrent.Zoom = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom);
                        break;
                    case "Sharpness":
                        capPropCurrent.Sharpness = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness);
                        break;
                    case "Saturation":
                        capPropCurrent.Saturation = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation);
                        break;
                    case "Hue":
                        capPropCurrent.Hue = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue);
                        break;
                    case "Gain":
                        capPropCurrent.Gain = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain);
                        break;
                    case "Exposure":
                        capPropCurrent.Exposure = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure);
                        break;
                    case "Temperature":
                        capPropCurrent.Temperature = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Temperature);
                        break;
                    default:
                        break;
                }
                 
                 // Sets the Brightness property to the value passed to it 
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
            
        }

        public double GetDefaultSetting(string name)
        {
            try
            {
                double myVal = 0;
                switch (name)
                {
                    case "Focus":
                        myVal = capPropDefault.Focus;
                        break;
                    case "Brightness":
                        myVal = capPropDefault.Brightness; // Sets the Brightness property to the value passed to it
                        break;
                    case "Contrast":
                        myVal = capPropDefault.Contrast;
                        break;
                    case "Gamma":
                        myVal = capPropDefault.Gamma;
                        break;
                    case "Tilt":
                        myVal = capPropDefault.Tilt;
                        break;
                    case "Roll":
                        myVal = capPropDefault.Roll;
                        break;
                    case "Pan":
                        myVal = capPropDefault.Pan;
                        break;
                    case "Backlight":
                        myVal = capPropDefault.Backlight;
                        break;
                    case "Zoom":
                        myVal = capPropDefault.Zoom;
                        break;
                    case "Sharpness":
                        myVal = capPropDefault.Sharpness;
                        break;
                    case "Saturation":
                        myVal = capPropDefault.Saturation;
                        break;
                    case "Hue":
                        myVal = capPropDefault.Hue;
                        break;
                    case "Gain":
                        myVal = capPropDefault.Gain;
                        break;
                    case "Exposure":
                        myVal = capPropDefault.Exposure;
                        break;
                    case "Temperature":
                        myVal = capPropDefault.Temperature;
                        break;
                    default:
                        break;
                }
                return myVal;
                // Sets the Brightness property to the value passed to it 
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }

        }

        public double GetCurrentSetting(string name)
        {
            try
            {
                double myVal = 0;
                switch (name)
                {
                    case "Focus":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus);
                        break;
                    case "Brightness":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness); // Sets the Brightness property to the value passed to it
                        break;
                    case "Contrast":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast);
                        break;
                    case "Gamma":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma);
                        break;
                    case "Tilt":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt);
                        break;
                    case "Roll":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll);
                        break;
                    case "Pan":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan);
                        break;
                    case "Backlight":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight);
                        break;
                    case "Zoom":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom);
                        break;
                    case "Sharpness":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness);
                        break;
                    case "Saturation":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation);
                        break;
                    case "Hue":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue);
                        break;
                    case "Gain":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain);
                        break;
                    case "Exposure":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure);
                        break;
                    case "Temperature":
                        myVal = webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Temperature);
                        break;
                    default:
                        break;
                }
                return myVal;
                // Sets the Brightness property to the value passed to it 
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }

        }

        public async Task SetSettingValue(string name,double value)
        {
            try
            {
                switch (name)
                {
                    case "Focus":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, value); 
                        capPropCurrent.Focus = value;
                        break;
                    case "Brightness":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, value);  
                        capPropCurrent.Brightness = value;
                        break;
                    case "Contrast":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, value);
                        capPropCurrent.Contrast = value;
                        break;
                    case "Gamma":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma, value);
                        capPropCurrent.Gamma = value;
                        break;
                    case "Tilt":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt, value);
                        capPropCurrent.Tilt = value;
                        break;
                    case "Roll":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll, value);
                        capPropCurrent.Roll = value;
                        break;
                    case "Pan":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan, value);
                        capPropCurrent.Pan = value;
                        break;
                    case "Backlight":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight, value);
                        capPropCurrent.Backlight = value;
                        break;
                    case "Zoom":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom, value);
                        capPropCurrent.Zoom = value;
                        break;
                    case "Sharpness":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness, value);
                        capPropCurrent.Sharpness = value;
                        break;
                    case "Saturation":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation, value);
                        capPropCurrent.Saturation = value;
                        break;
                    case "Hue":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue, value);
                        capPropCurrent.Hue = value;
                        break;
                    case "Gain":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain, value);
                        capPropCurrent.Gain = value;
                        break;
                    case "Exposure":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, value);
                        capPropCurrent.Exposure = value;
                        break;
                    case "Temperature":
                        webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Temperature, value);
                        capPropCurrent.Temperature = value;
                        break;
                    default:
                        break;
                }
                SaveDefaultSetting(name, value);
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }

        }

        public async Task SetSettingsValues()
        {
            try
            {
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, capPropCurrent.Focus); 
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, capPropCurrent.Brightness); 
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, capPropCurrent.Contrast);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma, capPropCurrent.Gamma);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt, capPropCurrent.Tilt);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll, capPropCurrent.Roll);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan, capPropCurrent.Pan);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight, capPropCurrent.Backlight);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom, capPropCurrent.Zoom);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness, capPropCurrent.Sharpness);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation, capPropCurrent.Saturation);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue, capPropCurrent.Hue);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain, capPropCurrent.Gain);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, capPropCurrent.Exposure);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Temperature, capPropCurrent.Temperature);
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }

        }

        public void RestoreSettingsValues()
        {
            try
            {
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, capPropDefault.Focus); 
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, capPropDefault.Brightness);  
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, capPropDefault.Contrast);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma, capPropDefault.Gamma);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt, capPropDefault.Tilt);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll, capPropDefault.Roll);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan, capPropDefault.Pan);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight, capPropDefault.Backlight);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom, capPropDefault.Zoom);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness, capPropDefault.Sharpness);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation, capPropDefault.Saturation);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue, capPropDefault.Hue);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain, capPropDefault.Gain);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, capPropDefault.Exposure);
                webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Temperature, capPropDefault.Temperature);
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }

        public void WebCamFace_ImageGrabbed(object sender, EventArgs e)
        {
            /*webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, 900);*/
 
            webCam.Retrieve(frameCapture);

            if (frameCapture != null)
            {


                Image<Bgr, Byte> img = frameCapture.ToImage<Bgr, byte>();


                var imageFrame = frameCapture.ToImage<Bgr, byte>();

                if (imageFrame != null)
                {
                    var bitmapImage = imageFrame.ToBitmap(imageFrame.Width, imageFrame.Height);
                    switch (camOrientation)
                    {

                        case 0:
                            /*nothing to do, webcam is set correctly*/
                            break;
                        case 90:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 180:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 270:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                            break;
                        default:
                            break;

                    }
                    Random rand = new Random();

                }


                if (imageFrame != null)
                {
                    var grayFrame = imageFrame.Convert<Gray, byte>();
                    var faceRecs = faceDetection.DetectMultiScale(grayFrame, 1.3, 5);
                    var eyes = eyeDetection.DetectMultiScale(grayFrame, 1.3, 5);

                    if (faceSquare)
                    {
                        foreach (var face in faceRecs)
                        {
                            imageFrame.Draw(face, new Bgr(System.Drawing.Color.BurlyWood), 3);
                            facesPresent = true;

                        }
                        if (faceRecs.Length > 0)
                        {
                            imageFrame.Save("FacialData/FaceLog.png");
                        }

                    }
                    if (!eyeSquare)
                    {
                        foreach (var eye in eyes)
                        {
                            imageFrame.Draw(eye, new Bgr(System.Drawing.Color.Yellow), 3);
                        }

                    }



                    /*
                    imageFrame._EqualizeHist();
                    var test= webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus);
                    imageFrame.Draw(test.ToString(),new Point(100,imageFrame.Height),Emgu.CV.CvEnum.FontFace.HersheyPlain,16,new Bgr(200,0,0));
                    */

                    displayImage = imageFrame.ToBitmap();
                    recognitionFrame = frameCapture;

                    if (facialRec == true)
                    {
                        if (faceRecognitionAttempts < maxFaceAttempts)
                        {
                            if (scanInProgress != true)
                            {
                                Task.Factory.StartNew(() => FacePrediction(userGuid, recognitionFrame.ToImage<Gray, byte>()));
                            }
                        }
                        else
                        {
                            rejected = true;
                        }

                    }
                    else if (faceTrain)
                    {

                        if( faces.Count()<scanLimit)
                        {
                            FaceTrainingTick(userGuid.ToString(), recognitionFrame);
                        }
                        else
                        {
                            faceTrain=!FaceTrain();
                        }

                    }


                }

            }

        }

        void ReInitalize()
        {
            facialRec = false;
            webCam = new VideoCapture(0, VideoCapture.API.DShow);

            try
            {
                frameCapture = new Mat();
                webCam.ImageGrabbed += WebCamBase_ImageGrabbed; 
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
            }
        }
        #region Payment Image Captue
        public bool PaymentCapture(int CamOrientation, string transactionguid)
        {
            VideoStart();

            webCam.Retrieve(frameCapture);

            var imageFrame = frameCapture.ToImage<Bgr, byte>();

            try
            {
                if (imageFrame != null)
                {
                    var bitmapImage = imageFrame.ToBitmap();
                    switch (CamOrientation)
                    {

                        case 0:
                            /*nothing to do, webcam is set correctly*/
                            break;
                        case 90:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 180:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 270:
                            bitmapImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                            break;
                        default:
                            break;

                    }

                    imageFrame = bitmapImage.ToImage<Bgr, Byte>();
                }

                if (imageFrame != null)
                {
                    var bitmapImage = imageFrame.ToBitmap();
                    /*var b2 = imageFrame.ToJpegData();*/
                    string captureFolder = CreatePaymentImageCaptureFolder();
                    /*public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams);*/

                    System.Drawing.Imaging.ImageCodecInfo myImageCodecInfo;
                    System.Drawing.Imaging.Encoder myEncoder;
                    System.Drawing.Imaging.EncoderParameter myEncoderParameter;
                    System.Drawing.Imaging.EncoderParameters myEncoderParameters;

                    myImageCodecInfo = GetEncoderInfo("image/png");

                    myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                    myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 25L);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    bitmapImage.Save(captureFolder + "\\" + transactionguid + ".png", myImageCodecInfo, myEncoderParameters);

                    return true;
                }

                return false;
            } 
            catch (Exception ex)
            {
                VideoStop();
                log.Error($"The video was stop. Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                return false;
            }

        }
        public static string CreatePaymentImageCaptureFolder()
        {
            string imageCaptureDirectory = AppDomain.CurrentDomain.BaseDirectory + "PaymentCapture\\" + DateTime.Now.ToString("MM.dd.yyyy");
            if (!System.IO.Directory.Exists(imageCaptureDirectory))
                System.IO.Directory.CreateDirectory(imageCaptureDirectory);
            return imageCaptureDirectory;

        }
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            System.Drawing.Imaging.ImageCodecInfo[] encoders;
            encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        #endregion
        #region VideoCapture
        void VideoStart()
        {
            webCam.Start();
            if (videoWriter != null)
            {
                videoWriter.Dispose();
            }
        }
        public async Task VideoStop()
        {
            webCam.Stop();

            webCam.Dispose();
            ReInitalize();
            if (videoWriter != null)
            {
                videoWriter.Dispose();
            }
            if (displayImage != null)
            {
                displayImage.Dispose();
            }


        }
        public string CameraVideo(string fileName = "")
        {
            bool initalized = false;

            if (fileName != "")
            {
                initalized = true;
                int fourcc = VideoWriter.Fourcc('H', '2', '6', '4');
                videoWriter = new VideoWriter(fileName, fourcc, 30, new System.Drawing.Size(webCam.Width, webCam.Height), true);
            }

            try
            {
                if (webCam.Retrieve(frameCapture))
                {
                    if (videoWriter != null && videoWriter.Ptr != new IntPtr(0x00000000))
                    {
                        videoWriter.Write(frameCapture);
                    }
                    else
                    {
                        int fourcc = VideoWriter.Fourcc('H', '2', '6', '4');
                        if (!Directory.Exists("Video"))
                        {
                            Directory.CreateDirectory("Video");
                        }

                        if (!initalized)
                        {
                            fileName = "Video\\UnAuthorizedPull" + DateTime.Now.ToString("d-MM-yyyy-HH-mm-ss") + ".mp4";
                        }


                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        videoWriter = new VideoWriter(fileName, fourcc, 30, new System.Drawing.Size(webCam.Width, webCam.Height), true);

                        videoWriter.Write(frameCapture);

                        initalized = true;

                    }

                }
            }
            catch (Exception ex)
            {
                fileName = "";
            }
            return fileName;
        }
        
        #endregion
        public void FaceCamStart(int CamOrientation)
        {
            webCam.ImageGrabbed += WebCamFace_ImageGrabbed;
            approved = false;
            rejected = false;
            faceTrain = false;
            TrainingInProgress = false;
            VideoStart();
            facesPresent = false;
            this.camOrientation = CamOrientation;
        }
        public void BaseCamStart(int CamOrientation)
        {
            approved = false;
            rejected = false;
            faceTrain = false;
            TrainingInProgress = false;
            webCam.ImageGrabbed += WebCamBase_ImageGrabbed;
            VideoStart();
            this.camOrientation = CamOrientation;
        }
        public bool FaceTrainingTick(string userGuid, Mat fCapture)
        {

            if (fCapture != null)
            {
                var imageFrame = fCapture.ToImage<Gray, byte>();


                try
                {
                    /*error is being thrown here, from the logs*/

                    if (imageFrame.Height != 0)
                    {
                        var bitmapImage = imageFrame.ToBitmap();

                        switch (camOrientation)
                        {

                            case 0:
                                /*nothing to do, webcam is set correctly*/
                                break;
                            case 90:
                                bitmapImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                break;
                            case 180:
                                bitmapImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;
                            case 270:
                                bitmapImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                                break;
                            default:
                                break;

                        }
                        imageFrame = bitmapImage.ToImage<Gray, byte>();
                    }
                }
                catch (Exception ex)
                {

                }


                if (imageFrame != null)
                {
                    var faceHolder = faceDetection.DetectMultiScale(imageFrame, 1.3, 5);


                    int centerX = imageFrame.Width / 2;
                    int centerY = imageFrame.Height / 2;
                    double shortestdistance = 3000;
                    System.Drawing.Rectangle closestToCenter = new System.Drawing.Rectangle();
                    int tag = 2;
                    string log = "";

                    foreach (var face in faceHolder)
                    {
                        var xCenter = (face.Location.X + face.Right) / 2;
                        var yCenter = (face.Location.Y + face.Bottom) / 2;

                        var distanceHolder = Math.Sqrt(Math.Pow((xCenter - centerX), 2) + Math.Pow((yCenter - centerY), 2));

                        if (distanceHolder < shortestdistance)
                        {
                            shortestdistance = distanceHolder;
                            closestToCenter = face;
                        }

                        log = log + tag.ToString() + ":" + distanceHolder.ToString() + "\n";
                        tag--;

                    }

                    File.WriteAllText("FacialData/TrainLog.txt", log);

                    if (faceHolder.Length > 0)
                    {
                        if (!Directory.Exists("FacialData/Faces/" + userGuid.ToString()))
                        {
                            Directory.CreateDirectory("FacialData/Faces/" + userGuid.ToString());
                        }

                        var processedImage = imageFrame.Copy(closestToCenter).Resize(processedImageWidth, processedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                        processedImage._EqualizeHist();
                        faces.Add(processedImage);


                        if (!Directory.Exists("FacialData/FaceModels"))
                        {
                            Directory.CreateDirectory("FacialData/FaceModels");
                        }

                        ID.Add(0);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    return false;
                }

                
                return false;
            }
            return false;

        }
        public bool FaceTrain()
        {
            if (TrainingInProgress == false)
            {
                TrainingInProgress = true;
                List<Mat> facesMat = new List<Mat>();
                VideoStop();

                string[] holder = default;
                bool hasSaltDir = false;

                if (Directory.Exists(saltDirectory))
                {
                    holder = Directory.GetFiles(saltDirectory);
                    hasSaltDir = true;

                    try
                    {
                        int holderLen = holder.Length;
                    }
                    catch (Exception ex)
                    {
                        hasSaltDir = false; 
                    }

                }
                else
                {
                    Directory.CreateDirectory(saltDirectory);
                }



                for (int i = 0; i < faces.Count(); i++)
                {
                    facesMat.Add(faces[i].Mat);
                }

                if (hasSaltDir)
                {
                    for (int i = 0; i < holder.Length; i++)
                    {

                        var load = new Mat(holder[i], Emgu.CV.CvEnum.ImreadModes.Grayscale);

                        var loadedImage = load.ToImage<Gray, byte>();
                        loadedImage._EqualizeHist();

                        var LoadedFaces = faceDetection.DetectMultiScale(load, 1.3, 5);

                        if (LoadedFaces.Length > 0)
                        {
                            var loadHolder = loadedImage.Copy(LoadedFaces[0]).Resize(processedImageWidth, processedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                            load = loadHolder.Mat;

                            facesMat.Add(load);
                            ID.Add(1);

                            if (!Directory.Exists(saltDirectory + "/Accepted"))
                            {
                                Directory.CreateDirectory(saltDirectory + "/Accepted");
                            }

                            var savePath = "FacialData\\SaltImages\\Accepted\\" + holder[i].Split('\\')[1];
                            loadedImage.Save(savePath);

                        }
                        else
                        {
                            if (!Directory.Exists(saltDirectory + "/Rejected"))
                            {
                                Directory.CreateDirectory(saltDirectory + "/Rejected");
                            }

                            var savePath = "FacialData\\SaltImages\\Rejected\\" + holder[i].Split('\\')[1];
                            loadedImage.Save(savePath);
                            File.Delete(holder[i]);
                            /* File.Copy(saltDirectory + "/" + holder[i].Split('\\')[1], saltDirectory + " /Rejected/" + holder[i].Split('\\')[1]);
                             */
                        }



                    }
                }

                var start = DateTime.UtcNow;
                var faceInput = facesMat.ToArray();
                var idInput = ID.ToArray();
                try
                {

                    var x = faceInput.ToArray();
                    faceRecogntion.Train(x, idInput);
                    var end = DateTime.UtcNow;
                    var result = DateTime.Compare(start, end);

                    if (!Directory.Exists("FacialData/FaceModels"))
                    {
                        Directory.CreateDirectory("FacialData/FaceModels");
                    }
                    faceRecogntion.Write(@"FacialData/FaceModels/" + userGuid.ToString() + ".yml");
                    TrainingInProgress = false;
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        public async Task FacePrediction(Guid userGuid, Image<Gray, byte> imageFrame)
        {
            scanInProgress = true;

            FaceRecognizer.PredictionResult prediction = new FaceRecognizer.PredictionResult();
            try
            {
                if (webCam.IsOpened)
                {

                    try
                    {
                        /*added per Ben 10282020*/

                        if (imageFrame.Height != 0)
                        {

                            var bitmapImage = imageFrame.ToBitmap();
                            switch (camOrientation)
                            {

                                case 0:
                                    //nothing to do, webcam is set correctly
                                    break;
                                case 90:
                                    bitmapImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                case 180:
                                    bitmapImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case 270:
                                    bitmapImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                                    break;
                                default:
                                    break;

                            }
                            imageFrame = bitmapImage.ToImage<Gray, byte>();
                        }
                        /*end added per Ben 10282020*/
                    }
                    catch (Exception ex)
                    { 
                        log.Error($"The video was stop. Error message {ex.Message} InnerException Message {ex.InnerException.Message}");           
                    }

                    var colorFrame = imageFrame.Convert<Gray, byte>();


                    List<FaceRecognizer.PredictionResult> Predictions = new List<FaceRecognizer.PredictionResult>();
                    imageFrame._EqualizeHist();

                    if (imageFrame != null)
                    {
                        
                        var faceHolder = faceDetection.DetectMultiScale(imageFrame, 1.3, 5);

                        int centerX = imageFrame.Width / 2;
                        int centerY = imageFrame.Height / 2;
                        double shortestdistance = 3000;
                        System.Drawing.Rectangle closestToCenter = new System.Drawing.Rectangle();


                        foreach (var face in faceHolder)
                        {
                            var xCenter = (face.Location.X + face.Right) / 2;
                            var yCenter = (face.Location.Y + face.Bottom) / 2;


                            var distanceHolder = Math.Sqrt(Math.Pow((xCenter - centerX), 2) + Math.Pow((yCenter - centerY), 2));

                            if (distanceHolder < shortestdistance)
                            {
                                shortestdistance = distanceHolder;
                                closestToCenter = face;
                            }
                            /*
                             log = log + "\n" + tag.ToString() + ":" + distanceHolder.ToString();
                             tag--;
                            */
                        }

                        var proccessedImage = imageFrame.Copy(closestToCenter).Resize(processedImageWidth, processedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                        var proccesdImageColor = colorFrame.Copy(closestToCenter).Resize(processedImageWidth, processedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                        if (faceHolder.Count() != 0 && approved == false)
                        {

                            SnapShot = proccessedImage.ToBitmap();

                            faceRecognitionAttempts++;

                            //proccessedImage._EqualizeHist();
                            //proccessedImage.Save("FacialData/Faces/TestFace.jpg");
                            var path = "FacialData\\FaceModels\\" + userGuid + ".yml";
                            if (File.Exists(path))
                            {

                                //Logging.WriteMessage("YML File found: " + path);

                                faceRecogntion.Read(path);
                                //var result = faceRecogntion.Predict(proccessedImage);
                                //var result;

                                //await Task.Run(() =>
                                //{
                                var result = faceRecogntion.Predict(proccessedImage);
                                prediction = result;



                            }
                        }
                        predictionDistance = prediction.Distance;
                        predictionLabel = prediction.Label;

                        if (prediction.Label == 0 && prediction.Distance > 6000)
                        {
                            approved = true;
                        }



                    }


                }
            }
            catch (Exception ex)
            {
                log.Error($"The video was stop. Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
            }
             
            scanInProgress = false;


        }


        public void clearFaces()
        {
            approved = false;
            rejected = false;
            faceTrain = false;
            TrainingInProgress = false;
            faces.Clear();
            ID.Clear();
            
            faceRecognitionAttempts = 0;

        }
        
        public async Task Stop()
        { 
            await VideoStop();
            clearFaces();
            displayImage = null;
        } 
        ~Camera()
        {
            CameraConfiguration.setConfigValue("Brightness", capPropCurrent.Brightness.ToString());
            CameraConfiguration.setConfigValue("Contrast", capPropCurrent.Contrast.ToString());
            CameraConfiguration.setConfigValue("Gamma", capPropCurrent.Gamma.ToString());
            CameraConfiguration.setConfigValue("Tilt", capPropCurrent.Tilt.ToString());
            CameraConfiguration.setConfigValue("Roll", capPropCurrent.Roll.ToString());
            CameraConfiguration.setConfigValue("Focus", capPropCurrent.Focus.ToString());
            CameraConfiguration.setConfigValue("Pan", capPropCurrent.Pan.ToString());
            CameraConfiguration.setConfigValue("Backlight", capPropCurrent.Backlight.ToString());
            CameraConfiguration.setConfigValue("Zoom", capPropCurrent.Zoom.ToString());
            CameraConfiguration.setConfigValue("Sharpness", capPropCurrent.Sharpness.ToString());
            CameraConfiguration.setConfigValue("Saturation", capPropCurrent.Saturation.ToString());
            CameraConfiguration.setConfigValue("Hue", capPropCurrent.Hue.ToString());
            CameraConfiguration.setConfigValue("Gain", capPropCurrent.Gain.ToString());
            CameraConfiguration.setConfigValue("Exposure", capPropCurrent.Exposure.ToString());
            CameraConfiguration.setConfigValue("Temperature", capPropCurrent.Temperature.ToString());
        }
    }
}
