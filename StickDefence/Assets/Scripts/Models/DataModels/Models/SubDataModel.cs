using System;
using Enums;
using TonkoGames.StateMachine.Enums;
using Models.DataModels.Data;
using UniRx;

namespace Models.DataModels.Models
{
    public interface ISubDataModel
    {
        #region Fields
        IReadOnlyReactiveProperty<TutorialStepsEnum> LuckySpinTutorialStep { get; }

        IReadOnlyReactiveProperty<bool> IsADSRemove { get; }
        
        IReadOnlyReactiveProperty<float> SoundVolume { get; }
        IReadOnlyReactiveProperty<float> MusicVolume { get; }
        #endregion
        
        #region Seters

        void SetLuckySpinTutorialStep(TutorialStepsEnum value);

        void SetSoundVolume(float value);
        void SetMusicVolume(float value);
        void SetADSRemove(bool value);

        #endregion
    }

    public class SubDataModel : ISubDataModel
    {
        #region Fields

        private ReactiveProperty<TutorialStepsEnum> _luckySpinTutorialStep = new ReactiveProperty<TutorialStepsEnum>();

        private ReactiveProperty<bool> _isADSRemove = new ReactiveProperty<bool>();

        private ReactiveProperty<float> _soundVolume = new ReactiveProperty<float>();
        private ReactiveProperty<float> _musicVolume = new ReactiveProperty<float>();
        public IReadOnlyReactiveProperty<TutorialStepsEnum> LuckySpinTutorialStep => _luckySpinTutorialStep;
        public IReadOnlyReactiveProperty<bool> IsADSRemove => _isADSRemove;
        public IReadOnlyReactiveProperty<float> SoundVolume => _soundVolume;
        public IReadOnlyReactiveProperty<float> MusicVolume => _musicVolume;

        #endregion

        #region Setters

        public void SetLuckySpinTutorialStep(TutorialStepsEnum value) => _luckySpinTutorialStep.Value = value;

        public void SetADSRemove(bool value) => _isADSRemove.Value = value;

        public void SetSoundVolume(float value) => _soundVolume.Value = CheckVolumeValue(value);

        public void SetMusicVolume(float value) => _musicVolume.Value = CheckVolumeValue(value);

        private float CheckVolumeValue(float value)
        {
            return value switch
            {
                > 1 => 1,
                < 0 => 0,
                _ => value
            };
        }

        #endregion
        
        #region Storage
        public SubData GetSubData()
        {
            SubData subData = new SubData
            {
                LuckySpinTutorialStep = _luckySpinTutorialStep.Value,
                IsADSRemove = _isADSRemove.Value,
                SoundVolume = _soundVolume.Value,
                MusicVolume = _musicVolume.Value,
            };
            return subData;
        }

        public void SetSubData(SubData subData)
        {
            _luckySpinTutorialStep.Value = subData.LuckySpinTutorialStep;
            _isADSRemove.Value = subData.IsADSRemove;
            _soundVolume.Value = subData.SoundVolume;
            _musicVolume.Value = subData.MusicVolume;
        }
        
        public void SetAndInitEmptySubData(SubData subData)
        {
            subData.SoundVolume = 0.5f;
            subData.MusicVolume = 0.5f;
            SetSubData(subData);
        }
        #endregion
    }
}