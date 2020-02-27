using System;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
	public class Polyhedron3DEqualityChecker
	{
		public Polyhedron3DEqualityChecker(ApproxComp approxComp)
		{
			this.approxComp = approxComp;
		}

		public Boolean Equal(IPolyhedron3D polyhedron1, IPolyhedron3D polyhedron2)
		{
			foreach (IPolyhedronVertex3D polyhedron1Vertex in polyhedron1.VertexList)
			{
				if (!ContainsVertex(polyhedron2, polyhedron1Vertex)) return false;
			}
			return true;
		}

		private Boolean ContainsVertex(IPolyhedron3D polyhedron, IPolyhedronVertex3D vertex)
		{
			Vector3D vertexVector = new Vector3D(vertex.XCoord, vertex.YCoord, vertex.ZCoord);
			foreach (IPolyhedronVertex3D polyhedronVertex in polyhedron.VertexList)
			{
				Vector3D currentVector = new Vector3D(polyhedronVertex.XCoord,
				                                      polyhedronVertex.YCoord,
				                                      polyhedronVertex.ZCoord);
				Vector3D delta = vertexVector - currentVector;
				if (approxComp.EQ(delta.Length, 0)) return true;
			}
			return false;
		}

		private readonly ApproxComp approxComp;
	}
}