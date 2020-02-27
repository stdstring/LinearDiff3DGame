using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ��������� Point3D ������������ 3D �����
    /// </summary>
    public struct Point3D
    {
        /// <summary>
        /// ����������� ��������� Point3D
        /// </summary>
        /// <param name="coordX">X ���������� 3D �����</param>
        /// <param name="coordY">Y ���������� 3D �����</param>
        /// <param name="coordZ">Z ���������� 3D �����</param>
        public Point3D(Double coordX, Double coordY, Double coordZ)
        {
            m_XCoord = coordX;
            m_YCoord = coordY;
            m_ZCoord = coordZ;
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
    }
}
