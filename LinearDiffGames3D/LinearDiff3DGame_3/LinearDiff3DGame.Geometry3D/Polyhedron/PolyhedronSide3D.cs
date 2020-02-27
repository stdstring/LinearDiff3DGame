using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ������������ ����� 3-������� �������������
    /// </summary>
    public class PolyhedronSide3D
    {
        /// <summary>
        /// ����������� ������ PolyhedronSide3D
        /// </summary>
        /// <param name="sideID">ID �����</param>
        /// <param name="sideNormal">������� ������� � �����</param>
        public PolyhedronSide3D(Int32 sideID, Vector3D sideNormal)
        {
            ID = sideID;
            m_VertexList = new CyclicList<PolyhedronVertex3D>();
            m_SideNormal = sideNormal;
        }

        /// <summary>
        /// ����������� ������ PolyhedronSide3D
        /// </summary>
        /// <param name="vertexList">������ ������</param>
        /// <param name="sideID">ID �����</param>
        /// <param name="sideNormal">������� ������� � �����</param>
        public PolyhedronSide3D(IList<PolyhedronVertex3D> vertexList, Int32 sideID, Vector3D sideNormal)
        {
            ID = sideID;

            m_VertexList = new CyclicList<PolyhedronVertex3D>();
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                m_VertexList.Add(vertexList[vertexIndex]);
            }

            m_SideNormal = sideNormal;
        }

        /// <summary>
        /// ID - ���������� ������������� �����
        /// </summary>
        public readonly Int32 ID;

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

        /* ��� �� ���� ������  ... � ���� �� �������� ����������������� ������ ������ ������ ����� � ������� ������ ������, ������� ����������� ������ ����� */
#warning Incorrect architecture

        /// <summary>
        /// ������ ������, ������� ����������� ������ �����
        /// </summary>
        public ICyclicList<PolyhedronVertex3D> VertexList
        {
            get
            {
                return m_VertexList;
            }
        }

        /*/// <summary>
        /// ��������-���������� ��� ������� (������) � ������� �� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="index">������ ������� � ������ ������</param>
        /// <returns>���� �� ������ �� ������ ������</returns>
        public PolyhedronVertex3D this[Int32 index]
        {
            get
            {
                return m_VertexList[index];
            }
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
        /// ����� AddVertex ��������� ������� item � ����� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="item">����������� �������</param>
        public void AddVertex(PolyhedronVertex3D item)
        {
            m_VertexList.Add(item);
        }

        /// <summary>
        /// ����� InsertVertex ��������� ������� item � ������ ������ �� ������� index, ������� ����������� ������ �����
        /// </summary>
        /// <param name="index">������ ����������� �������</param>
        /// <param name="item">����������� �������</param>
        public void InsertVertex(Int32 index, PolyhedronVertex3D item)
        {
            m_VertexList.Insert(index, item);
        }

        /// <summary>
        /// ����� RemoveVertex ������� ������� item �� ������ ������, ������� ����������� ������ �����
        /// </summary>
        /// <param name="item">��������� �������</param>
        public void RemoveVertex(PolyhedronVertex3D item)
        {
            m_VertexList.Remove(item);
        }*/
        /* ��� �� ���� ������  ... � ���� �� �������� ����������������� ������ ������ ������ ����� � ������� ������ ������, ������� ����������� ������ ����� */

        /// <summary>
        /// ����� HasVertex ��������� ����������� �� ������� vertex ������ ����� (��������� �� � ������ ������ ������ �����)
        /// </summary>
        /// <param name="vertex">����������� �������</param>
        /// <returns>true, ���� ������� vertex ����������� ������ �����; ����� - false</returns>
        public Boolean HasVertex(PolyhedronVertex3D vertex)
        {
            return (m_VertexList.IndexOf(vertex) != -1);
        }

        /// <summary>
        /// ����� GetNeighbourSide ���������� ��� ������ ����� ������ �� �����, ������������� ��������� edgeVertex1 � edgeVertex2
        /// </summary>
        /// <param name="edgeVertex1">������ ������� �����</param>
        /// <param name="edgeVertex2">������ ������� �����</param>
        /// <returns>����� �� ����� ��� ������ �����</returns>
        public PolyhedronSide3D GetNeighbourSide(PolyhedronVertex3D edgeVertex1, PolyhedronVertex3D edgeVertex2)
        {
            PolyhedronSide3D neighbourSide = null;

            // �������� �� ��, ��� edgeVertex1 � edgeVertex2 ����������� ������� �����
            if (!m_VertexList.Contains(edgeVertex1) || !m_VertexList.Contains(edgeVertex2))
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must belong this (current) side");
            }
            // �������� �� ��, ��� edgeVertex1 � edgeVertex2 �������� �����
            if (!Object.ReferenceEquals(m_VertexList.GetNextItem(edgeVertex1), edgeVertex2) &&
                !Object.ReferenceEquals(m_VertexList.GetPrevItem(edgeVertex1), edgeVertex2))
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must form an edge");
            }

            // ���� �� ���� ������, ������� ����������� ������� edgeVertex1
            for (Int32 sideIndex = 0; sideIndex < edgeVertex1.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = edgeVertex1.SideList[sideIndex];

                if (!currentSide.VertexList.Contains(edgeVertex2))
                {
                    continue;
                }
                else if (Object.ReferenceEquals(currentSide, this))
                {
                    continue;
                }
                else
                {
                    neighbourSide = currentSide;
                    break;
                }
            }
            // ���� �� ���� ������, ������� ����������� ������� edgeVertex1

            return neighbourSide;
        }

        /// <summary>
        /// m_VertexList - ������ ������, ������� ����������� ������ �����
        /// </summary>
        private CyclicList<PolyhedronVertex3D> m_VertexList;
        /// <summary>
        /// m_SideNormal - "�������" ������� � ����� (����������� �����)
        /// </summary>
        private Vector3D m_SideNormal;
    }
}
