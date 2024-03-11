using UI.UIManager;

namespace Tools.Blockers
{
    public class GlobalWindowBlockerDisposable
    {
        public static GlobalWindowBlocker SetBlock(IWindowManager windowManager, bool useFade = false)
        {
            var result = new GlobalWindowBlocker(windowManager);
            result.Block(useFade);
            return result;
        }

        public static GlobalWindowBlockerWithWaiter SetBlockerWithWaiter(IWindowManager windowManager, bool useFade = false)
        {
            var result = new GlobalWindowBlockerWithWaiter(windowManager);
            result.Block(useFade);
            return result;
        }
    }
}