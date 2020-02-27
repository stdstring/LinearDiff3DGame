using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
	public interface IPolyhedronSide3D
	{
		Int32 ID { get; }
		Vector3D SideNormal { get; }
		IList<IPolyhedronVertex3D> VertexList { get; }
	}

	public class PolyhedronSide3D : IPolyhedronSide3D
	{
		public PolyhedronSide3D(Int32 sideID, Vector3D sideNormal)
		{
			id = sideID;
			vertexList = new List<IPolyhedronVertex3D>();
			this.sideNormal = sideNormal;
		}

		public PolyhedronSide3D(IList<IPolyhedronVertex3D> vertexList, Int32 sideID, Vector3D sideNormal)
		{
			id = sideID;
			this.vertexList = new List<IPolyhedronVertex3D>();
			for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
				this.vertexList.Add(vertexList[vertexIndex]);
			this.sideNormal = sideNormal;
		}

		public Int32 ID
		{
			get { return id; }
		}

		public Vector3D SideNormal
		{
			get { return sideNormal; }
		}

		public IList<IPolyhedronVertex3D> VertexList
		{
			get { return vertexList; }
		}

		[Obsolete]
		public Boolean HasVertex(IPolyhedronVertex3D vertex)
		{
			return (vertexList.IndexOf(vertex) != -1);
		}

		private readonly Int32 id;
		private readonly List<IPolyhedronVertex3D> vertexList;
		private readonly Vector3D sideNormal;
	}
}