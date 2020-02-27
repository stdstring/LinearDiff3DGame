using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    public class Polyhedron3DGraphFactory
    {
        // ��������� ����� ��� ���������� ����� ��������� ������������� �� ��� ��������� (������ ������, �����, ������)
        public IPolyhedron3DGraph CreatePolyhedronGraph(IPolyhedron3D polyhedron)
        {
            VertexSidesDictionary vertexSidesDict = VertexSidesDictionary.Create(polyhedron);

            List<IPolyhedron3DGraphNode> nodeList = new List<IPolyhedron3DGraphNode>();

            // ���������� ����� ����� :
            // ���� �� ���� ������ �� ������ ������
            for(Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                IPolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];

                IPolyhedron3DGraphNode currentNode = new Polyhedron3DGraphNode(currentSide.ID, 0, currentSide.SideNormal);
#warning ����� ����� !!!!!! ��������� ������������ ��������� �������� ������� �������
                currentNode.SupportFuncValue = (currentSide.VertexList[0].XCoord*currentSide.SideNormal.X +
                                                currentSide.VertexList[0].YCoord*currentSide.SideNormal.Y +
                                                currentSide.VertexList[0].ZCoord*currentSide.SideNormal.Z);

                nodeList.Add(currentNode);
            }
            // ���� �� ���� ������ �� ������ ������

            // ���������� ������ ����� ������ ����� :
            // ���� �� ���� ������ �� ������ ������
            for(Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                IPolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];

                // ���� �� ���� ������ ������� �����
                for(Int32 sideVertexIndex = 0; sideVertexIndex < currentSide.VertexList.Count; ++sideVertexIndex)
                {
                    // ��� �������� ����� ������� ����� ���������, �������� �� �������
                    IPolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[sideVertexIndex];
                    IPolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(sideVertexIndex);
                    IPolyhedronSide3D neighbourSide = GetNeighbourSide(currentSide, leftEdgeVertex, rightEdgeVertex,
                                                                       vertexSidesDict);

                    Int32 currentNodeIndex = currentSide.ID;
                    Int32 neighbourNodeIndex = neighbourSide.ID;
                    IPolyhedron3DGraphNode currentNode = nodeList[currentNodeIndex];
                    IPolyhedron3DGraphNode neighbourNode = nodeList[neighbourNodeIndex];

                    // � ����� ������ �����, � �������� ������ ������ ����, ����������� ������ �� ����, ��������������� ��������� �����
                    currentNode.ConnectionList.Add(neighbourNode);
                }
                // ���� �� ���� ������ ������� �����
            }
            // ���� �� ���� ������ �� ������ ������

            return new Polyhedron3DGraph(nodeList);
        }

        private static IPolyhedronSide3D GetNeighbourSide(IPolyhedronSide3D side,
                                                          IPolyhedronVertex3D edgeVertex1,
                                                          IPolyhedronVertex3D edgeVertex2,
                                                          VertexSidesDictionary vertexSidesDict)
        {
            IPolyhedronSide3D neighbourSide = null;

            // �������� �� ��, ��� edgeVertex1 � edgeVertex2 ����������� ������� �����
            Debug.Assert(side.VertexList.Contains(edgeVertex1) && side.VertexList.Contains(edgeVertex2),
                         "edgeVertex1 and edgeVertex2 must belong this (current) side");
            /*if (!side.VertexList.Contains(edgeVertex1) || !side.VertexList.Contains(edgeVertex2))
            {
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must belong this (current) side");
            }*/
            // �������� �� ��, ��� edgeVertex1 � edgeVertex2 �������� �����
            Debug.Assert(ReferenceEquals(side.VertexList.GetNextItem(edgeVertex1), edgeVertex2) ||
                         ReferenceEquals(side.VertexList.GetPrevItem(edgeVertex1), edgeVertex2),
                         "edgeVertex1 and edgeVertex2 must form an edge");
            /*if (!ReferenceEquals(side.VertexList.GetNextItem(edgeVertex1), edgeVertex2) &&
                !ReferenceEquals(side.VertexList.GetPrevItem(edgeVertex1), edgeVertex2))
            {
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must form an edge");
            }*/

            IList<IPolyhedronSide3D> edge1SideList = vertexSidesDict.GetSideList4Vertex(edgeVertex1);
            // ���� �� ���� ������, ������� ����������� ������� edgeVertex1
            for(Int32 sideIndex = 0; sideIndex < edge1SideList.Count; ++sideIndex)
            {
                IPolyhedronSide3D currentSide = edge1SideList[sideIndex];

                if(!currentSide.VertexList.Contains(edgeVertex2))
                {
                    continue;
                }
                if(ReferenceEquals(currentSide, side))
                {
                    continue;
                }
                neighbourSide = currentSide;
                break;
            }
            // ���� �� ���� ������, ������� ����������� ������� edgeVertex1

            return neighbourSide;
        }
    }
}