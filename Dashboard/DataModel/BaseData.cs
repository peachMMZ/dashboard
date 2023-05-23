using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard
{
    class BaseData
    {
        public BaseData(float data)
        {
            this.Data = data;
        }
        private float data = 0f;
        public float Data 
        { 
            get { return data; } 
            set 
            { 
                data = value;
                if (Max == 0f)
                {
                    Max = data;
                }
                if (Min == 0f)
                {
                    Min = data;
                }
                if (data > Max)
                {
                    Max = data;
                }
                if (data < Min)
                {
                    Min = data;
                }
                Average = (Average + data) / 2;
            } 
        }
        public float Max { get; set; }
        public float Min { get; set; }
        public float Average { get; set; } = 0f;

        public void Reset()
        {
            this.Reset(0f);
        }

        public void Reset(float data)
        {
            this.data = data;
            this.Max = data;
            this.Min = data;
            this.Average = data;
        }

        public string Format(string name, string unit)
        {
            return $"<size=24>{name}</size>\n" +
                $"<size=18><color=yellow>{LanguageManager.Instance.CurrentLanguage.Current}: {Data:0.00}{unit}</color></size>\t" +
                $"<size=18><color=red>{LanguageManager.Instance.CurrentLanguage.Max}: {Max:0.00}</color></size>\t" +
                $"<size=18><color=aqua>{LanguageManager.Instance.CurrentLanguage.Min}: {Min:0.00}</color></size>\t" +
                $"<size=18><color=lime>{LanguageManager.Instance.CurrentLanguage.Avg}: {Average:0.00}</color></size>";
        }

        public string FormatData()
        {
            return $"<size=18><color=yellow>{Data:0.00}</color></size>";
        }

        public string FormatMax()
        {
            return $"<size=18><color=red>{Max:0.00}</color></size>";
        }

        public string FormatMin()
        {
            return $"<size=18><color=aqua>{Min:0.00}</color></size>";
        }

        public string FormatAvg()
        {
            return $"<size=18><color=lime>{Average:0.00}</color></size>";
        }
    }
}
