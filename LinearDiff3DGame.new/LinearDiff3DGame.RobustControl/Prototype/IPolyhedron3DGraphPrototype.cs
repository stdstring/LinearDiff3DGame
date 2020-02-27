using System.Collections.Generic;

namespace LinearDiff3DGame.RobustControl.Prototype
{
    public interface IPolyhedron3DGraphPrototype
    {
        IList<IPolyhedron3DGraphPrototypeNode> NodeList { get; }
    }
}