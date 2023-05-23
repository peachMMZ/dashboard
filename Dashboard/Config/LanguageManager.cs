using Localisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard
{
    public class LanguageManager : SingleInstance<LanguageManager>
    {
        public override string Name { get; } = "Language Manager";

        public Action<string> OnLanguageChanged;

        private string currentLanguageName;
        private string lastLanguageName = "English";

        public ILanguage CurrentLanguage { get; private set; } = new English();
        Dictionary<string, ILanguage> Dic_Language = new Dictionary<string, ILanguage>
        {
            {"简体中文", new Chinese() },
            {"English", new English() }
        };
 
        void Awake()
        {
            OnLanguageChanged += ChangeLanguage;
        }

        void Update()
        {
            currentLanguageName = LocalisationManager.Instance.currLangName;
            if (!lastLanguageName.Equals(currentLanguageName))
            {
                lastLanguageName = currentLanguageName;
                OnLanguageChanged.Invoke(currentLanguageName);
            }
        }

        void ChangeLanguage(string value)
        {
            try
            {
                CurrentLanguage = Dic_Language[value];
            }
            catch
            {
                CurrentLanguage = Dic_Language["English"];
            }
        }
    }

    public interface ILanguage
    {
        // 测量项
        string Speed { get; }
        string Acceleration { get; }
        string Palstance { get; }

        // 数据项
        string Current { get; }
        string Max { get; }
        string Min { get; }
        string Avg { get; }
    }

    public class Chinese : ILanguage
    {
        public string Speed => "速度";

        public string Acceleration => "加速度";

        public string Palstance => "转速";

        public string Current => "实时值";

        public string Max => "最大值";

        public string Min => "最小值";

        public string Avg => "平均值";
    }

    public class English : ILanguage
    {
        public string Speed => "Speed";

        public string Acceleration => "Acceleration";

        public string Palstance => "Palstance";

        public string Current => "Current";

        public string Max => "Max";

        public string Min => "Min";

        public string Avg => "Avg";
    }
}
