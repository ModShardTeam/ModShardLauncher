using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    // TODO: Add a way to add a new category
    public class LocalizableSpeechBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableSpeechBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "kill" => "kill_end",
                "killVelmir" => "killVelmir_end",
                "killArna" => "killArna_end",
                "killJonna" => "killJonna_end",
                "killJorgrim" => "killJorgrim_end",
                "killDirwin" => "killDirwin_end",
                "killAllies" => "killAllies_end",
                "killAlliesJorgrim" => "killAllies_Jorgrim_end",
                "killCrit" => "killCrit_end",
                "killCritJorgrim" => "killCrit_Jorgrim_end",
                "killCritVelmir" => "killCrit_Velmir_end",
                "killCritBlade" => "killCritBlade_end",
                "killCritShot" => "killCritShot_end",
                "killCritShotDirwin" => "killCritShot_Dirwin_end",
                "throwDamage" => "throwDamage_end",
                "healthLoss" => "healthLoss_end",
                "hitMissed" => "hitMissed_end",
                "hitDodged" => "hitDodged_end",
                "hitFumbled" => "hitFumbled_end",
                "hitFullyBlocked" => "hitFullyBlocked_end",
                "counterCrit" => "counterCrit_end",
                "dodge" => "dodge_end",
                "injuryLight" => "injuryLight_end",
                "injuryTorso" => "injuryTorso_end",
                "injuryLeg" => "injuryLeg_end",
                "injuryArm" => "injuryArm_end",
                "injuryHead" => "injuryHead_end",
                "debuffDaze" => "debuffDaze_end",
                "debuffWet" => "debuffWet_end",
                "debuffAftermath" => "debuffAftermath_end",
                "debuffAftermath_Jorgrim" => "debuffAftermath_Jorgrim_end",
                "debuffNet" => "debuffNet_end",
                "debuffDOT" => "debuffDOT_end",
                "debuffFire" => "debuffFire_end",
                "debuffAcid" => "debuffAcid_end",
                "debuffAlco1" => "debuffAlco1_end",
                "debuffAlco2" => "debuffAlco2_end",
                "debuffAlco3" => "debuffAlco3_end",
                "debuffAlco4" => "debuffAlco4_end",
                "useDrug" => "useDrug_end",
                "useMedicine" => "useMedicine_end",
                "useTrap" => "useTrap_end",
                "useTrapDirwin" => "useTrapDirwin_end",
                "hunger" => "hunger_end",
                "thirsty" => "thirsty_end",
                "pain" => "pain_end",
                "intoxication" => "intoxication_end",
                "fatigue" => "fatigue_end",
                "bleeding" => "bleeding_end",
                "bleedingStacks" => "bleedingStacks_end",
                "lockPicked" => "lockPicked_end",
                "lockFailed" => "lockFailed_end",
                "doorDestroyed" => "doorDestroyed_end",
                "trapFind" => "trapFind_end",
                "trapHit" => "trapHit_end",
                "trapDisarm" => "trapDisarm_end",
                "trapAvoid" => "trapAvoid_end",
                "interruption" => "interruption_end",
                "Ambush" => "Ambush_end",
                "container" => "container_end",
                "containerRich" => "containerRich_end",
                "optimism" => "optimism_end",
                "highBackfireDmg" => "highBackfireDMG_end",
                "heroism" => "heroism_end",
                "secondWind" => "second_wind_end",
                "prudence" => "prudence_end",
                "swimLowMP" => "swimLowMP_end",
                "vampireSuspicious" => "vampire_suspicious_end",
                "vampireHostile" => "vampire_hostile_end",
                "vampireAttack" => "vampire_attack_end",
                "vampireSkill" => "vampire_skill_end",
                "vampireDeath" => "vampire_death_end",
                "vampireAllyDeath" => "vampire_ally_death_end",
                "vampireInjury" => "vampire_injury_end",
                "vampireLordAlarm" => "vampire_lord_alarm_end",
                "idleUndead" => "idleUndead_end",
                "alarmUndead" => "alarmUndead_end",
                "hostileUndead" => "hostileUndead_end",
                "attackUndead" => "attackUndead_end",
                "moveUndead" => "moveUndead_end",
                "injuryUndead" => "injuryUndead_end",
                "deathUndead" => "deathUndead_end",
                "hostileNecromancer" => "hostileNecromancer_end",
                "attackNecromancer" => "attackNecromancer_end",
                "idleBandit" => "idleBandit_end",
                "idleBanditWeather" => "idleBanditWeather_end",
                "idleBossBandit" => "idleBossBandit_end",
                "alarmBandit" => "alarmBandit_end",
                "hostileSurfaceAlliesBandit" => "hostileSurfaceAlliesBandit_end",
                "hostileSurfaceAlliesBandit_Dwarf" => "hostileSurfaceAlliesBandit_Dwarf_end",
                "hostileAlliesBandit" => "hostileAlliesBandit_end",
                "hostileMageBandit" => "hostileMageBandit_end",
                "castingMageBandit" => "castingMageBandit_end",
                "miscastMageBandit" => "miscastMageBandit_end",
                "miracleBandit" => "miracleBandit_end",
                "miracleBanditAllies" => "miracleBanditAllies_end",
                "castingBandit" => "castingBandit_end",
                "castingAlliesBandit" => "castingAlliesBandit_end",
                "chargeBandit" => "chargeBandit_end",
                "bottleThrowBandit" => "bottleThrowBandit_end",
                "fireBandit" => "fireBandit_end",
                "fireAlliesBandit" => "fireAlliesBandit_end",
                "fireDeathBandit" => "fireDeathBandit_end",
                "combatBeastBandit" => "combatBeastBandit_end",
                "retreatAlliesBandit" => "retreatAlliesBandit_end",
                "aggroStrongBandit" => "aggroStrongBandit_end",
                "trapBandit" => "trapBandit_end",
                "trapDisarmBandit" => "trapDisarmBandit_end",
                "hostileBandit" => "hostileBandit_end",
                "attackBandit" => "attackBandit_end",
                "attackskillAlliesBandit" => "attackskillAlliesBandit_end",
                "deathAlliesBandit" => "deathAlliesBandit_end",
                "doorAlarmBandit" => "doorAlarmBandit_end",
                "doorHostileBandit" => "doorHostileBandit_end",
                "injuryBandit" => "injuryBandit_end",
                "injuryHeadBandit" => "injuryHeadBandit_end",
                "injuryHandBandit" => "injuryHandBandit_end",
                "injuryLegBandit" => "injuryLegBandit_end",
                "bleedBandit" => "bleedBandit_end",
                "injuryTorsoBandit" => "injuryTorsoBandit_end",
                "deathBandit" => "deathBandit_end",
                "retreatBandit" => "retreatBandit_end",
                "attackFreak" => "attackFreak_end",
                "npcNoTurn" => "npc_no_turn_end",
                "npcRain1" => "npc_rain_1_end",
                "npcRain2" => "npc_rain_2_end",
                "npcRain3" => "npc_rain_3_end",
                "npcPeasant" => "npc_peasant_end",
                "npcGuardNeutral" => "npc_guard_neutral_end",
                "npcGuardTrueneutral" => "npc_guard_trueneutral_end",
                "npcGuardAdmiration" => "npc_guard_admiration_end",
                "npcGuardAmity" => "npc_guard_amity_end",
                "npcGuardRespect" => "npc_guard_respect_end",
                "npcGuardDislike" => "npc_guard_dislike_end",
                "npcGuardHatred" => "npc_guard_hatred_end",
                "npcPeasantNeutral" => "npc_peasant_neutral_end",
                "npcPeasantTrueneutral" => "npc_peasant_trueneutral_end",
                "npcPeasantAdmiration" => "npc_peasant_admiration_end",
                "npcPeasantAmity" => "npc_peasant_amity_end",
                "npcPeasantRespect" => "npc_peasant_respect_end",
                "npcPeasantDislike" => "npc_peasant_dislike_end",
                "npcPeasantHatred" => "npc_peasant_hatred_end",
                "npcPeasantMale" => "npc_peasant_male_end",
                "npcLowhp" => "npc_lowhp_end",
                "npcTrader" => "npc_trader_end",
                "npcWineTrader" => "npc_wine_trader_end",
                "npcFoodTrader" => "npc_food_trader_end",
                "npcVegetableTrader" => "npc_vegetable_trader_end",
                "npcCheeseTrader" => "npc_cheese_trader_end",
                "npcFruitTrader" => "npc_fruit_trader_end",
                "npcButcher" => "npc_butcher_end",
                "npcSmith" => "npc_smith_end",
                "npcCarpenter" => "npc_carpenter_end",
                "npcTailor" => "npc_tailor_end",
                "npcElder" => "npc_elder_end",
                "npcHerbalist" => "npc_herbalist_end",
                "npcDrunkard" => "npc_drunkard_end",
                "npcCaptainTraining" => "npc_captain_training_end",
                "npcMiller" => "npc_miller_end",
                "npcHierophant" => "npc_hierophant_end",
                "npcMercTrader" => "npc_merc_trader_end",
                "npcMercRepair" => "npc_merc_repair_end",
                "npcMercElder" => "npc_merc_elder_end",
                "doorBarred" => "door_barred_end",
                "hatLine" => "hat_line_end",
                "hatLineMiscast" => "hat_lineMiscast_end",
                "hatLineMiracle" => "hat_lineMiracle_end",
                "doorClosed" => "door_closed_end",
                "npcGuardSergeant" => "npc_guard_sergeant_end",
                "playerBed" => "player_bed_end",
                "playerBedKh" => "player_bed_kh_end",
                "playerBedCrime" => "player_bedCrime_end",
                "playerShout" => "player_shout_end",
                "prayHuman" => "pray_human_end",
                "prayHumanCD" => "pray_human_CD_end",
                "prayElf" => "pray_elf_end",
                "prayElfCD" => "pray_elf_CD_end",
                "prayDwarf" => "pray_dwarf_end",
                "prayDwarfCD" => "pray_dwarf_CD_end",
                "prayDwarfShrine" => "pray_dwarf_shrine_end",
                "playerDigHands" => "player_dig_hands_end",
                "playerGraveDig" => "player_grave_dig_end",
                "playerCook" => "player_cook_end",
                "leprosyDoor" => "leprosyDoor_end",
                "secretRoom" => "secretRoom_end",
                "hostage" => "hostage_end",
                "noPelt" => "noPelt_end",
                "cough" => "cough_end",
                "stash" => "stash_end",
                "npcNotalk" => "npc_notalk_end",
                "npcNotalkHostile" => "npc_notalk_hostile_end",
                "verrenNotalkHostile" => "verren_notalk_hostile_end",
                "npcCrimeAlarm" => "npc_crimeAlarm_end",
                "npcCrimeTheft" => "npc_crimeTheft_end",
                "npcCrimeBrawl" => "npc_crimeBrawl_end",
                "npcCrimeGeneric" => "npc_crimeGeneric_end",
                "npcCrimeKill" => "npc_crimeKill_end",
                "npcCrimeGuard" => "npc_crimeGuard_end",
                "npcCrimeBrawlTarget0" => "npc_crimeBrawlTarget1_end", // Looks weird but it's valid because table is fucked
                "npcCrimeBrawlTarget1" => "npc_crimeBrawlTarget1", // This needs to inject at ind + 1
                "npcPest" => "npc_pest_end",
                "npcNightmares" => "npc_nightmares_end",
                "npcAmbushes" => "npc_ambushes_end",
                "npcJerald" => "npc_herald_end",
                "npcLeifIntro" => "npc_leif_intro_end",
                "npcMercIntro" => "npc_merc_intro_end",
                "sceneGeneric" => "scene_generic_end",
                "sceneDarrelOldcaravan" => "scene_darrel_oldcaravan_end",
                "npcTraderSkadia" => "npc_trader_skadia_end",
                "npcTraderNistra" => "npc_trader_nistra_end",
                "npcTraderFjall" => "npc_trader_fjall_end",
                "npcElfBarker" => "npc_elf_barker_end",
                "npcTraderJibey" => "npc_trader_jibey_end",
                "interruptVerren" => "interruptVerren_end",
                "exitGuinnelHouse" => "exitGuinnelHouse_end",
                "examGuinnelHouseApparatus01" => "exam_GuinnelHouse_apparatus01_end",
                "examGuinnelHouseFlasks" => "exam_GuinnelHouse_flasks_end",
                "examGuinnelHousePapers" => "exam_GuinnelHouse_papers_end",
                "examGuinnelHouseApparatus02" => "exam_GuinnelHouse_apparatus02_end",
                "examGuinnelHouseTable" => "exam_GuinnelHouse_table_end",
                "guinnelVerrenComment01" => "guinnelVerrenComment01_end",
                "guinnelVerrenComment02" => "guinnelVerrenComment02_end",
                "lowcreyInterrupt" => "lowcreyInterrupt_end",
                "doorFuneralCave" => "doorFuneralCave_end",
                "barrierCradle" => "barrierCradle_end",
                "essenceAlert" => "essenceAlert_end",
                "manticoreDeath" => "manticoreDeath_end",
                "manticoreDisenchant" => "manticoreDisenchant_end",
                "orderPrisonShout" => "orderPrisonShout_end",
                "orderPrisonShoutInside" => "orderPrisonShoutInside_end",
                "orderPrisonMercHarpy" => "orderPrisonMercHarpy_end",
                "runicStonesAltar" => "runicStonesAltar_end",
                "examineWell" => "examine_well_end",
                "bedrollAmbush" => "bedrollAmbush_end",
                "cantExit" => "cantExit_end",
                "unholyAltarNoRelic" => "unholyAltarNoRelic_end",
                "bodiesNoOil" => "bodiesNoOil_end",
                "bloodFeverBleed" => "bloodFeverBleed_end",
                "bedrollDebuff" => "bedrollDebuff_end",
                "nightmaresSleep" => "nightmaresSleep_end",
                "noPickaxe" => "noPickaxe_end",
                "FireBarrage" => "Fire_Barrage_end",
                "MCFireBarrage" => "MC_Fire_Barrage_end",
                "FlameWave" => "Flame_Wave_end",
                "MCFlameWave" => "MC_Flame_Wave_end",
                "FlameRing" => "Flame_Ring_end",
                "MCFlameRing" => "MC_Flame_Ring_end",
                "Incineration" => "Incineration_end",
                "MCIncineration" => "MC_Incineration_end",
                "MeltingRay" => "Melting_Ray_end",
                "MCMeltingRay" => "MC_Melting_Ray_end",
                "MagmaRain" => "Magma_Rain_end",
                "MCMagmaRain" => "MC_Magma_Rain_end",
                "Inferno" => "Inferno_end",
                "MCInferno" => "MC_Inferno_end",
                "RunicBoulder" => "Runic_Boulder_end",
                "MCRunicBoulder" => "MC_Runic_Boulder_end",
                "StoneSpikes" => "Stone_Spikes_end",
                "MCStoneSpikes" => "MC_Stone_Spikes_end",
                "Stone_Armor" => "Stone_Armor_end",
                "MCStoneArmor" => "MC_Stone_Armor_end",
                "Petrification" => "Petrification_end",
                "MCPetrification" => "MC_Petrification_end",
                "EarthQuake" => "Earth_Quake_end",
                "MCEarthQuake" => "MC_Earth_Quake_end",
                "RunicExplosion" => "Runic_Explosion_end",
                "MCRunicExplosion" => "MC_Runic_Explosion_end",
                "BoulderToss" => "Boulder_Toss_end",
                "MCBoulderToss" => "MC_Boulder_Toss_end",
                "Discharge" => "Discharge_end",
                "MCDischarge" => "MC_Discharge_end",
                "Impulse" => "Impulse_end",
                "MCImpulse" => "MC_Impulse_end",
                "StaticField" => "Static_Field_end",
                "MCStaticField" => "MC_Static_Field_end",
                "ShortCircuit" => "Short_Circuit_end",
                "MCShortCircuit" => "MC_Short_Circuit_end",
                "BallLightning" => "Ball_Lightning_end",
                "MCBallLightning" => "MC_Ball_Lightning_end",
                "ChainLightning" => "Chain_Lightning_end",
                "MCChainLightning" => "MC_Chain_Lightning_end",
                "Tempest" => "Tempest_end",
                "MCTempest" => "MC_Tempest_end",
                "SealOfFinesse" => "Seal_of_Finesse_end",
                "MCSealOfFinesse" => "MC_Seal_of_Finesse_end",
                "SealOfPower" => "Seal_of_Power_end",
                "MCSealOfPower" => "MC_Seal_of_Power_end",
                "SealOfInsight" => "Seal_of_Insight_end",
                "MCSealOfInsight" => "MC_Seal_of_Insight_end",
                "SealOfCleansing" => "Seal_of_Cleansing_end",
                "MCSealOfCleansing" => "MC_Seal_of_Cleansing_end",
                "SealOfReflection" => "Seal_of_Reflection_end",
                "MCSealOfReflection" => "MC_Seal_of_Reflection_end",
                "SealOfShackles" => "Seal_of_Shackles_end",
                "MCSealOfShackles" => "MC_Seal_of_Shackles_end",
                "Darkbolt" => "Darkbolt_end",
                "Curse" => "Curse_end",
                "DarkResurrection" => "Dark_Resurrection_end",
                "DarkBlessing" => "Dark_Blessing_end",
                "SignOfDarkness" => "Sign_of_Darkness_end",
                "DeathTouch" => "Death_Touch_end",
                "SoulSacrifice" => "Soul_Sacrifice_end",
                "SigilOfBinding" => "Sigil_of_Binding_end",
                "MarkOfTheFeast" => "Mark_of_the_Feast_end",
                "SacrificialBlood" => "Sacrificial_blood_end",
                "SummonBloodGolem" => "Summon_Blood_Golem_end",
                "VampireRune" => "Vampire_Rune_end",
                "LifeLeech" => "Life_Leech_end",
                "Chance" => "CHANCE_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "kill" or
                "killVelmir" or
                "killArna" or
                "killJonna" or
                "killJorgrim" or
                "killDirwin" or
                "killAllies" or
                "killAlliesJorgrim" or
                "killCrit" or
                "killCritJorgrim" or
                "killCritVelmir" or
                "killCritBlade" or
                "killCritShot" or
                "killCritShotDirwin" or
                "throwDamage" or
                "healthLoss" or
                "hitMissed" or
                "hitDodged" or
                "hitFumbled" or
                "hitFullyBlocked" or
                "counterCrit" or
                "dodge" or
                "injuryLight" or
                "injuryTorso" or
                "injuryLeg" or
                "injuryArm" or
                "injuryHead" or
                "debuffDaze" or
                "debuffWet" or
                "debuffAftermath" or
                "debuffAftermath_Jorgrim" or
                "debuffNet" or
                "debuffDOT" or
                "debuffFire" or
                "debuffAcid" or
                "debuffAlco1" or
                "debuffAlco2" or
                "debuffAlco3" or
                "debuffAlco4" or
                "useDrug" or
                "useMedicine" or
                "useTrap" or
                "useTrapDirwin" or
                "hunger" or
                "thirsty" or
                "pain" or
                "intoxication" or
                "fatigue" or
                "bleeding" or
                "bleedingStacks" or
                "lockPicked" or
                "lockFailed" or
                "doorDestroyed" or
                "trapFind" or
                "trapHit" or
                "trapDisarm" or
                "trapAvoid" or
                "interruption" or
                "Ambush" or
                "container" or
                "containerRich" or
                "optimism" or
                "highBackfireDmg" or
                "heroism" or
                "secondWind" or
                "prudence" or
                "swimLowMP" or
                "vampireSuspicious" or
                "vampireHostile" or
                "vampireAttack" or
                "vampireSkill" or
                "vampireDeath" or
                "vampireAllyDeath" or
                "vampireInjury" or
                "vampireLordAlarm" or
                "idleUndead" or
                "alarmUndead" or
                "hostileUndead" or
                "attackUndead" or
                "moveUndead" or
                "injuryUndead" or
                "deathUndead" or
                "hostileNecromancer" or
                "attackNecromancer" or
                "idleBandit" or
                "idleBanditWeather" or
                "idleBossBandit" or
                "alarmBandit" or
                "hostileSurfaceAlliesBandit" or
                "hostileSurfaceAlliesBandit_Dwarf" or
                "hostileAlliesBandit" or
                "hostileMageBandit" or
                "castingMageBandit" or
                "miscastMageBandit" or
                "miracleBandit" or
                "miracleBanditAllies" or
                "castingBandit" or
                "castingAlliesBandit" or
                "chargeBandit" or
                "bottleThrowBandit" or
                "fireBandit" or
                "fireAlliesBandit" or
                "fireDeathBandit" or
                "combatBeastBandit" or
                "retreatAlliesBandit" or
                "aggroStrongBandit" or
                "trapBandit" or
                "trapDisarmBandit" or
                "hostileBandit" or
                "attackBandit" or
                "attackskillAlliesBandit" or
                "deathAlliesBandit" or
                "doorAlarmBandit" or
                "doorHostileBandit" or
                "injuryBandit" or
                "injuryHeadBandit" or
                "injuryHandBandit" or
                "injuryLegBandit" or
                "bleedBandit" or
                "injuryTorsoBandit" or
                "deathBandit" or
                "retreatBandit" or
                "attackFreak" or
                "npcNoTurn" or
                "npcRain1" or
                "npcRain2" or
                "npcRain3" or
                "npcPeasant" or
                "npcGuardNeutral" or
                "npcGuardTrueneutral" or
                "npcGuardAdmiration" or
                "npcGuardAmity" or
                "npcGuardRespect" or
                "npcGuardDislike" or
                "npcGuardHatred" or
                "npcPeasantNeutral" or
                "npcPeasantTrueneutral" or
                "npcPeasantAdmiration" or
                "npcPeasantAmity" or
                "npcPeasantRespect" or
                "npcPeasantDislike" or
                "npcPeasantHatred" or
                "npcPeasantMale" or
                "npcLowhp" or
                "npcTrader" or
                "npcWineTrader" or
                "npcFoodTrader" or
                "npcVegetableTrader" or
                "npcCheeseTrader" or
                "npcFruitTrader" or
                "npcButcher" or
                "npcSmith" or
                "npcCarpenter" or
                "npcTailor" or
                "npcElder" or
                "npcHerbalist" or
                "npcDrunkard" or
                "npcCaptainTraining" or
                "npcMiller" or
                "npcHierophant" or
                "npcMercTrader" or
                "npcMercRepair" or
                "npcMercElder" or
                "doorBarred" or
                "hatLine" or
                "hatLineMiscast" or
                "hatLineMiracle" or
                "doorClosed" or
                "npcGuardSergeant" or
                "playerBed" or
                "playerBedKh" or
                "playerBedCrime" or
                "playerShout" or
                "prayHuman" or
                "prayHumanCD" or
                "prayElf" or
                "prayElfCD" or
                "prayDwarf" or
                "prayDwarfCD" or
                "prayDwarfShrine" or
                "playerDigHands" or
                "playerGraveDig" or
                "playerCook" or
                "leprosyDoor" or
                "secretRoom" or
                "hostage" or
                "noPelt" or
                "cough" or
                "stash" or
                "npcNotalk" or
                "npcNotalkHostile" or
                "verrenNotalkHostile" or
                "npcCrimeAlarm" or
                "npcCrimeTheft" or
                "npcCrimeBrawl" or
                "npcCrimeGeneric" or
                "npcCrimeKill" or
                "npcCrimeGuard" or
                "npcCrimeBrawlTarget0" or
                "npcCrimeBrawlTarget1" or
                "npcPest" or
                "npcNightmares" or
                "npcAmbushes" or
                "npcJerald" or
                "npcLeifIntro" or
                "npcMercIntro" or
                "sceneGeneric" or
                "sceneDarrelOldcaravan" or
                "npcTraderSkadia" or
                "npcTraderNistra" or
                "npcTraderFjall" or
                "npcElfBarker" or
                "npcTraderJibey" or
                "interruptVerren" or
                "exitGuinnelHouse" or
                "examGuinnelHouseApparatus01" or
                "examGuinnelHouseFlasks" or
                "examGuinnelHousePapers" or
                "examGuinnelHouseApparatus02" or
                "examGuinnelHouseTable" or
                "guinnelVerrenComment01" or
                "guinnelVerrenComment02" or
                "lowcreyInterrupt" or
                "doorFuneralCave" or
                "barrierCradle" or
                "essenceAlert" or
                "manticoreDeath" or
                "manticoreDisenchant" or
                "orderPrisonShout" or
                "orderPrisonShoutInside" or
                "orderPrisonMercHarpy" or
                "runicStonesAltar" or
                "examineWell" or
                "bedrollAmbush" or
                "cantExit" or
                "unholyAltarNoRelic" or
                "bodiesNoOil" or
                "bloodFeverBleed" or
                "bedrollDebuff" or
                "nightmaresSleep" or
                "noPickaxe" or
                "FireBarrage" or
                "MCFireBarrage" or
                "FlameWave" or
                "MCFlameWave" or
                "FlameRing" or
                "MCFlameRing" or
                "Incineration" or
                "MCIncineration" or
                "MeltingRay" or
                "MCMeltingRay" or
                "MagmaRain" or
                "MCMagmaRain" or
                "Inferno" or
                "MCInferno" or
                "RunicBoulder" or
                "MCRunicBoulder" or
                "StoneSpikes" or
                "MCStoneSpikes" or
                "Stone_Armor" or
                "MCStoneArmor" or
                "Petrification" or
                "MCPetrification" or
                "EarthQuake" or
                "MCEarthQuake" or
                "RunicExplosion" or
                "MCRunicExplosion" or
                "BoulderToss" or
                "MCBoulderToss" or
                "Discharge" or
                "MCDischarge" or
                "Impulse" or
                "MCImpulse" or
                "StaticField" or
                "MCStaticField" or
                "ShortCircuit" or
                "MCShortCircuit" or
                "BallLightning" or
                "MCBallLightning" or
                "ChainLightning" or
                "MCChainLightning" or
                "Tempest" or
                "MCTempest" or
                "SealOfFinesse" or
                "MCSealOfFinesse" or
                "SealOfPower" or
                "MCSealOfPower" or
                "SealOfInsight" or
                "MCSealOfInsight" or
                "SealOfCleansing" or
                "MCSealOfCleansing" or
                "SealOfReflection" or
                "MCSealOfReflection" or
                "SealOfShackles" or
                "MCSealOfShackles" or
                "Darkbolt" or
                "Curse" or
                "DarkResurrection" or
                "DarkBlessing" or
                "SignOfDarkness" or
                "DeathTouch" or
                "SoulSacrifice" or
                "SigilOfBinding" or
                "MarkOfTheFeast" or
                "SacrificialBlood" or
                "SummonBloodGolem" or
                "VampireRune" or
                "LifeLeech"
                 => $";{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                "Chance" => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableSpeechBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        # region API METHODS
        public LocalizableSpeechBuilder WithKill(LocalizedStrings text) => Add("kill", text);
        public LocalizableSpeechBuilder WithKillVelmir(LocalizedStrings text) => Add("killVelmir", text);
        public LocalizableSpeechBuilder WithKillArna(LocalizedStrings text) => Add("killArna", text);
        public LocalizableSpeechBuilder WithKillJonna(LocalizedStrings text) => Add("killJonna", text);
        public LocalizableSpeechBuilder WithKillJorgrim(LocalizedStrings text) => Add("killJorgrim", text);
        public LocalizableSpeechBuilder WithKillDirwin(LocalizedStrings text) => Add("killDirwin", text);
        public LocalizableSpeechBuilder WithKillAllies(LocalizedStrings text) => Add("killAllies", text);
        public LocalizableSpeechBuilder WithKillAlliesJorgrim(LocalizedStrings text) => Add("killAlliesJorgrim", text);
        public LocalizableSpeechBuilder WithKillCrit(LocalizedStrings text) => Add("killCrit", text);
        public LocalizableSpeechBuilder WithKillCritJorgrim(LocalizedStrings text) => Add("killCritJorgrim", text);
        public LocalizableSpeechBuilder WithKillCritVelmir(LocalizedStrings text) => Add("killCritVelmir", text);
        public LocalizableSpeechBuilder WithKillCritBlade(LocalizedStrings text) => Add("killCritBlade", text);
        public LocalizableSpeechBuilder WithKillCritShot(LocalizedStrings text) => Add("killCritShot", text);
        public LocalizableSpeechBuilder WithKillCritShotDirwin(LocalizedStrings text) => Add("killCritShotDirwin", text);
        public LocalizableSpeechBuilder WithThrowDamage(LocalizedStrings text) => Add("throwDamage", text);
        public LocalizableSpeechBuilder WithHealthLoss(LocalizedStrings text) => Add("healthLoss", text);
        public LocalizableSpeechBuilder WithHitMissed(LocalizedStrings text) => Add("hitMissed", text);
        public LocalizableSpeechBuilder WithHitDodged(LocalizedStrings text) => Add("hitDodged", text);
        public LocalizableSpeechBuilder WithHitFumbled(LocalizedStrings text) => Add("hitFumbled", text);
        public LocalizableSpeechBuilder WithHitFullyBlocked(LocalizedStrings text) => Add("hitFullyBlocked", text);
        public LocalizableSpeechBuilder WithCounterCrit(LocalizedStrings text) => Add("counterCrit", text);
        public LocalizableSpeechBuilder WithDodge(LocalizedStrings text) => Add("dodge", text);
        public LocalizableSpeechBuilder WithInjuryLight(LocalizedStrings text) => Add("injuryLight", text);
        public LocalizableSpeechBuilder WithInjuryTorso(LocalizedStrings text) => Add("injuryTorso", text);
        public LocalizableSpeechBuilder WithInjuryLeg(LocalizedStrings text) => Add("injuryLeg", text);
        public LocalizableSpeechBuilder WithInjuryArm(LocalizedStrings text) => Add("injuryArm", text);
        public LocalizableSpeechBuilder WithInjuryHead(LocalizedStrings text) => Add("injuryHead", text);
        public LocalizableSpeechBuilder WithDebuffDaze(LocalizedStrings text) => Add("debuffDaze", text);
        public LocalizableSpeechBuilder WithDebuffWet(LocalizedStrings text) => Add("debuffWet", text);
        public LocalizableSpeechBuilder WithDebuffAftermath(LocalizedStrings text) => Add("debuffAftermath", text);
        public LocalizableSpeechBuilder WithDebuffAftermathJorgrim(LocalizedStrings text) => Add("debuffAftermath_Jorgrim", text);
        public LocalizableSpeechBuilder WithDebuffNet(LocalizedStrings text) => Add("debuffNet", text);
        public LocalizableSpeechBuilder WithDebuffDOT(LocalizedStrings text) => Add("debuffDOT", text);
        public LocalizableSpeechBuilder WithDebuffFire(LocalizedStrings text) => Add("debuffFire", text);
        public LocalizableSpeechBuilder WithDebuffAcid(LocalizedStrings text) => Add("debuffAcid", text);
        public LocalizableSpeechBuilder WithDebuffAlco1(LocalizedStrings text) => Add("debuffAlco1", text);
        public LocalizableSpeechBuilder WithDebuffAlco2(LocalizedStrings text) => Add("debuffAlco2", text);
        public LocalizableSpeechBuilder WithDebuffAlco3(LocalizedStrings text) => Add("debuffAlco3", text);
        public LocalizableSpeechBuilder WithDebuffAlco4(LocalizedStrings text) => Add("debuffAlco4", text);
        public LocalizableSpeechBuilder WithUseDrug(LocalizedStrings text) => Add("useDrug", text);
        public LocalizableSpeechBuilder WithUseMedicine(LocalizedStrings text) => Add("useMedicine", text);
        public LocalizableSpeechBuilder WithUseTrap(LocalizedStrings text) => Add("useTrap", text);
        public LocalizableSpeechBuilder WithUseTrapDirwin(LocalizedStrings text) => Add("useTrapDirwin", text);
        public LocalizableSpeechBuilder WithHunger(LocalizedStrings text) => Add("hunger", text);
        public LocalizableSpeechBuilder WithThirsty(LocalizedStrings text) => Add("thirsty", text);
        public LocalizableSpeechBuilder WithPain(LocalizedStrings text) => Add("pain", text);
        public LocalizableSpeechBuilder WithIntoxication(LocalizedStrings text) => Add("intoxication", text);
        public LocalizableSpeechBuilder WithFatigue(LocalizedStrings text) => Add("fatigue", text);
        public LocalizableSpeechBuilder WithBleeding(LocalizedStrings text) => Add("bleeding", text);
        public LocalizableSpeechBuilder WithBleedingStacks(LocalizedStrings text) => Add("bleedingStacks", text);
        public LocalizableSpeechBuilder WithLockPicked(LocalizedStrings text) => Add("lockPicked", text);
        public LocalizableSpeechBuilder WithLockFailed(LocalizedStrings text) => Add("lockFailed", text);
        public LocalizableSpeechBuilder WithDoorDestroyed(LocalizedStrings text) => Add("doorDestroyed", text);
        public LocalizableSpeechBuilder WithTrapFind(LocalizedStrings text) => Add("trapFind", text);
        public LocalizableSpeechBuilder WithTrapHit(LocalizedStrings text) => Add("trapHit", text);
        public LocalizableSpeechBuilder WithTrapDisarm(LocalizedStrings text) => Add("trapDisarm", text);
        public LocalizableSpeechBuilder WithTrapAvoid(LocalizedStrings text) => Add("trapAvoid", text);
        public LocalizableSpeechBuilder WithInterruption(LocalizedStrings text) => Add("interruption", text);
        public LocalizableSpeechBuilder WithAmbush(LocalizedStrings text) => Add("Ambush", text);
        public LocalizableSpeechBuilder WithContainer(LocalizedStrings text) => Add("container", text);
        public LocalizableSpeechBuilder WithContainerRich(LocalizedStrings text) => Add("containerRich", text);
        public LocalizableSpeechBuilder WithOptimism(LocalizedStrings text) => Add("optimism", text);
        public LocalizableSpeechBuilder WithHighBackfireDmg(LocalizedStrings text) => Add("highBackfireDmg", text);
        public LocalizableSpeechBuilder WithHeroism(LocalizedStrings text) => Add("heroism", text);
        public LocalizableSpeechBuilder WithSecondWind(LocalizedStrings text) => Add("secondWind", text);
        public LocalizableSpeechBuilder WithPrudence(LocalizedStrings text) => Add("prudence", text);
        public LocalizableSpeechBuilder WithSwimLowMP(LocalizedStrings text) => Add("swimLowMP", text);
        public LocalizableSpeechBuilder WithVampireSuspicious(LocalizedStrings text) => Add("vampireSuspicious", text);
        public LocalizableSpeechBuilder WithVampireHostile(LocalizedStrings text) => Add("vampireHostile", text);
        public LocalizableSpeechBuilder WithVampireAttack(LocalizedStrings text) => Add("vampireAttack", text);
        public LocalizableSpeechBuilder WithVampireSkill(LocalizedStrings text) => Add("vampireSkill", text);
        public LocalizableSpeechBuilder WithVampireDeath(LocalizedStrings text) => Add("vampireDeath", text);
        public LocalizableSpeechBuilder WithVampireAllyDeath(LocalizedStrings text) => Add("vampireAllyDeath", text);
        public LocalizableSpeechBuilder WithVampireInjury(LocalizedStrings text) => Add("vampireInjury", text);
        public LocalizableSpeechBuilder WithVampireLordAlarm(LocalizedStrings text) => Add("vampireLordAlarm", text);
        public LocalizableSpeechBuilder WithIdleUndead(LocalizedStrings text) => Add("idleUndead", text);
        public LocalizableSpeechBuilder WithAlarmUndead(LocalizedStrings text) => Add("alarmUndead", text);
        public LocalizableSpeechBuilder WithHostileUndead(LocalizedStrings text) => Add("hostileUndead", text);
        public LocalizableSpeechBuilder WithAttackUndead(LocalizedStrings text) => Add("attackUndead", text);
        public LocalizableSpeechBuilder WithMoveUndead(LocalizedStrings text) => Add("moveUndead", text);
        public LocalizableSpeechBuilder WithInjuryUndead(LocalizedStrings text) => Add("injuryUndead", text);
        public LocalizableSpeechBuilder WithDeathUndead(LocalizedStrings text) => Add("deathUndead", text);
        public LocalizableSpeechBuilder WithHostileNecromancer(LocalizedStrings text) => Add("hostileNecromancer", text);
        public LocalizableSpeechBuilder WithAttackNecromancer(LocalizedStrings text) => Add("attackNecromancer", text);
        public LocalizableSpeechBuilder WithIdleBandit(LocalizedStrings text) => Add("idleBandit", text);
        public LocalizableSpeechBuilder WithIdleBanditWeather(LocalizedStrings text) => Add("idleBanditWeather", text);
        public LocalizableSpeechBuilder WithIdleBossBandit(LocalizedStrings text) => Add("idleBossBandit", text);
        public LocalizableSpeechBuilder WithAlarmBandit(LocalizedStrings text) => Add("alarmBandit", text);
        public LocalizableSpeechBuilder WithHostileSurfaceAlliesBandit(LocalizedStrings text) => Add("hostileSurfaceAlliesBandit", text);
        public LocalizableSpeechBuilder WithHostileSurfaceAlliesBanditDwarf(LocalizedStrings text) => Add("hostileSurfaceAlliesBandit_Dwarf", text);
        public LocalizableSpeechBuilder WithHostileAlliesBandit(LocalizedStrings text) => Add("hostileAlliesBandit", text);
        public LocalizableSpeechBuilder WithHostileMageBandit(LocalizedStrings text) => Add("hostileMageBandit", text);
        public LocalizableSpeechBuilder WithCastingMageBandit(LocalizedStrings text) => Add("castingMageBandit", text);
        public LocalizableSpeechBuilder WithMiscastMageBandit(LocalizedStrings text) => Add("miscastMageBandit", text);
        public LocalizableSpeechBuilder WithMiracleBandit(LocalizedStrings text) => Add("miracleBandit", text);
        public LocalizableSpeechBuilder WithMiracleBanditAllies(LocalizedStrings text) => Add("miracleBanditAllies", text);
        public LocalizableSpeechBuilder WithCastingBandit(LocalizedStrings text) => Add("castingBandit", text);
        public LocalizableSpeechBuilder WithCastingAlliesBandit(LocalizedStrings text) => Add("castingAlliesBandit", text);
        public LocalizableSpeechBuilder WithChargeBandit(LocalizedStrings text) => Add("chargeBandit", text);
        public LocalizableSpeechBuilder WithBottleThrowBandit(LocalizedStrings text) => Add("bottleThrowBandit", text);
        public LocalizableSpeechBuilder WithFireBandit(LocalizedStrings text) => Add("fireBandit", text);
        public LocalizableSpeechBuilder WithFireAlliesBandit(LocalizedStrings text) => Add("fireAlliesBandit", text);
        public LocalizableSpeechBuilder WithFireDeathBandit(LocalizedStrings text) => Add("fireDeathBandit", text);
        public LocalizableSpeechBuilder WithCombatBeastBandit(LocalizedStrings text) => Add("combatBeastBandit", text);
        public LocalizableSpeechBuilder WithRetreatAlliesBandit(LocalizedStrings text) => Add("retreatAlliesBandit", text);
        public LocalizableSpeechBuilder WithAggroStrongBandit(LocalizedStrings text) => Add("aggroStrongBandit", text);
        public LocalizableSpeechBuilder WithTrapBandit(LocalizedStrings text) => Add("trapBandit", text);
        public LocalizableSpeechBuilder WithTrapDisarmBandit(LocalizedStrings text) => Add("trapDisarmBandit", text);
        public LocalizableSpeechBuilder WithHostileBandit(LocalizedStrings text) => Add("hostileBandit", text);
        public LocalizableSpeechBuilder WithAttackBandit(LocalizedStrings text) => Add("attackBandit", text);
        public LocalizableSpeechBuilder WithAttackskillAlliesBandit(LocalizedStrings text) => Add("attackskillAlliesBandit", text);
        public LocalizableSpeechBuilder WithDeathAlliesBandit(LocalizedStrings text) => Add("deathAlliesBandit", text);
        public LocalizableSpeechBuilder WithDoorAlarmBandit(LocalizedStrings text) => Add("doorAlarmBandit", text);
        public LocalizableSpeechBuilder WithDoorHostileBandit(LocalizedStrings text) => Add("doorHostileBandit", text);
        public LocalizableSpeechBuilder WithInjuryBandit(LocalizedStrings text) => Add("injuryBandit", text);
        public LocalizableSpeechBuilder WithInjuryHeadBandit(LocalizedStrings text) => Add("injuryHeadBandit", text);
        public LocalizableSpeechBuilder WithInjuryHandBandit(LocalizedStrings text) => Add("injuryHandBandit", text);
        public LocalizableSpeechBuilder WithInjuryLegBandit(LocalizedStrings text) => Add("injuryLegBandit", text);
        public LocalizableSpeechBuilder WithBleedBandit(LocalizedStrings text) => Add("bleedBandit", text);
        public LocalizableSpeechBuilder WithInjuryTorsoBandit(LocalizedStrings text) => Add("injuryTorsoBandit", text);
        public LocalizableSpeechBuilder WithDeathBandit(LocalizedStrings text) => Add("deathBandit", text);
        public LocalizableSpeechBuilder WithRetreatBandit(LocalizedStrings text) => Add("retreatBandit", text);
        public LocalizableSpeechBuilder WithAttackFreak(LocalizedStrings text) => Add("attackFreak", text);
        public LocalizableSpeechBuilder WithNpcNoTurn(LocalizedStrings text) => Add("npcNoTurn", text);
        public LocalizableSpeechBuilder WithNpcRain1(LocalizedStrings text) => Add("npcRain1", text);
        public LocalizableSpeechBuilder WithNpcRain2(LocalizedStrings text) => Add("npcRain2", text);
        public LocalizableSpeechBuilder WithNpcRain3(LocalizedStrings text) => Add("npcRain3", text);
        public LocalizableSpeechBuilder WithNpcPeasant(LocalizedStrings text) => Add("npcPeasant", text);
        public LocalizableSpeechBuilder WithNpcGuardNeutral(LocalizedStrings text) => Add("npcGuardNeutral", text);
        public LocalizableSpeechBuilder WithNpcGuardTrueneutral(LocalizedStrings text) => Add("npcGuardTrueneutral", text);
        public LocalizableSpeechBuilder WithNpcGuardAdmiration(LocalizedStrings text) => Add("npcGuardAdmiration", text);
        public LocalizableSpeechBuilder WithNpcGuardAmity(LocalizedStrings text) => Add("npcGuardAmity", text);
        public LocalizableSpeechBuilder WithNpcGuardRespect(LocalizedStrings text) => Add("npcGuardRespect", text);
        public LocalizableSpeechBuilder WithNpcGuardDislike(LocalizedStrings text) => Add("npcGuardDislike", text);
        public LocalizableSpeechBuilder WithNpcGuardHatred(LocalizedStrings text) => Add("npcGuardHatred", text);
        public LocalizableSpeechBuilder WithNpcPeasantNeutral(LocalizedStrings text) => Add("npcPeasantNeutral", text);
        public LocalizableSpeechBuilder WithNpcPeasantTrueneutral(LocalizedStrings text) => Add("npcPeasantTrueneutral", text);
        public LocalizableSpeechBuilder WithNpcPeasantAdmiration(LocalizedStrings text) => Add("npcPeasantAdmiration", text);
        public LocalizableSpeechBuilder WithNpcPeasantAmity(LocalizedStrings text) => Add("npcPeasantAmity", text);
        public LocalizableSpeechBuilder WithNpcPeasantRespect(LocalizedStrings text) => Add("npcPeasantRespect", text);
        public LocalizableSpeechBuilder WithNpcPeasantDislike(LocalizedStrings text) => Add("npcPeasantDislike", text);
        public LocalizableSpeechBuilder WithNpcPeasantHatred(LocalizedStrings text) => Add("npcPeasantHatred", text);
        public LocalizableSpeechBuilder WithNpcPeasantMale(LocalizedStrings text) => Add("npcPeasantMale", text);
        public LocalizableSpeechBuilder WithNpcLowhp(LocalizedStrings text) => Add("npcLowhp", text);
        public LocalizableSpeechBuilder WithNpcTrader(LocalizedStrings text) => Add("npcTrader", text);
        public LocalizableSpeechBuilder WithNpcWineTrader(LocalizedStrings text) => Add("npcWineTrader", text);
        public LocalizableSpeechBuilder WithNpcFoodTrader(LocalizedStrings text) => Add("npcFoodTrader", text);
        public LocalizableSpeechBuilder WithNpcVegetableTrader(LocalizedStrings text) => Add("npcVegetableTrader", text);
        public LocalizableSpeechBuilder WithNpcCheeseTrader(LocalizedStrings text) => Add("npcCheeseTrader", text);
        public LocalizableSpeechBuilder WithNpcFruitTrader(LocalizedStrings text) => Add("npcFruitTrader", text);
        public LocalizableSpeechBuilder WithNpcButcher(LocalizedStrings text) => Add("npcButcher", text);
        public LocalizableSpeechBuilder WithNpcSmith(LocalizedStrings text) => Add("npcSmith", text);
        public LocalizableSpeechBuilder WithNpcCarpenter(LocalizedStrings text) => Add("npcCarpenter", text);
        public LocalizableSpeechBuilder WithNpcTailor(LocalizedStrings text) => Add("npcTailor", text);
        public LocalizableSpeechBuilder WithNpcElder(LocalizedStrings text) => Add("npcElder", text);
        public LocalizableSpeechBuilder WithNpcHerbalist(LocalizedStrings text) => Add("npcHerbalist", text);
        public LocalizableSpeechBuilder WithNpcDrunkard(LocalizedStrings text) => Add("npcDrunkard", text);
        public LocalizableSpeechBuilder WithNpcCaptainTraining(LocalizedStrings text) => Add("npcCaptainTraining", text);
        public LocalizableSpeechBuilder WithNpcMiller(LocalizedStrings text) => Add("npcMiller", text);
        public LocalizableSpeechBuilder WithNpcHierophant(LocalizedStrings text) => Add("npcHierophant", text);
        public LocalizableSpeechBuilder WithNpcMercTrader(LocalizedStrings text) => Add("npcMercTrader", text);
        public LocalizableSpeechBuilder WithNpcMercRepair(LocalizedStrings text) => Add("npcMercRepair", text);
        public LocalizableSpeechBuilder WithNpcMercElder(LocalizedStrings text) => Add("npcMercElder", text);
        public LocalizableSpeechBuilder WithDoorBarred(LocalizedStrings text) => Add("doorBarred", text);
        public LocalizableSpeechBuilder WithHatLine(LocalizedStrings text) => Add("hatLine", text);
        public LocalizableSpeechBuilder WithHatLineMiscast(LocalizedStrings text) => Add("hatLineMiscast", text);
        public LocalizableSpeechBuilder WithHatLineMiracle(LocalizedStrings text) => Add("hatLineMiracle", text);
        public LocalizableSpeechBuilder WithDoorClosed(LocalizedStrings text) => Add("doorClosed", text);
        public LocalizableSpeechBuilder WithNpcGuardSergeant(LocalizedStrings text) => Add("npcGuardSergeant", text);
        public LocalizableSpeechBuilder WithPlayerBed(LocalizedStrings text) => Add("playerBed", text);
        public LocalizableSpeechBuilder WithPlayerBedKh(LocalizedStrings text) => Add("playerBedKh", text);
        public LocalizableSpeechBuilder WithPlayerBedCrime(LocalizedStrings text) => Add("playerBedCrime", text);
        public LocalizableSpeechBuilder WithPlayerShout(LocalizedStrings text) => Add("playerShout", text);
        public LocalizableSpeechBuilder WithPrayHuman(LocalizedStrings text) => Add("prayHuman", text);
        public LocalizableSpeechBuilder WithPrayHumanCD(LocalizedStrings text) => Add("prayHumanCD", text);
        public LocalizableSpeechBuilder WithPrayElf(LocalizedStrings text) => Add("prayElf", text);
        public LocalizableSpeechBuilder WithPrayElfCD(LocalizedStrings text) => Add("prayElfCD", text);
        public LocalizableSpeechBuilder WithPrayDwarf(LocalizedStrings text) => Add("prayDwarf", text);
        public LocalizableSpeechBuilder WithPrayDwarfCD(LocalizedStrings text) => Add("prayDwarfCD", text);
        public LocalizableSpeechBuilder WithPrayDwarfShrine(LocalizedStrings text) => Add("prayDwarfShrine", text);
        public LocalizableSpeechBuilder WithPlayerDigHands(LocalizedStrings text) => Add("playerDigHands", text);
        public LocalizableSpeechBuilder WithPlayerGraveDig(LocalizedStrings text) => Add("playerGraveDig", text);
        public LocalizableSpeechBuilder WithPlayerCook(LocalizedStrings text) => Add("playerCook", text);
        public LocalizableSpeechBuilder WithLeprosyDoor(LocalizedStrings text) => Add("leprosyDoor", text);
        public LocalizableSpeechBuilder WithSecretRoom(LocalizedStrings text) => Add("secretRoom", text);
        public LocalizableSpeechBuilder WithHostage(LocalizedStrings text) => Add("hostage", text);
        public LocalizableSpeechBuilder WithNoPelt(LocalizedStrings text) => Add("noPelt", text);
        public LocalizableSpeechBuilder WithCough(LocalizedStrings text) => Add("cough", text);
        public LocalizableSpeechBuilder WithStash(LocalizedStrings text) => Add("stash", text);
        public LocalizableSpeechBuilder WithNpcNotalk(LocalizedStrings text) => Add("npcNotalk", text);
        public LocalizableSpeechBuilder WithNpcNotalkHostile(LocalizedStrings text) => Add("npcNotalkHostile", text);
        public LocalizableSpeechBuilder WithVerrenNotalkHostile(LocalizedStrings text) => Add("verrenNotalkHostile", text);
        public LocalizableSpeechBuilder WithNpcCrimeAlarm(LocalizedStrings text) => Add("npcCrimeAlarm", text);
        public LocalizableSpeechBuilder WithNpcCrimeTheft(LocalizedStrings text) => Add("npcCrimeTheft", text);
        public LocalizableSpeechBuilder WithNpcCrimeBrawl(LocalizedStrings text) => Add("npcCrimeBrawl", text);
        public LocalizableSpeechBuilder WithNpcCrimeGeneric(LocalizedStrings text) => Add("npcCrimeGeneric", text);
        public LocalizableSpeechBuilder WithNpcCrimeKill(LocalizedStrings text) => Add("npcCrimeKill", text);
        public LocalizableSpeechBuilder WithNpcCrimeGuard(LocalizedStrings text) => Add("npcCrimeGuard", text);
        public LocalizableSpeechBuilder WithNpcCrimeBrawlTarget0(LocalizedStrings text) => Add("npcCrimeBrawlTarget0", text);
        public LocalizableSpeechBuilder WithNpcCrimeBrawlTarget1(LocalizedStrings text) => Add("npcCrimeBrawlTarget1", text);
        public LocalizableSpeechBuilder WithNpcPest(LocalizedStrings text) => Add("npcPest", text);
        public LocalizableSpeechBuilder WithNpcNightmares(LocalizedStrings text) => Add("npcNightmares", text);
        public LocalizableSpeechBuilder WithNpcAmbushes(LocalizedStrings text) => Add("npcAmbushes", text);
        public LocalizableSpeechBuilder WithNpcJerald(LocalizedStrings text) => Add("npcJerald", text);
        public LocalizableSpeechBuilder WithNpcLeifIntro(LocalizedStrings text) => Add("npcLeifIntro", text);
        public LocalizableSpeechBuilder WithNpcMercIntro(LocalizedStrings text) => Add("npcMercIntro", text);
        public LocalizableSpeechBuilder WithSceneGeneric(LocalizedStrings text) => Add("sceneGeneric", text);
        public LocalizableSpeechBuilder WithSceneDarrelOldcaravan(LocalizedStrings text) => Add("sceneDarrelOldcaravan", text);
        public LocalizableSpeechBuilder WithNpcTraderSkadia(LocalizedStrings text) => Add("npcTraderSkadia", text);
        public LocalizableSpeechBuilder WithNpcTraderNistra(LocalizedStrings text) => Add("npcTraderNistra", text);
        public LocalizableSpeechBuilder WithNpcTraderFjall(LocalizedStrings text) => Add("npcTraderFjall", text);
        public LocalizableSpeechBuilder WithNpcElfBarker(LocalizedStrings text) => Add("npcElfBarker", text);
        public LocalizableSpeechBuilder WithNpcTraderJibey(LocalizedStrings text) => Add("npcTraderJibey", text);
        public LocalizableSpeechBuilder WithInterruptVerren(LocalizedStrings text) => Add("interruptVerren", text);
        public LocalizableSpeechBuilder WithExitGuinnelHouse(LocalizedStrings text) => Add("exitGuinnelHouse", text);
        public LocalizableSpeechBuilder WithExamGuinnelHouseApparatus01(LocalizedStrings text) => Add("examGuinnelHouseApparatus01", text);
        public LocalizableSpeechBuilder WithExamGuinnelHouseFlasks(LocalizedStrings text) => Add("examGuinnelHouseFlasks", text);
        public LocalizableSpeechBuilder WithExamGuinnelHousePapers(LocalizedStrings text) => Add("examGuinnelHousePapers", text);
        public LocalizableSpeechBuilder WithExamGuinnelHouseApparatus02(LocalizedStrings text) => Add("examGuinnelHouseApparatus02", text);
        public LocalizableSpeechBuilder WithExamGuinnelHouseTable(LocalizedStrings text) => Add("examGuinnelHouseTable", text);
        public LocalizableSpeechBuilder WithGuinnelVerrenComment01(LocalizedStrings text) => Add("guinnelVerrenComment01", text);
        public LocalizableSpeechBuilder WithGuinnelVerrenComment02(LocalizedStrings text) => Add("guinnelVerrenComment02", text);
        public LocalizableSpeechBuilder WithLowcreyInterrupt(LocalizedStrings text) => Add("lowcreyInterrupt", text);
        public LocalizableSpeechBuilder WithDoorFuneralCave(LocalizedStrings text) => Add("doorFuneralCave", text);
        public LocalizableSpeechBuilder WithBarrierCradle(LocalizedStrings text) => Add("barrierCradle", text);
        public LocalizableSpeechBuilder WithEssenceAlert(LocalizedStrings text) => Add("essenceAlert", text);
        public LocalizableSpeechBuilder WithManticoreDeath(LocalizedStrings text) => Add("manticoreDeath", text);
        public LocalizableSpeechBuilder WithManticoreDisenchant(LocalizedStrings text) => Add("manticoreDisenchant", text);
        public LocalizableSpeechBuilder WithOrderPrisonShout(LocalizedStrings text) => Add("orderPrisonShout", text);
        public LocalizableSpeechBuilder WithOrderPrisonShoutInside(LocalizedStrings text) => Add("orderPrisonShoutInside", text);
        public LocalizableSpeechBuilder WithOrderPrisonMercHarpy(LocalizedStrings text) => Add("orderPrisonMercHarpy", text);
        public LocalizableSpeechBuilder WithRunicStonesAltar(LocalizedStrings text) => Add("runicStonesAltar", text);
        public LocalizableSpeechBuilder WithExamineWell(LocalizedStrings text) => Add("examineWell", text);
        public LocalizableSpeechBuilder WithBedrollAmbush(LocalizedStrings text) => Add("bedrollAmbush", text);
        public LocalizableSpeechBuilder WithCantExit(LocalizedStrings text) => Add("cantExit", text);
        public LocalizableSpeechBuilder WithUnholyAltarNoRelic(LocalizedStrings text) => Add("unholyAltarNoRelic", text);
        public LocalizableSpeechBuilder WithBodiesNoOil(LocalizedStrings text) => Add("bodiesNoOil", text);
        public LocalizableSpeechBuilder WithBloodFeverBleed(LocalizedStrings text) => Add("bloodFeverBleed", text);
        public LocalizableSpeechBuilder WithBedrollDebuff(LocalizedStrings text) => Add("bedrollDebuff", text);
        public LocalizableSpeechBuilder WithNightmaresSleep(LocalizedStrings text) => Add("nightmaresSleep", text);
        public LocalizableSpeechBuilder WithNoPickaxe(LocalizedStrings text) => Add("noPickaxe", text);
        public LocalizableSpeechBuilder WithFireBarrage(LocalizedStrings text) => Add("FireBarrage", text);
        public LocalizableSpeechBuilder WithMCFireBarrage(LocalizedStrings text) => Add("MCFireBarrage", text);
        public LocalizableSpeechBuilder WithFlameWave(LocalizedStrings text) => Add("FlameWave", text);
        public LocalizableSpeechBuilder WithMCFlameWave(LocalizedStrings text) => Add("MCFlameWave", text);
        public LocalizableSpeechBuilder WithFlameRing(LocalizedStrings text) => Add("FlameRing", text);
        public LocalizableSpeechBuilder WithMCFlameRing(LocalizedStrings text) => Add("MCFlameRing", text);
        public LocalizableSpeechBuilder WithIncineration(LocalizedStrings text) => Add("Incineration", text);
        public LocalizableSpeechBuilder WithMCIncineration(LocalizedStrings text) => Add("MCIncineration", text);
        public LocalizableSpeechBuilder WithMeltingRay(LocalizedStrings text) => Add("MeltingRay", text);
        public LocalizableSpeechBuilder WithMCMeltingRay(LocalizedStrings text) => Add("MCMeltingRay", text);
        public LocalizableSpeechBuilder WithMagmaRain(LocalizedStrings text) => Add("MagmaRain", text);
        public LocalizableSpeechBuilder WithMCMagmaRain(LocalizedStrings text) => Add("MCMagmaRain", text);
        public LocalizableSpeechBuilder WithInferno(LocalizedStrings text) => Add("Inferno", text);
        public LocalizableSpeechBuilder WithMCInferno(LocalizedStrings text) => Add("MCInferno", text);
        public LocalizableSpeechBuilder WithRunicBoulder(LocalizedStrings text) => Add("RunicBoulder", text);
        public LocalizableSpeechBuilder WithMCRunicBoulder(LocalizedStrings text) => Add("MCRunicBoulder", text);
        public LocalizableSpeechBuilder WithStoneSpikes(LocalizedStrings text) => Add("StoneSpikes", text);
        public LocalizableSpeechBuilder WithMCStoneSpikes(LocalizedStrings text) => Add("MCStoneSpikes", text);
        public LocalizableSpeechBuilder WithStoneArmor(LocalizedStrings text) => Add("Stone_Armor", text);
        public LocalizableSpeechBuilder WithMCStoneArmor(LocalizedStrings text) => Add("MCStoneArmor", text);
        public LocalizableSpeechBuilder WithPetrification(LocalizedStrings text) => Add("Petrification", text);
        public LocalizableSpeechBuilder WithMCPetrification(LocalizedStrings text) => Add("MCPetrification", text);
        public LocalizableSpeechBuilder WithEarthQuake(LocalizedStrings text) => Add("EarthQuake", text);
        public LocalizableSpeechBuilder WithMCEarthQuake(LocalizedStrings text) => Add("MCEarthQuake", text);
        public LocalizableSpeechBuilder WithRunicExplosion(LocalizedStrings text) => Add("RunicExplosion", text);
        public LocalizableSpeechBuilder WithMCRunicExplosion(LocalizedStrings text) => Add("MCRunicExplosion", text);
        public LocalizableSpeechBuilder WithBoulderToss(LocalizedStrings text) => Add("BoulderToss", text);
        public LocalizableSpeechBuilder WithMCBoulderToss(LocalizedStrings text) => Add("MCBoulderToss", text);
        public LocalizableSpeechBuilder WithDischarge(LocalizedStrings text) => Add("Discharge", text);
        public LocalizableSpeechBuilder WithMCDischarge(LocalizedStrings text) => Add("MCDischarge", text);
        public LocalizableSpeechBuilder WithImpulse(LocalizedStrings text) => Add("Impulse", text);
        public LocalizableSpeechBuilder WithMCImpulse(LocalizedStrings text) => Add("MCImpulse", text);
        public LocalizableSpeechBuilder WithStaticField(LocalizedStrings text) => Add("StaticField", text);
        public LocalizableSpeechBuilder WithMCStaticField(LocalizedStrings text) => Add("MCStaticField", text);
        public LocalizableSpeechBuilder WithShortCircuit(LocalizedStrings text) => Add("ShortCircuit", text);
        public LocalizableSpeechBuilder WithMCShortCircuit(LocalizedStrings text) => Add("MCShortCircuit", text);
        public LocalizableSpeechBuilder WithBallLightning(LocalizedStrings text) => Add("BallLightning", text);
        public LocalizableSpeechBuilder WithMCBallLightning(LocalizedStrings text) => Add("MCBallLightning", text);
        public LocalizableSpeechBuilder WithChainLightning(LocalizedStrings text) => Add("ChainLightning", text);
        public LocalizableSpeechBuilder WithMCChainLightning(LocalizedStrings text) => Add("MCChainLightning", text);
        public LocalizableSpeechBuilder WithTempest(LocalizedStrings text) => Add("Tempest", text);
        public LocalizableSpeechBuilder WithMCTempest(LocalizedStrings text) => Add("MCTempest", text);
        public LocalizableSpeechBuilder WithSealOfFinesse(LocalizedStrings text) => Add("SealOfFinesse", text);
        public LocalizableSpeechBuilder WithMCSealOfFinesse(LocalizedStrings text) => Add("MCSealOfFinesse", text);
        public LocalizableSpeechBuilder WithSealOfPower(LocalizedStrings text) => Add("SealOfPower", text);
        public LocalizableSpeechBuilder WithMCSealOfPower(LocalizedStrings text) => Add("MCSealOfPower", text);
        public LocalizableSpeechBuilder WithSealOfInsight(LocalizedStrings text) => Add("SealOfInsight", text);
        public LocalizableSpeechBuilder WithMCSealOfInsight(LocalizedStrings text) => Add("MCSealOfInsight", text);
        public LocalizableSpeechBuilder WithSealOfCleansing(LocalizedStrings text) => Add("SealOfCleansing", text);
        public LocalizableSpeechBuilder WithMCSealOfCleansing(LocalizedStrings text) => Add("MCSealOfCleansing", text);
        public LocalizableSpeechBuilder WithSealOfReflection(LocalizedStrings text) => Add("SealOfReflection", text);
        public LocalizableSpeechBuilder WithMCSealOfReflection(LocalizedStrings text) => Add("MCSealOfReflection", text);
        public LocalizableSpeechBuilder WithSealOfShackles(LocalizedStrings text) => Add("SealOfShackles", text);
        public LocalizableSpeechBuilder WithMCSealOfShackles(LocalizedStrings text) => Add("MCSealOfShackles", text);
        public LocalizableSpeechBuilder WithDarkbolt(LocalizedStrings text) => Add("Darkbolt", text);
        public LocalizableSpeechBuilder WithCurse(LocalizedStrings text) => Add("Curse", text);
        public LocalizableSpeechBuilder WithDarkResurrection(LocalizedStrings text) => Add("DarkResurrection", text);
        public LocalizableSpeechBuilder WithDarkBlessing(LocalizedStrings text) => Add("DarkBlessing", text);
        public LocalizableSpeechBuilder WithSignOfDarkness(LocalizedStrings text) => Add("SignOfDarkness", text);
        public LocalizableSpeechBuilder WithDeathTouch(LocalizedStrings text) => Add("DeathTouch", text);
        public LocalizableSpeechBuilder WithSoulSacrifice(LocalizedStrings text) => Add("SoulSacrifice", text);
        public LocalizableSpeechBuilder WithSigilOfBinding(LocalizedStrings text) => Add("SigilOfBinding", text);
        public LocalizableSpeechBuilder WithMarkOfTheFeast(LocalizedStrings text) => Add("MarkOfTheFeast", text);
        public LocalizableSpeechBuilder WithSacrificialBlood(LocalizedStrings text) => Add("SacrificialBlood", text);
        public LocalizableSpeechBuilder WithSummonBloodGolem(LocalizedStrings text) => Add("SummonBloodGolem", text);
        public LocalizableSpeechBuilder WithVampireRune(LocalizedStrings text) => Add("VampireRune", text);
        public LocalizableSpeechBuilder WithLifeLeech(LocalizedStrings text) => Add("LifeLeech", text);
        public LocalizableSpeechBuilder WithChance(LocalizedStrings text) => Add("Chance", text);
        #endregion
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableSpeech(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable effect: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable effect: Nothing to inject.");
            }
        }
    }

    // API method exposed to modders
    public static LocalizableSpeechBuilder InjectTableLocalizableSpeech() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableSpeech(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_speech";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get hook for the key
            string hook = LocalizableSpeechBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found.");;
            
            // Prepare line
            string newline = LocalizableSpeechBuilder.FormatLineForKey(key, id, text);
            
            // Add line to table
            if (key == "npcCrimeBrawlTarget1")
                table.Insert(ind + 2, newline);
            else
                table.Insert(ind, newline);
            Log.Information($"Injected {key} into table '{tableName}' at hook '{hook}'.");
        }
        ModLoader.SetTable(table, tableName);
    }
}