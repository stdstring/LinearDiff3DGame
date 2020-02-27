using System;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.SuspiciousConnections
{
    /// <summary>
    /// вспомогательная структура, представляющая связь между двумя узлами графа
    /// при построении данной структуры узлы сортируются так, чтобы ID 1-го узла был меньше ID 2-го узла
    /// </summary>
    internal struct GraphConnection
    {
        /// <summary>
        /// конструктор структуры GraphConnection
        /// </summary>
        /// <param name="node1">узел 1 связи</param>
        /// <param name="node2">узел 2 связи</param>
        public GraphConnection(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
#warning никак не проверяется, что узлы node1 и node2 на самом деле образуют связь

            if (ReferenceEquals(node1, node2))
            {
#warning может более специализированное исключение
                throw new Exception("node1 and node2 must be different !!!");
            }

            Node1 = (node1.ID < node2.ID ? node1 : node2);
            Node2 = (node2.ID < node1.ID ? node1 : node2);
        }

        /// <summary>
        /// узел 1 связи
        /// </summary>
        public readonly Polyhedron3DGraphNode Node1;

        /// <summary>
        /// узел 2 связи
        /// </summary>
        public readonly Polyhedron3DGraphNode Node2;
    }
}