using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.AdvMath.Matrix;
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
	public class MaxStableBridgeBuilderWithoutScaling
	{
		public MaxStableBridgeBuilderWithoutScaling()
		{
			IMaxStableBridgeBuilderInitializer initializer = MaxStableBridgeBuilderInitializer.CreateMaterialPointData();
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

			inverseTime = 0;
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

		public Double CurrentInverseTime
		{
			get { return inverseTime; }
		}

		public Double DeltaT
		{
			get { return deltaT; }
		}

		public void NextIteration()
		{
			inverseTime += deltaT;
			fundCauchyMatrix.Calculate(inverseTime);

			++generationID;
			currentPolyhedronGraph = firstGamer.Action(currentPolyhedronGraph,
			                                           fundCauchyMatrix.Current,
			                                           generationID,
			                                           Matrix.IdentityMatrix(3));

			SuspiciousConnectionSet connSet = new SuspiciousConnectionSet();
			currentPolyhedronGraph = secondGamer.Action(currentPolyhedronGraph,
			                                            fundCauchyMatrix.Current,
			                                            connSet,
			                                            Matrix.IdentityMatrix(3));
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
			// формируем список новых граней
			for (Int32 sideIndex = 0; sideIndex < oldSideList.Count; ++sideIndex)
			{
				Vector3D normalInEqCoords = oldSideList[sideIndex].SideNormal;
				Vector3D normalInNormalCoords = TrasformVector2NormalCoords(normalInEqCoords, transformMatrix);
				normalInNormalCoords = Vector3DUtils.NormalizeVector(normalInNormalCoords);
				newSideList.Add(new PolyhedronSide3D(sideIndex, normalInNormalCoords));
			}
			// формируем список новых вершин
			for (Int32 vertexIndex = 0; vertexIndex < oldVertexList.Count; ++vertexIndex)
			{
				Point3D vertexInEqCoords = new Point3D(oldVertexList[vertexIndex].XCoord,
				                                       oldVertexList[vertexIndex].YCoord,
				                                       oldVertexList[vertexIndex].ZCoord);
				newVertexList.Add(new PolyhedronVertex3D(TrasformPoint2NormalCoords(vertexInEqCoords, transformMatrix),
				                                         vertexIndex));
			}
			// формируем список вершин для каждой новой грани
			for (Int32 sideIndex = 0; sideIndex < newSideList.Count; ++sideIndex)
			{
				IPolyhedronSide3D newCurrentSide = newSideList[sideIndex];
				IPolyhedronSide3D oldCurrentSide = oldSideList[sideIndex];
				for (Int32 vertexIndex = 0; vertexIndex < oldCurrentSide.VertexList.Count; ++vertexIndex)
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

		private Double inverseTime;
	}
}