using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;
/*using LinearDiff3DGame.System;*/

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// класс представляющий и инкапсулирующий действия первого игрока
    /// в результате действий первого игрока мы получаем граф G(...Fi...)
    /// </summary>
    internal class FirstGamer
    {
        /// <summary>
        /// конструктор класса FirstGamer
        /// </summary>
        /// <param name="approxComparer">сравниватель, для приближенного сравнения действительных чисел</param>
        /// <param name="matrixB">столбец (матрица) B для данного первого игрока</param>
        /// <param name="deltaT">шаг разбиения оси t</param>
        /// <param name="mpMax">максимальное значение ограничения (в виде отрезка) на множество данного первого игрока</param>
        /// <param name="mpMin">минимальное значение ограничения (в виде отрезка) на множество данного первого игрока</param>
        public FirstGamer(ApproxComp approxComparer, Matrix matrixB, Double deltaT, Double mpMax, Double mpMin)
        {
            m_ApproxComparer = approxComparer;
            m_MatrixB = matrixB;
            m_DeltaT = deltaT;
            m_MpMax = mpMax;
            m_MpMin = mpMin;
        }

        /// <summary>
        /// метод Action - это действие первого игрока над системой (графом) в данный момент времени
        /// </summary>
        /// <param name="graph">граф, соответствующий многограннику Wi</param>
        /// <param name="fundCauchyMatrix">фундаментальная матрица Коши (точнее ее часть) в данный момент времени</param>
        /// <returns>граф системы, после действия первого игрока</returns>
        public Polyhedron3DGraph Action(Polyhedron3DGraph graph, Matrix fundCauchyMatrix)
        {
            // столбец (матрица) D для данного первого игрока в данный момент времени
            Matrix matrixD = fundCauchyMatrix * m_MatrixB;

            // вычисляем радиус векторы вершин отрезка Pi
            List<Vector3D> pointPiSet = new List<Vector3D>(2);
            pointPiSet.Add(new Vector3D(m_MpMax * matrixD[1, 1], m_MpMax * matrixD[2, 1], m_MpMax * matrixD[3, 1]));
            pointPiSet.Add(new Vector3D(m_MpMin * matrixD[1, 1], m_MpMin * matrixD[2, 1], m_MpMin * matrixD[3, 1]));

            // направляющий вектор отрезка Pi
            Vector3D directingVectorPi = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
            directingVectorPi.Normalize();

            // количество узлов в графе G(...Wi...)
            Int32 graphGWiNodeCount = graph.NodeList.Count;
            // строим граф G(...Fi...); при этом граф строится не заново, а за счет модификации графа graph G(...Wi...)
            Polyhedron3DGraph graphGFi = BuildGFiGrid(graph, directingVectorPi);

            // подсчет опорной функции для старых узлов (для многогранника Fi)
            for (Int32 nodeIndex = 0; nodeIndex < graphGWiNodeCount; ++nodeIndex)
            {
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
                Polyhedron3DGraphNode currentNode = graphGFi.NodeList[nodeIndex];
                currentNode.SupportFuncValue += m_DeltaT * Math.Max(-(currentNode.NodeNormal * pointPiSet[0]),
                                                                    -(currentNode.NodeNormal * pointPiSet[1]));
            }
            // подсчет опорной функции для старых узлов (для многогранника Fi)

            return graphGFi;
        }

        /// <summary>
        /// метод BuildGFiGrid строит сетку G(...Fi...) (см. алгоритм)
        /// </summary>
        /// <param name="graph">граф, который достраивается до сетки G(...Fi...)</param>
        /// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
        /// <returns>сетка G(...Fi...)</returns>
        private Polyhedron3DGraph BuildGFiGrid(Polyhedron3DGraph graph, Vector3D directingVectorPi)
        {
            // объект для поиска пересечений графа с G(...Pi...)
            CrossingObjectFinder finder = new CrossingObjectFinder(m_ApproxComparer);

            // первый (запомненный) объект пересечения
            CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorPi);
            // текущий объект пересечения
            CrossingObject currentCrossingObject = firstCrossingObject;
            // строим узел на пересечении текущего объекта и G(...Pi...) и запоминаем его
            // если этот узел отсутствует в списке узлов, то добавляем его и соответствующие ссылки на данный узел
            Polyhedron3DGraphNode firstCrossingNode = BuildCrossingNode(currentCrossingObject, graph, directingVectorPi);
            // текущий узел пересечения
            Polyhedron3DGraphNode currentCrossingNode = firstCrossingNode;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                graph.NodeList.Add(currentCrossingNode);
                AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
            }

            // Цикл (пока текущий объект не станет равным запомненному)
            do
            {
                // предыдущий объект пересечения
                CrossingObject previousCrossingObject = currentCrossingObject;
                // предыдущий узел пересечения
                Polyhedron3DGraphNode previousCrossingNode = currentCrossingNode;
                // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
                currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, currentCrossingNode, directingVectorPi);
                // строим узел на пересечении текущего объекта и G(...Pi...)
                // если этот узел отсутствует в списке узлов (этот узел будет присутствовать в списке узлов, если текущий объект – узел, либо если начальным объектом была связь и мы в нее пришли), то добавляем его и соответствующие ссылки на данный узел
                // отдельно обрабатываем случай если мы пришли в первый (запомненный) объект пересечения (для простоты реализации алгоритма)
                currentCrossingNode = (currentCrossingObject == firstCrossingObject ?
                                       firstCrossingNode :
                                       BuildCrossingNode(currentCrossingObject, graph, directingVectorPi));
                if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                    currentCrossingObject != firstCrossingObject)
                {
                    graph.NodeList.Add(currentCrossingNode);
                    AddCrossingNodeBetweenConn(currentCrossingObject.PositiveNode, currentCrossingObject.NegativeNode, currentCrossingNode);
                }

                // если предыдущий и текущий объекты – узлы
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                {
                    // переход к следующей итерации цикла
                    // continue;
                }

                // если предыдущий объект узел, а текущий связь
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                {
                    // Строим связи между предыдущим узлом и узлом пересечения на текущем объекте
                    AddConns4PrevNodeCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
                }

                // если предыдущий объект связь, а текущий узел
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                {
                    // Строим связи между узлом пересечения на предыдущем объекте и текущем узле
                    AddConns4PrevConnCurrentNodeCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
                }

                // если предыдущий и текущий объекты - связи
                if (previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                    currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
                {
                    // Строим связи между узлом пересечения на предыдущем объекте и узлом пересечения на текущем объекте
                    // Строим связь между узлом пересечения на предыдущем объекте и узлом текущей связи, который не принадлежит предыдущей связи
                    AddConns4PrevConnCurrentConnCase(previousCrossingObject, previousCrossingNode, currentCrossingObject, currentCrossingNode);
                }
            }
            while (currentCrossingObject != firstCrossingObject);
            // Цикл (пока текущий объект не станет равным запомненному)

            return graph;
        }

        /// <summary>
        /// метод CalcCrossingNodeNormal вычисляет нормаль узла на текущем пересечении графа с G(...Pi...)
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
        /// <returns>нормаль узла на текущем пересечении графа с G(...Pi...)</returns>
        private Vector3D CalcCrossingNodeNormal(CrossingObject currentCrossingObject, Vector3D directingVectorPi)
        {
            Vector3D crossingNodeNormal = Vector3D.ZeroVector3D;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
                // Строим вектор, перпендикулярный векторам, связанным текущей связью,
                // как векторное произведение положительного узла связи на отрицательный
                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
                // Вычисляем векторное произведение построенного вектора и направляющего вектора Pi
                crossingNodeNormal = Vector3D.VectorProduct(npm, directingVectorPi);
                crossingNodeNormal.Normalize();
            }
            else
            {
                crossingNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
            }

            return crossingNodeNormal;
        }

        /// <summary>
        /// метод BuildCrossingNode создает и возвращает узел на текущем пересечении графа с G(...Pi...)
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="graph">граф</param>
        /// <param name="directingVectorPi">направляющий вектор отрезка Pi</param>
        /// <returns>созданный узел на текущем пересечении графа с G(...Pi...)</returns>
        private Polyhedron3DGraphNode BuildCrossingNode(CrossingObject currentCrossingObject, Polyhedron3DGraph graph, Vector3D directingVectorPi)
        {
            Polyhedron3DGraphNode crossingNode = null;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusNodeNormal = currentCrossingObject.NegativeNode.NodeNormal;
                Vector3D crossingNodeNormal = CalcCrossingNodeNormal(currentCrossingObject, directingVectorPi);
                
                // Строим узел, связанный с полученным (выше) вектором и возвращаем его
                crossingNode = new Polyhedron3DGraphNode(graph.NodeList.Count, crossingNodeNormal);
                // подсчет значения опорной функции для построенного узла
                // (l1, l):
                Double scalarProduct1 = plusNodeNormal * crossingNodeNormal;
                // (l2, l):
                Double scalarProduct2 = minusNodeNormal * crossingNodeNormal;
                // (l1, l2):
                Double scalarProduct12 = plusNodeNormal * minusNodeNormal;
                // delta = 1 - (l1, l2)*(l1, l2)
                Double delta = 1 - scalarProduct12 * scalarProduct12;

                Double alpha = (scalarProduct1 - scalarProduct12 * scalarProduct2) / delta;
                Double beta = (scalarProduct2 - scalarProduct12 * scalarProduct1) / delta;

#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ОПОРНОЙ ФУНКЦИИ
#warning ОЧЕНЬ ВАЖНО !!!!!! считаем, что отрезок Pi проходит через точку 0 !!!!!!!!!!
                crossingNode.SupportFuncValue = alpha * currentCrossingObject.PositiveNode.SupportFuncValue +
                                                beta * currentCrossingObject.NegativeNode.SupportFuncValue;
            }
            else
            {
                // Вычисляем скалярное произведение вектора, связанного с текущим узлом, и направляющего вектора Pi
                // Если скалярное произведение <> 0, то это ошибка работы алгоритма
#warning Check is absent !!!

                crossingNode = currentCrossingObject.PositiveNode;
            }

            return crossingNode;
        }

        /// <summary>
        /// метод AddCrossingNodeBetweenConn добавляет узел crossingNode на пересечении связи и G(...Pi...) и соответствующим образом правит/добавляет ссылки
        /// </summary>
        /// <param name="connPlusNode">положительный узел пересекаемой связи</param>
        /// <param name="connMinusNode">отрицательный узел пересекаемой связи</param>
        /// <param name="crossingNode">узел на пересечении связи и G(...Pi...)</param>
        private void AddCrossingNodeBetweenConn(Polyhedron3DGraphNode connPlusNode, Polyhedron3DGraphNode connMinusNode, Polyhedron3DGraphNode crossingNode)
        {
            // возложение на метод левых функций
            /*// добавляем новый узел в список узлов графа
            graph.NodeList.Add(crossingNode);*/
            // возложение на метод левых функций

            // добавляем в список ссылок нового узла ссылки сначала на положительный узел связи, потом на отрицательный
            crossingNode.ConnectionList.Add(connPlusNode);
            crossingNode.ConnectionList.Add(connMinusNode);
            // для узлов, образующих связь, меняем их ссылки друг на друга (которые и образуют связь) на ссылку на новый узел
            connPlusNode.ConnectionList[connPlusNode.ConnectionList.IndexOf(connMinusNode)] = crossingNode;
            connMinusNode.ConnectionList[connMinusNode.ConnectionList.IndexOf(connPlusNode)] = crossingNode;
        }

        /// <summary>
        /// метод AddConns4PrevNodeCurrentConnCase добавляет необходимые связи в случае, если предыдущий объект пересечения - узел, а текущий - связь
        /// связи добавляются для того, чтобы граф оставался триангулированным
        /// </summary>
        /// <param name="previousCrossingObject">предыдущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="previousCrossingNode">узел на предыдущем пересечении графа с G(...Pi...)</param>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Pi...)</param>
        private void AddConns4PrevNodeCurrentConnCase(CrossingObject previousCrossingObject, Polyhedron3DGraphNode previousCrossingNode, CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode &&
                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
                         "previous crossing object must be node and current crossing object - connection");

            // строим связи между предыдущим узлом и узлом пересечения на текущем объекте :

            // отрицательный узел текущей связи
            Polyhedron3DGraphNode connMinusNode = currentCrossingObject.NegativeNode;
            // ссылку на текущий узел пересечения в список ссылок предыдущего узла вставляем после ссылки на отрицательный узел текущей связи
            /*Int32 PrevNode2CurrentMinusNodeConnIndex = PreviousCrossingNode.GetConnectionIndex(CurrentConnMinusNode);
            PreviousCrossingNode.InsertNodeConnection(PrevNode2CurrentMinusNodeConnIndex + 1, CurrentCrossingNode);*/
            previousCrossingNode.ConnectionList.Insert(previousCrossingNode.ConnectionList.IndexOf(connMinusNode) + 1,
                                                       currentCrossingNode);
            // ссылку на предыдущий узел вставляем после ссылки на положительный узел текущей связи (на позицию номер 1)
            currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
        }

        /// <summary>
        /// метод AddConns4PrevConnCurrentNodeCase добавляет необходимые связи в случае, если предыдущий объект пересечения - связь, а текущий - узел
        /// связи добавляются для того, чтобы граф оставался триангулированным
        /// </summary>
        /// <param name="previousCrossingObject">предыдущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="previousCrossingNode">узел на предыдущем пересечении графа с G(...Pi...)</param>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Pi...)</param>
        private void AddConns4PrevConnCurrentNodeCase(CrossingObject previousCrossingObject, Polyhedron3DGraphNode previousCrossingNode, CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode,
                         "previous crossing object must be connection and current crossing object - node");

            // строим связи между узлом пересечения на предыдущем объекте и текущем узле :

            // положительный узел предыдущей связи
            Polyhedron3DGraphNode connPlusNode = previousCrossingObject.PositiveNode;
            // ссылку на предыдущий узел пересечения в список ссылок текущего узла вставляем после ссылки на положительный узел предыдущей связи
            /*Int32 CurrentNode2PrevPlusNodeConnIndex = CurrentCrossingNode.GetConnectionIndex(PreviousConnPlusNode);
            CurrentCrossingNode.InsertNodeConnection(CurrentNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);*/
            currentCrossingNode.ConnectionList.Insert(currentCrossingNode.ConnectionList.IndexOf(connPlusNode) + 1,
                                                      previousCrossingNode);
            // ссылку на текущий узел добавляем в конец списка ссылок предыдущего узла
            previousCrossingNode.ConnectionList.Add(currentCrossingNode);
        }

        /// <summary>
        /// метод AddConns4PrevConnCurrentConnCase добавляет необходимые связи в случае, если и предыдущий, и текущий объекты пересечения - связи
        /// связи добавляются для того, чтобы граф оставался триангулированным
        /// </summary>
        /// <param name="previousCrossingObject">предыдущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="previousCrossingNode">узел на предыдущем пересечении графа с G(...Pi...)</param>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Pi...)</param>
        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Pi...)</param>
        private void AddConns4PrevConnCurrentConnCase(CrossingObject previousCrossingObject, Polyhedron3DGraphNode previousCrossingNode, CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode)
        {
            Debug.Assert(previousCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection &&
                         currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection,
                         "previous and current crossing objects must be connections");

            // строим связи между узлом пересечения на предыдущем объекте и узлом пересечения на текущем объекте
            // строим связь между узлом пересечения на предыдущем объекте и узлом текущей связи, который не принадлежит предыдущей связи
            // у связай общий отрицательный узел (случай 3а)
            if (Object.ReferenceEquals(previousCrossingObject.NegativeNode, currentCrossingObject.NegativeNode))
            {
                // положительный узел предыдущей связи (узел номер 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.PositiveNode;
                // общий отрицательный узел (узел номер 2)
                // Polyhedron3DGraphNode node2 = previousCrossingObject.NegativeNode;
                // положительный узел текущей связи (узел номер 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.PositiveNode;
                // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на узел 1
                /*Int32 CurrentPlusNode2PrevPlusNodeConnIndex = CurrentConnPlusNode.GetConnectionIndex(PreviousConnPlusNode);
                CurrentConnPlusNode.InsertNodeConnection(CurrentPlusNode2PrevPlusNodeConnIndex + 1, PreviousCrossingNode);*/
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(node1) + 1, previousCrossingNode);
                // для предыдущего узла пересечения: в конец списка ссылок добавляется сначала ссылка на новый узел пересечения, потом ссылка на узел номер 3
                /*PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);
                PreviousCrossingNode.AddNodeConnection(CurrentConnPlusNode);*/
                previousCrossingNode.ConnectionList.Add(currentCrossingNode);
                previousCrossingNode.ConnectionList.Add(node3);
                // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 3 (т.е. на позицию номер 1)
                currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
            }
            // у связай общий положительный узел (случай 3б)
            else if (Object.ReferenceEquals(previousCrossingObject.PositiveNode, currentCrossingObject.PositiveNode))
            {
                // отрицательный узел предыдущей связи (узел номер 1)
                Polyhedron3DGraphNode node1 = previousCrossingObject.NegativeNode;
                // общий положительный узел (узел номер 2)
                // Polyhedron3DGraphNode node2 = previousCrossingObject.PositiveNode;
                // отрицательный узел текущей связи (узел номер 3)
                Polyhedron3DGraphNode node3 = currentCrossingObject.NegativeNode;
                // для узла номер 3: ссылка на предыдущей узел пересечения вставляется после ссылки на текущий узел пересечения
                /*Int32 CurrentMinusNode2CurrentCrossingNodeConnIndex = CurrentConnMinusNode.GetConnectionIndex(CurrentCrossingNode);
                CurrentConnMinusNode.InsertNodeConnection(CurrentMinusNode2CurrentCrossingNodeConnIndex + 1, PreviousCrossingNode);*/
                node3.ConnectionList.Insert(node3.ConnectionList.IndexOf(currentCrossingNode) + 1, previousCrossingNode);
                // для предыдущего узла пересечения: в конец списка ссылок добавляется сначала ссылка на узел номер 3, потом ссылка на новый узел пересечения
                /*PreviousCrossingNode.AddNodeConnection(CurrentConnMinusNode);
                PreviousCrossingNode.AddNodeConnection(CurrentCrossingNode);*/
                previousCrossingNode.ConnectionList.Add(node3);
                previousCrossingNode.ConnectionList.Add(currentCrossingNode);
                // для текущего узла пересечения: ссылка на предыдущий узел пересечения вставляется после ссылки на узел номер 1 (т.е. на позицию номер 1)
                currentCrossingNode.ConnectionList.Insert(1, previousCrossingNode);
            }
            // ошибка работы алгоритма
            else
            {
#warning может более специализированное исключение
                throw new Exception("AddConns4PrevConnCurrentConnCase method incorrect work");
            }
        }

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private ApproxComp m_ApproxComparer;

        /// <summary>
        /// столбец (матрица) B для данного первого игрока
        /// </summary>
        private Matrix m_MatrixB;

        /// <summary>
        /// шаг разбиения оси t
        /// </summary>
        private Double m_DeltaT;
        /// <summary>
        /// максимальное значение ограничения (в виде отрезка) на множество данного первого игрока
        /// </summary>
        private Double m_MpMax;
        /// <summary>
        /// минимальное значение ограничения (в виде отрезка) на множество данного первого игрока
        /// </summary>
        private Double m_MpMin;
    }
}
