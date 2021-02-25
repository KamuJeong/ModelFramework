using System;
using System.Collections.Generic;

namespace Kamu.ModelFramework
{
    public abstract class ModelProvider
    {
        #region [Model factories]

        private Dictionary<string, Func<Model>> _modelFactory = new Dictionary<string, Func<Model>>();

        protected void Register(string query, Func<Model> factory) => _modelFactory.Add(query, factory);

        #endregion

        public Uri Uri { get; }
        
        protected ModelContainer Models { get; }

        public ModelProvider(Uri uri, ModelContainer container)
        {
            Models = container; 
            Uri = uri;
        }

        private Model Get(string query)
        {
            var modelUri = Uri.Model(query);

            try
            {
                var model = Models.Find(modelUri);
                if(model == null)
                {
                    model = _modelFactory[query].Invoke();
                    model.Uri = modelUri;
                    model.Provider = this;
                    Models.Add(model);
                }
                return model;
            }
            catch(KeyNotFoundException)
            {
                throw new ArgumentException($"\'{query}\' is not valid");   
            }
        }

        public virtual bool Open() => true;

        public virtual void Close()     
        { 
        }

        protected void Abort()
        {
            Models.Close(Uri, DetachingSource.Provider);
            Close();
        }

        public Model Load(string query)
        {
            var model = Get(query);
            Load(model);
            model.OnChanged(ChangingSource.Load);
            return model;
        }

        public Model GetOrLoad(Uri modelUri)
        {
            if(modelUri.Provider() == Uri)  
                return GetOrLoad(modelUri.Model());

            ModelProvider provider;
            try
            {
                provider = Models.GetProvider(modelUri.Provider());
            }
            catch
            {
                if(!Models.Open(modelUri.Provider()))
                {
                    throw;
                }
                provider = Models.GetProvider(modelUri.Provider());
            }
            return provider.GetOrLoad(modelUri.Model());
        }

        public Model GetOrLoad(string query) => Models.Find(Uri.Model(query)) ?? Load(query);

        protected abstract void Load(Model model);

        public abstract void Update(Model model);

        public abstract void Save(Model model);

        protected void InvokeChanged(Model model, ChangingSource source) => model.OnChanged(source);
    }   
}
