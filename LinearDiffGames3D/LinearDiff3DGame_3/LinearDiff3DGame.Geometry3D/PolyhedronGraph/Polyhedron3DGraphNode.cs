using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D
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
        /// <param name="nodeNormal">"внешняя" нормаль к грани, которая соответствует данному узлу графа</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Vector3D nodeNormal)
        {
            m_ID = nodeID;
            m_NodeNormal = nodeNormal;

            m_NodeConnectionList = new CyclicList<Polyhedron3DGraphNode>();
        }

        /// <summary>
        /// конструктор класса Polyhedron3DGraphNode
        /// </summary>
        /// <param name="nodeID">ID узла</param>
        /// <param name="nodeNormal">"внешняя" нормаль к грани, которая соответствует данному узлу графа</param>
        /// <param name="nodeConnectionList">список связей данного узла</param>
        public Polyhedron3DGraphNode(Int32 nodeID, Vector3D nodeNormal, IList<Polyhedron3DGraphNode> nodeConnectionList)
        {
            m_ID = nodeID;
            m_NodeNormal = nodeNormal;

            m_NodeConnectionList = new CyclicList<Polyhedron3DGraphNode>();
            for (Int32 connectionIndex = 0; connectionIndex < nodeConnectionList.Count; ++connectionIndex)
            {
                m_NodeConnectionList.Add(nodeConnectionList[connectionIndex]);
            }
        }

        /// <summary>
        /// список связей данного узла
        /// </summary>
        public ICyclicList<Polyhedron3DGraphNode> ConnectionList
        {
            get
            {
                return m_NodeConnectionList;
            }
        }

        /// <summary>
        /// ID - свойство для доступа (чтение/запись) к ID узла
        /// </summary>
        public Int32 ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }

        /// <summary>
        /// NodeNormal - свойство для доступа (чтение) к "внешней" нормали к грани, которая соответствует данному узлу графа
        /// </summary>
        public Vector3D NodeNormal
        {
            get
            {
                return m_NodeNormal;
            }
        }

#warning очень нехорошо, т.к. в будущем возможно сопоставление узлу графа нескольких аттрибутов
        /// <summary>
        /// значение опорной функции для узла (дополнительный аттрибут)
        /// </summary>
        public Double SupportFuncValue
        {
            get
            {
                return m_SupportFuncValue;
            }
            set
            {
                m_SupportFuncValue = value;
            }
        }

        /// <summary>
        /// m_ID - уникальный идентификатор узла (совпадает с ID грани, которая данному узлу соответствует)
        /// </summary>
        private Int32 m_ID;
        /// <summary>
        /// m_NodeNormal - "внешняя" нормаль к грани, которая соответствует данному узлу графа
        /// </summary>
        private Vector3D m_NodeNormal;
        /// <summary>
        /// m_NodeConnectionList - список связей данного узла
        /// </summary>
        private CyclicList<Polyhedron3DGraphNode> m_NodeConnectionList;

#warning очень нехорошо, т.к. в будущем возможно сопоставление узлу графа нескольких аттрибутов
        /// <summary>
        /// значение опорной функции для узла (дополнительный аттрибут)
        /// </summary>
        private Double m_SupportFuncValue;
    }
}
