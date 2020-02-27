using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Geometry3D.PolyhedronFactory
{
    /// <summary>
    /// ������� ��� ��������� ��������� (������, �����) ��������� ������������� �� �������� ��������
    /// </summary>
    public class Polyhedron3DFromPointsFactory
    {
        /// <summary>
        /// ����������� ������ Polyhedron3DFromPointsFactory
        /// </summary>
        /// <param name="approxComparer">������������, ��� ������������� ��������� �������������� �����</param>
        public Polyhedron3DFromPointsFactory(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
            m_VertexSidesDictionary = new VertexSidesDictionary();
        }

        /// <summary>
        /// ��������� ����� ��� ��������� ��������� (������, �����) ��������� ������������� �� �������� ��������
        /// </summary>
        /// <param name="vertexes">������ �������� ������</param>
        /// <returns>���������� �������� ������������</returns>
        public Polyhedron3D CreatePolyhedron(IList<Point3D> vertexes)
        {
            List<PolyhedronSide3D> sideList = new List<PolyhedronSide3D>();
            List<PolyhedronVertex3D> vertexList = new List<PolyhedronVertex3D>();

            for(Int32 vertexIndex = 0; vertexIndex < vertexes.Count; ++vertexIndex)
                vertexList.Add(new PolyhedronVertex3D(vertexes[vertexIndex], vertexIndex));

            // ��������� ������ �����
            PolyhedronSide3D firstSide = GetFirstSide(vertexList);
            // ���������� ���������� ������ ����� � ������ ������
            sideList.Add(firstSide);

            List<PolyhedronVertex3D> sideVertexList = new List<PolyhedronVertex3D>();

            // ���� �� ���� ������ �� ������ ������
            for(Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = sideList[sideIndex];

                // ���� �� ���� ������ ������� �����
                for(Int32 csVertexIndex = 0; csVertexIndex < currentSide.VertexList.Count; ++csVertexIndex)
                {
                    PolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[csVertexIndex];
                    PolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(csVertexIndex);

                    // CurrentEdge: LeftEdgeVertex-RightEdgeVertex
                    Int32 sideCount4CurrentEdge = GetSideCount4Edge(leftEdgeVertex, rightEdgeVertex);
                    // ���� ������� ����� ����������� ���� ������, �� ������� � ��������� ��������
                    if(sideCount4CurrentEdge != 1) continue;

                    // ���� �� ���� ��������
                    for(Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
                    {
                        PolyhedronVertex3D currentVertex = vertexList[vertexIndex];

                        // �������� �������� �� ����� ������� ����� � ������� ��������� ������� (��� ���� �����, ������������� ����������� �����, ����������� � ������ ������ ���� �����)
                        if(ReferenceEquals(leftEdgeVertex, currentVertex) ||
                           ReferenceEquals(rightEdgeVertex, currentVertex))
                            continue;
                        sideVertexList.Clear();
                        sideVertexList.Add(leftEdgeVertex);
                        sideVertexList.Add(rightEdgeVertex);
                        sideVertexList.Add(currentVertex);
                        Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
                        // ���� �������� ��������
                        if(checkResult)
                        {
                            // "�������" ������� � �����
                            Vector3D externalNormal = GetSideExternalNormal(vertexList, leftEdgeVertex, rightEdgeVertex,
                                                                            currentVertex);
                            // ���� ����� ����� ��������� � ����� �� ����� �����������
                            if(IsSideAlreadyAdded(sideList, externalNormal)) continue;

                            // ������������� ������ ������ �����
                            List<PolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList,
                                                                                           externalNormal);
                            // ID �����
                            Int32 sideID = sideList[sideList.Count - 1].ID + 1;

                            PolyhedronSide3D newSide = new PolyhedronSide3D(orderVertexList, sideID, externalNormal);
                            sideList.Add(newSide);
                            // � ������� ������� - ������ ������ ��������� ��� ������� ����� ����� + �� ����
                            foreach(PolyhedronVertex3D vertex in orderVertexList)
                                m_VertexSidesDictionary.AddSide4Vertex(vertex, newSide);
                            // ����� �� ����� �� ��������� ��������
                            break;
                        }
                    }
                    // ���� �� ���� ��������
                }
                // ���� �� ���� ������ ������� �����
            }
            // ���� �� ���� ������ �� ������ ������

            return new Polyhedron3D(sideList, vertexList);
        }

        /// <summary>
        /// ����� GetSideCount4Edge ���������� ���������� ������, ������� ����������� �������� (�� 2 ��������) �����
        /// </summary>
        /// <param name="edgeVertex1">������ ������� �����</param>
        /// <param name="edgeVertex2">������ ������� �����</param>
        /// <returns>���������� ������, ������� ����������� �������� �����</returns>
        private Int32 GetSideCount4Edge(PolyhedronVertex3D edgeVertex1, PolyhedronVertex3D edgeVertex2)
        {
            Int32 sideCount4Edge = 0;
            IList<PolyhedronSide3D> edge1SideList = m_VertexSidesDictionary.GetSideList4Vertex(edgeVertex1);

            for(Int32 sideIndex = 0; sideIndex < edge1SideList.Count; ++sideIndex)
                if(edge1SideList[sideIndex].HasVertex(edgeVertex2)) ++sideCount4Edge;

            return sideCount4Edge;
        }

        /// <summary>
        /// ����� IsSideAlreadyAdded ��������� ���� �� ��������� ����������� ����� � ������ ������
        /// ��� �������� ������������ ������� ������
        /// </summary>
        /// <param name="sideList">������ ������</param>
        /// <param name="externalNormal">"�������" ������� ����������� �����</param>
        /// <returns>true, ���� ����� ��������� � ������ ������, ����� false</returns>
        private Boolean IsSideAlreadyAdded(IList<PolyhedronSide3D> sideList, Vector3D externalNormal)
        {
            Boolean checkResult = false;

            // ��� �������� ���������� "�������" ������� ����������� ����� � "��������" ��������� ��� ����������� � ������ ������
            for(Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                Double cosAngleBetweenVectors = Vector3DUtils.CosAngleBetweenVectors(sideList[sideIndex].SideNormal,
                                                                                     externalNormal);
                if(m_ApproxComparer.EQ(cosAngleBetweenVectors, 1))
                {
                    checkResult = true;
                    break;
                }
            }

            return checkResult;
        }

        /// <summary>
        /// ����� GetFirstSide ������ � ���������� ������ ����� ������������
        /// ��. �������� ��������� ��������� (������, �����) ��������� ������������� �� �������� ��������
        /// </summary>
        /// <param name="vertexList">������ ������ ��������������� �������������</param>
        /// <returns>������ ����� �������������</returns>
        private PolyhedronSide3D GetFirstSide(IList<PolyhedronVertex3D> vertexList)
        {
            PolyhedronSide3D firstSide = null;

            List<PolyhedronVertex3D> sideVertexList = new List<PolyhedronVertex3D>();

            // ����� ������ ������� �� ������ ������ � �������� ������ ������� (������) �����
            PolyhedronVertex3D vertex1 = vertexList[0];
            // ���� �� ���� �������� (����� ������) �� ������ ������
            for(Int32 vertex2Index = 1; vertex2Index < vertexList.Count; ++vertex2Index)
            {
                // ����� ������� ������� � �������� ������ ������� �����
                PolyhedronVertex3D vertex2 = vertexList[vertex2Index];

                // ���� �� ���� �������� ����� ������ �� ����� 
                for(Int32 vertex3Index = vertex2Index + 1; vertex3Index < vertexList.Count; ++vertex3Index)
                {
                    // ������� ������� ���������� ������ �������� �����
                    PolyhedronVertex3D vertex3 = vertexList[vertex3Index];

                    sideVertexList.Clear();
                    sideVertexList.Add(vertex1);
                    sideVertexList.Add(vertex2);
                    sideVertexList.Add(vertex3);

                    // �������� �������� �� ����� ����������� ����� � ������ ������� ����� (��� ���� �����, ������������� ����������� �����, ����������� � ������ ������ ���� �����)
                    Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
                    // ���� �������� ��������
                    if(checkResult)
                    {
                        // "�������" ������� � �����
                        Vector3D externalNormal = GetSideExternalNormal(vertexList, vertex1, vertex2, vertex3);
                        // ������������� ������ ������ �����
                        List<PolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList, externalNormal);

                        firstSide = new PolyhedronSide3D(orderVertexList, 0, externalNormal);
                        // � ������� ������� - ������ ������ ��������� ��� ������� ������ ����� + �� ����
                        foreach(PolyhedronVertex3D vertex in orderVertexList)
                            m_VertexSidesDictionary.AddSide4Vertex(vertex, firstSide);

                        // �� ����� �������� ����� �� ������ !!!!!!!!!
                        return firstSide;
                    }
                    // ���� �������� ��������
                }
                // ���� �� ���� �������� ����� ������ �� ����� 
            }
            // ���� �� ���� �������� (����� ������) �� ������ ������

            return firstSide;
        }

        /// <summary>
        /// ����� DoesVertexesFormSide ��������� �������� �� ����� (��������� �������������) ����� � ������� (��� �������)
        /// ��� ���� �������, ������������� ����������� ����� ��������� � ������ ������ ���� �����
        /// ������ ������ ����� - sideVertexList; ������ ��� ������� - ������� (����� � �������), �� ������� �������� �����
        /// </summary>
        /// <param name="vertexList">������ ������ ��������������� �������������</param>
        /// <param name="sideVertexList">������ ������ �����; ������ ��� �������� - ������� (����� � �������), �� ������� �������� �����</param>
        /// <returns>true - ���� ����� � ������� �������� �����, ����� false</returns>
        private Boolean DoesVertexesFormSide(IList<PolyhedronVertex3D> vertexList,
                                             IList<PolyhedronVertex3D> sideVertexList)
        {
            Boolean checkResult = true;

            Vector3D sideVector1 = Vector3D.ZeroVector3D;
            Vector3D sideVector2 = Vector3D.ZeroVector3D;

            // ����� ������ ����� �� ������ ����� ����� ������� (������ O)
            sideVector1.XCoord = sideVertexList[1].XCoord - sideVertexList[0].XCoord;
            sideVector1.YCoord = sideVertexList[1].YCoord - sideVertexList[0].YCoord;
            sideVector1.ZCoord = sideVertexList[1].ZCoord - sideVertexList[0].ZCoord;
            sideVector2.XCoord = sideVertexList[2].XCoord - sideVertexList[0].XCoord;
            sideVector2.YCoord = sideVertexList[2].YCoord - sideVertexList[0].YCoord;
            sideVector2.ZCoord = sideVertexList[2].ZCoord - sideVertexList[0].ZCoord;

            // ���� ���������� ������������, ������������ � ������ ��� (��� ���������� ������������ �� ������� 0)
            Int32 mixedProductSign = 0;
            // ���� �� ���� �������� �������������
            for(Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D currentVertex = vertexList[vertexIndex];

                // ���� ������� ������� ��������� � ����� �� ����, ���������� ����������� �����
                if(ReferenceEquals(currentVertex, sideVertexList[0]) ||
                   ReferenceEquals(currentVertex, sideVertexList[1]) ||
                   ReferenceEquals(currentVertex, sideVertexList[2]))
                    continue;

                Vector3D currentVector = Vector3D.ZeroVector3D;
                currentVector.XCoord = currentVertex.XCoord - sideVertexList[0].XCoord;
                currentVector.YCoord = currentVertex.YCoord - sideVertexList[0].YCoord;
                currentVector.ZCoord = currentVertex.ZCoord - sideVertexList[0].ZCoord;

                // vector a = currentVector, vector b = sideVector1, vector c = sideVector2
                Double mixedProduct = Vector3D.MixedProduct(currentVector, sideVector1, sideVector2);

                // ���� ��������� ������������ == 0
                if(m_ApproxComparer.EQ(mixedProduct, 0))
                    sideVertexList.Add(currentVertex);
                    // ���� ��������� ������������ != 0
                else
                {
                    // ���� ��������� ������������ ���� ��������� � ������ ���
                    if(mixedProductSign == 0)
                        mixedProductSign = Math.Sign(mixedProduct);
                        // ����� ... ���� ���� ���������� ������������ �� ��������� � �����������
                    else if(mixedProductSign != Math.Sign(mixedProduct))
                    {
                        checkResult = false;
                        break;
                    }
                }
            }
            // ���� �� ���� �������� �������������

            return checkResult;
        }

        /// <summary>
        /// ����� GetSideExternalNormal ���������� "�������" (�.�. ������������ ������ �������������) ������� � �����
        /// ����� �������� ����� ��������� vertex1, vertex2 � vertex3
        /// </summary>
        /// <param name="vertexList">������ ������ ��������������� �������������</param>
        /// <param name="vertex1">������� ����� vertex1</param>
        /// <param name="vertex2">������� ����� vertex2</param>
        /// <param name="vertex3">������� ����� vertex3</param>
        /// <returns>"�������" ������� � �����, �������� ����� ��������� vertex1, vertex2 � vertex3</returns>
        private Vector3D GetSideExternalNormal(IList<PolyhedronVertex3D> vertexList, PolyhedronVertex3D vertex1,
                                               PolyhedronVertex3D vertex2, PolyhedronVertex3D vertex3)
        {
            Vector3D externalNormal = Vector3D.ZeroVector3D;

            if(ReferenceEquals(vertex1, vertex2) ||
               ReferenceEquals(vertex1, vertex3) ||
               ReferenceEquals(vertex2, vertex3))
            {
#warning ����� ����� ������������������ ����������
                throw new ArgumentException("vertex1, vertex2, vertex3 must be different");
            }

            // ������� � ��������� ��������� ����� ��������� ���������, ���������� ����� 3 �����
            // A = (y2-y1)*(z3-z1)-(z2-z1)*(y3-y1)
            externalNormal.XCoord = (vertex2.YCoord - vertex1.YCoord) * (vertex3.ZCoord - vertex1.ZCoord) -
                                    (vertex2.ZCoord - vertex1.ZCoord) * (vertex3.YCoord - vertex1.YCoord);
            // B = (z2-z1)*(x3-x1)-(x2-x1)*(z3-z1)
            externalNormal.YCoord = (vertex2.ZCoord - vertex1.ZCoord) * (vertex3.XCoord - vertex1.XCoord) -
                                    (vertex2.XCoord - vertex1.XCoord) * (vertex3.ZCoord - vertex1.ZCoord);
            // C = (x2-x1)*(y3-y1)-(y2-y1)*(x3-x1)
            externalNormal.ZCoord = (vertex2.XCoord - vertex1.XCoord) * (vertex3.YCoord - vertex1.YCoord) -
                                    (vertex2.YCoord - vertex1.YCoord) * (vertex3.XCoord - vertex1.XCoord);

            // ��������� ���������� �������
            externalNormal = Vector3DUtils.NormalizeVector(externalNormal);

            // �������� ������-������ �� ����� ����� �� ������� �� ��������� � ����� ����� �� ���������
            // �.�. �� �������� � �������� ��������������, �� ���������� ������������ ������� ������� � ������������ ������-������� ������ ���� >0 (��� ��������)
            Double scalarProduct = 0;

            //for (Int32 vertexIndex = 0; ((scalarProduct == 0) && (vertexIndex < vertexList.Count)); ++vertexIndex)
            for(Int32 vertexIndex = 0;
                (m_ApproxComparer.EQ(scalarProduct, 0) && vertexIndex < vertexList.Count);
                ++vertexIndex)
            {
                PolyhedronVertex3D currentVertex = vertexList[vertexIndex];
                scalarProduct = externalNormal.XCoord * (vertex1.XCoord - currentVertex.XCoord) +
                                externalNormal.YCoord * (vertex1.YCoord - currentVertex.YCoord) +
                                externalNormal.ZCoord * (vertex1.ZCoord - currentVertex.ZCoord);
            }
            //for (Int32 vertexIndex = 0; ((scalarProduct == 0) && (vertexIndex < vertexList.Count)); ++vertexIndex)

            //if (scalarProduct == 0)
            if(m_ApproxComparer.EQ(scalarProduct, 0))
            {
#warning ����� ����� ������������������ ����������
                throw new Exception("Can't calulate external normal for side !!!!");
            }

            if(scalarProduct < 0)
                externalNormal *= -1.0;

            return externalNormal;
        }

        /// <summary>
        /// ����� OrderSideVertexList ������������� ������� �����, �������� � ��������������� ������ ������ sideVertexList
        /// ������� ��������������� ���, ����� ��� ��������� ������ �.�. ���� �������� �� ����� � ����� "�������" ������� externalNormal � �����
        /// ��� ���� ������ ��������������� ������ sideVertexList, ������������ � ����� OrderSideVertexList �������� ��� ���������
        /// </summary>
        /// <param name="sideVertexList">��������������� ������ ������</param>
        /// <param name="externalNormal">������� ������� � �����</param>
        /// <returns>������ ������������� ������</returns>
        private List<PolyhedronVertex3D> OrderSideVertexList(IEnumerable<PolyhedronVertex3D> sideVertexList,
                                                             Vector3D externalNormal)
        {
            // ��������������� ������ ������
            List<PolyhedronVertex3D> disorderVertexList = new List<PolyhedronVertex3D>(sideVertexList);
            // ������������� ������ ������
            List<PolyhedronVertex3D> orderVertexList = new List<PolyhedronVertex3D>(disorderVertexList.Count);

            // ����� ������ ������� �� ������ ��������������� ������,
            // ���������� �� � ������ ������������� ������ � ������� �� �� ������ ��������������� ������
            orderVertexList.Add(disorderVertexList[0]);
            disorderVertexList.RemoveAt(0);

            // ���� ���� ������ ��������������� ������ �� ����
            while(disorderVertexList.Count > 0)
            {
                // ��������� ������� ��� ����������
                PolyhedronVertex3D nextAddedVertex = disorderVertexList[0];
                // ��������� ����������� �������
                PolyhedronVertex3D lastAddedVertex = orderVertexList[orderVertexList.Count - 1];

                // ���������� ������ ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
                Vector3D lineFormingVector = Vector3D.ZeroVector3D;
                lineFormingVector.XCoord = nextAddedVertex.XCoord - lastAddedVertex.XCoord;
                lineFormingVector.YCoord = nextAddedVertex.YCoord - lastAddedVertex.YCoord;
                lineFormingVector.ZCoord = nextAddedVertex.ZCoord - lastAddedVertex.ZCoord;
                lineFormingVector = Vector3DUtils.NormalizeVector(lineFormingVector);
                // "������" ������� (������� � ������� �����) � ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
                // "������" ������� � ������ �������� ��� ������ ���������� ������������ ����������� ������� ������ �� "�������" ������� � ������� �����
                Vector3D lineNormalVector = Vector3D.VectorProduct(lineFormingVector, externalNormal);

                // ���� �� ���� �������� �� ������ ��������������� ������, ����� ������
                for(Int32 disorderedVertexIndex = 1;
                    disorderedVertexIndex < disorderVertexList.Count;
                    ++disorderedVertexIndex)
                {
                    PolyhedronVertex3D disorderedVertex = disorderVertexList[disorderedVertexIndex];
                    // ��������� ������������ "������" ������� � ������ � ������ �������, ������������ �� ��������� ����������� � ������� �������
                    Double scalarProduct = lineNormalVector.XCoord * (disorderedVertex.XCoord - lastAddedVertex.XCoord) +
                                           lineNormalVector.YCoord * (disorderedVertex.YCoord - lastAddedVertex.YCoord) +
                                           lineNormalVector.ZCoord * (disorderedVertex.ZCoord - lastAddedVertex.ZCoord);

                    // if (ScalarProduct = 0)
                    if(m_ApproxComparer.EQ(scalarProduct, 0))
                    {
#warning ����� ����� ������������������ ����������
                        throw new Exception("unexpected scalar product value !!! (scalar product value == 0)");
                    }
                    // if (ScalarProduct > 0)
                    if(m_ApproxComparer.GT(scalarProduct, 0))
                    {
                        nextAddedVertex = disorderedVertex;

                        lineFormingVector.XCoord = nextAddedVertex.XCoord - lastAddedVertex.XCoord;
                        lineFormingVector.YCoord = nextAddedVertex.YCoord - lastAddedVertex.YCoord;
                        lineFormingVector.ZCoord = nextAddedVertex.ZCoord - lastAddedVertex.ZCoord;
                        lineFormingVector = Vector3DUtils.NormalizeVector(lineFormingVector);

                        // "������" ������� � ������ �������� ��� ������ ���������� ������������ ����������� ������� ������ �� "�������" ������� � ������� ����� (???????????????)
                        lineNormalVector = Vector3D.VectorProduct(lineFormingVector, externalNormal);
                    }
                }
                // ���� �� ���� �������� �� ������ ��������������� ������, ����� ������

                orderVertexList.Add(nextAddedVertex);
                disorderVertexList.Remove(nextAddedVertex);
            }
            // ���� ���� ������ ��������������� ������ �� ����

            return orderVertexList;
        }

        /// <summary>
        /// ������������, ��� ������������� ��������� �������������� �����
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;

        /// <summary>
        /// ������� ������� - ������ ������
        /// </summary>
        private readonly VertexSidesDictionary m_VertexSidesDictionary;
    }
}