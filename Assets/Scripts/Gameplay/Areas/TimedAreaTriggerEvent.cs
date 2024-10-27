using System;
using Common;
using Cysharp.Threading.Tasks;
using Gameplay.Characters;
using UnityEngine;

namespace Gameplay.Areas
{
    public interface ITimedAreaTrigger : IDisposable
    {
        event Action<Collider> OnEventAreaTrigger;
    }
    public class TimedAreaTriggerEvent : ITimedAreaTrigger
    {
        private readonly CustomTimer _timer;
        private readonly CharacterView _view;
        private Collider _areaBuilding;

        public event Action<Collider> OnEventAreaTrigger;
        public TimedAreaTriggerEvent(CharacterView view)
        {
            _view = view;
            _timer = new CustomTimer(1f);

            _view.OnTriggerEnterEvent += EnterAreaHandler;
            _view.OnTriggerExitEvent += ExitAreaHandler;
        }

        private void EnterAreaHandler(Collider building)
        {
            _areaBuilding = building;
            _timer.Dispose();
            _timer.StartAsync(FireEvent).Forget();
        }

        private void FireEvent() => OnEventAreaTrigger?.Invoke(_areaBuilding);

        private void ExitAreaHandler(Collider _)
        {
            _timer.Dispose();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _view.OnTriggerEnterEvent -= EnterAreaHandler;
            _view.OnTriggerExitEvent -= ExitAreaHandler;
        }
    }
}