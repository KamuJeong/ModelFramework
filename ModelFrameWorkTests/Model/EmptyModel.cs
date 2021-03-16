using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    class EmptyModel : Model
    {
        public override void CopyFrom(Model model)
        {
            var good = (EmptyModel)model;
        }
    }
}
