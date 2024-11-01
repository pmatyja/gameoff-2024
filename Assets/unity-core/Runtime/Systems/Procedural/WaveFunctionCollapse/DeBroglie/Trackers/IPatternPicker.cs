using DeBroglie.Wfc;
using System;

namespace DeBroglie.Trackers
{
    /// <summary>
    /// Class implementing the heuristic choice of pattern at a given index
    /// </summary>
    internal interface IPatternPicker
    {
        void Init(WavePropagator wavePropagator);
        int GetRandomPossiblePatternAt(int index, Func<double> randomDouble);
    }
}
