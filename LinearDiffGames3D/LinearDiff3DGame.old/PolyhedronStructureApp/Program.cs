using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PolyhedronStructureViewer
{
    class Program
    {
        private const String InputDataFileName = "input.dat";
        static void Main(string[] args)
        {
            Program MainProgram = new Program();

            Point3D[] VertexArray = MainProgram.GetVertexArrayFromFile(InputDataFileName);
            PolyhedronStructureClass PSC = new PolyhedronStructureClass(VertexArray);

            String[] PolyhedronStructureDescription = PSC.GetPolyhedronStructureDescription();
            Console.WriteLine("Polyhedron Structure Description :");
            for (Int32 PSDIndex = 0; PSDIndex < PolyhedronStructureDescription.Length; PSDIndex++)
            {
                Console.WriteLine(PolyhedronStructureDescription[PSDIndex]);
            }

            Console.WriteLine();

            String[] PolyhedronGraphDescription = PSC.GetPolyhedronGraphDescription();
            Console.WriteLine("Polyhedron Graph Description :");
            for (Int32 PGDIndex = 0; PGDIndex < PolyhedronGraphDescription.Length; PGDIndex++)
            {
                Console.WriteLine(PolyhedronGraphDescription[PGDIndex]);
            }

            Console.WriteLine();

            /*String[] OrderedPolyhedronGraphDescription = PSC.GetOrderedPolyhedronGraphDescription();
            Console.WriteLine("Polyhedron Graph Description after ordering connections :");
            for (Int32 PGDIndex = 0; PGDIndex < OrderedPolyhedronGraphDescription.Length; PGDIndex++)
            {
                Console.WriteLine(OrderedPolyhedronGraphDescription[PGDIndex]);
            }

            Console.WriteLine();*/

            String[] PolyhedronStructureDescription2 = PSC.GetPolyhedronStructureDescription2();
            Console.WriteLine("Polyhedron Structure Description after restoration from graph :");
            for (Int32 PSDIndex = 0; PSDIndex < PolyhedronStructureDescription2.Length; PSDIndex++)
            {
                Console.WriteLine(PolyhedronStructureDescription2[PSDIndex]);
            }

            Console.WriteLine();

            /*String[] GFiGraphDescription = PSC.GetGFiGraphDescription();
            Console.WriteLine("GFi Graph Description :");
            for (Int32 PGDIndex = 0; PGDIndex < GFiGraphDescription.Length; PGDIndex++)
            {
                Console.WriteLine(GFiGraphDescription[PGDIndex]);
            }*/

            Console.ReadLine();
        }

        private Point3D[] GetVertexArrayFromFile(String FileName)
        {
            /*List<Point3D> VertexArrayList = new List<Point3D>();

            using (StreamReader sr = new StreamReader(FileName))
            {
                while (!sr.EndOfStream)
                {
                    String[] StrPointData = sr.ReadLine().Split(' ', '\t');
                    // if (StrPointData.Length != 3) ???????
                    Double XCoord = Double.Parse(StrPointData[0]);
                    Double YCoord = Double.Parse(StrPointData[1]);
                    Double ZCoord = Double.Parse(StrPointData[2]);

                    VertexArrayList.Add(new Point3D(XCoord, YCoord, ZCoord));
                }
            }

            return VertexArrayList.ToArray();*/
            /*Double MaxCValue = 2.5;
            Int32 VertexCount = 5;

            Point3D[] FinalSet = new Point3D[VertexCount];
            FinalSet[0] = new Point3D(0, 0, 0);
            FinalSet[1] = new Point3D(MaxCValue, MaxCValue, MaxCValue);
            FinalSet[2] = new Point3D(-MaxCValue, MaxCValue, MaxCValue);
            FinalSet[3] = new Point3D(-MaxCValue, -MaxCValue, MaxCValue);
            FinalSet[4] = new Point3D(MaxCValue, -MaxCValue, MaxCValue);*/

            Point3D[] FinalSet = new Point3D[6];
            FinalSet[0] = new Point3D(0, 0, 1);
            FinalSet[1] = new Point3D(1, 1, 0);
            FinalSet[2] = new Point3D(-1, 1, 0);
            FinalSet[3] = new Point3D(-1, -1, 0);
            FinalSet[4] = new Point3D(1, -1, 0);
            FinalSet[5] = new Point3D(0, 0, -1);

            return FinalSet;
        }
    }
}
