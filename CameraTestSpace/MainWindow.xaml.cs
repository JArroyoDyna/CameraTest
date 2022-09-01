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
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void slGamma_ValueChanged(object sender, EventArgs e) { try { webCam.gamma = slGamma.Value; } catch (Exception) { throw; } }
        private void slTilt_ValueChanged(object sender, EventArgs e) { try { webCam.brightness = slTilt.Value; } catch (Exception) { throw; } }
        private void slRoll_ValueChanged(object sender, EventArgs e) { try { webCam.brightness = slRoll.Value; } catch (Exception) { throw; } }
        private void slPan_ValueChanged(object sender, EventArgs e) { try { webCam.pan = slPan.Value; } catch (Exception) { throw; } }
        private void slBacklight_ValueChanged(object sender, EventArgs e) { try { webCam.backlight = slBacklight.Value; } catch (Exception) { throw; } }
        private void slZoom_ValueChanged(object sender, EventArgs e) { try { webCam.zoom = slZoom.Value; } catch (Exception) { throw; } }
        private void slTemperature_ValueChanged(object sender, EventArgs e) { try { webCam.temperature = slTemperature.Value; } catch (Exception) { throw; } }
        private void slSharpness_ValueChanged(object sender, EventArgs e) { try { webCam.sharpness = slSharpness.Value; } catch (Exception) { throw; } }
        private void slSaturation_ValueChanged(object sender, EventArgs e) { try { webCam.saturation = slSaturation.Value; } catch (Exception) { throw; } }
        private void slHue_ValueChanged(object sender, EventArgs e) { try { webCam.hue = slHue.Value; } catch (Exception) { throw; } }       
        private void slFocus_ValueChanged(object sender, EventArgs e) { try { webCam.focus = slFocus.Value; } catch (Exception) { throw; } }
        private void slGain_ValueChanged(object sender, EventArgs e) { try { webCam.gain = slGain.Value; } catch (Exception) { throw; } }
        
    }
}
