using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ������� ��� ���������� ����� ��������� ������������� �� ��� ��������� (������ ������, �����, ������)
    /// </summary>
    public class Polyhedron3DGraphFactory
    {
        /// <summary>
        /// ����������� ������ Polyhedron3DGraphFactory
        /// </summary>
        public Polyhedron3DGraphFactory()
        {
        }

        /// <summary>
        /// ��������� ����� ��� ���������� ����� ��������� ������������� �� ��� ��������� (������ ������, �����, ������)
        /// </summary>
        /// <param name="polyhedron">�������� 3-������ ������������</param>
        /// <returns>����������� ���� ��������� �������������</returns>
        public Polyhedron3DGraph CreatePolyhedronGraph(Polyhedron3D polyhedron)
        {
            List<Polyhedron3DGraphNode> nodeList = new List<Polyhedron3DGraphNode>();

            // ���������� ����� ����� :
            // ���� �� ���� ������ �� ������ ������
            for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];
                
                Polyhedron3DGraphNode currentNode = new Polyhedron3DGraphNode(currentSide.ID, currentSide.SideNormal);
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
                currentNode.SupportFuncValue = (currentSide.VertexList[0].XCoord * currentSide.SideNormal.XCoord +
                                                currentSide.VertexList[0].YCoord * currentSide.SideNormal.YCoord +
                                                currentSide.VertexList[0].ZCoord * currentSide.SideNormal.ZCoord);

                nodeList.Add(currentNode);
            }
            // ���� �� ���� ������ �� ������ ������

            // ���������� ������ ����� ������ ����� :
            // ���� �� ���� ������ �� ������ ������
            for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];

                // ���� �� ���� ������ ������� �����
                for (Int32 sideVertexIndex = 0; sideVertexIndex < currentSide.VertexList.Count; ++sideVertexIndex)
                {
                    // ��� �������� ����� ������� ����� ���������, �������� �� �������
                    PolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[sideVertexIndex];
                    PolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(sideVertexIndex);
                    PolyhedronSide3D neighbourSide = currentSide.GetNeighbourSide(leftEdgeVertex, rightEdgeVertex);

                    Int32 currentPGNodeIndex = currentSide.ID;
                    Int32 neighbourPGNodeIndex = neighbourSide.ID;
                    Polyhedron3DGraphNode currentPGNode = nodeList[currentPGNodeIndex];
                    Polyhedron3DGraphNode neighbourPGNode = nodeList[neighbourPGNodeIndex];

                    // � ����� ������ �����, � �������� ������ ������ ����, ����������� ������ �� ����, ��������������� ��������� �����
                    currentPGNode.ConnectionList.Add(neighbourPGNode);
                }
                // ���� �� ���� ������ ������� �����
            }
            // ���� �� ���� ������ �� ������ ������

            return new Polyhedron3DGraph(nodeList);
        }
    }
}
