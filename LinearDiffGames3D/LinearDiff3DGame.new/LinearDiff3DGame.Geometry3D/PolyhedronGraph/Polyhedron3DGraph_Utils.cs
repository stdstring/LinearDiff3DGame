using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    public class Polyhedron3DGraph_Utils
    {
        public IPolyhedron3DGraphNode FindOppositeNode(IPolyhedron3DGraph graph,
                                                      IPolyhedron3DGraphNode graphNode)
        {
            Vector3D oppositeNormal = new Vector3D(-graphNode.NodeNormal.X,
                                                   -graphNode.NodeNormal.Y,
                                                   -graphNode.NodeNormal.Z);
            IPolyhedron3DGraphNode oppositeNode = null;
            Double currentCosValue = 0;
            foreach(IPolyhedron3DGraphNode node in graph.NodeList)
            {
                Double cosValue = node.NodeNormal * oppositeNormal / (node.NodeNormal.Length * oppositeNormal.Length);
                if(cosValue > currentCosValue)
                {
                    oppositeNode = node;
                    currentCosValue = cosValue;
                }
                if(currentCosValue == 1) break;
            }
            return oppositeNode;
        }

        // направление, толщина
        public Pair<Vector3D, Double> CalcThickness(IPolyhedron3DGraphNode node,
                                                    IPolyhedron3DGraphNode oppositeNode)
        {
            Vector3D vector1 = node.SupportFuncValue * node.NodeNormal;
            Vector3D vector2 = oppositeNode.SupportFuncValue * oppositeNode.NodeNormal;
            Vector3D direction = vector1 - vector2;
            Double thicknessValue = direction.Length;
            direction = Vector3DUtils.NormalizeVector(direction);
            return new Pair<Vector3D, Double>(direction, thicknessValue);
        }

        // Max - направление, толщина
        // Min - направление, толщина
        public IDictionary<String, Pair<Vector3D, Double>> CalcGraphExtremeThickness(IPolyhedron3DGraph graph)
        {
            Pair<Vector3D, Double> maxThickness = new Pair<Vector3D, Double>(Vector3D.ZeroVector3D, 0);
            Pair<Vector3D, Double> minThickness = new Pair<Vector3D, Double>(Vector3D.ZeroVector3D, Double.MaxValue);
            foreach(IPolyhedron3DGraphNode node in graph.NodeList)
            {
                IPolyhedron3DGraphNode oppositeNode = FindOppositeNode(graph, node);
                Pair<Vector3D, Double> currentThickness = CalcThickness(node, oppositeNode);
                if(currentThickness.Item2 < minThickness.Item2)
                    minThickness = currentThickness;
                if(currentThickness.Item2 > maxThickness.Item2)
                    maxThickness = currentThickness;
            }
            return new Dictionary<String, Pair<Vector3D, Double>>
                       {
                           {MaxThicknessKey, maxThickness},
                           {MinThicknessKey, minThickness}
                       };
        }

        public Boolean NeedScaling(Pair<Vector3D, Double> minThickness,
                                   Pair<Vector3D, Double> maxThickness,
                                   Double thresholdValue)
        {
            return maxThickness.Item2 / minThickness.Item2 > thresholdValue;
        }

        // направление, коэффициент масштабирования
        public Pair<Vector3D, Double> CalcScaling(Pair<Vector3D, Double> minThickness,
                                                  Pair<Vector3D, Double> maxThickness,
                                                  Double thresholdValue)
        {
            // направление a - направление min толщины
            // направление b - направление max толщины
            return new Pair<Vector3D, Double>(minThickness.Item1, thresholdValue);
            Vector3D a = minThickness.Item1;
            if (a.Length != 1) a = Vector3DUtils.NormalizeVector(a);
            Vector3D b = maxThickness.Item1;
            if (b.Length != 1) b = Vector3DUtils.NormalizeVector(b);
            Vector3D bParallel = Vector3DUtils.GetParallelComponent(b, a);
            Vector3D bPerpendicular = Vector3DUtils.GetPerpendicularComponent(b, a);
            if (minThickness.Item2 <= bParallel.Length * maxThickness.Item2)
                return new Pair<Vector3D, Double>(minThickness.Item1, thresholdValue);
            Double scaleRatio = maxThickness.Item2 * bPerpendicular.Length /
                                Math.Sqrt(Sqr(minThickness.Item2) - Sqr(bParallel.Length * maxThickness.Item2));
            return new Pair<Vector3D, Double>(a, scaleRatio);
        }

        public const String MaxThicknessKey = "Max";
        public const String MinThicknessKey = "Min";

        // TODO : это тут так временно и локально
        private static Double Sqr(Double x)
        {
            return x * x;
        }
    }
}