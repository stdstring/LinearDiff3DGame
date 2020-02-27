using System;
using System.Collections.Generic;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraph
{
    public interface IPolyhedron3DGraphNode
    {
        // ID узла
        Int32 ID { get; }

        // ID поколения
        Int32 GenerationID { get; }

        // список связей данного узла
        IList<IPolyhedron3DGraphNode> ConnectionList { get; }

        // "внешняя" нормаль
        Vector3D NodeNormal { get; set; }

        // значение опорной функции для узла
        Double SupportFuncValue { get; set; }
    }
}