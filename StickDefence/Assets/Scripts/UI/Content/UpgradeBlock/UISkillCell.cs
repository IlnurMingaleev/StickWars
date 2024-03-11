using System;
using Enums;
using TonkoGames.Controllers.Core;
using Models.Controllers;
using Models.DataModels;
using Models.DataModels.Data;
using Models.Player;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Content.UpgradeBlock
{
    public class UISkillCell : UIBehaviour
    {
        [SerializeField] private GameObject _openBlock;
        [SerializeField] private GameObject _lockBlock;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _addPlusBlock;
        [SerializeField] private CurrencyIconBlock _currencyIconBlock;
        [SerializeField] private TMP_Text _costLabel;
        [SerializeField] private UIButton _pickButton;

        public event Action<int> CellPicked;

        private int _index = 0;

        public int Index => _index;

        private IReadOnlyReactiveProperty<int> _currencyCount;

        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly IPlayer _player;

        private SkillCellData _skillCellData;
        
        public void Init(int index)
        {
            _index = index;
            _skillCellData = _dataCentralService.PumpingDataModel.GetSkillCellData(_index);
            _currencyCount =
                _player.SubscribeToCurrencyBuyType(_configManager.PumpingConfigSo.SkillCells[_index].CurrencyType);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _pickButton.OnClickAsObservable.TakeUntilDisable(this).Subscribe(_ => ButtonClick());
        }

        public void SetupCell(SkillCellData skillCellData)
        {
            _skillCellData = skillCellData;
            SetupCell();
        }
        
        private void SetupCell()
        {
            if (_skillCellData.IsOpen)
            {
                _lockBlock.SetActive(false);
                _openBlock.SetActive(true);
                if (_skillCellData.SkillType != SkillTypesEnum.None)
                {
                    _icon.enabled = true;
                    _icon.sprite = _configManager.SpritesSo.UISkillsIcons[_skillCellData.SkillType];
                    _addPlusBlock.SetActive(false);
                }
                else
                {
                    _icon.enabled = false;
                    _addPlusBlock.SetActive(true);
                }
            }
            else
            {
                _lockBlock.SetActive(true);
                _openBlock.SetActive(false);

                var configCell = _configManager.PumpingConfigSo.SkillCells[_index];
                _costLabel.text = SetScoreExt.ConvertIntToStringValue(configCell.Cost, 1);
                _currencyIconBlock.SetPerType(configCell.CurrencyType);
            }
        }


        private void ButtonClick()
        {
            if (_skillCellData.IsOpen)
            {
                _skillCellData.SkillType = SkillTypesEnum.None;
                _dataCentralService.PumpingDataModel.UpdateSkillCellData(_index, _skillCellData);
                CellPicked?.Invoke(_index);
            }
            else if(_currencyCount.Value >= _configManager.PumpingConfigSo.SkillCells[_index].Cost)
            {
                _skillCellData.IsOpen = true;
                _dataCentralService.PumpingDataModel.UpdateSkillCellData(_index, _skillCellData);
            }
            SetupCell();
            _dataCentralService.SaveFull();
        }
    }
}