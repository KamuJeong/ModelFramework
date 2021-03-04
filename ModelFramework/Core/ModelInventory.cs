using System;
using System.Collections.Generic;
using System.Linq;

namespace Kamu.ModelFramework
{
    /// <summary>
    /// Management of live models 
    /// </summary>
    public sealed class ModelInventory : ModelContainer
    {
        private Dictionary<Uri, Model> _modelCollection = new Dictionary<Uri, Model>();

        public int Count => _modelCollection.Count();
 
        internal override void Abort(ModelProvider provider)
        {
            foreach(var model  in _modelCollection
                                    .Where(p => p.Value.IsComeFrom(provider))
                                    .Select(p => p.Value).ToArray())
            {
                model.Detach(DetachingSource.Abort);
            }
        }

        internal override void TryRemove(ModelProvider provider)
        {
            if (_modelCollection.All(p => !p.Value.IsComeFrom(provider)))
            {
                _providerCollection.Remove(provider.Uri);
                provider.Close();
            }
        }

        public TModel Get<TModel>(Uri modelUri) where TModel : Model
            => ((Find(modelUri) ?? GetProvider(modelUri.Provider()).Load(modelUri.Model())) as TModel) 
                ?? throw new InvalidCastException();
 
        internal override void Add(Model model)  => _modelCollection.Add(model.Uri, model);

        internal override void Delete(Model model)
        {
            _modelCollection.Remove(model.Uri);
            TryRemove(model.Provider);
        }

        internal override Model Find(Uri modelUri) => _modelCollection.TryGetValue(modelUri, out var model)?  model : null;
    }
}
