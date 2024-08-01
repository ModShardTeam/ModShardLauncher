using System;
using System.Linq;
using System.Text;
using Serilog;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public enum MetaCaterory
    {
        Weapons,
        Utilities,
        Sorceries,
    }
    public enum SkillType
    {
        Summon,
        Buff,
        Projectile,
        Passive,
    }
    public class SkillNode
    {
        public string name;
        public int tier;
        public int x;
        public int y;
        public SkillNode[] dependancy;
        public SkillNode(string name, int tier, int x, int y, params SkillNode[] dependancy)
        {
            this.name = name;
            this.tier = tier;
            this.x = x;
            this.y = y;
            this.dependancy = dependancy;
        }
    }
    public static partial class Msl
    {
        public static void AddSkillTree(string skillTreeName, MetaCaterory metaCaterory, string branchSprite, params SkillNode[] skills)
        {
            string skillsId = string.Join(", ", skills.Select(x => x.name));
            string skillsTier1Id = string.Join(", ", skills.Where(x => x.tier == 1).Select(x => x.name));
            UndertaleGameObject skillTree = AddObject($"o_skill_category_{skillTreeName}", "", "o_skill_category", true, false, true, CollisionShapeFlags.Circle);

            AddNewEvent(skillTree, @$"
                event_inherited()
                text = ""{skillTreeName}""
                skill = [{skillsId}]
                branch_sprite = {branchSprite}
            ", EventType.Create, 0);

            string ctr_builder = @"
pushi.e {0}
conv.i.v
pushi.e {1}
conv.i.v
pushi.e {2}
conv.i.v
push.v self.connectionsRender
push.i gml_Script_ctr_SkillPoint
conv.i.v
call.i @@NewGMLObject@@(argc=5)
pop.v.v local._{2}";

            string dependancy_builder = @"
pushloc.v local._{0}
call.i @@NewGMLArray@@(argc=4)
dup.v 1 8
dup.v 0
push.v stacktop.addConnectedPoints
callv.v 1
popz.v
            ";

            StringBuilder sb = new();
            //sb.Append("call.i event_inherited(argc=0))\npopz.v");

            foreach(SkillNode skill in skills)
            {
                sb.AppendFormat(ctr_builder, skill.x, skill.y, skill.name);
            }
            foreach(SkillNode skill in skills)
            {
                /* foreach(string dep in skill.Dependancy)
                {
                    sb.AppendFormat("pushloc.v local._{0}\n", dep);
                }
                sb.AppendFormat(dependancy_builder, skill.Name); */
            }
            AddNewEvent(skillTree, sb.ToString(), EventType.Other, 24, true);

            LoadGML("gml_Object_o_skillmenu_Create_0")
                .MatchFrom("var _metaCategoriesArray = ")
                .InsertBelow($"array_push(_metaCategoriesArray[{(int)metaCaterory}], o_skill_category_{skillTreeName})")
                .Save();

            LoadGML("gml_GlobalScript_scr_skill_tier_init")
                .MatchFrom("}")
                .InsertBelow($"global.{skillTreeName}_tier1 = [\"{skillTreeName}\", {skillsTier1Id}]")
                .Save();

        }
        public static UndertaleGameObject AddPassiveSkill(string skillName)
        {
            return AddObject($"o_pass_skill_{skillName}", $"s_{skillName}", "o_skill_passive", true, false, true, CollisionShapeFlags.Circle);
        }
        public static UndertaleGameObject AddBuffSkill(string skillName)
        {
            AddObject($"o_skill_{skillName}_ico", $"s_{skillName}", "o_skill_ico", true, false, true, CollisionShapeFlags.Circle);
            AddObject($"o_skill_{skillName}", $"s_{skillName}", "o_skill", true, false, true, CollisionShapeFlags.Circle);
            AddObject($"o_b_{skillName}", $"s_b_{skillName}", "o_buff_maneuver", true, false, true, CollisionShapeFlags.Circle);
            return AddObject($"o_pass_skill_{skillName}", $"s_{skillName}", "o_skill_passive", true, false, true, CollisionShapeFlags.Circle);
        }
        public static void AddSkill(string skillName, SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.Summon:
                    AddObject($"o_skill_{skillName}_ico", $"s_{skillName}", "o_skill_ico", true, false, true, CollisionShapeFlags.Circle);
                    AddObject($"o_skill_{skillName}", $"s_{skillName}", "o_skill", true, false, true, CollisionShapeFlags.Circle);
			        AddObject($"o_{skillName}_birth", $"s_spell_{skillName}_birth", "o_spelllbirth", true, false, true, CollisionShapeFlags.Circle);
                    break;
                case SkillType.Buff:
                    
                    break;
                case SkillType.Projectile:
                    AddObject($"o_skill_{skillName}_ico", $"s_{skillName}", "o_skill_ico", true, false, true, CollisionShapeFlags.Circle);
			        AddObject($"o_skill_{skillName}", $"s_{skillName}", "o_skill", true, false, true, CollisionShapeFlags.Circle);
			        //AddObject($"o_{skillName}", $"s_spell_{skillName}", "o_shell_damage", true, false, true, CollisionShapeFlags.Circle);
			        AddObject($"o_{skillName}_birth", $"s_spell_{skillName}_birth", "o_spelllbirth", true, false, true, CollisionShapeFlags.Circle);
                    break;
                case SkillType.Passive:
                    
                    break;
            }
        }
    }
}