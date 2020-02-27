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

namespace LinearDiff3DGame.MaxStableBridge.Old
{
    /// <summary>
    /// 
    /// </summary>
    public class MaxStableBridgeBuilder_old
    {
        public MaxStableBridgeBuilder_old()
        {
            IMaxStableBridgeBuilderInitializer initializer = MaxStableBridgeBuilderInitializer.CreateOscillatorData();
            m_MatrixA = (Matrix)initializer.GetDataByKey("MatrixA");
            m_MatrixB = (Matrix)initializer.GetDataByKey("MatrixB");
            m_MatrixC = (Matrix)initializer.GetDataByKey("MatrixC");
            m_DeltaT = (Double)initializer.GetDataByKey("DeltaT");
            Double mpMax = (Double)initializer.GetDataByKey("MpMax");
            Double mpMin = (Double)initializer.GetDataByKey("MpMin");
            Double mqMax = (Double)initializer.GetDataByKey("MqMax");
            Double mqMin = (Double)initializer.GetDataByKey("MqMin");
            Point3D[] terminalSetVertexes = (Point3D[])initializer.GetDataByKey("TerminalSet");
            Int32[] y1y2y3Indexes = (Int32[])initializer.GetDataByKey("Y1Y2Y3Indexes");
            maxminThresholdValue = (Double)initializer.GetDataByKey("ScalingMaxMinThresholdValue");

            const Double epsilon = 1e-9;
            m_ApproxComparer = new ApproxComp(epsilon);

            m_Corrector = new BridgeGraphCorrector_old(m_ApproxComparer);

            m_GenerationID = 0;
            m_FictiousNodeRemover = new FictiousNodeRemover();

            m_FirstGamer = new FirstGamer(m_ApproxComparer, m_MatrixB, m_DeltaT, mpMax, mpMin);
            m_SecondGamer = new SecondGamer(m_ApproxComparer, m_MatrixC, m_DeltaT, mqMax, mqMin);

            m_CurrentPolyhedronInEq =
                new Polyhedron3DFromPointsFactory(m_ApproxComparer).CreatePolyhedron(terminalSetVertexes);
            m_CurrentPolyhedronGraph = new Polyhedron3DGraphFactory().CreatePolyhedronGraph(m_CurrentPolyhedronInEq);
            m_CurrentPolyhedronGraph = new Polyhedron3DGraphSimpleTriangulator().Triangulate(m_CurrentPolyhedronGraph);

            m_InverseTime = 0;
            m_FundCauchyMatrix = new FundCauchyMatrix(m_MatrixA, y1y2y3Indexes, m_DeltaT / 10);

            //Matrix transformMatrix = new InverseMatrixBuilder().InverseMatrix(m_FundCauchyMatrix.GetFundCauchyMatrix());
            //m_CurrentPolyhedronInNorm = TranformPolyhedron2NormalCoords(m_CurrentPolyhedronInEq, transformMatrix);
        }

        public Polyhedron3D CurrentPolyhedron
        {
            get
            {
                return m_CurrentPolyhedronInEq;
                //return m_CurrentPolyhedronInNorm;
            }
        }

        public Matrix DirectTransformation { get { return directScaling; } }

        public Matrix ReverseTransformation { get { return reverseScaling; } }

        public Double CurrentInverseTime { get { return m_InverseTime; } }

        public Double DeltaT { get { return m_DeltaT; } }

