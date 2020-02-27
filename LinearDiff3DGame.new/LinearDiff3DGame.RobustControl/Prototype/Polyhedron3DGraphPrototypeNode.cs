using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.RobustControl.Prototype
{
	internal class Polyhedron3DGraphPrototypeNode : IPolyhedron3DGraphPrototypeNode
	{
		public Polyhedron3DGraphPrototypeNode(Int32 id, Vector3D nodeNormal)
		{
			ID = id;
			ConnectionList = new List<IPolyhedron3DGraphPrototypeNode>();
			NodeNormal = nodeNormal;
		}

		public Int32 ID { get; private set; }

		public IList<IPolyhedron3DGraphPrototypeNode> ConnectionList { get; private set; }

		public Vector3D NodeNormal { get; private set; }

		public Pair<Double> SupportFuncValues { get; set; }
	}
}