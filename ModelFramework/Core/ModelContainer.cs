using System;
using System.Collections.Generic;
using System.Linq;

namespace Kamu.ModelFramework
{
    /// <summary>
    /// Management of live providers
    /// </summary>
    public abstract class ModelContainer
    {
        private Dictionary<Uri, ModelProvider> _providerCollection = new Dictionary<Uri, ModelProvider>();

        public IEnumerable<Uri> GetProviders() => _providerCollection.Keys;

        internal protected ModelProvider GetProvider(Uri providerUri)
        {
            if(_providerCollection.TryGetValue(providerUri, out ModelProvider provider))
            {
                return provider;
            }
            throw new ArgumentException("not opened");
        }

        public bool Open(Uri providerUri)
        {
            if(string.IsNullOrEmpty(providerUri.Scheme))
                return false;

            if(GetProviders().Any(p => p == providerUri))
                throw new ArgumentException("alreay opened");

            var provider = ModelProviderFactory.Create(providerUri, this);
            if(provider.Open())
            {
                _providerCollection.Add(providerUri, provider);
                return true;
            }

            return false;
        }

        public virtual void Close(Uri providerUri, DetachingSource source)
        {
            if(_providerCollection.TryGetValue(providerUri, out ModelProvider provider))
            {
                _providerCollection.Remove(providerUri);
                provider.Close();
            }
        }

        internal abstract void Add(Model model); 
        
        internal abstract Model Find(Uri modelUri);

    }
}
