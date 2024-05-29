using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Config
{
    public class ConfigEffectData
    {
        public string Name { get; set; }
        public bool Revert { get; set; }
        public List<string>[] Args { get; set; }

        public static ConfigEffectData ParseEffectData(int maxLevel, string effect)
        {
            if (string.IsNullOrEmpty(effect))
                return null;

            ConfigEffectData data = new ConfigEffectData();
            int nameSemicolon = effect.IndexOf(';');
            //if (nameSemicolon < 0)
            //{
            //    UnityLog.Error($"解析ConfigEffectData失败：请用分号隔开效果名字。Effect:{effect}");
            //    return null;
            //}

            string[] nameArgs;
            if (nameSemicolon < 0)
            {
                nameArgs = effect.Split(':');
            }
            else
            {
                nameArgs = effect.Substring(0, nameSemicolon).Split(':');
            }
            data.Name = nameArgs[0].ToLower();
            if (nameArgs.Length > 1)
            {
                data.Revert = nameArgs[1].ToLower() == "revert";
            }

            if (nameSemicolon > 0)
            {
                string argString = effect.Substring(nameSemicolon + 1, effect.Length - nameSemicolon - 1);

                Regex regex = new Regex(@"\[(\w|\d|;|\.|-)*\]");
                MatchCollection matches = regex.Matches(argString);

                string[] levelArgs;
                if (maxLevel > matches.Count)
                {
                    if (maxLevel != 1 || matches.Count != 0)
                    {
                        UnityLog.Error($"解析ConfigEffectData失败：最大等级与效果分级不匹配，请填写每一级的效果。MaxLevel:{maxLevel}, Effect:{effect}");
                        return null;
                    }

                    // 最大等级为1的时候，允许简写
                    levelArgs = new string[1];
                    levelArgs[0] = argString;
                }
                else
                {
                    levelArgs = new string[matches.Count];
                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match match = matches[i];
                        levelArgs[i] = match.Value.Substring(1, match.Value.Length - 2);
                    }
                }

                data.Args = new List<string>[levelArgs.Length];
                for (int i = 0; i < levelArgs.Length; i++)
                {
                    List<string> args = new List<string>();
                    args.AddRange(levelArgs[i].Split(';'));
                    data.Args[i] = args;
                }
            }
            else
            {
                data.Args = new List<string>[1];
            }
            return data;
        }
    }
}
