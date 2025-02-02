using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;

namespace GuitarAmpSim
{
    public abstract class SampleAggregatorBase : ISampleSource
    {
        private readonly ISampleSource _source;

        public CSCore.WaveFormat WaveFormat => _source.WaveFormat; // Explicitly CSCore

        public bool CanSeek => _source.CanSeek;

        public long Position
        {
            get => _source.Position;
            set => _source.Position = value;
        }

        public long Length => _source.Length;

        protected SampleAggregatorBase(ISampleSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);
            Transform(buffer, offset, samplesRead);
            return samplesRead;
        }

        protected abstract void Transform(float[] buffer, int offset, int count);

        public void Dispose() => _source.Dispose();
    }
}
