using Abu;
using UI.UIManager;
using UnityEngine;

namespace UI.Windows
{
    public class TutorialWindow : Window
    {
        [SerializeField] private TutorialFadeImage _tutorialFadeImage;
        [SerializeField] private float _fadeWeight;

        public TutorialFadeImage TutorialFadeImage => _tutorialFadeImage;

        public void SetFadeNormal()
        {
            var colorTmp = _tutorialFadeImage.color;
            colorTmp.a = _fadeWeight;
            _tutorialFadeImage.color = colorTmp;
        }

        public void SetFadeZero()
        {
            var colorTmp = _tutorialFadeImage.color;
            colorTmp.a = 0;
            _tutorialFadeImage.color = colorTmp;
        }
    }
}