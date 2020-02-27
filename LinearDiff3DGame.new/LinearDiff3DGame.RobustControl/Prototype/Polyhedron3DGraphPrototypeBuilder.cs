using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.AdvMath.Algorithms;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.RobustControl.Algorithms;

namespace LinearDiff3DGame.RobustControl.Prototype
{
    // строим прототип граф многогранника polyhedron1 + k*polyhedron2
    internal class Polyhedron3DGraphPrototypeBuilder
    {
        public Polyhedron3DGraphPrototypeBuilder(ApproxComp approxComp)
        {
            this.approxComp = approxComp;
        }

        public IPolyhedron3DGraphPrototype Build(IPolyhedron3D polyhedron1,
                                                 IPolyhedron3DGraph graph1,
                                                 IPolyhedron3D polyhedron2,
                                                 IPolyhedron3DGraph graph2)
        {
            // 1) копирование графа многогранника 1 в объект прототип
            IPolyhedron3DGraphPrototype prototype = new Polyhedron3DGraphPrototype(graph1);
            // 2) учет в опорной функции наличия 2-го многогранника для добавленных узлов
            //prototype = ContributionFromSeconPolyhedron(prototype, polyhedron2);
            // 3) добавление графа многогранника 2 в объект прототип
            IList<IPolyhedron3DGraphPrototypeNode> crossingNodes = new List<IPolyhedron3DGraphPrototypeNode>();
            prototype = AddSecondPolyhedron(prototype, polyhedron1, graph2, crossingNodes);
            // 4) триангуляция полученного прототипа
            // ...
            return prototype;
        }

        private LocationBase FindLocation(IPolyhedron3DGraphPrototype source,
                                          IPolyhedron3DGraphNode node)
        {
            // узлы триугольника 1, 2, 3 ВСЕГДА идут против ч.с., если смотреть с конца внешней нормали (к треугольнику)
            Triple<IPolyhedron3DGraphPrototypeNode> startTriangle =
                new Triple<IPolyhedron3DGraphPrototypeNode>(source.NodeList[0],
                                                            source.NodeList[0].ConnectionList[0],
                                                            source.NodeList[0].ConnectionList[1]);
            return FindLocation(node, startTriangle);
        }

        private LocationBase FindLocation(IPolyhedron3DGraphNode node,
                                          Triple<IPolyhedron3DGraphPrototypeNode> startTriangle)
        {
            Func<Triple<IPolyhedron3DGraphPrototypeNode>, Triple<Int32>> locationParamsCalc =
                location => CalcLocationParams(location.Item1.NodeNormal,
                                               location.Item2.NodeNormal,
                                               location.Item3.NodeNormal,
                                               node.NodeNormal);
            Func<Triple<IPolyhedron3DGraphPrototypeNode>, Triple<Int32>, Boolean> searchCriteria =
                (location, locationParams) => locationParams.Item1 >= 0 &&
                                              locationParams.Item2 >= 0 &&
                                              locationParams.Item3 >= 0;
            Func<Triple<IPolyhedron3DGraphPrototypeNode>, Triple<Int32>, Triple<IPolyhedron3DGraphPrototypeNode>> nextItemSelector =
                (location, locationParams) =>
                    {
                        if(locationParams.Item1 > 0)
                        {
                            // берем треугольник со стороной 1-2
                            return new Triple<IPolyhedron3DGraphPrototypeNode>(
                                location.Item1.ConnectionList.GetPrevItem(location.Item2),
                                location.Item2,
                                location.Item1);
                        }
                        if(locationParams.Item2 > 0)
                        {
                            // берем треугольник со стороной 2-3
                            return new Triple<IPolyhedron3DGraphPrototypeNode>(
                                location.Item2.ConnectionList.GetPrevItem(location.Item3),
                                location.Item3,
                                location.Item2);
                        }
                        if(locationParams.Item3 > 0)
                        {
                            // берем треугольник со стороной 3-1
                            return new Triple<IPolyhedron3DGraphPrototypeNode>(
                                location.Item3.ConnectionList.GetPrevItem(location.Item1),
                                location.Item1,
                                location.Item3);
                        }
                        throw new AlgorithmException("Unexpected calculation flow");
                    };
            ConvexSystemFinder finder = new ConvexSystemFinder();
            Pair<Triple<IPolyhedron3DGraphPrototypeNode>, Triple<Int32>> nodeLocation =
                finder.SafeSearch(startTriangle, locationParamsCalc, searchCriteria, nextItemSelector);
            // TODO : нужна модификатия Location изз-за возможной близости к другим объектам
            return new LocationFactory().Create(node, nodeLocation.Item1, nodeLocation.Item2);
        }

