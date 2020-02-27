using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.AdvMath.Algorithms;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using IPolyhedron3DGraphNodeMap =
    System.Collections.Generic.IDictionary<LinearDiff3DGame.Geometry3D.PolyhedronGraph.IPolyhedron3DGraphNode,
        LinearDiff3DGame.RobustControl.Prototype.IPolyhedron3DGraphPrototypeNode>;
using IPolyhedron3DGraphPrototypeMap =
    System.Collections.Generic.IDictionary<LinearDiff3DGame.RobustControl.Prototype.IPolyhedron3DGraphPrototypeNode,
        LinearDiff3DGame.Geometry3D.PolyhedronGraph.IPolyhedron3DGraphNode>;

namespace LinearDiff3DGame.RobustControl.Prototype
{
    internal class Polyhedron3DGraphPrototypeBuilderSimple
    {
        public Polyhedron3DGraphPrototypeBuilderSimple(ApproxComp approxComp)
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
            // ...
            // 3) добавление графа многогранника 2 в объект прототип
            IList<IPolyhedron3DGraphPrototypeNode> crossingNodes = new List<IPolyhedron3DGraphPrototypeNode>();
            // ...
            // 4) триангуляция полученного прототипа
            // ...
            return prototype;
        }

        private IPolyhedron3DGraphPrototype AddSecondPolyhedron(IPolyhedron3DGraphPrototype source,
                                                                IPolyhedron3D firstPolyhedron,
                                                                IPolyhedron3DGraph secondPolyhedronGraph,
                                                                IList<IPolyhedron3DGraphPrototypeNode> crossingNodes)
        {
            IPolyhedron3DGraphNodeMap graphNodesDict =
                new Dictionary<IPolyhedron3DGraphNode, IPolyhedron3DGraphPrototypeNode>();
            IPolyhedron3DGraphPrototypeMap prototypeNodesDict =
                new Dictionary<IPolyhedron3DGraphPrototypeNode, IPolyhedron3DGraphNode>();
            IDictionary<IPolyhedron3DGraphNode, LocationTriangle> locationDict =
                new Dictionary<IPolyhedron3DGraphNode, LocationTriangle>();
            // 1) Добавляем все узлы второго графа в прототип
            Int32 nodeId = source.NodeList.Last().ID + 1;
            secondPolyhedronGraph.NodeList.ForEach(node =>
                                                       {
                                                           LocationTriangle location = FindLocation(node, source);
                                                           IPolyhedron3DGraphPrototypeNode prototypeNode =
                                                               CreatePrototypeNode(location.SourceNode, nodeId++);
                                                           source.NodeList.Add(prototypeNode);
                                                           graphNodesDict.Add(node, prototypeNode);
                                                           prototypeNodesDict.Add(prototypeNode, node);
                                                           locationDict.Add(node, location);
                                                       });
            // 2) Воссоздание связей
            prototypeNodesDict.ForEach(pair =>
                                           {
                                               IPolyhedron3DGraphPrototypeNode prototypeNode = pair.Key;
                                               IPolyhedron3DGraphNode node = pair.Value;
                                           });
            return source;
        }

        private LocationTriangle FindLocation(IPolyhedron3DGraphNode node,
                                              IPolyhedron3DGraphPrototype source)
        {
            // узлы триугольника 1, 2, 3 ВСЕГДА идут против ч.с., если смотреть с конца внешней нормали (к треугольнику)
            Triple<IPolyhedron3DGraphPrototypeNode> startTriangle =
                new Triple<IPolyhedron3DGraphPrototypeNode>(source.NodeList[0],
                                                            source.NodeList[0].ConnectionList[0],
                                                            source.NodeList[0].ConnectionList[1]);
            return FindLocation(node, startTriangle);
        }

        private LocationTriangle FindLocation(IPolyhedron3DGraphNode node,
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
            Func<Triple<IPolyhedron3DGraphPrototypeNode>, Triple<Int32>, Triple<IPolyhedron3DGraphPrototypeNode>>
                nextItemSelector =
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
            return LocationFactory.Create(node, nodeLocation.Item1, nodeLocation.Item2);
        }

