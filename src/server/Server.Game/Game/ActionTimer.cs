using System;
using System.Threading;

namespace Server.Game
{
    public class ActionTimer : IDisposable
    {
        private readonly Action _action;
        private readonly TimeSpan _delay;

        private bool _actionIsExecuting;
        private Timer _timer;

        public ActionTimer(Action action, TimeSpan delay)
        {
            _action = action;
            _delay = delay;
        }

        public void Start()
        {
            Stop();
            _timer = new Timer(ExecuteAction, null, _delay, _delay);
        }

        private void ExecuteAction(object state)
        {
            if (_actionIsExecuting)
                return;

            try
            {
                _actionIsExecuting = true;
                _action();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            finally
            {
                _actionIsExecuting = false;
            }
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}