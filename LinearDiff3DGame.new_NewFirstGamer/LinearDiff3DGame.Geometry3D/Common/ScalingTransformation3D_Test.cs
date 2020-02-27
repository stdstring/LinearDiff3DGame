using System;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.MatrixUtils;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.Common
{
    [TestFixture]
    public class ScalingTransformation3D_Test
    {
        [Test]
        public void RevereseMatrix()
        {
            Vector3D direction = Vector3DUtils.NormalizeVector(new Vector3D(1, 2, 0.5));
            const Double scaleRatio = 5.2;
            Matrix directTransformation = ScalingTransformation3D.GetTransformationMatrix(direction, scaleRatio);
            Matrix reverseTransformation = ScalingTransformation3D.GetTransformationMatrix(direction, 1 / scaleRatio);
            Matrix matrixMultiplication = directTransformation * reverseTransformation;
            MatrixApproxComparer matrixComparer = new MatrixApproxComparer(new ApproxComp(epsilon));
            Assert.IsTrue(matrixComparer.Equals(matrixMultiplication, Matrix.IdentityMatrix(3)));
        }

        [Test]
        public void CheckScalingTransformation()
        {
            Vector3D[] vectorSet = new[]
                                       {
                                           new Vector3D(0, 0, 1),
                                           new Vector3D(0, 1, 0),
                                           new Vector3D(1, 0, 0),
                                           new Vector3D(1, 2, 3),
                                           new Vector3D(-2, 1, -3),
                                           new Vector3D(-2, -1, 5)
                                       };
            Assert.IsTrue(CheckScalingTransformation(new Vector3D(0, 0, 1), 2.5, vectorSet));
            Vector3D direction = Vector3DUtils.NormalizeVector(new Vector3D(5, 1, 3));
            Assert.IsTrue(CheckScalingTransformation(direction, 3.512, vectorSet));
        }

        private static Boolean CheckScalingTransformation(Vector3D direction, Double scaleRatio,
                                                          params Vector3D[] vectors)
        {
            Vector3DApproxComparer vectorComparer = new Vector3DApproxComparer(new ApproxComp(epsilon));
            Matrix transformation = ScalingTransformation3D.GetTransformationMatrix(direction, scaleRatio);
            foreach(Vector3D sourceVector in vectors)
            {
                Matrix afterTransformation = transformation * Geometry3DObjectFactory.CreateMatrix(sourceVector);
                Vector3D destVector = Geometry3DObjectFactory.CreateVector(afterTransformation);
                Vector3D sourceParallel = Vector3DUtils.GetParallelComponent(sourceVector, direction);
                Vector3D sourcePerpendicular = Vector3DUtils.GetPerpendicularComponent(sourceVector, direction);
                Vector3D destParallel = Vector3DUtils.GetParallelComponent(destVector, direction);
                Vector3D destPerpendicular = Vector3DUtils.GetPerpendicularComponent(destVector, direction);
                if(!vectorComparer.Equals(scaleRatio * sourceParallel, destParallel)) return false;
                if(!vectorComparer.Equals(sourcePerpendicular, destPerpendicular)) return false;
            }
            return true;
        }

        private const Double epsilon = 1e-9;
    }
}