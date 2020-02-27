using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.Geometry3D.PolyhedronFactory
{
	/// <summary>
	/// фабрика для получения структуры (граней, ребер) выпуклого многогранника по заданным вершинам
	/// </summary>
	public class Polyhedron3DFromPointsFactory
	{
		public Polyhedron3DFromPointsFactory(ApproxComp approxComparer)
		{
			this.approxComparer = approxComparer;
			vertexSidesDictionary = new VertexSidesDictionary();
		}

		// фабричный метод для получения структуры (граней, ребер) выпуклого многогранника по заданным вершинам
		public IPolyhedron3D CreatePolyhedron(IList<Point3D> vertexes)
		{
			List<IPolyhedronSide3D> sideList = new List<IPolyhedronSide3D>();
			List<IPolyhedronVertex3D> vertexList = new List<IPolyhedronVertex3D>();

			for (Int32 vertexIndex = 0; vertexIndex < vertexes.Count; ++vertexIndex)
				vertexList.Add(new PolyhedronVertex3D(vertexes[vertexIndex], vertexIndex));

			// Получение первой грани
			IPolyhedronSide3D firstSide = GetFirstSide(vertexList);
			// Добавление полученной первой грани в список граней
			sideList.Add(firstSide);

			List<IPolyhedronVertex3D> sideVertexList = new List<IPolyhedronVertex3D>();

			// Цикл по всем граням из списка граней
			for (Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
			{
				IPolyhedronSide3D currentSide = sideList[sideIndex];

				// Цикл по всем ребрам текущей грани
				for (Int32 csVertexIndex = 0; csVertexIndex < currentSide.VertexList.Count; ++csVertexIndex)
				{
					IPolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[csVertexIndex];
					IPolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(csVertexIndex);

					// CurrentEdge: LeftEdgeVertex-RightEdgeVertex
					Int32 sideCount4CurrentEdge = GetSideCount4Edge(leftEdgeVertex, rightEdgeVertex);
					// Если текущее ребро принадлежит двум граням, то переход к следующей итерации
					if (sideCount4CurrentEdge != 1) continue;

					// Цикл по всем вершинам
					for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
					{
						IPolyhedronVertex3D currentVertex = vertexList[vertexIndex];

						// Проверка образуют ли грань текущее ребро и текущая свободная вершина (при этом точки, принадлежащие проверяемой грани, добавляются в список вершин этой грани)
						if (ReferenceEquals(leftEdgeVertex, currentVertex) ||
						    ReferenceEquals(rightEdgeVertex, currentVertex))
							continue;
						sideVertexList.Clear();
						sideVertexList.Add(leftEdgeVertex);
						sideVertexList.Add(rightEdgeVertex);
						sideVertexList.Add(currentVertex);
						Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
						// Если проверка пройдена
						if (checkResult)
						{
							// "внешняя" нормаль к грани
							Vector3D externalNormal = GetSideExternalNormal(vertexList, leftEdgeVertex, rightEdgeVertex,
							                                                currentVertex);
							// Если новая грань совпадает с одной из ранее добавленных
							if (IsSideAlreadyAdded(sideList, externalNormal)) continue;

							// упорядоченный список вершин грани
							List<IPolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList,
							                                                                externalNormal);
							// ID грани
							Int32 sideID = sideList[sideList.Count - 1].ID + 1;

							IPolyhedronSide3D newSide = new PolyhedronSide3D(orderVertexList, sideID, externalNormal);
							sideList.Add(newSide);
							// в словарь вершина - список граней добавляем все вершины новой грани + ее саму
							foreach (IPolyhedronVertex3D vertex in orderVertexList)
								vertexSidesDictionary.AddSide4Vertex(vertex, newSide);
							// Выход из цикла по свободным вершинам
							break;
						}
					}
					// Цикл по всем вершинам
				}
				// Цикл по всем ребрам текущей грани
			}
			// Цикл по всем граням из списка граней

			return new Polyhedron3D(sideList, vertexList);
		}

		// метод GetSideCount4Edge возвращает количество граней, которым принадлежит заданное (по 2 вершинам) ребро
		private Int32 GetSideCount4Edge(IPolyhedronVertex3D edgeVertex1, IPolyhedronVertex3D edgeVertex2)
		{
			Int32 sideCount4Edge = 0;
			IList<IPolyhedronSide3D> edge1SideList = vertexSidesDictionary.GetSideList4Vertex(edgeVertex1);

			for (Int32 sideIndex = 0; sideIndex < edge1SideList.Count; ++sideIndex)
				if (edge1SideList[sideIndex].VertexList.Contains(edgeVertex2)) ++sideCount4Edge;

			return sideCount4Edge;
		}

		// метод IsSideAlreadyAdded проверяет была ли добавлена проверяемая грань в список граней; при проверке сравниваются нормали граней
		private Boolean IsSideAlreadyAdded(IList<IPolyhedronSide3D> sideList, Vector3D externalNormal)
		{
			Boolean checkResult = false;

			// для проверки сравниваем "внешнюю" нормаль проверяемой грани с "внешними" нормалями уже добавленных в список граней
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

		// метод GetFirstSide находи и возвращает первую грань многоранника (см. алгоритм получения структуры (граней, ребер) выпуклого многогранника по заданным вершинам)
		private IPolyhedronSide3D GetFirstSide(IList<IPolyhedronVertex3D> vertexList)
		{
			IPolyhedronSide3D firstSide = null;

			List<IPolyhedronVertex3D> sideVertexList = new List<IPolyhedronVertex3D>();

			// Берем первую вершину из списка вершин в качестве первой вершины (первой) грани
			IPolyhedronVertex3D vertex1 = vertexList[0];
			// Цикл по всем вершинам (кроме первой) из списка вершин
			for (Int32 vertex2Index = 1; vertex2Index < vertexList.Count; ++vertex2Index)
			{
				// Берем текущую вершину в качестве второй вершины грани
				IPolyhedronVertex3D vertex2 = vertexList[vertex2Index];

				// Цикл по всем вершинам после второй до конца 
				for (Int32 vertex3Index = vertex2Index + 1; vertex3Index < vertexList.Count; ++vertex3Index)
				{
					// Текущая вершина становится третей вершиной грани
					IPolyhedronVertex3D vertex3 = vertexList[vertex3Index];

					sideVertexList.Clear();
					sideVertexList.Add(vertex1);
					sideVertexList.Add(vertex2);
					sideVertexList.Add(vertex3);

					// Проверка образуют ли грань построенное ребро и третья вершина грани (при этом точки, принадлежащие проверяемой грани, добавляются в список вершин этой грани)
					Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
					// Если проверка пройдена
					if (checkResult)
					{
						// "внешняя" нормаль к грани
						Vector3D externalNormal = GetSideExternalNormal(vertexList, vertex1, vertex2, vertex3);
						// упорядоченный список вершин грани
						List<IPolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList, externalNormal);

						firstSide = new PolyhedronSide3D(orderVertexList, 0, externalNormal);
						// в словарь вершина - список граней добавляем все вершины первой грани + ее саму
						foreach (IPolyhedronVertex3D vertex in orderVertexList)
							vertexSidesDictionary.AddSide4Vertex(vertex, firstSide);

						// не очень красивый выход из метода !!!!!!!!!
						return firstSide;
					}
					// Если проверка пройдена
				}
				// Цикл по всем вершинам после второй до конца 
			}
			// Цикл по всем вершинам (кроме первой) из списка вершин

			return firstSide;
		}

		// метод DoesVertexesFormSide проверяет образуют ли грань (выпуклого многогранника) ребро и вершина (три вершины)
		// при этом вершины, принадлежащие проверяемой грани заносятся в список вершин этой грани
		// список вершин грани - sideVertexList; первые три вершины - вершины (ребро и вершина), по которым строится грань
		private Boolean DoesVertexesFormSide(IList<IPolyhedronVertex3D> vertexList,
		                                     IList<IPolyhedronVertex3D> sideVertexList)
		{
			Boolean checkResult = true;

			// пусть первая точка из списка будет общим началом (точкой O)
			Double x1 = sideVertexList[1].XCoord - sideVertexList[0].XCoord;
			Double y1 = sideVertexList[1].YCoord - sideVertexList[0].YCoord;
			Double z1 = sideVertexList[1].ZCoord - sideVertexList[0].ZCoord;
			Vector3D sideVector1 = new Vector3D(x1, y1, z1);
			Double x2 = sideVertexList[2].XCoord - sideVertexList[0].XCoord;
			Double y2 = sideVertexList[2].YCoord - sideVertexList[0].YCoord;
			Double z2 = sideVertexList[2].ZCoord - sideVertexList[0].ZCoord;
			Vector3D sideVector2 = new Vector3D(x2, y2, z2);

			// Знак смешанного произведения, вычисленного в первый раз (для смешанного произведения не равного 0)
			Int32 mixedProductSign = 0;
			// Цикл по всем вершинам многогранника
			for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
			{
				IPolyhedronVertex3D currentVertex = vertexList[vertexIndex];

				// Если текущая вершина совпадает с одной из трех, образующих проверяемую грань
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

				// Если смешанное произведение == 0
				if (approxComparer.EQ(mixedProduct, 0))
					sideVertexList.Add(currentVertex);
					// Если смешанное произведение != 0
				else
				{
					// если смешанное произведение было вычислено в первый раз
					if (mixedProductSign == 0)
						mixedProductSign = Math.Sign(mixedProduct);
						// иначе ... если знак смешанного произведения не совпадает с запомненным
					else if (mixedProductSign != Math.Sign(mixedProduct))
					{
						checkResult = false;
						break;
					}
				}
			}
			// Цикл по всем вершинам многогранника

			return checkResult;
		}

		/// метод GetSideExternalNormal возвращает "внешнюю" (т.е. направленную наружу многогранника) нормаль к грани
		/// грань задается тремя вершинами vertex1, vertex2 и vertex3
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

			// нормаль к плоскости вычисляем через уравнение плоскости, проходящее через 3 точки
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
			// нормируем полученную нормаль
			externalNormal = Vector3DUtils.NormalizeVector(externalNormal);

			// построим радиус-вектор из любой точки не лежащей на плоскости в любую точку на плоскости
			// т.к. мы работаем с выпуклым многогранником, то скалярного произведение внешней нормали и построенного радиус-вектора должно быть >0 (что очевидно)
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

		// метод OrderSideVertexList упорядочивает вершины грани, заданные в неупорядоченном списке вершин sideVertexList
		// вершины упорядочиваются так, чтобы они следовали против ч.с. если смотреть на грань с конца "внешней" нормали externalNormal к грани
		// при этом список неупорядоченных вершин sideVertexList, передаваемый в метод OrderSideVertexList остается без изменений
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
			// неупорядоченный список вершин
			List<IPolyhedronVertex3D> disorderVertexList = new List<IPolyhedronVertex3D>(sideVertexList);
			// упорядоченный список вершин
			List<IPolyhedronVertex3D> orderVertexList = new List<IPolyhedronVertex3D>(disorderVertexList.Count);

			// Берем первую вершину из списка неупорядоченных вершин,
			// записываем ее в список упорядоченных вершин и удаляем ее из списка неупорядоченных вершин
			orderVertexList.Add(disorderVertexList[0]);
			disorderVertexList.RemoveAt(0);

			// Цикл пока список неупорядоченных вершин не пуст
			while (disorderVertexList.Count > 0)
			{
				// Следующая вершина для добавления
				IPolyhedronVertex3D nextAddedVertex = disorderVertexList[0];
				// Последняя добавленная вершина
				IPolyhedronVertex3D lastAddedVertex = orderVertexList[orderVertexList.Count - 1];

				// образующий вектор прямой, проходящей через последнюю добавленную и следующую для добавлении вершину
				Vector3D lineFormingVector = lineFormingVectorCreater(lastAddedVertex, nextAddedVertex);
				// "правая" нормаль (лежащая в текущей грани) к прямой, проходящей через последнюю добавленную и следующую для добавлении вершину
				// "правую" нормаль к прямой получаем при помощи векторного произведения образующего вектора прямой на "внешнюю" нормаль к текущей грани
				Vector3D lineNormalVector = Vector3DUtils.VectorProduct(lineFormingVector, externalNormal);

				// Цикл по всем вершинам из списка неупорядоченных вершин, кроме первой
				for (Int32 disorderedVertexIndex = 1;
				     disorderedVertexIndex < disorderVertexList.Count;
				     ++disorderedVertexIndex)
				{
					IPolyhedronVertex3D disorderedVertex = disorderVertexList[disorderedVertexIndex];
					// скалярное произведение "правой" нормали к прямой и радиус вектора, проведенного из последней добавленной в текущую вершину
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

						// "правую" нормаль к прямой получаем при помощи векторного произведения образующего вектора прямой на "внешнюю" нормаль к текущей грани (???????????????)
						lineNormalVector = Vector3DUtils.VectorProduct(lineFormingVector, externalNormal);
					}
				}
				// Цикл по всем вершинам из списка неупорядоченных вершин, кроме первой

				orderVertexList.Add(nextAddedVertex);
				disorderVertexList.Remove(nextAddedVertex);
			}
			// Цикл пока список неупорядоченных вершин не пуст

			return orderVertexList;
		}

		private readonly ApproxComp approxComparer;

		private readonly VertexSidesDictionary vertexSidesDictionary;
	}
}