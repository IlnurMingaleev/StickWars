using UI.UIManager;

namespace UI.Windows
{
    public class BlockerWindow : Window
    {
        public override WindowPriority Priority { get; set; } = WindowPriority.Blocker;
        protected override bool DisableMultiTouchOnShow => false;
        
    }
}