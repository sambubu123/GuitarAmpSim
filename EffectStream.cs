using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;

namespace GuitarAmpSim
{
    public class EffectStream : SampleAggregatorBase
    {
        public EffectStream(ISampleSource source) : base(source) { }

        protected override void Transform(float[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
            {
                buffer[i] = (float)Math.Tanh(buffer[i] * 5.0f);
            }
            Console.WriteLine($"Processing Audio - First Sample Value: {buffer[0]}");
        }
    }
}
