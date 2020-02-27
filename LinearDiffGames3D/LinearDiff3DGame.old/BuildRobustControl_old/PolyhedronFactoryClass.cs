using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PolyhedronFactoryClass
    {
        /*public static void ConstructPolyhedron(String inputDataFileName, PolyhedronClass polyhedron)
        {
            String[] polyhedronDataFromFile = null;

            using (StreamReader reader = new StreamReader(inputDataFileName))
            {
                polyhedronDataFromFile = reader.ReadToEnd().Split(new String[] { "\r\n" }, StringSplitOptions.None);
            }

            Int32 polyhedronDataItemIndex = 0;

            // ������� ���� ���������� ������
            for (Int32 vertexIndex = 0; polyhedronDataFromFile[polyhedronDataItemIndex] != String.Empty; polyhedronDataItemIndex++, vertexIndex++)
            {
                String[] vertexCoords = polyhedronDataFromFile[polyhedronDataItemIndex].Split(' ');

                if (vertexCoords.Length != 3)
                {
#warning Incorrect format of input data's file
                }

                Double vertexXCoord = Double.Parse(vertexCoords[0], CultureInfo.InvariantCulture);
                Double vertexYCoord = Double.Parse(vertexCoords[1], CultureInfo.InvariantCulture);
                Double vertexZCoord = Double.Parse(vertexCoords[2], CultureInfo.InvariantCulture);

                polyhedron.m_VertexList.Add(new VertexClass(vertexXCoord, vertexYCoord, vertexZCoord, vertexIndex));
            }

            // ���������� ������ ������
            polyhedronDataItemIndex++;

            // ������ ���� ������� ������� ������ � ������������ ������� ������� � ����� (� ������ ������)
            // � ����� ������ �������� ������, ������������ ����� (� ������������� ������ �.�.)
            //
            // ������� ����� ������, ��� � ����� ������� polyhedronDataFromFile ���� ������ ������ !!!!
            for (Int32 sideIndex = 0; polyhedronDataItemIndex < polyhedronDataFromFile.Length - 1; polyhedronDataItemIndex++)
            {
                if (polyhedronDataFromFile.Length - polyhedronDataItemIndex < 2)
                {
#warning Incorrect format of input data's file
                }

                // �������� � ������ ������� ��� ������ �����
                String[] sideNormalCoords = polyhedronDataFromFile[polyhedronDataItemIndex].Split(' ');
                if (sideNormalCoords.Length != 3)
                {
#warning Incorrect format of input data's file
                }

                Double sideNormalXCoord = Double.Parse(sideNormalCoords[0], CultureInfo.InvariantCulture);
                Double sideNormalYCoord = Double.Parse(sideNormalCoords[1], CultureInfo.InvariantCulture);
                Double sideNormalZCoord = Double.Parse(sideNormalCoords[2], CultureInfo.InvariantCulture);

                // �������� �� ������ ������� ��� ������ �����
                polyhedronDataItemIndex++;

                String[] vertexIndicies = polyhedronDataFromFile[polyhedronDataItemIndex].Split(' ');
                List<VertexClass> vertexList = new List<VertexClass>(vertexIndicies.Length);

                foreach (String vertexIndex in vertexIndicies)
                {
                    vertexList.Add(polyhedron.m_VertexList[Int32.Parse(vertexIndex)]);
                }

                polyhedron.m_SideList.Add(new SideClass(vertexList, sideIndex, new Vector3D(sideNormalXCoord, sideNormalYCoord, sideNormalZCoord)));
            }
        }*/

        /// <summary>
        /// ����� ConstructPolyhedron ������� �������� ������������ �� ��������� ������ ������
        /// </summary>
        /// <param name="polyhedronVertexes">����� ������ ��������� �������������</param>
        /// <returns>��������� �������� ������������</returns>
        public static PolyhedronClass ConstructPolyhedron(Point3D[] polyhedronVertexes)
        {
            PolyhedronClass polyhedron = new PolyhedronClass();

            for (Int32 vertexIndex = 0; vertexIndex < polyhedronVertexes.Length; vertexIndex++)
            {
                polyhedron.m_VertexList.Add(new VertexClass(polyhedronVertexes[vertexIndex], vertexIndex + 1));
            }

            RecievePolyhedronStructure(polyhedron);

            return polyhedron;
        }

        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private const Double m_Epsilon = 1e-9;

        /// <summary>
        /// ����������� ������ PolyhedronFactoryClass
        /// �� ���������, �.�. ��������� ������� ��� ������ PolyhedronClass ��� ��������� �� �����
        /// </summary>
        private PolyhedronFactoryClass()
        {
        }

        /// <summary>
        /// ����� RecievePolyhedronStructure �������� (���������������) ��������� �������������
        /// </summary>
        /// <param name="polyhedron">������������, ��������� �������� �� ��������</param>
        private static void RecievePolyhedronStructure(PolyhedronClass polyhedron)
        {
            SideClass FirstSide = GetFirstSide(polyhedron);
            polyhedron.m_SideList.Add(FirstSide);

            List<VertexClass> CheckedSideVertexList = new List<VertexClass>();

            // ���� �� ���� ������ �� ������ ������
            for (Int32 SideIndex = 0; SideIndex < polyhedron.m_SideList.Count; SideIndex++)
            {
                SideClass CurrentSide = polyhedron.m_SideList[SideIndex];

                // ���� �� ���� ������ ������� �����
                for (Int32 CurrentSideVertexIndex = 0; CurrentSideVertexIndex < CurrentSide.VertexCount; CurrentSideVertexIndex++)
                {
                    VertexClass LeftEdgeVertex = CurrentSide[CurrentSideVertexIndex];
                    VertexClass RightEdgeVertex = (CurrentSideVertexIndex == (CurrentSide.VertexCount - 1) ? CurrentSide[0] : CurrentSide[CurrentSideVertexIndex + 1]);

                    // CurrentEdge: LeftEdgeVertex-RightEdgeVertex
                    Int32 SideCount4CurrentEdge = GetSideCount4Edge(LeftEdgeVertex, RightEdgeVertex);
                    // if ((SideCount4CurrentEdge != 1) && (SideCount4CurrentEdge != 2)) !!!!!
                    if (SideCount4CurrentEdge == 2) continue;

                    // ���� �� ���� ��������
                    for (Int32 VertexIndex = 0; VertexIndex < polyhedron.m_VertexList.Count; VertexIndex++)
                    {
                        VertexClass CurrentVertex = polyhedron.m_VertexList[VertexIndex];

                        if ((Object.ReferenceEquals(LeftEdgeVertex, CurrentVertex)) || (Object.ReferenceEquals(RightEdgeVertex, CurrentVertex)))
                        {
                            continue;
                        }

                        CheckedSideVertexList.Clear();
                        CheckedSideVertexList.Add(LeftEdgeVertex);
                        CheckedSideVertexList.Add(RightEdgeVertex);
                        CheckedSideVertexList.Add(CurrentVertex);

                        Boolean CheckResult = DoesVertexesFormSide(CheckedSideVertexList, polyhedron);
                        if (CheckResult)
                        {
                            // "�������" ������� � �����
                            Vector3D SideNormalVector = GetSideExternalNormal(LeftEdgeVertex, RightEdgeVertex, CurrentVertex, polyhedron);
                            if (IsSideAlreadyAdded(SideNormalVector, polyhedron)) continue;

                            // ������������� ������ ������ �����
                            List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList, polyhedron);
                            // ID �����
                            Int32 NewSideID = polyhedron.m_SideList[polyhedron.m_SideList.Count - 1].ID + 1;

                            SideClass NewSide = new SideClass(OrderedSideVertexList, NewSideID, SideNormalVector);
                            polyhedron.m_SideList.Add(NewSide);
                            break;
                        }
                    }
                }
            }
            // ����� ������
        }

        /// <summary>
        /// ����� GetFirstSide ������ � ���������� ������ ����� ������������ (��. ��������)
        /// </summary>
        /// <param name="polyhedron">������������, ��������� �������� �� ��������</param>
        /// <returns>������ ����� �������������</returns>
        private static SideClass GetFirstSide(PolyhedronClass polyhedron)
        {
            SideClass FirstSide = null;

            VertexClass Vertex1 = null;
            VertexClass Vertex2 = null;
            VertexClass Vertex3 = null;

            List<VertexClass> CheckedSideVertexList = new List<VertexClass>();

            Vertex1 = polyhedron.m_VertexList[0];
            for (Int32 Vertex2Index = 1; Vertex2Index < polyhedron.m_VertexList.Count; Vertex2Index++)
            {
                // �����: Vertex1 - Vertex2
                Vertex2 = polyhedron.m_VertexList[Vertex2Index];

                for (Int32 Vertex3Index = 1; Vertex3Index < polyhedron.m_VertexList.Count; Vertex3Index++)
                {
                    if (Vertex2Index == Vertex3Index) continue;

                    Vertex3 = polyhedron.m_VertexList[Vertex3Index];
                    CheckedSideVertexList.Clear();
                    CheckedSideVertexList.Add(Vertex1);
                    CheckedSideVertexList.Add(Vertex2);
                    CheckedSideVertexList.Add(Vertex3);

                    Boolean CheckResult = DoesVertexesFormSide(CheckedSideVertexList, polyhedron);
                    if (CheckResult)
                    {
                        // ������������� ������ ������ �����
                        List<VertexClass> OrderedSideVertexList = OrderingSideVertexList(CheckedSideVertexList, polyhedron);
                        // "�������" ������� � �����
                        Vector3D FirstSideNormalVector = GetSideExternalNormal(Vertex1, Vertex2, Vertex3, polyhedron);
                        FirstSide = new SideClass(OrderedSideVertexList, 1, FirstSideNormalVector);

                        // �� ����� �������� ����� �� ������ !!!!!!!!!
                        return FirstSide;
                    }
                }
            }

            return FirstSide;
        }

        /// <summary>
        /// ����� IsSideAlreadyAdded ��������� ���� �� ��������� ����������� ����� � ������ ������
        /// ��� �������� ������������ ������� ������
        /// </summary>
        /// <param name="SideNormalVector">������� ����������� �����</param>
        /// <param name="polyhedron">������������, ��������� �������� �� ��������</param>
        /// <returns>true, ���� ����� ��������� � ������ ������, ����� false</returns>
        private static Boolean IsSideAlreadyAdded(Vector3D SideNormalVector, PolyhedronClass polyhedron)
        {
            Boolean CheckResult = false;

            // ��� �������� ���������� "�������" ������� ����������� ����� � "��������" ��������� ��� ����������� � ������ ������
            // ��� ��������, �� ���� ���� �� ������������ ���-�������, �� ��� ����� ���������� �������� ��������� 2-3 ����� � ����������� ��������
            // ��-�� ����, ��� ������� ���������� �� ������ �������� � ��-�� ������������ ���������� ������� ����� � ��� �� ��������� ����� �� ��������
            // (��� ������ � ���-������� ������������ ������ ��������� �� ��������, � �� ������������ ��� � ��� �����)
            foreach (SideClass Side in polyhedron.m_SideList)
            {
                if (Side.SideNormal.ApproxEquals(SideNormalVector))
                {
                    CheckResult = true;
                    break;
                }
            }

            return CheckResult;
        }

        /// <summary>
        /// ����� OrderingSideVertexList ������������� ������� �����, �������� � ������ ��������������� ������ SideVertexList
        /// ������� ��������������� ���, ����� ��� ��������� ������ �.�. ���� �������� �� ����� � ����� "�������" ������� � �����
        /// ��� ���� ������ ��������������� ������ SideVertexList, ������������ � ����� OrderingSideVertexList �������� ��� ���������
        /// </summary>
        /// <param name="SideVertexList">������ ��������������� ������</param>
        /// <param name="polyhedron">������������, ��������� �������� �� ��������</param>
        /// <returns>������ ������������� ������</returns>
        private static List<VertexClass> OrderingSideVertexList(List<VertexClass> SideVertexList, PolyhedronClass polyhedron)
        {
            // "�������" ������� � ������� ����� (������� ������� �� �������������)
            Vector3D SideNormalVector = GetSideExternalNormal(SideVertexList[0], SideVertexList[1], SideVertexList[2], polyhedron);

            // ���������� ����������� ��������� ������. ������������ ��� ������ ?
            List<VertexClass> DisorderSideVertexList = new List<VertexClass>(SideVertexList);
            List<VertexClass> OrderSideVertexList = new List<VertexClass>(DisorderSideVertexList.Count);

            OrderSideVertexList.Add(DisorderSideVertexList[0]);
            DisorderSideVertexList.RemoveAt(0);

            while (DisorderSideVertexList.Count > 0)
            {
                VertexClass NextAddedVertex = null;
                Int32 NextAddedVertexIndex = -1;
                VertexClass LastAddedVertex = OrderSideVertexList[OrderSideVertexList.Count - 1];

                // ���������� ������ ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
                Vector3D LineFormingVector = Vector3D.ZeroVector3D;
                // "������" ������� (������� � ������� �����) � ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
                Vector3D LineNormalVector = Vector3D.ZeroVector3D;

                for (Int32 DisorderedVertexIndex = 0; DisorderedVertexIndex < DisorderSideVertexList.Count; DisorderedVertexIndex++)
                {
                    VertexClass DisorderedVertex = DisorderSideVertexList[DisorderedVertexIndex];
                    // ��������� ������������ "������" ������� � ������ � ������ �������, ������������ �� ��������� ����������� � ������� �������
                    Double ScalarProduct = LineNormalVector.XCoord * (DisorderedVertex.XCoord - LastAddedVertex.XCoord) +
                                           LineNormalVector.YCoord * (DisorderedVertex.YCoord - LastAddedVertex.YCoord) +
                                           LineNormalVector.ZCoord * (DisorderedVertex.ZCoord - LastAddedVertex.ZCoord);

                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (ScalarProduct >= 0)
                    {
                        NextAddedVertex = DisorderedVertex;
                        NextAddedVertexIndex = DisorderedVertexIndex;

                        LineFormingVector.XCoord = NextAddedVertex.XCoord - LastAddedVertex.XCoord;
                        LineFormingVector.YCoord = NextAddedVertex.YCoord - LastAddedVertex.YCoord;
                        LineFormingVector.ZCoord = NextAddedVertex.ZCoord - LastAddedVertex.ZCoord;
                        LineFormingVector.Normalize();

                        // "������" ������� � ������ �������� ��� ������ ���������� ������������ ����������� ������� ������ �� "�������" ������� � ������� ����� (???????????????)
                        LineNormalVector = Vector3D.VectorProduct(LineFormingVector, SideNormalVector);
                    }
                }

                OrderSideVertexList.Add(NextAddedVertex);
                DisorderSideVertexList.RemoveAt(NextAddedVertexIndex);
            }

            return OrderSideVertexList;
        }

        /// <summary>
        /// ����� DoesVertexesFormSide ��������� �������� �� ����� (��������� �������������) ����� � ������� (��� �������)
        /// ��� ��� �����, ������������� ����������� ����� ��������� � ������ ������ ���� �����
        /// ������ ������ ����� - SideVertexList; ������ ��� ������� - ������� (����� � �������), �� ������� �������� �����
        /// </summary>
        /// <param name="SideVertexList">������ ������ �����; ������ ��� �������� - ������� (����� � �������), �� ������� �������� �����</param>
        /// <param name="polyhedron">������������, ��������� �������� �� ��������</param>
        /// <returns>true - ���� ����� � ������� ��������� �����, ����� false</returns>
        private static Boolean DoesVertexesFormSide(List<VertexClass> SideVertexList, PolyhedronClass polyhedron)
        {
            Boolean CheckResult = true;

            Vector3D SideVector1 = Vector3D.ZeroVector3D;
            Vector3D SideVector2 = Vector3D.ZeroVector3D;

            // ����� ������ ����� �� ������ ����� ����� ������� (������ O)
            SideVector1.XCoord = SideVertexList[1].XCoord - SideVertexList[0].XCoord;
            SideVector1.YCoord = SideVertexList[1].YCoord - SideVertexList[0].YCoord;
            SideVector1.ZCoord = SideVertexList[1].ZCoord - SideVertexList[0].ZCoord;
            SideVector2.XCoord = SideVertexList[2].XCoord - SideVertexList[0].XCoord;
            SideVector2.YCoord = SideVertexList[2].YCoord - SideVertexList[0].YCoord;
            SideVector2.ZCoord = SideVertexList[2].ZCoord - SideVertexList[0].ZCoord;

            Int32 FirstMixedProductValueSign = 0;
            foreach (VertexClass CurrentVertex in polyhedron.m_VertexList)
            {
                if ((Object.ReferenceEquals(CurrentVertex, SideVertexList[0])) || (Object.ReferenceEquals(CurrentVertex, SideVertexList[1])) || (Object.ReferenceEquals(CurrentVertex, SideVertexList[2])))
                {
                    continue;
                }

                Vector3D CurrentVector = Vector3D.ZeroVector3D;
                CurrentVector.XCoord = CurrentVertex.XCoord - SideVertexList[0].XCoord;
                CurrentVector.YCoord = CurrentVertex.YCoord - SideVertexList[0].YCoord;
                CurrentVector.ZCoord = CurrentVertex.ZCoord - SideVertexList[0].ZCoord;

                // vector a = CurrentVector, vector b = SideVector1, vector c = SideVector2
                Double CurrentMixedProduct = Vector3D.MixedProduct(CurrentVector, SideVector1, SideVector2);

                // Math.Abs(MixedProduct) < Eps ???
                // if (CurrentMixedProduct == 0)
                if (Math.Abs(CurrentMixedProduct) < m_Epsilon)
                {
                    SideVertexList.Add(CurrentVertex);
                }
                else
                {
                    if (FirstMixedProductValueSign == 0)
                    {
                        FirstMixedProductValueSign = Math.Sign(CurrentMixedProduct);
                    }
                    else if (FirstMixedProductValueSign != Math.Sign(CurrentMixedProduct))
                    {
                        CheckResult = false;
                        break;
                    }
                }
            }

            return CheckResult;
        }

        /// <summary>
        /// ����� GetSideExternalNormal ���������� "�������" (�.�. ������������ ������ �������������) ������� � �����
        /// ����� �������� ����� ��������� Vertex1, Vertex2 � Vertex3
        /// </summary>
        /// <param name="Vertex1">������� ����� Vertex1</param>
        /// <param name="Vertex2">������� ����� Vertex2</param>
        /// <param name="Vertex3">������� ����� Vertex3</param>
        /// <param name="polyhedron">������������, ��������� �������� �� ��������</param>
        /// <returns>"�������" ������� � �����, �������� ����� ��������� Vertex1, Vertex2 � Vertex3</returns>
        private static Vector3D GetSideExternalNormal(VertexClass Vertex1, VertexClass Vertex2, VertexClass Vertex3, PolyhedronClass polyhedron)
        {
            Vector3D ExternalNormal = Vector3D.ZeroVector3D;

            if ((Object.ReferenceEquals(Vertex1, Vertex2)) || (Object.ReferenceEquals(Vertex1, Vertex3)) || (Object.ReferenceEquals(Vertex2, Vertex3)))
            {
                throw new ArgumentException("Vertex1, Vertex2, Vertex3 must be different");
            }

            // ������� � ��������� ��������� ����� ��������� ���������, ���������� ����� 3 �����
            // A = (y2-y1)*(z3-z1)-(z2-z1)*(y3-y1)
            ExternalNormal.XCoord = (Vertex2.YCoord - Vertex1.YCoord) * (Vertex3.ZCoord - Vertex1.ZCoord) -
                                    (Vertex2.ZCoord - Vertex1.ZCoord) * (Vertex3.YCoord - Vertex1.YCoord);
            // B = (z2-z1)*(x3-x1)-(x2-x1)*(z3-z1)
            ExternalNormal.YCoord = (Vertex2.ZCoord - Vertex1.ZCoord) * (Vertex3.XCoord - Vertex1.XCoord) -
                                    (Vertex2.XCoord - Vertex1.XCoord) * (Vertex3.ZCoord - Vertex1.ZCoord);
            // C = (x2-x1)*(y3-y1)-(y2-y1)*(x3-x1)
            ExternalNormal.ZCoord = (Vertex2.XCoord - Vertex1.XCoord) * (Vertex3.YCoord - Vertex1.YCoord) -
                                    (Vertex2.YCoord - Vertex1.YCoord) * (Vertex3.XCoord - Vertex1.XCoord);

            // ��������� ���������� �������            
            ExternalNormal.Normalize();

            // �������� ������-������ �� ����� ����� �� ������� �� ��������� � ����� ����� �� ���������
            // �.�. �� �������� � �������� ��������������, �� ���������� ������������ ������� ������� � ������������ ������-������� ������ ���� >0 (��� ��������)
            Double ScalarProduct = 0;
            // Math.Abs(ScalarProduct) < Epsilon ???
            //for (Int32 VertexIndex = 0; ((ScalarProduct == 0) && (VertexIndex < m_PolyhedronVertexList.Count)); VertexIndex++)
            for (Int32 VertexIndex = 0; ((Math.Abs(ScalarProduct) < m_Epsilon) && (VertexIndex < polyhedron.m_VertexList.Count)); VertexIndex++)
            {
                VertexClass CurrentVertex = polyhedron.m_VertexList[VertexIndex];
                ScalarProduct = ExternalNormal.XCoord * (Vertex1.XCoord - CurrentVertex.XCoord) +
                                ExternalNormal.YCoord * (Vertex1.YCoord - CurrentVertex.YCoord) +
                                ExternalNormal.ZCoord * (Vertex1.ZCoord - CurrentVertex.ZCoord);
            }
            // Math.Abs(ScalarProduct) < Epsilon ???
            //if (ScalarProduct == 0)
            if (Math.Abs(ScalarProduct) < m_Epsilon)
            {
                // some exception !!!
            }
            if (ScalarProduct < 0)
            {
                ExternalNormal.XCoord *= -1;
                ExternalNormal.YCoord *= -1;
                ExternalNormal.ZCoord *= -1;
            }

            return ExternalNormal;
        }

        /// <summary>
        /// ����� GetSideCount4Edge ���������� ���������� ������, ������� ����������� �������� (�� 2 ��������) �����
        /// </summary>
        /// <param name="EdgeVertex1">������ ������� �����</param>
        /// <param name="EdgeVertex2">������ ������� �����</param>
        /// <returns>���������� ������, ������� ����������� �������� �����</returns>
        private static Int32 GetSideCount4Edge(VertexClass EdgeVertex1, VertexClass EdgeVertex2)
        {
            Int32 SideCount4Edge = 0;

            for (Int32 EdgeVertex1SideListIndex = 0; EdgeVertex1SideListIndex < EdgeVertex1.SideCount; EdgeVertex1SideListIndex++)
            {
                if (EdgeVertex1[EdgeVertex1SideListIndex].HasVertex(EdgeVertex2)) SideCount4Edge++;
            }

            return SideCount4Edge;
        }
    }
}
