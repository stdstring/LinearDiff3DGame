using System;

namespace LinearDiff3DGame.Geometry3D.Common
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
            : this()
        {
            KoeffA = koeffA;
            KoeffB = koeffB;
            KoeffC = koeffC;
            KoeffD = koeffD;
        }

        /// <summary>
        /// коэффициент A основного уравнения плоскости
        /// </summary>
        public Double KoeffA
        {
            get; private set;
        }
        /// <summary>
        /// коэффициент B основного уравнения плоскости
        /// </summary>
        public Double KoeffB
        {
            get; private set;
        }
        /// <summary>
        /// коэффициент C основного уравнения плоскости
        /// </summary>
        public Double KoeffC
        {
            get; private set;
        }
        /// <summary>
        /// коэффициент D основного уравнения плоскости
        /// </summary>
        public Double KoeffD
        {
            get; private set;
        }
    }
}