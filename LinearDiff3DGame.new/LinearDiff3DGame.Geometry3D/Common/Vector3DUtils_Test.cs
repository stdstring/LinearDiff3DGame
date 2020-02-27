using System;
using LinearDiff3DGame.AdvMath.Common;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.Common
{
    [TestFixture]
    public class Vector3DUtils_Test
    {
        [Test]
        public void GetParallelComponent()
        {
            Vector3D source = new Vector3D(1, 2, 3);
            Vector3D direction = new Vector3D(2, 3, 1);
            Vector3D parallelComponent = Vector3DUtils.GetParallelComponent(source, direction);
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(parallelComponent, direction), 1));
        }

        [Test]
        public void GetPerpendicularComponent()
        {
            Vector3D source = new Vector3D(1, 2, 3);
            Vector3D direction = new Vector3D(2, 3, 1);
            Vector3D perpendicularComponent = Vector3DUtils.GetPerpendicularComponent(source, direction);
            Vector3D parallelComponent = Vector3DUtils.GetParallelComponent(source, direction);
            Assert.IsTrue(approxComp.EQ(parallelComponent * perpendicularComponent, 0));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(perpendicularComponent, direction), 0));
        }

        [Test]
        public void CosAngleBetweenVectors()
        {
            Vector3D vector1 = new Vector3D(1, 0, 0);
            Vector3D vector2 = new Vector3D(Math.Sqrt(3), 1, 0);
            Vector3D vector3 = new Vector3D(1, 1, 0);
            Vector3D vector4 = new Vector3D(1, Math.Sqrt(3), 0);
            Vector3D vector5 = new Vector3D(0, 0, 1);
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(vector1, vector1), 1));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(vector1, vector2), Math.Sqrt(3) / 2));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(vector1, vector3), Math.Sqrt(2) / 2));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(vector1, vector4), 0.5));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.CosAngleBetweenVectors(vector1, vector5), 0));
        }

        [Test]
        public void AngleBetweenVectors()
        {
            Vector3D vector1 = new Vector3D(1, 0, 0);
            Vector3D vector2 = new Vector3D(Math.Sqrt(3), 1, 0);
            Vector3D vector3 = new Vector3D(1, 1, 0);
            Vector3D vector4 = new Vector3D(1, Math.Sqrt(3), 0);
            Vector3D vector5 = new Vector3D(0, 0, 1);
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.AngleBetweenVectors(vector1, vector1), 0));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.AngleBetweenVectors(vector1, vector2), Math.PI / 6));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.AngleBetweenVectors(vector1, vector3), Math.PI / 4));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.AngleBetweenVectors(vector1, vector4), Math.PI / 3));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.AngleBetweenVectors(vector1, vector5), Math.PI / 2));
        }

        [Test]
        public void NormalizeVector()
        {
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.NormalizeVector(new Vector3D(1, 2, 3)).Length, 1));
            Assert.Throws<ArgumentException>(() => Vector3DUtils.NormalizeVector(Vector3D.ZeroVector3D));
        }

        [Test]
        public void ScalarProduct()
        {
            Vector3D vector1 = new Vector3D(Math.Sqrt(3), 1, 0);
            Vector3D vector2 = new Vector3D(1, Math.Sqrt(3), 0);
            Double expectedScalarProduct = 4 * Math.Cos(Math.PI / 6);
            Assert.IsTrue(approxComp.EQ(vector1 * vector2, expectedScalarProduct));
        }

        [Test]
        public void VectorProduct()
        {
            Vector3D vector1 = new Vector3D(Math.Sqrt(3), 1, 0);
            Vector3D vector2 = new Vector3D(1, Math.Sqrt(3), 0);
            Vector3DApproxComparer vectorComparer = new Vector3DApproxComparer(approxComp);
            Assert.IsTrue(vectorComparer.Equals(new Vector3D(0, 0, 4 * Math.Sin(Math.PI / 6)),
                                                Vector3DUtils.VectorProduct(vector1, vector2)));
        }

        [Test]
        public void MixedProduct()
        {
            Vector3D vector1 = new Vector3D(Math.Sqrt(3), 1, 0);
            Vector3D vector2 = new Vector3D(1, Math.Sqrt(3), 0);
            Vector3D vector3 = new Vector3D(1, 1, 0);
            Vector3D vector4 = new Vector3D(1, 0, 5);
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.MixedProduct(vector3, vector1, vector2), 0));
            Assert.IsTrue(approxComp.EQ(Vector3DUtils.MixedProduct(vector4, vector1, vector2),
                                        20 * Math.Sin(Math.PI / 6)));
        }

        private readonly ApproxComp approxComp = new ApproxComp(1e-9);
    }
}