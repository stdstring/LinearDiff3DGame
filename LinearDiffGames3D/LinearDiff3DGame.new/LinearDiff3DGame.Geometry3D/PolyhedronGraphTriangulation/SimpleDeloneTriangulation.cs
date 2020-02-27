using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.Geometry3D.PolyhedronGraphTriangulation
{
    public class SimpleDeloneTriangulation
    {
        // TODO : реFUCKторинг
        public void Action(IList<IPolyhedron3DGraphNode> sector,
                           IList<Pair<IPolyhedron3DGraphNode, IPolyhedron3DGraphNode>> connSet)
        {
            Double maxCosAngle4Conn = 1;
            IPolyhedron3DGraphNode connNode1 = null;
            IPolyhedron3DGraphNode connNode2 = null;

            for (Int32 node1Index = 0; node1Index < sector.Count; ++node1Index)
            {
                IPolyhedron3DGraphNode node1 = sector[node1Index];
                for (Int32 node2Index = node1Index + 2; node2Index < sector.Count; ++node2Index)
                {
                    if (node1Index == 0 && node2Index == sector.Count - 1) continue;
                    IPolyhedron3DGraphNode node2 = sector[node2Index];
                    if (!CheckNodeVisibility(sector, node1, node2)) continue;

                    IPolyhedron3DGraphNode beforeNode1 = sector[sector.GetPrevItemIndex(node1Index)];
                    IPolyhedron3DGraphNode afterNode1 = sector[sector.GetNextItemIndex(node1Index)];
                    IPolyhedron3DGraphNode beforeNode2 = sector[sector.GetPrevItemIndex(node2Index)];
                    IPolyhedron3DGraphNode afterNode2 = sector[sector.GetNextItemIndex(node2Index)];

                    // тут считаем, что как-бы все происходит на плоскости
                    Vector3D vector12 = node2.NodeNormal - node1.NodeNormal;
                    Vector3D vector21 = node1.NodeNormal - node2.NodeNormal;
                    Vector3D vector1b = beforeNode1.NodeNormal - node1.NodeNormal;
                    Vector3D vector1a = afterNode1.NodeNormal - node1.NodeNormal;
                    Vector3D vector2b = beforeNode2.NodeNormal - node2.NodeNormal;
                    Vector3D vector2a = afterNode2.NodeNormal - node2.NodeNormal;
                    Double cos21b = Vector3DUtils.CosAngleBetweenVectors(vector1b,
                                                                         vector12);
                    Double cos21a = Vector3DUtils.CosAngleBetweenVectors(vector1a,
                                                                         vector12);
                    Double cos12b = Vector3DUtils.CosAngleBetweenVectors(vector2b,
                                                                         vector21);
                    Double cos12a = Vector3DUtils.CosAngleBetweenVectors(vector2a,
                                                                         vector21);
                    Double maxCosValue =
                        new[] {Math.Abs(cos21b), Math.Abs(cos21a), Math.Abs(cos12b), Math.Abs(cos12a)}.Max();
                    if (maxCosValue < maxCosAngle4Conn)
                    {
                        maxCosAngle4Conn = maxCosValue;
                        connNode1 = node1;
                        connNode2 = node2;
                    }
                }
            }

            // инвариант работы алгоритма
            Debug.Assert(maxCosAngle4Conn < 1 && maxCosAngle4Conn > -1);
            Debug.Assert(connNode1 != null);
            Debug.Assert(connNode2 != null);

            // create connection 1-2
            connNode1.ConnectionList.Insert(connNode1.ConnectionList.IndexOf(sector.GetPrevItem(connNode1)), connNode2);
            connNode2.ConnectionList.Insert(connNode2.ConnectionList.IndexOf(sector.GetPrevItem(connNode2)), connNode1);
            if (connSet != null)
                connSet.Add(new Pair<IPolyhedron3DGraphNode, IPolyhedron3DGraphNode>(connNode1, connNode2));

            IList<IPolyhedron3DGraphNode> subSector = new List<IPolyhedron3DGraphNode>();
            for (IPolyhedron3DGraphNode currentNode = connNode1;
                 currentNode != connNode2;
                 currentNode = sector.GetNextItem(currentNode))
                subSector.Add(currentNode);
            subSector.Add(connNode2);
            if (subSector.Count > 3)
                Action(subSector, connSet);

            subSector.Clear();
            for (IPolyhedron3DGraphNode currentNode = connNode2;
                 currentNode != connNode1;
                 currentNode = sector.GetNextItem(currentNode))
                subSector.Add(currentNode);
            subSector.Add(connNode1);
            if (subSector.Count > 3)
                Action(subSector, connSet);
        }

        //public void Action(ICyclicList<Polyhedron3DGraphNode> sector,
        //                   IList<Pair<Polyhedron3DGraphNode, Polyhedron3DGraphNode>> connSet)
        //{
        //    Double minAngle4Conn = 0;
        //    Polyhedron3DGraphNode connNode1 = null;
        //    Polyhedron3DGraphNode connNode2 = null;

        //    for (Int32 node1Index = 0; node1Index < sector.Count; ++node1Index)
        //    {
        //        Polyhedron3DGraphNode node1 = sector[node1Index];

        //        for (Int32 node2Index = node1Index + 2; node2Index < sector.Count; ++node2Index)
        //        {
        //            if (node1Index == 0 && node2Index == sector.Count - 1)
        //            {
        //                break;
        //            }

        //            Polyhedron3DGraphNode node2 = sector[node2Index];

        //            if (!CheckNodeVisibility(sector, node1, node2))
        //                break;

        //            Polyhedron3DGraphNode beforeNode1 = sector[sector.GetPrevItemIndex(node1Index)];
        //            Polyhedron3DGraphNode afterNode1 = sector[sector.GetNextItemIndex(node1Index)];
        //            Polyhedron3DGraphNode beforeNode2 = sector[sector.GetPrevItemIndex(node2Index)];
        //            Polyhedron3DGraphNode afterNode2 = sector[sector.GetNextItemIndex(node2Index)];

        //            Vector3D plane12Normal = Vector3D.VectorProduct(node1.NodeNormal, node2.NodeNormal);
        //            Double angle1 = Vector3DUtils.AngleBetweenVectors(plane12Normal,
        //                                                              Vector3D.VectorProduct(beforeNode1.NodeNormal,
        //                                                                                     node1.NodeNormal));
        //            angle1 = (angle1 > Math.PI/2 ? Math.PI - angle1 : angle1);
        //            Double angle2 = Vector3DUtils.AngleBetweenVectors(plane12Normal,
        //                                                              Vector3D.VectorProduct(node1.NodeNormal,
        //                                                                                     afterNode1.NodeNormal));
        //            angle2 = (angle2 > Math.PI/2 ? Math.PI - angle2 : angle2);
        //            Double angle3 = Vector3DUtils.AngleBetweenVectors(plane12Normal,
        //                                                              Vector3D.VectorProduct(beforeNode2.NodeNormal,
        //                                                                                     node2.NodeNormal));
        //            angle3 = (angle3 > Math.PI/2 ? Math.PI - angle3 : angle3);
        //            Double angle4 = Vector3DUtils.AngleBetweenVectors(plane12Normal,
        //                                                              Vector3D.VectorProduct(node2.NodeNormal,
        //                                                                                     afterNode2.NodeNormal));
        //            angle4 = (angle4 > Math.PI/2 ? Math.PI - angle4 : angle4);
        //            Double minAngle = GetMinValue(angle1, angle2, angle3, angle4);
        //            if (minAngle > minAngle4Conn)
        //            {
        //                minAngle4Conn = minAngle;
        //                connNode1 = node1;
        //                connNode2 = node2;
        //            }
        //        }
        //    }

        //    Debug.Assert(minAngle4Conn > 0, "minAngle4Conn == 0 !!! O_o");
        //    Debug.Assert(connNode1 != null, "connNode1 == null !!! O_o");
        //    Debug.Assert(connNode2 != null, "connNode2 == null !!! O_o");

        //    // create connection 1-2
        //    connNode1.ConnectionList.Insert(connNode1.ConnectionList.IndexOf(sector.GetPrevItem(connNode1)), connNode2);
        //    connNode2.ConnectionList.Insert(connNode2.ConnectionList.IndexOf(sector.GetPrevItem(connNode2)), connNode1);
        //    if (connSet != null)
        //    {
        //        connSet.Add(new Pair<Polyhedron3DGraphNode, Polyhedron3DGraphNode>(connNode1, connNode2));
        //    }

        //    ICyclicList<Polyhedron3DGraphNode> subSector = new CyclicList<Polyhedron3DGraphNode>();
        //    for (Polyhedron3DGraphNode currentNode = connNode1;
        //         currentNode != connNode2;
        //         currentNode = sector.GetNextItem(currentNode))
        //    {
        //        subSector.Add(currentNode);
        //    }
        //    subSector.Add(connNode2);
        //    if (subSector.Count > 3)
        //    {
        //        Action(subSector, connSet);
        //    }

        //    subSector.Clear();
        //    for (Polyhedron3DGraphNode currentNode = connNode2;
        //         currentNode != connNode1;
        //         currentNode = sector.GetNextItem(currentNode))
        //    {
        //        subSector.Add(currentNode);
        //    }
        //    subSector.Add(connNode1);
        //    if (subSector.Count > 3)
        //    {
        //        Action(subSector, connSet);
        //    }
        //}

        private static Boolean CheckNodeVisibility(IList<IPolyhedron3DGraphNode> sectorNodes,
                                                   IPolyhedron3DGraphNode node1,
                                                   IPolyhedron3DGraphNode node2)
        {
            Vector3D normal12 = Vector3DUtils.VectorProduct(node1.NodeNormal, node2.NodeNormal);

            for (IPolyhedron3DGraphNode node = node1;
                 node != node2;
                 node = sectorNodes.GetNextItem(node))
            {
                if (ReferenceEquals(node, node1))
                    continue;
                Double scalarProductValue = Vector3DUtils.ScalarProduct(node.NodeNormal, normal12);
                if (scalarProductValue >= 0)
                    return false;
            }

            for (IPolyhedron3DGraphNode node = node2;
                 node != node1;
                 node = sectorNodes.GetNextItem(node))
            {
                if (ReferenceEquals(node, node2))
                    continue;
                Double scalarProductValue = Vector3DUtils.ScalarProduct(node.NodeNormal, normal12);
                if (scalarProductValue <= 0)
                    return false;
            }

            return true;
        }

        /*/// <summary>
        /// проверка того, что в секторе sectorNodeList узел nodek виден из узла node1
        /// </summary>
        /// <param name="sectorNodes"></param>
        /// <param name="node1"></param>
        /// <param name="nodek"></param>
        /// <returns></returns>
        private Boolean CheckNodeVisibility(ICyclicList<Polyhedron3DGraphNode> sectorNodes, Polyhedron3DGraphNode node1, Polyhedron3DGraphNode nodek)
        {
            // в списке sectorNodes узлы распологаются против ч.с.
            // => список sectorNodes обходим в обратном порядке, чтобы получить упорядочение по ч.с.
            Polyhedron3DGraphNode node2 = sectorNodes.GetPrevItem(node1);
            Polyhedron3DGraphNode nodem = sectorNodes.GetNextItem(node1);

            // соотношение номер 1 (!!! ВНИМАНИЕ. ОЧЕНЬ ВАЖЕН ПОРЯДОК. СМ. КНИГУ !!!)
            //Double mixedProductk12 = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            //Double mixedProductk1m = Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);
            Double mixedProductk12 = -Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, node2.NodeNormal);
            Double mixedProductk1m = -Vector3D.MixedProduct(nodek.NodeNormal, node1.NodeNormal, nodem.NodeNormal);
            // должно быть : mixedProductk12 > 0, mixedProductk1m < 0
            if (mixedProductk12 <= 0 || mixedProductk1m >= 0)
            {
                return false;
            }

            // соотношение номер 2
            // описание алгоритма см. в документах (или на листочке)
            for (Polyhedron3DGraphNode nodei = node2; nodei != nodem; nodei = sectorNodes.GetPrevItem(nodei))
            {
                Polyhedron3DGraphNode nodeip1 = sectorNodes.GetPrevItem(nodei);

                if (ReferenceEquals(nodek, nodei) || ReferenceEquals(nodek, nodeip1))
                {
                    continue;
                }

                Vector3D v0 = Vector3D.VectorProduct(Vector3D.VectorProduct(nodeip1.NodeNormal, nodei.NodeNormal),
                                                     Vector3D.VectorProduct(node1.NodeNormal, nodek.NodeNormal));

                Double scalarProduct01 = Vector3D.ScalarProduct(v0, node1.NodeNormal);
                Double scalarProduct0k = Vector3D.ScalarProduct(v0, nodek.NodeNormal);
                Double scalarProduct1k = Vector3D.ScalarProduct(node1.NodeNormal, nodek.NodeNormal);

                Double scalarProduct0i = Vector3D.ScalarProduct(v0, nodei.NodeNormal);
                Double scalarProduct0ip1 = Vector3D.ScalarProduct(v0, nodeip1.NodeNormal);
                Double scalarProductiip1 = Vector3D.ScalarProduct(nodei.NodeNormal, nodeip1.NodeNormal);

                Double lambda1 = (scalarProduct01 - scalarProduct0k * scalarProduct1k) / (1 - scalarProduct1k * scalarProduct1k);
                Double lambda2 = (scalarProduct0k - scalarProduct01 * scalarProduct1k) / (1 - scalarProduct1k * scalarProduct1k);
                Double lambda3 = (scalarProduct0i - scalarProduct0ip1 * scalarProductiip1) / (1 - scalarProductiip1 * scalarProductiip1);
                Double lambda4 = (scalarProduct0ip1 - scalarProduct0i * scalarProductiip1) / (1 - scalarProductiip1 * scalarProductiip1);

                // must be max(lambda1, lambda2, lambda3, lambda4) > 0
                if (GetMaxValue(lambda1, lambda2, lambda3, lambda4) <= 0)
                {
                    return false;
                }
                // must be min(lambda1, lambda2, lambda3, lambda4) < 0
                if (GetMinValue(lambda1, lambda2, lambda3, lambda4) >= 0)
                {
                    return false;
                }
            }

            return true;
        }*/
    }
}