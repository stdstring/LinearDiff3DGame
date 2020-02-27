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
        /// <summary>
        /// конструктор класса Polyhedron3DFromPointsFactory
        /// </summary>
        /// <param name="approxComparer">сравниватель, для приближенного сравнения действительных чисел</param>
        public Polyhedron3DFromPointsFactory(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
            m_VertexSidesDictionary = new VertexSidesDictionary();
        }

        /// <summary>
        /// фабричный метод для получения структуры (граней, ребер) выпуклого многогранника по заданным вершинам
        /// </summary>
        /// <param name="vertexes">список заданных вершин</param>
        /// <returns>полученный выпуклый многогранник</returns>
        public Polyhedron3D CreatePolyhedron(IList<Point3D> vertexes)
        {
            List<PolyhedronSide3D> sideList = new List<PolyhedronSide3D>();
            List<PolyhedronVertex3D> vertexList = new List<PolyhedronVertex3D>();

            for(Int32 vertexIndex = 0; vertexIndex < vertexes.Count; ++vertexIndex)
                vertexList.Add(new PolyhedronVertex3D(vertexes[vertexIndex], vertexIndex));

            // Получение первой грани
            PolyhedronSide3D firstSide = GetFirstSide(vertexList);
            // Добавление полученной первой грани в список граней
            sideList.Add(firstSide);

            List<PolyhedronVertex3D> sideVertexList = new List<PolyhedronVertex3D>();

            // Цикл по всем граням из списка граней
            for(Int32 sideIndex = 0; sideIndex < sideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = sideList[sideIndex];

                // Цикл по всем ребрам текущей грани
                for(Int32 csVertexIndex = 0; csVertexIndex < currentSide.VertexList.Count; ++csVertexIndex)
                {
                    PolyhedronVertex3D leftEdgeVertex = currentSide.VertexList[csVertexIndex];
                    PolyhedronVertex3D rightEdgeVertex = currentSide.VertexList.GetNextItem(csVertexIndex);

                    // CurrentEdge: LeftEdgeVertex-RightEdgeVertex
                    Int32 sideCount4CurrentEdge = GetSideCount4Edge(leftEdgeVertex, rightEdgeVertex);
                    // Если текущее ребро принадлежит двум граням, то переход к следующей итерации
                    if(sideCount4CurrentEdge != 1) continue;

                    // Цикл по всем вершинам
                    for(Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
                    {
                        PolyhedronVertex3D currentVertex = vertexList[vertexIndex];

                        // Проверка образуют ли грань текущее ребро и текущая свободная вершина (при этом точки, принадлежащие проверяемой грани, добавляются в список вершин этой грани)
                        if(ReferenceEquals(leftEdgeVertex, currentVertex) ||
                           ReferenceEquals(rightEdgeVertex, currentVertex))
                            continue;
                        sideVertexList.Clear();
                        sideVertexList.Add(leftEdgeVertex);
                        sideVertexList.Add(rightEdgeVertex);
                        sideVertexList.Add(currentVertex);
                        Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
                        // Если проверка пройдена
                        if(checkResult)
                        {
                            // "внешняя" нормаль к грани
                            Vector3D externalNormal = GetSideExternalNormal(vertexList, leftEdgeVertex, rightEdgeVertex,
                                                                            currentVertex);
                            // Если новая грань совпадает с одной из ранее добавленных
                            if(IsSideAlreadyAdded(sideList, externalNormal)) continue;

                            // упорядоченный список вершин грани
                            List<PolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList,
                                                                                           externalNormal);
                            // ID грани
                            Int32 sideID = sideList[sideList.Count - 1].ID + 1;

                            PolyhedronSide3D newSide = new PolyhedronSide3D(orderVertexList, sideID, externalNormal);
                            sideList.Add(newSide);
                            // в словарь вершина - список граней добавляем все вершины новой грани + ее саму
                            foreach(PolyhedronVertex3D vertex in orderVertexList)
                                m_VertexSidesDictionary.AddSide4Vertex(vertex, newSide);
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

        /// <summary>
        /// метод GetSideCount4Edge возвращает количество граней, которым принадлежит заданное (по 2 вершинам) ребро
        /// </summary>
        /// <param name="edgeVertex1">первая вершина ребра</param>
        /// <param name="edgeVertex2">вторая вершина ребра</param>
        /// <returns>количество граней, которым принадлежит заданное ребро</returns>
        private Int32 GetSideCount4Edge(PolyhedronVertex3D edgeVertex1, PolyhedronVertex3D edgeVertex2)
        {
            Int32 sideCount4Edge = 0;
            IList<PolyhedronSide3D> edge1SideList = m_VertexSidesDictionary.GetSideList4Vertex(edgeVertex1);

            for(Int32 sideIndex = 0; sideIndex < edge1SideList.Count; ++sideIndex)
                if(edge1SideList[sideIndex].HasVertex(edgeVertex2)) ++sideCount4Edge;

            return sideCount4Edge;
        }

        /// <summary>
        /// метод IsSideAlreadyAdded проверяет была ли добавлена проверяемая грань в список граней
        /// при проверке сравниваются нормали граней
        /// </summary>
        /// <param name="sideList">список граней</param>
        /// <param name="externalNormal">"внешняя" нормаль проверяемой грани</param>
        /// <returns>true, если грань находится в списке граней, иначе false</returns>
        private Boolean IsSideAlreadyAdded(IList<PolyhedronSide3D> sideList, Vector3D externalNormal)
        {
            Boolean checkResult = false;

            // для проверки сравниваем "внешнюю" нормаль проверяемой грани с "внешними" нормалями уже добавленных в список граней
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
        /// метод GetFirstSide находи и возвращает первую грань многоранника
        /// см. алгоритм получения структуры (граней, ребер) выпуклого многогранника по заданным вершинам
        /// </summary>
        /// <param name="vertexList">список вершин конструируемого многогранника</param>
        /// <returns>первая грань многогранника</returns>
        private PolyhedronSide3D GetFirstSide(IList<PolyhedronVertex3D> vertexList)
        {
            PolyhedronSide3D firstSide = null;

            List<PolyhedronVertex3D> sideVertexList = new List<PolyhedronVertex3D>();

            // Берем первую вершину из списка вершин в качестве первой вершины (первой) грани
            PolyhedronVertex3D vertex1 = vertexList[0];
            // Цикл по всем вершинам (кроме первой) из списка вершин
            for(Int32 vertex2Index = 1; vertex2Index < vertexList.Count; ++vertex2Index)
            {
                // Берем текущую вершину в качестве второй вершины грани
                PolyhedronVertex3D vertex2 = vertexList[vertex2Index];

                // Цикл по всем вершинам после второй до конца 
                for(Int32 vertex3Index = vertex2Index + 1; vertex3Index < vertexList.Count; ++vertex3Index)
                {
                    // Текущая вершина становится третей вершиной грани
                    PolyhedronVertex3D vertex3 = vertexList[vertex3Index];

                    sideVertexList.Clear();
                    sideVertexList.Add(vertex1);
                    sideVertexList.Add(vertex2);
                    sideVertexList.Add(vertex3);

                    // Проверка образуют ли грань построенное ребро и третья вершина грани (при этом точки, принадлежащие проверяемой грани, добавляются в список вершин этой грани)
                    Boolean checkResult = DoesVertexesFormSide(vertexList, sideVertexList);
                    // Если проверка пройдена
                    if(checkResult)
                    {
                        // "внешняя" нормаль к грани
                        Vector3D externalNormal = GetSideExternalNormal(vertexList, vertex1, vertex2, vertex3);
                        // упорядоченный список вершин грани
                        List<PolyhedronVertex3D> orderVertexList = OrderSideVertexList(sideVertexList, externalNormal);

                        firstSide = new PolyhedronSide3D(orderVertexList, 0, externalNormal);
                        // в словарь вершина - список граней добавляем все вершины первой грани + ее саму
                        foreach(PolyhedronVertex3D vertex in orderVertexList)
                            m_VertexSidesDictionary.AddSide4Vertex(vertex, firstSide);

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

        /// <summary>
        /// метод DoesVertexesFormSide проверяет образуют ли грань (выпуклого многогранника) ребро и вершина (три вершины)
        /// при этом вершины, принадлежащие проверяемой грани заносятся в список вершин этой грани
        /// список вершин грани - sideVertexList; первые три вершины - вершины (ребро и вершина), по которым строится грань
        /// </summary>
        /// <param name="vertexList">список вершин конструируемого многогранника</param>
        /// <param name="sideVertexList">список вершин грани; первые три элемента - вершины (ребро и вершина), по которым строится грань</param>
        /// <returns>true - если ребро и вершина образуют грань, иначе false</returns>
        private Boolean DoesVertexesFormSide(IList<PolyhedronVertex3D> vertexList,
                                             IList<PolyhedronVertex3D> sideVertexList)
        {
            Boolean checkResult = true;

            Vector3D sideVector1 = Vector3D.ZeroVector3D;
            Vector3D sideVector2 = Vector3D.ZeroVector3D;

            // пусть первая точка из списка будет общим началом (точкой O)
            sideVector1.XCoord = sideVertexList[1].XCoord - sideVertexList[0].XCoord;
            sideVector1.YCoord = sideVertexList[1].YCoord - sideVertexList[0].YCoord;
            sideVector1.ZCoord = sideVertexList[1].ZCoord - sideVertexList[0].ZCoord;
            sideVector2.XCoord = sideVertexList[2].XCoord - sideVertexList[0].XCoord;
            sideVector2.YCoord = sideVertexList[2].YCoord - sideVertexList[0].YCoord;
            sideVector2.ZCoord = sideVertexList[2].ZCoord - sideVertexList[0].ZCoord;

            // Знак смешанного произведения, вычисленного в первый раз (для смешанного произведения не равного 0)
            Int32 mixedProductSign = 0;
            // Цикл по всем вершинам многогранника
            for(Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D currentVertex = vertexList[vertexIndex];

                // Если текущая вершина совпадает с одной из трех, образующих проверяемую грань
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

                // Если смешанное произведение == 0
                if(m_ApproxComparer.EQ(mixedProduct, 0))
                    sideVertexList.Add(currentVertex);
                    // Если смешанное произведение != 0
                else
                {
                    // если смешанное произведение было вычислено в первый раз
                    if(mixedProductSign == 0)
                        mixedProductSign = Math.Sign(mixedProduct);
                        // иначе ... если знак смешанного произведения не совпадает с запомненным
                    else if(mixedProductSign != Math.Sign(mixedProduct))
                    {
                        checkResult = false;
                        break;
                    }
                }
            }
            // Цикл по всем вершинам многогранника

            return checkResult;
        }

        /// <summary>
        /// метод GetSideExternalNormal возвращает "внешнюю" (т.е. направленную наружу многогранника) нормаль к грани
        /// грань задается тремя вершинами vertex1, vertex2 и vertex3
        /// </summary>
        /// <param name="vertexList">список вершин конструируемого многогранника</param>
        /// <param name="vertex1">вершина грани vertex1</param>
        /// <param name="vertex2">вершина грани vertex2</param>
        /// <param name="vertex3">вершина грани vertex3</param>
        /// <returns>"внешняя" нормаль к грани, заданной тремя вершинами vertex1, vertex2 и vertex3</returns>
        private Vector3D GetSideExternalNormal(IList<PolyhedronVertex3D> vertexList, PolyhedronVertex3D vertex1,
                                               PolyhedronVertex3D vertex2, PolyhedronVertex3D vertex3)
        {
            Vector3D externalNormal = Vector3D.ZeroVector3D;

            if(ReferenceEquals(vertex1, vertex2) ||
               ReferenceEquals(vertex1, vertex3) ||
               ReferenceEquals(vertex2, vertex3))
            {
#warning может более специализированное исключение
                throw new ArgumentException("vertex1, vertex2, vertex3 must be different");
            }

            // нормаль к плоскости вычисляем через уравнение плоскости, проходящее через 3 точки
            // A = (y2-y1)*(z3-z1)-(z2-z1)*(y3-y1)
            externalNormal.XCoord = (vertex2.YCoord - vertex1.YCoord) * (vertex3.ZCoord - vertex1.ZCoord) -
                                    (vertex2.ZCoord - vertex1.ZCoord) * (vertex3.YCoord - vertex1.YCoord);
            // B = (z2-z1)*(x3-x1)-(x2-x1)*(z3-z1)
            externalNormal.YCoord = (vertex2.ZCoord - vertex1.ZCoord) * (vertex3.XCoord - vertex1.XCoord) -
                                    (vertex2.XCoord - vertex1.XCoord) * (vertex3.ZCoord - vertex1.ZCoord);
            // C = (x2-x1)*(y3-y1)-(y2-y1)*(x3-x1)
            externalNormal.ZCoord = (vertex2.XCoord - vertex1.XCoord) * (vertex3.YCoord - vertex1.YCoord) -
                                    (vertex2.YCoord - vertex1.YCoord) * (vertex3.XCoord - vertex1.XCoord);

            // нормируем полученную нормаль
            externalNormal = Vector3DUtils.NormalizeVector(externalNormal);

            // построим радиус-вектор из любой точки не лежащей на плоскости в любую точку на плоскости
            // т.к. мы работаем с выпуклым многогранником, то скалярного произведение внешней нормали и построенного радиус-вектора должно быть >0 (что очевидно)
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
#warning может более специализированное исключение
                throw new Exception("Can't calulate external normal for side !!!!");
            }

            if(scalarProduct < 0)
                externalNormal *= -1.0;

            return externalNormal;
        }

        /// <summary>
        /// метод OrderSideVertexList упорядочивает вершины грани, заданные в неупорядоченном списке вершин sideVertexList
        /// вершины упорядочиваются так, чтобы они следовали против ч.с. если смотреть на грань с конца "внешней" нормали externalNormal к грани
        /// при этом список неупорядоченных вершин sideVertexList, передаваемый в метод OrderSideVertexList остается без изменений
        /// </summary>
        /// <param name="sideVertexList">неупорядоченный список вершин</param>
        /// <param name="externalNormal">внешняя нормаль к грани</param>
        /// <returns>список упорядоченных вершин</returns>
        private List<PolyhedronVertex3D> OrderSideVertexList(IEnumerable<PolyhedronVertex3D> sideVertexList,
                                                             Vector3D externalNormal)
        {
            // неупорядоченный список вершин
            List<PolyhedronVertex3D> disorderVertexList = new List<PolyhedronVertex3D>(sideVertexList);
            // упорядоченный список вершин
            List<PolyhedronVertex3D> orderVertexList = new List<PolyhedronVertex3D>(disorderVertexList.Count);

            // Берем первую вершину из списка неупорядоченных вершин,
            // записываем ее в список упорядоченных вершин и удаляем ее из списка неупорядоченных вершин
            orderVertexList.Add(disorderVertexList[0]);
            disorderVertexList.RemoveAt(0);

            // Цикл пока список неупорядоченных вершин не пуст
            while(disorderVertexList.Count > 0)
            {
                // Следующая вершина для добавления
                PolyhedronVertex3D nextAddedVertex = disorderVertexList[0];
                // Последняя добавленная вершина
                PolyhedronVertex3D lastAddedVertex = orderVertexList[orderVertexList.Count - 1];

                // образующий вектор прямой, проходящей через последнюю добавленную и следующую для добавлении вершину
                Vector3D lineFormingVector = Vector3D.ZeroVector3D;
                lineFormingVector.XCoord = nextAddedVertex.XCoord - lastAddedVertex.XCoord;
                lineFormingVector.YCoord = nextAddedVertex.YCoord - lastAddedVertex.YCoord;
                lineFormingVector.ZCoord = nextAddedVertex.ZCoord - lastAddedVertex.ZCoord;
                lineFormingVector = Vector3DUtils.NormalizeVector(lineFormingVector);
                // "правая" нормаль (лежащая в текущей грани) к прямой, проходящей через последнюю добавленную и следующую для добавлении вершину
                // "правую" нормаль к прямой получаем при помощи векторного произведения образующего вектора прямой на "внешнюю" нормаль к текущей грани
                Vector3D lineNormalVector = Vector3D.VectorProduct(lineFormingVector, externalNormal);

                // Цикл по всем вершинам из списка неупорядоченных вершин, кроме первой
                for(Int32 disorderedVertexIndex = 1;
                    disorderedVertexIndex < disorderVertexList.Count;
                    ++disorderedVertexIndex)
                {
                    PolyhedronVertex3D disorderedVertex = disorderVertexList[disorderedVertexIndex];
                    // скалярное произведение "правой" нормали к прямой и радиус вектора, проведенного из последней добавленной в текущую вершину
                    Double scalarProduct = lineNormalVector.XCoord * (disorderedVertex.XCoord - lastAddedVertex.XCoord) +
                                           lineNormalVector.YCoord * (disorderedVertex.YCoord - lastAddedVertex.YCoord) +
                                           lineNormalVector.ZCoord * (disorderedVertex.ZCoord - lastAddedVertex.ZCoord);

                    // if (ScalarProduct = 0)
                    if(m_ApproxComparer.EQ(scalarProduct, 0))
                    {
#warning может более специализированное исключение
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

                        // "правую" нормаль к прямой получаем при помощи векторного произведения образующего вектора прямой на "внешнюю" нормаль к текущей грани (???????????????)
                        lineNormalVector = Vector3D.VectorProduct(lineFormingVector, externalNormal);
                    }
                }
                // Цикл по всем вершинам из списка неупорядоченных вершин, кроме первой

                orderVertexList.Add(nextAddedVertex);
                disorderVertexList.Remove(nextAddedVertex);
            }
            // Цикл пока список неупорядоченных вершин не пуст

            return orderVertexList;
        }

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;

        /// <summary>
        /// словарь вершина - список граней
        /// </summary>
        private readonly VertexSidesDictionary m_VertexSidesDictionary;
    }
}