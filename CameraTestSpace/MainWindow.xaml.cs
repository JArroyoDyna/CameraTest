using CameraSystem;
using DynaTouch.CameraSystem;
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

                    //throw;
                }

            }

        }
        private void slBrightness_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.brightness = slBrightness.Value;
                webCam.SetSettingsValues();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void slContrast_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webCam.contrast = slContrast.Value;
                webCam.SetSettingsValues();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void slGamma_ValueChanged(object sender, EventArgs e) { try { webCam.gamma = slGamma.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slTilt_ValueChanged(object sender, EventArgs e) { try { webCam.brightness = slTilt.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slRoll_ValueChanged(object sender, EventArgs e) { try { webCam.brightness = slRoll.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slPan_ValueChanged(object sender, EventArgs e) { try { webCam.pan = slPan.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slBacklight_ValueChanged(object sender, EventArgs e) { try { webCam.backlight = slBacklight.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slZoom_ValueChanged(object sender, EventArgs e) { try { webCam.zoom = slZoom.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slTemperature_ValueChanged(object sender, EventArgs e) { try { webCam.temperature = slTemperature.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slSharpness_ValueChanged(object sender, EventArgs e) { try { webCam.sharpness = slSharpness.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slSaturation_ValueChanged(object sender, EventArgs e) { try { webCam.saturation = slSaturation.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slHue_ValueChanged(object sender, EventArgs e) { try { webCam.hue = slHue.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slFocus_ValueChanged(object sender, EventArgs e) { try { webCam.focus = slFocus.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slGain_ValueChanged(object sender, EventArgs e) { try { webCam.gain = slGain.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }
        private void slExposure_ValueChanged(object sender, EventArgs e) { try { webCam.exposure = slExposure.Value; webCam.SetSettingsValues(); } catch (Exception) { throw; } }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!wasConfigReaded)
            { 
                slBrightness.Value = CameraConfiguration.getConfigValue("Brightness") == null ? webCam.brightness : Convert.ToDouble(CameraConfiguration.getConfigValue("Brightness"));
                slContrast.Value = CameraConfiguration.getConfigValue("Contrast") == null ? webCam.contrast : Convert.ToDouble(CameraConfiguration.getConfigValue("Contrast"));
                slGamma.Value = CameraConfiguration.getConfigValue("Gamma") == null ? webCam.gamma : Convert.ToDouble(CameraConfiguration.getConfigValue("Gamma"));
                slTilt.Value = CameraConfiguration.getConfigValue("Tilt") == null ? webCam.tilt : Convert.ToDouble(CameraConfiguration.getConfigValue("Tilt"));
                slRoll.Value = CameraConfiguration.getConfigValue("Roll") == null ? webCam.roll : Convert.ToDouble(CameraConfiguration.getConfigValue("Roll"));
                slFocus.Value = CameraConfiguration.getConfigValue("Focus") == null ? webCam.focus : Convert.ToDouble(CameraConfiguration.getConfigValue("Focus"));
                slPan.Value = CameraConfiguration.getConfigValue("Pan") == null ? webCam.focus : Convert.ToDouble(CameraConfiguration.getConfigValue("Pan"));
                slBacklight.Value = CameraConfiguration.getConfigValue("Pan") == null ? webCam.backlight : Convert.ToDouble(CameraConfiguration.getConfigValue("Backlight"));
                slZoom.Value = CameraConfiguration.getConfigValue("Zoom") == null ? webCam.zoom : Convert.ToDouble(CameraConfiguration.getConfigValue("Zoom"));
                slSharpness.Value = CameraConfiguration.getConfigValue("Sharpness") == null ? webCam.sharpness : Convert.ToDouble(CameraConfiguration.getConfigValue("Sharpness"));
                slSaturation.Value = CameraConfiguration.getConfigValue("Saturation") == null ? webCam.saturation : Convert.ToDouble(CameraConfiguration.getConfigValue("Saturation"));
                slHue.Value = CameraConfiguration.getConfigValue("Hue") == null ? webCam.hue : Convert.ToDouble(CameraConfiguration.getConfigValue("Hue"));
                slGain.Value = CameraConfiguration.getConfigValue("Gain") == null ? webCam.gain : Convert.ToDouble(CameraConfiguration.getConfigValue("gain"));
                slExposure.Value = CameraConfiguration.getConfigValue("Exposure") == null ? webCam.exposure : Convert.ToDouble(CameraConfiguration.getConfigValue("Exposure"));

                wasConfigReaded = true;
            }
        }
    }
}
