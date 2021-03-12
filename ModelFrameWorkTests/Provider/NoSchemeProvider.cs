using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    public class NoSchemeProvider : ModelProvider
    {
        protected override bool Opening() => true;

        protected override void Closing() {}

        protected override bool Saving(Model model)
        {
            throw new NotImplementedException();
        }

        protected override Model Create(string query)
        {
            throw new NotImplementedException();
        }

        protected override bool Loading(Model model)
        {
            throw new NotImplementedException();
        }
    }
}
