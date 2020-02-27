using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.Geometry3D.PolyhedronFactory
{
    public class Polyhedron3DFromGraphFactory
    {
        public Polyhedron3DFromGraphFactory(ApproxComp approxComparer, ILinearEquationsSystemSolver lesSolver)
        {
            //this.approxComparer = approxComparer;
            this.lesSolver = lesSolver;
        }

        public IPolyhedron3D CreatePolyhedron(IPolyhedron3DGraph graph)
        {
            IDictionary<PointIndexTriplet, IPolyhedronVertex3D> calculatedVertexes =
                new Dictionary<PointIndexTriplet, IPolyhedronVertex3D>();
            IDictionary<Int32, Plane3D> planes = new Dictionary<Int32, Plane3D>(graph.NodeList.Count);

            graph.NodeList.Aggregate(planes, (accum, node) =>
                                                 {
                                                     Vector3D planeNormal = node.NodeNormal;
                                                     Double supportFuncValue = node.SupportFuncValue;
                                                     // planeNormal = {A;B;C}, supportFuncValue + D = 0
                                                     Plane3D plane = new Plane3D(planeNormal.X,
                                                                                 planeNormal.Y,
                                                                                 planeNormal.Z,
                                                                                 -supportFuncValue);
                                                     accum.Add(node.ID, plane);
                                                     return accum;
                                                 });

            List<IPolyhedronSide3D> sideList = new List<IPolyhedronSide3D>();
            List<IPolyhedronVertex3D> vertexList = new List<IPolyhedronVertex3D>();
#warning это внешний параметр
            ApproxComp vertexNearnessComparer = new ApproxComp(1e-9);
            foreach (IPolyhedron3DGraphNode node in graph.NodeList)
            {
                IList<IPolyhedronVertex3D> sideVertexList = new List<IPolyhedronVertex3D>();
                for (Int32 connIndex = 0; connIndex < node.ConnectionList.Count; ++connIndex)
                {
                    IPolyhedron3DGraphNode conn = node.ConnectionList[connIndex];
                    IPolyhedron3DGraphNode connNext = node.ConnectionList.GetNextItem(connIndex);
                    PointIndexTriplet triplet = PointIndexTriplet.GetProperPointIndexTriplet(node.ID,
                                                                                             conn.ID,
                                                                                             connNext.ID);
                    IPolyhedronVertex3D vertex;
                    if (!calculatedVertexes.TryGetValue(triplet, out vertex))
                    {
                        Plane3D plane1 = planes[node.ID];
                        Plane3D plane2 = planes[conn.ID];
                        Plane3D plane3 = planes[connNext.ID];
                        Point3D crossingPoint = CalcPlanesCrossingPoint(plane1, plane2, plane3);
                        vertex = FindVertex(calculatedVertexes.Values, crossingPoint, vertexNearnessComparer);
                        if (vertex == null)
                        {
                            Int32 vertexID = vertexList.Count == 0 ? 0 : vertexList[vertexList.Count - 1].ID + 1;
                            vertex = new PolyhedronVertex3D(crossingPoint, vertexID);
                            vertexList.Add(vertex);
                        }
                        calculatedVertexes.Add(triplet, vertex);
                    }
                    if (sideVertexList.IndexOf(vertex) == -1)
                        sideVertexList.Add(vertex);
                }
                //Debug.Assert(sideVertexList.Count > 2);
                Int32 sideID = sideList.Count == 0 ? 0 : sideList[sideList.Count - 1].ID + 1;
                IPolyhedronSide3D side = new PolyhedronSide3D(sideVertexList, sideID, node.NodeNormal);
                sideList.Add(side);
            }

            return new Polyhedron3D(sideList, vertexList);
        }

        //public IPolyhedron3D CreatePolyhedron2(Polyhedron3DGraph graph)
        //{
        //    List<Plane3D> planeList = new List<Plane3D>(graph.NodeList.Count);

        //    // ÷икл по всем узлам графа из списка узлов графа
        //    for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
        //    {
        //        // каждому узлу графа соответствует грань
        //        // дл€ "восстановлени€" этой грани каждому узлу графу сопостовл€ем плоскость, в которой лежит искома€ грань
        //        Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];

        //        // ѕолучение вектора нормали дл€ текущего узла графа (из данных текущего узла)
        //        Vector3D planeNormal = currentNode.NodeNormal;
        //        // значение опорной функции дл€ текущего узла
        //        Double supportFuncValue = currentNode.SupportFuncValue;

        //        // ѕостроение плоскости по полученному вектору нормали и значению опорной функции
        //        // planeNormal = {A;B;C}, supportFuncValue + D = 0
        //        planeList.Add(new Plane3D(planeNormal.X, planeNormal.Y, planeNormal.Z, -supportFuncValue));
        //    }
        //    // ÷икл по всем узлам графа из списка узлов графа

        //    // формируем список вершин и граней многогранника заново
        //    List<IPolyhedronSide3D> sideList = new List<IPolyhedronSide3D>();
        //    List<IPolyhedronVertex3D> vertexList = new List<IPolyhedronVertex3D>();

        //    // ÷икл по всем узлам графа из списка узлов графа
        //    for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
        //    {
        //        Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];

        //        // ѕолучение плоскости (номер 1) из списка плоскостей, соответствующей текущему узлу
        //        Plane3D currentPlane = planeList[currentNode.ID];
        //        // —оздание грани (с пустым списком вершин), лежащей в полученной плоскости
        //        IPolyhedronSide3D currentSide = new PolyhedronSide3D(currentNode.ID, currentNode.NodeNormal);
        //        sideList.Add(currentSide);

        //        // ÷икл по всем св€з€м из списка св€зей текущего узла графа
        //        for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
        //        {
        //            // ѕолучение узла графа (номер 2) дл€ текущей св€зи и узла графа (номер 3) дл€ следующей св€зи
        //            Polyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];
        //            Polyhedron3DGraphNode nextConn = currentNode.ConnectionList.GetNextItem(connIndex);

        //            // ѕолучение плоскостей (номер 2 и 3) из списка плоскостей, соответствующих полученным узлам (номер 2 и 3)
        //            Plane3D currentConnPlane = planeList[currentConn.ID];
        //            Plane3D nextConnPlane = planeList[nextConn.ID];

        //            // ѕолучение точки пересечени€ трех плоскостей (номер 1, 2 и 3)
        //            Point3D planesCrossingPoint = CalcPlanesCrossingPoint(currentPlane, currentConnPlane, nextConnPlane);

        //            // ѕроверка, присутствует ли полученна€ точка в списке вершин
        //            IPolyhedronVertex3D calcVertex = FindVertex4Point(vertexList, planesCrossingPoint);
        //            // ≈сли проверка неуспешна
        //            if (ReferenceEquals(calcVertex, null))
        //            {
        //                calcVertex = new PolyhedronVertex3D(planesCrossingPoint, vertexList.Count);
        //                vertexList.Add(calcVertex);
        //            }

        //            // ѕроверка, добавлена ли полученна€ вершина в список вершин текущей грани (либо в конец, либо в начало)
        //            if (currentSide.VertexList.Count == 0 ||
        //                (!ReferenceEquals(currentSide.VertexList[0], calcVertex) &&
        //                 !ReferenceEquals(currentSide.VertexList[currentSide.VertexList.Count - 1], calcVertex)))
        //                currentSide.VertexList.Add(calcVertex);
        //        }
        //        // ÷икл по всем св€з€м из списка св€зей текущего узла графа
        //    }
        //    // ÷икл по всем узлам графа из списка узлов графа

        //    return new Polyhedron3D(sideList, vertexList);
        //}

        // вычисление точки пересечени€ трех плоскостей
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

            Matrix solution = lesSolver.Solve(matrixA, matrixB);

            return new Point3D(solution[1, 1], solution[2, 1], solution[3, 1]);
        }

        //// проверка, присутствует ли провер€ема€ точка в списке вершин
        //private IPolyhedronVertex3D FindVertex4Point(IList<IPolyhedronVertex3D> vertexList, Point3D point)
        //{
        //    IPolyhedronVertex3D findingVertex = null;

        //    for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
        //    {
        //        IPolyhedronVertex3D currentVertex = vertexList[vertexIndex];

        //        if (approxComparer.EQ(point.X, currentVertex.XCoord) &&
        //            approxComparer.EQ(point.Y, currentVertex.YCoord) &&
        //            approxComparer.EQ(point.Z, currentVertex.ZCoord))
        //        {
        //            findingVertex = currentVertex;
        //            break;
        //        }
        //    }

        //    return findingVertex;
        //}

        public static IPolyhedronVertex3D FindVertex(ICollection<IPolyhedronVertex3D> vertexes,
                                                     Point3D crossingPoint,
                                                     ApproxComp approxComp)
        {
            return vertexes.FirstOrDefault(vertex => VertexApproxEqual(vertex, crossingPoint, approxComp));
        }

        //private static Double VertexApproxEqual(IPolyhedronVertex3D vertex1, IPolyhedronVertex3D vertex2,
        //                                        ApproxComp approxComp)
        //{
        //    Double squaredDistance = (vertex1.XCoord - vertex2.XCoord)*(vertex1.XCoord - vertex2.XCoord) +
        //                             (vertex1.YCoord - vertex2.YCoord)*(vertex1.YCoord - vertex2.YCoord) +
        //                             (vertex1.ZCoord - vertex2.ZCoord)*(vertex1.ZCoord - vertex2.ZCoord);
        //    return squaredDistance;
        //}

        private static Boolean VertexApproxEqual(IPolyhedronVertex3D vertex, Point3D point, ApproxComp approxComp)
        {
            Double squaredDistance = (vertex.XCoord - point.X)*(vertex.XCoord - point.X) +
                                     (vertex.YCoord - point.Y)*(vertex.YCoord - point.Y) +
                                     (vertex.ZCoord - point.Z)*(vertex.ZCoord - point.Z);
            return approxComp.EQ(squaredDistance, 0);
        }

        //private readonly ApproxComp approxComparer;

        private readonly ILinearEquationsSystemSolver lesSolver;

        private struct PointIndexTriplet
        {
            private PointIndexTriplet(Int32 index1, Int32 index2, Int32 index3)
                : this()
            {
                Index1 = index1;
                Index2 = index2;
                Index3 = index3;
            }

            public static PointIndexTriplet GetProperPointIndexTriplet(Int32 index1, Int32 index2, Int32 index3)
            {
                if (index1 == index2 || index2 == index3 || index3 == index1)
                    throw new ArgumentException("index1, index2, index3 must be different.");
                if (index1 < index2 && index2 < index3) return new PointIndexTriplet(index1, index2, index3);
                if (index1 < index3 && index3 < index2) return new PointIndexTriplet(index1, index3, index2);
                if (index2 < index1 && index1 < index3) return new PointIndexTriplet(index2, index1, index3);
                if (index2 < index3 && index3 < index1) return new PointIndexTriplet(index2, index3, index1);
                if (index3 < index1 && index1 < index2) return new PointIndexTriplet(index3, index1, index2);
                return new PointIndexTriplet(index3, index2, index1);
            }

            public Int32 Index1 { get; private set; }
            public Int32 Index2 { get; private set; }
            public Int32 Index3 { get; private set; }
        }
    }
}