        private Triple<IPolyhedron3DGraphPrototypeNode> GetStartTriangle4Location(LocationBase location)
        {
            switch (location.LocationType)
            {
                case LocationType.Node:
                    throw new NotSupportedException("Not supported in this version");
                case LocationType.Connection:
                    throw new NotSupportedException("Not supported in this version");
                case LocationType.Triangle:
                    LocationTriangle locationTriangle = (LocationTriangle)location;
                    return new Triple<IPolyhedron3DGraphPrototypeNode>(locationTriangle.Node1,
                                                                       locationTriangle.Node2,
                                                                       locationTriangle.Node3);
                default:
                    throw new ArgumentOutOfRangeException("location");
            }
        }

        // решаем систему уравнений sourceVector = A*vector1 + B*vector2 + C*vector3
        // возвращает тройку чисел Sign(A), Sign(B), Sign(C)
        // TODO : подумать может можно по другому
        private Triple<Int32> CalcLocationParams(Vector3D vector1,
                                                 Vector3D vector2,
                                                 Vector3D vector3,
                                                 Vector3D sourceVector)
        {
            // deltaPositive = V1x*V2y*V3z + V2x*V3y*V1z+V3x*V1y*V2z
            Double deltaPositive = vector1.X*vector2.Y*vector3.Z +
                                   vector2.X*vector3.Y*vector1.Z +
                                   vector3.X*vector1.Y*vector2.Z;
            // deltaNegative = V1x*V3y*V2z + V2x*V1y*V3z + V3x*V2y*V1z
            Double deltaNegative = vector1.X*vector3.Y*vector2.Z +
                                   vector2.X*vector1.Y*vector3.Z +
                                   vector3.X*vector2.Y*vector1.Z;
            // delta1Positive = Vx*V2y*V3z + V2x*V3y*Vz+V3x*Vy*V2z
            Double delta1Positive = sourceVector.X*vector2.Y*vector3.Z +
                                    vector2.X*vector3.Y*sourceVector.Z +
                                    vector3.X*sourceVector.Y*vector2.Z;
            // delta1Negative = Vx*V3y*V2z + V2x*Vy*V3z + V3x*V2y*Vz
            Double delta1Negative = sourceVector.X*vector3.Y*vector2.Z +
                                    vector2.X*sourceVector.Y*vector3.Z +
                                    vector3.X*vector2.Y*sourceVector.Z;
            // delta2Positive = V1x*Vy*V3z + Vx*V3y*V1z+V3x*V1y*Vz
            Double delta2Positive = vector1.X*sourceVector.Y*vector3.Z +
                                    sourceVector.X*vector3.Y*vector1.Z +
                                    vector3.X*vector1.Y*sourceVector.Z;
            // delta2Negative = V1x*V3y*Vz + Vx*V1y*V3z + V3x*Vy*V1z
            Double delta2Negative = vector1.X*vector3.Y*sourceVector.Z +
                                    sourceVector.X*vector1.Y*vector3.Z +
                                    vector3.X*sourceVector.Y*vector1.Z;
            // delta3Positive = V1x*V2y*Vz + V2x*Vy*V1z+Vx*V1y*V2z
            Double delta3Positive = vector1.X*vector2.Y*sourceVector.Z +
                                    vector2.X*sourceVector.Y*vector1.Z +
                                    sourceVector.X*vector1.Y*vector2.Z;
            // delta3Negative = V1x*Vy*V2z + V2x*V1y*Vz + Vx*V2y*V1z
            Double delta3Negative = vector1.X*sourceVector.Y*vector2.Z +
                                    vector2.X*vector1.Y*sourceVector.Z +
                                    sourceVector.X*vector2.Y*vector1.Z;
            // deltaPositive != deltaNegative ибо ахтунг
            if(deltaPositive == deltaNegative)
                throw new ArgumentException("Too small numbers");
            Int32 deltaSign = deltaPositive > deltaNegative ? 1 : -1;
            Int32 delta1Sign = CalculateDifferenceSign(delta1Positive, delta1Negative);
            Int32 delta2Sign = CalculateDifferenceSign(delta2Positive, delta2Negative);
            Int32 delta3Sign = CalculateDifferenceSign(delta3Positive, delta3Negative);
            return new Triple<Int32>(delta1Sign/deltaSign, delta2Sign/deltaSign, delta3Sign/deltaSign);
        }

