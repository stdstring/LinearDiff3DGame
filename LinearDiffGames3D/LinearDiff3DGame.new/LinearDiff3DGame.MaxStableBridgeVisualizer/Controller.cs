using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.MaxStableBridge.Old;
using LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer
{
    internal class Controller
    {
        public Controller()
        {
            approxComparer = new ApproxComp(epsilon);
        }

        public IList<MaxStableBridgeSection> CalculateSectionList(Double finishT)
        {
            MaxStableBridgeBuilder_old builder = new MaxStableBridgeBuilder_old();

            Double currentT = builder.CurrentInverseTime;

            IList<MaxStableBridgeSection> sectionList =
                new List<MaxStableBridgeSection>((Int32) (finishT/builder.DeltaT) + 2);
            sectionList.Add(new MaxStableBridgeSection(currentT,
                new Polyhedron(builder.CurrentPolyhedron)));
            while (approxComparer.LE(currentT, finishT))
            {
                builder.NextIteration();
                currentT = builder.CurrentInverseTime;
                sectionList.Add(new MaxStableBridgeSection(currentT,
                    new Polyhedron(builder.CurrentPolyhedron)));
            }

            return sectionList;
        }

        private const Double epsilon = 1e-9;
        private readonly ApproxComp approxComparer;
    }
}
