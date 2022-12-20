using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.gpu.mockimpl
{
    public class MockValueProvider
    {
        decimal minValue, maxValue;

        public MockValueProvider(decimal minValue, decimal maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        private static Random Random = new Random();

        public decimal Value => Random.Next((int)minValue, (int)maxValue);
    }
}
