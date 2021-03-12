using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kamu.ModelFramework
{
    /// <summary>
    /// Management of live models 
    /// </summary>
    public sealed class ModelInventory : ModelContainer
    {
        private Dictionary<Uri, Model> _modelCollection = new Dictionary<Uri, Model>();

        public int Count
        {
            get
            {
                lock (_locker)
                {
                    return _modelCollection.Count();
                }
            }
        }

        private Model Find(Uri modelUri) => _modelCollection.TryGetValue(modelUri, out var model) ? model : null;

        private Model Create(Uri modelUri)
        {
            var provider = GetProvider(modelUri.Provider());
            var model = provider.Create(modelUri.Model());
            if (model != null)
            {
                model.Uri = modelUri;
                model.Provider = provider;
                _modelCollection.Add(modelUri, model);
            }
            else
            {
                CleanProvider(provider);
                return null;
            }
            return model;
        }

        private Model GetModelImpl(Uri modelUri)
        {
            lock (_locker)
            {
                return Find(modelUri) ?? Create(modelUri) ?? throw new ArgumentException($"{modelUri}");
            }
        }

        public TModel Get<TModel>(Uri modelUri) where TModel : Model
        {
            var model = GetModelImpl(modelUri);

            if (!model.IsInitialized)
            {
                if (!model.Load())
                {
                    model.Detach();
                    return null;
                }
            }

            return model as TModel ?? throw new InvalidCastException();
        }


        public async ValueTask<TModel> GetAsync<TModel>(Uri modelUri) where TModel : Model
        {
            var model = GetModelImpl(modelUri);

            if (!model.IsInitialized)
            {
                if (!await model.LoadAsync())
                {
                    model.Detach();
                    return null;
                }
            }

            return model as TModel ?? throw new InvalidCastException();
        }


        public void Delete(Model model)
        {
            lock (_locker)
            {
                _modelCollection.Remove(model.Uri);
                CleanProvider(model.Provider);
            }
        }

        private void CleanProvider(ModelProvider provider)
        {
            if (!provider.IsClosed)
            {
                if (_modelCollection.All(p => !p.Value.IsComeFrom(provider)))
                {
                    Delete(provider);
                }
            }
        }

        internal void Abort(ModelProvider provider)
        {
            if (!provider.IsClosed)
            {
                lock (_locker)
                {
                    foreach (var model in _modelCollection
                                            .Where(p => p.Value.IsComeFrom(provider))
                                            .Select(p => p.Value).ToArray())
                    {
                        model.Detach(DetachingSource.Abort);
                    }
                }
            }
        }
    }
}
