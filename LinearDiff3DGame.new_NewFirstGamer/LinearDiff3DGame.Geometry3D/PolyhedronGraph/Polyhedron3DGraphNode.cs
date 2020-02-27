using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    /// <summary>
    /// узел графа, представляющего 3-мерный многогранник
    /// граф, построенный на подобных узлах имеет следующую особенность: если есть путь из узла 1 в узел 2, то не обязательно, что есть путь из узла 2 в узел 1
    /// </summary>
    public class Polyhedron3DGraphNode
    {
        /// <summary>
        /// конструктор класса Polyhedron3DGraphNode
        /// </summary>
        /// <param name="nodeID">ID узла</param>
        /// <param name="generationID">ID поколения</param>
        /// <param name="nodeNormal">"внешняя" нормаль к грани, которая соответствует данному узлу графа</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Int32 generationID, Vector3D nodeNormal)
        {
            ID = nodeID;
            GenerationID = generationID;
            NodeNormal = nodeNormal;

            m_NodeConnectionList = new List<Polyhedron3DGraphNode>();
        }

        /// <summary>
        /// конструктор класса Polyhedron3DGraphNode
        /// </summary>
        /// <param name="nodeID">ID узла</param>
        /// <param name="generationID">ID поколения</param>
        /// <param name="nodeNormal">"внешняя" нормаль к грани, которая соответствует данному узлу графа</param>
        /// <param name="nodeConnectionList">список связей данного узла</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Int32 generationID, Vector3D nodeNormal,
                                     IList<Polyhedron3DGraphNode> nodeConnectionList)
        {
            ID = nodeID;
            GenerationID = generationID;
            NodeNormal = nodeNormal;

            m_NodeConnectionList = new List<Polyhedron3DGraphNode>();
            for(Int32 connectionIndex = 0; connectionIndex < nodeConnectionList.Count; ++connectionIndex)
                m_NodeConnectionList.Add(nodeConnectionList[connectionIndex]);
        }

        /// <summary>
        /// список связей данного узла
        /// </summary>
        public IList<Polyhedron3DGraphNode> ConnectionList { get { return m_NodeConnectionList; } }

        /// <summary>
        /// ID - свойство для доступа (чтение/запись) к ID узла
        /// </summary>
        public Int32 ID { get; set; }

        public Int32 GenerationID { get; private set; }

        /// <summary>
        /// NodeNormal - свойство для доступа (чтение) к "внешней" нормали к грани, которая соответствует данному узлу графа
        /// </summary>
        public Vector3D NodeNormal { get; set; }

        /// <summary>
        /// значение опорной функции для узла (дополнительный аттрибут)
        /// </summary>
        public Double SupportFuncValue { get; set; }

        /// <summary>
        /// m_NodeConnectionList - список связей данного узла
        /// </summary>
        private readonly List<Polyhedron3DGraphNode> m_NodeConnectionList;
    }
}