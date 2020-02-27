using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// фабрика для получения структуры (граней, ребер) выпуклого многогранника по заданному графу
    /// </summary>
    public class Polyhedron3DFromGraphFactory
    {
        /// <summary>
        /// конструктор класса Polyhedron3DFromGraphFactory
        /// </summary>
        /// <param name="approxComparer">сравниватель, для приближенного сравнения действительных чисел</param>
        public Polyhedron3DFromGraphFactory(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// фабричный метод для получения структуры (граней, ребер) выпуклого многогранника по заданному графу
        /// </summary>
        /// <param name="graph">граф</param>
        /// <returns>полученный выпуклый многогранник</returns>
        public Polyhedron3D CreatePolyhedron(Polyhedron3DGraph graph)
        {
            List<Plane3D> planeList = new List<Plane3D>(graph.NodeList.Count);

            // Цикл по всем узлам графа из списка узлов графа
            for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                // каждому узлу графа соответствует грань
                // для "восстановления" этой грани каждому узлу графу сопостовляем плоскость, в которой лежит искомая грань
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];

                // Получение вектора нормали для текущего узла графа (из данных текущего узла)
                Vector3D planeNormal = currentNode.NodeNormal;
                // значение опорной функции для текущего узла
                Double supportFuncValue = currentNode.SupportFuncValue;

                // Построение плоскости по полученному вектору нормали и значению опорной функции
                // planeNormal = {A;B;C}, supportFuncValue + D = 0
                planeList.Add(new Plane3D(planeNormal.XCoord, planeNormal.YCoord, planeNormal.ZCoord, -supportFuncValue));
            }
            // Цикл по всем узлам графа из списка узлов графа

            // формируем список вершин и граней многогранника заново
            List<PolyhedronSide3D> sideList = new List<PolyhedronSide3D>();
            List<PolyhedronVertex3D> vertexList = new List<PolyhedronVertex3D>();

            // Цикл по всем узлам графа из списка узлов графа
            for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];

                // Получение плоскости (номер 1) из списка плоскостей, соответствующей текущему узлу
                Plane3D currentPlane = planeList[currentNode.ID];
                // Создание грани (с пустым списком вершин), лежащей в полученной плоскости
                PolyhedronSide3D currentSide = new PolyhedronSide3D(currentNode.ID, currentNode.NodeNormal);
                sideList.Add(currentSide);

                // Цикл по всем связям из списка связей текущего узла графа
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    // Получение узла графа (номер 2) для текущей связи и узла графа (номер 3) для следующей связи
                    Polyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];
                    Polyhedron3DGraphNode nextConn = currentNode.ConnectionList.GetNextItem(connIndex);

                    // Получение плоскостей (номер 2 и 3) из списка плоскостей, соответствующих полученным узлам (номер 2 и 3)
                    Plane3D currentConnPlane = planeList[currentConn.ID];
                    Plane3D nextConnPlane = planeList[nextConn.ID];

                    // Получение точки пересечения трех плоскостей (номер 1, 2 и 3)
                    Point3D planesCrossingPoint = CalcPlanesCrossingPoint(currentPlane, currentConnPlane, nextConnPlane);

                    // Проверка, присутствует ли полученная точка в списке вершин
                    PolyhedronVertex3D calcVertex = FindVertex4Point(vertexList, planesCrossingPoint);
                    // Если проверка неуспешна
                    if (Object.ReferenceEquals(calcVertex, null))
                    {
                        calcVertex = new PolyhedronVertex3D(planesCrossingPoint, vertexList.Count);
                        vertexList.Add(calcVertex);
                    }

                    // Проверка, добавлена ли полученная вершина в список вершин текущей грани (либо в конец, либо в начало)
                    if (currentSide.VertexList.Count == 0 ||
                        (!Object.ReferenceEquals(currentSide.VertexList[0], calcVertex) && 
                         !Object.ReferenceEquals(currentSide.VertexList[currentSide.VertexList.Count - 1], calcVertex)))
                    {
                        currentSide.VertexList.Add(calcVertex);
                    }
                }
                // Цикл по всем связям из списка связей текущего узла графа
            }
            // Цикл по всем узлам графа из списка узлов графа

            return new Polyhedron3D(sideList, vertexList);
        }

        /// <summary>
        /// вычисление точки пересечения трех плоскостей
        /// </summary>
        /// <param name="plane1">1-я плоскость</param>
        /// <param name="plane2">2-я плоскость</param>
        /// <param name="plane3">3-я плоскость</param>
        /// <returns>точка пересечения трех плоскостей</returns>
        private Point3D CalcPlanesCrossingPoint(Plane3D plane1, Plane3D plane2, Plane3D plane3)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = plane1.KoeffA;
            matrixA[1, 2] = plane1.KoeffB;
            matrixA[1, 3] = plane1.KoeffC;
            matrixA[2, 1] = plane2.KoeffA;
            matrixA[2, 2] = plane2.KoeffB;
            matrixA[2, 3] = plane2.KoeffC;
            matrixA[3, 1] = plane3.KoeffA;
            matrixA[3, 2] = plane3.KoeffB;
            matrixA[3, 3] = plane3.KoeffC;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = -plane1.KoeffD;
            matrixB[2, 1] = -plane2.KoeffD;
            matrixB[3, 1] = -plane3.KoeffD;

            Matrix matrixError = null;

            LESKramer3Solver solver = new LESKramer3Solver();
            Matrix solution = solver.Solve(matrixA, matrixB, out matrixError);

            return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
        }

        /// <summary>
        /// проверка, присутствует ли проверяемая точка в списке вершин
        /// </summary>
        /// <param name="vertexList">список вершин</param>
        /// <param name="point">проверяемая точка</param>
        /// <returns>true, если проверяемая точка присутствует в списке; иначе - false</returns>
        private PolyhedronVertex3D FindVertex4Point(List<PolyhedronVertex3D> vertexList, Point3D point)
        {
            PolyhedronVertex3D findingVertex = null;

            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D currentVertex = vertexList[vertexIndex];

                if (m_ApproxComparer.EQ(point.XCoord, currentVertex.XCoord) &&
                    m_ApproxComparer.EQ(point.YCoord, currentVertex.YCoord) &&
                    m_ApproxComparer.EQ(point.ZCoord, currentVertex.ZCoord))
                {
                    findingVertex = currentVertex;
                    break;
                }
            }

            return findingVertex;
        }

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }
}
