using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// представляет грань 3-мерного многогранника
    /// </summary>
    public class PolyhedronSide3D
    {
        /// <summary>
        /// конструктор класса PolyhedronSide3D
        /// </summary>
        /// <param name="sideID">ID грани</param>
        /// <param name="sideNormal">внешняя нормаль к грани</param>
        public PolyhedronSide3D(Int32 sideID, Vector3D sideNormal)
        {
            ID = sideID;
            m_VertexList = new CyclicList<PolyhedronVertex3D>();
            m_SideNormal = sideNormal;
        }

        /// <summary>
        /// конструктор класса PolyhedronSide3D
        /// </summary>
        /// <param name="vertexList">список вершин</param>
        /// <param name="sideID">ID грани</param>
        /// <param name="sideNormal">внешняя нормаль к грани</param>
        public PolyhedronSide3D(IList<PolyhedronVertex3D> vertexList, Int32 sideID, Vector3D sideNormal)
        {
            ID = sideID;

            m_VertexList = new CyclicList<PolyhedronVertex3D>();
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                m_VertexList.Add(vertexList[vertexIndex]);
            }

            m_SideNormal = sideNormal;
        }

        /// <summary>
        /// ID - уникальный идентификатор грани
        /// </summary>
        public readonly Int32 ID;

        /// <summary>
        /// SideNormal - свойство для доступа (чтение) к "внешней" нормали грани
        /// </summary>
        public Vector3D SideNormal
        {
            get
            {
                return m_SideNormal;
            }
        }

        /* это не есть хорошо  ... к тому же возможна несогласованность списка вершин данной грани и списков граней вершин, которые принадлежат данной грани */
#warning Incorrect architecture

        /// <summary>
        /// список вершин, которые принадлежат данной грани
        /// </summary>
        public ICyclicList<PolyhedronVertex3D> VertexList
        {
            get
            {
                return m_VertexList;
            }
        }

        /*/// <summary>
        /// свойство-индексатор для доступа (чтение) к вершине из списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="index">индекс вершины в списке вершин</param>
        /// <returns>одна из вершин из списка вершин</returns>
        public PolyhedronVertex3D this[Int32 index]
        {
            get
            {
                return m_VertexList[index];
            }
        }

        /// <summary>
        /// свойство VertexCount возвращает количество вершин в списке вершин, которые принадлежат данной грани
        /// </summary>
        public Int32 VertexCount
        {
            get
            {
                return m_VertexList.Count;
            }
        }

        /// <summary>
        /// метод AddVertex добавляет вершину item в конец списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="item">добавляемая вершина</param>
        public void AddVertex(PolyhedronVertex3D item)
        {
            m_VertexList.Add(item);
        }

        /// <summary>
        /// метод InsertVertex добавляет вершину item в список вершин по индексу index, которые принадлежат данной грани
        /// </summary>
        /// <param name="index">индекс добавляемой вершины</param>
        /// <param name="item">добавляемая вершина</param>
        public void InsertVertex(Int32 index, PolyhedronVertex3D item)
        {
            m_VertexList.Insert(index, item);
        }

        /// <summary>
        /// метод RemoveVertex удаляет вершину item из списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="item">удаляемая вершина</param>
        public void RemoveVertex(PolyhedronVertex3D item)
        {
            m_VertexList.Remove(item);
        }*/
        /* это не есть хорошо  ... к тому же возможна несогласованность списка вершин данной грани и списков граней вершин, которые принадлежат данной грани */

        /// <summary>
        /// метод HasVertex проверяет принадлежит ли вершина vertex данной грани (находится ли в списке вершин данной грани)
        /// </summary>
        /// <param name="vertex">проверяемая вершина</param>
        /// <returns>true, если вершина vertex принадлежит данной грани; иначе - false</returns>
        public Boolean HasVertex(PolyhedronVertex3D vertex)
        {
            return (m_VertexList.IndexOf(vertex) != -1);
        }

        /// <summary>
        /// метод GetNeighbourSide возвращает для данной грани соседа по ребру, образованному вершинами edgeVertex1 и edgeVertex2
        /// </summary>
        /// <param name="edgeVertex1">первая вершина ребра</param>
        /// <param name="edgeVertex2">вторая вершина ребра</param>
        /// <returns>сосед по ребру для данной грани</returns>
        public PolyhedronSide3D GetNeighbourSide(PolyhedronVertex3D edgeVertex1, PolyhedronVertex3D edgeVertex2)
        {
            PolyhedronSide3D neighbourSide = null;

            // проверка на то, что edgeVertex1 и edgeVertex2 принадлежат текущей грани
            if (!m_VertexList.Contains(edgeVertex1) || !m_VertexList.Contains(edgeVertex2))
            {
#warning может более специализированное исключение
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must belong this (current) side");
            }
            // проверка на то, что edgeVertex1 и edgeVertex2 образуют ребро
            if (!Object.ReferenceEquals(m_VertexList.GetNextItem(edgeVertex1), edgeVertex2) &&
                !Object.ReferenceEquals(m_VertexList.GetPrevItem(edgeVertex1), edgeVertex2))
            {
#warning может более специализированное исключение
                throw new ArgumentException("edgeVertex1 and edgeVertex2 must form an edge");
            }

            // Цикл по всем граням, которым принадлежит вершина edgeVertex1
            for (Int32 sideIndex = 0; sideIndex < edgeVertex1.SideList.Count; ++sideIndex)
            {
                PolyhedronSide3D currentSide = edgeVertex1.SideList[sideIndex];

                if (!currentSide.VertexList.Contains(edgeVertex2))
                {
                    continue;
                }
                else if (Object.ReferenceEquals(currentSide, this))
                {
                    continue;
                }
                else
                {
                    neighbourSide = currentSide;
                    break;
                }
            }
            // Цикл по всем граням, которым принадлежит вершина edgeVertex1

            return neighbourSide;
        }

        /// <summary>
        /// m_VertexList - список вершин, которые принадлежат данной грани
        /// </summary>
        private CyclicList<PolyhedronVertex3D> m_VertexList;
        /// <summary>
        /// m_SideNormal - "внешняя" нормаль к грани (вычисляется извне)
        /// </summary>
        private Vector3D m_SideNormal;
    }
}
