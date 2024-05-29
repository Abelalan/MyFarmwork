using System.Collections.Generic;

namespace Config
{
    public partial struct BuffConfig
    {
        private static Dictionary<int, ConfigBuffData> buffDatas;

        public static void ParseConfig()
        {
            UnityLog.Info("===>开始解析BuffConfig");

            buffDatas = new Dictionary<int, ConfigBuffData>();
            for (int i = 0; i < Count; i++)
            {
                ConfigBuffData buffData = new ConfigBuffData();

                BuffConfig buffConfig = ByIndex(i);
                if (buffConfig.MaxStack > buffConfig.TriggerProbability.Length && buffConfig.MaxStack != 1 && buffConfig.SameStackAction != BuffSameStackActionType.MultiInstance)
                {
                    UnityLog.Error($"解析Buff失败：Buff最大等级与触发概率分级不匹配。Buff ID:{buffConfig.ID}, MaxStack:{buffConfig.MaxStack}");
                }

                if (buffConfig.MaxStack > buffConfig.TriggerInterval.Length && buffConfig.MaxStack != 1 && buffConfig.SameStackAction != BuffSameStackActionType.MultiInstance)
                {
                    UnityLog.Error($"解析Buff失败：Buff最大等级与触发间隔分级不匹配。Buff ID:{buffConfig.ID}, MaxStack:{buffConfig.MaxStack}");
                }

                buffData.TriggerHits = new ConfigEffectData[buffConfig.TriggerHits.Length];
                for (int hitIndex = 0; hitIndex < buffConfig.TriggerHits.Length; hitIndex++)
                {
                    buffData.TriggerHits[hitIndex] = ConfigEffectData.ParseEffectData(buffConfig.SameStackAction == BuffSameStackActionType.MultiInstance ? 1 : buffConfig.MaxStack, buffConfig.TriggerHits[hitIndex].Effect);
                }
                buffDatas.Add(buffConfig.ID, buffData);
            }

            UnityLog.Info("<===解析BuffConfig完毕");
        }

        public static ConfigBuffData FindBuffData(int id)
        {
            buffDatas.TryGetValue(id, out ConfigBuffData data);
            return data;
        }
    }

    public class ConfigBuffData
    {
        public ConfigEffectData[] TriggerHits { get; internal set; }
    }
}
