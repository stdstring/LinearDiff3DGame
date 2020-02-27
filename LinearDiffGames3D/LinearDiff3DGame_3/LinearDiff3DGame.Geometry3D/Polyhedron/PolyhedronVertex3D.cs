using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// представляет вершину 3-мерного многогранника
    /// </summary>
    public class PolyhedronVertex3D
    {
        /// <summary>
        /// конструктор класса PolyhedronVertex3D
        /// </summary>
        /// <param name="xCoord">X координата вершины</param>
        /// <param name="yCoord">Y координата вершины</param>
        /// <param name="zCoord">Z координата вершины</param>
        /// <param name="vertexID">ID вершины</param>
        public PolyhedronVertex3D(Double xCoord, Double yCoord, Double zCoord, Int32 vertexID)
        {
            ID = vertexID;

            m_SideList = new List<PolyhedronSide3D>();
            m_XCoord = xCoord;
            m_YCoord = yCoord;
            m_ZCoord = zCoord;
        }

        /// <summary>
        /// конструктор класса PolyhedronVertex3D
        /// </summary>
        /// <param name="vertex">координаты вершины (в виде объекта Point3D)</param>
        /// <param name="vertexID">ID вершины</param>
        public PolyhedronVertex3D(Point3D vertex, Int32 vertexID) : this(vertex.XCoord, vertex.YCoord, vertex.ZCoord, vertexID)
        {
        }

        /// <summary>
        /// ID - уникальный идентификатор вершины
        /// </summary>
        public readonly Int32 ID;

        /// <summary>
        /// Dimension - размерность вершины (в нашем случае 3)
        /// </summary>
        public const Int32 Dimension = 3;

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

        /* это не есть хорошо ... к тому же возможна несогласованность списка граней данной вершины и списков вершин граней, которым принадлежит данная вершина */
#warning Incorrect architecture

        /// <summary>
        /// список граней, которым принадлежит данная вершина
        /// </summary>
        public IList<PolyhedronSide3D> SideList
        {
            get
            {
                return m_SideList;
            }
        }

        /*/// <summary>
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
        /// свойство-индексатор для доступа (чтение) к грани из списка граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="index">индекс грани в списке граней</param>
        /// <returns>одна из граней из списка граней</returns>
        public PolyhedronSide3D this[Int32 index]
        {
            get
            {
                return m_SideList[index];
            }
        }

        /// <summary>
        /// метод AddSide добавляет грань item в список граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="item">добавляемая грань</param>
        public void AddSide(PolyhedronSide3D item)
        {
            m_SideList.Add(item);
        }

        /// <summary>
        /// метод RemoveSide удаляет грань item из списка граней, которым принадлежит данная вершина
        /// </summary>
        /// <param name="item">удаляемая грань</param>
        public void RemoveSide(PolyhedronSide3D item)
        {
            m_SideList.Remove(item);
        }*/
        /* это не есть хорошо ... к тому же возможна несогласованность списка граней данной вершины и списков вершин граней, которым принадлежит данная вершина */


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
        /// m_SideList - список граней, которым принадлежит данная вершина
        /// </summary>
        private List<PolyhedronSide3D> m_SideList;
    }
}
