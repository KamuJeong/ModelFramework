using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    public class NoSchemeProvider : ModelProvider
    {
        public override void Save(Model model)
        {
            throw new NotImplementedException();
        }

        public override void Update(Model model)
        {
            throw new NotImplementedException();
        }

        protected override Model Create(string query)
        {
            throw new NotImplementedException();
        }

        protected override void Load(Model model)
        {
            throw new NotImplementedException();
        }
    }
}
