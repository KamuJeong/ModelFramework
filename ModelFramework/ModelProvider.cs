using System;
using System.Collections.Generic;

namespace Kamu.ModelFramework
{
    public abstract class ModelProvider
    {
        #region [Provider primitives]

        public Uri Uri { get; private set; }
        
        protected ModelContainer Models { get; private set; }

        protected ModelProvider()
        {      
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

        #endregion

        #region [Model primitives]

        protected abstract Model Create(string query);

        protected abstract void Load(Model model);

        public abstract void Update(Model model);

        public abstract void Save(Model model);

        #endregion

        #region [Helpers]
        
        public Model Load(string query)
        {
            var model = Get(query);
            Load(model);
            model.OnChanged(ChangingSource.Load);
            return model;
        }

        private Model Get(string query)
        {
            var modelUri = Uri.Model(query);
            var model = Models.Find(modelUri);
            if (model == null)
            {
                model = Create(query);
                if (model == null)
                {
                    throw new ArgumentException($"\'{query}\' is not valid");
                }
                model.Uri = modelUri;
                model.Provider = this;
                Models.Add(model);
            }
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

        protected void InvokeChanged(Model model, ChangingSource source) => model.OnChanged(source);

        #endregion
    }   
}