        // TODO : вынести отсюдова ?????????????????
        private static Int32 CalculateDifferenceSign(Double number1, Double number2)
        {
            if(number1 == number2) return 0;
            if(number1 > number2) return 1;
            return -1;
        }

        private IPolyhedron3DGraphPrototype AddSecondPolyhedron(IPolyhedron3DGraphPrototype source,
                                                                IPolyhedron3D firstPolyhedron,
                                                                IPolyhedron3DGraph secondPolyhedronGraph,
                                                                IList<IPolyhedron3DGraphPrototypeNode> crossingNodes)
        {
            IDictionary<IPolyhedron3DGraphNode, IPolyhedron3DGraphPrototypeNode> graphNodesDict =
                new Dictionary<IPolyhedron3DGraphNode, IPolyhedron3DGraphPrototypeNode>();
            IDictionary<IPolyhedron3DGraphPrototypeNode, IPolyhedron3DGraphNode> prototypeNodesDict =
                new Dictionary<IPolyhedron3DGraphPrototypeNode, IPolyhedron3DGraphNode>();
            IDictionary<IPolyhedron3DGraphNode, LocationBase> locationDict =
                new Dictionary<IPolyhedron3DGraphNode, LocationBase>();
            // 1) Местоположение первого узла второго графа
            LocationBase locationFirst = FindLocation(source, secondPolyhedronGraph.NodeList[0]);
            if(locationFirst.LocationType != LocationType.Triangle)
                throw new NotImplementedException("Not implemented in this version");
            // 2) Добавляем узлы второго графа в прототип
            WaveProcessing<IPolyhedron3DGraphNode> addNodesProcessing = new WaveProcessing<IPolyhedron3DGraphNode>();
            Int32 freeNodeId = source.NodeList.Last().ID + 1;
            Action<IPolyhedron3DGraphNode, IPolyhedron3DGraphNode> addNodeProcess =
                (parent, node) =>
                    {
                        LocationBase parentLocation = locationDict[parent];
                        Triple<IPolyhedron3DGraphPrototypeNode> startTriangle = GetStartTriangle4Location(parentLocation);
                        LocationBase nodeLocation = FindLocation(node, startTriangle);
                        IPolyhedron3DGraphPrototypeNode destNode = CreateNode4Location(nodeLocation, freeNodeId++);
                        source.NodeList.Add(destNode);
                        graphNodesDict.Add(node, destNode);
                        prototypeNodesDict.Add(destNode, node);
                        locationDict.Add(node, nodeLocation);
                    };
            addNodesProcessing.Process(locationFirst.SourceNode, null, node => node.ConnectionList, addNodeProcess);
            // 3) Воссоздание связей
            secondPolyhedronGraph.NodeList
                .ForEach(node =>
                             {
                                 // проводим связи
                                 node.ConnectionList.ForEach(conn =>
                                                                 {
                                                                     AddSecondGraphConnection(node, conn, source, graphNodesDict);
                                                                 });
                             });
            return source;
        }

        private IPolyhedron3DGraphPrototypeNode CreateNode4Location(LocationBase location, Int32 nodeId)
        {
            switch (location.LocationType)
            {
                case LocationType.Node:
                    throw new NotSupportedException("Not supported in this version");
                case LocationType.Connection:
                    throw new NotSupportedException("Not supported in this version");
                case LocationType.Triangle:
                    IPolyhedron3DGraphPrototypeNode destNode =
                        new Polyhedron3DGraphPrototypeNode(nodeId, location.SourceNode.NodeNormal);
                    // TODO : support func value
                    //Enumerable.Repeat<IPolyhedron3DGraphPrototypeNode>(null, location.SourceNode.ConnectionList.Count)
                    //    .ForEach(connection => destNode.ConnectionList.Add(null));
                    return destNode;
                default:
                    throw new ArgumentOutOfRangeException("location");
            }
        }

