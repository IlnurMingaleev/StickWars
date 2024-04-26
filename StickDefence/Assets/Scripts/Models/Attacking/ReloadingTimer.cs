using System;
using UniRx;

namespace Models.Attacking
{
    public class ReloadingTimer
    {
        private int _currentMilliseconds = 0;
        private event Action TimeEnd;
        private readonly CompositeDisposable _disposable = new();
        
        public bool IsReloading { get; private set; }
        public void Init(int milliseconds, Action timeEnd)
        {
            _currentMilliseconds = milliseconds;
            TimeEnd = timeEnd;
        }
        
        public void StartTick()
        {
            if (_currentMilliseconds <= 0)
                return;

            _disposable.Clear();
            
            IsReloading = true;
            
            Observable.Timer (TimeSpan.FromMilliseconds(100))
                .Repeat()
                .Subscribe(_ => TimerSet()).AddTo(_disposable); 
        }

        public void FinishTimer()
        {
            IsReloading = false;
            _currentMilliseconds = 0;
            TimeEnd = null;
            Clear();
        }
        
        private void TimerSet()
        {
            _currentMilliseconds -= 100;
            if (_currentMilliseconds <= 0)
            {
                EndTick();
            }
        }

        private void EndTick()
        {
            TimeEnd?.Invoke();
            Clear();
        }

        public void Clear()
        {
            _disposable.Clear();
        }
    }
}