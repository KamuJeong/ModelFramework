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

            protected override bool Opening() => true;

            protected override void Closing() { }

            protected override bool Saving(Model model)
            {
                throw new NotImplementedException();
            }

            protected override bool Loading(Model model)
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

            protected override bool Opening() => true;

            protected override void Closing() { }

            protected override bool Saving(Model model)
            {
                throw new NotImplementedException();
            }

            protected override bool Loading(Model model)
            {
                throw new NotImplementedException();
            }
        }
    }
}
