using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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

        public virtual void CopyFrom(Model model)
        {
            Type type = GetType();

            if(!type.IsAssignableFrom(model.GetType()))
                throw new InvalidCastException();

            while(type != typeof(Model))
            {
                foreach(var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if(f.GetCustomAttribute(typeof(NoCopyAttribute)) == null)
                    {
                        f.SetValue(this, f.GetValue(model));
                    }
                }
                type = type.BaseType;
            }
        }

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
                int index = list.FindIndex(l => l.Item1 == handler);
                if(index >= 0)
                    list.RemoveAt(index);
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

            (EventHandler<EventArgs>, SynchronizationContext)[] arr = null; 

            lock (_changedEventCollection)
            {
                if(_changedEventCollection.Count > 0)
                {
                    arr = _changedEventCollection.ToArray();
                }
            }

            if(arr != null)
            {
                foreach ((var handler, var context) in arr)
                {
                    if (context != null) context.Post(_ => handler(this, new ChangingSourceEventArgs(source)), null);
                    else handler(this, new ChangingSourceEventArgs(source));
                }
            }
        }

        internal void OnDetached(DetachingSource source)
        {
            (EventHandler<EventArgs>, SynchronizationContext)[] arr = null; 

            lock (_detachedEventCollection)
            {
                if(_detachedEventCollection.Count > 0)
                {
                    arr = _detachedEventCollection.ToArray();
                }
            }

            if(arr != null)                    
            {
                foreach ((var handler, var context) in arr)
                {
                    if (context != null) context.Post(_ => handler(this, new DetachingSourceEventArgs(source)), null);
                    else handler(this, new DetachingSourceEventArgs(source));
                }
            }
        }
    }
}
