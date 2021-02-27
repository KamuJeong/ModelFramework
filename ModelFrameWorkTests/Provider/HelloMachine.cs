using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kamu.ModelFrameWorkTests
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
            switch(query)
            {
            case "good":        
                return new GoodModel();
            case "greeting":    
                return new HelloModel(good:(GoodModel)GetOrLoad("good"), empty:(EmptyModel)GetOrLoad(Uri.Scheme("empty").Model("empty")));
            }
            return null;
        }

        #endregion

        #region [Load]

        protected override void Load(Model model) => LoadModel((dynamic)model);

        private void LoadModel(EmptyModel model) 
        {

        }

        private void LoadModel(GoodModel model)
        {
        }

        private void LoadModel(HelloModel model)
        {
            model.Greeting = "Hello";
        }

        #endregion

        #region [Update]

        public override void Update(Model model) =>  UpdateModel((dynamic)model, ChangingSource.Update);
       
        public static Dictionary<string, string> Responses = new Dictionary<string, string>
        {
            ["hi"] = "Hello",
            ["how are you?"] = "I'm fine"
        };

        private void UpdateModel(HelloModel model, ChangingSource source) 
        {
            try
            {
                model.Greeting = Responses[model.Greeting.ToLower()];
                InvokeChanged(model, source);
            }
            catch(KeyNotFoundException)
            {
                throw new InvalidOperationException($"\'Can't respond to {model.Greeting}\'");
            }
        }

        private void UpdateModel(GoodModel model, ChangingSource source)
        {
            var greet = GetOrLoad("greeting") as HelloModel;

            switch(System.DateTime.Now.TimeOfDay.Hours)
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

        public override void Close()     
        { 
            Models.Close(Uri.Scheme("empty"), DetachingSource.Close);
        }

        public void WhoAreYou()
        {
            var model = GetOrLoad("greeting") as HelloModel;
            model.Greeting = "Hi! I'm a \'HelloMachine\'";

            InvokeChanged(model, ChangingSource.Provider);
        }

        #endregion
    }
}
 