        // решаем систему уравнений sourceVector = A*vector1 + B*vector2 + C*vector3
        // возвращает тройку чисел Sign(A), Sign(B), Sign(C)
        // TODO : подумать, наверное это не самый лучший вариант
        private static Triple<Int32> CalcLocationParams(Vector3D vector1,
                                                        Vector3D vector2,
                                                        Vector3D vector3,
                                                        Vector3D sourceVector)
        {
            // deltaPositive = V1x*V2y*V3z + V2x*V3y*V1z+V3x*V1y*V2z
            Double deltaPositive = vector1.X * vector2.Y * vector3.Z +
                                   vector2.X * vector3.Y * vector1.Z +
                                   vector3.X * vector1.Y * vector2.Z;
            // deltaNegative = V1x*V3y*V2z + V2x*V1y*V3z + V3x*V2y*V1z
            Double deltaNegative = vector1.X * vector3.Y * vector2.Z +
                                   vector2.X * vector1.Y * vector3.Z +
                                   vector3.X * vector2.Y * vector1.Z;
            // delta1Positive = Vx*V2y*V3z + V2x*V3y*Vz+V3x*Vy*V2z
            Double delta1Positive = sourceVector.X * vector2.Y * vector3.Z +
                                    vector2.X * vector3.Y * sourceVector.Z +
                                    vector3.X * sourceVector.Y * vector2.Z;
            // delta1Negative = Vx*V3y*V2z + V2x*Vy*V3z + V3x*V2y*Vz
            Double delta1Negative = sourceVector.X * vector3.Y * vector2.Z +
                                    vector2.X * sourceVector.Y * vector3.Z +
                                    vector3.X * vector2.Y * sourceVector.Z;
            // delta2Positive = V1x*Vy*V3z + Vx*V3y*V1z+V3x*V1y*Vz
            Double delta2Positive = vector1.X * sourceVector.Y * vector3.Z +
                                    sourceVector.X * vector3.Y * vector1.Z +
                                    vector3.X * vector1.Y * sourceVector.Z;
            // delta2Negative = V1x*V3y*Vz + Vx*V1y*V3z + V3x*Vy*V1z
            Double delta2Negative = vector1.X * vector3.Y * sourceVector.Z +
                                    sourceVector.X * vector1.Y * vector3.Z +
                                    vector3.X * sourceVector.Y * vector1.Z;
            // delta3Positive = V1x*V2y*Vz + V2x*Vy*V1z+Vx*V1y*V2z
            Double delta3Positive = vector1.X * vector2.Y * sourceVector.Z +
                                    vector2.X * sourceVector.Y * vector1.Z +
                                    sourceVector.X * vector1.Y * vector2.Z;
            // delta3Negative = V1x*Vy*V2z + V2x*V1y*Vz + Vx*V2y*V1z
            Double delta3Negative = vector1.X * sourceVector.Y * vector2.Z +
                                    vector2.X * vector1.Y * sourceVector.Z +
                                    sourceVector.X * vector2.Y * vector1.Z;
            // deltaPositive != deltaNegative ибо ахтунг
            if(deltaPositive == deltaNegative)
                throw new ArgumentException("Too small numbers");

            // TODO : подумать, может вынести как внешний алгоритм
            Func<Double, Double, Int32> calcDifferenceSign = (number1, number2) =>
                                                                 {
                                                                     if(number1 == number2) return 0;
                                                                     if(number1 > number2) return 1;
                                                                     return -1;
                                                                 };
            Int32 deltaSign = deltaPositive > deltaNegative ? 1 : -1;
            Int32 delta1Sign = calcDifferenceSign(delta1Positive, delta1Negative);
            Int32 delta2Sign = calcDifferenceSign(delta2Positive, delta2Negative);
            Int32 delta3Sign = calcDifferenceSign(delta3Positive, delta3Negative);
            return new Triple<Int32>(delta1Sign / deltaSign, delta2Sign / deltaSign, delta3Sign / deltaSign);
        }

