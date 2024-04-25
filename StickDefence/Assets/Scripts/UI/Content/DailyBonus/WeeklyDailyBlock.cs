using Enums;
using TonkoGames.Controllers.Core;
using I2.Loc;
using Models.SO.Iaps;
using TMPro;
using Tools.GameTools;
using UI.Common;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.DailyBonus
{
    public class WeeklyDailyBlock : MonoBehaviour
    {
        [SerializeField] private LocalizationParamsManager _dayLabelParam;
        [SerializeField] private UIButton _selectButton;
        [SerializeField] private GameObject _selectableGo;
        [SerializeField] private GameObject _glow;
        [SerializeField] private GameObject _collectedGo;
        [SerializeField] private TMP_Text _countLabel;

        private WeeklyDailyBonusWindow _weeklyDailyBonusWindow;

        private CompositeDisposable _disposable = new CompositeDisposable();

        private int _dayIndex = 0;
        private RewardConfig _rewardConfig;

        [Inject] private readonly ConfigManager _configManager;

        public void Init(int dayIndex, WeeklyDailyBonusWindow weeklyDailyBonusWindow, RewardConfig rewardConfig)
        {
            _weeklyDailyBonusWindow = weeklyDailyBonusWindow;
            _dayIndex = dayIndex;
            _rewardConfig = rewardConfig;
        }

        public void InitDay(bool selectable, bool collected)
        {
            _dayLabelParam.SetParameterValue(
                StringsHelper.ClassLocalizationParams.Value,
                (_dayIndex + 1).ToString()
            );

            _selectableGo.SetActive(selectable);
            if (selectable)
            {
                _selectButton.OnClickAsObservable.Subscribe(_ => OnSelect()).AddTo(_disposable);
            }

            _collectedGo.SetActive(collected);
            _glow.SetActive(!collected);
            _countLabel.gameObject.SetActive(!collected);
            SetCountLabel();
        }

        private void OnDisable()
        {
            _disposable.Clear();
        }


        private void OnSelect()
        {
            _disposable.Clear();
            _weeklyDailyBonusWindow.OnCollect(_rewardConfig);
        }

        private void SetCountLabel()
        {
            switch (_rewardConfig.RewardIconType)
            {
                case RewardIconTypeEnum.Gold:
                    _countLabel.text = _rewardConfig.CoinReward.ToString();
                    break;
                case RewardIconTypeEnum.Gem:
                    _countLabel.text = _rewardConfig.GemReward.ToString();
                    break;
               
            }
        }
    }
}