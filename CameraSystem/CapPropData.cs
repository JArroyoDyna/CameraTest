using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTestSpace
{
    public class CapPropData
    {
        public double Focus { get; set; } = -1;
        public double Brightness { get; set; } = 25;
        public double Contrast { get; set; }
        public double Gamma { get; set; } = 100;
        public double Tilt { get; set; } = -1;
        public double Roll { get; set; } = -1;
        public double Pan { get; set; } = -1;
        public double Backlight { get; set; } = 3;
        public double Zoom { get; set; } = -1;
        public double Sharpness { get; set; } = 2;
        public double Temperature { get; set; } = 2;
        public double Saturation { get; set; } = 64;
        public double Hue { get; set; } = 0;
        public double Gain { get; set; } = 1;
        public double Exposure { get; set; } = -6;
    }
}

