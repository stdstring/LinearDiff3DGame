using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ������������ 3-������ ������������
    /// </summary>
    public class Polyhedron3D
    {
        /// <summary>
        /// ����������� Polyhedron3D
        /// </summary>
        /// <param name="sideList">������ ������ �������������</param>
        /// <param name="vertexList">������ ������ �������������</param>
        public Polyhedron3D(List<PolyhedronSide3D> sideList, List<PolyhedronVertex3D> vertexList)
        {
            m_SideList = new List<PolyhedronSide3D>(sideList);
            m_VertexList = new List<PolyhedronVertex3D>(vertexList);
        }

        /* ��� �� ���� ������  ... � ���� �� �������� ����������������� ������ ������ � ������ ������ ������� ������������� */
#warning Incorrect architecture

        /// <summary>
        /// ������ ������ �������������
        /// </summary>
        public IList<PolyhedronSide3D> SideList
        {
            get
            {
                return m_SideList;
            }
        }

        /// <summary>
        /// ������ ������ �������������
        /// </summary>
        public IList<PolyhedronVertex3D> VertexList
        {
            get
            {
                return m_VertexList;
            }
        }
        /* ��� �� ���� ������  ... � ���� �� �������� ����������������� ������ ������ � ������ ������ ������� ������������� */

        /// <summary>
        /// ������ ������ �������������
        /// </summary>
        private List<PolyhedronSide3D> m_SideList;
        /// <summary>
        /// ������ ������ �������������
        /// </summary>
        private List<PolyhedronVertex3D> m_VertexList;
    }
}
