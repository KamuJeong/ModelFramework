using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kamu.ModelFramework
{
    public enum ChangingSource
    {
        Load,
        Save,
        Provider
    }

    public enum DetachingSource
    {
        Detach,
        Abort
    }

    public abstract class Model
    {
        public Uri Uri { get; internal set; }

        internal protected ModelProvider Provider { get; internal set; }

        public bool IsProviderAttached => Provider != null;

        public bool IsInitialized { get; private set; }

        public bool IsLoaded { get; private set; }

        internal bool IsComeFrom(ModelProvider provider) => Provider == provider;

        public bool Load()
        {
            if (IsProviderAttached)
            {
                IsInitialized = true;
                return Provider.Load(this);
            }
            return false;
        }

        public ValueTask<bool> LoadAsync()
        {
            if (IsProviderAttached)
            {
                IsInitialized = true;
                return Provider.LoadAsync(this);
            }
            return new ValueTask<bool>(false);
        }

        public bool Save()
        {
            if (IsProviderAttached)
            {
                return Provider.Save(this);
            }
            return false;
        }

        public ValueTask<bool> SaveAsync()
        {
            if (IsProviderAttached)
            {
                return Provider.SaveAsync(this);
            }
            return new ValueTask<bool>(false);
        }

        public void Detach(DetachingSource source = DetachingSource.Detach)
        {
            if (IsProviderAttached)
            {
                Provider.Delete(this);
                Provider = null;
                OnDetached(source);

                _changedEventCollection.Clear();
                _detachedEventCollection.Clear();
            }
        }

        private List<(EventHandler<EventArgs>, SynchronizationContext)> _changedEventCollection = new List<(EventHandler<EventArgs>, SynchronizationContext)>();

        private List<(EventHandler<EventArgs>, SynchronizationContext)> _detachedEventCollection = new List<(EventHandler<EventArgs>, SynchronizationContext)>();

        private void AddHandler(List<(EventHandler<EventArgs>, SynchronizationContext)> list, EventHandler<EventArgs> handler)
        {
            lock (list)
            {
                list.Add((handler, SynchronizationContext.Current));
            }
        }

        private void RemoveHandler(List<(EventHandler<EventArgs>, SynchronizationContext)> list, EventHandler<EventArgs> handler)
        {
            lock (list)
            {
                list.Remove((handler, SynchronizationContext.Current));
            }
        }

        public event EventHandler<EventArgs> Changed
        {
            add => AddHandler(_changedEventCollection, value);
            remove => RemoveHandler(_changedEventCollection, value);
        }

        public event EventHandler<EventArgs> Detached
        {
            add => AddHandler(_detachedEventCollection, value);
            remove => RemoveHandler(_detachedEventCollection, value);
        }

        internal virtual void OnChanged(ChangingSource source)
        {
            IsLoaded = true;

            var list = _changedEventCollection;
            var eventArgs = new ChangingSourceEventArgs(source);

            lock (_changedEventCollection)
            {
                _changedEventCollection = new List<(EventHandler<EventArgs>, SynchronizationContext)>();

                foreach ((var handler, var context) in list)
                {
                    eventArgs.LeaveAlive = false;

                    if (context != null) context.Post(_ => handler(this, eventArgs), null);
                    else handler(this, eventArgs);

                    if (eventArgs.LeaveAlive) _changedEventCollection.Add((handler, context));
                }
            }
        }

        internal void OnDetached(DetachingSource source)
        {
            lock (_detachedEventCollection)
            {
                foreach ((var handler, var context) in _detachedEventCollection)
                {
                    if (context != null) context.Post(_ => handler(this, new DetachingSourceEventArgs(source)), null);
                    else handler(this, new DetachingSourceEventArgs(source));
                }
                _detachedEventCollection.Clear();
            }
        }
    }
}
