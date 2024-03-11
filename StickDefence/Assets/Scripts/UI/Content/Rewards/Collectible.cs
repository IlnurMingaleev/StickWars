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
}