using System;
using System.Collections.Generic;
using System.Linq;

namespace Kamu.ModelFramework
{
    /// <summary>
    /// Management of live models 
    /// </summary>
    public class ModelInventory : ModelContainer
    {
        private Dictionary<Uri, Model> _modelCollection = new Dictionary<Uri, Model>();

       public int Count => _modelCollection.Count();
 
        public override void Close(Uri providerUri, DetachingSource source = DetachingSource.Close)
        {
            Clear(providerUri, source);
            base.Close(providerUri, source);
        }

        private void Clear(Uri providerUri, DetachingSource source)
        {
            var items = _modelCollection.Where(kv => kv.Value.Provider.Uri == providerUri).ToArray();
            foreach (var item in items)
            {
                item.Value.Provider = null;
                item.Value.OnDetached(source);
                _modelCollection.Remove(item.Key);
            }
        }        

        public TModel Get<TModel>(Uri modelUri) where TModel : Model
            => ((Find(modelUri) ?? GetProvider(modelUri.Provider()).Load(modelUri.Model())) as TModel) 
                ?? throw new InvalidCastException();
 
        internal override void Add(Model model)  => _modelCollection.Add(model.Uri, model);

        internal override Model Find(Uri modelUri) => _modelCollection.TryGetValue(modelUri, out var model)?  model : null;
    }
}
