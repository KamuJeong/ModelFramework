using System;


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

        internal bool IsComeFrom(ModelProvider provider) => Provider == provider;

        public void Detach(DetachingSource source = DetachingSource.Detach) 
        {
            if (IsProviderAttached)
            {
                Provider.Delete(this);
                Provider = null;
                OnDetached(source);
            }
        }

        public void Load()
        {
            if(IsProviderAttached)
            {
                Provider.Load(Uri.Model());
            }
        }

        public void Save()
        {
            if(IsProviderAttached)
            {
                Provider.Save(this);
            }
        }

        internal protected virtual void OnDetached(DetachingSource source) {}

        internal protected virtual void OnChanged(ChangingSource source) {}
    }
}
