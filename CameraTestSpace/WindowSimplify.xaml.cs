using CameraSystem;
using DynaTouch.CameraSystem;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

namespace CameraTestSpace
{
    /// <summary>
    /// Interaction logic for WindowSimplify.xaml
    /// </summary>
    public partial class WindowSimplify : Window
    {
        Settings settings;
        int curSettingsIndex=0;
        double curSettingsValue = 0;
        string curSettingsName = "";
        DispatcherTimer displayUpdate;// Updates Display Every Tick
        private Camera webCam;// The Camera Object Being Used
        public bool wasConfigReaded = false;
        private static readonly ILog log = LogManager.GetLogger(typeof(Camera));

        public WindowSimplify()
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

         
         

        private void btnPrevius_Click(object sender, RoutedEventArgs e)
        {
            if (0 == curSettingsIndex)
            {
                return;
            }

            LoadScreenSettings(-1);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (settings.SettingsItem.Count() - 1 == curSettingsIndex)
            {
                return;
            }
            LoadScreenSettings(1);
        }

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var slSettingValue = Convert.ToDouble(this.txbSetting.Text);
            Task.Factory.StartNew(() => webCam.SetSettingValue(curSettingsName, slSettingValue));
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            webCam.RestoreSettingsValues();
            curSettingsValue = webCam.GetDefaultSetting(curSettingsName);
            txbSetting.Text = Convert.ToString(curSettingsValue);
        }
  

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadXMLSettings();
        }

        private void LoadXMLSettings()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            string strSettingsXmlFilePath = System.IO.Path.Combine(strWorkPath, "CameraSettings.xml");

            var xmlContent = File.ReadAllText(strSettingsXmlFilePath);

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            settings = serializer.Deserialize(new StringReader(xmlContent)) as Settings;
            
            LoadScreenSettings(0);
        }
        private void LoadScreenSettings(int move)
        {
            curSettingsIndex += move;
            
            if (settings.SettingsItem.Count() > 0)
            {
                this.lblSetting.Content = settings.SettingsItem[curSettingsIndex].Name;
                curSettingsName = settings.SettingsItem[curSettingsIndex].Name;

                this.slSetting.Maximum = Convert.ToDouble(settings.SettingsItem[curSettingsIndex].Max);
                this.slSetting.Minimum = Convert.ToDouble(settings.SettingsItem[curSettingsIndex].Min);
                
                curSettingsValue = webCam.GetCurrentSetting(curSettingsName);
                txbSetting.Text = Convert.ToString(curSettingsValue);
            }
        }

        private void slSetting_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {  
            try
            {
                var slSettingValue = this.slSetting.Value; 
                Task.Factory.StartNew(() => webCam.SetSettingValue(curSettingsName, slSettingValue));
            }
            catch (Exception ex)
            {
                log.Error($"Error message: {ex.Message}. InnerException Message: {ex.InnerException.Message}.");
                throw;
            } 
        }

        private void slSetting_Loaded(object sender, RoutedEventArgs e)
        {
            this.slSetting.Value = curSettingsValue;
        }

        
    }
}
