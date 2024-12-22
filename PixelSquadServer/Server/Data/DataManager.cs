using Newtonsoft.Json;
using Server.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public class DataManager
    {
        public static DataManager Instance { get; } = new DataManager();

        public Dictionary<string, ClassData> ClassDataDictionary { get; private set; } = new Dictionary<string, ClassData>();
        public Dictionary<int, SkillData> SkillDataDictionary { get; private set; } = new Dictionary<int, SkillData>();

        private DataManager() { }

        public void LoadData()
        {
            try
            {
                // Load and parse ClassData.json
                if (File.Exists("ClassData.json"))
                {
                    string classDataJson = File.ReadAllText("ClassData.json");
                    ClassDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, ClassData>>(classDataJson);
                }
                else
                {
                    Console.WriteLine($"ClassData file not found at: {"ClassData.json"}");
                }

                // Load and parse SkillData.json
                if (File.Exists("SkillData.json"))
                {
                    string skillDataJson = File.ReadAllText("SkillData.json");
                    SkillDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, SkillData>>(skillDataJson);
                }
                else
                {
                    Console.WriteLine($"SkillData file not found at: {"SkillData.json"}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public ClassData GetClassData(string className)
        {
            if (ClassDataDictionary.TryGetValue(className, out var classData))
            {
                return classData;
            }
            else
            {
                Console.WriteLine($"Class data for '{className}' not found.");
                return null;
            }
        }

        public SkillData GetSkillData(int skillId)
        {
            if (SkillDataDictionary.TryGetValue(skillId, out var skillData))
            {
                return skillData;
            }
            else
            {
                Console.WriteLine($"Skill data for ID '{skillId}' not found.");
                return null;
            }
        }
    }

    public class ClassData
    {
        public float MaxHp { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float AttackDelay { get; set; }
        public float DashCooldown { get; set; }
        public int FirstSkillId { get; set; }
        public int SecondSkillId { get; set; }
    }

    public class SkillData
    {
        public string SkillName { get; set; }
        public string SkillType { get; set; }
        public string Description { get; set; }
        public float Range { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float DmgRatio { get; set; }
        public float Cooldown { get; set; }
    }
}