        private static IPolyhedron3DGraphPrototypeNode CreatePrototypeNode(IPolyhedron3DGraphNode node, Int32 nodeId)
        {
            IPolyhedron3DGraphPrototypeNode destNode =
                new Polyhedron3DGraphPrototypeNode(nodeId, node.NodeNormal);
            destNode.SupportFuncValues = new Pair<Double>(0, node.SupportFuncValue);
            Enumerable.Range(0, node.ConnectionList.Count).ForEach(_ => destNode.ConnectionList.Add(null));
            return destNode;
        }

        private void ProcessConnectionList(IPolyhedron3DGraphNode node,
                                           IPolyhedron3DGraphNodeMap graphNodesDict,
                                           IPolyhedron3DGraphPrototypeMap prototypeNodesDict,
                                           IList<IPolyhedron3DGraphPrototypeNode> crossingNodes)
        {
            IPolyhedron3DGraphPrototypeNode prototypeNode = graphNodesDict[node];
            for(Int32 connIndex = 0; connIndex < node.ConnectionList.Count; ++connIndex)
            {
                IPolyhedron3DGraphNode conn = node.ConnectionList[connIndex];
                IPolyhedron3DGraphPrototypeNode prototypeConn = graphNodesDict[conn];
            }
        }

        private void ProcessConnection(IPolyhedron3DGraphPrototypeNode from,
                                       LocationTriangle fromLocation,
                                       IPolyhedron3DGraphPrototypeNode to,
                                       IList<IPolyhedron3DGraphPrototypeNode> crossingNodes)
        {
            LocationTriangle currentLocation = fromLocation;
            IList<Pair<IPolyhedron3DGraphPrototypeNode>> conns4Crossing =
                new List<Pair<IPolyhedron3DGraphPrototypeNode>>
                    {
                        new Pair<IPolyhedron3DGraphPrototypeNode>(fromLocation.Node1, fromLocation.Node2),
                        new Pair<IPolyhedron3DGraphPrototypeNode>(fromLocation.Node2, fromLocation.Node3),
                        new Pair<IPolyhedron3DGraphPrototypeNode>(fromLocation.Node3, fromLocation.Node1)
                    };
            while(true)
            {
                Triple<Int32> locationParams = CalcLocationParams(currentLocation.Node1.NodeNormal,
                                                                  currentLocation.Node2.NodeNormal,
                                                                  currentLocation.Node3.NodeNormal,
                                                                  to.NodeNormal);
                if(locationParams.Item1 >= 0 && locationParams.Item2 >= 0 && locationParams.Item3 >= 0)
                {
                    break;
                }
                ConnCrossingContainer container = CalculateCrossing(from, to, conns4Crossing);
                IPolyhedron3DGraphPrototypeNode node1 = container.Nodes.Item1;
                IPolyhedron3DGraphPrototypeNode node2 = container.Nodes.Item2;
            }
        }

        private static ConnCrossingContainer CalculateCrossing(IPolyhedron3DGraphPrototypeNode from,
                                                               IPolyhedron3DGraphPrototypeNode to,
                                                               IEnumerable<Pair<IPolyhedron3DGraphPrototypeNode>>
                                                                   candidateConns)
        {
            foreach(Pair<IPolyhedron3DGraphPrototypeNode> candidateConn in candidateConns)
            {
                Vector3D crossingVector = CalcCrossingVector(from.NodeNormal,
                                                             to.NodeNormal,
                                                             candidateConn.Item1.NodeNormal,
                                                             candidateConn.Item2.NodeNormal);
                Pair<Double> crossingFactors = CalcCrossingFactors(crossingVector,
                                                                   candidateConn.Item1.NodeNormal,
                                                                   candidateConn.Item2.NodeNormal);
                if(crossingFactors.Item1 >= 0 && crossingFactors.Item2 >= 0)
                {
                    return new ConnCrossingContainer
                               {
                                   Nodes = candidateConn,
                                   CrossingVector = crossingVector,
                                   Factors = crossingFactors
                               };
                }
            }
            throw new AlgorithmException("Can't calculate crossing");
        }

