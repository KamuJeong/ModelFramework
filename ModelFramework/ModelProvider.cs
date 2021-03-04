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

        public void Abort() => Models.Abort(this);

        #endregion

        #region [Model primitives]

        protected abstract Model Create(string query);

        protected abstract void Load(Model model);

        public abstract void Save(Model model);

        #endregion

        #region [Helpers]
        
        public Model Load(string query)
        {
            try
            {
                var model = Get(query);
                Load(model);
                model.OnChanged(ChangingSource.Load);
                return model;
            }
            catch
            {
                Models.TryRemove(this);       
                throw;
            }
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

        internal void Delete(Model model) => Models.Delete(model);

        public Model GetOrLoad(Uri modelUri) => Models.GetProvider(modelUri.Provider()).GetOrLoad(modelUri.Model());

        public Model GetOrLoad(string query) => Models.Find(Uri.Model(query)) ?? Load(query);

        protected void InvokeChanged(Model model, ChangingSource source) => model.OnChanged(source);


        #endregion
    }   
}
