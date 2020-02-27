using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// ������� ��� ��������� ��������� (������, �����) ��������� ������������� �� ��������� �����
    /// </summary>
    public class Polyhedron3DFromGraphFactory
    {
        /// <summary>
        /// ����������� ������ Polyhedron3DFromGraphFactory
        /// </summary>
        /// <param name="approxComparer">������������, ��� ������������� ��������� �������������� �����</param>
        public Polyhedron3DFromGraphFactory(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// ��������� ����� ��� ��������� ��������� (������, �����) ��������� ������������� �� ��������� �����
        /// </summary>
        /// <param name="graph">����</param>
        /// <returns>���������� �������� ������������</returns>
        public Polyhedron3D CreatePolyhedron(Polyhedron3DGraph graph)
        {
            List<Plane3D> planeList = new List<Plane3D>(graph.NodeList.Count);

            // ���� �� ���� ����� ����� �� ������ ����� �����
            for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                // ������� ���� ����� ������������� �����
                // ��� "��������������" ���� ����� ������� ���� ����� ������������ ���������, � ������� ����� ������� �����
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];

                // ��������� ������� ������� ��� �������� ���� ����� (�� ������ �������� ����)
                Vector3D planeNormal = currentNode.NodeNormal;
                // �������� ������� ������� ��� �������� ����
                Double supportFuncValue = currentNode.SupportFuncValue;

                // ���������� ��������� �� ����������� ������� ������� � �������� ������� �������
                // planeNormal = {A;B;C}, supportFuncValue + D = 0
                planeList.Add(new Plane3D(planeNormal.XCoord, planeNormal.YCoord, planeNormal.ZCoord, -supportFuncValue));
            }
            // ���� �� ���� ����� ����� �� ������ ����� �����

            // ��������� ������ ������ � ������ ������������� ������
            List<PolyhedronSide3D> sideList = new List<PolyhedronSide3D>();
            List<PolyhedronVertex3D> vertexList = new List<PolyhedronVertex3D>();

            // ���� �� ���� ����� ����� �� ������ ����� �����
            for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];

                // ��������� ��������� (����� 1) �� ������ ����������, ��������������� �������� ����
                Plane3D currentPlane = planeList[currentNode.ID];
                // �������� ����� (� ������ ������� ������), ������� � ���������� ���������
                PolyhedronSide3D currentSide = new PolyhedronSide3D(currentNode.ID, currentNode.NodeNormal);
                sideList.Add(currentSide);

                // ���� �� ���� ������ �� ������ ������ �������� ���� �����
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    // ��������� ���� ����� (����� 2) ��� ������� ����� � ���� ����� (����� 3) ��� ��������� �����
                    Polyhedron3DGraphNode currentConn = currentNode.ConnectionList[connIndex];
                    Polyhedron3DGraphNode nextConn = currentNode.ConnectionList.GetNextItem(connIndex);

                    // ��������� ���������� (����� 2 � 3) �� ������ ����������, ��������������� ���������� ����� (����� 2 � 3)
                    Plane3D currentConnPlane = planeList[currentConn.ID];
                    Plane3D nextConnPlane = planeList[nextConn.ID];

                    // ��������� ����� ����������� ���� ���������� (����� 1, 2 � 3)
                    Point3D planesCrossingPoint = CalcPlanesCrossingPoint(currentPlane, currentConnPlane, nextConnPlane);

                    // ��������, ������������ �� ���������� ����� � ������ ������
                    PolyhedronVertex3D calcVertex = FindVertex4Point(vertexList, planesCrossingPoint);
                    // ���� �������� ���������
                    if (Object.ReferenceEquals(calcVertex, null))
                    {
                        calcVertex = new PolyhedronVertex3D(planesCrossingPoint, vertexList.Count);
                        vertexList.Add(calcVertex);
                    }

                    // ��������, ��������� �� ���������� ������� � ������ ������ ������� ����� (���� � �����, ���� � ������)
                    if (currentSide.VertexList.Count == 0 ||
                        (!Object.ReferenceEquals(currentSide.VertexList[0], calcVertex) && 
                         !Object.ReferenceEquals(currentSide.VertexList[currentSide.VertexList.Count - 1], calcVertex)))
                    {
                        currentSide.VertexList.Add(calcVertex);
                    }
                }
                // ���� �� ���� ������ �� ������ ������ �������� ���� �����
            }
            // ���� �� ���� ����� ����� �� ������ ����� �����

            return new Polyhedron3D(sideList, vertexList);
        }

        /// <summary>
        /// ���������� ����� ����������� ���� ����������
        /// </summary>
        /// <param name="plane1">1-� ���������</param>
        /// <param name="plane2">2-� ���������</param>
        /// <param name="plane3">3-� ���������</param>
        /// <returns>����� ����������� ���� ����������</returns>
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
        /// ��������, ������������ �� ����������� ����� � ������ ������
        /// </summary>
        /// <param name="vertexList">������ ������</param>
        /// <param name="point">����������� �����</param>
        /// <returns>true, ���� ����������� ����� ������������ � ������; ����� - false</returns>
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
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }
}
