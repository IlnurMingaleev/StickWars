#if  UNITY_ANDROID || UNITY_IPHONE
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
#endif
using UnityEngine;

namespace TonkoGames.Analytics
{
    public class FirebaseAnalyticPlatform : AbstractAnalyticPlatform
    {
#if  UNITY_ANDROID || UNITY_IPHONE
        private bool _canUseAnalytics;
        
        public override void Init()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    Debug.Log($"Firebase: {dependencyStatus} :)");
                    _canUseAnalytics = true;
                } else {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        
        public override void PushEvent(string name)
        {
            if (!_canUseAnalytics)
                return;

            FirebaseAnalytics.LogEvent(name);
        }

        public override void PushEvent(string name, string parameterName, string value)
        {
            if (!_canUseAnalytics)
                return;

            FirebaseAnalytics.LogEvent(name, parameterName, value); 
        }
        
        // public void LevelUp(string eventName)
        // {
        //     if(!_canUseAnalytics)
        //         return;
        //     FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin, new Parameter(FirebaseAnalytics.ParameterLevelName, eventName));
        // }
        
        // public void AnalyticsProgress() {
        //     // Log an event with a float.
        //     FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);
        //     FirebaseAnalytics.LogEvent("progress", "percent", 0);
        //     FirebaseAnalytics.LogEvent("tutorial_default");
        //     FirebaseAnalytics.LogEvent("tutorial", "name", "tutorial_default1");
        //     FirebaseAnalytics.LogEvent("tutorial", "name", "tutorial_default2");
        //     FirebaseAnalytics.LogEvent("tutorial", "name", "tutorial_default3");
        //     FirebaseAnalytics.LogEvent("tutorial", "event_name", "");
        // }
        //
        //
        // {
        //     // Log an event with multiple parameters.
        //     FirebaseAnalytics.LogEvent(
        //         FirebaseAnalytics.EventLevelUp,
        //         new Parameter(FirebaseAnalytics.ParameterLevel, 5),
        //         new Parameter(FirebaseAnalytics.ParameterCharacter, "mrspoon"),
        //         new Parameter("hit_accuracy", 3.14f));
        // }
#endif
    }
}