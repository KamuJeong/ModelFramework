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
        protected Dictionary<Uri, ModelProvider> _providerCollection = new Dictionary<Uri, ModelProvider>();

        public IEnumerable<Uri> GetProviders() => _providerCollection.Keys;

        internal protected ModelProvider GetProvider(Uri providerUri)
        {
            if(string.IsNullOrEmpty(providerUri.Scheme))
                throw new ArgumentNullException(nameof(providerUri));

            if (_providerCollection.TryGetValue(providerUri, out ModelProvider provider))
                return provider;

            return Open(providerUri);
        }

        private ModelProvider Open(Uri providerUri)
        {
            var provider = ModelProviderFactory.Create(providerUri, this);
            if(provider.Open())
            {
                _providerCollection.Add(providerUri, provider);
                return provider;
            }
            throw new ArgumentException($"can't open {providerUri}");
        }

        internal abstract void Abort(ModelProvider provider);

        internal abstract void TryRemove(ModelProvider provider);

        internal abstract void Add(Model model); 

        internal abstract void Delete(Model model);

        internal abstract Model Find(Uri modelUri);

    }
}
