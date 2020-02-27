using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// тип пересечения (графа с графом в виде большого круга ...  возможно использование и в общем случае ???)
    /// </summary>
    internal enum CrossingObjectType
    {
        /// <summary>
        /// пересечение на узле графа
        /// </summary>
        GraphNode = 0,
        /// <summary>
        /// пересечение на связи графа
        /// </summary>
        GraphConnection = 1
    }
}
