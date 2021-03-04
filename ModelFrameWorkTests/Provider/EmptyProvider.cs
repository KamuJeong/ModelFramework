using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kamu.ModelFrameworkTests
{
    [Scheme("empty")]
    class EmptyMachine : ModelProvider
    {
        /// <summary>
        /// Model basic functions
        /// 1. Create models
        /// 2. Load models
        /// 3. Update models
        /// 4. Save models
        /// </summary>
 
        #region [Create]

        protected override Model Create(string query)
        {
            switch(query)
            {
            case "empty":   return new EmptyModel();
            }
            return null;
        }

        #endregion

        #region [Load]

        protected override void Load(Model model) => LoadModel((dynamic)model);

        private void LoadModel(EmptyModel model) 
        {
        }

        #endregion

        #region [Save]

        public override void Save(Model model) =>  SaveModel((dynamic)model, ChangingSource.Save);
        private void SaveModel(EmptyModel model, ChangingSource source) 
        {
            var greet = GetOrLoad(Uri.Scheme("hello").Model("greeting")) as HelloModel;
            greet.Greeting = "Anybody here?";
        }

        #endregion

        /// <summary>
        /// Model specific function
        /// </summary>

        #region [Etc]

        #endregion
    }

    [Scheme("empty")]
    class NotImplementedEmptyMachine : ModelProvider
    {
        /// <summary>
        /// Model basic functions
        /// 1. Create models
        /// 2. Load models
        /// 3. Update models
        /// 4. Save models
        /// </summary>
 
        #region [Create]

        protected override Model Create(string query)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [Load]

        protected override void Load(Model model)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [Save]

        public override void Save(Model model)
        {
            throw new NotImplementedException();
        }



        #endregion

        /// <summary>
        /// Model specific function
        /// </summary>

        #region [Etc]

        #endregion
    }    
}