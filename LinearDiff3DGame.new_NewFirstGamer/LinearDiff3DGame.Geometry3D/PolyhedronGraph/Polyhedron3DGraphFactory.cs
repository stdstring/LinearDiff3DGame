using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    /// <summary>
    /// фабрика для построения графа выпуклого многогранника по его структуре (набору вершин, ребер, граней)
    /// </summary>
    public class Polyhedron3DGraphFactory
    {
        /// <summary>
        /// фабричный метод для построения графа выпуклого многогранника по его структуре (набору вершин, ребер, граней)
        /// </summary>
        /// <param name="polyhedron">выпуклый 3-мерный многогранник</param>
        /// <returns>построенный граф выпуклого многогранника</returns>
        public Polyhedron3DGraph CreatePolyhedronGraph(Polyhedron3D polyhedron)
        {
            VertexSidesDictionary vertexSidesDict = VertexSidesDictionary.Create(polyhedron);

            List<Polyhedron3DGraphNode> nodeList = new List<Polyhedron3DGraphNode>();

            // Построение узлов графа :
            // Цикл по всем граням из списка граней
            for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];

                Polyhedron3DGraphNode currentNode = new Polyhedron3DGraphNode(currentSide.ID, 0, currentSide.SideNormal);
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
                currentNode.SupportFuncValue = (currentSide.VertexList[0].XCoord*currentSide.SideNormal.XCoord +
                                                currentSide.VertexList[0].YCoord*currentSide.SideNormal.YCoord +
                                                currentSide.VertexList[0].ZCoord*currentSide.SideNormal.ZCoord);

                nodeList.Add(currentNode);
            }
            // Цикл по всем граням из списка граней

            // Построение связей между узлами графа :
            // Цикл по всем граням из списка граней
            for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];

                // Цикл по всем ребрам текущей грани
                for (Int32 sideVertexIndex = 0; sideVertexIndex < currentSide.VertexList.Count; ++sideVertexIndex)
                {
                    // Для текущего ребра находим грань владельца, отличную от текущей
                    PolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[sideVertexIndex];
                    PolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(sideVertexIndex);
                    PolyhedronSide3D neighbourSide = GetNeighbourSide(currentSide, leftEdgeVertex, rightEdgeVertex,
                                                                      vertexSidesDict);

                    Int32 currentPGNodeIndex = currentSide.ID;
                    Int32 neighbourPGNodeIndex = neighbourSide.ID;
                    Polyhedron3DGraphNode currentPGNode = nodeList[currentPGNodeIndex];
                    Polyhedron3DGraphNode neighbourPGNode = nodeList[neighbourPGNodeIndex];

                    // В конец списка узлов, с которыми связан данный узел, добавляется ссылка на узел, соответствующий найденной грани
                    currentPGNode.ConnectionList.Add(neighbourPGNode);
                }
                // Цикл по всем ребрам текущей грани
            }
            // Цикл по всем граням из списка граней

            return new Polyhedron3DGraph(nodeList);
        }

        private static PolyhedronSide3D GetNeighbourSide(PolyhedronSide3D side, PolyhedronVertex3D edgeVertex1,
                                                         PolyhedronVertex3D edgeVertex2,
                                                         VertexSidesDictionary vertexSidesDict)
        {
            PolyhedronSide3D neighbourSide = null;

            // проверка на то, что edgeVertex1 и edgeVertex2 принадлежат текущей грани
            Debug.Assert(side.VertexList.Contains(edgeVertex1) && side.VertexList.Contains(edgeVertex2),
                         "edgeVertex1 and edgeVertex2 must belong this (current) side");
            /*if (!side.VertexList.Contains(edgeVertex1) || !side.VertexList.Contains(edgeVertex2))
            {
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must belong this (current) side");
            }*/
            // проверка на то, что edgeVertex1 и edgeVertex2 образуют ребро
            Debug.Assert(ReferenceEquals(side.VertexList.GetNextItem(edgeVertex1), edgeVertex2) ||
                         ReferenceEquals(side.VertexList.GetPrevItem(edgeVertex1), edgeVertex2),
                         "edgeVertex1 and edgeVertex2 must form an edge");
            /*if (!ReferenceEquals(side.VertexList.GetNextItem(edgeVertex1), edgeVertex2) &&
                !ReferenceEquals(side.VertexList.GetPrevItem(edgeVertex1), edgeVertex2))
            {
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must form an edge");
            }*/

            IList<PolyhedronSide3D> edge1SideList = vertexSidesDict.GetSideList4Vertex(edgeVertex1);
            // Цикл по всем граням, которым принадлежит вершина edgeVertex1
            for (Int32 sideIndex = 0; sideIndex < edge1SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = edge1SideList[sideIndex];

                if (!currentSide.VertexList.Contains(edgeVertex2))
                {
                    continue;
                }
                if (ReferenceEquals(currentSide, side))
                {
                    continue;
                }
                neighbourSide = currentSide;
                break;
            }
            // Цикл по всем граням, которым принадлежит вершина edgeVertex1

            return neighbourSide;
        }
    }
}