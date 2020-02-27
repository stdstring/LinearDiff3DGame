using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.Check
{
    internal class AxialSymmetryCheck
    {
        public AxialSymmetryCheck(ApproxComp approxComp)
        {
            m_ApproxComp = approxComp;
        }

        public Boolean Check(Polyhedron3DGraph graph, Vector3D axis)
        {
            // словарь уже проверенных узлов
            Dictionary<Int32, Object> checkedNodes = new Dictionary<Int32, Object>();
            // цикл по всем узлам графа
            for (Int32 node1Index = 0; node1Index < graph.NodeList.Count; ++node1Index)
            {
                Polyhedron3DGraphNode node1 = graph.NodeList[node1Index];
                if (checkedNodes.ContainsKey(node1.ID))
                {
                    continue;
                }
                if (IsNodeOnAxis(node1, axis))
                {
                    checkedNodes.Add(node1.ID, null);
                    continue;
                }
                Polyhedron3DGraphNode node2 = FindSymmetricNode(graph, axis, node1Index, checkedNodes);
                if (node2 != null)
                {
                    checkedNodes.Add(node1.ID, null);
                    checkedNodes.Add(node2.ID, null);
                }
                else
                {
                    //throw new Exception("Graph hasn't axial symmetry");
                    return false;
                }
            }
            // все ОК - граф осесимметричен
            return true;
        }

        private Polyhedron3DGraphNode FindSymmetricNode(Polyhedron3DGraph graph, Vector3D axis, Int32 node1Index,
                                                        Dictionary<Int32, Object> checkedNodes)
        {
            Polyhedron3DGraphNode node1 = graph.NodeList[node1Index];
            // цикл по всем узлам графа, начиная после node1Index (т.к. узлы 0 ... node1Index-1 уже проверены
            for (Int32 node2Index = node1Index + 1; node2Index < graph.NodeList.Count; ++node2Index)
            {
                Polyhedron3DGraphNode node2 = graph.NodeList[node2Index];
                if (checkedNodes.ContainsKey(node2.ID))
                {
                    continue;
                }
                if (IsNodeOnAxis(node2, axis))
                {
                    continue;
                }
                // проверка, лежат ли node1, node2 и axis в одной плоскости
                Double mixedProduct = Vector3D.MixedProduct(node1.NodeNormal, node2.NodeNormal, axis);
                if (m_ApproxComp.EQ(mixedProduct, 0))
                {
                    return node2;
                }
            }
            // ничего не нашли
            return null;
        }

        private Boolean IsNodeOnAxis(Polyhedron3DGraphNode node1, Vector3D axis)
        {
            return m_ApproxComp.EQ(Vector3D.VectorProduct(node1.NodeNormal, axis).Length, 0);
        }

        private readonly ApproxComp m_ApproxComp;
    }
}