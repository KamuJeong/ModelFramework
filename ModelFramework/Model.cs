using System;


namespace Kamu.ModelFramework
{
    public enum ChangingSource
    {
        Load,
        Update,
        Provider
    }

    public enum DetachingSource
    {
        Close,
        Provider
    }


    public abstract class Model
    {
        internal protected ModelProvider Provider { get; internal set; } 

        public Uri Uri { get; internal set; }

        public bool IsProviderAttached => Provider != null;

        public void Reload()
        {
            if(IsProviderAttached)
            {
                Provider.Load(Uri.Model());
            }
        }

        public void Update()
        {
            if(IsProviderAttached)
            {
                Provider.Update(this);
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
