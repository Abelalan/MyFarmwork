using System.Collections.Generic;

namespace Config
{
    public partial struct SpellFieldConfig
    {
        private static Dictionary<int, ConfigSpellFieldData> spellFieldDatas;

        public static void ParseConfig()
        {
            spellFieldDatas = new Dictionary<int, ConfigSpellFieldData>();

            for (int i = 0; i < Count; i++)
            {
                SpellFieldConfig spellFieldConfig = ByIndex(i);
                ConfigSpellFieldData configSpellFieldData = new ConfigSpellFieldData();
                configSpellFieldData.EffectDatas = new ConfigEffectData[spellFieldConfig.Effects.Length];
                for (int effectIndex = 0; effectIndex < spellFieldConfig.Effects.Length; effectIndex++)
                {
                    configSpellFieldData.EffectDatas[effectIndex] = ConfigEffectData.ParseEffectData(1, spellFieldConfig.Effects[effectIndex]);
                }
                spellFieldDatas.Add(spellFieldConfig.ID, configSpellFieldData);
            }
        }

        public static ConfigSpellFieldData FindConfigSpellFieldData(int id)
        {
            spellFieldDatas.TryGetValue(id, out ConfigSpellFieldData data);
            return data;
        }
    }

    public class ConfigSpellFieldData
    {
        public ConfigEffectData[] EffectDatas { get; set; }
    }
}
