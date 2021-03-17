using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    class HelloGuyModel : HelloModel
    {
        public string Name { get; set; }
        public new string Greeting 
        { 
            get => base.Greeting + " " + Name;
            set => base.Greeting = value;
        }
    }
}
