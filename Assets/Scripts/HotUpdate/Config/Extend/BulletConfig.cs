using System.Collections.Generic;

namespace Config
{
    public partial struct BulletConfig
    {
        private static Dictionary<int, ConfigBulletData> bulletDatas;

        public static void ParseConfig()
        {
            bulletDatas = new Dictionary<int, ConfigBulletData>();

            for (int i = 0; i < Count; i++)
            {
                BulletConfig bulletConfig = ByIndex(i);
                ConfigBulletData configBulletData = new ConfigBulletData();
                configBulletData.CollideUnitEffectDatas = new ConfigEffectData[bulletConfig.CollideUnitEffects.Length];
                for (int effectIndex = 0; effectIndex < bulletConfig.CollideUnitEffects.Length; effectIndex++)
                {
                    configBulletData.CollideUnitEffectDatas[effectIndex] = ConfigEffectData.ParseEffectData(1, bulletConfig.CollideUnitEffects[effectIndex]);
                }
                configBulletData.CollideTerrainEffectDatas = new ConfigEffectData[bulletConfig.CollideTerrainEffects.Length];
                for (int effectIndex = 0; effectIndex < bulletConfig.CollideTerrainEffects.Length; effectIndex++)
                {
                    configBulletData.CollideTerrainEffectDatas[effectIndex] = ConfigEffectData.ParseEffectData(1, bulletConfig.CollideTerrainEffects[effectIndex]);
                }
                bulletDatas.Add(bulletConfig.ID, configBulletData);
            }
        }

        public static ConfigBulletData FindBulletData(int id)
        {
            bulletDatas.TryGetValue(id, out ConfigBulletData data);
            return data;
        }
    }

    public class ConfigBulletData
    {
        public ConfigEffectData[] CollideUnitEffectDatas { get; set; }
        public ConfigEffectData[] CollideTerrainEffectDatas { get; set; }
    }
}
