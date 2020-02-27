using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.Crossing
{
    internal class CrossingObjectsSearch
    {
        public CrossingObjectsSearch(ApproxComp approxComparer)
        {
            this.approxComparer = approxComparer;
        }

        public IList<CrossingObject> GetCrossingObjects(IPolyhedron3DGraph graph, Vector3D direction)
        {
            List<CrossingObject> crossingObjects = new List<CrossingObject>();
            CrossingObject first = GetFirstCrossingObject(graph.NodeList[0], direction);
            crossingObjects.Add(first);
            CrossingObject next = null;
            while ((next = GetNextCrossingObject(next ?? first, direction)) != first)
                crossingObjects.Add(next);
            return crossingObjects;
        }

        // TODO : рефакторинг
        public CrossingObject GetFirstCrossingObject(IPolyhedron3DGraphNode startNode, Vector3D direction)
        {
            CrossingObject firstCrossingObject = null;
            IPolyhedron3DGraphNode currentNode = startNode;
            Double currentScalarProduct = Vector3DUtils.ScalarProduct(currentNode.NodeNormal, direction);
            if (approxComparer.EQ(currentScalarProduct, 0))
                firstCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, currentNode, currentNode);
            while (firstCrossingObject == null)
            {
                Double bestScalarProduct = Double.NaN;
                IPolyhedron3DGraphNode bestNode = null;
                foreach (IPolyhedron3DGraphNode currentConn in currentNode.ConnectionList)
                {
                    Double scalarProduct = Vector3DUtils.ScalarProduct(currentConn.NodeNormal, direction);
                    // если скал€рное произведение = 0, то полученный узел становитс€ искомым объектом
                    if (approxComparer.EQ(scalarProduct, 0))
                    {
                        firstCrossingObject = new CrossingObject(CrossingObjectType.GraphNode,
                                                                 currentConn,
                                                                 currentConn);
                        break;
                    }
                    // если знаки скал€рных произведений currentScalarProduct и scalarProduct различаютс€
                    // то узлы currentNode и currentConn образуют искомый объект
                    if (Math.Sign(currentScalarProduct) != Math.Sign(scalarProduct))
                    {
                        IPolyhedron3DGraphNode plusNode = (currentScalarProduct > 0 ? currentNode : currentConn);
                        IPolyhedron3DGraphNode minusNode = (currentScalarProduct < 0 ? currentNode : currentConn);
                        firstCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, minusNode);
                        break;
                    }
                    // ищем узел, дл€ которого величина скал€рного произведени€ ближе всех к 0
                    // что означает, что сам узел ближе всех к плоскости, перпендикул€рной вектору directingVectorXi
                    if (Double.IsNaN(bestScalarProduct) || (Math.Abs(scalarProduct) < Math.Abs(bestScalarProduct)))
                    {
                        bestScalarProduct = scalarProduct;
                        bestNode = currentConn;
                    }
                }
                currentNode = bestNode;
                currentScalarProduct = bestScalarProduct;
            }
            return firstCrossingObject;
        }

        public CrossingObject GetNextCrossingObject(CrossingObject currentCrossingObject, Vector3D direction)
        {
            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
                return GetNextCrossingObject4Node(currentCrossingObject, direction);
            return GetNextCrossingObject4Connection(currentCrossingObject, direction);
        }

        private CrossingObject GetNextCrossingObject4Node(CrossingObject currentCrossingObject, Vector3D direction)
        {
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode);
            IPolyhedron3DGraphNode crossingNode = currentCrossingObject.PositiveNode;
            foreach (IPolyhedron3DGraphNode currentConnNode in crossingNode.ConnectionList)
            {
                IPolyhedron3DGraphNode nextConnNode = crossingNode.ConnectionList.GetNextItem(currentConnNode);
                Double currentScalarProdut = Vector3DUtils.ScalarProduct(currentConnNode.NodeNormal, direction);
                Double nextScalarProdut = Vector3DUtils.ScalarProduct(nextConnNode.NodeNormal, direction);
                // правильным объектом пересечени€ всегда будет такой, когда дл€ текущией св€зи следуща€ €вл€етс€ положительным узлом
                // если непон€тно, то нарисуй !!!
                if (approxComparer.EQ(currentScalarProdut, 0) && approxComparer.GT(nextScalarProdut, 0))
                    return new CrossingObject(CrossingObjectType.GraphNode, currentConnNode, currentConnNode);
                if (approxComparer.LT(currentScalarProdut, 0) && approxComparer.GT(nextScalarProdut, 0))
                    return new CrossingObject(CrossingObjectType.GraphConnection, nextConnNode, currentConnNode);
            }
            throw new Exception("Abnormal algorithm result");
        }

        private CrossingObject GetNextCrossingObject4Connection(CrossingObject currentCrossingObject, Vector3D direction)
        {
            Debug.Assert(currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection);
            IPolyhedron3DGraphNode positiveNode = currentCrossingObject.PositiveNode;
            IPolyhedron3DGraphNode negativeNode = currentCrossingObject.NegativeNode;
            IPolyhedron3DGraphNode positiveNextNode = positiveNode.ConnectionList.GetNextItem(negativeNode);
            IPolyhedron3DGraphNode negativePrevNode = negativeNode.ConnectionList.GetPrevItem(positiveNode);
            // инвариант целостности графа
            Debug.Assert(positiveNextNode == negativePrevNode);
            Double scalarProductValue = Vector3DUtils.ScalarProduct(positiveNextNode.NodeNormal, direction);
            if (approxComparer.EQ(scalarProductValue, 0))
                return new CrossingObject(CrossingObjectType.GraphNode, positiveNextNode, positiveNextNode);
            if (approxComparer.GT(scalarProductValue, 0))
                return new CrossingObject(CrossingObjectType.GraphConnection, positiveNextNode, negativeNode);
            return new CrossingObject(CrossingObjectType.GraphConnection, positiveNode, negativePrevNode);
        }

        private readonly ApproxComp approxComparer;
    }
}