        public void NextIteration()
        {
            CheckAndScaleIfNeed();

            m_InverseTime += m_DeltaT;
            m_FundCauchyMatrix.Calculate(m_InverseTime);

            ++m_GenerationID;
            m_CurrentPolyhedronGraph = m_FirstGamer.Action(m_CurrentPolyhedronGraph,
                                                           m_FundCauchyMatrix.Current,
                                                           m_GenerationID,
                                                           directScaling);

            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
            m_CurrentPolyhedronGraph = m_SecondGamer.Action(m_CurrentPolyhedronGraph,
                                                            m_FundCauchyMatrix.Current,
                                                            connSet,
                                                            directScaling);
            m_CurrentPolyhedronGraph = m_Corrector.CheckAndCorrectBridgeGraph(connSet, m_CurrentPolyhedronGraph);

            m_CurrentPolyhedronGraph = NormalizeGraph(m_CurrentPolyhedronGraph);
            m_CurrentPolyhedronInEq =
                new Polyhedron3DFromGraphFactory(m_ApproxComparer, new LESKramer3Solver()).CreatePolyhedron(m_CurrentPolyhedronGraph);

            m_CurrentPolyhedronGraph = m_FictiousNodeRemover.Action(m_CurrentPolyhedronInEq,
                                                                    m_CurrentPolyhedronGraph,
                                                                    m_Corrector);

            m_CurrentPolyhedronGraph = NormalizeGraph(m_CurrentPolyhedronGraph);
            m_CurrentPolyhedronInEq =
                new Polyhedron3DFromGraphFactory(m_ApproxComparer, new LESKramer3Solver()).CreatePolyhedron(m_CurrentPolyhedronGraph);

            //Matrix transformMatrix = new InverseMatrixBuilder().InverseMatrix(m_FundCauchyMatrix.GetFundCauchyMatrix());
            //m_CurrentPolyhedronInNorm = TranformPolyhedron2NormalCoords(m_CurrentPolyhedronInEq, transformMatrix);
        }

