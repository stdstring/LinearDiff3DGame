using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    class Program
    {
        static void Main(string[] args)
        {
            /*PolyhedronClass polyhedron = new PolyhedronClass();
            PolyhedronFactoryClass.ConstructPolyhedron("data1.dat", polyhedron);*/
            
            /*// всякие проверки
             Program currentProgram = new Program();
            
            currentProgram.Check1();
            currentProgram.Check2();*/

            /*Double startTime = 0;
            Double finishTime = 1.4;
            Double deltaScale = 0.05;*/

            RCInputDataReader inputData = new RCInputDataReader(m_InputDataFileName);
            RobustControlBuilderClass robustControlBuilder = RobustControlBuilderClass.CreateRobustControlBuilder(inputData);

            Double[] secondGamerControlList;
            Double[] scaleKoeffList;
            Int32[] nearestBottomBridgeIndicies;
            Boolean[] checkPointLocationList;
            Double[] timeValueList;

            Point3D startPoint = (Point3D)inputData.InputData["StartPoint"];

            Double[] robustControlList = robustControlBuilder.BuildRobustControl(startPoint, out timeValueList, out secondGamerControlList, out scaleKoeffList, out nearestBottomBridgeIndicies, out checkPointLocationList);

            using (StreamWriter sw = new StreamWriter("robust_control.dat", true))
            {
                List<DoublePair> maxStableBridgeSystemParams = robustControlBuilder.MaxStableBridgeSystemParams;
                for (Int32 paramsIndex = 0; paramsIndex < maxStableBridgeSystemParams.Count; paramsIndex++)
                {
                    sw.WriteLine("{0}\t{1}", maxStableBridgeSystemParams[paramsIndex].Number1,
                                             maxStableBridgeSystemParams[paramsIndex].Number2);
                }

                sw.WriteLine();

                for (Int32 robustControlIndex=0; robustControlIndex<robustControlList.Length;robustControlIndex++)
                {
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", timeValueList[robustControlIndex],
                                                                 robustControlList[robustControlIndex],
                                                                 secondGamerControlList[robustControlIndex],
                                                                 scaleKoeffList[robustControlIndex],
                                                                 nearestBottomBridgeIndicies[robustControlIndex],
                                                                 (checkPointLocationList[robustControlIndex] ? 1 : 0));
                }
            }

            Console.WriteLine("press [enter] ...");
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Check1()
        {
            // набор вершин многогранника (многогранник - куб)
            Point3D[] polyhedronVertexes = new Point3D[] { new Point3D(1, 1, 1),
                                                           new Point3D(-1, 1, 1),
                                                           new Point3D(1, -1, 1),
                                                           new Point3D(-1, -1, 1),
                                                           new Point3D(1, 1, -1),
                                                           new Point3D(-1, 1, -1),
                                                           new Point3D(1, -1, -1),
                                                           new Point3D(-1, -1, -1)};

            PolyhedronClass polyhedron = PolyhedronFactoryClass.ConstructPolyhedron(polyhedronVertexes);

            // луч №1 проходит через точку givenPoint1 (начинается естественно в точке O)
            Point3D givenPoint1 = new Point3D(0.5, 0.5, 0.5);
            // с многогранником должен пересекаться в точке (1, 1, 1)
            Point3D crossingPoint1 = polyhedron.GetCrossingPointWithRay_FullEmun(givenPoint1);
            Console.WriteLine("CrossingPoint1 = ({0}; {1}; {2})", crossingPoint1.XCoord, crossingPoint1.YCoord, crossingPoint1.ZCoord);

            // луч №2 проходит через точку givenPoint2 (начинается естественно в точке O)
            Point3D givenPoint2 = new Point3D(0, 0, 0.5);
            // с многогранником должен пересекаться в точке (0, 0, 1)
            Point3D crossingPoint2 = polyhedron.GetCrossingPointWithRay_FullEmun(givenPoint2);
            Console.WriteLine("CrossingPoint2 = ({0}; {1}; {2})", crossingPoint2.XCoord, crossingPoint2.YCoord, crossingPoint2.ZCoord);

            // луч №3 проходит через точку givenPoint3 (начинается естественно в точке O)
            Point3D givenPoint3 = new Point3D(0.25, 0, 0.5);
            // с многогранником должен пересекаться в точке (0.5, 0, 1)
            Point3D crossingPoint3 = polyhedron.GetCrossingPointWithRay_FullEmun(givenPoint3);
            Console.WriteLine("CrossingPoint3 = ({0}; {1}; {2})", crossingPoint3.XCoord, crossingPoint3.YCoord, crossingPoint3.ZCoord);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Check2()
        {
            // набор вершин многогранника (многогранник - куб)
            Point3D[] polyhedronVertexes = new Point3D[] { new Point3D(1, 1, 1),
                                                           new Point3D(-1, 1, 1),
                                                           new Point3D(1, -1, 1),
                                                           new Point3D(-1, -1, 1),
                                                           new Point3D(1, 1, -1),
                                                           new Point3D(-1, 1, -1),
                                                           new Point3D(1, -1, -1),
                                                           new Point3D(-1, -1, -1)};

            PolyhedronClass polyhedron = PolyhedronFactoryClass.ConstructPolyhedron(polyhedronVertexes);

            // ближайшая точка - (0.1, -0.1, 1)
            Point3D givenPoint1 = new Point3D(0.1, -0.1, 2);
            Point3D nearestPoint1 = polyhedron.GetNearestPoint4Given_FullEmun(givenPoint1);
            Console.WriteLine("NearestPoint1 = ({0}; {1}; {2})", nearestPoint1.XCoord, nearestPoint1.YCoord, nearestPoint1.ZCoord);

            // ближайшая точка - (1, 1, 1)
            Point3D givenPoint2 = new Point3D(2, 2, 2);
            Point3D nearestPoint2 = polyhedron.GetNearestPoint4Given_FullEmun(givenPoint2);
            Console.WriteLine("NearestPoint2 = ({0}; {1}; {2})", nearestPoint2.XCoord, nearestPoint2.YCoord, nearestPoint2.ZCoord);

            // ближайшая точка - (0.1, 0.1, 1)
            Point3D givenPoint3 = new Point3D(0.1, 0.1, 2);
            Point3D nearestPoint3 = polyhedron.GetNearestPoint4Given_FullEmun(givenPoint3);
            Console.WriteLine("NearestPoint3 = ({0}; {1}; {2})", nearestPoint3.XCoord, nearestPoint3.YCoord, nearestPoint3.ZCoord);

            // ближайшая точка - (0, 1, 1)
            Point3D givenPoint4 = new Point3D(0, 2, 2);
            Point3D nearestPoint4 = polyhedron.GetNearestPoint4Given_FullEmun(givenPoint4);
            Console.WriteLine("NearestPoint4 = ({0}; {1}; {2})", nearestPoint4.XCoord, nearestPoint4.YCoord, nearestPoint4.ZCoord);
        }

        /// <summary>
        /// 
        /// </summary>
        private const String m_InputDataFileName = "InputData.xml";

    }
}
