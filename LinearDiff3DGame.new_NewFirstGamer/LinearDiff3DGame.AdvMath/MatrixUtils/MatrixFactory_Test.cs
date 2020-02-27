using System;
using NUnit.Framework;

namespace LinearDiff3DGame.AdvMath.MatrixUtils
{
    [TestFixture]
    public class MatrixFactory_Test
    {
        [Test]
        public void CreateFromRawData()
        {
            Double[] rawData = new Double[] {9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1, 2};
            Matrix matrix = new MatrixFactory().CreateFromRawData(4, 3, rawData);
            Assert.AreEqual(4, matrix.RowCount);
            Assert.AreEqual(3, matrix.ColumnCount);
            for(Int32 rowIndex = 1; rowIndex <= matrix.RowCount; ++rowIndex)
            {
                for(Int32 columnIndex = 1; columnIndex <= matrix.ColumnCount; ++columnIndex)
                {
                    Assert.AreEqual(rawData[(rowIndex - 1) * matrix.ColumnCount + (columnIndex - 1)],
                                    matrix[rowIndex, columnIndex]);
                }
            }
        }
    }
}