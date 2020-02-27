using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    
    /// <summary>
    /// выпуклый 3D многогранник
    /// </summary>
    [Serializable]
    public class ConvexPolyhedron3D : ISerializable
    {
        /// <summary>
        /// контейнер с данными грани для операций сериализации/десериализации
        /// </summary>
        [Serializable]
        private struct SideSerializationData
        {
            /// <summary>
            /// координаты внешней нормали к грани
            /// </summary>
            public Double[] SideNormalData;
            /// <summary>
            /// список индексов вершин, принадлежащих грани (вершины упорядочены против ч.с., если смотреть с конца внешней нормали к грани)
            /// </summary>
            public Int32[] SideVertexIndexList;
        }

        /// <summary>
        /// конструктор класса ConvexPolyhedron3D
        /// </summary>
        /// <param name="approxComparer">сравниватель для операций приблизительного сравнения действительных чисел</param>
        public ConvexPolyhedron3D(ApproxComparer approxComparer)
        {
            m_ApproxComparer = approxComparer;

            m_SideList = new List<SideClass>();
            m_VertexList = new List<VertexClass>();
        }

        /// <summary>
        /// метод IsPointInside проверяет, является ли точка point внутренней для данного многогранника
        /// </summary>
        /// <param name="point">рассматриваемая точка</param>
        /// <returns>true, если точка point находится внутри данного многогранника; иначе false</returns>
        public Boolean IsPointInside(Point3D point)
        {
            // проверять находится ли точка внутри или снаружи многогранника можно следующим образом:
            // строим радиус-вектор из проверяемой точки в любую точку (например в любую вершину) каждой грани
            // если скалярное произведение таких радиус векторов и внешних нормалей к соответствующим граням
            // >= 0 для каждой грани, то данная точка внутренняя (или лежит на границе многогранника);
            // иначе точка лежит снаружи многогранника
            foreach (SideClass currentSide in m_SideList)
            {
                Vector3D raduisVector = new Vector3D(currentSide[0].XCoord - point.XCoord,
                                                     currentSide[0].YCoord - point.YCoord,
                                                     currentSide[0].ZCoord - point.ZCoord);
                Double scalarProduct = Vector3D.ScalarProduct(raduisVector, currentSide.SideNormal);
                // требуем, чтобы точка была чисто внутренней ... для этого скалярное произведение должно всегда быть > 0
                // if (scalarProdact <= 0)
                if (m_ApproxComparer.LE(scalarProduct, 0))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// метод IsPolyhedronInside проверяет, лежит ли выпуклый многогранник checkedPolyhedron внутри данного
        /// </summary>
        /// <param name="checkedPolyhedron">проверяемый многогранник</param>
        /// <returns>true, если выпуклый многогранник checkedPolyhedron лежит внутри данного ; иначе - false</returns>
        public Boolean IsPolyhedronInside(ConvexPolyhedron3D checkedPolyhedron)
        {
            // если все вершины выпуклого многогранника checkedPolyhedron лежат внутри данного многогранника
            // то и сам многогранник checkedPolyhedron лежит внутри данного
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
        /// метод IsPolyhedronConvex проверяет данный многогранник на выпуклость
        /// </summary>
        /// <returns>true, если данный многогранник выпуклый; иначе - false</returns>
        public Boolean IsPolyhedronConvex()
        {
            // если многогранник выпуклый, то для каждой грани скалярное произвеление внешней нормали к этой грани
            // и радиус-вектора, проведенного из любой вершины грани ко всем остальным вершинам <= 0
            foreach (SideClass side in m_SideList)
            {
                Vector3D sideNormal = side.SideNormal;
                VertexClass startVertex = side[0];

                foreach (VertexClass vertex in m_VertexList)
                {
                    Vector3D radiusVector = new Vector3D(vertex.XCoord - startVertex.XCoord,
                                                         vertex.YCoord - startVertex.YCoord,
                                                         vertex.ZCoord - startVertex.ZCoord);
                    Double scalarProduct = Vector3D.ScalarProduct(sideNormal, radiusVector);

                    //if (scalarProduct > 0)
                    if (m_ApproxComparer.GT(scalarProduct, 0)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// метод GetScaledPolyhedron возвращает отмаштабированный (подобный данному) многогранник
        /// коэффициент маштабирования - scaleKoeff
        /// </summary>
        /// <param name="scaleKoeff">коэффициент маштабирования</param>
        /// <returns>отмаштабированный (подобный данному) многогранник</returns>
        public ConvexPolyhedron3D GetScaledPolyhedron(Double scaleKoeff)
        {
            if (scaleKoeff <= 0)
            {
                throw new ArgumentException("scaleKoeff must be > 0 !!!");
            }

            ConvexPolyhedron3D scaledPolyhedron = new ConvexPolyhedron3D(m_ApproxComparer);

            // заполняем список вершин в scaledPolyhedron
            for (Int32 vertexIndex = 0; vertexIndex < m_VertexList.Count; vertexIndex++)
            {
                VertexClass currentVertex = m_VertexList[vertexIndex];
                scaledPolyhedron.m_VertexList.Add(new VertexClass(currentVertex.XCoord * scaleKoeff,
                                                                  currentVertex.YCoord * scaleKoeff,
                                                                  currentVertex.ZCoord * scaleKoeff,
                                                                  currentVertex.ID));
            }

            // заполняем список граней в scaledPolyhedron
            for (Int32 sideIndex = 0; sideIndex < m_SideList.Count; sideIndex++)
            {
                SideClass currentSide = m_SideList[sideIndex];

                List<VertexClass> vertexList = new List<VertexClass>();
                for (Int32 vertexIndex = 0; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    // ID вершины = индекс вершины + 1
                    vertexList.Add(scaledPolyhedron.m_VertexList[currentSide[vertexIndex].ID - 1]);
                }

                scaledPolyhedron.m_SideList.Add(new SideClass(vertexList, currentSide.ID, currentSide.SideNormal));
            }

            return scaledPolyhedron;
        }

        /// <summary>
        /// метод GetCrossingPointWithRay_FullEmun возвращает точку пересечения луча, выходящего из точки O
        /// и проходящего через заданную точку givenPoint, и данного многогранника
        /// точка пересечения ищется методом полного перебора по всем граням
        /// </summary>
        /// <param name="givenPoint">заданная точка, через которую проходит луч, начинающийся в точке O</param>
        /// <returns>точка пересечения</returns>
        public Point3D GetCrossingPointWithRay_FullEmun(Point3D givenPoint)
        {
            // точка O (начало координат)
            Point3D pointO = new Point3D(0, 0, 0);
            // текущая точка пересечения
            Point3D crossingPoint = new Point3D();
            // расстояние между текущей точкой пересечения и точкой O
            Double distance2CrossingPoint = Double.NaN;

            // полный перебор по всем граням
            foreach (SideClass currentSide in m_SideList)
            {
                // если скалярное произведение нормали текущей грани и радиус вектора точки givenPoint <= 0
                // то данную грань не рассматриваем, т.к. она не имеет точки пересечения с рассматриваемым лучом
                Double scalarProduct = currentSide.SideNormal.XCoord * givenPoint.XCoord +
                                       currentSide.SideNormal.YCoord * givenPoint.YCoord +
                                       currentSide.SideNormal.ZCoord * givenPoint.ZCoord;
                // if (scalarProduct <= 0)
                if (m_ApproxComparer.LE(scalarProduct, 0))
                {
                    continue;
                }

                PlaneClass currentPlane = new PlaneClass(currentSide.SideNormal, SupportFunc(currentSide.SideNormal));

                // возможная точка пересечения
                Point3D newCrossingPoint = AdvMath.CalcPlaneLineCrossingPoint(m_ApproxComparer, currentPlane, pointO, givenPoint);
                // расстояние точками newCrossingPoint и O
                Double distance2NewCrossingPoint = AdvMath.DistanceBetween2Points(pointO, newCrossingPoint);

                if ((Double.IsNaN(distance2CrossingPoint)) || (distance2NewCrossingPoint < distance2CrossingPoint))
                {
                    crossingPoint = newCrossingPoint;
                    distance2CrossingPoint = distance2NewCrossingPoint;
                }
            }

            return crossingPoint;
        }

        /// <summary>
        /// метод GetCrossingPointWithRay возвращает точку пересечения луча, выходящего из точки O
        /// и проходящего через заданную точку givenPoint, и данного многогранника
        /// точка пересечения ищется (поиском глобального минимума расстояния от точки O до точки пересечения ???)
        /// </summary>
        /// <param name="givenPoint">заданная точка, через которую проходит луч, начинающийся в точке O</param>
        /// <returns>точка пересечения</returns>
        public Point3D GetCrossingPointWithRay(Point3D givenPoint)
        {
            throw new NotImplementedException("!!!");
        }

        /// <summary>
        /// метод GetNearestPoint4Given_FullEmun возвращает наиболее близкую к точке givenPoint точку на поверхности многогранника
        /// полагаем при этом, что точка givenPoint лежит вне многогранника
        /// ближайшая точка ищется методом полного перебора по всем граням
        /// </summary>
        /// <param name="givenPoint">заданная точка, для которой ищется ближайшая на поверхности многогранника точка</param>
        /// <returns>ближайшая точка к givenPoint на поверхности многогранника</returns>
        public Point3D GetNearestPoint4Given_FullEmun(Point3D givenPoint)
        {
            // текущая ближайшая точка
            Point3D nearestPoint = new Point3D();
            // расстояние между текущей ближайшей точкой и точкой givenPoint
            Double distance2NearestPoint = Double.NaN;

            // полный перебор по всем граням
            foreach (SideClass currentSide in m_SideList)
            {
                // если скалярное произведение нормали текущей грани и вектора из точки givenPoint 
                // в любую вершину грани (например с индексом 0) >= 0, то данную грань не рассматриваем,
                // т.к. она находится с обратной стороны многогранника относительно точкки givenPoint
                Double scalarProduct = currentSide.SideNormal.XCoord * (currentSide[0].XCoord - givenPoint.XCoord) +
                                       currentSide.SideNormal.YCoord * (currentSide[0].YCoord - givenPoint.YCoord) +
                                       currentSide.SideNormal.ZCoord * (currentSide[0].ZCoord - givenPoint.ZCoord);
                // if (scalarProduct >= 0)
                if (m_ApproxComparer.GE(scalarProduct, 0))
                {
                    continue;
                }

                // полный перебор по всем треугольникам на которые можно разбить грань
                // грань на треугольники разбивается следующим образом: (вершина 0, вершина i-1, вершина i)
                // где i = 2 ... VertecCount-1
                VertexClass firstTriangleVertex = currentSide[0];
                for (Int32 vertexIndex = 2; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    VertexClass secondTriangleVertex = currentSide[vertexIndex - 1];
                    VertexClass thirdTriangleVertex = currentSide[vertexIndex];

                    Boolean isTrueNearestPoint = false;
                    // возможная ближайшая точка
                    Point3D newNearestPoint = CalcNearestPointOnTriangle(firstTriangleVertex,
                                                                         secondTriangleVertex,
                                                                         thirdTriangleVertex,
                                                                         currentSide.SideNormal,
                                                                         givenPoint,
                                                                         out isTrueNearestPoint);
                    // возможная ближайшая точка является истинной ближайшей точкой => конец работы алгоритма
                    if (isTrueNearestPoint)
                    {
                        return newNearestPoint;
                    }

                    // расстояние между заданной точкой givenPoint и возможной ближайшей точкой
                    Double distance2NewNearestPoint = AdvMath.DistanceBetween2Points(givenPoint, newNearestPoint);
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
        /// метод GetNearestPoint4Given возвращает наиболее близкую к точке givenPoint точку на поверхности многогранника
        /// полагаем при этом, что точка givenPoint лежит вне многогранника
        /// ближайшая точка ищется (поиском глобального минимума расстояния от точки givenPoint до точки на поверхности многогранника ???)
        /// </summary>
        /// <param name="givenPoint">заданная точка, для которой ищется ближайшая на поверхности многогранника точка</param>
        /// <returns>ближайшая точка к givenPoint на поверхности многогранника</returns>
        public Point3D GetNearestPoint4Given(Point3D givenPoint)
        {
            throw new NotImplementedException("!!!");
        }

        /// <summary>
        /// метод ToPoint3DArray представляет многогранник в виде массива его вершин
        /// </summary>
        /// <returns>многогранник в виде массива его вершин</returns>
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
        /// количество граней в данном многограннике
        /// </summary>
        public Int32 SideCount
        {
            get
            {
                return m_SideList.Count;
            }
        }

        /// <summary>
        /// грань, распологающаяся в списке граней по индексу sideIndex
        /// </summary>
        /// <param name="sideIndex"></param>
        /// <returns></returns>
        public SideClass GetSide(Int32 sideIndex)
        {
            return m_SideList[sideIndex];
        }

        /// <summary>
        /// количество вершин в данном многограннике
        /// </summary>
        public Int32 VertexCount
        {
            get
            {
                return m_VertexList.Count;
            }
        }

        /// <summary>
        /// вершина, распологающаяся в списке вершин по индексу sideIndex
        /// </summary>
        /// <param name="vertexIndex"></param>
        /// <returns></returns>
        public VertexClass GetVertex(Int32 vertexIndex)
        {
            return m_VertexList[vertexIndex];
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Double[] vertexesData = new Double[m_VertexList.Count * 3];
            for (Int32 vertexIndex = 0; vertexIndex < m_VertexList.Count; vertexIndex++)
            {
                vertexesData[vertexIndex * 3] = m_VertexList[vertexIndex].XCoord;
                vertexesData[vertexIndex * 3 + 1] = m_VertexList[vertexIndex].YCoord;
                vertexesData[vertexIndex * 3 + 2] = m_VertexList[vertexIndex].ZCoord;
            }

            SideSerializationData[] sidesData = new SideSerializationData[m_SideList.Count];
            for (Int32 sideIndex = 0; sideIndex < m_SideList.Count; sideIndex++)
            {
                SideClass currentSide = m_SideList[sideIndex];
                sidesData[sideIndex].SideNormalData = new Double[3];
                sidesData[sideIndex].SideNormalData[0] = currentSide.SideNormal.XCoord;
                sidesData[sideIndex].SideNormalData[1] = currentSide.SideNormal.YCoord;
                sidesData[sideIndex].SideNormalData[2] = currentSide.SideNormal.ZCoord;

                sidesData[sideIndex].SideVertexIndexList = new Int32[currentSide.VertexCount];
                for (Int32 vertexIndex = 0; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    sidesData[sideIndex].SideVertexIndexList[vertexIndex] = currentSide[vertexIndex].ID - 1;
                }
            }

            info.AddValue("VertexesData", vertexesData);
            info.AddValue("SidesData", sidesData);
        }

        #endregion

        protected ConvexPolyhedron3D(SerializationInfo info, StreamingContext context)
        {
            Double[] vertexesData = info.GetValue("VertexesData", typeof(Double[])) as Double[];
            SideSerializationData[] sidesData = info.GetValue("SidesData", typeof(SideSerializationData[])) as SideSerializationData[];

            m_VertexList = new List<VertexClass>();
            m_SideList = new List<SideClass>();

            if (vertexesData.Length % 3 != 0)
            {
#warning Exception's type ???
                throw new Exception("???");
            }

            for (Int32 vertexDataIndex = 0; vertexDataIndex < vertexesData.Length; vertexDataIndex += 3)
            {
                VertexClass vertex = new VertexClass(vertexesData[vertexDataIndex], vertexesData[vertexDataIndex + 1], vertexesData[vertexDataIndex + 2], vertexDataIndex / 3 + 1);
                m_VertexList.Add(vertex);
            }

            for (Int32 sideDataIndex = 0; sideDataIndex < sidesData.Length; sideDataIndex++)
            {
                SideSerializationData sideData = sidesData[sideDataIndex];

                Vector3D normal = new Vector3D(sideData.SideNormalData[0], sideData.SideNormalData[1], sideData.SideNormalData[2]);

                List<VertexClass> vertexList = new List<VertexClass>(sideData.SideVertexIndexList.Length);
                for (Int32 vertexIndex = 0; vertexIndex < m_VertexList.Count; vertexIndex++)
                {
                    vertexList.Add(m_VertexList[vertexIndex]);
                }

                SideClass side = new SideClass(vertexList, sideDataIndex+1, normal);
                m_SideList.Add(side);
            }
        }

        /// <summary>
        /// конструктор класса ConvexPolyhedron3D
        /// </summary>
        /// <param name="approxComparer">сравниватель для операций приблизительного сравнения действительных чисел</param>
        /// <param name="sideList">список граней</param>
        /// <param name="vertexList">список вершин</param>
        internal ConvexPolyhedron3D(ApproxComparer approxComparer, List<SideClass> sideList, List<VertexClass> vertexList)
        {
            m_ApproxComparer = approxComparer;

            m_SideList = new List<SideClass>();
            m_VertexList = new List<VertexClass>();

            // заполняем список вершин в многограннике
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; vertexIndex++)
            {
                VertexClass currentVertex = vertexList[vertexIndex];
                m_VertexList.Add(new VertexClass(currentVertex.XCoord,
                                                 currentVertex.YCoord,
                                                 currentVertex.ZCoord,
                                                 currentVertex.ID));
            }

            // заполняем список граней в многограннике
            for (Int32 sideIndex = 0; sideIndex < sideList.Count; sideIndex++)
            {
                SideClass currentSide = sideList[sideIndex];

                List<VertexClass> sideVertexList = new List<VertexClass>();
                for (Int32 vertexIndex = 0; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    // ID вершины = индекс вершины + 1 ???
                    sideVertexList.Add(m_VertexList[currentSide[vertexIndex].ID - 1]);
                }

                m_SideList.Add(new SideClass(sideVertexList, currentSide.ID, currentSide.SideNormal));
            }
        }

        /// <summary>
        /// опорная функция данного многогранника
        /// </summary>
        /// <param name="vectorArg">векторныйс аргумент, для которого ищется значение опорной функции</param>
        /// <returns>значение опорной функции</returns>
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
        /// метод CalcNearestPointOnTriangle вычисляет ближайшую точку для точки givenPoint
        /// на треугольнике, заданном 3-я вершинами
        /// </summary>
        /// <param name="triangleVertex1">1-я вершина треугольника</param>
        /// <param name="triangleVertex2">2-я вершина треугольника</param>
        /// <param name="triangleVertex3">3-я вершина треугольника</param>
        /// <param name="triangleNormal">внешняя нормаль к треугольнику ( = внешней нормали к грани, которая содержит данный треугольник)</param>
        /// <param name="givenPoint">заданная точка, для которой ищется ближайшая</param>
        /// <param name="isTrueNearestPoint">возвращает true, если найденная точка является истинной ближайшей точкой (сразу об этом сказать можно только в одном случае), иначе возвращается false</param>
        /// <returns>ближайшая точка на треугольнике к заданной</returns>
        private Point3D CalcNearestPointOnTriangle(VertexClass triangleVertex1, VertexClass triangleVertex2, VertexClass triangleVertex3, Vector3D triangleNormal, Point3D givenPoint, out Boolean isTrueNearestPoint)
        {
            isTrueNearestPoint = false;

            // пусть A - заданная точка, T1, T2, T3 - вершины треугольника, N - внешняя нормаль к треугольнику
            // для поиска ближайшей точки необходимо рещить следующее уравнение:
            // (-N) = x1*AT1 + x2*AT2 + x3*AT3, где AT1, AT2, AT3 - вектора, x1, x2, x3 - искомые коэффициенты
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

            // матрица абсолютных ошибок
            Matrix matrixError = null;
            // матрица с коеффициентами X (решение)
            Matrix matrixX = AdvMath.SolveEquationSystem3(matrixA, matrixB, out matrixError);
            Double x1 = matrixX[1, 1];
            Double x2 = matrixX[2, 1];
            Double x3 = matrixX[3, 1];

            // данный треугольник находится с обратной стороны многогранника относительно точки givenPoint
            if ((x1 < 0) && (x2 < 0) && (x3 < 0))
            {
                throw new ArgumentException("Wrong triangle !!!");
            }

            // ближайшая точка лежит внутри треугольника; она является истинной ближайшей точкой
            if ((x1 > 0) && (x2 > 0) && (x3 > 0))
            {
                isTrueNearestPoint = true;

                PlaneClass trianglePlane = new PlaneClass(triangleNormal, SupportFunc(triangleNormal));
                return AdvMath.CalcPlaneLineCrossingPoint(m_ApproxComparer, trianglePlane, givenPoint, triangleNormal);
            }

            // ближайшая точка лежит на ребре 1-2
            if ((x1 > 0) && (x2 > 0) && (x3 <= 0))
            {
                Point3D line2Point1 = new Point3D(triangleVertex1.XCoord, triangleVertex1.YCoord, triangleVertex1.ZCoord);
                Point3D line2Point2 = new Point3D(triangleVertex2.XCoord, triangleVertex2.YCoord, triangleVertex2.ZCoord);

                //Vector3D line1DirectionVector = CalcLineDirectionVector(givenPoint, line2Point1, line2Point2);
                //return AdvMathClass.CalcLineLineCrossingPoint(givenPoint, line1DirectionVector, line2Point1, line2Point2);

                return AdvMath.CalcPerpendicularLinesCrossingPoint(givenPoint, line2Point1, line2Point2);
            }
            // ближайшая точка лежит на ребре 1-3
            if ((x1 > 0) && (x2 <= 0) && (x3 > 0))
            {
                Point3D line2Point1 = new Point3D(triangleVertex1.XCoord, triangleVertex1.YCoord, triangleVertex1.ZCoord);
                Point3D line2Point2 = new Point3D(triangleVertex3.XCoord, triangleVertex3.YCoord, triangleVertex3.ZCoord);

                //Vector3D line1DirectionVector = CalcLineDirectionVector(givenPoint, line2Point1, line2Point2);
                //return AdvMathClass.CalcLineLineCrossingPoint(givenPoint, line1DirectionVector, line2Point1, line2Point2);

                return AdvMath.CalcPerpendicularLinesCrossingPoint(givenPoint, line2Point1, line2Point2);
            }
            // ближайшая точка лежит на ребре 2-3
            if ((x1 <= 0) && (x2 > 0) && (x3 > 0))
            {
                Point3D line2Point1 = new Point3D(triangleVertex2.XCoord, triangleVertex2.YCoord, triangleVertex2.ZCoord);
                Point3D line2Point2 = new Point3D(triangleVertex3.XCoord, triangleVertex3.YCoord, triangleVertex3.ZCoord);

                //Vector3D line1DirectionVector = CalcLineDirectionVector(givenPoint, line2Point1, line2Point2);
                //return AdvMathClass.CalcLineLineCrossingPoint(givenPoint, line1DirectionVector, line2Point1, line2Point2);
                return AdvMath.CalcPerpendicularLinesCrossingPoint(givenPoint, line2Point1, line2Point2);
            }

            // ближайшая точка - вершина номер 1
            if ((x1 > 0) && (x2 <= 0) && (x3 <= 0))
            {
                return new Point3D(triangleVertex1.XCoord, triangleVertex1.YCoord, triangleVertex1.ZCoord);
            }
            // ближайшая точка - вершина номер 2
            if ((x1 <= 0) && (x2 > 0) && (x3 <= 0))
            {
                return new Point3D(triangleVertex2.XCoord, triangleVertex2.YCoord, triangleVertex2.ZCoord);
            }
            // ближайшая точка - вершина номер 3
            if ((x1 <= 0) && (x2 <= 0) && (x3 > 0))
            {
                return new Point3D(triangleVertex3.XCoord, triangleVertex3.YCoord, triangleVertex3.ZCoord);
            }

            // сюда по идее мы никогда не попадаем
            throw new Exception("как мы сюда попали то ??? :o");
        }

        /// <summary>
        /// метод CalcLineDirectionVector вычисляет направляющий вектор прямой (номер 1), проходящей через точку point0
        /// и перпендикулярной (и пересекающей, т.е. не смежной !!!) прямой (номер 2), проходящей через точки point1 и point2
        /// </summary>
        /// <param name="point0">точка, через которую проходит прямая номер 1</param>
        /// <param name="point1">1-я точка, через которую проходит прямая номер 2</param>
        /// <param name="point2">2-я точка, через которую проходит прямая номер 2</param>
        /// <returns>направляющий вектор прямой номер 1</returns>
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

        /// <summary>
        /// сравниватель для операций приблизительного сравнения действительных чисел
        /// </summary>
        private ApproxComparer m_ApproxComparer;

        /// <summary>
        /// список вершин многогранника
        /// </summary>
        private List<VertexClass> m_VertexList;
        /// <summary>
        /// список граней многогранника
        /// </summary>
        private List<SideClass> m_SideList;
    }
}
