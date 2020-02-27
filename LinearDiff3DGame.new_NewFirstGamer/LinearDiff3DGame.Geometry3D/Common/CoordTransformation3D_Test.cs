using System;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.Common
{
    [TestFixture]
    public class CoordTransformation3D_Test
    {
        public CoordTransformation3D_Test()
        {
            approxComp = new ApproxComp(epsilon);
        }

        [Test]
        public void RxMatrixWithZeroAngle()
        {
            Matrix matrix = CoordTransformation3D.RxMatrix(0);
            Assert.IsTrue(IsMatrixEqual(matrix, Matrix.IdentityMatrix(4)));
        }

        [Test]
        public void RxMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 0;
            sourceVector[2, 1] = 0;
            sourceVector[3, 1] = 1;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 1;
            sourceVector2[2, 1] = 0;
            sourceVector2[3, 1] = 0;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.RxMatrix(45*Math.PI/180);
            Matrix transform = matrix*sourceVector;
            Assert.IsTrue(approxComp.EQ(0, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(-Math.Sqrt(2)/2, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(Math.Sqrt(2)/2, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(1, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        [Test]
        public void RyMatrixWithZeroAngle()
        {
            Matrix matrix = CoordTransformation3D.RyMatrix(0);
            Assert.IsTrue(IsMatrixEqual(matrix, Matrix.IdentityMatrix(4)));
        }

        [Test]
        public void RyMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 0;
            sourceVector[2, 1] = 0;
            sourceVector[3, 1] = 1;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 0;
            sourceVector2[2, 1] = 1;
            sourceVector2[3, 1] = 0;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.RyMatrix(45 * Math.PI / 180);
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(Math.Sqrt(2) / 2, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(Math.Sqrt(2) / 2, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(0, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        [Test]
        public void RzMatrixWithZeroAngle()
        {
            Matrix matrix = CoordTransformation3D.RzMatrix(0);
            Assert.IsTrue(IsMatrixEqual(matrix, Matrix.IdentityMatrix(4)));
        }

        [Test]
        public void RzMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 1;
            sourceVector[2, 1] = 0;
            sourceVector[3, 1] = 0;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 0;
            sourceVector2[2, 1] = 0;
            sourceVector2[3, 1] = 1;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.RzMatrix(45 * Math.PI / 180);
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(Math.Sqrt(2) / 2, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(Math.Sqrt(2) / 2, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(0, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        [Test]
        public void DMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 1;
            sourceVector[2, 1] = 1;
            sourceVector[3, 1] = 1;
            sourceVector[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.DMatrix(2, 3, 4);
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(2, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(3, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(4, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
        }

        [Test]
        public void Mx0yMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 1;
            sourceVector[2, 1] = 1;
            sourceVector[3, 1] = 0;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 0;
            sourceVector2[2, 1] = 0;
            sourceVector2[3, 1] = 1;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.Mx0yMatrix();
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(1, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(0, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(-1, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        [Test]
        public void My0zMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 0;
            sourceVector[2, 1] = 1;
            sourceVector[3, 1] = 1;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 1;
            sourceVector2[2, 1] = 0;
            sourceVector2[3, 1] = 0;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.My0zMatrix();
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(0, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(-1, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        [Test]
        public void Mz0xMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 1;
            sourceVector[2, 1] = 0;
            sourceVector[3, 1] = 1;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 0;
            sourceVector2[2, 1] = 1;
            sourceVector2[3, 1] = 0;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.Mz0xMatrix();
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(1, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(0, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(-1, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        [Test]
        public void TMatrix()
        {
            Matrix sourceVector = new Matrix(4, 1);
            sourceVector[1, 1] = 1;
            sourceVector[2, 1] = 0;
            sourceVector[3, 1] = 0;
            sourceVector[4, 1] = 1;
            Matrix sourceVector2 = new Matrix(4, 1);
            sourceVector2[1, 1] = 0;
            sourceVector2[2, 1] = 0;
            sourceVector2[3, 1] = 1;
            sourceVector2[4, 1] = 1;
            Matrix matrix = CoordTransformation3D.TMatrix(new Vector3D(1, 0, 0));
            Matrix transform = matrix * sourceVector;
            Assert.IsTrue(approxComp.EQ(2, transform[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform[2, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform[4, 1]));
            Matrix transform2 = matrix * sourceVector2;
            Assert.IsTrue(approxComp.EQ(1, transform2[1, 1]));
            Assert.IsTrue(approxComp.EQ(0, transform2[2, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[3, 1]));
            Assert.IsTrue(approxComp.EQ(1, transform2[4, 1]));
        }

        private Boolean IsMatrixEqual(Matrix m1, Matrix m2)
        {
            if (m1.RowCount != m2.RowCount || m1.ColumnCount != m2.ColumnCount)
                return false;
            for (Int32 rowIndex = 1; rowIndex <= m1.RowCount; ++rowIndex)
                for (Int32 columnIndex = 1; columnIndex <= m1.ColumnCount; ++columnIndex)
                    if (approxComp.NE(m1[rowIndex, columnIndex], m2[rowIndex, columnIndex])) return false;
            return true;
        }

        private readonly ApproxComp approxComp;
        private const Double epsilon = 1e-9;
    }
}