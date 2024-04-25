using Enums;

namespace UI.Content.Rewards
{
    public abstract class Collectible
    {
        public string Name { get; protected set; } = string.Empty;
        public int Amount { get; protected set; } = 0;
        public RewardType Type { get; protected set; } = RewardType.None;
    }
    
    public class CollectibleCoin : Collectible
    {
        public CollectibleCoin(int coin)
        {
            Amount = coin;
            Type = RewardType.Coin;
        }
    }
    
    public class CollectibleGem : Collectible
    {
        public CollectibleGem(int gem)
        {
            Amount = gem;
            Type = RewardType.Gem;
        }
    }

    public class CollectibleAttackSpeed : Collectible
    {
        public CollectibleAttackSpeed(int attackSpeedTime)
        {
            Amount = attackSpeedTime;
            Type = RewardType.AttackSpeed;
        }
    }
    public class CollectibleGainCoins : Collectible
    {
        public CollectibleGainCoins(int gainCoinsTime)
        {
            Amount = gainCoinsTime;
            Type = RewardType.GainCoins;
        }
    }
    public class CollectibleAutoMerge : Collectible
    {
        public CollectibleAutoMerge(int automergeTime)
        {
            Amount = automergeTime;
            Type = RewardType.AutoMerge;
        }
    }
}