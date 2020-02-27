using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// граф, сопостовляемый выпуклому 3-мерному многограннику (см. алгоритм построения максимальных стабильных мостов ...)
    /// </summary>
    public class Polyhedron3DGraph
    {
        /// <summary>
        /// конструктор класса Polyhedron3DGraph
        /// </summary>
        /// <param name="nodeList">список узлов графа (каждый узел содержит к тому же связи с другими узлами)</param>
        public Polyhedron3DGraph(List<Polyhedron3DGraphNode> nodeList)
        {
            m_PGNodeList = new List<Polyhedron3DGraphNode>(nodeList);
        }

        /// <summary>
        /// список узлов графа (каждый узел содержит к тому же связи с другими узлами)
        /// </summary>
        public IList<Polyhedron3DGraphNode> NodeList
        {
            get
            {
                return m_PGNodeList;
            }
        }

        /// <summary>
        /// список узлов графа (каждый узел содержит к тому же связи с другими узлами)
        /// </summary>
        private List<Polyhedron3DGraphNode> m_PGNodeList;
    }
}
