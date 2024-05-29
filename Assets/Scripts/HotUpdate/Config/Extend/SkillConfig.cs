using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Config
{
    public partial struct SkillConfig
    {
        private static Dictionary<int, ConfigSkillData> skillDatas;

        public static void ParseConfig()
        {
            UnityLog.Info("===>开始解析SkillConfig");

            skillDatas = new Dictionary<int, ConfigSkillData>();
            for (int i = 0; i < Count; i++)
            {
                SkillConfig skillConfig = ByIndex(i);

                ConfigSkillData skillData = new ConfigSkillData
                {
                    DetectHitData = new ConfigSkillDetectHitData[skillConfig.DetectHits.Length]
                };
                for (int detectHitIndex = 0; detectHitIndex < skillConfig.DetectHits.Length; detectHitIndex++)
                {
                    SkillHitData detectHit = skillConfig.DetectHits[detectHitIndex];
                    ConfigSkillDetectHitData hitData = new ConfigSkillDetectHitData();
                    hitData.EffectData = new ConfigEffectData[detectHit.Effects.Length];
                    for (int effectIndex = 0; effectIndex < detectHit.Effects.Length; effectIndex++)
                    {
                        hitData.EffectData[effectIndex] = ConfigEffectData.ParseEffectData(1, detectHit.Effects[effectIndex]);
                    }
                    skillData.DetectHitData[detectHitIndex] = hitData;
                }
                skillData.FXData = ParseFX(skillConfig.ID, skillConfig.FX);
                skillData.SFXData = ParseSFX(skillConfig.ID, skillConfig.SFX);
                skillDatas.Add(skillConfig.ID, skillData);
            }

            UnityLog.Info("<===解析SkillConfig完毕");
        }

        private static List<ConfigSkillFXItem> ParseFX(int id, string fx)
        {
            if (string.IsNullOrEmpty(fx))
                return null;

            List<ConfigSkillFXItem> fxList = new List<ConfigSkillFXItem>();
            Regex regex = new Regex(@"\[(\d|;|\.|-)*\]");
            MatchCollection matches = regex.Matches(fx);
            if (matches.Count == 0)
            {
                UnityLog.Error($"解析FX错误，请检查SkillConfig，ID:{id}, FX:{fx}");
                return null;
            }

            foreach (Match match in matches)
            {
                string content = match.Value.Substring(1, match.Value.Length - 2);
                string[] values = content.Split(';');
                ConfigSkillFXItem item = new ConfigSkillFXItem()
                {
                    ID = int.Parse(values[0]),
                    Delay = float.Parse(values[1]),
                };
                fxList.Add(item);
            }
            if (fxList.Count > 0)
            {
                fxList.Sort((a, b) =>
                {
                    return a.Delay.CompareTo(b.Delay);
                });
            }
            return fxList;
        }

        private static List<ConfigSkillSFXItem> ParseSFX(int id, string sfx)
        {
            if (string.IsNullOrEmpty(sfx))
                return null;

            List<ConfigSkillSFXItem> sfxList = new List<ConfigSkillSFXItem>();
            Regex regex = new Regex(@"\[(\d|;|\.|-)*\]");
            MatchCollection matches = regex.Matches(sfx);
            if (matches.Count == 0)
            {
                UnityLog.Error($"解析SFX错误，请检查SkillConfig，ID:{id}, SFX:{sfx}");
                return null;
            }

            foreach (Match match in matches)
            {
                string content = match.Value.Substring(1, match.Value.Length - 2);
                string[] values = content.Split(';');
                ConfigSkillSFXItem item = new ConfigSkillSFXItem()
                {
                    ID = int.Parse(values[0]),
                    Delay = float.Parse(values[1]),
                };
                sfxList.Add(item);
            }
            if (sfxList.Count > 0)
            {
                sfxList.Sort((a, b) =>
                {
                    return a.Delay.CompareTo(b.Delay);
                });
            }
            return sfxList;
        }

        public static ConfigSkillData FindConfigSkillData(int skillID)
        {
            skillDatas.TryGetValue(skillID, out ConfigSkillData skillData);
            return skillData;
        }
    }

    public class ConfigSkillData
    {
        public ConfigSkillDetectHitData[] DetectHitData { get; set; }
        public List<ConfigSkillFXItem> FXData { get; set; }
        public List<ConfigSkillSFXItem> SFXData { get; set; }
    }

    public class ConfigSkillFXItem
    {
        public int ID { get; set; }
        public float Delay { get; set; }
    }

    public class ConfigSkillSFXItem
    {
        public int ID { get; set; }
        public float Delay { get; set; }
    }

    public class ConfigSkillDetectHitData
    {
        public ConfigEffectData[] EffectData { get; set; }
    }
}
