using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public class CustomTimer : IDisposable
    {
        private readonly float _duration;
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public CustomTimer(float duration)
        {
            _duration = duration;
        }
        
        public async UniTask<CustomTimer> StartAsync(Action callback)
        {
            Stop();
            _cts = new CancellationTokenSource();
            _isRunning = true;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: _cts.Token);
                if (!_cts.Token.IsCancellationRequested)
                {
                    callback?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Timer canceled");
            }

            _isRunning = false;
            return this;
        }
        
        public async UniTask<CustomTimer> StartLoopAsync(Action callback)
        {
            Stop();
            _cts = new CancellationTokenSource();
            _isRunning = true;

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: _cts.Token);
                    if (!_cts.Token.IsCancellationRequested)
                    {
                        callback?.Invoke();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Loop force canceled");
            }

            _isRunning = false;
            return this;
        }
        public void Stop()
        {
            if (_isRunning && _cts != null)
            {
                _cts.Cancel();
                _isRunning = false;
            }
        }
        
        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }

        public bool IsActive()
        {
            return _isRunning;
        }
    }
}