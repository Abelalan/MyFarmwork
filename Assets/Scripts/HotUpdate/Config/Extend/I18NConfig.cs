using System.Collections.Generic;

namespace Config
{
    public partial struct I18NConfig
    {
        public static Dictionary<string, int> I18NKeyIndexMap;

        public static void ParseConfig()
        {
            I18NKeyIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < Count; i++)
            {
                I18NConfig config = ByIndex(i);
                I18NKeyIndexMap[config.key] = i;
            }
        }
    }
}
