using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// фабрика для построения графа выпуклого многогранника по его структуре (набору вершин, ребер, граней)
    /// </summary>
    public class Polyhedron3DGraphFactory
    {
        /// <summary>
        /// конструктор класса Polyhedron3DGraphFactory
        /// </summary>
        public Polyhedron3DGraphFactory()
        {
        }

        /// <summary>
        /// фабричный метод для построения графа выпуклого многогранника по его структуре (набору вершин, ребер, граней)
        /// </summary>
        /// <param name="polyhedron">выпуклый 3-мерный многогранник</param>
        /// <returns>построенный граф выпуклого многогранника</returns>
        public Polyhedron3DGraph CreatePolyhedronGraph(Polyhedron3D polyhedron)
        {
            List<Polyhedron3DGraphNode> nodeList = new List<Polyhedron3DGraphNode>();

            // Построение узлов графа :
            // Цикл по всем граням из списка граней
            for (Int32 sideIndex = 0; sideIndex < polyhedron.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = polyhedron.SideList[sideIndex];
                
                Polyhedron3DGraphNode currentNode = new Polyhedron3DGraphNode(currentSide.ID, currentSide.SideNormal);
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
                currentNode.SupportFuncValue = (currentSide.VertexList[0].XCoord * currentSide.SideNormal.XCoord +
                                                currentSide.VertexList[0].YCoord * currentSide.SideNormal.YCoord +
                                                currentSide.VertexList[0].ZCoord * currentSide.SideNormal.ZCoord);

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
                    PolyhedronSide3D neighbourSide = currentSide.GetNeighbourSide(leftEdgeVertex, rightEdgeVertex);

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
    }
}
