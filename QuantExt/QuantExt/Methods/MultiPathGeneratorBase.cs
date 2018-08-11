using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QuantExt
{
    public abstract class MultiPathGeneratorBase
    {
        public abstract Sample<MultiPath> Next();
        public abstract void Reset();
    }

    public class MultiPathGeneratorMersenneTwister : MultiPathGeneratorBase
    {
        private Sample<MultiPath> _next;
        private TimeGrid _grid;
        private ulong _seed;
        private bool _antitheticSampling;
        private bool _antitheticVariate;
        private QLNet.StochasticProcess _process;
        private MultiPathGenerator<InverseCumulativeRsg<RandomSequenceGenerator<MersenneTwisterUniformRng>, InverseCumulativeNormal>> _pg;                    
        
        public MultiPathGeneratorMersenneTwister(QLNet.StochasticProcess process, TimeGrid grid, ulong seed, bool antitheticSampling)
        {
            _process = process;
            _grid = grid;
            _seed = seed;
            _antitheticSampling = antitheticSampling;
            _antitheticVariate = true;

            Reset();            
        }

        public override Sample<MultiPath> Next()
        {                        
            Sample<QLNet.IPath> sample = _pg.next();
            MultiPath value = sample.value as MultiPath;
            _next = new Sample<MultiPath>(value, 1.0);
                                   
            return _next;               
        }

        public override void Reset()
        {
            PseudoRandom rsg = new PseudoRandom();
            _pg = rsg.make_sequence_generator(_process.size() * (_grid.size() - 1), _seed) as MultiPathGenerator<InverseCumulativeRsg<RandomSequenceGenerator<MersenneTwisterUniformRng>, InverseCumulativeNormal>>;            
        }
    }
}
