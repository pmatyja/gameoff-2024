using DeBroglie.Wfc;
using System;

namespace DeBroglie.Trackers
{
    class WeightedRandomPatternPicker : IPatternPicker
    {
        private Wave wave;

        private double[] frequencies;

        public WeightedRandomPatternPicker()
        {
        }

        public void Init(WavePropagator wavePropagator)
        {
            this.wave = wavePropagator.Wave;
            this.frequencies = wavePropagator.Frequencies;
        }

        public int GetRandomPossiblePatternAt(int index, Func<double> randomDouble)
        {
            return RandomPickerUtils.GetRandomPossiblePattern(this.wave, randomDouble, index, this.frequencies);
        }
    }
}
