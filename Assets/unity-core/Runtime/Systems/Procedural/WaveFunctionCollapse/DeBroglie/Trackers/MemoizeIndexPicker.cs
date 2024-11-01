using DeBroglie.Wfc;
using System;

namespace DeBroglie.Trackers
{
    internal class MemoizeIndexPicker : IIndexPicker, IChoiceObserver
    {
        private Deque<int> prevChoices;
        private Deque<int> futureChoices;
        private readonly IIndexPicker underlying;

        public MemoizeIndexPicker(IIndexPicker underlying)
        {
            this.underlying = underlying;
        }

        public void Init(WavePropagator wavePropagator)
        {
            wavePropagator.AddChoiceObserver(this);
            this.futureChoices = new Deque<int>();
            this.prevChoices = new Deque<int>();
        }

        public void Backtrack()
        {
            this.futureChoices.Shift(this.prevChoices.Pop());
        }

        public int GetRandomIndex(Func<double> randomDouble)
        {
            if (this.futureChoices.Count > 0)
            {
                return this.futureChoices.Unshift();
            }
            else
            {
                return this.underlying.GetRandomIndex(randomDouble);
            }
        }

        public void MakeChoice(int index, int pattern)
        {
            this.prevChoices.Push(index);
        }
    }
}
