using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Bridge;

namespace LinearDiff3DGame.MaxStableBridge.Generator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const Double epsilon = 1e-9;
            const String inputDataFile = "OscillatorInput.xml";
            const Double finishTime = 10;
            const String outputDataFile = "OscillatorBridge.dat";
            const Double z0 = 2.5;

            BridgeBuildController controller = new BridgeBuildController(inputDataFile);
            Console.WriteLine("Generating bridge ...");
            IList<Pair<Double, Polyhedron3D>> sourceBridge = controller.GenerateBridge(finishTime);
            Console.WriteLine("Post processing ...");
            PostProcess postProcess = new PostProcess(new ApproxComp(epsilon));
            IList<Pair<Double, Polyhedron3D>> bridge =
                sourceBridge.Select(pair => new Pair<Double, Polyhedron3D>(pair.Item1, postProcess.Process(pair.Item2))).ToList();
            Int32 sectionCount = sourceBridge.Count;
            Console.WriteLine("Serializing ...");
            BridgeSerializer bridgeSerializer = new BridgeSerializer();
            using(FileStream fs = new FileStream(outputDataFile, FileMode.Create, FileAccess.Write))
            {
                Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> bridgeData =
                    new Pair<int, IEnumerable<Pair<Double, Polyhedron3D>>>(sectionCount, bridge);
                bridgeSerializer.Serialize(fs, bridgeData);
            }

            Console.WriteLine("Generation complete !!!");
            Console.ReadLine();
        }

        //// Для надграфиков функции цены только !!!!!
        //// Что делаем:
        //// 1) Отсексаем все вершины с z > 0
        //// 2) Создаем грань из вершин с z = 0
        //// 3) Сдвигаем все вершины вверх на величину Z0
        //private static Polyhedron3D PostProcess(Polyhedron3D sourcePolyhedron, Double z0)
        //{
        //    const Double epsilon = 1e-9;
        //    ApproxComp approxComp = new ApproxComp(epsilon);
        //    Dictionary<PolyhedronVertex3D, PolyhedronVertex3D> old2newVertexes =
        //        new Dictionary<PolyhedronVertex3D, PolyhedronVertex3D>();
        //    IList<PolyhedronVertex3D> newVertexList = new List<PolyhedronVertex3D>();
        //    Int32 vertexID = 0;
        //    foreach(PolyhedronVertex3D vertex in sourcePolyhedron.VertexList)
        //    {
        //        if(approxComp.LE(vertex.ZCoord, 0))
        //        {
        //            PolyhedronVertex3D newVertex = new PolyhedronVertex3D(vertex.XCoord, vertex.YCoord, vertex.ZCoord, vertexID++);
        //            newVertexList.Add(newVertex);
        //            old2newVertexes.Add(vertex, newVertex);
        //        }
        //    }
        //    IList<PolyhedronSide3D> newSideList = new List<PolyhedronSide3D>();
        //    Int32 sideID = 0;
        //    foreach(PolyhedronSide3D side in sourcePolyhedron.SideList)
        //    {
        //        if(side.VertexList.All(old2newVertexes.ContainsKey))
        //            newSideList.Add(CreateNewSide(side, sideID++, old2newVertexes));
        //    }
        //        newSideList.Add(CreateBackSide(newSideList, approxComp));

        //    return new Polyhedron3D(newSideList, newVertexList);
        //}

        //private static PolyhedronSide3D CreateNewSide(PolyhedronSide3D oldSide,
        //                                              Int32 sideID,
        //                                              IDictionary<PolyhedronVertex3D, PolyhedronVertex3D> old2newVertexes)
        //{
        //    IList<PolyhedronVertex3D> newVertexList = new List<PolyhedronVertex3D>(oldSide.VertexList.Count);
        //    foreach(PolyhedronVertex3D oldVertex in oldSide.VertexList)
        //        newVertexList.Add(old2newVertexes[oldVertex]);
        //    return new PolyhedronSide3D(newVertexList, sideID, oldSide.SideNormal);
        //}

        //private static PolyhedronSide3D CreateBackSide(IList<PolyhedronSide3D> otherSides, ApproxComp approxComp)
        //{
        //    PolyhedronSide3D firstNeighbor = otherSides.First(side => side.VertexList.Count(vertex => approxComp.EQ(vertex.ZCoord, 0)) == 2);
        //    PolyhedronVertex3D firstVertex = null, secondVertex = null;
        //    for(Int32 vertexIndex = 0; vertexIndex < firstNeighbor.VertexList.Count; ++vertexIndex)
        //    {
        //        secondVertex = firstNeighbor.VertexList[vertexIndex];
        //        firstVertex = firstNeighbor.VertexList.GetNextItem(vertexIndex);
        //        if(approxComp.EQ(secondVertex.ZCoord, 0) && approxComp.EQ(firstVertex.ZCoord, 0))
        //            break;
        //    }
        //    IList<PolyhedronVertex3D> backVertexList = new List<PolyhedronVertex3D>();
        //    backVertexList.Add(firstVertex);
        //    backVertexList.Add(secondVertex);
        //    PolyhedronSide3D currentSide = otherSides.First(side => side != firstNeighbor &&
        //                                                            side.HasVertex(secondVertex) &&
        //                                                            side.VertexList.Count(vertex => approxComp.EQ(vertex.ZCoord, 0)) == 2);
        //    while (currentSide != firstNeighbor)
        //    {
        //        PolyhedronVertex3D currentVertex = backVertexList[backVertexList.Count - 1];
        //        backVertexList.Add(currentSide.VertexList.GetPrevItem(currentVertex));
        //        currentVertex = backVertexList[backVertexList.Count - 1];
        //        PolyhedronSide3D lastCurrentSide = currentSide;
        //        currentSide = otherSides.First(side => side != lastCurrentSide &&
        //                                               side.HasVertex(currentVertex) &&
        //                                               side.VertexList.Count(vertex => approxComp.EQ(vertex.ZCoord, 0)) == 2);
        //    }
        //    return new PolyhedronSide3D(backVertexList, otherSides[otherSides.Count - 1].ID + 1, new Vector3D(0, 0, 1));
        //}
    }
}