using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
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
        {
            m_KoeffA = koeffA;
            m_KoeffB = koeffB;
            m_KoeffC = koeffC;
            m_KoeffD = koeffD;
        }

        /// <summary>
        /// ����������� A ��������� ��������� ���������
        /// </summary>
        public Double KoeffA
        {
            get
            {
                return m_KoeffA;
            }
        }
        /// <summary>
        /// ����������� B ��������� ��������� ���������
        /// </summary>
        public Double KoeffB
        {
            get
            {
                return m_KoeffB;
            }
        }
        /// <summary>
        /// ����������� C ��������� ��������� ���������
        /// </summary>
        public Double KoeffC
        {
            get
            {
                return m_KoeffC;
            }
        }
        /// <summary>
        /// ����������� D ��������� ��������� ���������
        /// </summary>
        public Double KoeffD
        {
            get
            {
                return m_KoeffD;
            }
        }
        
        /// <summary>
        /// ����������� A ��������� ��������� ���������
        /// </summary>
        private Double m_KoeffA;
        /// <summary>
        /// ����������� B ��������� ��������� ���������
        /// </summary>
        private Double m_KoeffB;
        /// <summary>
        /// ����������� C ��������� ��������� ���������
        /// </summary>
        private Double m_KoeffC;
        /// <summary>
        /// ����������� D ��������� ��������� ���������
        /// </summary>
        private Double m_KoeffD;
    }
}
