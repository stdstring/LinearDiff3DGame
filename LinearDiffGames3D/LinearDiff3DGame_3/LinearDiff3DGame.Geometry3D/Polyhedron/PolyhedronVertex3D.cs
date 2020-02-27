using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ������������ ������� 3-������� �������������
    /// </summary>
    public class PolyhedronVertex3D
    {
        /// <summary>
        /// ����������� ������ PolyhedronVertex3D
        /// </summary>
        /// <param name="xCoord">X ���������� �������</param>
        /// <param name="yCoord">Y ���������� �������</param>
        /// <param name="zCoord">Z ���������� �������</param>
        /// <param name="vertexID">ID �������</param>
        public PolyhedronVertex3D(Double xCoord, Double yCoord, Double zCoord, Int32 vertexID)
        {
            ID = vertexID;

            m_SideList = new List<PolyhedronSide3D>();
            m_XCoord = xCoord;
            m_YCoord = yCoord;
            m_ZCoord = zCoord;
        }

        /// <summary>
        /// ����������� ������ PolyhedronVertex3D
        /// </summary>
        /// <param name="vertex">���������� ������� (� ���� ������� Point3D)</param>
        /// <param name="vertexID">ID �������</param>
        public PolyhedronVertex3D(Point3D vertex, Int32 vertexID) : this(vertex.XCoord, vertex.YCoord, vertex.ZCoord, vertexID)
        {
        }

        /// <summary>
        /// ID - ���������� ������������� �������
        /// </summary>
        public readonly Int32 ID;

        /// <summary>
        /// Dimension - ����������� ������� (� ����� ������ 3)
        /// </summary>
        public const Int32 Dimension = 3;

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

        /* ��� �� ���� ������ ... � ���� �� �������� ����������������� ������ ������ ������ ������� � ������� ������ ������, ������� ����������� ������ ������� */
#warning Incorrect architecture

        /// <summary>
        /// ������ ������, ������� ����������� ������ �������
        /// </summary>
        public IList<PolyhedronSide3D> SideList
        {
            get
            {
                return m_SideList;
            }
        }

        /*/// <summary>
        /// �������� SideCount ���������� ���������� ������ � ������ ������, ������� ����������� ������ �������
        /// </summary>
        public Int32 SideCount
        {
            get
            {
                return m_SideList.Count;
            }
        }

        /// <summary>
        /// ��������-���������� ��� ������� (������) � ����� �� ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="index">������ ����� � ������ ������</param>
        /// <returns>���� �� ������ �� ������ ������</returns>
        public PolyhedronSide3D this[Int32 index]
        {
            get
            {
                return m_SideList[index];
            }
        }

        /// <summary>
        /// ����� AddSide ��������� ����� item � ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="item">����������� �����</param>
        public void AddSide(PolyhedronSide3D item)
        {
            m_SideList.Add(item);
        }

        /// <summary>
        /// ����� RemoveSide ������� ����� item �� ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="item">��������� �����</param>
        public void RemoveSide(PolyhedronSide3D item)
        {
            m_SideList.Remove(item);
        }*/
        /* ��� �� ���� ������ ... � ���� �� �������� ����������������� ������ ������ ������ ������� � ������� ������ ������, ������� ����������� ������ ������� */


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
        /// m_SideList - ������ ������, ������� ����������� ������ �������
        /// </summary>
        private List<PolyhedronSide3D> m_SideList;
    }
}
