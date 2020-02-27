using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// класс ApproxCompClass применяется для приблизительного сравнения двух действительных чисел
    /// </summary>
    public class ApproxCompClass
    {
        /// <summary>
        /// Epsilon - величина, определяющая точность приблизительного сравнения
        /// </summary>
        public readonly Double Epsilon;

        /// <summary>
        /// конструктор класса ApproxCompClass; в нем (и только в нем) задается величина Epsilon
        /// </summary>
        /// <param name="Epsilon">величина, определяющая точность приблизительного сравнения</param>
        public ApproxCompClass(Double Epsilon)
        {
            this.Epsilon = Epsilon;
        }

        /// <summary>
        /// метод GreaterThan возвращает true, если число Number1 больше, чем число Number2
        /// с учетом того, что операции сравнения приблизительные, Number1 больше, чем Number2, если (Number1-Number2) > Epsilon
        /// </summary>
        /// <param name="Number1">исходное действительное число</param>
        /// <param name="Number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если Number1 > Number2, иначе false</returns>
        public Boolean GreaterThan(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta > Epsilon);
        }

        /// <summary>
        /// метод GreaterOrEqual возвращает true, если число Number1 больше или равно числу Number2
        /// с учетом того, что операции сравнения приблизительные, Number1 больше или равно Number2, если (Number1-Number2) >= -Epsilon
        /// </summary>
        /// <param name="Number1">исходное действительное число</param>
        /// <param name="Number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если Number1 >= Number2, иначе false</returns>
        public Boolean GreaterOrEqual(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta >= -Epsilon);
        }

        /// <summary>
        /// метод Equal возвращает true, если число Number1 равно числу Number2
        /// с учетом того, что операции сравнения приблизительные, Number1 равно Number2, если величина (Number1-Number2) принадлежит [-Epsilon, Epsilon]
        /// </summary>
        /// <param name="Number1">исходное действительное число</param>
        /// <param name="Number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если Number1 = Number2, иначе false</returns>
        public Boolean Equal(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return ((Delta >= -Epsilon) && (Delta <= Epsilon));
        }

        /// <summary>
        /// метод LessOrEqual возвращает true, если число Number1 меньше или равно числу Number2
        /// с учетом того, что операции сравнения приблизительные, Number1 меньше или равно Number2, если Epsilon >= (Number1-Number2)
        /// </summary>
        /// <param name="Number1">исходное действительное число</param>
        /// <param name="Number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, (Number2 >= Number1, иначе false</returns>
        public Boolean LessOrEqual(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta <= Epsilon);
        }

        /// <summary>
        /// метод LessThan возвращает true, если число Number1 меньше, чем число Number2
        /// с учетом того, что операции сравнения приблизительные, Number1 меньше, чем Number2, если -Epsilon > (Number1-Number2)
        /// </summary>
        /// <param name="Number1">исходное действительное число</param>
        /// <param name="Number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если Number2 > Number1, иначе false</returns>
        public Boolean LessThan(Double Number1, Double Number2)
        {
            Double Delta = Number1 - Number2;

            return (Delta < -Epsilon);
        }
    }

    /// <summary>
    /// структура Point3D представляет 3D точку
    /// </summary>
    public struct Point3D
    {
        /// <summary>
        /// m_XCoord - координата X точки
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y точки
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z точки
        /// </summary>
        private Double m_ZCoord;

        /// <summary>
        /// конструктор структуры Point3D
        /// </summary>
        /// <param name="XCoord">X координата 3D точки</param>
        /// <param name="YCoord">Y координата 3D точки</param>
        /// <param name="ZCoord">Z координата 3D точки</param>
        public Point3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// XCoord - свойство для доступа к координате X точки
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
        /// YCoord - свойство для доступа к координате Y точки
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
        /// ZCoord - свойство для доступа к координате Z точки
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
    /// структура Vector3D представляет 3D вектор
    /// </summary>
    public struct Vector3D
    {
        /// <summary>
        /// m_XCoord - координата X вектора
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y вектора
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z вектора
        /// </summary>
        private Double m_ZCoord;
        /// <summary>
        /// AcosDigits - число значащих знаков после запятой (дальше округляется)
        /// </summary>
        #warning константа AcosDigits должна быть либо как-то связана с классом ApproxCompClass, либо получатся из файла с входными данными
        private const Int32 CosDigits = 9;

        /// <summary>
        /// конструктор структуры Vector3D
        /// </summary>
        /// <param name="XCoord">X координата 3D вектора</param>
        /// <param name="YCoord">Y координата 3D вектора</param>
        /// <param name="ZCoord">Z координата 3D вектора</param>
        public Vector3D(Double XCoord, Double YCoord, Double ZCoord)
        {
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// метод Normalize нормирует текущий вектор
        /// </summary>
        public void Normalize()
        {
            Double VectorLength = Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);

            this.m_XCoord /= VectorLength;
            this.m_YCoord /= VectorLength;
            this.m_ZCoord /= VectorLength;
        }

        /// <summary>
        /// метод GetParallelComponent возвращает компоненту текущего вектора, параллельную вектору DirectingVector
        /// </summary>
        /// <param name="DirectingVector">направляющий вектор</param>
        /// <returns>компонента (вектор) текущего вектора, параллельная вектору DirectingVector</returns>
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
        /// метод GetPerpendicularComponent возвращает компоненту текущего вектора, перпендикулярную вектору DirectingVector
        /// </summary>
        /// <param name="DirectingVector">направляющий вектор</param>
        /// <returns>компонента текущего вектора, перпендикулярная вектору DirectingVector</returns>
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
        /// XCoord - свойство для доступа к координате X вектора
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
        /// YCoord - свойство для доступа к координате Y вектора
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
        /// ZCoord - свойство для доступа к координате Z вектора
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
        /// Length - длина вектора
        /// </summary>
        public Double Length
        {
            get
            {
                return Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);
            }
        }

        /// <summary>
        /// метод VectorAddition возвращает результат сложения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат сложения векторов a и b</returns>
        public static Vector3D VectorAddition(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.XCoord + b.XCoord, a.YCoord + b.YCoord, a.ZCoord + b.ZCoord);
        }

        /// <summary>
        /// метод VectorSubtraction возвращает результат разности векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат разности векторов a и b</returns>
        public static Vector3D VectorSubtraction(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.XCoord - b.XCoord, a.YCoord - b.YCoord, a.ZCoord - b.ZCoord);
        }

        /// <summary>
        /// метод VectorMultiplication возвращает результат умножения вектора a на число DoubleNumber
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="DoubleNumber">число DoubleNumber</param>
        /// <returns>результат умножения вектора a на число DoubleNumber</returns>
        public static Vector3D VectorMultiplication(Vector3D a, Double DoubleNumber)
        {
            return new Vector3D(DoubleNumber * a.XCoord, DoubleNumber * a.YCoord, DoubleNumber * a.ZCoord);
        }

        /// <summary>
        /// метод ScalarProduct возвращает результат скалярного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат скалярного произведения векторов a и b</returns>
        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.XCoord * b.XCoord + a.YCoord * b.YCoord + a.ZCoord * b.ZCoord;
        }

        /// <summary>
        /// метод VectorProduct возвращает результат (вектор) векторного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат (вектор) векторного произведения векторов a и b</returns>
        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double XCoord = a.YCoord * b.ZCoord - a.ZCoord * b.YCoord;
            Double YCoord = a.ZCoord * b.XCoord - a.XCoord * b.ZCoord;
            Double ZCoord = a.XCoord * b.YCoord - a.YCoord * b.XCoord;

            return new Vector3D(XCoord, YCoord, ZCoord);
        }

        /// <summary>
        /// метод MixedProduct возвращает результат смешанного произведения векторов a, b и c
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <param name="c">вектор c</param>
        /// <returns>результат смешанного произведения векторов a, b и c</returns>
        public static Double MixedProduct(Vector3D a, Vector3D b, Vector3D c)
        {
            return Vector3D.ScalarProduct(a, Vector3D.VectorProduct(b, c));
        }

        /// <summary>
        /// метод AngleBetweenVectors возвращает величину угла (в радианах) между векторами a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>угол (в радианах) между векторами a и b</returns>
        public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        {
            Double CosValue = Vector3D.ScalarProduct(a, b) / (a.Length * b.Length);
            // округление нужно потому, что из-за ошибок округления значение косинуса угла может стать > 1
            return Math.Acos(Math.Round(CosValue, CosDigits));
        }

        /// <summary>
        /// свойство ZeroVector3D - нулевой вектор
        /// </summary>
        public static Vector3D ZeroVector3D
        {
            get
            {
                return new Vector3D(0, 0, 0);
            }
        }

        /// <summary>
        /// оператор сложения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат сложения векторов a и b</returns>
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return Vector3D.VectorAddition(a, b);
        }

        /// <summary>
        /// оператор вычитания векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат разности векторов a и b</returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return Vector3D.VectorSubtraction(a, b);
        }

        /// <summary>
        /// оператор умножения вектора a на число DoubleNumber
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="DoubleNumber">число DoubleNumber</param>
        /// <returns>результат умножения вектора a на число DoubleNumber</returns>
        public static Vector3D operator *(Double DoubleNumber, Vector3D a)
        {
            return Vector3D.VectorMultiplication(a, DoubleNumber);
        }

        /// <summary>
        /// оператор скалярного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат скалярного произведения векторов a и b</returns>
        public static Double operator *(Vector3D a, Vector3D b)
        {
            return Vector3D.ScalarProduct(a, b);
        }
    }

    /// <summary>
    /// класс Matrix предназначен для хранения матриц NxM (элементов матриц) и манипуляций с ними
    /// </summary>
    public class Matrix : ICloneable
    {
        /// <summary>
        /// m_MatrixElements - массив для хранения элементов матрицы
        /// </summary>
        Double[,] m_MatrixElements = null;
        /// <summary>
        /// m_RowCount - количество строк
        /// </summary>
        Int32 m_RowCount = 0;
        /// <summary>
        /// m_ColumnCount - количество столбцов
        /// </summary>
        Int32 m_ColumnCount = 0;

        /// <summary>
        /// конструктор класса Matrix
        /// </summary>
        /// <param name="RowCount">количество строк</param>
        /// <param name="ColumnCount">количество столбцов</param>
        public Matrix(Int32 RowCount, Int32 ColumnCount)
        {
            m_MatrixElements = new Double[RowCount, ColumnCount];
            m_RowCount = RowCount;
            m_ColumnCount = ColumnCount;
        }

        /// <summary>
        /// свойство (индексатор) для доступа к элементам матрицы
        /// номер строки изменяется в диапазоне 1...RowCount
        /// номер столбца изменяется в диапазоне 1...ColumnCount
        /// </summary>
        /// <param name="RowIndex">номер (индекс) строки</param>
        /// <param name="ColumnIndex">номер (индекс) столбца</param>
        /// <returns>элемент матрицы</returns>
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
        /// свойтво RowCount возвращает количество строк
        /// </summary>
        public Int32 RowCount
        {
            get
            {
                return m_RowCount;
            }
        }

        /// <summary>
        /// свойтво RowCount возвращает количество столбцов
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                return m_ColumnCount;
            }
        }

        /// <summary>
        /// метод GetMatrixRow возвращает строку (в виде матрицы) матрицы
        /// </summary>
        /// <param name="RowIndex">номер (индекс) возвращаемой строки</param>
        /// <returns>возвращаемая строка (в виде матрицы)</returns>
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
        /// метод SetMatrixRow сохраняет строку (заданную в виде матрицы) в строке исходной матрицы с номером (индексом) RowIndex
        /// </summary>
        /// <param name="RowIndex">номер (индекс) строки исходной матрицы</param>
        /// <param name="RowMatrix">строка (заданная в виде матрицы), которую надо сохранить в исходной матрице</param>
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
        /// метод MatrixAddition возвращает результат сложения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат сложения матриц Matrix1 и Matrix2</returns>
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
        /// метод MatrixSubtraction возвращает результат разности матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат разности матриц Matrix1 и Matrix2</returns>
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
        /// метод MatrixMultiplication возвращает результат умножения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат умножения матриц Matrix1 и Matrix2</returns>
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
        /// метод MatrixMultiplication возвращает результат умножения матрицы Matrix1 на число DoubleNumber
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="DoubleNumber">число DoubleNumber</param>
        /// <returns>результат умножения матрицы Matrix1 на число DoubleNumber</returns>
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
        /// метод MatrixTransposing возвращает результат транспонирования матрицы Matrix1
        /// </summary>
        /// <param name="Matrix1">исходная матрица Matrix1</param>
        /// <returns>результат транспонирования матрицы Matrix1</returns>
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
        /// оператор сложения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат сложения матриц Matrix1 и Matrix2</returns>
        public static Matrix operator +(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixAddition(Matrix1, Matrix2);
        }

        /// <summary>
        /// оператор вычитания матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат разности матриц Matrix1 и Matrix2</returns>
        public static Matrix operator -(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixSubtraction(Matrix1, Matrix2);
        }

        /// <summary>
        /// оператор умножения матриц Matrix1 и Matrix2
        /// </summary>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <param name="Matrix2">матрица Matrix2</param>
        /// <returns>результат умножения матриц Matrix1 и Matrix2</returns>
        public static Matrix operator *(Matrix Matrix1, Matrix Matrix2)
        {
            return Matrix.MatrixMultiplication(Matrix1, Matrix2);
        }

        /// <summary>
        /// оператор умножения матрицы Matrix1 на число DoubleNumber
        /// </summary>
        /// <param name="DoubleNumber">число DoubleNumber</param>
        /// <param name="Matrix1">матрица Matrix1</param>
        /// <returns>результат умножения матрицы Matrix1 на число DoubleNumber</returns>
        public static Matrix operator *(Double DoubleNumber, Matrix Matrix1)
        {
            return Matrix.MatrixMultiplication(Matrix1, DoubleNumber);
        }

        /// <summary>
        /// метод Clone возвращает полную копию (deep copy) данной матрицы
        /// </summary>
        /// <returns>полная копия (deep copy) данной матрицы</returns>
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
        /// метод Clone возвращает полную копию (deep copy) данной матрицы (явная реализация метода Clone интерфейса ICloneable)
        /// </summary>
        /// <returns>полная копия (deep copy) данной матрицы</returns>
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    /// <summary>
    /// класс FundKoshiMatrix инкапсулирует вычисление фундаментальной матрицы Коши
    /// (а точнее матрицы, составленной из N строк матрицы Коши - см. алгоритмы решения линейных дифф. игр)
    /// </summary>
    public class FundKoshiMatrix
    {
        /// <summary>
        /// DeltaT - шаг по T решения дифференциального уравнения
        /// </summary>
        #warning параметр DeltaT должен задаваться снаружи
        private const Double m_DeltaT = 0.001;
        /// <summary>
        /// матрица A, по которой вычисляется фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши)
        /// </summary>
        private Matrix m_MatrixA = null;
        /// <summary>
        /// набор номеров строк, интересующих нас в фундаментальной матрице Коши
        /// (каждая строка вычисляется независимо, поэтому нет смысла вычислять фундаментальную матрицу Коши целиком)
        /// </summary>
        private Int32[] m_RowIndexes = null;
        /// <summary>
        /// обратное время (т.е. Theta - T) последнего вычисления фундаментальной матрицы Коши
        /// </summary>
        private Double m_LastInverseTime = Double.NaN;
        /// <summary>
        /// фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши), вычисленная для времени m_LastT
        /// </summary>
        private Matrix m_LastFundKoshiMatrix = null;

        /// <summary>
        /// метод GetZeroFundKoshiMatrix возвращает фундаментальную матрицу Коши (а точнее матрицу, составленную из N строк матрицы Коши) при T = 0 (T - обратное время)
        /// </summary>
        /// <returns>фундаментальная матрица Коши (а точнее матрица, составленная из N строк матрицы Коши) при T = 0 (T - обратное время)</returns>
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
        /// метод DeltaRowCalc вычисляет изменение вектора при шаге по времени m_DeltaT (по методу Рунге-Кутта)
        /// </summary>
        /// <param name="PreviousRow">предыдущый вектор (по времени)</param>
        /// <returns>изменение вектора при шаге по времени m_DeltaT </returns>
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
        /// конструктор класса FundKoshiMatrix
        /// </summary>
        /// <param name="MatrixA">матрица A, по которой вычисляется фундаментальная матрица Коши</param>
        /// <param name="RowIndexes">набор номеров строк, интересующих нас в фундаментальной матрице Коши</param>
        public FundKoshiMatrix(Matrix MatrixA, Int32[] RowIndexes)
        {
            // ?? may be клонирование не нужно
            //m_MatrixA = (MatrixA.Clone() as Matrix);
            #warning Возможно необходимо хранить в m_MatrixA ссылку на клон матрицы MatrixA
            m_MatrixA = MatrixA;
            m_RowIndexes = RowIndexes;

            // номера строк в наборе RowIndexes должны идти в порядке возрастания
            for (Int32 Index = 1; Index < RowIndexes.Length; Index++)
            {
                if (RowIndexes[Index - 1] >= RowIndexes[Index])
                {
                    #warning должны ли номера строк в наборе RowIndexes идти в порядке возрастания ???
                }
            }
        }

        /// <summary>
        /// метод FundKoshiMatrixCalc вычисляет фундаментальную матрицу Коши (а точнее матрицу, составленную из N строк матрицы Коши)
        /// в момент обратного времени InverseTime
        /// </summary>
        /// <param name="InverseTime">момент обратного времени, для котрого производятся вычисления</param>
        /// <returns>матрица, составленная из N строк фундаментальной матрицы Коши в момент обратного времени InverseTime</returns>
        public Matrix FundKoshiMatrixCalc(Double InverseTime)
        {
            if (m_LastInverseTime == InverseTime) return m_LastFundKoshiMatrix;

            Matrix FundKoshiMatrixValue = new Matrix(m_RowIndexes.Length, m_MatrixA.ColumnCount);

            // вычисление начального момента обратного времени и начального значения вычисляемой матрицы
            // если момент времени InverseTime, для которого нам надо вычислить матрицу больше, чем значение m_LastInverseTime,
            // то начальным моментом обратного времени будет значение m_LastInverseTime, а начальным значением вычисляемой матрицы - m_LastFundKoshiMatrix (значение матрицы, вычисленное для момента обратного m_LastInverseTime)
            // иначе начальным моментом обратного времени будет 0, начальным значением вычисляемой матрицы - матрица составленная из N строк из единичной
            Matrix InitialFundKoshiMatrix = (m_LastInverseTime < InverseTime ? m_LastFundKoshiMatrix : GetZeroTimeFundKoshiMatrix());
            Double CurrentInverseTime = (m_LastInverseTime < InverseTime ? m_LastInverseTime : 0);

            // преобразование начальной матрицы в массив строк
            Matrix[] FundKoshiMatrixValueRows = new Matrix[FundKoshiMatrixValue.RowCount];
            for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
            {
                FundKoshiMatrixValueRows[RowIndex] = InitialFundKoshiMatrix.GetMatrixRow(RowIndex + 1);
            }

            // вычисление конечного значения матрицы, составленной из N строк фундаментальной матрицы Коши
            while (CurrentInverseTime < InverseTime)
            {
                for (Int32 RowIndex = 0; RowIndex < FundKoshiMatrixValueRows.Length; RowIndex++)
                {
                    FundKoshiMatrixValueRows[RowIndex] += DeltaRowCalc(FundKoshiMatrixValueRows[RowIndex]);
                }

                CurrentInverseTime += m_DeltaT;
            }

            // преобразование массива строк в конечную матрицу
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
    /// класс CyclicList (потомок generic-класса List) представляет циклический список элементов типа T
    /// </summary>
    /// <typeparam name="T">тип элементов, хранящихся в циклическом списке</typeparam>
    public class CyclicList<T> : List<T>
    {
        /// <summary>
        /// метод NextItemIndex возвращает индекс следующего элемента в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItemIndex">индекс текущего элемента</param>
        /// <returns>индекс следующего элемента в циклическом списке</returns>
        public Int32 NextItemIndex(Int32 CurrentItemIndex)
        {
            if (CurrentItemIndex == -1)
            {
                #warning по идее кидать надо исключение потомок класса ArgumentException
                throw new ArgumentException("Finding item does not belong this list");
            }

            return (CurrentItemIndex == this.Count - 1 ? 0 : CurrentItemIndex + 1);
        }

        /// <summary>
        /// метод NextItemIndex возвращает индекс следующего элемента в циклическом списке по заданному текущему элементу
        /// если текущий элемент не уникален в списке, то метод NextItemIndex может возвратить неверный индекс
        /// в этом случае лучше пользоваться версией, которая возвращает индекс следующего элемента в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItem">текущий элемент</param>
        /// <returns>индекс следующего элемента в циклическом списке</returns>
        public Int32 NextItemIndex(T CurrentItem)
        {
            Int32 CurrentItemIndex = this.IndexOf(CurrentItem);

            return NextItemIndex(CurrentItemIndex);
        }

        /// <summary>
        /// метод NextItem возвращает следующей элемент в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItemIndex">индекс текущего элемента</param>
        /// <returns>следующей элемент в циклическом списке</returns>
        public T NextItem(Int32 CurrentItemIndex)
        {
            return this[NextItemIndex(CurrentItemIndex)];
        }

        /// <summary>
        /// метод NextItem возвращает следующей элемент в циклическом списке по заданному текущему элементу
        /// если текущий элемент не уникален в списке, то метод NextItem может возвратить неверный элемент
        /// в этом случае лучше пользоваться версией, которая возвращает следующей элемент в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItem">текущий элемент</param>
        /// <returns>следующей элемент в циклическом списке</returns>
        public T NextItem(T CurrentItem)
        {
            return this[NextItemIndex(CurrentItem)];
        }

        /// <summary>
        /// метод PrevItemIndex возвращает индекс предыдущего элемента в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItemIndex">индекс текущего элемента</param>
        /// <returns>индекс предыдущего элемента в циклическом списке</returns>
        public Int32 PrevItemIndex(Int32 CurrentItemIndex)
        {
            if (CurrentItemIndex == -1)
            {
                #warning по идее кидать надо исключение потомок класса ArgumentException
                throw new ArgumentException("Finding item does not belong this list");
            }

            return (CurrentItemIndex == 0 ? this.Count - 1 : CurrentItemIndex - 1);
        }

        /// <summary>
        /// метод PrevItemIndex возвращает индекс предыдущего элемента в циклическом списке по заданному текущему элементу
        /// если текущий элемент не уникален в списке, то метод PrevItemIndex может возвратить неверный индекс
        /// в этом случае лучше пользоваться версией, которая возвращает индекс предыдущего элемента в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItem">текущий элемент</param>
        /// <returns>индекс предыдущего элемента в циклическом списке</returns>
        public Int32 PrevItemIndex(T CurrentItem)
        {
            Int32 CurrentItemIndex = this.IndexOf(CurrentItem);

            return PrevItemIndex(CurrentItemIndex);
        }

        /// <summary>
        /// метод PrevItem возвращает предыдущий элемент в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItemIndex">индекс текущего элемента</param>
        /// <returns>предыдущий элемент в циклическом списке</returns>
        public T PrevItem(Int32 CurrentItemIndex)
        {
            return this[PrevItemIndex(CurrentItemIndex)];
        }

        /// <summary>
        /// метод PrevItem возвращает предыдущий элемент в циклическом списке по заданному текущему элементу
        /// если текущий элемент не уникален в списке, то метод PrevItem может возвратить неверный элемент
        /// в этом случае лучше пользоваться версией, которая возвращает предыдущий элемент в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="CurrentItem">текущий элемент</param>
        /// <returns>предыдущий элемент в циклическом списке</returns>
        public T PrevItem(T CurrentItem)
        {
            return this[PrevItemIndex(CurrentItem)];
        }
    }
}
