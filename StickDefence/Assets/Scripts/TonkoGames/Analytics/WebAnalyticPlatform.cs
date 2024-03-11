
namespace TonkoGames.Analytics
{
#if UNITY_WEBGL || UNITY_EDITOR
    public class WebAnalyticPlatform : AbstractAnalyticPlatform
    {
        public override void PushEvent(string name, int value)
        {
            GamePush.GP_Analytics.Goal(name, value);
        }
        
        public override void PushEvent(string name, string value)
        {
            GamePush.GP_Analytics.Goal(name, value);
        }
    }
#endif
}