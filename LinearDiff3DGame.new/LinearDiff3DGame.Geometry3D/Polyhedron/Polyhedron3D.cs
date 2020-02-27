using System.Collections.Generic;

namespace LinearDiff3DGame.Geometry3D.Polyhedron
{
    public interface IPolyhedron3D
    {
        IList<IPolyhedronSide3D> SideList { get; }
        IList<IPolyhedronVertex3D> VertexList { get; }
    }

    public class Polyhedron3D : IPolyhedron3D
    {
        public Polyhedron3D(IEnumerable<IPolyhedronSide3D> sideList, IEnumerable<IPolyhedronVertex3D> vertexList)
        {
            this.sideList = new List<IPolyhedronSide3D>(sideList);
            this.vertexList = new List<IPolyhedronVertex3D>(vertexList);
        }

        /* ��� �� ���� ������  ... � ���� �� �������� ����������������� ������ ������ � ������ ������ ������� ������������� */
        public IList<IPolyhedronSide3D> SideList
        {
            get { return sideList; }
        }

        public IList<IPolyhedronVertex3D> VertexList
        {
            get { return vertexList; }
        }
        /* ��� �� ���� ������  ... � ���� �� �������� ����������������� ������ ������ � ������ ������ ������� ������������� */

        private readonly List<IPolyhedronSide3D> sideList;

        private readonly List<IPolyhedronVertex3D> vertexList;
    }
}