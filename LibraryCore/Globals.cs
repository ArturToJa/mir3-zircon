﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Library.Network;
using Library.Network.ServerPackets;
using Library.SystemModels;
using MirDB;

namespace Library
{
    public static class Globals
    {
        public static ItemInfo GoldInfo;

        public static DBCollection<ItemInfo> ItemInfoList;
        public static DBCollection<MagicInfo> MagicInfoList;
        public static DBCollection<MapInfo> MapInfoList;
        public static DBCollection<InstanceInfo> InstanceInfoList;
        public static DBCollection<NPCPage> NPCPageList;
        public static DBCollection<MonsterInfo> MonsterInfoList;
        public static DBCollection<StoreInfo> StoreInfoList;
        public static DBCollection<NPCInfo> NPCInfoList;
        public static DBCollection<MovementInfo> MovementInfoList;
        public static DBCollection<QuestInfo> QuestInfoList;
        public static DBCollection<QuestTask> QuestTaskList;
        public static DBCollection<CompanionInfo> CompanionInfoList;
        public static DBCollection<CompanionLevelInfo> CompanionLevelInfoList;

        public static Random Random = new Random();

        public static readonly Regex EMailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.Compiled);
        public static readonly Regex PasswordRegex = new Regex(@"^[\S]{" + MinPasswordLength + "," + MaxPasswordLength + "}$", RegexOptions.Compiled);
        public static readonly Regex CharacterReg = new Regex(@"^[A-Za-z0-9]{" + MinCharacterNameLength + "," + MaxCharacterNameLength + @"}$", RegexOptions.Compiled);
        public static readonly Regex GuildNameRegex = new Regex(@"^[A-Za-z0-9]{" + MinGuildNameLength + "," + MaxGuildNameLength + "}$", RegexOptions.Compiled);

        public static Color NoneColour = Color.White,
                            FireColour = Color.OrangeRed,
                            IceColour = Color.PaleTurquoise,
                            LightningColour = Color.LightSkyBlue,
                            WindColour = Color.LightSeaGreen,
                            HolyColour = Color.DarkKhaki,
                            DarkColour = Color.SaddleBrown,
                            PhantomColour = Color.Purple,

                            BrownNameColour = Color.Brown,
                            RedNameColour = Color.Red;

        public const int
            MinPasswordLength = 5,
            MaxPasswordLength = 15,

            MinRealNameLength = 3,
            MaxRealNameLength = 20,

            MaxEMailLength = 50,

            MinCharacterNameLength = 3,
            MaxCharacterNameLength = 15,
            MaxCharacterCount = 4,

            MinGuildNameLength = 2,
            MaxGuildNameLength = 15,

            MaxChatLength = 120,
            MaxGuildNoticeLength = 4000,

            MaxBeltCount = 10,
            MaxAutoPotionCount = 8,

            MagicRange = 10,

            DuraLossRate = 15,

            GroupLimit = 15,

            CloakRange = 3,
            MarketPlaceFee = 0,
            AccessoryLevelCost = 0,
            AccessoryResetCost = 1000000,

            CraftWeaponPercentCost = 1000000,

            CommonCraftWeaponPercentCost = 30000000,
            SuperiorCraftWeaponPercentCost = 60000000,
            EliteCraftWeaponPercentCost = 80000000;

        public static decimal MarketPlaceTax = 0.07M;  //2.5x Item cost

        public static long
            GuildCreationCost = 7500000,
            GuildMemberCost = 1000000,
            GuildStorageCost = 350000,
            GuildWarCost = 200000;

        public static List<Size> ValidResolutions = new List<Size>
        {
            new Size(1024, 768),
            new Size(1366, 768),
            new Size(1280, 800),
            new Size(1440, 900),
            new Size(1600, 900),
            new Size(1920, 1080),
        };

        public static List<string> Languages = new List<string>
        {
            "English",
            "Chinese",
        };

        public struct RebirthData
        {
            public int RequiredLevel;
            public int LevelLoss;
            public Dictionary<Stat, int> BonusStatistics;
            public RebirthData(int _RequiredLevel, int _LevelLoss, Dictionary<Stat, int> _BonusStatistics)
            {
                RequiredLevel = _RequiredLevel;
                LevelLoss = _LevelLoss;
                BonusStatistics = _BonusStatistics;
            }
        }

