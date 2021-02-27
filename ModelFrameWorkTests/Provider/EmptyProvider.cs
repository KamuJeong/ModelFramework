using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kamu.ModelFrameWorkTests
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

        #region [Update]

        public override void Update(Model model) =>  UpdateModel((dynamic)model, ChangingSource.Update);
        private void UpdateModel(EmptyModel model, ChangingSource source) 
        {
            var greet = GetOrLoad(Uri.Scheme("hello").Model("greeting")) as HelloModel;
            greet.Greeting = "Anybody here?";
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