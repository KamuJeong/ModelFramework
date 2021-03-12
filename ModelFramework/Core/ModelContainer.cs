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
        protected object _locker = new object();

        protected Dictionary<Uri, ModelProvider> _providerCollection = new Dictionary<Uri, ModelProvider>();

        public IEnumerable<Uri> Providers
        {
            get
            {
                lock (_locker)
                {
                    return _providerCollection.Keys.ToArray();
                }
            }
        }

        protected void Delete(ModelProvider provider)
        {
            _providerCollection.Remove(provider.Uri);
            provider.Close();
        }

        protected ModelProvider GetProvider(Uri providerUri)
        {
            if (string.IsNullOrEmpty(providerUri.Scheme))
                throw new ArgumentNullException(nameof(providerUri));

            if (_providerCollection.TryGetValue(providerUri, out ModelProvider provider))
                return provider;

            provider = ModelProviderFactory.Create(providerUri, this);
            _providerCollection.Add(providerUri, provider);

            return provider;
        }
    }
}
