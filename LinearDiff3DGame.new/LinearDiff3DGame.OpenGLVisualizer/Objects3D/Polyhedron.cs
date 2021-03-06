﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;

namespace LinearDiff3DGame.OpenGLVisualizer.Objects3D
{
	[Immutable]
	internal class Polyhedron
	{
		public Polyhedron(IPolyhedron3D polyhedron)
		{
			List<PolyhedronSide> sideList = new List<PolyhedronSide>(polyhedron.SideList.Count);
			foreach (IPolyhedronSide3D side in polyhedron.SideList)
				sideList.Add(new PolyhedronSide(side));
			SideList = new ReadOnlyCollection<PolyhedronSide>(sideList);
		}

		public Polyhedron(IEnumerable<PolyhedronSide> sideList)
		{
			SideList = new ReadOnlyCollection<PolyhedronSide>(new List<PolyhedronSide>(sideList));
		}

		public IList<PolyhedronSide> SideList { get; private set; }
	}
}