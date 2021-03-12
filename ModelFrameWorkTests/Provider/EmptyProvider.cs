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
            switch (query)
            {
                case "empty": return new EmptyModel();
            }
            return null;
        }

        protected override bool Opening() => true;

        protected override void Closing() { }

        #endregion

        #region [Load]

        protected override bool Loading(Model model) => LoadModel((dynamic)model);

        private bool LoadModel(EmptyModel model) => true;

        #endregion

        #region [Save]

        protected override bool Saving(Model model) => SaveModel((dynamic)model);

        private bool SaveModel(EmptyModel model)
        {
            var greet = Models.Get<HelloModel>(Uri.Scheme("hello").Model("greeting"));
            greet.Greeting = "Anybody here?";

            InvokeChanged(greet, ChangingSource.Save);
            return true;
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

        protected override bool Opening() => true;

        protected override void Closing() { }

        #endregion

        #region [Load]

        protected override bool Loading(Model model)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [Save]

        protected override bool Saving(Model model)
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