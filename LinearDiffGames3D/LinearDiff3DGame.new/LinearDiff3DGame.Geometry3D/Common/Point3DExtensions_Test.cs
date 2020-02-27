using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.AdvMath.MatrixUtils;
using NUnit.Framework;

namespace LinearDiff3DGame.Geometry3D.Common
{
    [TestFixture]
    public class Point3DExtensions_Test
    {
        [Test]
        public void ToVector3D()
        {
            Assert.AreEqual(new Vector3D(1.0, 2.1, 1.2), new Point3D(1.0, 2.1, 1.2).ToVector3D());
        }

        [Test]
        public void ToMatrixColumn()
        {
            Matrix expectedColumn = new MatrixFactory().CreateFromRawData(3, 1, 1.0, 2.1, 1.2);
            MatrixComparer comparer = new MatrixComparer();
            Assert.IsTrue(comparer.Equals(expectedColumn, new Point3D(1.0, 2.1, 1.2).ToMatrixColumn()));
        }

        [Test]
        public void ToPoint3D()
        {
            Assert.Throws<IncorrectMatrixSizeException>(
                () => new MatrixFactory().CreateFromRawData(2, 2, 1.0, 1.0, 0.0, 0.0).ToPoint3D());
            Matrix column = new MatrixFactory().CreateFromRawData(3, 1, 1.0, 2.1, 1.2);
            Assert.AreEqual(new Point3D(1.0, 2.1, 1.2), column.ToPoint3D());
        }
    }
}