using System;
using System.Collections.Generic;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{ 
    class HelloModel : Model
    {
        [NoCopy]    private EmptyModel _empty;
        public EmptyModel Empty 
        {
            get => _empty;
            set => _empty = value;
        }

        [NoCopy]    private GoodModel _good;
        public GoodModel Good
        {
            get => _good;
            set => _good = value;
        }

        public HelloModel()
        {
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

        [NoCopy]    private List<ChangingSource> _changingEvents = new List<ChangingSource>();
        public List<ChangingSource> ChangingEvents => _changingEvents; 

        public int ChangedCount => ChangingEvents.Count;

        [NoCopy]    private bool _detachedCallback;
        public bool DetachedCallback 
        { 
            get => _detachedCallback;
            private set => _detachedCallback = value;
        }

        public void WhoAreYou() => (Provider as HelloMachine)?.WhoAreYou();
    }
}
