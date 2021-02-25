using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kamu.ModelFrameWorkTests
{
    public class EmptyMachine : ModelProvider
    {
        /// <summary>
        /// Cliche part
        /// 1. Set an apropriate name to the static readonly 'Scheme' property
        /// 2. Define a staic constructor to register a provider factory function
        /// 3. Define instance constructor to register model factory functions
        /// </summary>

        #region [Preparation]

        public static string Scheme { get; } = "empty";
        
        static EmptyMachine() => Kamu.ModelFramework.ModelProviderFactory.Register(Scheme, (uri, container) => new EmptyMachine(uri, container));

        private EmptyMachine(Uri uri, ModelContainer container) : base(uri, container)
        {
            Register("empty", () => new EmptyModel());
        }

        #endregion

        /// <summary>
        /// Basic model management part
        /// 1. Load override function
        /// 1. Update override function
        /// 2. Save override function
        /// </summary>

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