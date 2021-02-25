using System;

namespace Kamu.ModelFramework
{
    public class ChangingSourceEventArgs : EventArgs
    {
        public ChangingSource ChangingSource { get; }

        public ChangingSourceEventArgs(ChangingSource source) => ChangingSource = source; 
    }

    public class DetachingSourceEventArgs : EventArgs
    {
        public DetachingSource DetachingSource { get; }

        public DetachingSourceEventArgs(DetachingSource source) => DetachingSource = source;
    }

    public abstract class LiveModel : Model
    {
        public event EventHandler<EventArgs> Changed;

        public event EventHandler<EventArgs> Detached;

        internal protected sealed override void OnChanged(ChangingSource source) => Changed?.Invoke(this, new ChangingSourceEventArgs(source));

        internal protected sealed override void OnDetached(DetachingSource source) => Detached?.Invoke(this, new DetachingSourceEventArgs(source));
    }
}
