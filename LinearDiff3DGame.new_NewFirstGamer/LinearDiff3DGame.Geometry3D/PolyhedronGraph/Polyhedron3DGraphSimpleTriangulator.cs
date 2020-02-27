using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    /// <summary>
    /// класс для триангуляции графа выпуклого 3-мерного многогранника
    /// </summary>
    public class Polyhedron3DGraphSimpleTriangulator
    {
        /// <summary>
        /// операция триангуляции графа graph, построенного по структуре выпуклого 3-х мерного многогранника
        /// при этом предполагается (и никак не проверяется !!!), что все сектроры в графе - выпуклые
        /// </summary>
        /// <param name="graph">исходный граф</param>
        /// <returns>граф после операции триангуляции</returns>
        public Polyhedron3DGraph Triangulate(Polyhedron3DGraph graph)
        {
            IList<Polyhedron3DGraphNode> nodeList = graph.NodeList;

            // Цикл по всем узлам графа из списка узлов графа
            for (Int32 nodeIndex = 0; nodeIndex < nodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = nodeList[nodeIndex];

                // Цикл по всем связям текущего узла
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count;)
                {
                    // текущая и следующая связи (узлы)
                    Polyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];
                    Polyhedron3DGraphNode nextConn = currentNode.ConnectionList.GetNextItem(connIndex);

                    // строим наикратчайший путь обхода, начинающийся на текущей связи и заканчивающийся на следующей связи
                    List<Polyhedron3DGraphNode> shortestGraphPath = GetShortestGraphPath(currentNode, currentConn,
                                                                                         nextConn);

                    // если число узлов в построенном пути < 2, то это ошибка !!!!!!
                    if (shortestGraphPath.Count < 2)
                    {
#warning может более специализированное исключение
                        throw new Exception("Error at construction of the graph's path !!!");
                    }

                    // цикл со 2-го по N-1 узел из построенного пути обхода
                    // если число узлов в построенном пути = 2, то цикл выполнится 0 раз
                    for (Int32 graphPathIndex = 1; graphPathIndex < shortestGraphPath.Count - 1; graphPathIndex++)
                    {
                        // в список связей текущего узла добавляем связь (между текущим узлом и i-м узлом из построенного пути обхода) за последней добавленной связью или текущей, если добавлений не было
                        currentNode.ConnectionList.Insert(connIndex + 1, shortestGraphPath[graphPathIndex]);

                        // в список связей i-го узла из построенного пути обхода добавляем (эту же) связь перед связью, по которой мы пришли в этот узел
                        Int32 index =
                            shortestGraphPath[graphPathIndex].ConnectionList.IndexOf(
                                shortestGraphPath[graphPathIndex - 1]);
                        shortestGraphPath[graphPathIndex].ConnectionList.Insert(index, currentNode);

                        connIndex++;
                    }
                    // цикл со 2-го по N-1 узел из построенного пути обхода

                    connIndex++;
                }
                // Цикл по всем связям текущего узла
            }
            // Цикл по всем узлам графа из списка узлов графа

            // возвращаем граф после триангуляции
            return graph;
        }

        /// <summary>
        /// построения наикратчайшего пути обхода по графу по заданному началу и концу пути
        /// </summary>
        /// <param name="startNode">исходный узел</param>
        /// <param name="startConn">связь - начало пути</param>
        /// <param name="finishConn">связь - конец пути</param>
        /// <returns>наикратчайший путь обхода по графу по заданному началу и концу пути</returns>
        private List<Polyhedron3DGraphNode> GetShortestGraphPath(Polyhedron3DGraphNode startNode,
                                                                 Polyhedron3DGraphNode startConn,
                                                                 Polyhedron3DGraphNode finishConn)
        {
            List<Polyhedron3DGraphNode> shortestGraphPath = new List<Polyhedron3DGraphNode>();

            // рассматриваемый узел
            Polyhedron3DGraphNode currentNode = startConn;
            // узел, из которого мы пришли в рассматриваемый
            Polyhedron3DGraphNode prevNode = startNode;

            // пока рассматриваемый узел не совпадет с исходным узлом
            while (currentNode != startNode)
            {
                // Добавляем рассматриваемый узел в конец списка узлов, составляющих путь обхода
                shortestGraphPath.Add(currentNode);

                // индекс связи (узла) по которой мы пришли в рассматриваемый узел
                Int32 connFromIndex = currentNode.ConnectionList.IndexOf(prevNode);
                // индекс связи (узла) предыдущей для той, по которой мы пришли в рассматриваемый узел
                Int32 connToIndex = currentNode.ConnectionList.GetPrevItemIndex(connFromIndex);
                prevNode = currentNode;
                currentNode = currentNode.ConnectionList[connToIndex];

                /*// В качестве связи, для нахождения нового рассматриваемого узла, берем связь,
                // которая будет предыдущей (в списке связей текущего рассматриваемого узла) для связи,
                // по которой мы пришли в текущий рассматриваемый узел
                Polyhedron3DGraphNode temp = currentNode.ConnectionList.GetPrevItem(prevNode);
                // Узел, с которым связан текущий рассматриваемый узел полученной (см. выше) связью,
                // становится новым рассматриваемым узлом
                prevNode = currentNode;
                currentNode = temp;*/
            }
            // пока рассматриваемый узел не совпадет с исходным узлом

            if (prevNode != finishConn)
            {
#warning может более специализированное исключение
                throw new Exception("Error at construction of the graph's path !!!");
            }

            return shortestGraphPath;
        }
    }
}