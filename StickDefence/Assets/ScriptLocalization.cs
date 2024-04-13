using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static class Inaps
		{
			public static string VOTE
			{
				get { return LocalizationManager.GetTranslation("Inaps/VOTE"); }
			}

			public static string RUB
			{
				get { return LocalizationManager.GetTranslation("Inaps/RUB"); }
			}

			public static string OK
			{
				get { return LocalizationManager.GetTranslation("Inaps/OK"); }
			}

			public static string GP
			{
				get { return LocalizationManager.GetTranslation("Inaps/GP"); }
			}

			public static string DOLLAR
			{
				get { return LocalizationManager.GetTranslation("Inaps/DOLLAR"); }
			}

			public static string YAN
			{
				get { return LocalizationManager.GetTranslation("Inaps/YAN"); }
			}
		}

		public static class Messages
		{
			public static string Defeat
			{
				get { return LocalizationManager.GetTranslation("Messages/Defeat"); }
			}

			public static string NeedLogin
			{
				get { return LocalizationManager.GetTranslation("Messages/NeedLogin"); }
			}

			public static string SlotsFull
			{
				get { return LocalizationManager.GetTranslation("Messages/SlotsFull"); }
			}

			public static string WarningTitle
			{
				get { return LocalizationManager.GetTranslation("Messages/WarningTitle"); }
			}

			public static string NotEnoughFunds
			{
				get { return LocalizationManager.GetTranslation("Messages/NotEnoughFunds"); }
			}

			public static string MaxLevel
			{
				get { return LocalizationManager.GetTranslation("Messages/MaxLevel"); }
			}

			public static string AttackSpeed
			{
				get { return LocalizationManager.GetTranslation("Messages/AttackSpeedMessage"); }
			}

			public static string GainCoins
			{
				get { return LocalizationManager.GetTranslation("Messages/GainCoinsMessage"); }
			}

			public static string AutoMerge
			{
				get { return LocalizationManager.GetTranslation("Messages/AutoMergeMessage"); }
			}
		}

		public static class Names_Perks
		{
			public static string RegenHealth
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/RegenHealth"); }
			}

			public static string KillSilverBonus
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/KillSilverBonus"); }
			}

			public static string Health
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/Health"); }
			}

			public static string Damage
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/Damage"); }
			}

			public static string CriticalMultiplier
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/CriticalMultiplier"); }
			}

			public static string CriticalChance
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/CriticalChance"); }
			}

			public static string AttackSpeed
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/AttackSpeed"); }
			}

			public static string AttackRange
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/AttackRange"); }
			}

			public static string Defense
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/Defense"); }
			}

			public static string DailyGoldBonus
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/DailyGoldBonus"); }
			}
			public static string DecreasePrice
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/DecreasePrice"); }
			}

			public static string IncreaseProfit
			{
				get { return LocalizationManager.GetTranslation("Names/Perks/IncreaseProfit"); }
			}
			
			public static string RecruitsDamage
			{
				get { return LocalizationManager.GetTranslation( "Names/Perks/RecruitsDamage"); }
			}
		}

		public static class Names_Skills
		{
			public static string Lightning
			{
				get { return LocalizationManager.GetTranslation("Names/Skills/Lightning"); }
			}

			public static string ShockWave
			{
				get { return LocalizationManager.GetTranslation("Names/Skills/ShockWave"); }
			}

			public static string Spikes
			{
				get { return LocalizationManager.GetTranslation("Names/Skills/Spikes"); }
			}
		}

		public static class Windows_PlayResult
		{
			public static string StageClear
			{
				get { return LocalizationManager.GetTranslation("Windows/PlayResult/StageClear"); }
			}
		}

		public static class Windows_Shop
		{
			public static string RemoveADS
			{
				get { return LocalizationManager.GetTranslation("Windows/Shop/RemoveADS"); }
			}
		}

		public static class Windows_UpgradeBaseWindow
		{
			public static string SpikesDescription
			{
				get { return LocalizationManager.GetTranslation("Windows/UpgradeBaseWindow/SpikesDescription"); }
			}

			public static string ShockWaveDescription
			{
				get { return LocalizationManager.GetTranslation("Windows/UpgradeBaseWindow/ShockWaveDescription"); }
			}

			public static string LightningDescription
			{
				get { return LocalizationManager.GetTranslation("Windows/UpgradeBaseWindow/LightningDescription"); }
			}
		}

		public static class Windows_WeeklyDailyBonus
		{
			public static string DefaultChest
			{
				get { return LocalizationManager.GetTranslation("Windows/WeeklyDailyBonus/DefaultChest"); }
			}

			public static string VipChest
			{
				get { return LocalizationManager.GetTranslation("Windows/WeeklyDailyBonus/VipChest"); }
			}
		}

		public static class Buttons_Shop
		{
			public static string UnlockCannon
			{
				get { return LocalizationManager.GetTranslation("Buttons/CannonWarning"); }
			}
		}
	}


	public static class ScriptTerms
	{

		public static class Inaps
		{
			public const string VOTE = "Inaps/VOTE";
			public const string RUB = "Inaps/RUB";
			public const string OK = "Inaps/OK";
			public const string GP = "Inaps/GP";
			public const string DOLLAR = "Inaps/DOLLAR";
			public const string YAN = "Inaps/YAN";
		}

		public static class Messages
		{
			public const string Defeat = "Messages/Defeat";
			public const string NeedLogin = "Messages/NeedLogin";
			public const string SlotsFull = "Messages/SlotsFull";
			public const string WarningTitle = "Messages/WarningTitle";
			public const string NotEnoughFunds = "Messages/NotEnoughFunds";
			public const string MaxLevel = "Messages/MaxLevel";
			public const string AttackSpeed = "Messages/AttackSpeedMessage";
			public const string GainCoins = "Messages/GainCoinsMessage";
			public const string AutoMerge = "Messages/AutoMergeMessage";
		}

		public static class Names_Perks
		{
			public const string RegenHealth = "Names/Perks/RegenHealth";
			public const string KillSilverBonus = "Names/Perks/KillSilverBonus";
			public const string Health = "Names/Perks/Health";
			public const string Damage = "Names/Perks/Damage";
			public const string CriticalMultiplier = "Names/Perks/CriticalMultiplier";
			public const string CriticalChance = "Names/Perks/CriticalChance";
			public const string AttackSpeed = "Names/Perks/AttackSpeed";
			public const string AttackRange = "Names/Perks/AttackRange";
			public const string Defense = "Names/Perks/Defense";
			public const string DailyGoldBonus = "Names/Perks/DailyGoldBonus";
			public const string DecreasePrice = "Names/Perks/DecreasePrice";
			public const string IncreaseProfit = "Names/Perks/IncreaseProfit";
			public const string RecruitsDamage = "Names/Perks/RecruitsDamage";
		}

		public static class Names_Skills
		{
			public const string Lightning = "Names/Skills/Lightning";
			public const string ShockWave = "Names/Skills/ShockWave";
			public const string Spikes = "Names/Skills/Spikes";
		}

		public static class Windows_PlayResult
		{
			public const string StageClear = "Windows/PlayResult/StageClear";
		}

		public static class Windows_Shop
		{
			public const string RemoveADS = "Windows/Shop/RemoveADS";
		}

		public static class Windows_UpgradeBaseWindow
		{
			public const string SpikesDescription = "Windows/UpgradeBaseWindow/SpikesDescription";
			public const string ShockWaveDescription = "Windows/UpgradeBaseWindow/ShockWaveDescription";
			public const string LightningDescription = "Windows/UpgradeBaseWindow/LightningDescription";
		}

		public static class Windows_WeeklyDailyBonus
		{
			public const string DefaultChest = "Windows/WeeklyDailyBonus/DefaultChest";
			public const string VipChest = "Windows/WeeklyDailyBonus/VipChest";
		}
	}
}
	