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
			matrixA = (Matrix) initializer.GetDataByKey("MatrixA");
			matrixB = (Matrix) initializer.GetDataByKey("MatrixB");
			matrixC = (Matrix) initializer.GetDataByKey("MatrixC");
			deltaT = (Double) initializer.GetDataByKey("DeltaT");
			Double mpMax = (Double) initializer.GetDataByKey("MpMax");
			Double mpMin = (Double) initializer.GetDataByKey("MpMin");
			Double mqMax = (Double) initializer.GetDataByKey("MqMax");
			Double mqMin = (Double) initializer.GetDataByKey("MqMin");
			Point3D[] terminalSetVertexes = (Point3D[]) initializer.GetDataByKey("TerminalSet");
			Int32[] y1y2y3Indexes = (Int32[]) initializer.GetDataByKey("Y1Y2Y3Indexes");
			maxminThresholdValue = (Double) initializer.GetDataByKey("ScalingMaxMinThresholdValue");

			const Double epsilon = 1e-9;
			approxComparer = new ApproxComp(epsilon);

			corrector = new BridgeGraphCorrector_old(approxComparer);

			generationID = 0;
			fictiousNodeRemover = new FictiousNodeRemover();

			firstGamer = new FirstGamer(approxComparer, matrixB, deltaT, mpMax, mpMin);
			secondGamer = new SecondGamer(approxComparer, matrixC, deltaT, mqMax, mqMin);

			currentPolyhedronInEq =
				new Polyhedron3DFromPointsFactory(approxComparer).CreatePolyhedron(terminalSetVertexes);
			currentPolyhedronGraph = new Polyhedron3DGraphFactory().CreatePolyhedronGraph(currentPolyhedronInEq);
			currentPolyhedronGraph = new Polyhedron3DGraphSimpleTriangulator().Triangulate(currentPolyhedronGraph);

			m_InverseTime = 0;
			fundCauchyMatrix = new FundCauchyMatrix(matrixA, y1y2y3Indexes, deltaT/10);

			//Matrix transformMatrix = new InverseMatrixBuilder().InverseMatrix(m_FundCauchyMatrix.GetFundCauchyMatrix());
			//m_CurrentPolyhedronInNorm = TranformPolyhedron2NormalCoords(m_CurrentPolyhedronInEq, transformMatrix);
		}

		public IPolyhedron3D CurrentPolyhedron
		{
			get
			{
				return currentPolyhedronInEq;
				//return m_CurrentPolyhedronInNorm;
			}
		}

		public Matrix DirectTransformation
		{
			get { return directScaling; }
		}

		public Matrix ReverseTransformation
		{
			get { return reverseScaling; }
		}

		public Double CurrentInverseTime
		{
			get { return m_InverseTime; }
		}

		public Double DeltaT
		{
			get { return deltaT; }
		}

		public void NextIteration()
		{
			CheckAndScaleIfNeed();

			m_InverseTime += deltaT;
			fundCauchyMatrix.Calculate(m_InverseTime);

			++generationID;
			currentPolyhedronGraph = firstGamer.Action(currentPolyhedronGraph,
			                                           fundCauchyMatrix.Current,
			                                           generationID,
			                                           directScaling);

			SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
			currentPolyhedronGraph = secondGamer.Action(currentPolyhedronGraph,
			                                            fundCauchyMatrix.Current,
			                                            connSet,
			                                            directScaling);
			currentPolyhedronGraph = corrector.CheckAndCorrectBridgeGraph(connSet, currentPolyhedronGraph);

			//currentPolyhedronGraph = NormalizeGraph(currentPolyhedronGraph);
			currentPolyhedronInEq =
				new Polyhedron3DFromGraphFactory(approxComparer, new LESKramer3Solver()).CreatePolyhedron(currentPolyhedronGraph);

			currentPolyhedronGraph = fictiousNodeRemover.Action(currentPolyhedronInEq,
			                                                    currentPolyhedronGraph,
			                                                    corrector);

			//currentPolyhedronGraph = NormalizeGraph(currentPolyhedronGraph);
			currentPolyhedronInEq =
				new Polyhedron3DFromGraphFactory(approxComparer, new LESKramer3Solver()).CreatePolyhedron(currentPolyhedronGraph);

			//Matrix transformMatrix = new InverseMatrixBuilder().InverseMatrix(m_FundCauchyMatrix.GetFundCauchyMatrix());
			//m_CurrentPolyhedronInNorm = TranformPolyhedron2NormalCoords(m_CurrentPolyhedronInEq, transformMatrix);
		}

		private Boolean CheckAndScaleIfNeed()
		{
			Polyhedron3DGraph_Utils graphUtils = new Polyhedron3DGraph_Utils();
			IDictionary<String, Pair<Vector3D, Double>> thicknessPairs =
				graphUtils.CalcGraphExtremeThickness(currentPolyhedronGraph);
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

        //private static Polyhedron3DGraph NormalizeGraph(Polyhedron3DGraph graph)
        //{
        //    for (Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
        //        graph.NodeList[nodeIndex].ID = nodeIndex;
        //    return graph;
        //}

		private static IPolyhedron3D TranformPolyhedron2NormalCoords(IPolyhedron3D polyhedronInEqCoords,
		                                                             Matrix transformMatrix)
		{
			List<IPolyhedronSide3D> newSideList = new List<IPolyhedronSide3D>();
			List<IPolyhedronVertex3D> newVertexList = new List<IPolyhedronVertex3D>();
			IList<IPolyhedronSide3D> oldSideList = polyhedronInEqCoords.SideList;
			IList<IPolyhedronVertex3D> oldVertexList = polyhedronInEqCoords.VertexList;
			// ��������� ������ ����� ������
			for (Int32 sideIndex = 0; sideIndex < oldSideList.Count; ++sideIndex)
			{
				Vector3D normalInEqCoords = oldSideList[sideIndex].SideNormal;
				Vector3D normalInNormalCoords = TrasformVector2NormalCoords(normalInEqCoords, transformMatrix);
				normalInNormalCoords = Vector3DUtils.NormalizeVector(normalInNormalCoords);
				newSideList.Add(new PolyhedronSide3D(sideIndex, normalInNormalCoords));
			}
			// ��������� ������ ����� ������
			for (Int32 vertexIndex = 0; vertexIndex < oldVertexList.Count; ++vertexIndex)
			{
				Point3D vertexInEqCoords = new Point3D(oldVertexList[vertexIndex].XCoord,
				                                       oldVertexList[vertexIndex].YCoord,
				                                       oldVertexList[vertexIndex].ZCoord);
				newVertexList.Add(new PolyhedronVertex3D(TrasformPoint2NormalCoords(vertexInEqCoords, transformMatrix),
				                                         vertexIndex));
			}
			// ��������� ������ ������ ��� ������ ����� �����
			for (Int32 sideIndex = 0; sideIndex < newSideList.Count; ++sideIndex)
			{
				IPolyhedronSide3D newCurrentSide = newSideList[sideIndex];
				IPolyhedronSide3D oldCurrentSide = oldSideList[sideIndex];
				for (Int32 vertexIndex = 0; vertexIndex < oldCurrentSide.VertexList.Count; ++vertexIndex)
					newCurrentSide.VertexList.Add(newVertexList[oldCurrentSide.VertexList[vertexIndex].ID]);
			}
			/*// ��������� ������ ������ ��� ������ ����� �������
            for(Int32 vertexIndex=0;vertexIndex<newVertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D newCurrentVertex = newVertexList[vertexIndex];
                PolyhedronVertex3D oldCurrentVertex = oldVertexList[vertexIndex];
                for(Int32 sideIndex=0;sideIndex<oldCurrentVertex.SideList.Count;++sideIndex)
                {
                    newCurrentVertex.SideList.Add(newSideList[oldCurrentVertex.SideList[sideIndex].ID]);
                }
            }*/
			IPolyhedron3D polyhedronInNormCoords = new Polyhedron3D(newSideList, newVertexList);
			return polyhedronInNormCoords;
		}

		private static Point3D TrasformPoint2NormalCoords(Point3D pointInEqCoords, Matrix transformMatrix)
		{
			Matrix sourcePointCol = new Matrix(3, 1);
			sourcePointCol[1, 1] = pointInEqCoords.X;
			sourcePointCol[2, 1] = pointInEqCoords.Y;
			sourcePointCol[3, 1] = pointInEqCoords.Z;
			Matrix destPointCol = transformMatrix*sourcePointCol;
			return new Point3D(destPointCol[1, 1], destPointCol[2, 1], destPointCol[3, 1]);
		}

		private static Vector3D TrasformVector2NormalCoords(Vector3D vectorInEqCoords, Matrix transformMatrix)
		{
			Matrix sourceVectorCol = new Matrix(3, 1);
			sourceVectorCol[1, 1] = vectorInEqCoords.X;
			sourceVectorCol[2, 1] = vectorInEqCoords.Y;
			sourceVectorCol[3, 1] = vectorInEqCoords.Z;
			Matrix destVectorCol = transformMatrix*sourceVectorCol;
			return new Vector3D(destVectorCol[1, 1], destVectorCol[2, 1], destVectorCol[3, 1]);
		}

		private readonly ApproxComp approxComparer;

		private Int32 generationID;
		private readonly IBridgeGraphCorrector corrector;
		private readonly FictiousNodeRemover fictiousNodeRemover;

		private readonly Double deltaT;

		private readonly FirstGamer firstGamer;
		private readonly SecondGamer secondGamer;

		private readonly Matrix matrixA;
		private readonly Matrix matrixB;
		private readonly Matrix matrixC;
		private readonly FundCauchyMatrix fundCauchyMatrix;

		private IPolyhedron3D currentPolyhedronInEq;
		//private Polyhedron3D m_CurrentPolyhedronInNorm;

		private IPolyhedron3DGraph currentPolyhedronGraph;
		private Matrix directScaling = Matrix.IdentityMatrix(3);
		private Matrix reverseScaling = Matrix.IdentityMatrix(3);
		// ��������� �������� maxThickness/minThickness, ����� �������� �������� ���������������
		private readonly Double maxminThresholdValue;

		private Double m_InverseTime;
	}
}