using System;

namespace LinearDiff3DGame.Geometry3D.Common
{
    /// <summary>
    /// структура Vector3D представляет 3D вектор
    /// </summary>
    public struct Vector3D
    {
        /// <summary>
        /// конструктор структуры Vector3D
        /// </summary>
        /// <param name="coordX">X координата 3D вектора</param>
        /// <param name="coordY">Y координата 3D вектора</param>
        /// <param name="coordZ">Z координата 3D вектора</param>
        public Vector3D(Double coordX, Double coordY, Double coordZ)
        {
            m_XCoord = coordX;
            m_YCoord = coordY;
            m_ZCoord = coordZ;
        }

        ///// <summary>
        ///// метод Normalize нормирует текущий вектор
        ///// </summary>
        //public void Normalize()
        //{
        //    Double VectorLength = Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);

        //    m_XCoord /= VectorLength;
        //    m_YCoord /= VectorLength;
        //    m_ZCoord /= VectorLength;
        //}

        ///// <summary>
        ///// метод GetParallelComponent возвращает компоненту текущего вектора, параллельную вектору directingVector
        ///// </summary>
        ///// <param name="directingVector">направляющий вектор</param>
        ///// <returns>компонента (вектор) текущего вектора, параллельная вектору directingVector</returns>
        //public Vector3D GetParallelComponent(Vector3D directingVector)
        //{
        //    Double scalarProductValue = ScalarProduct(this, directingVector);

        //    Double parallelCompX = scalarProductValue * directingVector.XCoord / directingVector.Length;
        //    Double parallelCompY = scalarProductValue * directingVector.YCoord / directingVector.Length;
        //    Double parallelCompZ = scalarProductValue * directingVector.ZCoord / directingVector.Length;

        //    return new Vector3D(parallelCompX, parallelCompY, parallelCompZ);
        //}

        ///// <summary>
        ///// метод GetPerpendicularComponent возвращает компоненту текущего вектора, перпендикулярную вектору directingVector
        ///// </summary>
        ///// <param name="directingVector">направляющий вектор</param>
        ///// <returns>компонента текущего вектора, перпендикулярная вектору directingVector</returns>
        //public Vector3D GetPerpendicularComponent(Vector3D directingVector)
        //{
        //    Double scalarProductValue = ScalarProduct(this, directingVector);

        //    Double perpendicularCompX = m_XCoord - scalarProductValue * directingVector.XCoord / directingVector.Length;
        //    Double perpendicularCompY = m_YCoord - scalarProductValue * directingVector.YCoord / directingVector.Length;
        //    Double perpendicularCompZ = m_ZCoord - scalarProductValue * directingVector.ZCoord / directingVector.Length;

        //    return new Vector3D(perpendicularCompX, perpendicularCompY, perpendicularCompZ);
        //}

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

        public override string ToString()
        {
            return String.Format("({0} ; {1} ; {2})", m_XCoord, m_YCoord, m_ZCoord);
        }

        /// <summary>
        /// метод VectorAddition возвращает результат сложения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат сложения векторов a и b</returns>
        public static Vector3D VectorAddition(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.m_XCoord + b.m_XCoord, a.m_YCoord + b.m_YCoord, a.m_ZCoord + b.m_ZCoord);
        }

        /// <summary>
        /// метод VectorSubtraction возвращает результат разности векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат разности векторов a и b</returns>
        public static Vector3D VectorSubtraction(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.m_XCoord - b.m_XCoord, a.m_YCoord - b.m_YCoord, a.m_ZCoord - b.m_ZCoord);
        }

        /// <summary>
        /// метод VectorMultiplication возвращает результат умножения вектора a на число number
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="number">число number</param>
        /// <returns>результат умножения вектора a на число number</returns>
        public static Vector3D VectorMultiplication(Vector3D a, Double number)
        {
            return new Vector3D(number * a.m_XCoord, number * a.m_YCoord, number * a.m_ZCoord);
        }

        /// <summary>
        /// метод ScalarProduct возвращает результат скалярного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат скалярного произведения векторов a и b</returns>
        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.m_XCoord * b.m_XCoord + a.m_YCoord * b.m_YCoord + a.m_ZCoord * b.m_ZCoord;
        }

        /// <summary>
        /// метод VectorProduct возвращает результат (вектор) векторного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат (вектор) векторного произведения векторов a и b</returns>
        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double coordX = a.m_YCoord * b.m_ZCoord - a.m_ZCoord * b.m_YCoord;
            Double coordY = a.m_ZCoord * b.m_XCoord - a.m_XCoord * b.m_ZCoord;
            Double coordZ = a.m_XCoord * b.m_YCoord - a.m_YCoord * b.m_XCoord;

            return new Vector3D(coordX, coordY, coordZ);
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
            return ScalarProduct(a, VectorProduct(b, c));
        }

        ///// <summary>
        ///// метод AngleBetweenVectors возвращает величину угла (в радианах) между векторами a и b
        ///// </summary>
        ///// <param name="a">вектор a</param>
        ///// <param name="b">вектор b</param>
        ///// <returns>угол (в радианах) между векторами a и b</returns>
        //public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        //{
        //    Double cosValue = ScalarProduct(a, b) / (a.Length * b.Length);

        //    // из-за ошибок округления значение косинуса угла может стать > 1 (или < -1)
        //    if (cosValue > 1) cosValue = 1;
        //    if (cosValue < -1) cosValue = -1;

        //    return Math.Acos(cosValue);
        //}

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
            return VectorAddition(a, b);
        }

        /// <summary>
        /// оператор вычитания векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат разности векторов a и b</returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return VectorSubtraction(a, b);
        }

        /// <summary>
        /// оператор умножения вектора a на число number
        /// </summary>
        /// <param name="number">число number</param>
        /// <param name="a">вектор a</param>
        /// <returns>результат умножения вектора a на число number</returns>
        public static Vector3D operator *(Double number, Vector3D a)
        {
            return VectorMultiplication(a, number);
        }

        /// <summary>
        /// оператор умножения вектора a на число number
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="number">число number</param>
        /// <returns>результат умножения вектора a на число number</returns>
        public static Vector3D operator *(Vector3D a, Double number)
        {
            return VectorMultiplication(a, number);
        }

        /// <summary>
        /// оператор скалярного произведения векторов a и b
        /// </summary>
        /// <param name="a">вектор a</param>
        /// <param name="b">вектор b</param>
        /// <returns>результат скалярного произведения векторов a и b</returns>
        public static Double operator *(Vector3D a, Vector3D b)
        {
            return ScalarProduct(a, b);
        }

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
    }
}