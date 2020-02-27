using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.SuspiciousConnections
{
    /// <summary>
    /// вспомогательный класс; представляет список связей П, на которых предполагается отсутствие локальной выпуклости овыпукляемой функции системы
    /// </summary>
    public /*internal*/ class SuspiciousConnectionSet
    {
        /// <summary>
        /// внутренний класс; для реализации предиката, используемого в методе RemoveConnections списка связей
        /// </summary>
        private class RemoveConnectionsPredicate
        {
            /// <summary>
            /// конструктор класса RemoveConnectionsPredicate
            /// </summary>
            /// <param name="reasonNode">узел графа, для связей которого предикат должен сработать</param>
            public RemoveConnectionsPredicate(Polyhedron3DGraphNode reasonNode)
            {
                m_ReasonNode = reasonNode;
            }

            /// <summary>
            /// метод проверки связи на соответствие (что связь содержит переданный в конструкторе узел)
            /// </summary>
            /// <param name="obj">проверяемая связь</param>
            /// <returns>true, если связь подходящая; иначе - false</returns>
            public Boolean Match(GraphConnection obj)
            {
                return ReferenceEquals(obj.Node1, m_ReasonNode) || ReferenceEquals(obj.Node2, m_ReasonNode);
            }

            /// <summary>
            /// узел графа, для связей которого предикат должен сработать
            /// </summary>
            private readonly Polyhedron3DGraphNode m_ReasonNode;
        }

        /// <summary>
        /// конструктор класса SuspiciousConnectionSet
        /// </summary>
        public SuspiciousConnectionSet()
        {
            m_SuspiciousConnectionSet = new List<GraphConnection>();
        }

        /// <summary>
        /// количество связей в списке "подозрительных" связей
        /// </summary>
        public Int32 Count
        {
            get { return m_SuspiciousConnectionSet.Count; }
        }

        /// <summary>
        /// индексатор для доступа к связям из списка "подозрительных" связей
        /// </summary>
        /// <param name="index">индекс связи к которой осуществляется доступ</param>
        /// <returns>связь (в виде массива узлов графа) к которой осуществляется доступ</returns>
        public Polyhedron3DGraphNode[] this[Int32 index]
        {
            get
            {
                Polyhedron3DGraphNode[] connNodes = new Polyhedron3DGraphNode[2];

                GraphConnection currentConn = m_SuspiciousConnectionSet[index];
                connNodes[0] = currentConn.Node1;
                connNodes[1] = currentConn.Node2;

                return connNodes;
            }
        }

        /// <summary>
        /// добавление связи (заданной двумя узлами) в список "подозрительных" связей
        /// </summary>
        /// <param name="node1">узел 1 связи</param>
        /// <param name="node2">узел 2 связи</param>
        public void AddConnection(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);

            if (m_SuspiciousConnectionSet.IndexOf(conn) == -1)
            {
                m_SuspiciousConnectionSet.Add(conn);
            }
        }

        /// <summary>
        /// удаление связи из списка "подозрительных" связей
        /// </summary>
        /// <param name="index">индекс удаляемой связи</param>
        public void RemoveConnection(Int32 index)
        {
            m_SuspiciousConnectionSet.RemoveAt(index);
        }

        /// <summary>
        /// удаление связи (заданной двумя узлами) из списка "подозрительных" связей
        /// </summary>
        /// <param name="node1">узел 1 связи</param>
        /// <param name="node2">узел 2 связи</param>
        public void RemoveConnection(Polyhedron3DGraphNode node1, Polyhedron3DGraphNode node2)
        {
            GraphConnection conn = new GraphConnection(node1, node2);
            m_SuspiciousConnectionSet.Remove(conn);
        }

        /// <summary>
        /// удаление всех связей из списка "подозрительных" связей, которые содержат узел node
        /// </summary>
        /// <param name="node">узел, для которого происходит удаление всех связей из списка "подозрительных" связей</param>
        public void RemoveConnections(Polyhedron3DGraphNode node)
        {
            RemoveConnectionsPredicate predicate = new RemoveConnectionsPredicate(node);
            m_SuspiciousConnectionSet.RemoveAll(predicate.Match);
        }

        /// <summary>
        /// список "подозрительных" связей
        /// </summary>
        private readonly List<GraphConnection> m_SuspiciousConnectionSet;
    }
}