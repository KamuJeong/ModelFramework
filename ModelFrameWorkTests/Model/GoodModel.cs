using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    class GoodModel : Model
    {
        public void Abort() => Provider.Abort();
    }    
}