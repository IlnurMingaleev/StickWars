using System.Collections.Generic;
using Models.Player;
using UI.Common;
using UI.UIManager;
using UnityEngine;
using VContainer;

namespace UI.Content.Battle
{
    public class BattlePerksPanel : UIBehaviour
    {
        [SerializeField] private List<UIBattleUpgradePerk> _perkUpgrades;
        [SerializeField] private UIToggle _baseToggle;

        [Inject] private readonly IPlayer _player;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            foreach (var perkUpgrade in _perkUpgrades)
            {
                SetPerkUpgrade(perkUpgrade);
                perkUpgrade.PurchasedLevelUp += PerkPurchased;
            }
            
            _baseToggle.isOn = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var perkUpgrade in _perkUpgrades)
            {
                perkUpgrade.PurchasedLevelUp -= PerkPurchased;
            }
        }

        private void PerkPurchased(UIBattleUpgradePerk perkUpgrade)
        {
            _player.Pumping.UpgradeGamePerk(perkUpgrade.PerkType);
            SetPerkUpgrade(perkUpgrade);
        }
 
        private void SetPerkUpgrade(UIBattleUpgradePerk perkUpgrade)
        {
            perkUpgrade.Init(_player.Pumping.GamePerks[perkUpgrade.PerkType]);
        }
    }
}