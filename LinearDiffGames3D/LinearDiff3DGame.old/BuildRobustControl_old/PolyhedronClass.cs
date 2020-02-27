using System;
using System.Collections.Generic;
using System.Text;
using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    /// <summary>
    /// ����� PolyhedronClass ������������ �������� 3D ������������
    /// </summary>
    public class PolyhedronClass
    {
        /// <summary>
        /// ����� IsPointInside ���������, �������� �� ����� point ���������� ��� ������� �������������
        /// </summary>
        /// <param name="point">��������������� �����</param>
        /// <returns>true, ���� ����� point ��������� ������ ������� �������������; ����� false</returns>
        public Boolean IsPointInside(Point3D point)
        {
            // ��������� ��������� �� ����� ������ ��� ������� ������������� ����� ��������� �������:
            // ������ ������-������ �� ����������� ����� � ����� ����� (�������� � ����� �������) ������ �����
            // ���� ��������� ������������ ����� ������ �������� � ������� �������� � ��������������� ������
            // >= 0 ��� ������ �����, �� ������ ����� ���������� (��� ����� �� ������� �������������);
            // ����� ����� ����� ������� �������������
            foreach (SideClass currentSide in m_SideList)
            {
                Vector3D raduisVector = new Vector3D(currentSide[0].XCoord - point.XCoord,
                                                     currentSide[0].YCoord - point.YCoord,
                                                     currentSide[0].ZCoord - point.ZCoord);
                Double scalarProduct = Vector3D.ScalarProduct(raduisVector, currentSide.SideNormal);
                // �������, ����� ����� ���� ����� ���������� ... ��� ����� ��������� ������������ ������ ������ ���� > 0
                // if (scalarProdact <= 0)
                if (scalarProduct <= m_Epsilon)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// ����� IsPolyhedronInside ���������, ����� �� �������� ������������ checkedPolyhedron ������ �������
        /// </summary>
        /// <param name="checkedPolyhedron">����������� ������������</param>
        /// <returns>true, ���� �������� ������������ checkedPolyhedron ����� ������ ������� ; ����� - false</returns>
        public Boolean IsPolyhedronInside(PolyhedronClass checkedPolyhedron)
        {
            // ���� ��� ������� ��������� ������������� checkedPolyhedron ����� ������ ������� �������������
            // �� � ��� ������������ checkedPolyhedron ����� ������ �������
            foreach (VertexClass vertex in checkedPolyhedron.m_VertexList)
            {
                if (!IsPointInside(new Point3D(vertex.XCoord, vertex.YCoord, vertex.ZCoord)))
                {
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// ����� GetCrossingPointWithRay_FullEmun ���������� ����� ����������� ����, ���������� �� ����� O
        /// � ����������� ����� �������� ����� givenPoint, � ������� �������������
        /// ����� ����������� ������ ������� ������� �������� �� ���� ������
        /// </summary>
        /// <param name="givenPoint">�������� �����, ����� ������� �������� ���, ������������ � ����� O</param>
        /// <returns>����� �����������</returns>
        public Point3D GetCrossingPointWithRay_FullEmun(Point3D givenPoint)
        {
            // ����� O (������ ���������)
            Point3D pointO = new Point3D(0, 0, 0);
            // ������� ����� �����������
            Point3D crossingPoint = new Point3D();
            // ���������� ����� ������� ������ ����������� � ������ O
            Double distance2CrossingPoint = Double.NaN;

            // ������ ������� �� ���� ������
            foreach (SideClass currentSide in m_SideList)
            {
                // ���� ��������� ������������ ������� ������� ����� � ������ ������� ����� givenPoint <= 0
                // �� ������ ����� �� �������������, �.�. ��� �� ����� ����� ����������� � ��������������� �����
                Double scalarProduct = currentSide.SideNormal.XCoord * givenPoint.XCoord +
                                       currentSide.SideNormal.YCoord * givenPoint.YCoord +
                                       currentSide.SideNormal.ZCoord * givenPoint.ZCoord;
                // if (scalarProduct <= 0)
                if (scalarProduct <= m_Epsilon)
                {
                    continue;
                }

                PlaneClass currentPlane = new PlaneClass(currentSide.SideNormal, SupportFunc(currentSide.SideNormal));

                // ��������� ����� �����������
                Point3D newCrossingPoint = AdvMathClass.CalcPlaneLineCrossingPoint(currentPlane, pointO, givenPoint);
                // ���������� ������� newCrossingPoint � O
                Double distance2NewCrossingPoint = AdvMathClass.DistanceBetween2Points(pointO, newCrossingPoint);

                if ((Double.IsNaN(distance2CrossingPoint)) || (distance2NewCrossingPoint < distance2CrossingPoint))
                {
                    crossingPoint = newCrossingPoint;
                    distance2CrossingPoint = distance2NewCrossingPoint;
                }
            }

            return crossingPoint;
        }

        /// <summary>
        /// ����� GetCrossingPointWithRay ���������� ����� ����������� ����, ���������� �� ����� O
        /// � ����������� ����� �������� ����� givenPoint, � ������� �������������
        /// ����� ����������� ������ (������� ����������� �������� ���������� �� ����� O �� ����� ����������� ???)
        /// </summary>
        /// <param name="givenPoint">�������� �����, ����� ������� �������� ���, ������������ � ����� O</param>
        /// <returns>����� �����������</returns>
        public Point3D GetCrossingPointWithRay(Point3D givenPoint)
        {
            throw new NotImplementedException("!!!");
        }

        /// <summary>
        /// ����� GetNearestPoint4Given_FullEmun ���������� �������� ������� � ����� givenPoint ����� �� ����������� �������������
        /// �������� ��� ����, ��� ����� givenPoint ����� ��� �������������
        /// ��������� ����� ������ ������� ������� �������� �� ���� ������
        /// </summary>
        /// <param name="givenPoint">�������� �����, ��� ������� ������ ��������� �� ����������� ������������� �����</param>
        /// <returns>��������� ����� � givenPoint �� ����������� �������������</returns>
        public Point3D GetNearestPoint4Given_FullEmun(Point3D givenPoint)
        {
            // ������� ��������� �����
            Point3D nearestPoint = new Point3D();
            // ���������� ����� ������� ��������� ������ � ������ givenPoint
            Double distance2NearestPoint = Double.NaN;

            // ������ ������� �� ���� ������
            foreach (SideClass currentSide in m_SideList)
            {
                // ���� ��������� ������������ ������� ������� ����� � ������� �� ����� givenPoint 
                // � ����� ������� ����� (�������� � �������� 0) >= 0, �� ������ ����� �� �������������,
                // �.�. ��� ��������� � �������� ������� ������������� ������������ ������ givenPoint
                Double scalarProduct = currentSide.SideNormal.XCoord * (currentSide[0].XCoord - givenPoint.XCoord) +
                                       currentSide.SideNormal.YCoord * (currentSide[0].YCoord - givenPoint.YCoord) +
                                       currentSide.SideNormal.ZCoord * (currentSide[0].ZCoord - givenPoint.ZCoord);
                // if (scalarProduct >= 0)
                if (scalarProduct >= -m_Epsilon)
                {
                    continue;
                }

                // ������ ������� �� ���� ������������� �� ������� ����� ������� �����
                // ����� �� ������������ ����������� ��������� �������: (������� 0, ������� i-1, ������� i)
                // ��� i = 2 ... VertecCount-1
                VertexClass firstTriangleVertex = currentSide[0];
                for (Int32 vertexIndex = 2; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    VertexClass secondTriangleVertex = currentSide[vertexIndex - 1];
                    VertexClass thirdTriangleVertex = currentSide[vertexIndex];

                    Boolean isTrueNearestPoint = false;
                    // ��������� ��������� �����
                    Point3D newNearestPoint = CalcNearestPointOnTriangle(firstTriangleVertex,
                                                                         secondTriangleVertex,
                                                                         thirdTriangleVertex,
                                                                         currentSide.SideNormal,
                                                                         givenPoint,
                                                                         out isTrueNearestPoint);
                    // ��������� ��������� ����� �������� �������� ��������� ������ => ����� ������ ���������
                    if (isTrueNearestPoint)
                    {
                        return newNearestPoint;
                    }

                    // ���������� ����� �������� ������ givenPoint � ��������� ��������� ������
                    Double distance2NewNearestPoint = AdvMathClass.DistanceBetween2Points(givenPoint, newNearestPoint);
                    if ((Double.IsNaN(distance2NearestPoint)) || (distance2NewNearestPoint < distance2NearestPoint))
                    {
                        distance2NearestPoint = distance2NewNearestPoint;
                        nearestPoint = newNearestPoint;
                    }
                }
            }

            return nearestPoint;
        }

        /// <summary>
        /// ����� GetNearestPoint4Given ���������� �������� ������� � ����� givenPoint ����� �� ����������� �������������
        /// �������� ��� ����, ��� ����� givenPoint ����� ��� �������������
        /// ��������� ����� ������ (������� ����������� �������� ���������� �� ����� givenPoint �� ����� �� ����������� ������������� ???)
        /// </summary>
        /// <param name="givenPoint">�������� �����, ��� ������� ������ ��������� �� ����������� ������������� �����</param>
        /// <returns>��������� ����� � givenPoint �� ����������� �������������</returns>
        public Point3D GetNearestPoint4Given(Point3D givenPoint)
        {
            throw new NotImplementedException("!!!");
        }

        /// <summary>
        /// ����� GetScaledPolyhedron ���������� ����������������� (�������� �������) ������������
        /// ����������� �������������� - scaleKoeff
        /// </summary>
        /// <param name="scaleKoeff">����������� ��������������</param>
        /// <returns>����������������� (�������� �������) ������������</returns>
        public PolyhedronClass GetScaledPolyhedron(Double scaleKoeff)
        {
            if (scaleKoeff <= 0)
            {
                throw new ArgumentException("scaleKoeff must be > 0 !!!");
            }

            PolyhedronClass scaledPolyhedron = new PolyhedronClass();

            // ��������� ������ ������ � scaledPolyhedron
            for (Int32 vertexIndex = 0; vertexIndex < m_VertexList.Count; vertexIndex++)
            {
                VertexClass currentVertex = m_VertexList[vertexIndex];
                scaledPolyhedron.m_VertexList.Add(new VertexClass(currentVertex.XCoord * scaleKoeff,
                                                                  currentVertex.YCoord * scaleKoeff,
                                                                  currentVertex.ZCoord * scaleKoeff,
                                                                  currentVertex.ID));
            }

            // ��������� ������ ������ � scaledPolyhedron
            for (Int32 sideIndex = 0; sideIndex < m_SideList.Count; sideIndex++)
            {
                SideClass currentSide = m_SideList[sideIndex];
                
                List<VertexClass> vertexList = new List<VertexClass>();
                for (Int32 vertexIndex = 0; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    // ID ������� = ������ ������� + 1
                    vertexList.Add(scaledPolyhedron.m_VertexList[currentSide[vertexIndex].ID - 1]);
                }

                scaledPolyhedron.m_SideList.Add(new SideClass(vertexList, currentSide.ID, currentSide.SideNormal));
            }

            return scaledPolyhedron;
        }

        /// <summary>
        /// ����� ToPoint3DArray ������������ ������������ � ���� ������� ��� ������
        /// </summary>
        /// <returns>������������ � ���� ������� ��� ������</returns>
        public Point3D[] ToPoint3DArray()
        {
            Point3D[] point3DArray = new Point3D[m_VertexList.Count];

            for (Int32 vertexIndex = 0; vertexIndex < m_VertexList.Count; vertexIndex++)
            {
                VertexClass currentVertex = m_VertexList[vertexIndex];
                point3DArray[vertexIndex] = new Point3D(currentVertex.XCoord, currentVertex.YCoord, currentVertex.ZCoord);
            }

            return point3DArray;
        }

        /// <summary>
        /// ������ ������ �������������
        /// </summary>
        internal List<VertexClass> m_VertexList;
        /// <summary>
        /// ������ ������ �������������
        /// </summary>
        internal List<SideClass> m_SideList;
        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private readonly Double m_Epsilon = 1e-9;

        /// <summary>
        /// ����������� ������ PolyhedronClass
        /// </summary>
        internal PolyhedronClass()
        {
            m_VertexList = new List<VertexClass>();
            m_SideList = new List<SideClass>();
        }

        /// <summary>
        /// ������� ������� ������� �������������
        /// </summary>
        /// <param name="vectorArg">���������� ��������, ��� �������� ������ �������� ������� �������</param>
        /// <returns>�������� ������� �������</returns>
        private Double SupportFunc(Vector3D vectorArg)
        {
            Double supportFuncValue = Double.NaN;

            foreach (VertexClass currentVertex in m_VertexList)
            {
                Double scalarProduct = vectorArg.XCoord * currentVertex.XCoord + vectorArg.YCoord * currentVertex.YCoord + vectorArg.ZCoord * currentVertex.ZCoord;
                if ((Double.IsNaN(supportFuncValue)) || (supportFuncValue < scalarProduct))
                {
                    supportFuncValue = scalarProduct;
                }
            }

            return supportFuncValue;
        }

        /// <summary>
        /// ����� CalcNearestPointOnTriangle ��������� ��������� ����� ��� ����� givenPoint
        /// �� ������������, �������� 3-� ���������
        /// </summary>
        /// <param name="triangleVertex1">1-� ������� ������������</param>
        /// <param name="triangleVertex2">2-� ������� ������������</param>
        /// <param name="triangleVertex3">3-� ������� ������������</param>
        /// <param name="triangleNormal">������� ������� � ������������ ( = ������� ������� � �����, ������� �������� ������ �����������)</param>
        /// <param name="givenPoint">�������� �����, ��� ������� ������ ���������</param>
        /// <param name="isTrueNearestPoint">���������� true, ���� ��������� ����� �������� �������� ��������� ������ (����� �� ���� ������� ����� ������ � ����� ������), ����� ������������ false</param>
        /// <returns>��������� ����� �� ������������ � ��������</returns>
        private Point3D CalcNearestPointOnTriangle(VertexClass triangleVertex1, VertexClass triangleVertex2, VertexClass triangleVertex3, Vector3D triangleNormal, Point3D givenPoint, out Boolean isTrueNearestPoint)
        {
            isTrueNearestPoint = false;

            // ����� A - �������� �����, T1, T2, T3 - ������� ������������, N - ������� ������� � ������������
            // ��� ������ ��������� ����� ���������� ������ ��������� ���������:
            // (-N) = x1*AT1 + x2*AT2 + x3*AT3, ��� AT1, AT2, AT3 - �������, x1, x2, x3 - ������� ������������
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = triangleVertex1.XCoord - givenPoint.XCoord;
            matrixA[2, 1] = triangleVertex1.YCoord - givenPoint.YCoord;
            matrixA[3, 1] = triangleVertex1.ZCoord - givenPoint.ZCoord;
            matrixA[1, 2] = triangleVertex2.XCoord - givenPoint.XCoord;
            matrixA[2, 2] = triangleVertex2.YCoord - givenPoint.YCoord;
            matrixA[3, 2] = triangleVertex2.ZCoord - givenPoint.ZCoord;
            matrixA[1, 3] = triangleVertex3.XCoord - givenPoint.XCoord;
            matrixA[2, 3] = triangleVertex3.YCoord - givenPoint.YCoord;
            matrixA[3, 3] = triangleVertex3.ZCoord - givenPoint.ZCoord;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = -triangleNormal.XCoord;
            matrixB[2, 1] = -triangleNormal.YCoord;
            matrixB[3, 1] = -triangleNormal.ZCoord;

            // ������� ���������� ������
            Matrix matrixError = null;
            // ������� � �������������� X (�������)
            //Matrix matrixX = AdvMathClass.SolveEquationSystem3Gauss(matrixA, matrixB, out matrixError);
            Matrix matrixX = AdvMathClass.SolveEquationSystem3Kramer(matrixA, matrixB);
            Double x1 = matrixX[1, 1];
            Double x2 = matrixX[2, 1];
            Double x3 = matrixX[3, 1];

            // ������ ����������� ��������� � �������� ������� ������������� ������������ ����� givenPoint
            if ((x1 < 0) && (x2 < 0) && (x3 < 0))
            {
                throw new ArgumentException("Wrong triangle !!!");
            }

            // ��������� ����� ����� ������ ������������; ��� �������� �������� ��������� ������
            if ((x1 > 0) && (x2 > 0) && (x3 > 0))
            {
                isTrueNearestPoint = true;

                PlaneClass trianglePlane = new PlaneClass(triangleNormal, SupportFunc(triangleNormal));
                return AdvMathClass.CalcPlaneLineCrossingPoint(trianglePlane, givenPoint, triangleNormal);
            }
            
            // ��������� ����� ����� �� ����� 1-2
            if ((x1 > 0) && (x2 > 0) && (x3 <= 0))
            {
                Point3D line2Point1 = new Point3D(triangleVertex1.XCoord, triangleVertex1.YCoord, triangleVertex1.ZCoord);
                Point3D line2Point2 = new Point3D(triangleVertex2.XCoord, triangleVertex2.YCoord, triangleVertex2.ZCoord);

                //Vector3D line1DirectionVector = CalcLineDirectionVector(givenPoint, line2Point1, line2Point2);
                //return AdvMathClass.CalcLineLineCrossingPoint(givenPoint, line1DirectionVector, line2Point1, line2Point2);

                return AdvMathClass.CalcPerpendicularLinesCrossingPoint(givenPoint, line2Point1, line2Point2);
            }
            // ��������� ����� ����� �� ����� 1-3
            if ((x1 > 0) && (x2 <= 0) && (x3 > 0))
            {
                Point3D line2Point1 = new Point3D(triangleVertex1.XCoord, triangleVertex1.YCoord, triangleVertex1.ZCoord);
                Point3D line2Point2 = new Point3D(triangleVertex3.XCoord, triangleVertex3.YCoord, triangleVertex3.ZCoord);
                
                //Vector3D line1DirectionVector = CalcLineDirectionVector(givenPoint, line2Point1, line2Point2);
                //return AdvMathClass.CalcLineLineCrossingPoint(givenPoint, line1DirectionVector, line2Point1, line2Point2);

                return AdvMathClass.CalcPerpendicularLinesCrossingPoint(givenPoint, line2Point1, line2Point2);
            }
            // ��������� ����� ����� �� ����� 2-3
            if ((x1 <= 0) && (x2 > 0) && (x3 > 0))
            {
                Point3D line2Point1 = new Point3D(triangleVertex2.XCoord, triangleVertex2.YCoord, triangleVertex2.ZCoord);
                Point3D line2Point2 = new Point3D(triangleVertex3.XCoord, triangleVertex3.YCoord, triangleVertex3.ZCoord);
                
                //Vector3D line1DirectionVector = CalcLineDirectionVector(givenPoint, line2Point1, line2Point2);
                //return AdvMathClass.CalcLineLineCrossingPoint(givenPoint, line1DirectionVector, line2Point1, line2Point2);
                return AdvMathClass.CalcPerpendicularLinesCrossingPoint(givenPoint, line2Point1, line2Point2);
            }

            // ��������� ����� - ������� ����� 1
            if ((x1 > 0) && (x2 <= 0) && (x3 <= 0))
            {
                return new Point3D(triangleVertex1.XCoord, triangleVertex1.YCoord, triangleVertex1.ZCoord);
            }
            // ��������� ����� - ������� ����� 2
            if ((x1 <= 0) && (x2 > 0) && (x3 <= 0))
            {
                return new Point3D(triangleVertex2.XCoord, triangleVertex2.YCoord, triangleVertex2.ZCoord);
            }
            // ��������� ����� - ������� ����� 3
            if ((x1 <= 0) && (x2 <= 0) && (x3 > 0))
            {
                return new Point3D(triangleVertex3.XCoord, triangleVertex3.YCoord, triangleVertex3.ZCoord);
            }

            // ���� �� ���� �� ������� �� ��������
            throw new Exception("��� �� ���� ������ �� ??? :o");
        }

        /// <summary>
        /// ����� CalcLineDirectionVector ��������� ������������ ������ ������ (����� 1), ���������� ����� ����� point0
        /// � ���������������� (� ������������, �.�. �� ������� !!!) ������ (����� 2), ���������� ����� ����� point1 � point2
        /// </summary>
        /// <param name="point0">�����, ����� ������� �������� ������ ����� 1</param>
        /// <param name="point1">1-� �����, ����� ������� �������� ������ ����� 2</param>
        /// <param name="point2">2-� �����, ����� ������� �������� ������ ����� 2</param>
        /// <returns>������������ ������ ������ ����� 1</returns>
        private Vector3D CalcLineDirectionVector(Point3D point0, Point3D point1, Point3D point2)
        {
            Vector3D vector12 = new Vector3D(point2.XCoord - point1.XCoord,
                                             point2.YCoord - point1.YCoord,
                                             point2.ZCoord - point1.ZCoord);
            Vector3D vector01 = new Vector3D(point1.XCoord - point0.XCoord,
                                             point1.YCoord - point0.YCoord,
                                             point1.ZCoord - point0.ZCoord);
            Vector3D vector02 = new Vector3D(point2.XCoord - point0.XCoord,
                                             point2.YCoord - point0.YCoord,
                                             point2.ZCoord - point0.ZCoord);

            return Vector3D.VectorProduct(Vector3D.VectorProduct(vector01, vector02), vector12);
        }
    }
}