        private void AddSecondGraphConnection(IPolyhedron3DGraphNode from,
                                              IPolyhedron3DGraphNode to,
                                              IPolyhedron3DGraphPrototype source,
                                              IDictionary<IPolyhedron3DGraphNode, IPolyhedron3DGraphPrototypeNode> graphNodesDict)
        {
            IPolyhedron3DGraphPrototypeNode fromInPrototype = graphNodesDict[from];
            IPolyhedron3DGraphPrototypeNode toInPrototype = graphNodesDict[to];
        }

        private IPolyhedron3DGraphPrototypeNode CreateCrossingNode(IPolyhedron3DGraphPrototypeNode previous,
                                                                   IPolyhedron3DGraphNode finish,
                                                                   params Pair<IPolyhedron3DGraphPrototypeNode>[] checkConns)
        {
            foreach(Pair<IPolyhedron3DGraphPrototypeNode> conn in checkConns)
            {
                IPolyhedron3DGraphPrototypeNode node1 = conn.Item1;
                IPolyhedron3DGraphPrototypeNode node2 = conn.Item2;
                Vector3D? connCrossing = CalcConnectionsCrossing(previous.NodeNormal, finish.NodeNormal, node1.NodeNormal, node2.NodeNormal);
                if(!connCrossing.HasValue) continue;
                Pair<Double> crossingParams = CalcCrossingParams(node1.NodeNormal, node2.NodeNormal, connCrossing.Value);
                Double alpha = crossingParams.Item1;
                Double beta = crossingParams.Item2;
                Pair<Double> supportFuncValues = new Pair<Double>(node1.SupportFuncValues.Item1*alpha + node2.SupportFuncValues.Item1*beta,
                                                                  node1.SupportFuncValues.Item2*alpha + node2.SupportFuncValues.Item2*beta);
            }
            throw new NotImplementedException();
        }

        // TODO : и этому алгоритму здесь не место
        // TODO : по идее нужен как вектор, так и коэффициенты alpha, beta
        private Vector3D? CalcConnectionsCrossing(Vector3D firstConnEdge1,
                                                  Vector3D firstConnEdge2,
                                                  Vector3D secondConnEdge1,
                                                  Vector3D secondConnEdge2)
        {
            Vector3D firstNormal = Vector3DUtils.VectorProduct(firstConnEdge1, firstConnEdge2);
            Vector3D secondNormal = Vector3DUtils.VectorProduct(secondConnEdge1, secondConnEdge2);
            Vector3D destVector = Vector3DUtils.NormalizeVector(Vector3DUtils.VectorProduct(firstNormal, secondNormal));
            Pair<Double> crossingParams = CalcCrossingParams(firstConnEdge1, firstConnEdge2, destVector);
            // TODO : подумать над использованием ApproxComp
            Double alpha = crossingParams.Item1;
            Double beta = crossingParams.Item2;
            if(alpha*beta < 0) return null;
            // учитываем, что мы могли ошибиться в направлении вектора пересечения
            Int32 sign = alpha != 0 ? Math.Sign(alpha) : Math.Sign(beta);
            return sign*destVector;
        }

        // решаем уравнение N = alpha*N1 + beta*N2, где N, N1, N2 - вектора
        private Pair<Double> CalcCrossingParams(Vector3D normal1,
                                                Vector3D normal2,
                                                Vector3D crossingNormal)
        {
            // TODO : подумать, что будет при delta близкой к 0 и как этого избежать
            // TODO : алгоритм встречается более, чем в одном месте - вынести
            // (n1, n):
            Double scalarProduct1 = crossingNormal*normal1;
            // (n2, n):
            Double scalarProduct2 = crossingNormal*normal2;
            // (n1, n2):
            Double scalarProduct12 = normal1*normal2;
            // delta = 1 - (n1,n2)*(n1,n2)
            Double delta = 1 - scalarProduct12*scalarProduct12;
            // delta1 = (n,n1) - (n1,n2)*(n,n2)
            // delta2 = (n,n2) - (n1,n2)*(n,n1)
            Double alpha = (scalarProduct1 - scalarProduct12*scalarProduct2)/delta;
            Double beta = (scalarProduct2 - scalarProduct12*scalarProduct1)/delta;
            return new Pair<Double>(alpha, beta);
        }

        private readonly ApproxComp approxComp;

        private enum LocationType
        {
            // узел совпадает с другим узлом
            Node = 0,
            // узел лежит на связи
            Connection = 1,
            // узел лежит внутри треугольника
            Triangle = 2
        }

