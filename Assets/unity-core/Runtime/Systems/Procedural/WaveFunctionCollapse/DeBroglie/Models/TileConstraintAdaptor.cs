using DeBroglie.Constraints;
using DeBroglie.Wfc;

namespace DeBroglie.Models
{
    internal class TileConstraintAdaptor : IWaveConstraint
    {
        private readonly ITileConstraint underlying;
        private readonly TilePropagator propagator;

        public TileConstraintAdaptor(ITileConstraint underlying, TilePropagator propagator)
        {
            this.underlying = underlying;
            this.propagator = propagator;
        }

        public void Check(WavePropagator wavePropagator)
        {
            this.underlying.Check(this.propagator);
        }

        public void Init(WavePropagator wavePropagator)
        {
            this.underlying.Init(this.propagator);
        }
    }
}
