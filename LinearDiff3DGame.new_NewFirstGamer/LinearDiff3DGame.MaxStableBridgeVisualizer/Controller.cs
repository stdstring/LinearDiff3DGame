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
            m_ApproxComparer = new ApproxComp(Epsilon);
        }

        public IList<MaxStableBridgeSection> CalculateSectionList(Double finishT)
        {
            MaxStableBridgeBuilder_old builder = new MaxStableBridgeBuilder_old();

            Double currentT = builder.CurrentInverseTime;

            IList<MaxStableBridgeSection> sectionList =
                new List<MaxStableBridgeSection>((Int32) (finishT/builder.DeltaT) + 2);
            sectionList.Add(new MaxStableBridgeSection(currentT,
                new Polyhedron(builder.CurrentPolyhedron)));
            while (m_ApproxComparer.LE(currentT, finishT))
            {
                builder.NextIteration();
                currentT = builder.CurrentInverseTime;
                sectionList.Add(new MaxStableBridgeSection(currentT,
                    new Polyhedron(builder.CurrentPolyhedron)));
            }

            return sectionList;
        }

        private const Double Epsilon = 1e-9;
        private readonly ApproxComp m_ApproxComparer;
    }
}