        private static Vector3D CalcCrossingVector(Vector3D firstConnEdge1,
                                                   Vector3D firstConnEdge2,
                                                   Vector3D secondConnEdge1,
                                                   Vector3D secondConnEdge2)
        {
            // TODO : подумать и вынести куда-нибудь данный алгоритм
            // crossingVector = [[first1, first2], [secoind1, second2]], при правильном построении треугольника
            Vector3D first12 = Vector3DUtils.VectorProduct(firstConnEdge1, firstConnEdge2);
            Vector3D second12 = Vector3DUtils.VectorProduct(secondConnEdge1, secondConnEdge2);
            return Vector3DUtils.VectorProduct(first12, second12);
        }

        // решаем vector0 = A*vector1 + B*vector2; возвращаем пару (A, B)
        private static Pair<Double> CalcCrossingFactors(Vector3D vector0, Vector3D vector1, Vector3D vector2)
        {
            // TODO : подумать над методом
            // TODO : подумать и вынести куда-нибудь данный алгоритм
            // (vector0, vector1)
            Double scalarProduct1 = vector0 * vector1;
            // (vector0, vector2)
            Double scalarProduct2 = vector0 * vector2;
            // (vector1, vector2)
            Double scalarProduct12 = vector1 * vector2;
            // delta = 1 - (vector1, vector2)*(vector1, vector2)
            Double delta = 1 - scalarProduct12 * scalarProduct12;
            // delta1 = (vector0, vector1) - (vector1, vector2)*(vector0, vector2)
            // delta2 = (vector0, vector2) - (vector1, vector2)*(vector0, vector1)
            Double alpha = (scalarProduct1 - scalarProduct12 * scalarProduct2) / delta;
            Double beta = (scalarProduct2 - scalarProduct12 * scalarProduct1) / delta;
            return new Pair<Double>(alpha, beta);
        }

        private readonly ApproxComp approxComp;

        private class LocationTriangle
        {
            public LocationTriangle(IPolyhedron3DGraphNode sourceNode,
                                    IPolyhedron3DGraphPrototypeNode node1,
                                    IPolyhedron3DGraphPrototypeNode node2,
                                    IPolyhedron3DGraphPrototypeNode node3)
            {
                SourceNode = sourceNode;
                Node1 = node1;
                Node2 = node2;
                Node3 = node3;
            }

            public IPolyhedron3DGraphNode SourceNode { get; private set; }
            public IPolyhedron3DGraphPrototypeNode Node1 { get; private set; }
            public IPolyhedron3DGraphPrototypeNode Node2 { get; private set; }
            public IPolyhedron3DGraphPrototypeNode Node3 { get; private set; }
        }

        private static class LocationFactory
        {
            public static LocationTriangle Create(IPolyhedron3DGraphNode sourceNode,
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
                    throw new NotSupportedException("Not supported in this version");
                }
                if(locationParams.Item2 > 0 && locationParams.Item3 > 0 && locationParams.Item1 == 0)
                {
                    // связь 2-3
                    throw new NotSupportedException("Not supported in this version");
                }
                if(locationParams.Item3 > 0 && locationParams.Item1 > 0 && locationParams.Item2 == 0)
                {
                    // связь 3-1
                    throw new NotSupportedException("Not supported in this version");
                }
                if(locationParams.Item1 > 0 && locationParams.Item2 == 0 && locationParams.Item3 == 0)
                {
                    // узел 1
                    throw new NotSupportedException("Not supported in this version");
                }
                if(locationParams.Item2 > 0 && locationParams.Item3 == 0 && locationParams.Item1 == 0)
                {
                    // узел 2
                    throw new NotSupportedException("Not supported in this version");
                }
                if(locationParams.Item3 > 0 && locationParams.Item1 == 0 && locationParams.Item2 == 0)
                {
                    // узел 3
                    throw new NotSupportedException("Not supported in this version");
                }
                throw new ArgumentException("locationParams");
            }
        }

        private class ConnCrossingContainer
        {
            public Pair<IPolyhedron3DGraphPrototypeNode> Nodes { get; set; }
            public Pair<Double> Factors { get; set; }
            public Vector3D CrossingVector { get; set; }
        }
    }
}