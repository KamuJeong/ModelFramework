using System;
using System.Collections.Generic;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{ 
    class HelloModel : Model
    {
        public EmptyModel Empty { get; }

        public GoodModel Good { get; }



        public HelloModel(GoodModel good, EmptyModel empty)
        {
            Good = good;
            Empty = empty;

            this.Changed += ModelChanged;
            this.Detached += (s, e) => DetachedCallback = true; 
        }



        private void ModelChanged(object sender, EventArgs e)
        {
            if(e is ChangingSourceEventArgs args)
            {
                ChangingEvents.Add(args.ChangingSource);
            }
        }

        public string Greeting { get; set; }

        public override void CopyFrom(Model model)
        {
            var hello = (HelloModel)model;
            Greeting = hello.Greeting;
        }

        public List<ChangingSource> ChangingEvents { get; } = new List<ChangingSource>();

        public int ChangedCount => ChangingEvents.Count;

        public bool DetachedCallback { get; private set; }

        public void WhoAreYou() => (Provider as HelloMachine)?.WhoAreYou();
    }
}
