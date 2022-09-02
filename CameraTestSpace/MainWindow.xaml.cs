using CameraSystem;
using DynaTouch.CameraSystem;
using log4net;
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
using System.Windows.Threading;


namespace CameraTestSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer displayUpdate;// Updates Display Every Tick
        private Camera webCam;// The Camera Object Being Used
        public bool wasConfigReaded = false;
        private static readonly ILog log = LogManager.GetLogger(typeof(Camera));

        public MainWindow()
        {
            webCam = new Camera();



            InitializeComponent();

            displayUpdate = new DispatcherTimer();
            displayUpdate.Interval = TimeSpan.FromSeconds(.02);
            displayUpdate.Tick += DisplayUpdate_Tick;



            beginCam();


        }
        private void DisplayUpdate_Tick(object sender, EventArgs e)
        {
            SetDisplayImage();
        
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void beginCam()
        {
            /// 4 values can be set, 0, 90 180 270 based on the clockwise rotation from horizontal plane
            webCam.BaseCamStart(0);
            displayUpdate.Start();
            Display.Visibility = Visibility.Visible;

        }

        private void SetDisplayImage()
        {
            if (webCam.displayImage != null)
            {


                try
                {
                    IntPtr hBitmap = webCam.displayImage.GetHbitmap();
                    var bitMapHolder = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    Display.Source = bitMapHolder;

                    DeleteObject(hBitmap);
                }
                catch (Exception ex)
                {
                    log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                    //throw;
                }

            }

        }
        private void slBrightness_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Brightness = slBrightness.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slContrast_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Contrast = slContrast.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());

            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slGamma_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Gamma = slGamma.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slTilt_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Brightness = slTilt.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slRoll_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Roll = slRoll.Value;
                Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slPan_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Pan = slPan.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slBacklight_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Backlight = slBacklight.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slZoom_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Zoom = slZoom.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slTemperature_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Temperature = slTemperature.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slSharpness_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Sharpness = slSharpness.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slSaturation_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Saturation = slSaturation.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slHue_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Hue = slHue.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }

        private void slFocus_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Focus = slFocus.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slGain_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Gain = slGain.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }
        private void slExposure_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.capPropCurrent.Exposure = slExposure.Value;
                _ = Task.Factory.StartNew(() => webCam.SetSettingsValues());
            }
            catch (Exception ex)
            {
                log.Error($"Error message {ex.Message} InnerException Message {ex.InnerException.Message}");
                throw;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!wasConfigReaded)
            {
                slBrightness.Value = CameraConfiguration.getConfigValue("Brightness") == null ? webCam.capPropCurrent.Brightness : Convert.ToDouble(CameraConfiguration.getConfigValue("Brightness"));
                slContrast.Value = CameraConfiguration.getConfigValue("Contrast") == null ? webCam.capPropCurrent.Contrast : Convert.ToDouble(CameraConfiguration.getConfigValue("Contrast"));
                slGamma.Value = CameraConfiguration.getConfigValue("Gamma") == null ? webCam.capPropCurrent.Gamma : Convert.ToDouble(CameraConfiguration.getConfigValue("Gamma"));
                slTilt.Value = CameraConfiguration.getConfigValue("Tilt") == null ? webCam.capPropCurrent.Tilt : Convert.ToDouble(CameraConfiguration.getConfigValue("Tilt"));
                slRoll.Value = CameraConfiguration.getConfigValue("Roll") == null ? webCam.capPropCurrent.Roll : Convert.ToDouble(CameraConfiguration.getConfigValue("Roll"));
                slFocus.Value = CameraConfiguration.getConfigValue("Focus") == null ? webCam.capPropCurrent.Focus : Convert.ToDouble(CameraConfiguration.getConfigValue("Focus"));
                slPan.Value = CameraConfiguration.getConfigValue("Pan") == null ? webCam.capPropCurrent.Focus : Convert.ToDouble(CameraConfiguration.getConfigValue("Pan"));
                slBacklight.Value = CameraConfiguration.getConfigValue("Pan") == null ? webCam.capPropCurrent.Backlight : Convert.ToDouble(CameraConfiguration.getConfigValue("Backlight"));
                slZoom.Value = CameraConfiguration.getConfigValue("Zoom") == null ? webCam.capPropCurrent.Zoom : Convert.ToDouble(CameraConfiguration.getConfigValue("Zoom"));
                slSharpness.Value = CameraConfiguration.getConfigValue("Sharpness") == null ? webCam.capPropCurrent.Sharpness : Convert.ToDouble(CameraConfiguration.getConfigValue("Sharpness"));
                slSaturation.Value = CameraConfiguration.getConfigValue("Saturation") == null ? webCam.capPropCurrent.Saturation : Convert.ToDouble(CameraConfiguration.getConfigValue("Saturation"));
                slHue.Value = CameraConfiguration.getConfigValue("Hue") == null ? webCam.capPropCurrent.Hue : Convert.ToDouble(CameraConfiguration.getConfigValue("Hue"));
                slGain.Value = CameraConfiguration.getConfigValue("Gain") == null ? webCam.capPropCurrent.Gain : Convert.ToDouble(CameraConfiguration.getConfigValue("gain"));
                slExposure.Value = CameraConfiguration.getConfigValue("Exposure") == null ? webCam.capPropCurrent.Exposure : Convert.ToDouble(CameraConfiguration.getConfigValue("Exposure"));
                slTemperature.Value = CameraConfiguration.getConfigValue("Temperature") == null ? webCam.capPropCurrent.Temperature : Convert.ToDouble(CameraConfiguration.getConfigValue("Temperature"));
                wasConfigReaded = true;
            }
        }

        private void txtResetOnClick(object sender, RoutedEventArgs e)
        {
            slBrightness.Value = webCam.capPropDefault.Brightness;
            slContrast.Value = webCam.capPropDefault.Contrast;
            slGamma.Value = webCam.capPropDefault.Gamma;
            slTilt.Value = webCam.capPropDefault.Tilt;
            slRoll.Value = webCam.capPropDefault.Roll;
            slFocus.Value = webCam.capPropDefault.Focus;
            slPan.Value = webCam.capPropDefault.Pan;
            slBacklight.Value = webCam.capPropDefault.Backlight;
            slZoom.Value = webCam.capPropDefault.Zoom;
            slSharpness.Value = webCam.capPropDefault.Sharpness;
            slSaturation.Value = webCam.capPropDefault.Saturation;
            slHue.Value = webCam.capPropDefault.Hue;
            slGain.Value = webCam.capPropDefault.Gain;
            slExposure.Value = webCam.capPropDefault.Exposure;
            slTemperature.Value = webCam.capPropDefault.Temperature;

        }
    }
}