        private abstract class LocationBase
        {
            public LocationBase(IPolyhedron3DGraphNode sourceNode)
            {
                SourceNode = sourceNode;
            }

            public abstract LocationType LocationType { get; }
            public IPolyhedron3DGraphNode SourceNode { get; private set; }
        }

        private class LocationNode : LocationBase
        {
            public LocationNode(IPolyhedron3DGraphNode sourceNode,
                                IPolyhedron3DGraphPrototypeNode destNode)
                : base(sourceNode)
            {
                DestNode = destNode;
            }

            public override LocationType LocationType
            {
                get { return LocationType.Node; }
            }

            public IPolyhedron3DGraphPrototypeNode DestNode { get; private set; }
        }

        private class LocationConnection : LocationBase
        {
            public LocationConnection(IPolyhedron3DGraphNode sourceNode,
                                      IPolyhedron3DGraphPrototypeNode connNode1,
                                      IPolyhedron3DGraphPrototypeNode connNode2)
                : base(sourceNode)
            {
                ConnNode1 = connNode1;
                ConnNode2 = connNode2;
            }

            public override LocationType LocationType
            {
                get { return LocationType.Connection; }
            }

            public IPolyhedron3DGraphPrototypeNode ConnNode1 { get; private set; }
            public IPolyhedron3DGraphPrototypeNode ConnNode2 { get; private set; }
        }

        private class LocationTriangle : LocationBase
        {
            public LocationTriangle(IPolyhedron3DGraphNode sourceNode,
                                    IPolyhedron3DGraphPrototypeNode node1,
                                    IPolyhedron3DGraphPrototypeNode node2,
                                    IPolyhedron3DGraphPrototypeNode node3)
                : base(sourceNode)
            {
                Node1 = node1;
                Node2 = node2;
                Node3 = node3;
            }

            public override LocationType LocationType
            {
                get { return LocationType.Triangle; }
            }

            public IPolyhedron3DGraphPrototypeNode Node1 { get; private set; }
            public IPolyhedron3DGraphPrototypeNode Node2 { get; private set; }
            public IPolyhedron3DGraphPrototypeNode Node3 { get; private set; }
        }

        private class LocationFactory
        {
            public LocationBase Create(IPolyhedron3DGraphNode sourceNode,
                                       Triple<IPolyhedron3DGraphPrototypeNode> locationTriangle,
                                       Triple<Int32> locationParams)
            {
                if(locationParams.Item1 > 0 && locationParams.Item2 > 0 && locationParams.Item3 > 0)
                {
                    // треугольник 1-2-3
                    return new LocationTriangle(sourceNode,
                                                locationTriangle.Item1,
                                                locationTriangle.Item2,
                                                locationTriangle.Item3);
                }
                if(locationParams.Item1 > 0 && locationParams.Item2 > 0 && locationParams.Item3 == 0)
                {
                    // связь 1-2
                    return new LocationConnection(sourceNode,
                                                  locationTriangle.Item1,
                                                  locationTriangle.Item2);
                }
                if(locationParams.Item2 > 0 && locationParams.Item3 > 0 && locationParams.Item1 == 0)
                {
                    // связь 2-3
                    return new LocationConnection(sourceNode,
                                                  locationTriangle.Item2,
                                                  locationTriangle.Item3);
                }
                if(locationParams.Item3 > 0 && locationParams.Item1 > 0 && locationParams.Item2 == 0)
                {
                    // связь 3-1
                    return new LocationConnection(sourceNode,
                                                  locationTriangle.Item3,
                                                  locationTriangle.Item1);
                }
                if(locationParams.Item1 > 0 && locationParams.Item2 == 0 && locationParams.Item3 == 0)
                {
                    // узел 1
                    return new LocationNode(sourceNode, locationTriangle.Item1);
                }
                if(locationParams.Item2 > 0 && locationParams.Item3 == 0 && locationParams.Item1 == 0)
                {
                    // узел 2
                    return new LocationNode(sourceNode, locationTriangle.Item2);
                }
                if(locationParams.Item3 > 0 && locationParams.Item1 == 0 && locationParams.Item2 == 0)
                {
                    // узел 3
                    return new LocationNode(sourceNode, locationTriangle.Item2);
                }
                throw new ArgumentException("locationParams");
            }
        }
    }
}