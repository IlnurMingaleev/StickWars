using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using TonkoGames.Controllers.Core;
using Models.DataModels;
using Models.Player;
using UI.Common;
using UI.Content.UpgradeBlock;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.Lobby
{
    public class UpgradeBasePanel : UIBehaviour
    {
        [SerializeField] private List<UIPerkUpgrade> _perkUpgrades;
        [SerializeField] private List<UISkillUpgrade> _skillUpgrades;
        [SerializeField] private List<UISkillCell> _uiSkillCells;
        [SerializeField] private UIToggle _permanentToggle;
        [SerializeField] private UIToggle _skillToggle;
        [SerializeField] private UIButton _closeButton;

        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly IPlayer _player;
        
        private int _lastCellPickUpIndex = -1;
        private bool _isSelectableSkills = false;
        private CompositeDisposable _disposable = new();
        
        protected override void Awake()
        {
            for (int i = 0; i < _uiSkillCells.Count; i++)
            {
                _uiSkillCells[i].Init(i);
            }
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetPerksUpgrade();
            SetSkillsUpgrade();
            SetupSkillCells();
            _permanentToggle.isOn = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var perkUpgrade in _perkUpgrades)
            {
                perkUpgrade.PurchasedLevelUp -= PerkPurchased;
            }
            
            foreach (var perkUpgrade in _skillUpgrades)
            {
                perkUpgrade.PurchasedLevelUp -= SkillPurchased;
            }

            foreach (var cell in _uiSkillCells)
            {
                cell.CellPicked -= CellPickUp;
            }
            
            SetSkillsNormal();
            gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _disposable.Clear();
        }
        
        public void Open(Action closeAction)
        {
            gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
            _closeButton.OnClickAsObservable.Subscribe(_ =>
            {
                gameObject.SetActive(false);
                closeAction?.Invoke();
            }).AddTo(_disposable);
        }
        
        #region Perks
        private void SetPerksUpgrade()
        {
            foreach (var perkUpgrade in _perkUpgrades)
            {
                SetPerkUpgrade(perkUpgrade);
                perkUpgrade.PurchasedLevelUp += PerkPurchased;
            }
        }

        private void PerkPurchased(UIPerkUpgrade perkUpgrade)
        {
            _player.Pumping.UpgradeBasePerk(perkUpgrade.PerkType);
            SetPerkUpgrade(perkUpgrade);
        }
 
        private void SetPerkUpgrade(UIPerkUpgrade perkUpgrade)
        {
            perkUpgrade.Init(_player.Pumping.BasePerks[perkUpgrade.PerkType]);
        }

        #endregion
        
        #region Skills
        private void SetSkillsUpgrade()
        {
            foreach (var skillUpgrade in _skillUpgrades)
            {
                SetSkillUpgrade(skillUpgrade);
                skillUpgrade.SetNormalState();
                skillUpgrade.PurchasedLevelUp += SkillPurchased;
            }
        }

        private void SkillPurchased(UISkillUpgrade perkUpgrade)
        {
            _player.Pumping.UpgradeSkill(perkUpgrade.SkillType);
            SetSkillUpgrade(perkUpgrade);
        }
 
        private void SetSkillUpgrade(UISkillUpgrade perkUpgrade)
        {
            perkUpgrade.Init(_player.Pumping.Skills[perkUpgrade.SkillType]);
        }

        #endregion

        #region Cells

        private void SetupSkillCells()
        {
            foreach (var cell in _uiSkillCells)
            {
                cell.SetupCell(_dataCentralService.PumpingDataModel.GetSkillCellData(cell.Index));
                cell.CellPicked += CellPickUp;
            }
        }
        private void CellPickUp(int index)
        {
            if (!_skillToggle.isOn)
                _skillToggle.isOn = true;

            
            
            if (_lastCellPickUpIndex == index)
            {
                SetSkillsNormal();
            }
            else
            {
                if (!_isSelectableSkills)
                {
                    _isSelectableSkills = true;
                    foreach (var skill in _skillUpgrades)
                    {
                        skill.SetSelected(_dataCentralService.PumpingDataModel.SkillCellsReactive.Any(cell => cell.SkillType == skill.SkillType));
                        skill.SkillSelected += SkillSelect;
                    }
                }     
                _lastCellPickUpIndex = index;
            }

        }

        private void SkillSelect(SkillTypesEnum skillTypesEnum)
        {
            var skillCellData = _dataCentralService.PumpingDataModel.GetSkillCellData(_lastCellPickUpIndex);
            skillCellData.SkillType = skillTypesEnum;
            _dataCentralService.PumpingDataModel.UpdateSkillCellData(_lastCellPickUpIndex, skillCellData);
            _uiSkillCells[_lastCellPickUpIndex].SetupCell(skillCellData);
            _dataCentralService.SaveFull();
            SetSkillsNormal();
        }

        private void SetSkillsNormal()
        {
            foreach (var skill in _skillUpgrades)
            {
                skill.SetNormalState();
                skill.SkillSelected -= SkillSelect;
            }
            _lastCellPickUpIndex = -1;
            _isSelectableSkills = false;
        }

        #endregion
    }
}