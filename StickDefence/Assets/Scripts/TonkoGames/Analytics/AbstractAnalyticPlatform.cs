namespace TonkoGames.Analytics
{
    public abstract class AbstractAnalyticPlatform
    {
        public virtual void Init()
        {
            
        }
        public virtual void PushEvent(string name)
        {
        }
        public virtual void PushEvent(string name, string value)
        {
        }
        
        public virtual void PushEvent(string name, int value)
        {
        }
        public virtual void PushEvent(string name, string parameterName, string value)
        {
        }
    }
}