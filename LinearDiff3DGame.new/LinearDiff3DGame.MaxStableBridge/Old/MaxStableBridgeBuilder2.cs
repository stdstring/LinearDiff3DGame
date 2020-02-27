using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.AdvMath.Matrix;
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
            approxComparer = new ApproxComp(epsilon);

            deltaT = 0.1;

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

            matrixB1 = new Matrix(4, 1);
            matrixB1[1, 1] = 0;
            matrixB1[2, 1] = 2.577;
            matrixB1[3, 1] = 0;
            matrixB1[4, 1] = 0.288;

            matrixC1 = new Matrix(4, 1);
            matrixC1[1, 1] = 0;
            matrixC1[2, 1] = 0.032;
            matrixC1[3, 1] = 0;
            matrixC1[4, 1] = -0.27;

            const double mp1Max = 0.7;
            const double mp1Min = -0.7;
            firstGamer1 = new FirstGamer(approxComparer, matrixB1, deltaT, mp1Max, mp1Min);

            const double mq1Max = 22;
            const double mq1Min = -22;
            secondGamer1 = new SecondGamer(approxComparer, matrixC1, deltaT, mq1Max, mq1Min);

            matrixB2 = new Matrix(4, 1);
            matrixB2[1, 1] = 0;
            matrixB2[2, 1] = -2.886;
            matrixB2[3, 1] = 0;
            matrixB2[4, 1] = 40.234;

            matrixC2 = new Matrix(4, 1);
            matrixC2[1, 1] = 0;
            matrixC2[2, 1] = 0.135;
            matrixC2[3, 1] = 0;
            matrixC2[4, 1] = 0.014;

            const double mp2Max = 0.17;
            const double mp2Min = -0.17;
            firstGamer2 = new FirstGamer(approxComparer, matrixB2, deltaT, mp2Max, mp2Min);

            const double mq2Max = 18;
            const double mq2Min = -18;
            secondGamer2 = new SecondGamer(approxComparer, matrixC2, deltaT, mq2Max, mq2Min);

            cleaner = new BridgeGraphCleaner();
            generationID = 0;

            corrector = new BridgeGraphCorrector_old(approxComparer);

            const int vertexCount = 8;
            Point3D[] terminalSetVertexes = new Point3D[vertexCount];
            terminalSetVertexes[0] = new Point3D(50, 2, 0.5);
            terminalSetVertexes[1] = new Point3D(-50, 2, 0.5);
            terminalSetVertexes[2] = new Point3D(50, -2, 0.5);
            terminalSetVertexes[3] = new Point3D(-50, -2, 0.5);
            terminalSetVertexes[4] = new Point3D(50, 2, -0.5);
            terminalSetVertexes[5] = new Point3D(-50, 2, -0.5);
            terminalSetVertexes[6] = new Point3D(50, -2, -0.5);
            terminalSetVertexes[7] = new Point3D(-50, -2, -0.5);

            currentPolyhedron =
                new Polyhedron3DFromPointsFactory(approxComparer).CreatePolyhedron(terminalSetVertexes);
            currentPolyhedronGraph = new Polyhedron3DGraphFactory().CreatePolyhedronGraph(currentPolyhedron);
            currentPolyhedronGraph = new Polyhedron3DGraphSimpleTriangulator().Triangulate(currentPolyhedronGraph);

            inverseTime = 0;
            fundCauchyMatrix = new FundCauchyMatrix(matrixA, new[] {1, 3, 4}, deltaT/10);

            maxminThresholdValue = 1.1;
        }

        public void NextIteration()
        {
            CheckAndScaleIfNeed();

            inverseTime += deltaT;
            fundCauchyMatrix.Calculate(inverseTime);

            ++generationID;
            currentPolyhedronGraph = firstGamer1.Action(currentPolyhedronGraph,
                                                        fundCauchyMatrix.Current,
                                                        generationID,
                                                        directScaling);
            currentPolyhedronGraph = firstGamer2.Action(currentPolyhedronGraph,
                                                        fundCauchyMatrix.Current,
                                                        generationID,
                                                        directScaling);

            //m_CurrentPolyhedronGraph = m_Cleaner.Action(m_MinAngle, m_CurrentPolyhedronGraph, m_Corrector, startIndex);
            //m_CurrentPolyhedronGraph = m_Cleaner.Action(m_MinAngle, m_CurrentPolyhedronGraph, m_Corrector, connSet, startIndex);
            //m_CurrentPolyhedronGraph = m_Corrector.CheckAndCorrectBridgeGraph(connSet, m_CurrentPolyhedronGraph);

            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
            currentPolyhedronGraph = secondGamer1.Action(currentPolyhedronGraph,
                                                         fundCauchyMatrix.Current,
                                                         connSet,
                                                         directScaling);
            currentPolyhedronGraph = corrector.CheckAndCorrectBridgeGraph(connSet, currentPolyhedronGraph);

            currentPolyhedronGraph = secondGamer2.Action(currentPolyhedronGraph,
                                                         fundCauchyMatrix.Current,
                                                         connSet,
                                                         directScaling);
            currentPolyhedronGraph = corrector.CheckAndCorrectBridgeGraph(connSet, currentPolyhedronGraph);

            //currentPolyhedronGraph = NormalizeGraph(currentPolyhedronGraph);
            currentPolyhedron =
                new Polyhedron3DFromGraphFactory(approxComparer, new LESKramer3Solver()).CreatePolyhedron(
                    currentPolyhedronGraph);
        }

        public IPolyhedron3D CurrentPolyhedron
        {
            get { return currentPolyhedron; }
        }

        public Double CurrentInverseTime
        {
            get { return inverseTime; }
        }

        private Boolean CheckAndScaleIfNeed()
        {
            Polyhedron3DGraph_Utils graphUtils = new Polyhedron3DGraph_Utils();
            IDictionary<String, Pair<Vector3D, Double>> thicknessPairs =
                graphUtils.CalcGraphExtremeThickness(currentPolyhedronGraph);
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
                                                                                               1/scalingParams.Item2);
                Polyhedron3DGraph_ScaleTransformer transformer =
                    new Polyhedron3DGraph_ScaleTransformer();
                currentPolyhedronGraph = transformer.Process(currentPolyhedronGraph,
                                                             currentDirectScaling,
                                                             currentReverseScaling);
                directScaling = currentDirectScaling*prevDirectScaling;
                reverseScaling = prevReverseScaling*currentReverseScaling;
                return true;
            }
            return false;
        }

        private readonly ApproxComp approxComparer;
        private readonly Matrix matrixA;
        private readonly Double deltaT;
        private IPolyhedron3D currentPolyhedron;
        private IPolyhedron3DGraph currentPolyhedronGraph;
        private Double inverseTime;
        private readonly FundCauchyMatrix fundCauchyMatrix;
        private readonly Matrix matrixB1;
        private readonly Matrix matrixC1;
        private readonly FirstGamer firstGamer1;
        private readonly SecondGamer secondGamer1;
        private readonly Matrix matrixB2;
        private readonly Matrix matrixC2;
        private readonly FirstGamer firstGamer2;
        private readonly SecondGamer secondGamer2;
        private Matrix directScaling = Matrix.IdentityMatrix(3);
        private Matrix reverseScaling = Matrix.IdentityMatrix(3);
        // пороговое значение maxThickness/minThickness, после которого начинаем масштабирование
        private readonly Double maxminThresholdValue;
        private BridgeGraphCleaner cleaner;
        private Int32 generationID;
        private readonly IBridgeGraphCorrector corrector;
    }
}