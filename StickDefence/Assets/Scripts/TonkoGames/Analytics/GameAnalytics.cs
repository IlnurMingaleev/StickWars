using Tools.GameTools;
using UnityEngine;

namespace TonkoGames.Analytics
{
    public class GameAnalytics : Singleton<GameAnalytics>
    {
        private AbstractAnalyticPlatform _analyticPlatform;
 
        private void Awake()
        {
#if UNITY_EDITOR
            _analyticPlatform = new FirebaseAnalyticPlatform();
#elif UNITY_WEBGL
            _analyticPlatform = new WebAnalyticPlatform();
#elif UNITY_ANDROID || UNITY_IPHONE
            _analyticPlatform = new FirebaseAnalyticPlatform();
#endif
            _analyticPlatform.Init();
        }

        public void PushEvent(string name) => _analyticPlatform.PushEvent(name);
        public void PushEvent(string name, string value) => _analyticPlatform.PushEvent(name, value);
        public void PushEvent(string name, int value) => _analyticPlatform.PushEvent(name, value);

        public void PushEvent(string name, string parameterName, string value) => _analyticPlatform.PushEvent(name, parameterName, value);
    }
}