using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    public class CalculatorModel : Model
    {
        public long Result => (Provider as CalculatorProvider).Result;

        public int ChangeCount { get; set; }
    }
}
