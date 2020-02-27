using System;
using System.Collections.Generic;
using System.Text;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// класс VertexClass представляет 3D вершину многогранника
    /// </summary>
    public class VertexClass
    {
        /// <summary>
        /// m_SideList - список граней, которым принадлежит данная вершина
        /// </summary>
        private List<SideClass> m_SideList;

        /// <summary>
        /// m_XCoord - координата X вершины
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y вершины
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z вершины
        /// </summary>
        private Double m_ZCoord;

        /// <summary>
        /// Dimension - размерность вершины (в нашем случае 3)
        /// </summary>
        public readonly Int32 Dimension = 3;
        /// <summary>
        /// Index - индекс вершины в списке всех вершин многогранника
        /// </summary>
        public readonly Int32 Index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XCoord"></param>
        /// <param name="YCoord"></param>
        /// <param name="ZCoord"></param>
        /// <param name="VertexIndex"></param>
        public VertexClass(Double XCoord, Double YCoord, Double ZCoord, Int32 VertexIndex)
        {
            this.Index = VertexIndex;

            this.m_SideList = new List<SideClass>();
            this.m_XCoord = XCoord;
            this.m_YCoord = YCoord;
            this.m_ZCoord = ZCoord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Vertex"></param>
        /// <param name="VertexIndex"></param>
        public VertexClass(Point3D Vertex, Int32 VertexIndex) : this(Vertex.XCoord, Vertex.YCoord, Vertex.ZCoord, VertexIndex)
        {
        }

        /// <summary>
        /// метод AddSide добавляет грань Side в список граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="Side">добавляемая грань</param>
        public void AddSide(SideClass Side)
        {
            m_SideList.Add(Side);
        }

        /// <summary>
        /// метод RemoveSide удаляет грань Side из списка граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="Side">удаляемая грань</param>
        public void RemoveSide(SideClass Side)
        {
            m_SideList.Remove(Side);
        }

        /// <summary>
        /// свойство-индексатор для доступа (чтение) к грани из списка граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="SideIndex">индекс грани в списке граней</param>
        /// <returns></returns>
        public SideClass this[Int32 SideIndex]
        {
            get
            {
                return m_SideList[SideIndex];
            }
            /*set
            {
            }*/
        }

        /// <summary>
        /// свойство SideCount возвращает количество граней в списке граней, которым принадлежит данная вершина
        /// </summary>
        public Int32 SideCount
        {
            get
            {
                return m_SideList.Count;
            }
        }

        /// <summary>
        /// XCoord - свойство для доступа к координате X вершины
        /// </summary>
        public Double XCoord
        {
            get
            {
                return m_XCoord;
            }
            set
            {
                m_XCoord = value;
            }
        }
        /// <summary>
        /// YCoord - свойство для доступа к координате Y вершины
        /// </summary>
        public Double YCoord
        {
            get
            {
                return m_YCoord;
            }
            set
            {
                m_YCoord = value;
            }
        }
        /// <summary>
        /// ZCoord - свойство для доступа к координате Z вершины
        /// </summary>
        public Double ZCoord
        {
            get
            {
                return m_ZCoord;
            }
            set
            {
                m_ZCoord = value;
            }
        }
    }

    /// <summary>
    /// класс SideClass представляет грань многогранника
    /// </summary>
    public class SideClass
    {
        /// <summary>
        /// m_VertexList - список вершин, которые принадлежат данной грани
        /// </summary>
        private CyclicList<VertexClass> m_VertexList;
        /// <summary>
        /// m_SideNormal - "внешняя" нормаль к грани (вычисляется извне)
        /// </summary>
        private Vector3D m_SideNormal;
        /// <summary>
        /// m_Index - индекс грани в списке всех граней многогранника
        /// </summary>
        private Int32 m_Index;

        public SideClass(List<VertexClass> VertexList, Int32 SideIndex, Vector3D SideNormal)
        {
            this.m_Index = SideIndex;

            this.m_VertexList = new CyclicList<VertexClass>();
            foreach (VertexClass Vertex in VertexList)
            {
                AddVertex(Vertex);
            }

            this.m_SideNormal = SideNormal;
        }

        /// <summary>
        /// метод AddVertex добавляет вершину Vertex в список вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="Vertex">добавляемая вершина</param>
        public void AddVertex(VertexClass Vertex)
        {
            m_VertexList.Add(Vertex);
            Vertex.AddSide(this);
        }

        /// <summary>
        /// метод RemoveVertex удаляет вершину Vertex из списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="Vertex">удаляемая вершина</param>
        public void RemoveVertex(VertexClass Vertex)
        {
            m_VertexList.Remove(Vertex);
            Vertex.RemoveSide(this);
        }

        /// <summary>
        /// метод HasVertex возвращает true, если вершина Vertex принадлежит данной плоскости
        /// </summary>
        /// <param name="Vertex"></param>
        /// <returns></returns>
        public Boolean HasVertex(VertexClass Vertex)
        {
            return (m_VertexList.IndexOf(Vertex) != -1);
        }

        /// <summary>
        /// метод GetNeighbourSide возвращает для данной грани соседа по ребру, образованному вершинами EdgeVertex1 и EdgeVertex2
        /// </summary>
        /// <param name="EdgeVertex1">первая вершина ребра</param>
        /// <param name="EdgeVertex2">вторая вершина ребра</param>
        /// <returns>сосед по ребру для данной грани</returns>
        public SideClass GetNeighbourSide(VertexClass EdgeVertex1, VertexClass EdgeVertex2)
        {
            SideClass NeighbourSide = null;

            // индексы вершин 1 и 2 в списке вершин данной грани
            Int32 EdgeVertex1Index = m_VertexList.IndexOf(EdgeVertex1);
            Int32 EdgeVertex2Index = m_VertexList.IndexOf(EdgeVertex2);

            // проверка на то, что EdgeVertex1 и EdgeVertex2 принадлежат текущей грани
            if ((EdgeVertex1Index == -1) || (EdgeVertex2Index == -1))
            {
                #warning по идее кидать надо исключение потомок класса ArgumentException
                throw new ArgumentException("EdgeVertex1 and EdgeVertex2 must belong this (current) side");
            }
            // проверка на то, что EdgeVertex1 и EdgeVertex2 на самом деле ребро
            if ((m_VertexList.NextItemIndex(EdgeVertex1Index) != EdgeVertex2Index) && (m_VertexList.PrevItemIndex(EdgeVertex1Index) != EdgeVertex2Index))
            {
                #warning по идее кидать надо исключение потомок класса ArgumentException
                throw new ArgumentException("EdgeVertex1 and EdgeVertex2 must form edge");
            }

            for (Int32 SideIndex = 0; SideIndex < EdgeVertex1.SideCount; SideIndex++)
            {
                if (EdgeVertex1[SideIndex].m_VertexList.IndexOf(EdgeVertex2) == -1)
                {
                    continue;
                }
                else if (Object.ReferenceEquals(EdgeVertex1[SideIndex], this))
                {
                    continue;
                }
                else
                {
                    NeighbourSide = EdgeVertex1[SideIndex];
                    break;
                }
            }

            return NeighbourSide;
        }

        /// <summary>
        /// свойство-индексатор для доступа (чтение) к вершине из списка вершин, которые принадлежат данной грани
        /// </summary>
        /// <param name="VertexIndex">индекс вершины в списке вершин</param>
        /// <returns></returns>
        public VertexClass this[Int32 VertexIndex]
        {
            get
            {
                return m_VertexList[VertexIndex];
            }
            /*set
            {
            }*/
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
        /// SideNormal - свойство для доступа (чтение) к "внешней" нормали грани
        /// </summary>
        public Vector3D SideNormal
        {
            get
            {
                return m_SideNormal;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PolyhedronClass
    {
    }
}
