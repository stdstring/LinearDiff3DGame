using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ����, �������������� ��������� 3-������� ������������� (��. �������� ���������� ������������ ���������� ������ ...)
    /// </summary>
    public class Polyhedron3DGraph
    {
        /// <summary>
        /// ����������� ������ Polyhedron3DGraph
        /// </summary>
        /// <param name="nodeList">������ ����� ����� (������ ���� �������� � ���� �� ����� � ������� ������)</param>
        public Polyhedron3DGraph(List<Polyhedron3DGraphNode> nodeList)
        {
            m_PGNodeList = new List<Polyhedron3DGraphNode>(nodeList);
        }

        /// <summary>
        /// ������ ����� ����� (������ ���� �������� � ���� �� ����� � ������� ������)
        /// </summary>
        public IList<Polyhedron3DGraphNode> NodeList
        {
            get
            {
                return m_PGNodeList;
            }
        }

        /// <summary>
        /// ������ ����� ����� (������ ���� �������� � ���� �� ����� � ������� ������)
        /// </summary>
        private List<Polyhedron3DGraphNode> m_PGNodeList;
    }
}
