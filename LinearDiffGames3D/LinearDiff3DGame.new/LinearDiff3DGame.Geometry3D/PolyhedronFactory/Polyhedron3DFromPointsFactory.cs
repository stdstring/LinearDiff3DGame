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
		public Polyhedron3DFromPointsFactory(ApproxComp approxComparer)
		{
			this.approxComparer = approxComparer;
			vertexSidesDictionary = new VertexSidesDictionary();
		}

		// ��������� ����� ��� ��������� ��������� (������, �����) ��������� ������������� �� �������� ��������
		public IPolyhedron3D CreatePolyhedron(IList<Point3D> vertexes)
		{
			List<IPolyhedronSide3D> sideList = new List<IPolyhedronSide3D>();
			List<IPolyhedronVertex3D> vertexList = new List<IPolyhedronVertex3D>();

			for (Int32 vertexIndex = 0; vertexIndex < vertexes.Count; ++vertexIndex)
				vertexList.Add(new PolyhedronVertex3D(vertexes[vertexIndex], vertexIndex));

			// ��������� ������ �����
			IPolyhedronSide3D firstSide = GetFirstSide(vertexList);
			// ���������� ���������� ������ ����� � ������ ������
			sideList.Add(firstSide);

			List<IPolyhedronVertex3D> sideVertexList = new List<IPolyhedronVertex3D>();

			// ���� �� ���� ������ �� ������ ������
			for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
			{
				IPolyhedronSide3D currentSide = sideList[sideIndex];

				// ���� �� ���� ������ ������� �����
				for (Int32 csVertexIndex = 0; csVertexIndex < currentSide.VertexList.Count; ++csVertexIndex)
				{
					IPolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[csVertexIndex];
					IPolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(csVertexIndex);

					// CurrentEdge: LeftEdgeVertex-RightEdgeVertex
					Int32 sideCount4CurrentEdge = GetSideCount4Edge(leftEdgeVertex, rightEdgeVertex);
					// ���� ������� ����� ����������� ���� ������, �� ������� � ��������� ��������
					if (sideCount4CurrentEdge != 1) continue;

					// ���� �� ���� ��������
					for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
					{
						IPolyhedronVertex3D currentVertex = vertexList[vertexIndex];

						// �������� �������� �� ����� ������� ����� � ������� ��������� ������� (��� ���� �����, ������������� ����������� �����, ����������� � ������ ������ ���� �����)
						if (ReferenceEquals(leftEdgeVertex, currentVertex) ||
						    ReferenceEquals(rightEdgeVertex, currentVertex))
							continue;
						sideVertexList.Clear();
						sideVertexList.Add(leftEdgeVertex);
						sideVertexList.Add(rightEdgeVertex);
						sideVertexList.Add(currentVertex);
						Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
						// ���� �������� ��������
						if (checkResult)
						{
							// "�������" ������� � �����
							Vector3D externalNormal = GetSideExternalNormal(vertexList, leftEdgeVertex, rightEdgeVertex,
							                                                currentVertex);
							// ���� ����� ����� ��������� � ����� �� ����� �����������
							if (IsSideAlreadyAdded(sideList, externalNormal)) continue;

							// ������������� ������ ������ �����
							List<IPolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList,
							                                                                externalNormal);
							// ID �����
							Int32 sideID = sideList[sideList.Count - 1].ID + 1;

							IPolyhedronSide3D newSide = new PolyhedronSide3D(orderVertexList, sideID, externalNormal);
							sideList.Add(newSide);
							// � ������� ������� - ������ ������ ��������� ��� ������� ����� ����� + �� ����
							foreach (IPolyhedronVertex3D vertex in orderVertexList)
								vertexSidesDictionary.AddSide4Vertex(vertex, newSide);
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

		// ����� GetSideCount4Edge ���������� ���������� ������, ������� ����������� �������� (�� 2 ��������) �����
		private Int32 GetSideCount4Edge(IPolyhedronVertex3D edgeVertex1, IPolyhedronVertex3D edgeVertex2)
		{
			Int32 sideCount4Edge = 0;
			IList<IPolyhedronSide3D> edge1SideList = vertexSidesDictionary.GetSideList4Vertex(edgeVertex1);

			for (Int32 sideIndex = 0; sideIndex < edge1SideList.Count; ++sideIndex)
				if (edge1SideList[sideIndex].VertexList.Contains(edgeVertex2)) ++sideCount4Edge;

			return sideCount4Edge;
		}

		// ����� IsSideAlreadyAdded ��������� ���� �� ��������� ����������� ����� � ������ ������; ��� �������� ������������ ������� ������
		private Boolean IsSideAlreadyAdded(IList<IPolyhedronSide3D> sideList, Vector3D externalNormal)
		{
			Boolean checkResult = false;

			// ��� �������� ���������� "�������" ������� ����������� ����� � "��������" ��������� ��� ����������� � ������ ������
			for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
			{
				Double cosAngleBetweenVectors = Vector3DUtils.CosAngleBetweenVectors(sideList[sideIndex].SideNormal,
				                                                                     externalNormal);
				if (approxComparer.EQ(cosAngleBetweenVectors, 1))
				{
					checkResult = true;
					break;
				}
			}

			return checkResult;
		}

		// ����� GetFirstSide ������ � ���������� ������ ����� ������������ (��. �������� ��������� ��������� (������, �����) ��������� ������������� �� �������� ��������)
		private IPolyhedronSide3D GetFirstSide(IList<IPolyhedronVertex3D> vertexList)
		{
			IPolyhedronSide3D firstSide = null;

			List<IPolyhedronVertex3D> sideVertexList = new List<IPolyhedronVertex3D>();

			// ����� ������ ������� �� ������ ������ � �������� ������ ������� (������) �����
			IPolyhedronVertex3D vertex1 = vertexList[0];
			// ���� �� ���� �������� (����� ������) �� ������ ������
			for (Int32 vertex2Index = 1; vertex2Index < vertexList.Count; ++vertex2Index)
			{
				// ����� ������� ������� � �������� ������ ������� �����
				IPolyhedronVertex3D vertex2 = vertexList[vertex2Index];

				// ���� �� ���� �������� ����� ������ �� ����� 
				for (Int32 vertex3Index = vertex2Index + 1; vertex3Index < vertexList.Count; ++vertex3Index)
				{
					// ������� ������� ���������� ������ �������� �����
					IPolyhedronVertex3D vertex3 = vertexList[vertex3Index];

					sideVertexList.Clear();
					sideVertexList.Add(vertex1);
					sideVertexList.Add(vertex2);
					sideVertexList.Add(vertex3);

					// �������� �������� �� ����� ����������� ����� � ������ ������� ����� (��� ���� �����, ������������� ����������� �����, ����������� � ������ ������ ���� �����)
					Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
					// ���� �������� ��������
					if (checkResult)
					{
						// "�������" ������� � �����
						Vector3D externalNormal = GetSideExternalNormal(vertexList, vertex1, vertex2, vertex3);
						// ������������� ������ ������ �����
						List<IPolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList, externalNormal);

						firstSide = new PolyhedronSide3D(orderVertexList, 0, externalNormal);
						// � ������� ������� - ������ ������ ��������� ��� ������� ������ ����� + �� ����
						foreach (IPolyhedronVertex3D vertex in orderVertexList)
							vertexSidesDictionary.AddSide4Vertex(vertex, firstSide);

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

		// ����� DoesVertexesFormSide ��������� �������� �� ����� (��������� �������������) ����� � ������� (��� �������)
		// ��� ���� �������, ������������� ����������� ����� ��������� � ������ ������ ���� �����
		// ������ ������ ����� - sideVertexList; ������ ��� ������� - ������� (����� � �������), �� ������� �������� �����
		private Boolean DoesVertexesFormSide(IList<IPolyhedronVertex3D> vertexList,
		                                     IList<IPolyhedronVertex3D> sideVertexList)
		{
			Boolean checkResult = true;

			// ����� ������ ����� �� ������ ����� ����� ������� (������ O)
			Double x1 = sideVertexList[1].XCoord - sideVertexList[0].XCoord;
			Double y1 = sideVertexList[1].YCoord - sideVertexList[0].YCoord;
			Double z1 = sideVertexList[1].ZCoord - sideVertexList[0].ZCoord;
			Vector3D sideVector1 = new Vector3D(x1, y1, z1);
			Double x2 = sideVertexList[2].XCoord - sideVertexList[0].XCoord;
			Double y2 = sideVertexList[2].YCoord - sideVertexList[0].YCoord;
			Double z2 = sideVertexList[2].ZCoord - sideVertexList[0].ZCoord;
			Vector3D sideVector2 = new Vector3D(x2, y2, z2);

			// ���� ���������� ������������, ������������ � ������ ��� (��� ���������� ������������ �� ������� 0)
			Int32 mixedProductSign = 0;
			// ���� �� ���� �������� �������������
			for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
			{
				IPolyhedronVertex3D currentVertex = vertexList[vertexIndex];

				// ���� ������� ������� ��������� � ����� �� ����, ���������� ����������� �����
				if (ReferenceEquals(currentVertex, sideVertexList[0]) ||
				    ReferenceEquals(currentVertex, sideVertexList[1]) ||
				    ReferenceEquals(currentVertex, sideVertexList[2]))
					continue;

				Double x = currentVertex.XCoord - sideVertexList[0].XCoord;
				Double y = currentVertex.YCoord - sideVertexList[0].YCoord;
				Double z = currentVertex.ZCoord - sideVertexList[0].ZCoord;
				Vector3D currentVector = new Vector3D(x, y, z);

				// vector a = currentVector, vector b = sideVector1, vector c = sideVector2
				Double mixedProduct = Vector3DUtils.MixedProduct(currentVector, sideVector1, sideVector2);

				// ���� ��������� ������������ == 0
				if (approxComparer.EQ(mixedProduct, 0))
					sideVertexList.Add(currentVertex);
					// ���� ��������� ������������ != 0
				else
				{
					// ���� ��������� ������������ ���� ��������� � ������ ���
					if (mixedProductSign == 0)
						mixedProductSign = Math.Sign(mixedProduct);
						// ����� ... ���� ���� ���������� ������������ �� ��������� � �����������
					else if (mixedProductSign != Math.Sign(mixedProduct))
					{
						checkResult = false;
						break;
					}
				}
			}
			// ���� �� ���� �������� �������������

			return checkResult;
		}

		/// ����� GetSideExternalNormal ���������� "�������" (�.�. ������������ ������ �������������) ������� � �����
		/// ����� �������� ����� ��������� vertex1, vertex2 � vertex3
		private Vector3D GetSideExternalNormal(IList<IPolyhedronVertex3D> vertexList,
		                                       IPolyhedronVertex3D vertex1,
		                                       IPolyhedronVertex3D vertex2,
		                                       IPolyhedronVertex3D vertex3)
		{
			if (ReferenceEquals(vertex1, vertex2) ||
			    ReferenceEquals(vertex1, vertex3) ||
			    ReferenceEquals(vertex2, vertex3))
			{
				throw new ArgumentException("vertex1, vertex2, vertex3 must be different");
			}

			// ������� � ��������� ��������� ����� ��������� ���������, ���������� ����� 3 �����
			// A = (y2-y1)*(z3-z1)-(z2-z1)*(y3-y1)
			Double x = (vertex2.YCoord - vertex1.YCoord)*(vertex3.ZCoord - vertex1.ZCoord) -
			           (vertex2.ZCoord - vertex1.ZCoord)*(vertex3.YCoord - vertex1.YCoord);
			// B = (z2-z1)*(x3-x1)-(x2-x1)*(z3-z1)
			Double y = (vertex2.ZCoord - vertex1.ZCoord)*(vertex3.XCoord - vertex1.XCoord) -
			           (vertex2.XCoord - vertex1.XCoord)*(vertex3.ZCoord - vertex1.ZCoord);
			// C = (x2-x1)*(y3-y1)-(y2-y1)*(x3-x1)
			Double z = (vertex2.XCoord - vertex1.XCoord)*(vertex3.YCoord - vertex1.YCoord) -
			           (vertex2.YCoord - vertex1.YCoord)*(vertex3.XCoord - vertex1.XCoord);
			Vector3D externalNormal = new Vector3D(x, y, z);
			// ��������� ���������� �������
			externalNormal = Vector3DUtils.NormalizeVector(externalNormal);

			// �������� ������-������ �� ����� ����� �� ������� �� ��������� � ����� ����� �� ���������
			// �.�. �� �������� � �������� ��������������, �� ���������� ������������ ������� ������� � ������������ ������-������� ������ ���� >0 (��� ��������)
			Double scalarProduct = 0;

			//for (Int32 vertexIndex = 0; ((scalarProduct == 0) && (vertexIndex < vertexList.Count)); ++vertexIndex)
			for (Int32 vertexIndex = 0;
			     (approxComparer.EQ(scalarProduct, 0) && vertexIndex < vertexList.Count);
			     ++vertexIndex)
			{
				IPolyhedronVertex3D currentVertex = vertexList[vertexIndex];
				scalarProduct = externalNormal.X*(vertex1.XCoord - currentVertex.XCoord) +
				                externalNormal.Y*(vertex1.YCoord - currentVertex.YCoord) +
				                externalNormal.Z*(vertex1.ZCoord - currentVertex.ZCoord);
			}
			//for (Int32 vertexIndex = 0; ((scalarProduct == 0) && (vertexIndex < vertexList.Count)); ++vertexIndex)

			//if (scalarProduct == 0)
			if (approxComparer.EQ(scalarProduct, 0))
			{
				throw new Exception("Can't calulate external normal for side !!!!");
			}

			if (scalarProduct < 0)
				externalNormal *= -1.0;

			return externalNormal;
		}

		// ����� OrderSideVertexList ������������� ������� �����, �������� � ��������������� ������ ������ sideVertexList
		// ������� ��������������� ���, ����� ��� ��������� ������ �.�. ���� �������� �� ����� � ����� "�������" ������� externalNormal � �����
		// ��� ���� ������ ��������������� ������ sideVertexList, ������������ � ����� OrderSideVertexList �������� ��� ���������
		// </summary>
		private List<IPolyhedronVertex3D> OrderSideVertexList(IEnumerable<IPolyhedronVertex3D> sideVertexList,
		                                                      Vector3D externalNormal)
		{
			Func<IPolyhedronVertex3D, IPolyhedronVertex3D, Vector3D> lineFormingVectorCreater =
				(vertex0, vertex1) =>
					{
						Double x = vertex1.XCoord - vertex0.XCoord;
						Double y = vertex1.YCoord - vertex0.YCoord;
						Double z = vertex1.ZCoord - vertex0.ZCoord;
						return Vector3DUtils.NormalizeVector(new Vector3D(x, y, z));
					};
			// ��������������� ������ ������
			List<IPolyhedronVertex3D> disorderVertexList = new List<IPolyhedronVertex3D>(sideVertexList);
			// ������������� ������ ������
			List<IPolyhedronVertex3D> orderVertexList = new List<IPolyhedronVertex3D>(disorderVertexList.Count);

			// ����� ������ ������� �� ������ ��������������� ������,
			// ���������� �� � ������ ������������� ������ � ������� �� �� ������ ��������������� ������
			orderVertexList.Add(disorderVertexList[0]);
			disorderVertexList.RemoveAt(0);

			// ���� ���� ������ ��������������� ������ �� ����
			while (disorderVertexList.Count > 0)
			{
				// ��������� ������� ��� ����������
				IPolyhedronVertex3D nextAddedVertex = disorderVertexList[0];
				// ��������� ����������� �������
				IPolyhedronVertex3D lastAddedVertex = orderVertexList[orderVertexList.Count - 1];

				// ���������� ������ ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
				Vector3D lineFormingVector = lineFormingVectorCreater(lastAddedVertex, nextAddedVertex);
				// "������" ������� (������� � ������� �����) � ������, ���������� ����� ��������� ����������� � ��������� ��� ���������� �������
				// "������" ������� � ������ �������� ��� ������ ���������� ������������ ����������� ������� ������ �� "�������" ������� � ������� �����
				Vector3D lineNormalVector = Vector3DUtils.VectorProduct(lineFormingVector, externalNormal);

				// ���� �� ���� �������� �� ������ ��������������� ������, ����� ������
				for (Int32 disorderedVertexIndex = 1;
				     disorderedVertexIndex < disorderVertexList.Count;
				     ++disorderedVertexIndex)
				{
					IPolyhedronVertex3D disorderedVertex = disorderVertexList[disorderedVertexIndex];
					// ��������� ������������ "������" ������� � ������ � ������ �������, ������������ �� ��������� ����������� � ������� �������
					Double scalarProduct = lineNormalVector.X*(disorderedVertex.XCoord - lastAddedVertex.XCoord) +
					                       lineNormalVector.Y*(disorderedVertex.YCoord - lastAddedVertex.YCoord) +
					                       lineNormalVector.Z*(disorderedVertex.ZCoord - lastAddedVertex.ZCoord);

					// if (ScalarProduct = 0)
					if (approxComparer.EQ(scalarProduct, 0))
					{
						throw new Exception("unexpected scalar product value !!! (scalar product value == 0)");
					}
					// if (ScalarProduct > 0)
					if (approxComparer.GT(scalarProduct, 0))
					{
						nextAddedVertex = disorderedVertex;
						lineFormingVector = lineFormingVectorCreater(lastAddedVertex, nextAddedVertex);

						// "������" ������� � ������ �������� ��� ������ ���������� ������������ ����������� ������� ������ �� "�������" ������� � ������� ����� (???????????????)
						lineNormalVector = Vector3DUtils.VectorProduct(lineFormingVector, externalNormal);
					}
				}
				// ���� �� ���� �������� �� ������ ��������������� ������, ����� ������

				orderVertexList.Add(nextAddedVertex);
				disorderVertexList.Remove(nextAddedVertex);
			}
			// ���� ���� ������ ��������������� ������ �� ����

			return orderVertexList;
		}

		private readonly ApproxComp approxComparer;

		private readonly VertexSidesDictionary vertexSidesDictionary;
	}
}