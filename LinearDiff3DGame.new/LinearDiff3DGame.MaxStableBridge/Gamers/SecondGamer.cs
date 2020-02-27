using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Crossing;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    // второй игрок
    // в результате действий второго игрока мы получаем список связей П, на которых может нарушаться локальная выпуклость функции fi_i
    internal class SecondGamer
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
        public SecondGamer(ApproxComp approxComparer, Matrix matrixC, Double deltaT, Double mqMax, Double mqMin)
        {
            this.approxComparer = approxComparer;
            this.matrixC = matrixC;
            this.deltaT = deltaT;
            this.mqMax = mqMax;
            this.mqMin = mqMin;
        }

        // действие второго игрока над системой (графом) на данном шаге
        public IPolyhedron3DGraph Action(IPolyhedron3DGraph graph,
                                         Matrix fundCauchyMatrix,
                                         SuspiciousConnectionSet connSet,
                                         Matrix scalingMatrix)
        {
            // столбец (матрица) E для данного второго игрока в данный момент времени
            Matrix matrixE = CalcMatrixE(fundCauchyMatrix, scalingMatrix);

            // вычисляем радиус векторы вершин отрезка Qi
            List<Vector3D> pointQiSet = new List<Vector3D>(2);
            pointQiSet.Add(new Vector3D(mqMax*matrixE[1, 1], mqMax*matrixE[2, 1], mqMax*matrixE[3, 1]));
            pointQiSet.Add(new Vector3D(mqMin*matrixE[1, 1], mqMin*matrixE[2, 1], mqMin*matrixE[3, 1]));

            // направляющий вектор отрезка Qi
            Vector3D directingQi = new Vector3D(matrixE[1, 1], matrixE[2, 1], matrixE[3, 1]);
            directingQi = Vector3DUtils.NormalizeVector(directingQi);

            // подсчет функции eta_i для узлов графа
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
#warning ОЧЕНЬ ВАЖНО !!!!!! ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ПОЛУЧЕНИЯ ЗНАЧЕНИЯ ФУНКЦИИ eta_i
                IPolyhedron3DGraphNode currentNode = graph.NodeList[nodeIndex];
                currentNode.SupportFuncValue -= deltaT*Math.Max(currentNode.NodeNormal*pointQiSet[0],
                                                                currentNode.NodeNormal*pointQiSet[1]);
            }
            // подсчет функции eta_i для узлов графа

            FillSuspiciousConnectionSet(graph, directingQi, connSet);
            return graph;
        }

        private Matrix CalcMatrixE(Matrix fundCauchyMatrix, Matrix scalingMatrix)
        {
            Matrix matrixEBeforeScaling = fundCauchyMatrix*matrixC;
            return scalingMatrix*matrixEBeforeScaling;
        }

        // заполнение списка связей П связями, на которых может нарушаться локальная выпуклость функции fi_i
        private void FillSuspiciousConnectionSet(IPolyhedron3DGraph graph,
                                                 Vector3D directingVectorQi,
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

        // добавление в набор П объект пересечения и необходимые соседние с ним связи
        private static void AddConns2SuspiciousConnectionSet(CrossingObject currentCrossingObject,
                                                             SuspiciousConnectionSet connSet)
        {
            // если текущий объект – узел
            if(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                IPolyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;
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
                IPolyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
                IPolyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
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

        private readonly ApproxComp approxComparer;

        // столбец (матрица) C для данного второго игрока
        private readonly Matrix matrixC;

        // шаг разбиения оси t
        private readonly Double deltaT;

        // максимальное значение ограничения (в виде отрезка) на множество данного второго игрока
        private readonly Double mqMax;

        // минимальное значение ограничения (в виде отрезка) на множество данного второго игрока
        private readonly Double mqMin;
    }
}