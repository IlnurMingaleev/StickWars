using System.Linq;
using TonkoGames.Controllers.Core;
using Enums;
using Models.DataModels;
using Models.IAP;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.Shop
{
    public class CoinBuyShop : UIBehaviour
    {
        [SerializeField] private IapTypeEnum _iapType;
        [SerializeField] private UIButton _buyIAP;
        [SerializeField] private TMP_Text _labelCount;
        [SerializeField] private TMP_Text _costCountLabel;
        [SerializeField] private GameObject _glow;
        [SerializeField] private ParticleSystem _chestParticle;

        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ConfigManager _configManager;

        private int _rewardCount = 0;
        private int _costCount = 0;
        
        protected override void Awake()
        {
            base.Awake();
            var config =
                _configManager.IapSO.IapRewardData.FirstOrDefault(model => model.IapType == _iapType);
            _rewardCount = config.Value;
            _costCount = config.Cost;
            
            _labelCount.text = SetScoreExt.ConvertIntToStringValue(_rewardCount, 1);
            _costCountLabel.text = SetScoreExt.ConvertIntToStringValue(_costCount, 1);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _dataCentralService.StatsDataModel.GemsCount.TakeUntilDisable(this).Subscribe(CheckCanBuy);
            
            _buyIAP.OnClick.AsObservable().TakeUntilDisable(this).Subscribe(_ =>
            {
                _dataCentralService.StatsDataModel.AddCoinsCount(_rewardCount);
                _dataCentralService.StatsDataModel.MinusGemsCount(_costCount);
                _dataCentralService.SaveFull();
            });
        }

        private void CheckCanBuy(int gems)
        {
            if (gems > _costCount)
            {
                if (_chestParticle != null)
                    _chestParticle.Play();
                
                if (_glow != null)
                    _glow.SetActive(true);
                
                _buyIAP.IsInteractable = true;
            }
            else
            {
                if (_chestParticle != null)
                    _chestParticle.Stop();
                if (_glow != null)
                    _glow.SetActive(false);
                _buyIAP.IsInteractable = false;
            }
        }
    }
}