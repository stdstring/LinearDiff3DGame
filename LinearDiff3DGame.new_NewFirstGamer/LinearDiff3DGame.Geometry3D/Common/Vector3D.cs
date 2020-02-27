using System;

namespace LinearDiff3DGame.Geometry3D.Common
{
    /// <summary>
    /// ��������� Vector3D ������������ 3D ������
    /// </summary>
    public struct Vector3D
    {
        /// <summary>
        /// ����������� ��������� Vector3D
        /// </summary>
        /// <param name="coordX">X ���������� 3D �������</param>
        /// <param name="coordY">Y ���������� 3D �������</param>
        /// <param name="coordZ">Z ���������� 3D �������</param>
        public Vector3D(Double coordX, Double coordY, Double coordZ)
        {
            m_XCoord = coordX;
            m_YCoord = coordY;
            m_ZCoord = coordZ;
        }

        ///// <summary>
        ///// ����� Normalize ��������� ������� ������
        ///// </summary>
        //public void Normalize()
        //{
        //    Double VectorLength = Math.Sqrt(m_XCoord * m_XCoord + m_YCoord * m_YCoord + m_ZCoord * m_ZCoord);

        //    m_XCoord /= VectorLength;
        //    m_YCoord /= VectorLength;
        //    m_ZCoord /= VectorLength;
        //}

        ///// <summary>
        ///// ����� GetParallelComponent ���������� ���������� �������� �������, ������������ ������� directingVector
        ///// </summary>
        ///// <param name="directingVector">������������ ������</param>
        ///// <returns>���������� (������) �������� �������, ������������ ������� directingVector</returns>
        //public Vector3D GetParallelComponent(Vector3D directingVector)
        //{
        //    Double scalarProductValue = ScalarProduct(this, directingVector);

        //    Double parallelCompX = scalarProductValue * directingVector.XCoord / directingVector.Length;
        //    Double parallelCompY = scalarProductValue * directingVector.YCoord / directingVector.Length;
        //    Double parallelCompZ = scalarProductValue * directingVector.ZCoord / directingVector.Length;

        //    return new Vector3D(parallelCompX, parallelCompY, parallelCompZ);
        //}

        ///// <summary>
        ///// ����� GetPerpendicularComponent ���������� ���������� �������� �������, ���������������� ������� directingVector
        ///// </summary>
        ///// <param name="directingVector">������������ ������</param>
        ///// <returns>���������� �������� �������, ���������������� ������� directingVector</returns>
        //public Vector3D GetPerpendicularComponent(Vector3D directingVector)
        //{
        //    Double scalarProductValue = ScalarProduct(this, directingVector);

        //    Double perpendicularCompX = m_XCoord - scalarProductValue * directingVector.XCoord / directingVector.Length;
        //    Double perpendicularCompY = m_YCoord - scalarProductValue * directingVector.YCoord / directingVector.Length;
        //    Double perpendicularCompZ = m_ZCoord - scalarProductValue * directingVector.ZCoord / directingVector.Length;

        //    return new Vector3D(perpendicularCompX, perpendicularCompY, perpendicularCompZ);
        //}

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

        public override string ToString()
        {
            return String.Format("({0} ; {1} ; {2})", m_XCoord, m_YCoord, m_ZCoord);
        }

        /// <summary>
        /// ����� VectorAddition ���������� ��������� �������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D VectorAddition(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.m_XCoord + b.m_XCoord, a.m_YCoord + b.m_YCoord, a.m_ZCoord + b.m_ZCoord);
        }

        /// <summary>
        /// ����� VectorSubtraction ���������� ��������� �������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D VectorSubtraction(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.m_XCoord - b.m_XCoord, a.m_YCoord - b.m_YCoord, a.m_ZCoord - b.m_ZCoord);
        }

        /// <summary>
        /// ����� VectorMultiplication ���������� ��������� ��������� ������� a �� ����� number
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="number">����� number</param>
        /// <returns>��������� ��������� ������� a �� ����� number</returns>
        public static Vector3D VectorMultiplication(Vector3D a, Double number)
        {
            return new Vector3D(number * a.m_XCoord, number * a.m_YCoord, number * a.m_ZCoord);
        }

        /// <summary>
        /// ����� ScalarProduct ���������� ��������� ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� ���������� ������������ �������� a � b</returns>
        public static Double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.m_XCoord * b.m_XCoord + a.m_YCoord * b.m_YCoord + a.m_ZCoord * b.m_ZCoord;
        }

        /// <summary>
        /// ����� VectorProduct ���������� ��������� (������) ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� (������) ���������� ������������ �������� a � b</returns>
        public static Vector3D VectorProduct(Vector3D a, Vector3D b)
        {
            Double coordX = a.m_YCoord * b.m_ZCoord - a.m_ZCoord * b.m_YCoord;
            Double coordY = a.m_ZCoord * b.m_XCoord - a.m_XCoord * b.m_ZCoord;
            Double coordZ = a.m_XCoord * b.m_YCoord - a.m_YCoord * b.m_XCoord;

            return new Vector3D(coordX, coordY, coordZ);
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
            return ScalarProduct(a, VectorProduct(b, c));
        }

        ///// <summary>
        ///// ����� AngleBetweenVectors ���������� �������� ���� (� ��������) ����� ��������� a � b
        ///// </summary>
        ///// <param name="a">������ a</param>
        ///// <param name="b">������ b</param>
        ///// <returns>���� (� ��������) ����� ��������� a � b</returns>
        //public static Double AngleBetweenVectors(Vector3D a, Vector3D b)
        //{
        //    Double cosValue = ScalarProduct(a, b) / (a.Length * b.Length);

        //    // ��-�� ������ ���������� �������� �������� ���� ����� ����� > 1 (��� < -1)
        //    if (cosValue > 1) cosValue = 1;
        //    if (cosValue < -1) cosValue = -1;

        //    return Math.Acos(cosValue);
        //}

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
            return VectorAddition(a, b);
        }

        /// <summary>
        /// �������� ��������� �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� �������� �������� a � b</returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return VectorSubtraction(a, b);
        }

        /// <summary>
        /// �������� ��������� ������� a �� ����� number
        /// </summary>
        /// <param name="number">����� number</param>
        /// <param name="a">������ a</param>
        /// <returns>��������� ��������� ������� a �� ����� number</returns>
        public static Vector3D operator *(Double number, Vector3D a)
        {
            return VectorMultiplication(a, number);
        }

        /// <summary>
        /// �������� ��������� ������� a �� ����� number
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="number">����� number</param>
        /// <returns>��������� ��������� ������� a �� ����� number</returns>
        public static Vector3D operator *(Vector3D a, Double number)
        {
            return VectorMultiplication(a, number);
        }

        /// <summary>
        /// �������� ���������� ������������ �������� a � b
        /// </summary>
        /// <param name="a">������ a</param>
        /// <param name="b">������ b</param>
        /// <returns>��������� ���������� ������������ �������� a � b</returns>
        public static Double operator *(Vector3D a, Vector3D b)
        {
            return ScalarProduct(a, b);
        }

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
    }
}