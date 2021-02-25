using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests.Provider
{
    class NestedProviders
    {
        public class NestedPublicProvider : ModelProvider
        {
            /// <summary>
            /// Cliche part
            /// 1. Set an apropriate name to the static readonly 'Scheme' property
            /// 2. Define a staic constructor to register a provider factory function
            /// 3. Define instance constructor to register model factory functions
            /// </summary>

            #region [Preparation]

            public static string Scheme { get; } = "nested.public";

            static NestedPublicProvider() => Kamu.ModelFramework.ModelProviderFactory.Register(Scheme, (uri, container) => new NestedPublicProvider(uri, container));

            private NestedPublicProvider(Uri uri, ModelContainer container) : base(uri, container)
            {
            }

            #endregion

            public override void Save(Model model)
            {
                throw new NotImplementedException();
            }

            public override void Update(Model model)
            {
                throw new NotImplementedException();
            }

            protected override void Load(Model model)
            {
                throw new NotImplementedException();
            }
        }

        private class NestedPrivateProvider : ModelProvider
        {
            /// <summary>
            /// Cliche part
            /// 1. Set an apropriate name to the static readonly 'Scheme' property
            /// 2. Define a staic constructor to register a provider factory function
            /// 3. Define instance constructor to register model factory functions
            /// </summary>

            #region [Preparation]

            public static string Scheme { get; } = "nested.private";

            static NestedPrivateProvider() => Kamu.ModelFramework.ModelProviderFactory.Register(Scheme, (uri, container) => new NestedPrivateProvider(uri, container));

            private NestedPrivateProvider(Uri uri, ModelContainer container) : base(uri, container)
            {
            }

            #endregion

            public override void Save(Model model)
            {
                throw new NotImplementedException();
            }

            public override void Update(Model model)
            {
                throw new NotImplementedException();
            }

            protected override void Load(Model model)
            {
                throw new NotImplementedException();
            }
        }
    }
}
