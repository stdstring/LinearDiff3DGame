using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Geometry3D
{
    /// <summary>
    /// структура, представляющая 3D плоскость
    /// </summary>
    public struct Plane3D
    {
        /// <summary>
        /// конструктор структуры Plane3D
        /// </summary>
        /// <param name="koeffA">коэффициент A основного уравнения плоскости</param>
        /// <param name="koeffB">коэффициент B основного уравнения плоскости</param>
        /// <param name="koeffC">коэффициент C основного уравнения плоскости</param>
        /// <param name="koeffD">коэффициент D основного уравнения плоскости</param>
        public Plane3D(Double koeffA, Double koeffB, Double koeffC, Double koeffD)
        {
            m_KoeffA = koeffA;
            m_KoeffB = koeffB;
            m_KoeffC = koeffC;
            m_KoeffD = koeffD;
        }

        /// <summary>
        /// коэффициент A основного уравнения плоскости
        /// </summary>
        public Double KoeffA
        {
            get
            {
                return m_KoeffA;
            }
        }
        /// <summary>
        /// коэффициент B основного уравнения плоскости
        /// </summary>
        public Double KoeffB
        {
            get
            {
                return m_KoeffB;
            }
        }
        /// <summary>
        /// коэффициент C основного уравнения плоскости
        /// </summary>
        public Double KoeffC
        {
            get
            {
                return m_KoeffC;
            }
        }
        /// <summary>
        /// коэффициент D основного уравнения плоскости
        /// </summary>
        public Double KoeffD
        {
            get
            {
                return m_KoeffD;
            }
        }
        
        /// <summary>
        /// коэффициент A основного уравнения плоскости
        /// </summary>
        private Double m_KoeffA;
        /// <summary>
        /// коэффициент B основного уравнения плоскости
        /// </summary>
        private Double m_KoeffB;
        /// <summary>
        /// коэффициент C основного уравнения плоскости
        /// </summary>
        private Double m_KoeffC;
        /// <summary>
        /// коэффициент D основного уравнения плоскости
        /// </summary>
        private Double m_KoeffD;
    }
}
