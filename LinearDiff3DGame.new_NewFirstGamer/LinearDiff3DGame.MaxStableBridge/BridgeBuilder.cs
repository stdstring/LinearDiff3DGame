using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Geometry3D.PolyhedronFactory;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.Corrector;
using LinearDiff3DGame.MaxStableBridge.FictiousNodes;
using LinearDiff3DGame.MaxStableBridge.Gamers;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge
{
    public class BridgeBuilder
    {
        public BridgeBuilder(BridgeBuilderData initData)
        {
            approxComp = initData.ApproxComp;
            deltaT = initData.DeltaT;
            matrixA = initData.MatrixA;
            scalingMaxMinThreshold = initData.ScalingMaxMinThreshold;
            firstGamers = CreateFirstGamers(initData.FirstGamers);
            secondGamers = CreateSecondGamers(initData.SecondGamers);
            // TODO : сделать выбор алгоритма
            Polyhedron3DFromPointsFactory polyhedronFactory = new Polyhedron3DFromPointsFactory(approxComp);
            CurrentTSection = polyhedronFactory.CreatePolyhedron(initData.TerminalSet);
            Polyhedron3DGraphFactory graphFactory = new Polyhedron3DGraphFactory();
            Polyhedron3DGraphSimpleTriangulator triangulator = new Polyhedron3DGraphSimpleTriangulator();
            currentTSectionGraph = triangulator.Triangulate(graphFactory.CreatePolyhedronGraph(CurrentTSection));
            InverseTime = 0.0;
            DirectTransform = Matrix.IdentityMatrix(3);
            ReverseTransform = Matrix.IdentityMatrix(3);
            // TODO : ваще уродливая вещь
            fundCauchyMatrix = new FundCauchyMatrix(matrixA, initData.Y1Y2Y3Indexes, deltaT / 10);
        }

        // TODO : реFUCKторинг
        public Polyhedron3D NextIteration()
        {
            CheckAndScaleIfNeed();
            InverseTime += deltaT;
            fundCauchyMatrix.Calculate(InverseTime);
            ++generationID;
            foreach(FirstGamer gamer in firstGamers)
            {
                currentTSectionGraph = gamer.Action(currentTSectionGraph,
                                                    fundCauchyMatrix.Current,
                                                    generationID,
                                                    DirectTransform);
            }
            // TODO : сделать выбор алгоритма
            IBridgeGraphCorrector corrector = new BridgeGraphCorrector_old(approxComp);
            foreach(SecondGamer gamer in secondGamers)
            {
                SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
                currentTSectionGraph = gamer.Action(currentTSectionGraph,
                                                    fundCauchyMatrix.Current,
                                                    connSet,
                                                    DirectTransform);
                currentTSectionGraph = corrector.CheckAndCorrectBridgeGraph(connSet,
                                                                            currentTSectionGraph);
            }
            // TODO : нужно ли удаление фиктивных граней
            Polyhedron3DFromGraphFactory polyhedronFactory = new Polyhedron3DFromGraphFactory(approxComp, new LESKramer3Solver());
            
            currentTSectionGraph = NormalizeGraph(currentTSectionGraph);
            Polyhedron3D intermediateTSection = polyhedronFactory.CreatePolyhedron(currentTSectionGraph);
            FictiousNodeRemover fictiousNodeRemover = new FictiousNodeRemover();
            currentTSectionGraph = fictiousNodeRemover.Action(intermediateTSection,
                                                              currentTSectionGraph,
                                                              corrector);
            currentTSectionGraph = NormalizeGraph(currentTSectionGraph);
            return CurrentTSection = polyhedronFactory.CreatePolyhedron(currentTSectionGraph);
            //return CurrentTSection = GetCurrentTSection(currentTSectionGraph);
        }

        public Matrix DirectTransform { get; private set; }
        public Matrix ReverseTransform { get; private set; }
        public Double InverseTime { get; private set; }
        public Polyhedron3D CurrentTSection { get; private set; }
        public Double DeltaT { get { return deltaT; } }

        private static IEnumerable<FirstGamer> CreateFirstGamers(IEnumerable<FirstGamerInitData> firstGamersData)
        {
            IList<FirstGamer> firstGamerList = new List<FirstGamer>();
            foreach(FirstGamerInitData firstGamerData in firstGamersData)
                firstGamerList.Add(new FirstGamer(firstGamerData));
            return firstGamerList;
        }

        private static IEnumerable<SecondGamer> CreateSecondGamers(IEnumerable<GamerInitData> secondGamersData)
        {
            IList<SecondGamer> secondGamerList = new List<SecondGamer>();
            foreach(GamerInitData secondGamerData in secondGamersData)
                secondGamerList.Add(new SecondGamer(secondGamerData));
            return secondGamerList;
        }

        // TODO : перенести в класс, ответственный за трансформацию
        // TODO : подумать о сигнатуре метода
        // ReSharper disable UnusedMethodReturnValue.Local
        private Boolean CheckAndScaleIfNeed()
        // ReSharper restore UnusedMethodReturnValue.Local
        {
            return true;            Polyhedron3DGraph_Utils graphUtils = new Polyhedron3DGraph_Utils();
            IDictionary<String, Pair<Vector3D, Double>> thicknessPairs =
                graphUtils.CalcGraphExtremeThickness(currentTSectionGraph);
            Pair<Vector3D, Double> maxThickness = thicknessPairs[Polyhedron3DGraph_Utils.MaxThicknessKey];
            Pair<Vector3D, Double> minThickness = thicknessPairs[Polyhedron3DGraph_Utils.MinThicknessKey];
            if(graphUtils.NeedScaling(minThickness, maxThickness, scalingMaxMinThreshold))
            {
                Matrix prevDirectScaling = DirectTransform;
                Matrix prevReverseScaling = ReverseTransform;
                Pair<Vector3D, Double> scalingParams = graphUtils.CalcScaling(minThickness,
                                                                              maxThickness,
                                                                              scalingMaxMinThreshold);
                Matrix currentDirectScaling = ScalingTransformation3D.GetTransformationMatrix(scalingParams.Item1,
                                                                                              scalingParams.Item2);
                Matrix currentReverseScaling = ScalingTransformation3D.GetTransformationMatrix(scalingParams.Item1,
                                                                                               1 / scalingParams.Item2);
                Polyhedron3DGraph_ScaleTransformer transformer =
                    new Polyhedron3DGraph_ScaleTransformer();
                currentTSectionGraph = transformer.Process(currentTSectionGraph,
                                                           currentDirectScaling,
                                                           currentReverseScaling);
                DirectTransform = currentDirectScaling * prevDirectScaling;
                ReverseTransform = prevReverseScaling * currentReverseScaling;
                return true;
            }
            return false;
        }

        private static Polyhedron3DGraph NormalizeGraph(Polyhedron3DGraph graph)
        {
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
                graph.NodeList[nodeIndex].ID = nodeIndex;
            return graph;
        }

        private Polyhedron3D GetCurrentTSection(Polyhedron3DGraph sourceGraph)
        {
            // Обязательно здесь сделать копию !!! у нас граф до сих пор мьютабелен
            Polyhedron3DGraph graph = new Polyhedron3DGraph(sourceGraph);
            Polyhedron3DGraph_ScaleTransformer transformer =
                    new Polyhedron3DGraph_ScaleTransformer();
            graph = transformer.Process(graph, ReverseTransform, DirectTransform);
            Polyhedron3DFromGraphFactory polyhedronFactory = new Polyhedron3DFromGraphFactory(approxComp, new LESKramer3Solver());
            return polyhedronFactory.CreatePolyhedron(graph);
        }

        private readonly ApproxComp approxComp;
        private readonly Double deltaT;
        private readonly Matrix matrixA;
        private readonly IEnumerable<FirstGamer> firstGamers;
        private readonly IEnumerable<SecondGamer> secondGamers;
        // TODO : перенести в класс, ответственный за трансформацию
        private readonly Double scalingMaxMinThreshold;
        // TODO : ваще уродливая вещь
        private readonly FundCauchyMatrix fundCauchyMatrix;
        private Polyhedron3DGraph currentTSectionGraph;
        private Int32 generationID;
    }
}