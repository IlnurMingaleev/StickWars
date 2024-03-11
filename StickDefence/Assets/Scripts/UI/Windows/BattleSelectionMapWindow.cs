using UI.UIManager;

namespace UI.Windows
{
    public class BattleSelectionMapWindow : Window
    {
        protected override void OnActivate()
        {
            base.OnActivate();
            _manager.AddCurrentWindow(this);
        }
    }
}