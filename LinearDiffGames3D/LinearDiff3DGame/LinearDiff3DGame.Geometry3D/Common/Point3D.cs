using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// структура Point3D представляет 3D точку
    /// </summary>
    public struct Point3D
    {
        /// <summary>
        /// конструктор структуры Point3D
        /// </summary>
        /// <param name="coordX">X координата 3D точки</param>
        /// <param name="coordY">Y координата 3D точки</param>
        /// <param name="coordZ">Z координата 3D точки</param>
        public Point3D(Double coordX, Double coordY, Double coordZ)
        {
            m_XCoord = coordX;
            m_YCoord = coordY;
            m_ZCoord = coordZ;
        }

        /// <summary>
        /// XCoord - свойство для доступа к координате X точки
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
        /// YCoord - свойство для доступа к координате Y точки
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
        /// ZCoord - свойство для доступа к координате Z точки
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

        /// <summary>
        /// m_XCoord - координата X точки
        /// </summary>
        private Double m_XCoord;
        /// <summary>
        /// m_YCoord - координата Y точки
        /// </summary>
        private Double m_YCoord;
        /// <summary>
        /// m_ZCoord - координата Z точки
        /// </summary>
        private Double m_ZCoord;
    }
}
