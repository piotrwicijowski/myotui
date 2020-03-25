using System.ComponentModel;

namespace myotui.Models.Config
{
    public class SizeHint
    {
        [DefaultValue(SizeMode.Fill)]
        public SizeMode Mode {get; set;} = SizeMode.Fill;
        [DefaultValue(0)]
        public int Fixed {get; set;} = 0;
        [DefaultValue(1.0)]
        public double FillRatio {get; set;} = 1.0;
        [DefaultValue(0.0)]
        public double FillMinPercentage {get; set;} = 0.0;
        [DefaultValue(100.0)]
        public double FillMaxPercentage {get; set;} = 100.0;
    }
}