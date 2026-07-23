using System;
using System.Threading;

namespace c2flux
{
    public sealed class PauseTokenSource : IDisposable
    {
        private readonly ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);

        public bool IsPaused
        {
            get { return !_pauseEvent.IsSet; }
        }

        public PauseToken Token
        {
            get { return new PauseToken(_pauseEvent); }
        }

        public void Pause()
        {
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            _pauseEvent.Set();
        }

        public void Dispose()
        {
            _pauseEvent.Set();
            _pauseEvent.Dispose();
        }
    }

    public readonly struct PauseToken
    {
        private readonly ManualResetEventSlim _pauseEvent;

        internal PauseToken(ManualResetEventSlim pauseEvent)
        {
            _pauseEvent = pauseEvent;
        }

        public void WaitWhilePaused(CancellationToken cancellationToken)
        {
            _pauseEvent?.Wait(cancellationToken);
        }
    }
}