        private Boolean CheckAndScaleIfNeed()
        {
            Polyhedron3DGraph_Utils graphUtils = new Polyhedron3DGraph_Utils();
            IDictionary<String, Pair<Vector3D, Double>> thicknessPairs =
                graphUtils.CalcGraphExtremeThickness(m_CurrentPolyhedronGraph);
            Pair<Vector3D, Double> maxThickness = thicknessPairs[Polyhedron3DGraph_Utils.MaxThicknessKey];
            Pair<Vector3D, Double> minThickness = thicknessPairs[Polyhedron3DGraph_Utils.MinThicknessKey];
            if(graphUtils.NeedScaling(minThickness, maxThickness, maxminThresholdValue))
            {
                Matrix prevDirectScaling = directScaling;
                Matrix prevReverseScaling = reverseScaling;
                Pair<Vector3D, Double> scalingParams = graphUtils.CalcScaling(minThickness, maxThickness,
                                                                              maxminThresholdValue);
                Matrix currentDirectScaling = ScalingTransformation3D.GetTransformationMatrix(scalingParams.Item1,
                                                                                              scalingParams.Item2);
                Matrix currentReverseScaling = ScalingTransformation3D.GetTransformationMatrix(scalingParams.Item1,
                                                                                               1 / scalingParams.Item2);
                Polyhedron3DGraph_ScaleTransformer transformer =
                    new Polyhedron3DGraph_ScaleTransformer();
                m_CurrentPolyhedronGraph = transformer.Process(m_CurrentPolyhedronGraph,
                                                               currentDirectScaling,
                                                               currentReverseScaling);
                directScaling = currentDirectScaling * prevDirectScaling;
                reverseScaling = prevReverseScaling * currentReverseScaling;
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

        private static Polyhedron3D TranformPolyhedron2NormalCoords(Polyhedron3D polyhedronInEqCoords,
                                                                    Matrix transformMatrix)
        {
            List<PolyhedronSide3D> newSideList = new List<PolyhedronSide3D>();
            List<PolyhedronVertex3D> newVertexList = new List<PolyhedronVertex3D>();
            IList<PolyhedronSide3D> oldSideList = polyhedronInEqCoords.SideList;
            IList<PolyhedronVertex3D> oldVertexList = polyhedronInEqCoords.VertexList;
            // формируем список новых граней
            for(Int32 sideIndex = 0; sideIndex < oldSideList.Count; ++sideIndex)
            {
                Vector3D normalInEqCoords = oldSideList[sideIndex].SideNormal;
                Vector3D normalInNormalCoords = TrasformVector2NormalCoords(normalInEqCoords, transformMatrix);
                normalInNormalCoords = Vector3DUtils.NormalizeVector(normalInNormalCoords);
                newSideList.Add(new PolyhedronSide3D(sideIndex, normalInNormalCoords));
            }
            // формируем список новых вершин
            for(Int32 vertexIndex = 0; vertexIndex < oldVertexList.Count; ++vertexIndex)
            {
                Point3D vertexInEqCoords = new Point3D(oldVertexList[vertexIndex].XCoord,
                                                       oldVertexList[vertexIndex].YCoord,
                                                       oldVertexList[vertexIndex].ZCoord);
                newVertexList.Add(new PolyhedronVertex3D(TrasformPoint2NormalCoords(vertexInEqCoords, transformMatrix),
                                                         vertexIndex));
            }
            // формируем список вершин для каждой новой грани
            for(Int32 sideIndex = 0; sideIndex < newSideList.Count; ++sideIndex)
            {
                PolyhedronSide3D newCurrentSide = newSideList[sideIndex];
                PolyhedronSide3D oldCurrentSide = oldSideList[sideIndex];
                for(Int32 vertexIndex = 0; vertexIndex < oldCurrentSide.VertexList.Count; ++vertexIndex)
                    newCurrentSide.VertexList.Add(newVertexList[oldCurrentSide.VertexList[vertexIndex].ID]);
            }
            /*// формируем список граней для каждой новой вершины
            for(Int32 vertexIndex=0;vertexIndex<newVertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D newCurrentVertex = newVertexList[vertexIndex];
                PolyhedronVertex3D oldCurrentVertex = oldVertexList[vertexIndex];
                for(Int32 sideIndex=0;sideIndex<oldCurrentVertex.SideList.Count;++sideIndex)
                {
                    newCurrentVertex.SideList.Add(newSideList[oldCurrentVertex.SideList[sideIndex].ID]);
                }
            }*/
            Polyhedron3D polyhedronInNormCoords = new Polyhedron3D(newSideList, newVertexList);
            return polyhedronInNormCoords;
        }

        private static Point3D TrasformPoint2NormalCoords(Point3D pointInEqCoords, Matrix transformMatrix)
        {
            Matrix sourcePointCol = new Matrix(3, 1);
            sourcePointCol[1, 1] = pointInEqCoords.XCoord;
            sourcePointCol[2, 1] = pointInEqCoords.YCoord;
            sourcePointCol[3, 1] = pointInEqCoords.ZCoord;
            Matrix destPointCol = transformMatrix * sourcePointCol;
            return new Point3D(destPointCol[1, 1], destPointCol[2, 1], destPointCol[3, 1]);
        }

        private static Vector3D TrasformVector2NormalCoords(Vector3D vectorInEqCoords, Matrix transformMatrix)
        {
            Matrix sourceVectorCol = new Matrix(3, 1);
            sourceVectorCol[1, 1] = vectorInEqCoords.XCoord;
            sourceVectorCol[2, 1] = vectorInEqCoords.YCoord;
            sourceVectorCol[3, 1] = vectorInEqCoords.ZCoord;
            Matrix destVectorCol = transformMatrix * sourceVectorCol;
            return new Vector3D(destVectorCol[1, 1], destVectorCol[2, 1], destVectorCol[3, 1]);
        }

        private readonly ApproxComp m_ApproxComparer;

        private Int32 m_GenerationID;
        private readonly IBridgeGraphCorrector m_Corrector;
        private readonly FictiousNodeRemover m_FictiousNodeRemover;

        private readonly Double m_DeltaT;

        private readonly FirstGamer m_FirstGamer;
        private readonly SecondGamer m_SecondGamer;

        private readonly Matrix m_MatrixA;
        private readonly Matrix m_MatrixB;
        private readonly Matrix m_MatrixC;
        private readonly FundCauchyMatrix m_FundCauchyMatrix;

        private Polyhedron3D m_CurrentPolyhedronInEq;
        //private Polyhedron3D m_CurrentPolyhedronInNorm;

        private Polyhedron3DGraph m_CurrentPolyhedronGraph;
        private Matrix directScaling = Matrix.IdentityMatrix(3);
        private Matrix reverseScaling = Matrix.IdentityMatrix(3);
        // пороговое значение maxThickness/minThickness, после которого начинаем масштабирование
        private readonly Double maxminThresholdValue;

        private Double m_InverseTime;
    }
}