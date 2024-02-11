using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModShardLauncher.Mods
{
    public class Weapon
    {
        public Weapon()
        {
            NameList = new Dictionary<ModLanguage, string>();
            WeaponDescriptions = new Dictionary<ModLanguage, string>();
            foreach(ModLanguage lan in Enum.GetValues(typeof(ModLanguage)))
            {
                NameList.Add(lan, "None");
                WeaponDescriptions.Add(lan, "None");
            } 
        }
        public Weapon(string property, List<string>? description = null, List<string>? names = null)
        {
            NameList = new Dictionary<ModLanguage, string>();
            WeaponDescriptions = new Dictionary<ModLanguage, string>();
            Name = property.Split(";")[0];
            ID = property.Split(";")[1];
            Set(property);
            int index = 0;
            foreach (ModLanguage lan in Enum.GetValues(typeof(ModLanguage)))
            {
                if (description != null) WeaponDescriptions.Add(lan, description[index]);
                else WeaponDescriptions.Add(lan, "None");
                if (names != null) NameList.Add(lan, names[index]);
                else NameList.Add(lan, "None");
                index++;
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name;
        /// <summary>
        /// 译名列表
        /// </summary>
        public Dictionary<ModLanguage, string> NameList;
        /// <summary>
        /// 描述翻译列表
        /// </summary>
        public Dictionary<ModLanguage, string> WeaponDescriptions;
        /// <summary>
        /// 内部ID
        /// </summary>
        public string? ID;
        /// <summary>
        /// 武器的种类(单/双手等)
        /// </summary>
        public string? Slot;
        /// <summary>
        /// 武器品质
        /// </summary>
        public string? Rare;
        /// <summary>
        /// 武器材料
        /// </summary>
        public string? Material;
        /// <summary>
        /// [!] 未知属性
        /// </summary>
        public string? tags;
        /// <summary>
        /// [!] 未知属性
        /// </summary>
        public string? NoDrop;
        /// <summary>
        /// 最大耐久
        /// </summary>
        public int MaxDuration;
        /// <summary>
        /// 武器价格
        /// </summary>
        public int Price;
        /// <summary>
        /// 武器级别
        /// </summary>
        public int Lv;
        /// <summary>
        /// [!] 未知属性
        /// </summary>
        public int E;
        /// <summary>
        /// [!] 未知属性
        /// </summary>
        public int Rng;
        /// <summary>
        /// 武器伤害(但是好像没用)
        /// </summary>
        public int WeaponDamage;
        /// <summary>
        /// 护甲破坏
        /// </summary>
        public int ArmorDamage;
        /// <summary>
        /// 护甲穿透
        /// </summary>
        public int ArmorPiercing;
        /// <summary>
        /// 肢体伤害
        /// </summary>
        public int BodypartDamage;
        /// <summary>
        /// 劈砍伤害
        /// </summary>
        public int SlashingDamage;
        /// <summary>
        /// 穿刺伤害
        /// </summary>
        public int PiercingDamage;
        /// <summary>
        /// 钝击伤害
        /// </summary>
        public int BluntDamage;
        /// <summary>
        /// 撕裂伤害
        /// </summary>
        public int RendingDamage;
        /// <summary>
        /// 灼烧伤害
        /// </summary>
        public int FireDamage;
        /// <summary>
        /// 电击伤害
        /// </summary>
        public int ShockDamage;
        /// <summary>
        /// 中毒伤害
        /// </summary>
        public int PoisonDamage;
        /// <summary>
        /// 腐蚀伤害
        /// </summary>
        public int CausticDamage;
        /// <summary>
        /// 霜冻伤害
        /// </summary>
        public int FrostDamage;
        /// <summary>
        /// 秘术伤害
        /// </summary>
        public int ArcaneDamage;
        /// <summary>
        /// 邪术伤害
        /// </summary>
        public int UnholyDamage;
        /// <summary>
        /// 神圣伤害
        /// </summary>
        public int SacredDamage;
        /// <summary>
        /// 灵术伤害
        /// </summary>
        public int PsionicDamage;
        /// <summary>
        /// 准度
        /// </summary>
        public int HitChance;
        /// <summary>
        /// 暴击几率
        /// </summary>
        public int CRT;
        /// <summary>
        /// 暴击效果
        /// </summary>
        public int CRTD;
        /// <summary>
        /// 格挡几率
        /// </summary>
        public int PRR;
        /// <summary>
        /// 格挡力量
        /// </summary>
        public int BlockPower;
        /// <summary>
        /// 反击几率
        /// </summary>
        public int CTA;
        /// <summary>
        /// 失手几率
        /// </summary>
        public int FMB;
        /// <summary>
        /// 闪躲几率
        /// </summary>
        public int EVS;
        /// <summary>
        /// 出血几率
        /// </summary>
        public int BleedingChance;
        /// <summary>
        /// 击晕几率
        /// </summary>
        public int DazeChance;
        /// <summary>
        /// 硬直几率
        /// </summary>
        public int StunChance;
        /// <summary>
        /// 击退几率
        /// </summary>
        public int KnockbackChance;
        /// <summary>
        /// 限制移动几率
        /// </summary>
        public int ImmobChance;
        /// <summary>
        /// 破衡几率
        /// </summary>
        public int StaggerChance;
        /// <summary>
        /// 精力
        /// </summary>
        public int MP;
        /// <summary>
        /// 精力自动恢复
        /// </summary>
        public int MPRestoration;
        /// <summary>
        /// 冷却时间
        /// </summary>
        public int CooldownReduction;
        /// <summary>
        /// 技能精力消耗
        /// </summary>
        public int SkillsEnergyCost;
        /// <summary>
        /// 法术精力消耗
        /// </summary>
        public int SpellsEnergyCost;
        /// <summary>
        /// 法力
        /// </summary>
        public int MagicPower;
        /// <summary>
        /// 失误几率
        /// </summary>
        public int MiscastChance;
        /// <summary>
        /// 奇观几率
        /// </summary>
        public int MiracleChance;
        /// <summary>
        /// 奇观效果
        /// </summary>
        public int MiraclePower;
        /// <summary>
        /// 失误伤害
        /// </summary>
        public int BackfireDamage;
        /// <summary>
        /// 炎术法力
        /// </summary>
        public int PyromanticPower;
        /// <summary>
        /// 地术法力
        /// </summary>
        public int GeomanticPower;
        /// <summary>
        /// 毒术法力
        /// </summary>
        public int VenomanticPower;
        /// <summary>
        /// 电术法力
        /// </summary>
        public int ElectromanticPower;
        /// <summary>
        /// 冰术法力
        /// </summary>
        public int CryomanticPower;
        /// <summary>
        /// 秘术法力
        /// </summary>
        public int ArcanisticPower;
        /// <summary>
        /// 星术法力
        /// </summary>
        public int AstromanticPower;
        /// <summary>
        /// 灵术法力
        /// </summary>
        public int PsimanticPower;
        /// <summary>
        /// 时间法力
        /// </summary>
        public int ChronomanticPower;
        /// <summary>
        /// 生命自动恢复
        /// </summary>
        public int HealthRestoration;
        /// <summary>
        /// 生命吸取
        /// </summary>
        public int Lifesteal;
        /// <summary>
        /// 精力吸取
        /// </summary>
        public int Manasteal;
        /// <summary>
        /// 距离加成
        /// </summary>
        public int BonusRange;
        /// <summary>
        /// [!] 未知属性
        /// </summary>
        public int RangeModifier;
        /// <summary>
        /// 所受伤害
        /// </summary>
        public int DamageReceived;
        /// <summary>
        /// 反伤
        /// </summary>
        public int DamageReturned;
        /// <summary>
        /// 治疗效果
        /// </summary>
        public int HealingReceived;
        /// <summary>
        /// 潜行
        /// </summary>
        public int STL;
        /// <summary>
        /// 行动响声
        /// </summary>
        public int NoiseProduced;
        /// <summary>
        /// 平衡
        /// </summary>
        public int Balance;
        /// <summary>
        /// 副手效果
        /// </summary>
        public int OffhandEfficiency;
        /// <summary>
        /// [!] 未知属性
        /// </summary>
        public int SlayingChance;
        public (string, string, string) AsString()
        {
            return Weapon2String(this);
        }
        public void CloneDefaults(string name)
        {
            Weapon weapon = Msl.GetWeapon(name);
            Set(Weapon2String(weapon).Item1);
        }

        public virtual void SetDefaults()
        {

        }
        public void Set(string property)
        {
            List<string> attributes = property.Split(";").ToList();
            attributes.RemoveAt(attributes.Count - 1);
            string str2 =
            "Name;ID;Slot;Rare;Material;MaxDuration;Lv;E;Price;Rng;WeaponDamage;ArmorDamage;" +
            "ArmorPiercing;BodypartDamage;SlashingDamage;PiercingDamage;BluntDamage;RendingDamage;" +
            "FireDamage;ShockDamage;PoisonDamage;CausticDamage;FrostDamage;ArcaneDamage;" +
            "UnholyDamage;SacredDamage;PsionicDamage;HitChance;CRT;CRTD;PRR;BlockPower;CTA;FMB;EVS;" +
            "BleedingChance;DazeChance;StunChance;KnockbackChance;ImmobChance;StaggerChance;MP;MPRestoration;" +
            "CooldownReduction;SkillsEnergyCost;SpellsEnergyCost;MagicPower;MiscastChance;MiracleChance;" +
            "MiraclePower;BackfireDamage;PyromanticPower;GeomanticPower;VenomanticPower;ElectromanticPower;" +
            "CryomanticPower;ArcanisticPower;AstromanticPower;PsimanticPower;ChronomanticPower;HealthRestoration;" +
            "Lifesteal;Manasteal;BonusRange;RangeModifier;DamageReceived;DamageReturned;HealingReceived;" +
            "STL;NoiseProduced;Balance;OffhandEfficiency;SlayingChance;tags;NoDrop;";
            List<string> attributes2 = str2.Split(";").ToList();
            attributes2.Remove("");
            foreach (string attr in attributes2)
            {
                if (attr == "Name" || attr == "ID") continue;
                FieldInfo? fieldOrNull = typeof(Weapon).GetField(attr, BindingFlags.Public | BindingFlags.Instance);
                FieldInfo field = Msl.ThrowIfNull(fieldOrNull);

                if (field.FieldType == typeof(string)) field.SetValue(this, attributes[attributes2.IndexOf(attr)]);
                else if (attributes[attributes2.IndexOf(attr)] == "") field.SetValue(this, 0);
                else field.SetValue(this, int.Parse(attributes[attributes2.IndexOf(attr)]));
            }
        }
        public static (string, string, string) Weapon2String(Weapon weapon)
        {
            Type type = typeof(Weapon);
            string str =
            "Name;ID;Slot;Rare;Material;MaxDuration;Lv;E;Price;Rng;WeaponDamage;ArmorDamage;" +
            "ArmorPiercing;BodypartDamage;SlashingDamage;PiercingDamage;BluntDamage;RendingDamage;" +
            "FireDamage;ShockDamage;PoisonDamage;CausticDamage;FrostDamage;ArcaneDamage;" +
            "UnholyDamage;SacredDamage;PsionicDamage;HitChance;CRT;CRTD;PRR;BlockPower;CTA;FMB;EVS;" +
            "BleedingChance;DazeChance;StunChance;KnockbackChance;ImmobChance;StaggerChance;MP;MPRestoration;" +
            "CooldownReduction;SkillsEnergyCost;SpellsEnergyCost;MagicPower;MiscastChance;MiracleChance;" +
            "MiraclePower;BackfireDamage;PyromanticPower;GeomanticPower;VenomanticPower;ElectromanticPower;" +
            "CryomanticPower;ArcanisticPower;AstromanticPower;PsimanticPower;ChronomanticPower;HealthRestoration;" +
            "Lifesteal;Manasteal;BonusRange;RangeModifier;DamageReceived;DamageReturned;HealingReceived;" +
            "STL;NoiseProduced;Balance;OffhandEfficiency;SlayingChance;tags;NoDrop;";

            List<string> attributes = str.Split(";").ToList();
            attributes.Remove("");
            string ret = "";

            foreach (string attr in attributes)
            {
                FieldInfo? fieldOrNull = type.GetField(attr, BindingFlags.Public | BindingFlags.Instance);
                FieldInfo field = Msl.ThrowIfNull(fieldOrNull);

                object? value = field.GetValue(weapon);
                if (value != null)
                {
                    if((field.FieldType == typeof(int) && (value as int?) != 0) || field.FieldType == typeof(string))
                        ret += value.ToString() + ";";
                    else ret += ";";
                }
                else ret += ";";
            }
            var des = weapon.Name + ";" + string.Join(";", weapon.WeaponDescriptions.Values.ToList());
            var name = weapon.Name + ";" + string.Join(";", weapon.NameList.Values.ToList());
            return (ret, des, name);
        }
        public static Weapon String2Weapon(string property)
        {
            Weapon weapon = new();
            List<string> attributes = property.Split(";").ToList();
            attributes.RemoveAt(attributes.Count - 1);
            string str2 =
            "Name;ID;Slot;Rare;Material;MaxDuration;Lv;E;Price;Rng;WeaponDamage;ArmorDamage;" +
            "ArmorPiercing;BodypartDamage;SlashingDamage;PiercingDamage;BluntDamage;RendingDamage;" +
            "FireDamage;ShockDamage;PoisonDamage;CausticDamage;FrostDamage;ArcaneDamage;" +
            "UnholyDamage;SacredDamage;PsionicDamage;HitChance;CRT;CRTD;PRR;BlockPower;CTA;FMB;EVS;" +
            "BleedingChance;DazeChance;StunChance;KnockbackChance;ImmobChance;StaggerChance;MP;MPRestoration;" +
            "CooldownReduction;SkillsEnergyCost;SpellsEnergyCost;MagicPower;MiscastChance;MiracleChance;" +
            "MiraclePower;BackfireDamage;PyromanticPower;GeomanticPower;VenomanticPower;ElectromanticPower;" +
            "CryomanticPower;ArcanisticPower;AstromanticPower;PsimanticPower;ChronomanticPower;HealthRestoration;" +
            "Lifesteal;Manasteal;BonusRange;RangeModifier;DamageReceived;DamageReturned;HealingReceived;" +
            "STL;NoiseProduced;Balance;OffhandEfficiency;SlayingChance;tags;NoDrop;";
            List<string> attributes2 = str2.Split(";").ToList();
            attributes2.Remove("");
            foreach (string attr in attributes2)
            {
                FieldInfo? fieldOrNull = typeof(Weapon).GetField(attr, BindingFlags.Public | BindingFlags.Instance);
                FieldInfo field = Msl.ThrowIfNull(fieldOrNull);

                if (field.FieldType == typeof(string)) field.SetValue(weapon, attributes[attributes2.IndexOf(attr)]);
                else if (attributes[attributes2.IndexOf(attr)] == "") field.SetValue(weapon, 0);
                else field.SetValue(weapon, int.Parse(attributes[attributes2.IndexOf(attr)]));
            }
            return weapon;
        }
    }
}