        public static List<RebirthData> RebirthDataList = new List<RebirthData>
        {
            new RebirthData(0, 0, new Dictionary<Stat, int>{}), // This is default 0th rebirth, it has no requirements and gives no bonuses
            new RebirthData(60, 0, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 588},
                {Stat.LightningAttack, 5},
                {Stat.FireAttack, 5},
                {Stat.IceAttack, 5},
                {Stat.WindAttack, 5},
                {Stat.DarkAttack, 5},
                {Stat.HolyAttack, 5},
                {Stat.PhantomAttack, 5}}),
            new RebirthData(150, 50, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 1250},
                {Stat.LightningAttack, 10},
                {Stat.FireAttack, 10},
                {Stat.IceAttack, 10},
                {Stat.WindAttack, 10},
                {Stat.DarkAttack, 10},
                {Stat.HolyAttack, 10},
                {Stat.PhantomAttack, 10}}),
            new RebirthData(300, 100, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 2000},
                {Stat.LightningAttack, 15},
                {Stat.FireAttack, 15},
                {Stat.IceAttack, 15},
                {Stat.WindAttack, 15},
                {Stat.DarkAttack, 15},
                {Stat.HolyAttack, 15},
                {Stat.PhantomAttack, 15}}),
            new RebirthData(450, 150, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 2857},
                {Stat.LightningAttack, 20},
                {Stat.FireAttack, 20},
                {Stat.IceAttack, 20},
                {Stat.WindAttack, 20},
                {Stat.DarkAttack, 20},
                {Stat.HolyAttack, 20},
                {Stat.PhantomAttack, 20}}),
            new RebirthData(600, 200, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 3846},
                {Stat.LightningAttack, 25},
                {Stat.FireAttack, 25},
                {Stat.IceAttack, 25},
                {Stat.WindAttack, 25},
                {Stat.DarkAttack, 25},
                {Stat.HolyAttack, 25},
                {Stat.PhantomAttack, 25}}),
            new RebirthData(900, 250, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 5000},
                {Stat.LightningAttack, 30},
                {Stat.FireAttack, 30},
                {Stat.IceAttack, 30},
                {Stat.WindAttack, 30},
                {Stat.DarkAttack, 30},
                {Stat.HolyAttack, 30},
                {Stat.PhantomAttack, 30}}),
            new RebirthData(1200, 400, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 6364},
                {Stat.LightningAttack, 35},
                {Stat.FireAttack, 35},
                {Stat.IceAttack, 35},
                {Stat.WindAttack, 35},
                {Stat.DarkAttack, 35},
                {Stat.HolyAttack, 35},
                {Stat.PhantomAttack, 35}}),
            new RebirthData(1800, 500, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 8000},
                {Stat.LightningAttack, 40},
                {Stat.FireAttack, 40},
                {Stat.IceAttack, 40},
                {Stat.WindAttack, 40},
                {Stat.DarkAttack, 40},
                {Stat.HolyAttack, 40},
                {Stat.PhantomAttack, 40}}),
            new RebirthData(1800, 500, new Dictionary<Stat, int>{
                {Stat.ExperienceRate, 10000},
                {Stat.LightningAttack, 50},
                {Stat.FireAttack, 50},
                {Stat.IceAttack, 50},
                {Stat.WindAttack, 50},
                {Stat.DarkAttack, 50},
                {Stat.HolyAttack, 50},
                {Stat.PhantomAttack, 50}})
        };

        public struct ExperienceData
        {
            public int Level;
            public decimal Experience;
            public ExperienceData(int _Level, decimal _Experience)
            {
                Level = _Level;
                Experience = _Experience;
            }

            public override string ToString()
            {
                return Level.ToString() + " " + Experience.ToString(); 
            }
        }

        public static List<ExperienceData> ExperienceList = new List<ExperienceData>
        {
            new ExperienceData(1, 1),
            new ExperienceData(100, 10000),
            new ExperienceData(1000, 1000000)
        };

        public struct EquipmentUpgradeCost
        {
            public int NumberOfItems;
            public float GoldMultiplier;
            public int SpecialItem;
        }

        public static List<EquipmentUpgradeCost> EquipmentUpgradeList = new List<EquipmentUpgradeCost>
        {
            new EquipmentUpgradeCost
            {
                NumberOfItems = 0,
                GoldMultiplier = 5,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 0,
                GoldMultiplier = 10,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 0,
                GoldMultiplier = 15,
                SpecialItem = 1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 25,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 35,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 50,
                SpecialItem = 1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 65,
                SpecialItem = 2
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 2,
                GoldMultiplier = 80,
                SpecialItem = 2
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 2,
                GoldMultiplier = 100,
                SpecialItem = 3
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 3,
                GoldMultiplier = 125,
                SpecialItem = 3
            },
        };

        public static List<EquipmentUpgradeCost> SpecialEquipmentUpgradeList = new List<EquipmentUpgradeCost>
        {
            new EquipmentUpgradeCost
            {
                NumberOfItems = 0,
                GoldMultiplier = 5,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 0,
                GoldMultiplier = 10,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 0,
                GoldMultiplier = 15,
                SpecialItem = 4
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 20,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 25,
                SpecialItem = -1
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 35,
                SpecialItem = 4
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 1,
                GoldMultiplier = 45,
                SpecialItem = 5
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 2,
                GoldMultiplier = 60,
                SpecialItem = 5
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 2,
                GoldMultiplier = 80,
                SpecialItem = 6
            },
            new EquipmentUpgradeCost
            {
                NumberOfItems = 3,
                GoldMultiplier = 100,
                SpecialItem = 6
            },
        };

        public static Dictionary<int, string> UpgradeSacrificeItems = new Dictionary<int, string>
        {
            { 11, "Demonic Items" },
            { 12, "Striker Items" },
            { 13, "Angelic Items" },
            { 14, "Hazardous Items" },
            { 15, "Vintage Items" },
            { 16, "Gods Items" },
            { 17, "Mystical Items" },
            { 18, "Kings Items" },
            { 120, "Cosmos Parts" },
            { 140, "Chaos Parts" },
            { 160, "Life Parts" },
        };

        public static List<string> UpgradeSpecialItems = new List<string>
        {
            "null",
            "Iron",
            "Black Iron",
            "Forged Steel",
            "Dark Essence",
            "Heaven Essence",
            "Chaotic Essence",
        };

        public static Dictionary<MagicType, MagicType> MagicEnhancement = new Dictionary<MagicType, MagicType>
        {
            {MagicType.Swordsmanship, MagicType.Slaying },
            {MagicType.Thrusting, MagicType.EnhancedThrusting },
            {MagicType.EnhancedThrusting, MagicType.AwakenedThrusting },
            {MagicType.HalfMoon, MagicType.DestructiveSurge },
            {MagicType.FlamingSword, MagicType.EnhancedFlamingSword },
            {MagicType.DragonRise, MagicType.EnhancedDragonRise},
            {MagicType.BladeStorm, MagicType.EnhancedBladeStorm},
            {MagicType.Interchange, MagicType.EnhancedInterchange},
            {MagicType.MassBeckon, MagicType.EnhancedMassBeckon},
            {MagicType.Beckon, MagicType.EnhancedBeckon },
            {MagicType.Defiance, MagicType.EnhancedDefiance},
            {MagicType.Might, MagicType.EnhancedMight},
            {MagicType.ShoulderDash, MagicType.Assault},
            {MagicType.ReflectDamage, MagicType.EnhancedReflectDamage},
            {MagicType.SwiftBlade, MagicType.EnhancedSwiftBlade},
            {MagicType.SeismicSlam, MagicType.EnhancedSeismicSlam},

            {MagicType.FireBall, MagicType.AdamantineFireBall},
            {MagicType.ScortchedEarth, MagicType.EnhancedScorchedEarth},
            {MagicType.FireWall, MagicType.EnhancedFireWall},
            {MagicType.FireStorm, MagicType.Asteroid},
            {MagicType.IceBolt, MagicType.IceBlades},
            {MagicType.FrozenEarth, MagicType.GreaterFrozenEarth},
            {MagicType.IceStorm, MagicType.EnhancedIceStorm},
            {MagicType.LightningBall, MagicType.ThunderBolt},
            {MagicType.LightningWave, MagicType.ChainLightning},
            {MagicType.LightningBeam, MagicType.EnhancedThunderBeam},
            {MagicType.ElectricShock, MagicType.EnhancedElectricShock},
            {MagicType.GustBlast, MagicType.Cyclone},
            {MagicType.BlowEarth, MagicType.EnhancedBlowEarth},
            {MagicType.Tempest, MagicType.EnhancedTempest},
            {MagicType.Renounce, MagicType.EnhancedRenounce},
            {MagicType.Teleportation, MagicType.EnhancedTeleportation},
            {MagicType.GeoManipulation, MagicType.EnhancedGeoManipulation},
            {MagicType.ExpelUndead, MagicType.EnhancedExpelUndead},
            {MagicType.MagicShield, MagicType.EnhancedMagicShield},

            {MagicType.Heal, MagicType.MassHeal},
            {MagicType.PoisonDust, MagicType.EnhancedPoisonDust},
            {MagicType.EvilSlayer, MagicType.GreaterEvilSlayer},
            {MagicType.ExplosiveTalisman, MagicType.ImprovedExplosiveTalisman},
            {MagicType.Resurrection, MagicType.EnhancedResurrection},
            {MagicType.SummonSkeleton, MagicType.SummonShinsu},
            {MagicType.CelestialLight, MagicType.EnhancedCelestialLight},
            {MagicType.BloodLust, MagicType.EnhancedBloodLust},
            {MagicType.ElementalSuperiority, MagicType.EnhancedElementalSuperiority},
            {MagicType.Resilience, MagicType.EnhancedResilience},
            {MagicType.MagicResistance, MagicType.EnhancedMagicResistance},
            {MagicType.Transparency, MagicType.EnhancedTransparency},
            {MagicType.Invisibility, MagicType.MassInvisibility},
            {MagicType.Infection, MagicType.EnhancedInfection},
            {MagicType.TaoistCombatKick, MagicType.EnhancedTaoCombatKick},
        };

        public static Dictionary<MagicType, MagicType> MagicAwakening = new Dictionary<MagicType, MagicType>
        {
            {MagicType.Slaying, MagicType.AwakenedSlaying },
            {MagicType.EnhancedThrusting, MagicType.AwakenedThrusting },
            {MagicType.DestructiveSurge, MagicType.AwakenedDestructiveSurge },
            {MagicType.EnhancedFlamingSword, MagicType.AwakenedFlamingSword },
            {MagicType.EnhancedDragonRise, MagicType.AwakenedDragonRise },
            {MagicType.EnhancedBladeStorm, MagicType.AwakenedBladeStorm },
            {MagicType.EnhancedInterchange, MagicType.AwakenedInterchange },
            {MagicType.EnhancedMassBeckon, MagicType.AwakenedMassBeckon },
            {MagicType.EnhancedBeckon, MagicType.AwakenedBeckon},
            {MagicType.EnhancedDefiance, MagicType.AwakenedDefiance },
            {MagicType.EnhancedMight, MagicType.AwakenedMight },
            {MagicType.Assault, MagicType.AwakenedAssault },
            {MagicType.EnhancedReflectDamage, MagicType.AwakenedReflectDamage },
            {MagicType.EnhancedSwiftBlade, MagicType.AwakenedSwiftBlade },
            {MagicType.EnhancedSeismicSlam, MagicType.AwakenedSeismicSlam },

            {MagicType.AdamantineFireBall, MagicType.MeteorShower},
            {MagicType.EnhancedScorchedEarth, MagicType.AwakenedScorchedEarth},
            {MagicType.EnhancedFireWall, MagicType.AwakenedFireWall},
            {MagicType.Asteroid, MagicType.AwakenedAsteroid},
            {MagicType.IceBlades, MagicType.AwakenedIceBlades},
            {MagicType.GreaterFrozenEarth, MagicType.AwakenedFrozenEarth},
            {MagicType.EnhancedIceStorm, MagicType.AwakenedIceStorm},
            {MagicType.ThunderBolt, MagicType.ThunderStrike},
            {MagicType.ChainLightning, MagicType.AwakenedChainLightning},
            {MagicType.EnhancedThunderBeam, MagicType.AwakenedThunderBeam},
            {MagicType.EnhancedElectricShock, MagicType.AwakenedElectricShock},
            {MagicType.Cyclone, MagicType.DragonTornado},
            {MagicType.EnhancedBlowEarth, MagicType.AwakenedBlowEarth},
            {MagicType.EnhancedTempest, MagicType.AwakenedTempest},
            {MagicType.EnhancedRenounce, MagicType.AdvancedRenounce},
            {MagicType.EnhancedTeleportation, MagicType.AwakenedTeleportation},
            {MagicType.EnhancedGeoManipulation, MagicType.AwakenedGeoManipulation},
            {MagicType.EnhancedExpelUndead, MagicType.AwakenedExpelUndead},
            {MagicType.EnhancedMagicShield, MagicType.AwakenedMagicShield},

            {MagicType.MassHeal, MagicType.AwakenedMassHeal},
            {MagicType.EnhancedPoisonDust, MagicType.AwakenedPoisonDust},
            {MagicType.GreaterEvilSlayer, MagicType.AwakenedEvilSlayer},
            {MagicType.ImprovedExplosiveTalisman, MagicType.AwakenedGreaterTaoExplosion},
            {MagicType.EnhancedResurrection, MagicType.AwakenedResurrection},
            {MagicType.SummonShinsu, MagicType.SummonDemonicCreature},
            {MagicType.EnhancedCelestialLight, MagicType.AwakenedCelestialLight},
            {MagicType.EnhancedBloodLust, MagicType.AwakenedBloodLust},
            {MagicType.EnhancedElementalSuperiority, MagicType.AwakenedElementalSuperiority},
            {MagicType.EnhancedResilience, MagicType.AwakenedResilience},
            {MagicType.EnhancedMagicResistance, MagicType.AwakenedMagicResistance},
            {MagicType.EnhancedTransparency, MagicType.AwakenedTransparency},
            {MagicType.MassInvisibility, MagicType.AwakenedMassInvisibility},
            {MagicType.EnhancedInfection, MagicType.AwakenedInfection},
            {MagicType.EnhancedTaoCombatKick, MagicType.AwakenedTaoCombatKick},
        };

        public const int InventorySize = 49,
                         EquipmentSize = 17,
                         CompanionInventorySize = 40,
                         CompanionEquipmentSize = 4,
                         PartsStorageOffset = 2000,
                         EquipmentOffSet = 1000,
                         StorageSize = 100;

        public const int AttackDelay = 1500,
                         ASpeedRate = 50,
                         ProjectileSpeed = 48;

        public static TimeSpan TurnTime = TimeSpan.FromMilliseconds(300),
                               HarvestTime = TimeSpan.FromMilliseconds(600),
                               MoveTime = TimeSpan.FromMilliseconds(600),
                               AttackTime = TimeSpan.FromMilliseconds(600),
                               CastTime = TimeSpan.FromMilliseconds(600),
                               MagicDelay = TimeSpan.FromMilliseconds(2000);


        public static bool RealNameRequired = false,
                           BirthDateRequired = false;
    }

    public sealed class SelectInfo
    {
        public int CharacterIndex { get; set; }
        public string CharacterName { get; set; }
        public int Level { get; set; }
        public MirGender Gender { get; set; }
        public MirClass Class { get; set; }
        public int Location { get; set; }
        public DateTime LastLogin { get; set; }
    }

    public sealed class StartInformation
    {
        public int Index { get; set; }
        public uint ObjectID { get; set; }
        public string Name { get; set; }
        public Color NameColour { get; set; }
        public string GuildName { get; set; }
        public string GuildRank { get; set; }

        public MirClass Class { get; set; }
        public MirGender Gender { get; set; }
        public Point Location { get; set; }
        public MirDirection Direction { get; set; }

        public int MapIndex { get; set; }
        public int InstanceIndex { get; set; }

        public long Gold { get; set; }
        public int GameGold { get; set; }

        public int Level { get; set; }
        public int HairType { get; set; }
        public Color HairColour { get; set; }
        public int Weapon { get; set; }
        public int Armour { get; set; }
        public int Shield { get; set; }
        public Color ArmourColour { get; set; }
        public int ArmourImage { get; set; }

        public int EmblemShape { get; set; }
        public int WingsShape { get; set; }
        
        public decimal Experience { get; set; }

        public int CurrentHP { get; set; }
        public int CurrentMP { get; set; }

        public AttackMode AttackMode { get; set; }
        public PetMode PetMode { get; set; }

        public int HermitPoints { get; set; }

        public float DayTime { get; set; }
        public bool AllowGroup { get; set; }

        public List<ClientUserItem> Items { get; set; }
        public List<ClientBeltLink> BeltLinks { get; set; }
        public List<ClientAutoPotionLink> AutoPotionLinks { get; set; }

        public List<ClientUserMagic> Magics { get; set; }
        public List<ClientBuffInfo> Buffs { get; set; }

        public PoisonType Poison { get; set; }

        public bool InSafeZone { get; set; }
        public bool Observable { get; set; }

        public bool Dead { get; set; }

        public HorseType Horse { get; set; } //Horse Armour too

        public int HelmetShape { get; set; }
        public int HorseShape { get; set; }

        public List<ClientUserQuest> Quests { get; set; }

        public List<int> CompanionUnlocks { get; set; }
        public List<CompanionInfo> AvailableCompanions = new List<CompanionInfo>();

        public List<ClientUserCompanion> Companions { get; set; }

        public int Companion { get; set; }

        public int StorageSize { get; set; }

        public string FiltersClass { get; set; }
        public string FiltersRarity { get; set; }
        public string FiltersItemType { get; set; }

        [CompleteObject]
        public void OnComplete()
        {
            foreach (int index in CompanionUnlocks)
                AvailableCompanions.Add(Globals.CompanionInfoList.Binding.First(x => x.Index == index));
        }
    }

    public sealed class ClientUserItem
    {
        public ItemInfo Info;

        public int Index { get; set; } //ItemID
        public int InfoIndex { get; set; }

        public int SetValue { get; set; }

        public long Count { get; set; }

        public int Slot { get; set; }

        public int Level { get; set; }
        public int GemCount { get; set; }

        public Color Colour { get; set; }

        public TimeSpan ResetCoolDown { get; set; }

        public bool New;
        public DateTime NextReset;

        public Stats AddedStats { get; set; }

        public UserItemFlags Flags { get; set; }
        public TimeSpan ExpireTime { get; set; }


        [IgnorePropertyPacket]
        public int Weight
        {
            get
            {
                switch (Info.ItemType)
                {
                    case ItemType.Poison:
                    case ItemType.Amulet:
                        return Info.Weight;
                    default:
                        return (int)Math.Min(int.MaxValue, Info.Weight * Count);
                }
            }
        }

        [CompleteObject]
        public void Complete()
        {
            Info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);

            NextReset = Time.Now + ResetCoolDown;
        }

        public ClientUserItem()
        { }
        public ClientUserItem(ItemInfo info, long count)
        {
            Info = info;
            Count = count;
            SetValue = info.SetValue;
            Level = 1;
            AddedStats = new Stats();
        }
        public ClientUserItem(ClientUserItem item, long count)
        {
            Info = item.Info;

            Index = item.Index;
            InfoIndex = item.InfoIndex;

            SetValue = item.SetValue;

            Count = count;

            Slot = item.Slot;

            Level = item.Level;
            GemCount = item.GemCount;

            Colour = item.Colour;

            Flags = item.Flags;
            ExpireTime = item.ExpireTime;

            New = item.New;

            AddedStats = new Stats(item.AddedStats);
        }

        public long Price(long count)
        {
            if ((Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return 0;

            decimal p = Info.Price;

            p = p * (AddedStats.Count * 0.1M + 1M);

            if (Info.Stats[Stat.SaleBonus20] > 0 && Info.Stats[Stat.SaleBonus20] <= count)
                p *= 1.2M;
            else if (Info.Stats[Stat.SaleBonus15] > 0 && Info.Stats[Stat.SaleBonus15] <= count)
                p *= 1.15M;
            else if (Info.Stats[Stat.SaleBonus10] > 0 && Info.Stats[Stat.SaleBonus10] <= count)
                p *= 1.1M;
            else if (Info.Stats[Stat.SaleBonus5] > 0 && Info.Stats[Stat.SaleBonus5] <= count)
                p *= 1.05M;

            return (long)(p * count * Info.SellRate);
        }

        public bool CanAccessoryUpgrade()
        {
            switch (Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return false;

            }

            return (Flags & UserItemFlags.NonRefinable) != UserItemFlags.NonRefinable && (Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable;
        }
        public bool CanFragment()
        {
            if ((Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable || (Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return false;

            switch (Info.Rarity)
            {
                case Rarity.Common:
                    if (Info.RequiredAmount <= 15) return false;
                    break;
                case Rarity.Superior:
                    break;
                case Rarity.Elite:
                    break;
            }

            switch (Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                    break;
                default:
                    return false;
            }

            return true;
        }
        public int FragmentCost()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 9;
                        /* case ItemType.Helmet:
                         case ItemType.Necklace:
                         case ItemType.Bracelet:
                         case ItemType.Ring:
                         case ItemType.Shoes:
                             return Info.RequiredAmount * 7000 / 9;*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armour:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 2;
                        /*  case ItemType.Helmet:
                          case ItemType.Necklace:
                          case ItemType.Bracelet:
                          case ItemType.Ring:
                          case ItemType.Shoes:
                              return Info.RequiredAmount * 10000 / 10;*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armour:
                            return 250000;
                        case ItemType.Helmet:
                            return 50000;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 150000;
                        case ItemType.Shoes:
                            return 30000;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
        public int FragmentCount()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                        /*  case ItemType.Helmet:
                              return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                          case ItemType.Necklace:
                              return Math.Max(1, Info.RequiredAmount / 8);
                          case ItemType.Bracelet:
                              return Math.Max(1, Info.RequiredAmount / 15);
                          case ItemType.Ring:
                              return Math.Max(1, Info.RequiredAmount / 9);
                          case ItemType.Shoes:
                              return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                        /*  case ItemType.Helmet:
                              return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                          case ItemType.Necklace:
                              return Math.Max(1, Info.RequiredAmount / 10);
                          case ItemType.Bracelet:
                              return Math.Max(1, Info.RequiredAmount / 15);
                          case ItemType.Ring:
                              return Math.Max(1, Info.RequiredAmount / 10);
                          case ItemType.Shoes:
                              return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                            return 50;
                        case ItemType.Helmet:
                            return 5;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 10;
                        case ItemType.Shoes:
                            return 3;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
    }

    public sealed class ClientBeltLink
    {
        public int Slot { get; set; }
        public int LinkInfoIndex { get; set; }
        public int LinkItemIndex { get; set; }
    }

    public sealed class ClientAutoPotionLink
    {
        public int Slot { get; set; }
        public int LinkInfoIndex { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Enabled { get; set; }
    }

    public class ClientUserMagic
    {
        public int Index { get; set; }
        public int InfoIndex { get; set; }
        public MagicInfo Info;

        public SpellKey Set1Key { get; set; }
        public SpellKey Set2Key { get; set; }
        public SpellKey Set3Key { get; set; }
        public SpellKey Set4Key { get; set; }

        public int Level { get; set; }
        public long Experience { get; set; }

        public TimeSpan Cooldown { get; set; }

        public DateTime NextCast;


        [IgnorePropertyPacket]
        public int Cost => Info.BaseCost + Level * Info.LevelCost / 3;

        [CompleteObject]
        public void Complete()
        {
            NextCast = Time.Now + Cooldown;
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }

    public class CellLinkInfo
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public long Count { get; set; }
    }

    public class ClientBuffInfo
    {
        public int Index { get; set; }
        public BuffType Type { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public TimeSpan TickFrequency { get; set; }
        public Stats Stats { get; set; }
        public bool Pause { get; set; }
        public int ItemIndex { get; set; }
    }

    public class ClientRefineInfo
    {
        public int Index { get; set; }
        public ClientUserItem Weapon { get; set; }
        public RefineType Type { get; set; }
        public RefineQuality Quality { get; set; }
        public int Chance { get; set; }
        public int MaxChance { get; set; }
        public TimeSpan ReadyDuration { get; set; }

        public DateTime RetrieveTime;

        [CompleteObject]
        public void Complete()
        {
            RetrieveTime = Time.Now + ReadyDuration;
        }
    }

    public sealed class RankInfo
    {
        public int Rank { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public MirClass Class { get; set; }
        public int Level { get; set; }
        public bool Online { get; set; }
        public bool Observable { get; set; }
        public int Rebirth { get; set; }
    }

    public class ClientMarketPlaceInfo
    {
        public int Index { get; set; }
        public ClientUserItem Item { get; set; }
        public int Price { get; set; }
        public string Seller { get; set; }
        public string Message { get; set; }
        public bool IsOwner { get; set; }

        public bool Loading;
    }

    public class ClientMailInfo
    {
        public int Index { get; set; }
        public bool Opened { get; set; }
        public bool HasItem { get; set; }
        public DateTime Date { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int Gold { get; set; }
        public List<ClientUserItem> Items { get; set; }
    }

    public class ClientGuildInfo
    {
        public string GuildName { get; set; }

        public string Notice { get; set; }

        public int MemberLimit { get; set; }

        public long GuildFunds { get; set; }
        public long DailyGrowth { get; set; }

        public long TotalContribution { get; set; }
        public long DailyContribution { get; set; }

        public int UserIndex { get; set; }

        public int StorageLimit { get; set; }
        public int Tax { get; set; }

        public string DefaultRank { get; set; }
        public GuildPermission DefaultPermission { get; set; }

        public List<ClientGuildMemberInfo> Members { get; set; }

        public List<ClientUserItem> Storage { get; set; }

        [IgnorePropertyPacket]
        public GuildPermission Permission => Members.FirstOrDefault(x => x.Index == UserIndex)?.Permission ?? GuildPermission.None;
    }

    public class ClientGuildMemberInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }
        public long TotalContribution { get; set; }
        public long DailyContribution { get; set; }
        public TimeSpan Online { get; set; }

        public GuildPermission Permission { get; set; }

        public DateTime LastOnline;
        public uint ObjectID { get; set; }

        [CompleteObject]
        public void Complete()
        {
            if (Online == TimeSpan.MinValue)
                LastOnline = DateTime.MaxValue;
            else
                LastOnline = Time.Now - Online;
        }
    }

    public class ClientUserQuest
    {
        public int Index { get; set; }

        [IgnorePropertyPacket]
        public QuestInfo Quest { get; set; }

        public int QuestIndex { get; set; }

        public bool Track { get; set; }

        public bool Completed { get; set; }

        public int SelectedReward { get; set; }

        [IgnorePropertyPacket]
        public bool IsComplete => Tasks.Count == Quest.Tasks.Count && Tasks.All(x => x.Completed);

        public List<ClientUserQuestTask> Tasks { get; set; }

        [CompleteObject]
        public void Complete()
        {
            Quest = Globals.QuestInfoList.Binding.First(x => x.Index == QuestIndex);
        }
    }

    public class ClientUserQuestTask
    {
        public int Index { get; set; }

        [IgnorePropertyPacket]
        public QuestTask Task { get; set; }

        public int TaskIndex { get; set; }

        public long Amount { get; set; }

        [IgnorePropertyPacket]
        public bool Completed => Amount >= Task.Amount;

        [CompleteObject]
        public void Complete()
        {
            Task = Globals.QuestTaskList.Binding.First(x => x.Index == TaskIndex);
        }
    }

    public class ClientCompanionObject
    {
        public string Name { get; set; }

        public int HeadShape { get; set; }
        public int BackShape { get; set; }
    }

    public class ClientUserCompanion
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public int CompanionIndex { get; set; }
        public CompanionInfo CompanionInfo;

        public int Level { get; set; }
        public int Hunger { get; set; }
        public int Experience { get; set; }

        public Stats Level3 { get; set; }
        public Stats Level5 { get; set; }
        public Stats Level7 { get; set; }
        public Stats Level10 { get; set; }
        public Stats Level11 { get; set; }
        public Stats Level13 { get; set; }
        public Stats Level15 { get; set; }

        public string CharacterName { get; set; }

        public List<ClientUserItem> Items { get; set; }

        public ClientUserItem[] EquipmentArray = new ClientUserItem[Globals.CompanionEquipmentSize], InventoryArray = new ClientUserItem[Globals.CompanionInventorySize];


        [CompleteObject]
        public void OnComplete()
        {
            CompanionInfo = Globals.CompanionInfoList.Binding.First(x => x.Index == CompanionIndex);

            foreach (ClientUserItem item in Items)
            {
                if (item.Slot < Globals.EquipmentOffSet)
                    InventoryArray[item.Slot] = item;
                else
                    EquipmentArray[item.Slot - Globals.EquipmentOffSet] = item;
            }
        }
    }

    public class ClientPlayerInfo
    {
        public uint ObjectID { get; set; }

        public string Name { get; set; }
    }
    public class ClientObjectData
    {
        public uint ObjectID;

        public int MapIndex;
        public Point Location;

        public string Name;

        //Guild/Grorup
        public MonsterInfo MonsterInfo;
        public ItemInfo ItemInfo;

        public string PetOwner;

        public int Health;
        public int MaxHealth;

        public int Mana;
        public int MaxMana;
        public Stats Stats { get; set; }

        public bool Dead;
    }

    public class ClientBlockInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }

    public class ClientFortuneInfo
    {
        public int ItemIndex { get; set; }
        public ItemInfo ItemInfo;

        public TimeSpan CheckTime { get; set; }
        public long DropCount { get; set; }
        public decimal Progress { get; set; }

        public DateTime CheckDate;

        [CompleteObject]
        public void OnComplete()
        {
            ItemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == ItemIndex);

            CheckDate = Time.Now - CheckTime;
        }
    }
    public class CompanionFiltersInfo
    {
        public string FilterClass { get; set; }
        public string FilterRarity { get; set; }
        public string FilterItemType { get; set; }
    }
}