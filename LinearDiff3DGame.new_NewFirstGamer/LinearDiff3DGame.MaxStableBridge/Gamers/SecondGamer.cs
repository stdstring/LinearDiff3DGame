using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Crossing;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    /// <summary>
    /// класс представляющий и инкапсулирующий действия второго игрока
    /// в результате действий второго игрока мы получаем список связей П, на которых может нарушаться локальная выпуклость функции fi_i
    /// </summary>
    public /*internal*/ class SecondGamer
    {
        public SecondGamer(GamerInitData initData)
        {
            approxComparer = initData.ApproxComp;
            matrixC = initData.Matrix;
            mqMax = initData.MaxSection;
            mqMin = initData.MinSection;
            deltaT = initData.DeltaT;
        }

        [Obsolete]
        /// <summary>
        /// конструктор класса SecondGamer
        /// </summary>
        /// <param name="approxComparer">сравниватель, для приближенного сравнения действительных чисел</param>
        /// <param name="matrixC">столбец (матрица) C для данного первого игрока</param>
        /// <param name="deltaT">шаг разбиения оси t</param>
        /// <param name="mqMax">максимальное значение ограничения (в виде отрезка) на множество данного второго игрока</param>
        /// <param name="mqMin">минимальное значение ограничения (в виде отрезка) на множество данного второго игрока</param>
        public SecondGamer(ApproxComp approxComparer, Matrix matrixC, Double deltaT, Double mqMax, Double mqMin)
        {
            this.approxComparer = approxComparer;
            this.matrixC = matrixC;
            this.deltaT = deltaT;
            this.mqMax = mqMax;
            this.mqMin = mqMin;
        }

        /// <summary>
        /// метод Action - это действие второго игрока над системой (графом) в данный момент времени
        /// </summary>
        /// <param name="graph">граф</param>
        /// <param name="fundCauchyMatrix">фундаментальная матрица Коши (точнее ее часть) в данный момент времени</param>
        /// <param name="connSet">список связей П</param>
        /// <param name="scalingMatrix">матрица масштабирования</param>
        /// <returns>граф системы, после действия второго игрока</returns>
        public Polyhedron3DGraph Action(Polyhedron3DGraph graph, Matrix fundCauchyMatrix,
                                        SuspiciousConnectionSet connSet, Matrix scalingMatrix)
        {
            // столбец (матрица) E для данного второго игрока в данный момент времени
            Matrix matrixE = CalcMatrixE(fundCauchyMatrix, scalingMatrix);

            // вычисляем радиус векторы вершин отрезка Qi
            List<Vector3D> pointQiSet = new List<Vector3D>(2);
            pointQiSet.Add(new Vector3D(mqMax * matrixE[1, 1], mqMax * matrixE[2, 1], mqMax * matrixE[3, 1]));
            pointQiSet.Add(new Vector3D(mqMin * matrixE[1, 1], mqMin * matrixE[2, 1], mqMin * matrixE[3, 1]));

            // направляющий вектор отрезка Qi
            Vector3D directingQi = new Vector3D(matrixE[1, 1], matrixE[2, 1], matrixE[3, 1]);
            directingQi = Vector3DUtils.NormalizeVector(directingQi);

            // подсчет функции eta_i для узлов графа
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ФУНКЦИИ eta_i
                Polyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                currentNode.SupportFuncValue -= deltaT * Math.Max(currentNode.NodeNormal * pointQiSet[0],
                                                                  currentNode.NodeNormal * pointQiSet[1]);
            }
            // подсчет функции eta_i для узлов графа

            FillSuspiciousConnectionSet(graph, directingQi, connSet);
            return graph;
        }

        private Matrix CalcMatrixE(Matrix fundCauchyMatrix, Matrix scalingMatrix)
        {
            Matrix matrixEBeforeScaling = fundCauchyMatrix * matrixC;
            return scalingMatrix * matrixEBeforeScaling;
        }

        /// <summary>
        /// метод FillSuspiciousConnectionSet заполняет список связей П связями, на которых может нарушаться локальная выпуклость функции fi_i
        /// </summary>
        /// <param name="graph">граф</param>
        /// <param name="directingVectorQi">направляющий вектор отрезка Qi</param>
        /// <param name="connSet">список связей П</param>
        private void FillSuspiciousConnectionSet(Polyhedron3DGraph graph, Vector3D directingVectorQi,
                                                 SuspiciousConnectionSet connSet)
        {
            // объект для поиска пересечений графа с Zi
            CrossingObjectsSearch finder = new CrossingObjectsSearch(approxComparer);

            // первый (запомненный) объект пересечения
            CrossingObject firstCrossingObject = finder.GetFirstCrossingObject(graph.NodeList[0], directingVectorQi);
            // текущий объект пересечения
            CrossingObject currentCrossingObject = firstCrossingObject;
            // добавляем в набор П сам объект пересечения и необходимые соседние с ним связи
            AddConns2SuspiciousConnectionSet(currentCrossingObject, connSet);

            // Цикл (пока текущий объект не станет равным запомненному)
            do
            {
                // получаем следующий по движению объект (связь, либо узел) и делаем его текущим
                currentCrossingObject = finder.GetNextCrossingObject(currentCrossingObject, directingVectorQi);
                // добавляем в набор П сам объект пересечения и необходимые соседние с ним связи
                AddConns2SuspiciousConnectionSet(currentCrossingObject, connSet);
            } while(currentCrossingObject != firstCrossingObject);
            // Цикл (пока текущий объект не станет равным запомненному)
        }

        /// <summary>
        /// метод AddConns2SuspiciousConnectionSet добавляет в набор П объект пересечения и необходимые соседние с ним связи
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения</param>
        /// <param name="connSet">список связей П</param>
        private void AddConns2SuspiciousConnectionSet(CrossingObject currentCrossingObject, SuspiciousConnectionSet connSet)
        {
            // если текущий объект – узел
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                Polyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;

                // Цикл по всем связям текущего узла (текущего объекта пересечения)
                for(Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    // Если текущая связь отсутствует в наборе П, то добавляем ее в этот набор
                    connSet.AddConnection(currentNode, currentNode.ConnectionList[connIndex]);
                }
                // Цикл по всем связям текущего узла (текущего объекта пересечения)
            }
                // если текущий объект – связь
            else
            {
                Polyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
                Polyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;

                // Если текущая связь в наборе П отсутствует, то эту связь добавляем в набор П
                connSet.AddConnection(positiveNode, negativeNode);

                // цикл по всем связям положительного узла текущей связи
                for(Int32 connIndex = 0; connIndex < positiveNode.ConnectionList.Count; ++connIndex)
                {
                    // если рассматриваемая связь положительного узла текущей связи отсутствует в наборе П, то добавляем ее в этот набор
                    connSet.AddConnection(positiveNode, positiveNode.ConnectionList[connIndex]);
                }
                // цикл по всем связям отрицательного узла текущей связи
                for(Int32 connIndex = 0; connIndex < negativeNode.ConnectionList.Count; ++connIndex)
                {
                    // если рассматриваемая связь отрицательного узла текущей связи отсутствует в наборе П, то добавляем ее в этот набор
                    connSet.AddConnection(negativeNode, negativeNode.ConnectionList[connIndex]);
                }

                /*// Для положительного узла связи, берем ссылку на предыдущий узел относительно 	данной связи; назовем этот узел – узлом 1
                Polyhedron3DGraphNode node1 = positiveNode.ConnectionList.GetPrevItem(negativeNode);
                Double scalarProduct1 = node1.NodeNormal * directingVectorQi;
                // Для положительного узла связи, берем ссылку на следующий узел относительно 	данной связи; назовем этот узел – узлом 2
                Polyhedron3DGraphNode node2 = positiveNode.ConnectionList.GetNextItem(negativeNode);
                Double scalarProduct2 = node2.NodeNormal * directingVectorQi;

                // Если узел 1 является положительным узлом, то
                if (approxComparer.GT(scalarProduct1, 0))
                {
                    // Если связь узел 1 – отрицательный узел текущей связи отсутствует в наборе П, то это связь добавляем в набор П
                    connSet.AddConnection(node1, negativeNode);
                }
                // Если узел 1 является отрицательным узлом, то
                if (approxComparer.LT(scalarProduct1, 0))
                {
                    // Если связь узел 1 – положительный узел текущей связи отсутствует в наборе П, то это связь добавляем в набор П
                    connSet.AddConnection(positiveNode, node1);
                }
                // Если узел 2 является положительным узлом, то
                if (approxComparer.GT(scalarProduct2, 0))
                {
                    // Если связь узел 2 – отрицательный узел текущей связи отсутствует в наборе П, то это связь добавляем в набор П
                    connSet.AddConnection(node2, negativeNode);
                }
                // Если узел 2 является отрицательным узлом, то
                if (approxComparer.LT(scalarProduct2, 0))
                {
                    // Если связь узел 2 – положительный узел текущей связи отсутствует в наборе П, то это связь добавляем в набор П
                    connSet.AddConnection(positiveNode, node2);
                }*/
            }
        }

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private readonly ApproxComp approxComparer;

        /// <summary>
        /// столбец (матрица) C для данного второго игрока
        /// </summary>
        private readonly Matrix matrixC;

        /// <summary>
        /// шаг разбиения оси t
        /// </summary>
        private readonly Double deltaT;

        /// <summary>
        /// максимальное значение ограничения (в виде отрезка) на множество данного второго игрока
        /// </summary>
        private readonly Double mqMax;

        /// <summary>
        /// минимальное значение ограничения (в виде отрезка) на множество данного второго игрока
        /// </summary>
        private readonly Double mqMin;
    }
}