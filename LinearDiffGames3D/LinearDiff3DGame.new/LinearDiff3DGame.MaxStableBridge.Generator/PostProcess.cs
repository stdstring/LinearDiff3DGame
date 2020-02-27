using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.MaxStableBridge.Generator
{
	internal class PostProcess
	{
		public PostProcess(ApproxComp approxComp)
		{
			this.approxComp = approxComp;
		}

		public IPolyhedron3D Process(IPolyhedron3D source)
		{
			SideVertexData svData = GetSuitableSideAndVertex(source);
			IPolyhedronSide3D backSide = CreateBackSide(svData);
			svData.SideList.Add(backSide);
			return new Polyhedron3D(svData.SideList, svData.VertexList);
		}

		private SideVertexData GetSuitableSideAndVertex(IPolyhedron3D source)
		{
			IList<IPolyhedronSide3D> sideList = new List<IPolyhedronSide3D>();
			IList<IPolyhedronVertex3D> vertexList = new List<IPolyhedronVertex3D>();
			IDictionary<Point3D, IPolyhedronVertex3D> vertexDictionary = new Dictionary<Point3D, IPolyhedronVertex3D>();
			IList<IPolyhedronVertex3D> vertexList0 = new List<IPolyhedronVertex3D>();
			Vector3D suitableSemispace = new Vector3D(0, 0, -1);
			foreach (IPolyhedronSide3D side in source.SideList)
			{
				if (approxComp.LE(side.SideNormal*suitableSemispace, 0)) continue;
				IList<IPolyhedronVertex3D> sideVertexList = new List<IPolyhedronVertex3D>();
				foreach (IPolyhedronVertex3D vertex in side.VertexList)
				{
					IPolyhedronSide3D otherSemispaceSide = source
						.GetSides4Vertex(vertex)
						.FirstOrDefault(s => approxComp.LE(s.SideNormal*suitableSemispace, 0));
					Point3D newPoint = otherSemispaceSide == null
					                   	? new Point3D(vertex.XCoord, vertex.YCoord, vertex.ZCoord)
					                   	: new Point3D(vertex.XCoord, vertex.YCoord, 0);
					IPolyhedronVertex3D newVertex;
					if (!vertexDictionary.TryGetValue(newPoint, out newVertex))
					{
						newVertex = new PolyhedronVertex3D(newPoint, vertexList.Count);
						vertexList.Add(newVertex);
						vertexDictionary.Add(newPoint, newVertex);
					}
					if (otherSemispaceSide != null && !vertexList0.Contains(newVertex))
						vertexList0.Add(newVertex);
					sideVertexList.Add(newVertex);
				}
				Vector3D newNormal = side.SideNormal;
				IPolyhedronSide3D newSide = new PolyhedronSide3D(sideVertexList, sideList.Count, newNormal);
				sideList.Add(newSide);
			}
			return new SideVertexData(sideList, vertexList, vertexList0);
		}

		private static IPolyhedronSide3D CreateBackSide(SideVertexData svData)
		{
			IList<IPolyhedronVertex3D> orderedVertexList = svData.VertexList0
				.OrderBy(vertex => GetBackSideVertexOrderKey(vertex))
				.ToList();
			return new PolyhedronSide3D(orderedVertexList, svData.SideList.Count, new Vector3D(0, 0, 1));
		}

		private static Double GetBackSideVertexOrderKey(IPolyhedronVertex3D vertex)
		{
			Double x = vertex.XCoord;
			Double y = vertex.YCoord;
			Double length = Math.Sqrt(x*x + y*y);
			if (x >= 0 && y >= 0) return y/length;
			if (x < 0 && y >= 0) return 2 - x/length;
			if (x <= 0 && y < 0) return 4 - y/length;
			// x > 0 && y<0
			return 6 + x/length;
		}

		private readonly ApproxComp approxComp;

		private struct SideVertexData
		{
			public SideVertexData(IList<IPolyhedronSide3D> sideList,
			                      IList<IPolyhedronVertex3D> vertexList,
			                      IList<IPolyhedronVertex3D> vertexList0)
				: this()
			{
				SideList = sideList;
				VertexList = vertexList;
				VertexList0 = vertexList0;
			}

			public IList<IPolyhedronSide3D> SideList { get; private set; }
			public IList<IPolyhedronVertex3D> VertexList { get; private set; }
			public IList<IPolyhedronVertex3D> VertexList0 { get; private set; }
		}
	}
}