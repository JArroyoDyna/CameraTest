using CameraSystem;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
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

        public double openniFocalLength = -1;
        public double androidFocalLength = -1;
        public double xiLensFocalLength = -1;
        public double openniDepthGeneratorFocalLength = -1;
        public double focus = -1;

        public double brightness = 25; 
        public double contrast = 0;
        public double gamma = 100;
        public double tilt = -1;
        public double roll = -1;
        public double pan = -1;
        public double backlight = 3;
        public double zoom = -1;
        public double sharpness = 2;
        public double temperature = 2;
        public double saturation = 64;
        public double hue = 0;
        public double gain = 1;
        public double exposure= -6;

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

        

        //public int focalLength;

        public Camera()
        { 
            faceRecogntion = new EigenFaceRecognizer(30, double.PositiveInfinity);

            faceDetection = new CascadeClassifier((@"FacialData/haarcascade_frontalface_default.xml"));
            eyeDetection = new CascadeClassifier(("FacialData/haarcascade_eye.xml"));
            faces = new List<Image<Gray, byte>>();
            ID = new List<int>();
             
            webCam = new VideoCapture(0, VideoCapture.API.DShow);// creation of a VideoCapture Object
    
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

            }

            TrainingInProgress = false;

        }  
        private void WebCamBase_ImageGrabbed(object sender, EventArgs e)
        {
            #region CapturePropertiesForBaseCamera




            //SetCameraSize(450, 800);

            #endregion


            // Any Changes to captures properties have to be made before webCam.Retrive Is Called
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

                    imageFrame = bitmapImage.ToImage<Bgr, byte>();
                    displayImage = bitmapImage;
                    //Random rand = new Random();
                    //bitmapImage.Save(rand.Next(1, 90000).ToString() + ".png");
                }
            }

        }

        public void SetSettingsValues()
        {


            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, focus);// Sets the Focus property to the Value to the value passed to it
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, brightness); // Sets the Brightness property to the value passed to it
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, contrast);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma, gamma);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Tilt, tilt);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Roll, roll);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Pan, pan);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Backlight, backlight);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Zoom, zoom);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Sharpness, sharpness);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Saturation, saturation);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Hue, hue);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gain, gain);
            webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, exposure);


        }

        public void WebCamFace_ImageGrabbed(object sender, EventArgs e)
        {
            //webCam.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, 900);
 
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



                    //imageFrame._EqualizeHist();

                    //var test= webCam.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus);
                    //imageFrame.Draw(test.ToString(),new Point(100,imageFrame.Height),Emgu.CV.CvEnum.FontFace.HersheyPlain,16,new Bgr(200,0,0));
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

                    imageFrame = bitmapImage.ToImage<Bgr, Byte>();
                }

                if (imageFrame != null)
                {
                    var bitmapImage = imageFrame.ToBitmap();
                    //var b2 = imageFrame.ToJpegData();
                    string captureFolder = CreatePaymentImageCaptureFolder();
                    //public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams);

                    System.Drawing.Imaging.ImageCodecInfo myImageCodecInfo;
                    System.Drawing.Imaging.Encoder myEncoder;
                    System.Drawing.Imaging.EncoderParameter myEncoderParameter;
                    System.Drawing.Imaging.EncoderParameters myEncoderParameters;

                    myImageCodecInfo = GetEncoderInfo("image/png");

                    myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                    myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 25L);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    //bitmapImage.Save(captureFolder +"\\" + PayUtilityNowKioskUser.SessionGuid.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    bitmapImage.Save(captureFolder + "\\" + transactionguid + ".png", myImageCodecInfo, myEncoderParameters);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                VideoStop();
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
                    //error is being thrown here, from the logs

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
                        //throw;
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
                            //File.Copy(saltDirectory + "/" + holder[i].Split('\\')[1], saltDirectory + " /Rejected/" + holder[i].Split('\\')[1]);
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
                        //added per Ben 10282020

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
                        //end added per Ben 10282020
                    }
                    catch (Exception ex)
                    {

                        //throw;
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
                            //log = log + "\n" + tag.ToString() + ":" + distanceHolder.ToString();
                            //tag--;

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
            CameraConfiguration.setConfigValue("Brightness", brightness.ToString());
            CameraConfiguration.setConfigValue("Contrast", contrast.ToString());
            CameraConfiguration.setConfigValue("Gamma", gamma.ToString());
            CameraConfiguration.setConfigValue("Tilt", tilt.ToString());
            CameraConfiguration.setConfigValue("Roll", roll.ToString());
            CameraConfiguration.setConfigValue("Focus", focus.ToString());
            CameraConfiguration.setConfigValue("Pan", pan.ToString());
            CameraConfiguration.setConfigValue("Backlight", backlight.ToString());
            CameraConfiguration.setConfigValue("Zoom", zoom.ToString());
            CameraConfiguration.setConfigValue("Sharpness", sharpness.ToString());
            CameraConfiguration.setConfigValue("Saturation", saturation.ToString());
            CameraConfiguration.setConfigValue("Hue", hue.ToString());
            CameraConfiguration.setConfigValue("Gain", gain.ToString());
            CameraConfiguration.setConfigValue("Exposure", exposure.ToString());
        } 
    }
}
