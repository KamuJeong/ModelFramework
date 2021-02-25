using System;
using System.Collections.Generic;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameWorkTests
{ 
    public class HelloModel : Model
    {
        public EmptyModel Empty { get; }

        public GoodModel Good { get; }

        public HelloModel(GoodModel good, EmptyModel empty)
        {
            Good = good;
            Empty = empty;
        }

        public string Greeting { get; set; }

        public List<ChangingSource> ChangingEvents { get; } = new List<ChangingSource>();

        public int ChangedCount => ChangingEvents.Count;

        public bool DetachedCallback { get; private set; }

        protected override void OnChanged(ChangingSource source)
        {
            ChangingEvents.Add(source);
        }

        protected override void OnDetached(DetachingSource source)
        {
            DetachedCallback = true;
        }

        public void WhoAreYou() => (Provider as HelloMachine)?.WhoAreYou();
    }
}
