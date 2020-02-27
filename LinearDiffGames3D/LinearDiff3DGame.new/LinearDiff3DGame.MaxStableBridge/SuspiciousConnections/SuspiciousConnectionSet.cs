using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.SuspiciousConnections
{
    // список связей П, на которых предполагается отсутствие локальной выпуклости овыпукляемой функции системы
    internal class SuspiciousConnectionSet
    {
        public SuspiciousConnectionSet()
        {
            suspiciousConnectionSet = new List<GraphConnection>();
        }

        public Int32 Count
        {
            get { return suspiciousConnectionSet.Count; }
        }

        // индексатор для доступа к связям из списка "подозрительных" связей; возвращает связь (в виде массива узлов графа) к которой осуществляется доступ
        public IPolyhedron3DGraphNode[] this[Int32 index]
        {
            get
            {
                IPolyhedron3DGraphNode[] connNodes = new IPolyhedron3DGraphNode[2];

                GraphConnection currentConn = suspiciousConnectionSet[index];
                connNodes[0] = currentConn.Node1;
                connNodes[1] = currentConn.Node2;

                return connNodes;
            }
        }

        // добавление связи (заданной двумя узлами) в список "подозрительных" связей
        public void AddConnection(IPolyhedron3DGraphNode node1, IPolyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);
            if (suspiciousConnectionSet.IndexOf(conn) == -1)
                suspiciousConnectionSet.Add(conn);
        }

        // удаление связи из списка "подозрительных" связей
        public void RemoveConnection(Int32 index)
        {
            suspiciousConnectionSet.RemoveAt(index);
        }

        // удаление связи (заданной двумя узлами) из списка "подозрительных" связей
        public void RemoveConnection(IPolyhedron3DGraphNode node1, IPolyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);
            suspiciousConnectionSet.Remove(conn);
        }

        // удаление всех связей из списка "подозрительных" связей, которые содержат узел node
        public void RemoveConnections(IPolyhedron3DGraphNode node)
        {
            suspiciousConnectionSet.RemoveAll(
                conn => ReferenceEquals(conn.Node1, node) || ReferenceEquals(conn.Node2, node));
        }

        // список "подозрительных" связей
        private readonly List<GraphConnection> suspiciousConnectionSet;
    }
}