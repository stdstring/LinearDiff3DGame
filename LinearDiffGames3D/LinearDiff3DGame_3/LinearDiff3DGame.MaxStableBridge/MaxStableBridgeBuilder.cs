using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// 
    /// </summary>
    public class MaxStableBridgeBuilder
    {
        public MaxStableBridgeBuilder(Dictionary<String, Object> dataContainer)
        {
            Double epsilon = (Double)dataContainer["Epsilon"];
            m_ApproxComparer = new ApproxComp(epsilon);

            m_MatrixA = dataContainer["MatrixA"] as Matrix;            

            m_DeltaT = (Double)dataContainer["DeltaT"];

            m_MatrixB = dataContainer["MatrixB"] as Matrix;
            m_MatrixC = dataContainer["MatrixC"] as Matrix;

            Double mpMax = (Double)dataContainer["MpMax"];
            Double mpMin = (Double)dataContainer["MpMin"];
            m_FirstGamer = new FirstGamer(m_ApproxComparer, m_MatrixB, m_DeltaT, mpMax, mpMin);

            Double mqMax = (Double)dataContainer["MqMax"];
            Double mqMin = (Double)dataContainer["MqMin"];
            m_SecondGamer = new SecondGamer(m_ApproxComparer, m_MatrixC, m_DeltaT, mqMax, mqMin);

            m_Corrector = new BridgeGraphCorrector(m_ApproxComparer);

            Point3D[] terminalSetVertexes = dataContainer["TerminalSet"] as Point3D[];
            m_CurrentPolyhedron = new Polyhedron3DFromPointsFactory(m_ApproxComparer).CreatePolyhedron(terminalSetVertexes);
            m_CurrentPolyhedronGraph = new Polyhedron3DGraphFactory().CreatePolyhedronGraph(m_CurrentPolyhedron);
            m_CurrentPolyhedronGraph = new Polyhedron3DGraphTriangulator().Triangulate(m_CurrentPolyhedronGraph);

            m_InverseTime = 0;
            m_FundCauchyMatrix = new FundCauchyMatrix(m_MatrixA, new Int32[] { 1, 2, 3 }, m_DeltaT / 10);
        }

        public void NextIteration()
        {
            m_InverseTime += m_DeltaT;
            m_FundCauchyMatrix.CalcFundCauchyMatrix(m_InverseTime);

            m_CurrentPolyhedronGraph = m_FirstGamer.Action(m_CurrentPolyhedronGraph, m_FundCauchyMatrix.GetFundCauchyMatrix());
            SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
            m_CurrentPolyhedronGraph = m_SecondGamer.Action(m_CurrentPolyhedronGraph, m_FundCauchyMatrix.GetFundCauchyMatrix(), connSet);
            m_CurrentPolyhedronGraph = m_Corrector.CheckAndCorrectBridgeGraph(connSet, m_CurrentPolyhedronGraph);

            m_CurrentPolyhedronGraph = NormalizeGraph(m_CurrentPolyhedronGraph);
            m_CurrentPolyhedron = new Polyhedron3DFromGraphFactory(m_ApproxComparer).CreatePolyhedron(m_CurrentPolyhedronGraph);
        }

        public Polyhedron3D CurrentPolyhedron
        {
            get
            {
                return m_CurrentPolyhedron;
            }
        }

        public Double CurrentInverseTime
        {
            get
            {
                return m_InverseTime;
            }
        }

        private Polyhedron3DGraph NormalizeGraph(Polyhedron3DGraph graph)
        {
            for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                graph.NodeList[nodeIndex].ID = nodeIndex;
            }

            return graph;
        }

        /// <summary>
        /// 
        /// </summary>
        private ApproxComp m_ApproxComparer;

        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixA;

        /// <summary>
        /// 
        /// </summary>
        private Double m_DeltaT;

        private Polyhedron3D m_CurrentPolyhedron;
        private Polyhedron3DGraph m_CurrentPolyhedronGraph;
        private Double m_InverseTime;
        private FundCauchyMatrix m_FundCauchyMatrix;

        private Matrix m_MatrixB;
        private Matrix m_MatrixC;

        private FirstGamer m_FirstGamer;
        private SecondGamer m_SecondGamer;
        private BridgeGraphCorrector m_Corrector;
    }
}
