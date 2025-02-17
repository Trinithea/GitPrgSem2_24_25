using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lidl2048.Model
{
    internal class MyColors
    {
        private int _baseNumber {  get; set; }
        public Dictionary<int, string> ColorList = new Dictionary<int, string>
        {
            { 0, "#97989c"},
            { 2, "#EEE4DA" },
            { 4, "#EDE0C8" },
            { 8, "#F2B179" },
            { 16, "#F59563" },
            { 32, "#F67C5F" },
            { 64, "#F65E3B" },
            { 128, "#EDCF72" },
            { 256, "#EDCC61" },
            { 512, "#EDC850" },
            { 1024, "#EDC53F" },
            { 2048, "#EDC22E" }
        };

        public MyColors(int b)
        {
            _baseNumber = b;
            ColorList = new Dictionary<int, string>
            {
                { 0, "#97989c"},
                { b, "#EEE4DA" },
                { b * (int)Math.Pow(2, 1), "#EDE0C8" },
                { b * (int)Math.Pow(2, 2), "#F2B179" },
                { b * (int)Math.Pow(2, 3), "#F59563" },
                { b * (int)Math.Pow(2, 4), "#F67C5F" },
                { b * (int)Math.Pow(2, 5), "#F65E3B" },
                { b * (int)Math.Pow(2, 6), "#EDCF72" },
                { b * (int)Math.Pow(2, 7), "#EDCC61" },
                { b * (int)Math.Pow(2, 8), "#EDC850" },
                { b * (int)Math.Pow(2, 9), "#EDC53F" },
                { b * (int)Math.Pow(2, 10), "#EDC22E" }
            };
        }
    }
}
