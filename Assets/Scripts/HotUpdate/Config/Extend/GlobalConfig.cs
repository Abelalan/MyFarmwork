using System.Collections.Generic;

namespace Config
{
    public partial struct GlobalConfig
    {
        private static Dictionary<int, GlobalConfig> cache;

        private static void LoadCache()
        {
            if (cache != null)
                return;

            cache = new Dictionary<int, GlobalConfig>();
            for (int i = 0; i < Count; i++)
            {
                GlobalConfig config = ByIndex(i);
                int hash = config.Name.GetHashCode();
                cache.Add(hash, config);
            }
        }

        public static GlobalConfig ByName(string name)
        {
            int hash = name.GetHashCode();
            if (!cache.TryGetValue(hash, out GlobalConfig config))
            {
                UnityLog.Error($"找不到GlobalCache：{name}");
                return default;
            }

            return config;
        }


        public static void InitGlobalConfig()
        {
            LoadCache();
        }
    }
}
