﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    public static class Polyhedron3DExtensions
    {
        public static IList<PolyhedronSide3D> GetSides4Vertex(this Polyhedron3D polyhedron, PolyhedronVertex3D vertex)
        {
            if(!polyhedron.VertexList.Contains(vertex))
                throw new AlgorithmException("Vertex does not belong polyhedron");
            return polyhedron.SideList
                .Where(s => s.VertexList.Contains(vertex))
                .ToList();
        }

        // Список соседних граней, для грани side, упорядоченный против ч.с, если смотреть с конца внешней нормали
        public static IList<PolyhedronSide3D> GetNeighbours4Side(this Polyhedron3D polyhedron,
                                                                 PolyhedronSide3D side)
        {
            if(!polyhedron.SideList.Contains(side))
                throw new AlgorithmException("Side does not belong polyhedron");
            IList<PolyhedronSide3D> sideList = new List<PolyhedronSide3D>(side.VertexList.Count);
            for(Int32 vertexIndex = 0; vertexIndex < side.VertexList.Count; ++vertexIndex)
            {
                PolyhedronVertex3D vertex = side.VertexList[vertexIndex];
                PolyhedronVertex3D nextVertex = side.VertexList.GetNextItem(vertexIndex);
                PolyhedronSide3D neighbourSide = polyhedron.FindNeighbour4Side(side, vertex, nextVertex);
                if(neighbourSide == null) throw new AlgorithmException("Can't find neighbour for side");
                sideList.Add(neighbourSide);
            }
            return sideList;
        }

        public static PolyhedronSide3D FindNeighbour4Side(this Polyhedron3D polyhedron,
                                                          PolyhedronSide3D side,
                                                          PolyhedronVertex3D vertex1,
                                                          PolyhedronVertex3D vertex2)
        {
            if(!polyhedron.SideList.Contains(side))
                throw new AlgorithmException("Side does not belong polyhedron");
            if(!side.VertexList.Contains(vertex1) || !side.VertexList.Contains(vertex2))
                throw new AlgorithmException("Vertex1 and vertex2 does not belong side");
            IList<PolyhedronSide3D> neighbourSides = polyhedron.SideList
                .Where(s => s != side && s.VertexList.Contains(vertex1) && s.VertexList.Contains(vertex2))
                .ToList();
            return neighbourSides.Count != 1 ? neighbourSides[0] : null;
        }
    }
}