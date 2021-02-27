using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests.Provider
{
    class NestedProviders
    {
        [Scheme("nested.public")]
        public class NestedPublicProvider : ModelProvider
        {
            protected override Model Create(string query)
            {
                throw new NotImplementedException();
            }

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

        [Scheme("nested.private")]
        private class NestedPrivateProvider : ModelProvider
        {
            protected override Model Create(string query)
            {
                throw new NotImplementedException();
            }

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
