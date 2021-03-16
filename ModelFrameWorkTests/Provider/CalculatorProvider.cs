using System;
using System.Threading;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    [Scheme("calculator")]
    public class CalculatorProvider : ModelProviderWithExclusiveScheduler
    {
        private long _result;
        public long Result => _result;

        protected override Model Create(string query)
        {
            if (query == "note")
            {
                return new CalculatorModel();
            }
            return null;
        }

        protected override bool Opening()
        {
            Interlocked.Exchange(ref _result, 5);

            return true;
        }

        protected override void Closing() {}

        protected override bool Loading(Model model)
        {
            Interlocked.Add(ref _result, 2);
            return true;
        }

        protected override bool Saving(Model model)
        {
            Interlocked.Exchange(ref _result, Result * 3);
            return true;
        }
    }
}
