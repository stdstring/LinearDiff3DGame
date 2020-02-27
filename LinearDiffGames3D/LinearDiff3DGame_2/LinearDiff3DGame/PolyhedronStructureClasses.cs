using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// ����� VertexClass ������������ 3D ������� �������������
    /// </summary>
    public class VertexClass
    {
        /// <summary>
        /// m_SideList - ������ ������, ������� ����������� ������ �������
        /// </summary>
        private List<SideClass> m_SideList;

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
        /// Dimension - ����������� ������� (� ����� ������ 3)
        /// </summary>
        public readonly Int32 Dimension = 3;
        /// <summary>
        /// Index - ������ ������� � ������ ���� ������ �������������
        /// </summary>
        public readonly Int32 Index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XCoord"></param>
        /// <param name="YCoord"></param>
        /// <param name="ZCoord"></param>
        /// <param name="VertexIndex"></param>
        public VertexClass(Double XCoord, Double YCoord, Double ZCoord, Int32 VertexIndex)
        {
            this.Index = VertexIndex;

            this.m_SideList = new List<SideClass>();
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Vertex"></param>
        /// <param name="VertexIndex"></param>
        public VertexClass(Point3D Vertex, Int32 VertexIndex) : this(Vertex.XCoord, Vertex.YCoord, Vertex.ZCoord, VertexIndex)
        {
        }

        /// <summary>
        /// ����� AddSide ��������� ����� Side � ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="Side">����������� �����</param>
        public void AddSide(SideClass Side)
        {
            m_SideList.Add(Side);
        }

        /// <summary>
        /// ����� RemoveSide ������� ����� Side �� ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="Side">��������� �����</param>
        public void RemoveSide(SideClass Side)
        {
            m_SideList.Remove(Side);
        }

        /// <summary>
        /// ��������-���������� ��� ������� (������) � ����� �� ������ ������, ������� ����������� ������ �������
        /// </summary>
        /// <param name="SideIndex">������ ����� � ������ ������</param>
        /// <returns></returns>
        public SideClass this[Int32 SideIndex]
        {
            get
            {
                return m_SideList[SideIndex];
            }
            /*set
            {
            }*/
        }

        /// <summary>
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
    }

    /// <summary>
    /// ����� SideClass ������������ ����� �������������
    /// </summary>
    public class SideClass
    {
        /// <summary>
        /// m_VertexList - ������ ������, ������� ����������� ������ �����
        /// </summary>
        private CyclicList<VertexClass> m_VertexList;
        /// <summary>
        /// m_SideNormal - "�������" ������� � ����� (����������� �����)
        /// </summary>
        private Vector3D m_SideNormal;
        /// <summary>
        /// m_Index - ������ ����� � ������ ���� ������ �������������
        /// </summary>
        private Int32 m_Index;

        public SideClass(List<VertexClass> VertexList, Int32 SideIndex, Vector3D SideNormal)
        {
            this.m_Index = SideIndex;

            this.m_VertexList = new CyclicList<VertexClass>();
            foreach (VertexClass Vertex in VertexList)
            {
                AddVertex(Vertex);
            }

            this.m_SideNormal = SideNormal;
        }

        /// <summary>
        /// ����� AddVertex ��������� ������� Vertex � ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="Vertex">����������� �������</param>
        public void AddVertex(VertexClass Vertex)
        {
            m_VertexList.Add(Vertex);
            Vertex.AddSide(this);
        }

        /// <summary>
        /// ����� RemoveVertex ������� ������� Vertex �� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="Vertex">��������� �������</param>
        public void RemoveVertex(VertexClass Vertex)
        {
            m_VertexList.Remove(Vertex);
            Vertex.RemoveSide(this);
        }

        /// <summary>
        /// ����� HasVertex ���������� true, ���� ������� Vertex ����������� ������ ���������
        /// </summary>
        /// <param name="Vertex"></param>
        /// <returns></returns>
        public Boolean HasVertex(VertexClass Vertex)
        {
            return (m_VertexList.IndexOf(Vertex) != -1);
        }

        /// <summary>
        /// ����� GetNeighbourSide ���������� ��� ������ ����� ������ �� �����, ������������� ��������� EdgeVertex1 � EdgeVertex2
        /// </summary>
        /// <param name="EdgeVertex1">������ ������� �����</param>
        /// <param name="EdgeVertex2">������ ������� �����</param>
        /// <returns>����� �� ����� ��� ������ �����</returns>
        public SideClass GetNeighbourSide(VertexClass EdgeVertex1, VertexClass EdgeVertex2)
        {
            SideClass NeighbourSide = null;

            // ������� ������ 1 � 2 � ������ ������ ������ �����
            Int32 EdgeVertex1Index = m_VertexList.IndexOf(EdgeVertex1);
            Int32 EdgeVertex2Index = m_VertexList.IndexOf(EdgeVertex2);

            // �������� �� ��, ��� EdgeVertex1 � EdgeVertex2 ����������� ������� �����
            if ((EdgeVertex1Index == -1) || (EdgeVertex2Index == -1))
            {
                #warning �� ���� ������ ���� ���������� ������� ������ ArgumentException
                throw new ArgumentException("EdgeVertex1 and EdgeVertex2 must belong this (current) side");
            }
            // �������� �� ��, ��� EdgeVertex1 � EdgeVertex2 �� ����� ���� �����
            if ((m_VertexList.NextItemIndex(EdgeVertex1Index) != EdgeVertex2Index) && (m_VertexList.PrevItemIndex(EdgeVertex1Index) != EdgeVertex2Index))
            {
                #warning �� ���� ������ ���� ���������� ������� ������ ArgumentException
                throw new ArgumentException("EdgeVertex1 and EdgeVertex2 must form edge");
            }

            for (Int32 SideIndex = 0; SideIndex < EdgeVertex1.SideCount; SideIndex++)
            {
                if (EdgeVertex1[SideIndex].m_VertexList.IndexOf(EdgeVertex2) == -1)
                {
                    continue;
                }
                else if (Object.ReferenceEquals(EdgeVertex1[SideIndex], this))
                {
                    continue;
                }
                else
                {
                    NeighbourSide = EdgeVertex1[SideIndex];
                    break;
                }
            }

            return NeighbourSide;
        }

        /// <summary>
        /// ��������-���������� ��� ������� (������) � ������� �� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="VertexIndex">������ ������� � ������ ������</param>
        /// <returns></returns>
        public VertexClass this[Int32 VertexIndex]
        {
            get
            {
                return m_VertexList[VertexIndex];
            }
            /*set
            {
            }*/
        }

        /// <summary>
        /// �������� VertexCount ���������� ���������� ������ � ������ ������, ������� ����������� ������ �����
        /// </summary>
        public Int32 VertexCount
        {
            get
            {
                return m_VertexList.Count;
            }
        }

        /// <summary>
        /// SideNormal - �������� ��� ������� (������) � "�������" ������� �����
        /// </summary>
        public Vector3D SideNormal
        {
            get
            {
                return m_SideNormal;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PolyhedronClass
    {
    }
}
