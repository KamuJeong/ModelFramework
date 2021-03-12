using System;

namespace Kamu.ModelFramework
{
    public class OneShotEventArgs : EventArgs
    {
        public bool LeaveAlive { get; set; }
    }


    public class ChangingSourceEventArgs : OneShotEventArgs
    {
        public ChangingSource ChangingSource { get; }

        public ChangingSourceEventArgs(ChangingSource source) => ChangingSource = source;
    }

    public class DetachingSourceEventArgs : EventArgs
    {
        public DetachingSource DetachingSource { get; }

        public DetachingSourceEventArgs(DetachingSource source) => DetachingSource = source;
    }
}
