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
using LinearDiff3DGame.MaxStableBridge.Cleaner;
using LinearDiff3DGame.MaxStableBridge.Corrector;
using LinearDiff3DGame.MaxStableBridge.Gamers;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Old
{
    public class MaxStableBridgeBuilder2
    {
        public MaxStableBridgeBuilder2()
        {
            const double epsilon = 1e-9;
            m_ApproxComparer = new ApproxComp(epsilon);

            m_DeltaT = 0.1;

            matrixA = new Matrix(4, 4);
            matrixA[1, 1] = 0;
            matrixA[1, 2] = 1;
            matrixA[1, 3] = 0;
            matrixA[1, 4] = 0;
            matrixA[2, 1] = 0;
            matrixA[2, 2] = -0.032;
            matrixA[2, 3] = 0;
            matrixA[2, 4] = -0.135;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;
            matrixA[3, 4] = 1;
            matrixA[4, 1] = 0;
            matrixA[4, 2] = 0.27;
            matrixA[4, 3] = 0;
            matrixA[4, 4] = -0.014;

            m_MatrixB1 = new Matrix(4, 1);
            m_MatrixB1[1, 1] = 0;
            m_MatrixB1[2, 1] = 2.577;
            m_MatrixB1[3, 1] = 0;
            m_MatrixB1[4, 1] = 0.288;

            m_MatrixC1 = new Matrix(4, 1);
            m_MatrixC1[1, 1] = 0;
            m_MatrixC1[2, 1] = 0.032;
            m_MatrixC1[3, 1] = 0;
            m_MatrixC1[4, 1] = -0.27;

            Double mp1Max = 0.7;
            Double mp1Min = -0.7;
            m_FirstGamer1 = new FirstGamer(m_ApproxComparer, m_MatrixB1, m_DeltaT, mp1Max, mp1Min);

            Double mq1Max = 22;
            Double mq1Min = -22;
            m_SecondGamer1 = new SecondGamer(m_ApproxComparer, m_MatrixC1, m_DeltaT, mq1Max, mq1Min);

            m_MatrixB2 = new Matrix(4, 1);
            m_MatrixB2[1, 1] = 0;
            m_MatrixB2[2, 1] = -2.886;
            m_MatrixB2[3, 1] = 0;
            m_MatrixB2[4, 1] = 40.234;

            m_MatrixC2 = new Matrix(4, 1);
            m_MatrixC2[1, 1] = 0;
            m_MatrixC2[2, 1] = 0.135;
            m_MatrixC2[3, 1] = 0;
            m_MatrixC2[4, 1] = 0.014;

            Double mp2Max = 0.17;
            Double mp2Min = -0.17;
            m_FirstGamer2 = new FirstGamer(m_ApproxComparer, m_MatrixB2, m_DeltaT, mp2Max, mp2Min);

            Double mq2Max = 18;
            Double mq2Min = -18;
            m_SecondGamer2 = new SecondGamer(m_ApproxComparer, m_MatrixC2, m_DeltaT, mq2Max, mq2Min);

            m_Cleaner = new BridgeGraphCleaner(m_ApproxComparer);
            m_GenerationID = 0;

            //m_Corrector = new BridgeGraphCorrector_new_2(m_ApproxComparer);
            m_Corrector = new BridgeGraphCorrector_old(m_ApproxComparer);

            Int32 vertexCount = 8;
            Point3D[] terminalSetVertexes = new Point3D[vertexCount];
            terminalSetVertexes[0] = new Point3D(50, 2, 0.5);
            terminalSetVertexes[1] = new Point3D(-50, 2, 0.5);
            terminalSetVertexes[2] = new Point3D(50, -2, 0.5);
            terminalSetVertexes[3] = new Point3D(-50, -2, 0.5);
            terminalSetVertexes[4] = new Point3D(50, 2, -0.5);
            terminalSetVertexes[5] = new Point3D(-50, 2, -0.5);
            terminalSetVertexes[6] = new Point3D(50, -2, -0.5);
            terminalSetVertexes[7] = new Point3D(-50, -2, -0.5);

            m_CurrentPolyhedron =
                new Polyhedron3DFromPointsFactory(m_ApproxComparer).CreatePolyhedron(terminalSetVertexes);
            m_CurrentPolyhedronGraph = new Polyhedron3DGraphFactory().CreatePolyhedronGraph(m_CurrentPolyhedron);
            m_CurrentPolyhedronGraph = new Polyhedron3DGraphSimpleTriangulator().Triangulate(m_CurrentPolyhedronGraph);

            m_InverseTime = 0;
            m_FundCauchyMatrix = new FundCauchyMatrix(matrixA, new[] {1, 3, 4}, m_DeltaT / 10);

            maxminThresholdValue = 1.1;
        }

        public void NextIteration()
        {
            CheckAndScaleIfNeed();

            m_InverseTime += m_DeltaT;
            m_FundCauchyMatrix.Calculate(m_InverseTime);

            ++m_GenerationID;
            m_CurrentPolyhedronGraph = m_FirstGamer1.Action(m_CurrentPolyhedronGraph,
                                                            m_FundCauchyMatrix.Current,
                                                            m_GenerationID,
                                                            directScaling);
            m_CurrentPolyhedronGraph = m_FirstGamer2.Action(m_CurrentPolyhedronGraph,
                                                            m_FundCauchyMatrix.Current,
                                                            m_GenerationID,
                                                            directScaling);

            //m_CurrentPolyhedronGraph = m_Cleaner.Action(m_MinAngle, m_CurrentPolyhedronGraph, m_Corrector, startIndex);
            //m_CurrentPolyhedronGraph = m_Cleaner.Action(m_MinAngle, m_CurrentPolyhedronGraph, m_Corrector, connSet, startIndex);
            //m_CurrentPolyhedronGraph = m_Corrector.CheckAndCorrectBridgeGraph(connSet, m_CurrentPolyhedronGraph);

            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
            m_CurrentPolyhedronGraph = m_SecondGamer1.Action(m_CurrentPolyhedronGraph,
                                                             m_FundCauchyMatrix.Current,
                                                             connSet,
                                                             directScaling);
            m_CurrentPolyhedronGraph = m_Corrector.CheckAndCorrectBridgeGraph(connSet, m_CurrentPolyhedronGraph);

            m_CurrentPolyhedronGraph = m_SecondGamer2.Action(m_CurrentPolyhedronGraph,
                                                             m_FundCauchyMatrix.Current,
                                                             connSet,
                                                             directScaling);
            m_CurrentPolyhedronGraph = m_Corrector.CheckAndCorrectBridgeGraph(connSet, m_CurrentPolyhedronGraph);

            m_CurrentPolyhedronGraph = NormalizeGraph(m_CurrentPolyhedronGraph);
            m_CurrentPolyhedron =
                new Polyhedron3DFromGraphFactory(m_ApproxComparer, new LESKramer3Solver()).CreatePolyhedron(m_CurrentPolyhedronGraph);
        }

        public Polyhedron3D CurrentPolyhedron { get { return m_CurrentPolyhedron; } }

        public Double CurrentInverseTime { get { return m_InverseTime; } }

        private Boolean CheckAndScaleIfNeed()
        {
            Polyhedron3DGraph_Utils graphUtils = new Polyhedron3DGraph_Utils();
            IDictionary<String, Pair<Vector3D, Double>> thicknessPairs =
                graphUtils.CalcGraphExtremeThickness(m_CurrentPolyhedronGraph);
            Pair<Vector3D, Double> maxThickness = thicknessPairs[Polyhedron3DGraph_Utils.MaxThicknessKey];
            Pair<Vector3D, Double> minThickness = thicknessPairs[Polyhedron3DGraph_Utils.MinThicknessKey];
            if (graphUtils.NeedScaling(minThickness, maxThickness, maxminThresholdValue))
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

        /// <summary>
        /// 
        /// </summary>
        private readonly ApproxComp m_ApproxComparer;

        /// <summary>
        /// 
        /// </summary>
        private readonly Matrix matrixA;

        /// <summary>
        /// 
        /// </summary>
        private readonly Double m_DeltaT;

        private Polyhedron3D m_CurrentPolyhedron;
        private Polyhedron3DGraph m_CurrentPolyhedronGraph;
        private Double m_InverseTime;
        private readonly FundCauchyMatrix m_FundCauchyMatrix;

        private readonly Matrix m_MatrixB1;
        private readonly Matrix m_MatrixC1;
        private readonly FirstGamer m_FirstGamer1;
        private readonly SecondGamer m_SecondGamer1;

        private readonly Matrix m_MatrixB2;
        private readonly Matrix m_MatrixC2;
        private readonly FirstGamer m_FirstGamer2;
        private readonly SecondGamer m_SecondGamer2;

        private Matrix directScaling = Matrix.IdentityMatrix(3);
        private Matrix reverseScaling = Matrix.IdentityMatrix(3);
        // пороговое значение maxThickness/minThickness, после которого начинаем масштабирование
        private readonly Double maxminThresholdValue;

        private BridgeGraphCleaner m_Cleaner;
        private Int32 m_GenerationID;

        private readonly IBridgeGraphCorrector m_Corrector;
        //private BridgeGraphCorrector_new_2 m_Corrector;
        //private BridgeGraphCorrector_old m_Corrector;
    }
}