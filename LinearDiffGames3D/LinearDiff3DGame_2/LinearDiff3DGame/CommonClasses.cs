using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// ����� ApproxCompClass ����������� ��� ���������������� ��������� ���� �������������� �����
    /// </summary>
    public class ApproxCompClass
    {
        /// <summary>
        /// Epsilon - ��������, ������������ �������� ���������������� ���������
        /// </summary>
        public readonly Double Epsilon;

        /// <summary>
        /// ����������� ������ ApproxCompClass; � ��� (� ������ � ���) �������� �������� Epsilon
        /// </summary>
        /// <param name="Epsilon">��������, ������������ �������� ���������������� ���������</param>
        public ApproxCompClass(Double Epsilon)
        {
            this.Epsilon = Epsilon;
        }

        /// <summary>
        /// ����� GreaterThan ���������� true, ���� ����� Number1 ������, ��� ����� Number2
        /// � ������ ����, ��� �������� ��������� ���������������, Number1 ������, ��� Number2, ���� (Number1-Number2) > Epsilon
        /// </summary>
        /// <param name="Number1">�������� �������������� �����</param>
        /// <param name="Number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� Number1 > Number2, ����� false</returns>
        public Boolean GreaterThan(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta > Epsilon);
        }

        /// <summary>
        /// ����� GreaterOrEqual ���������� true, ���� ����� Number1 ������ ��� ����� ����� Number2
        /// � ������ ����, ��� �������� ��������� ���������������, Number1 ������ ��� ����� Number2, ���� (Number1-Number2) >= -Epsilon
        /// </summary>
        /// <param name="Number1">�������� �������������� �����</param>
        /// <param name="Number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� Number1 >= Number2, ����� false</returns>
        public Boolean GreaterOrEqual(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta >= -Epsilon);
        }

        /// <summary>
        /// ����� Equal ���������� true, ���� ����� Number1 ����� ����� Number2
        /// � ������ ����, ��� �������� ��������� ���������������, Number1 ����� Number2, ���� �������� (Number1-Number2) ����������� [-Epsilon, Epsilon]
        /// </summary>
        /// <param name="Number1">�������� �������������� �����</param>
        /// <param name="Number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� Number1 = Number2, ����� false</returns>
        public Boolean Equal(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return ((Delta >= -Epsilon) && (Delta <= Epsilon));
        }

        /// <summary>
        /// ����� LessOrEqual ���������� true, ���� ����� Number1 ������ ��� ����� ����� Number2
        /// � ������ ����, ��� �������� ��������� ���������������, Number1 ������ ��� ����� Number2, ���� Epsilon >= (Number1-Number2)
        /// </summary>
        /// <param name="Number1">�������� �������������� �����</param>
        /// <param name="Number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, (Number2 >= Number1, ����� false</returns>
        public Boolean LessOrEqual(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta <= Epsilon);
        }

        /// <summary>
        /// ����� LessThan ���������� true, ���� ����� Number1 ������, ��� ����� Number2
        /// � ������ ����, ��� �������� ��������� ���������������, Number1 ������, ��� Number2, ���� -Epsilon > (Number1-Number2)
        /// </summary>
        /// <param name="Number1">�������� �������������� �����</param>
        /// <param name="Number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� Number2 > Number1, ����� false</returns>
        public Boolean LessThan(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta < -Epsilon);
        }
    }

    /// <summary>
    /// ��������� Point3D ������������ 3D �����
    /// </summary>
    public struct Point3D
    {
        /// <summary>
        /// m_XCoord - ���������� X �����
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - ���������� Y �����
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - ���������� Z �����
        /// </summary>
        private Double m_ZCoord;

        /// <summary>
        /// ����������� ��������� Point3D
        /// </summary>
        /// <param name="XCoord">X ���������� 3D �����</param>
        /// <param name="YCoord">Y ���������� 3D �����</param>
        /// <param name="ZCoord">Z ���������� 3D �����</param>
        public Point3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// XCoord - �������� ��� ������� � ���������� X �����
        /// </summary>
        public Double XCoord
        {
            get
            {
                return m_XCoord;
            }
            set
            {
                m_XCoord = value;
            }
        }
        /// <summary>
        /// YCoord - �������� ��� ������� � ���������� Y �����
        /// </summary>
        public Double YCoord
        {
            get
            {
                return m_YCoord;
            }
            set
            {
                m_YCoord = value;
            }
        }
        /// <summary>
        /// ZCoord - �������� ��� ������� � ���������� Z �����
        /// </summary>
        public Double ZCoord
        {
            get
            {
                return m_ZCoord;
            }
            set
            {
                m_ZCoord = value;
            }
        }
    }

    /// <summary>
    /// ��������� Vector3D ������������ 3D ������
    /// </summary>
    public struct Vector3D
    {
        /// <summary>
        /// m_XCoord - ���������� X �������
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - ���������� Y �������
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - ���������� Z �������
        /// </summary>
        private Double m_ZCoord;
        /// <summary>
        /// AcosDigits - ����� �������� ������ ����� ������� (������ �����������)
        /// </summary>
        #warning ��������� AcosDigits ������ ���� ���� ���-�� ������� � ������� ApproxCompClass, ���� ��������� �� ����� � �������� �������
        private const Int32 CosDigits = 9;

        /// <summary>
        /// ����������� ��������� Vector3D
        /// </summary>
        /// <param name="XCoord">X ���������� 3D �������</param>
        /// <param name="YCoord">Y ���������� 3D �������</param>
        /// <param name="ZCoord">Z ���������� 3D �������</param>
        public Vector3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// ����� Normalize ��������� ������� ������
        /// </summary>
        public void Normalize()
        {
            Double VectorLength = Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);

            this.m_XCoord /= VectorLength;
            this.m_YCoord /= VectorLength;
            this.m_ZCoord /= VectorLength;
        }

        /// <summary>
        /// ����� GetParallelComponent ���������� ���������� �������� �������, ������������ ������� DirectingVector
        /// </summary>
        /// <param name="DirectingVector">������������ ������</param>
        /// <returns>���������� (������) �������� �������, ������������ ������� DirectingVector</returns>
        public Vector3D GetParallelComponent(Vector3D DirectingVector)
        {
            if (DirectingVector.Length != 1) DirectingVector.Normalize();

            Double ScalarProductValue = Vector3D.ScalarProduct(this, DirectingVector);

            Double ParallelCompX = ScalarProductValue * DirectingVector.XCoord;
            Double ParallelCompY = ScalarProductValue * DirectingVector.YCoord;
            Double ParallelCompZ = ScalarProductValue * DirectingVector.ZCoord;

            return new Vector3D(ParallelCompX, ParallelCompY, ParallelCompZ);
        }

        /// <summary>
        /// ����� GetPerpendicularComponent ���������� ���������� �������� �������, ���������������� ������� DirectingVector
        /// </summary>
        /// <param name="DirectingVector">������������ ������</param>
        /// <returns>���������� �������� �������, ���������������� ������� DirectingVector</returns>
        public Vector3D GetPerpendicularComponent(Vector3D DirectingVector)
        {
            if (DirectingVector.Length != 1) DirectingVector.Normalize();

            Double ScalarProductValue = Vector3D.ScalarProduct(this, DirectingVector);

            Double PerpendicularCompX = this.XCoord - ScalarProductValue * DirectingVector.XCoord;
            Double PerpendicularCompY = this.YCoord - ScalarProductValue * DirectingVector.YCoord;
            Double PerpendicularCompZ = this.ZCoord - ScalarProductValue * DirectingVector.ZCoord;

            return new Vector3D(PerpendicularCompX, PerpendicularCompY, PerpendicularCompZ);
        }

        /// <summary>
        /// XCoord - �������� ��� ������� � ���������� X �������
        /// </summary>
        public Double XCoord
        {
            get
            {
                return m_XCoord;
            }
            set
            {
                m_XCoord = value;
            }
        }
        /// <summary>
        /// YCoord - �������� ��� ������� � ���������� Y �������
        /// </summary>
        public Double YCoord
        {
            get
            {
                return m_YCoord;
            }
            set
            {
                m_YCoord = value;
            }
        }
        /// <summary>
        /// ZCoord - �������� ��� ������� � ���������� Z �������
        /// </summary>
        public Double ZCoord
        {
            get
            {
                return m_ZCoord;
            }
            set
            {
                m_ZCoord = value;
            }
        }

        /// <summary>
        /// Length - ����� �������
        /// </summary>
        public Double Length
        {
            get
            {
                return Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);
            }
        }

        /// <summary>
        /// ����� VectorAddition ���������� ��������� �������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D VectorAddition(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.XCoord + b.XCoord, a.YCoord + b.YCoord, a.ZCoord + b.ZCoord);
        }

        /// <summary>
        /// ����� VectorSubtraction ���������� ��������� �������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D VectorSubtraction(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.XCoord - b.XCoord, a.YCoord - b.YCoord, a.ZCoord - b.ZCoord);
        }

        /// <summary>
        /// ����� VectorMultiplication ���������� ��������� ��������� ������� a �� ����� DoubleNumber
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="DoubleNumber">����� DoubleNumber</param>
        /// <returns>��������� ��������� ������� a �� ����� DoubleNumber</returns>
        public static Vector3D VectorMultiplication(Vector3D a, Double DoubleNumber)
        {
            return new Vector3D(DoubleNumber * a.XCoord, DoubleNumber * a.YCoord, DoubleNumber * a.ZCoord);
        }

        /// <summary>
        /// ����� ScalarProduct ���������� ��������� ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� ���������� ������������ �������� a � b</returns>
        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.XCoord * b.XCoord + a.YCoord * b.YCoord + a.ZCoord * b.ZCoord;
        }

        /// <summary>
        /// ����� VectorProduct ���������� ��������� (������) ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� (������) ���������� ������������ �������� a � b</returns>
        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double XCoord = a.YCoord * b.ZCoord - a.ZCoord * b.YCoord;
            Double YCoord = a.ZCoord * b.XCoord - a.XCoord * b.ZCoord;
            Double ZCoord = a.XCoord * b.YCoord - a.YCoord * b.XCoord;

            return new Vector3D(XCoord, YCoord, ZCoord);
        }

        /// <summary>
        /// ����� MixedProduct ���������� ��������� ���������� ������������ �������� a, b � c
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <param name="c">������ c</param>
        /// <returns>��������� ���������� ������������ �������� a, b � c</returns>
        public static Double MixedProduct(Vector3D a, Vector3D b, Vector3D c)
        {
            return Vector3D.ScalarProduct(a, Vector3D.VectorProduct(b, c));
        }

        /// <summary>
        /// ����� AngleBetweenVectors ���������� �������� ���� (� ��������) ����� ��������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>���� (� ��������) ����� ��������� a � b</returns>
        public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        {
            Double CosValue = Vector3D.ScalarProduct(a, b) / (a.Length * b.Length);
            // ���������� ����� ������, ��� ��-�� ������ ���������� �������� �������� ���� ����� ����� > 1
            return Math.Acos(Math.Round(CosValue, CosDigits));
        }

        /// <summary>
        /// �������� ZeroVector3D - ������� ������
        /// </summary>
        public static Vector3D ZeroVector3D
        {
            get
            {
                return new Vector3D(0, 0, 0);
            }
        }

        /// <summary>
        /// �������� �������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return Vector3D.VectorAddition(a, b);
        }

        /// <summary>
        /// �������� ��������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return Vector3D.VectorSubtraction(a, b);
        }

        /// <summary>
        /// �������� ��������� ������� a �� ����� DoubleNumber
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="DoubleNumber">����� DoubleNumber</param>
        /// <returns>��������� ��������� ������� a �� ����� DoubleNumber</returns>
        public static Vector3D operator *(Double DoubleNumber, Vector3D a)
        {
            return Vector3D.VectorMultiplication(a, DoubleNumber);
        }

        /// <summary>
        /// �������� ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� ���������� ������������ �������� a � b</returns>
        public static Double operator *(Vector3D a, Vector3D b)
        {
            return Vector3D.ScalarProduct(a, b);
        }
    }

    /// <summary>
    /// ����� Matrix ������������ ��� �������� ������ NxM (��������� ������) � ����������� � ����
    /// </summary>
    public class Matrix : ICloneable
    {
        /// <summary>
        /// m_MatrixElements - ������ ��� �������� ��������� �������
        /// </summary>
        Double[,] m_MatrixElements = null;
        /// <summary>
        /// m_RowCount - ���������� �����
        /// </summary>
        Int32 m_RowCount = 0;
        /// <summary>
        /// m_ColumnCount - ���������� ��������
        /// </summary>
        Int32 m_ColumnCount = 0;

        /// <summary>
        /// ����������� ������ Matrix
        /// </summary>
        /// <param name="RowCount">���������� �����</param>
        /// <param name="ColumnCount">���������� ��������</param>
        public Matrix(Int32 RowCount, Int32 ColumnCount)
        {
            m_MatrixElements = new Double[RowCount, ColumnCount];
            m_RowCount = RowCount;
            m_ColumnCount = ColumnCount;
        }

        /// <summary>
        /// �������� (����������) ��� ������� � ��������� �������
        /// ����� ������ ���������� � ��������� 1...RowCount
        /// ����� ������� ���������� � ��������� 1...ColumnCount
        /// </summary>
        /// <param name="RowIndex">����� (������) ������</param>
        /// <param name="ColumnIndex">����� (������) �������</param>
        /// <returns>������� �������</returns>
        public Double this[Int32 RowIndex, Int32 ColumnIndex]
        {
            get
            {
                if ((RowIndex < 1) || (RowIndex > m_RowCount))
                {
                    throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count");
                }
                if ((ColumnIndex < 1) || (ColumnIndex > m_ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("Column index must be between 1 and column's count");
                }

                return m_MatrixElements[(RowIndex - 1), (ColumnIndex - 1)];
            }
            set
            {
                if ((RowIndex < 1) || (RowIndex > m_RowCount))
                {
                    throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count");
                }
                if ((ColumnIndex < 1) || (ColumnIndex > m_ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("Column index must be between 1 and column's count");
                }

                m_MatrixElements[(RowIndex - 1), (ColumnIndex - 1)] = value;
            }
        }

        /// <summary>
        /// ������� RowCount ���������� ���������� �����
        /// </summary>
        public Int32 RowCount
        {
            get
            {
                return m_RowCount;
            }
        }

        /// <summary>
        /// ������� RowCount ���������� ���������� ��������
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                return m_ColumnCount;
            }
        }

        /// <summary>
        /// ����� GetMatrixRow ���������� ������ (� ���� �������) �������
        /// </summary>
        /// <param name="RowIndex">����� (������) ������������ ������</param>
        /// <returns>������������ ������ (� ���� �������)</returns>
        public Matrix GetMatrixRow(Int32 RowIndex)
        {
            if ((RowIndex < 1) || (RowIndex > m_RowCount))
            {
                throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count");
            }

            Matrix RowMatrix = new Matrix(1, m_ColumnCount);

            for (Int32 ColumnIndex = 1; ColumnIndex <= m_ColumnCount; ColumnIndex++)
            {
                RowMatrix[1, ColumnIndex] = this[RowIndex, ColumnIndex];
            }

            return RowMatrix;
        }

        /// <summary>
        /// ����� SetMatrixRow ��������� ������ (�������� � ���� �������) � ������ �������� ������� � ������� (��������) RowIndex
        /// </summary>
        /// <param name="RowIndex">����� (������) ������ �������� �������</param>
        /// <param name="RowMatrix">������ (�������� � ���� �������), ������� ���� ��������� � �������� �������</param>
        public void SetMatrixRow(Int32 RowIndex, Matrix RowMatrix)
        {
            if ((RowIndex < 1) || (RowIndex > m_RowCount))
            {
                throw new ArgumentOutOfRangeException("Row index must be between 1 and row's count");
            }
            if (RowMatrix.RowCount > 1)
            {
                throw new ArgumentOutOfRangeException("RowMatrix isn't the row");
            }
            if (m_ColumnCount != RowMatrix.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of RowMatrix should be equaled to column's count of this matrix");
            }

            for (Int32 ColumnIndex = 1; ColumnIndex <= m_ColumnCount; ColumnIndex++)
            {
                this[RowIndex, ColumnIndex] = RowMatrix[1, ColumnIndex];
            }
        }

        /// <summary>
        /// ����� MatrixAddition ���������� ��������� �������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix MatrixAddition(Matrix Matrix1, Matrix Matrix2)
        {
            if (Matrix1.RowCount != Matrix2.RowCount)
            {
                throw new ArgumentOutOfRangeException("Row's count of the first matrix should be equaled to row's count of the second matrix");
            }
            if (Matrix1.ColumnCount != Matrix2.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to column's count of the second matrix");
            }

            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix1.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[RowIndex, ColumnIndex] + Matrix2[RowIndex, ColumnIndex];
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixSubtraction ���������� ��������� �������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix MatrixSubtraction(Matrix Matrix1, Matrix Matrix2)
        {
            if (Matrix1.RowCount != Matrix2.RowCount)
            {
                throw new ArgumentOutOfRangeException("Row's count of the first matrix should be equaled to row's count of the second matrix");
            }
            if (Matrix1.ColumnCount != Matrix2.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to column's count of the second matrix");
            }

            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix1.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[RowIndex, ColumnIndex] - Matrix2[RowIndex, ColumnIndex];
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixMultiplication ���������� ��������� ��������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� ��������� ������ Matrix1 � Matrix2</returns>
        public static Matrix MatrixMultiplication(Matrix Matrix1, Matrix Matrix2)
        {
            if (Matrix1.ColumnCount != Matrix2.RowCount)
            {
                throw new ArgumentOutOfRangeException("Column's count of the first matrix should be equaled to row's count of the second matrix");
            }

            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix2.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = 0;
                    for (Int32 InternalIndex = 1; InternalIndex <= Matrix1.ColumnCount; InternalIndex++)
                    {
                        ResultMatrix[RowIndex, ColumnIndex] += Matrix1[RowIndex, InternalIndex] * Matrix2[InternalIndex, ColumnIndex];
                    }
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixMultiplication ���������� ��������� ��������� ������� Matrix1 �� ����� DoubleNumber
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="DoubleNumber">����� DoubleNumber</param>
        /// <returns>��������� ��������� ������� Matrix1 �� ����� DoubleNumber</returns>
        public static Matrix MatrixMultiplication(Matrix Matrix1, Double DoubleNumber)
        {
            Matrix ResultMatrix = new Matrix(Matrix1.RowCount, Matrix1.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[RowIndex, ColumnIndex] * DoubleNumber;
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// ����� MatrixTransposing ���������� ��������� ���������������� ������� Matrix1
        /// </summary>
        /// <param name="Matrix1">�������� ������� Matrix1</param>
        /// <returns>��������� ���������������� ������� Matrix1</returns>
        public static Matrix MatrixTransposing(Matrix Matrix1)
        {
            Matrix ResultMatrix = new Matrix(Matrix1.ColumnCount, Matrix1.RowCount);

            for (Int32 RowIndex = 1; RowIndex <= ResultMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ResultMatrix.ColumnCount; ColumnIndex++)
                {
                    ResultMatrix[RowIndex, ColumnIndex] = Matrix1[ColumnIndex, RowIndex];
                }
            }

            return ResultMatrix;
        }

        /// <summary>
        /// �������� �������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix operator +(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixAddition(Matrix1, Matrix2);
        }

        /// <summary>
        /// �������� ��������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� �������� ������ Matrix1 � Matrix2</returns>
        public static Matrix operator -(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixSubtraction(Matrix1, Matrix2);
        }

        /// <summary>
        /// �������� ��������� ������ Matrix1 � Matrix2
        /// </summary>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <param name="Matrix2">������� Matrix2</param>
        /// <returns>��������� ��������� ������ Matrix1 � Matrix2</returns>
        public static Matrix operator *(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixMultiplication(Matrix1, Matrix2);
        }

        /// <summary>
        /// �������� ��������� ������� Matrix1 �� ����� DoubleNumber
        /// </summary>
        /// <param name="DoubleNumber">����� DoubleNumber</param>
        /// <param name="Matrix1">������� Matrix1</param>
        /// <returns>��������� ��������� ������� Matrix1 �� ����� DoubleNumber</returns>
        public static Matrix operator *(Double DoubleNumber, Matrix Matrix1)
        {
            return Matrix.MatrixMultiplication(Matrix1, DoubleNumber);
        }

        /// <summary>
        /// ����� Clone ���������� ������ ����� (deep copy) ������ �������
        /// </summary>
        /// <returns>������ ����� (deep copy) ������ �������</returns>
        public Matrix Clone()
        {
            Matrix CloneMatrix = new Matrix(m_RowCount, m_ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= m_RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= m_ColumnCount; ColumnIndex++)
                {
                    CloneMatrix[RowIndex, ColumnIndex] = this[ColumnIndex, RowIndex];
                }
            }

            return CloneMatrix;
        }

        /// <summary>
        /// ����� Clone ���������� ������ ����� (deep copy) ������ ������� (����� ���������� ������ Clone ���������� ICloneable)
        /// </summary>
        /// <returns>������ ����� (deep copy) ������ �������</returns>
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    /// <summary>
    /// ����� FundKoshiMatrix ������������� ���������� ��������������� ������� ����
    /// (� ������ �������, ������������ �� N ����� ������� ���� - ��. ��������� ������� �������� ����. ���)
    /// </summary>
    public class FundKoshiMatrix
    {
        /// <summary>
        /// DeltaT - ��� �� T ������� ����������������� ���������
        /// </summary>
        #warning �������� DeltaT ������ ���������� �������
        private const Double m_DeltaT = 0.001;
        /// <summary>
        /// ������� A, �� ������� ����������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
        /// </summary>
        private Matrix m_MatrixA = null;
        /// <summary>
        /// ����� ������� �����, ������������ ��� � ��������������� ������� ����
        /// (������ ������ ����������� ����������, ������� ��� ������ ��������� ��������������� ������� ���� �������)
        /// </summary>
        private Int32[] m_RowIndexes = null;
        /// <summary>
        /// �������� ����� (�.�. Theta - T) ���������� ���������� ��������������� ������� ����
        /// </summary>
        private Double m_LastInverseTime = Double.NaN;
        /// <summary>
        /// ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����), ����������� ��� ������� m_LastT
        /// </summary>
        private Matrix m_LastFundKoshiMatrix = null;

        /// <summary>
        /// ����� GetZeroFundKoshiMatrix ���������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0 (T - �������� �����)
        /// </summary>
        /// <returns>��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����) ��� T = 0 (T - �������� �����)</returns>
        private Matrix GetZeroTimeFundKoshiMatrix()
        {
            Matrix ZeroTimeFundKoshiMatrix = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            for (Int32 RowIndex = 1; RowIndex <= ZeroTimeFundKoshiMatrix.RowCount; RowIndex++)
            {
                for (Int32 ColumnIndex = 1; ColumnIndex <= ZeroTimeFundKoshiMatrix.ColumnCount; ColumnIndex++)
                {
                    ZeroTimeFundKoshiMatrix[RowIndex, ColumnIndex] = (ColumnIndex == m_RowIndexes[RowIndex - 1] ? 1 : 0);
                }
            }

            return ZeroTimeFundKoshiMatrix;
        }

        /// <summary>
        /// ����� DeltaRowCalc ��������� ��������� ������� ��� ���� �� ������� m_DeltaT (�� ������ �����-�����)
        /// </summary>
        /// <param name="PreviousRow">���������� ������ (�� �������)</param>
        /// <returns>��������� ������� ��� ���� �� ������� m_DeltaT </returns>
        private Matrix DeltaRowCalc(Matrix PreviousRow)
        {
            Matrix nu1 = PreviousRow * m_MatrixA;
            Matrix nu2 = (PreviousRow + (m_DeltaT / 2) * nu1) * m_MatrixA;
            Matrix nu3 = (PreviousRow + (m_DeltaT / 2) * nu2) * m_MatrixA;
            Matrix nu4 = (PreviousRow + m_DeltaT * nu3) * m_MatrixA;
            Matrix DeltaRow = (m_DeltaT / 6) * (nu1 + 2 * nu2 + 2 * nu3 + nu4);

            return DeltaRow;
        }

        /// <summary>
        /// ����������� ������ FundKoshiMatrix
        /// </summary>
        /// <param name="MatrixA">������� A, �� ������� ����������� ��������������� ������� ����</param>
        /// <param name="RowIndexes">����� ������� �����, ������������ ��� � ��������������� ������� ����</param>
        public FundKoshiMatrix(Matrix MatrixA, Int32[] RowIndexes)
        {
            // ?? may be ������������ �� �����
            //m_MatrixA = (MatrixA.Clone() as Matrix);
            #warning �������� ���������� ������� � m_MatrixA ������ �� ���� ������� MatrixA
            m_MatrixA = MatrixA;
            m_RowIndexes = RowIndexes;

            // ������ ����� � ������ RowIndexes ������ ���� � ������� �����������
            for (Int32 Index = 1; Index < RowIndexes.Length; Index++)
            {
                if (RowIndexes[Index - 1] >= RowIndexes[Index])
                {
                    #warning ������ �� ������ ����� � ������ RowIndexes ���� � ������� ����������� ???
                }
            }
        }

        /// <summary>
        /// ����� FundKoshiMatrixCalc ��������� ��������������� ������� ���� (� ������ �������, ������������ �� N ����� ������� ����)
        /// � ������ ��������� ������� InverseTime
        /// </summary>
        /// <param name="InverseTime">������ ��������� �������, ��� ������� ������������ ����������</param>
        /// <returns>�������, ������������ �� N ����� ��������������� ������� ���� � ������ ��������� ������� InverseTime</returns>
        public Matrix FundKoshiMatrixCalc(Double InverseTime)
        {
            if (m_LastInverseTime == InverseTime) return m_LastFundKoshiMatrix;

            Matrix FundKoshiMatrixValue = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            // ���������� ���������� ������� ��������� ������� � ���������� �������� ����������� �������
            // ���� ������ ������� InverseTime, ��� �������� ��� ���� ��������� ������� ������, ��� �������� m_LastInverseTime,
            // �� ��������� �������� ��������� ������� ����� �������� m_LastInverseTime, � ��������� ��������� ����������� ������� - m_LastFundKoshiMatrix (�������� �������, ����������� ��� ������� ��������� m_LastInverseTime)
            // ����� ��������� �������� ��������� ������� ����� 0, ��������� ��������� ����������� ������� - ������� ������������ �� N ����� �� ���������
            Matrix InitialFundKoshiMatrix = (m_LastInverseTime < InverseTime ? m_LastFundKoshiMatrix : GetZeroTimeFundKoshiMatrix());
            Double CurrentInverseTime = (m_LastInverseTime < InverseTime ? m_LastInverseTime : 0);

            // �������������� ��������� ������� � ������ �����
            Matrix[] FundKoshiMatrixValueRows = new Matrix[FundKoshiMatrixValue.RowCount];
            for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
            {
                FundKoshiMatrixValueRows[RowIndex] = InitialFundKoshiMatrix.GetMatrixRow(RowIndex + 1);
            }

            // ���������� ��������� �������� �������, ������������ �� N ����� ��������������� ������� ����
            while (CurrentInverseTime < InverseTime)
            {
                for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
                {
                    FundKoshiMatrixValueRows[RowIndex] += DeltaRowCalc(FundKoshiMatrixValueRows[RowIndex]);
                }

                CurrentInverseTime += m_DeltaT;
            }

            // �������������� ������� ����� � �������� �������
            for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
            {
                FundKoshiMatrixValue.SetMatrixRow(RowIndex + 1, FundKoshiMatrixValueRows[RowIndex]);
            }

            m_LastInverseTime = InverseTime;
            m_LastFundKoshiMatrix = FundKoshiMatrixValue;

            return FundKoshiMatrixValue;
        }
    }

    /// <summary>
    /// ����� CyclicList (������� generic-������ List) ������������ ����������� ������ ��������� ���� T
    /// </summary>
    /// <typeparam name="T">��� ���������, ���������� � ����������� ������</typeparam>
    public class CyclicList<T> : List<T>
    {
        /// <summary>
        /// ����� NextItemIndex ���������� ������ ���������� �������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItemIndex">������ �������� ��������</param>
        /// <returns>������ ���������� �������� � ����������� ������</returns>
        public Int32 NextItemIndex(Int32 CurrentItemIndex)
        {
            if (CurrentItemIndex == -1)
            {
                #warning �� ���� ������ ���� ���������� ������� ������ ArgumentException
                throw new ArgumentException("Finding item does not belong this list");
            }

            return (CurrentItemIndex == this.Count - 1 ? 0 : CurrentItemIndex + 1);
        }

        /// <summary>
        /// ����� NextItemIndex ���������� ������ ���������� �������� � ����������� ������ �� ��������� �������� ��������
        /// ���� ������� ������� �� �������� � ������, �� ����� NextItemIndex ����� ���������� �������� ������
        /// � ���� ������ ����� ������������ �������, ������� ���������� ������ ���������� �������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItem">������� �������</param>
        /// <returns>������ ���������� �������� � ����������� ������</returns>
        public Int32 NextItemIndex(T CurrentItem)
        {
            Int32 CurrentItemIndex = this.IndexOf(CurrentItem);

            return NextItemIndex(CurrentItemIndex);
        }

        /// <summary>
        /// ����� NextItem ���������� ��������� ������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItemIndex">������ �������� ��������</param>
        /// <returns>��������� ������� � ����������� ������</returns>
        public T NextItem(Int32 CurrentItemIndex)
        {
            return this[NextItemIndex(CurrentItemIndex)];
        }

        /// <summary>
        /// ����� NextItem ���������� ��������� ������� � ����������� ������ �� ��������� �������� ��������
        /// ���� ������� ������� �� �������� � ������, �� ����� NextItem ����� ���������� �������� �������
        /// � ���� ������ ����� ������������ �������, ������� ���������� ��������� ������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItem">������� �������</param>
        /// <returns>��������� ������� � ����������� ������</returns>
        public T NextItem(T CurrentItem)
        {
            return this[NextItemIndex(CurrentItem)];
        }

        /// <summary>
        /// ����� PrevItemIndex ���������� ������ ����������� �������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItemIndex">������ �������� ��������</param>
        /// <returns>������ ����������� �������� � ����������� ������</returns>
        public Int32 PrevItemIndex(Int32 CurrentItemIndex)
        {
            if (CurrentItemIndex == -1)
            {
                #warning �� ���� ������ ���� ���������� ������� ������ ArgumentException
                throw new ArgumentException("Finding item does not belong this list");
            }

            return (CurrentItemIndex == 0 ? this.Count - 1 : CurrentItemIndex - 1);
        }

        /// <summary>
        /// ����� PrevItemIndex ���������� ������ ����������� �������� � ����������� ������ �� ��������� �������� ��������
        /// ���� ������� ������� �� �������� � ������, �� ����� PrevItemIndex ����� ���������� �������� ������
        /// � ���� ������ ����� ������������ �������, ������� ���������� ������ ����������� �������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItem">������� �������</param>
        /// <returns>������ ����������� �������� � ����������� ������</returns>
        public Int32 PrevItemIndex(T CurrentItem)
        {
            Int32 CurrentItemIndex = this.IndexOf(CurrentItem);

            return PrevItemIndex(CurrentItemIndex);
        }

        /// <summary>
        /// ����� PrevItem ���������� ���������� ������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItemIndex">������ �������� ��������</param>
        /// <returns>���������� ������� � ����������� ������</returns>
        public T PrevItem(Int32 CurrentItemIndex)
        {
            return this[PrevItemIndex(CurrentItemIndex)];
        }

        /// <summary>
        /// ����� PrevItem ���������� ���������� ������� � ����������� ������ �� ��������� �������� ��������
        /// ���� ������� ������� �� �������� � ������, �� ����� PrevItem ����� ���������� �������� �������
        /// � ���� ������ ����� ������������ �������, ������� ���������� ���������� ������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="CurrentItem">������� �������</param>
        /// <returns>���������� ������� � ����������� ������</returns>
        public T PrevItem(T CurrentItem)
        {
            return this[PrevItemIndex(CurrentItem)];
        }
    }
}
