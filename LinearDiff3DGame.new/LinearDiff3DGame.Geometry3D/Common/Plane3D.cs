using System;

namespace LinearDiff3DGame.Geometry3D.Common
{
    /// <summary>
    /// ���������, �������������� 3D ���������
    /// </summary>
    public struct Plane3D
    {
        /// <summary>
        /// ����������� ��������� Plane3D
        /// </summary>
        /// <param name="koeffA">����������� A ��������� ��������� ���������</param>
        /// <param name="koeffB">����������� B ��������� ��������� ���������</param>
        /// <param name="koeffC">����������� C ��������� ��������� ���������</param>
        /// <param name="koeffD">����������� D ��������� ��������� ���������</param>
        public Plane3D(Double koeffA, Double koeffB, Double koeffC, Double koeffD)
            : this()
        {
            KoeffA = koeffA;
            KoeffB = koeffB;
            KoeffC = koeffC;
            KoeffD = koeffD;
        }

        /// <summary>
        /// ����������� A ��������� ��������� ���������
        /// </summary>
        public Double KoeffA
        {
            get; private set;
        }
        /// <summary>
        /// ����������� B ��������� ��������� ���������
        /// </summary>
        public Double KoeffB
        {
            get; private set;
        }
        /// <summary>
        /// ����������� C ��������� ��������� ���������
        /// </summary>
        public Double KoeffC
        {
            get; private set;
        }
        /// <summary>
        /// ����������� D ��������� ��������� ���������
        /// </summary>
        public Double KoeffD
        {
            get; private set;
        }
    }
}