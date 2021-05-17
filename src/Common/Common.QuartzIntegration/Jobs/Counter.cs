using System.Threading;

namespace Bifrost.Common.QuartzIntegration.Jobs
{
    /// <summary>
    ///     A thread safe counter
    /// </summary>
    public class Counter
    {
        private int _counter;

        public int Value => _counter;

        public void Increment()
        {
            Interlocked.Increment(ref _counter);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _counter);
        }
    }
}