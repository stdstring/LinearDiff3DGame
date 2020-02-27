using System;
using LinearDiff3DGame.AdvMath.Common;

namespace LinearDiff3DGame.Geometry3D.Common
{
    public class Vector3DApproxComparer
    {
        public Vector3DApproxComparer(ApproxComp comparer)
        {
            this.comparer = comparer;
        }

        public Boolean Equals(Vector3D vector1, Vector3D vector2)
        {
            return comparer.EQ(0, (vector1 - vector2).Length);
        }

        private readonly ApproxComp comparer;
    }
}