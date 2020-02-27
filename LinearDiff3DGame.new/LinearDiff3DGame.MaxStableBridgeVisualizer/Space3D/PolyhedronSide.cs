using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.MaxStableBridgeVisualizer.Space3D
{
	// представление грани многогранника
	internal class PolyhedronSide
	{
		internal PolyhedronSide(IPolyhedronSide3D side)
		{
			// init normal
			normal = new Object3D(side.SideNormal.X,
			                      side.SideNormal.Y,
			                      side.SideNormal.Z);
			// init vertex list
			IList<Object3D> vertexList = new List<Object3D>(side.VertexList.Count);
			for (Int32 vertexIndex = 0; vertexIndex < side.VertexList.Count; ++vertexIndex)
			{
				IPolyhedronVertex3D currentVertex = side.VertexList[vertexIndex];
				vertexList.Add(new Object3D(currentVertex.XCoord,
				                            currentVertex.YCoord,
				                            currentVertex.ZCoord));
			}
			this.vertexList = new ReadOnlyCollection<Object3D>(vertexList);
		}

		public Object3D Normal
		{
			get { return normal; }
		}

		public IList<Object3D> VertexList
		{
			get { return vertexList; }
		}

		private readonly Object3D normal;
		private readonly ReadOnlyCollection<Object3D> vertexList;
	}
}