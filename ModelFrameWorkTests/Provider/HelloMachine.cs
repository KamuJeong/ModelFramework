using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kamu.ModelFrameworkTests
{
    [Scheme("hello")]
    class HelloMachine : ModelProvider
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
                case "good":
                    return new GoodModel();
                case "greeting":
                    return new HelloModel(good: Models.Get<GoodModel>(Uri.Model("good")), empty: Models.Get<EmptyModel>(Uri.Scheme("empty").Model("empty")));
            }
            return null;
        }

        protected override bool Opening() => true;

        protected override void Closing() { }

        #endregion

        #region [Load]

        protected override bool Loading(Model model) => LoadModel((dynamic)model);

        private bool LoadModel(EmptyModel model) => true;

        private bool LoadModel(GoodModel model) => true;

        private bool LoadModel(HelloModel model)
        {
            model.Greeting = "Hello";
            return true;
        }

        #endregion

        #region [Save]

        protected override bool Saving(Model model) => SaveModel((dynamic)model, ChangingSource.Save);

        public static Dictionary<string, string> Responses = new Dictionary<string, string>
        {
            ["hi"] = "Hello",
            ["how are you?"] = "I'm fine"
        };

        private bool SaveModel(HelloModel model, ChangingSource source)
        {
            try
            {
                model.Greeting = Responses[model.Greeting.ToLower()];
                InvokeChanged(model, source);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        private bool SaveModel(GoodModel model, ChangingSource source)
        {
            var greet = Models.Get<HelloModel>(Uri.Model("greeting"));

            switch (System.DateTime.Now.TimeOfDay.Hours)
            {
                case int h when h < 5:
                    greet.Greeting = "Good night!";
                    break;
                case int h when h < 12:
                    greet.Greeting = "Good morning!";
                    break;
                case int h when h < 17:
                    greet.Greeting = "Good afternoon!";
                    break;
                default:
                    greet.Greeting = "Good evening!";
                    break;
            }
            InvokeChanged(greet, source);

            return true;
        }

        #endregion

        /// <summary>
        /// Model specific function
        /// </summary>

        #region [Etc]

        public void WhoAreYou()
        {
            var model = Models.Get<HelloModel>(Uri.Model("greeting"));
            model.Greeting = "Hi! I'm a \'HelloMachine\'";

            InvokeChanged(model, ChangingSource.Provider);
        }

        #endregion
    }
}
