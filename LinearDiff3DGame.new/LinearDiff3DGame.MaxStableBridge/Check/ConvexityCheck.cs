﻿using System;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.LinearEquationsSet;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;
using LinearDiff3DGame.MaxStableBridge.SuspiciousConnections;

namespace LinearDiff3DGame.MaxStableBridge.Check
{
    internal class ConvexityCheck
    {
        public ConvexityCheck(ApproxComp approxComp)
        {
            approxComparer = approxComp;
            lesSolver = new LESKramer3Solver();
        }

        public Boolean Check(IPolyhedron3DGraph graph)
        {
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode node1 = graph.NodeList[nodeIndex];
                for(Int32 connIndex = 0; connIndex < node1.ConnectionList.Count; ++connIndex)
                {
                    IPolyhedron3DGraphNode node2 = node1.ConnectionList[connIndex];
                    // если ID узла 2 < ID узла 1, то данную связь уже проверили
                    if(node2.ID < node1.ID)
                    {
                        continue;
                    }
                    // узел 3; связь 1-3 предыдущая по отношению к связи 1-2
                    IPolyhedron3DGraphNode node3 = node1.ConnectionList.GetPrevItem(node2);
                    // узел 4; связь 1-4 следующая по отношению к связи 1-2
                    IPolyhedron3DGraphNode node4 = node1.ConnectionList.GetNextItem(node2);
                    //
                    if(!CheckConnConvexity(node1, node2, node3, node4))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Boolean Check(IPolyhedron3DGraph graph, SuspiciousConnectionSet connSet)
        {
            Boolean result = true;
            for(Int32 nodeIndex = 0; nodeIndex < graph.NodeList.Count; ++nodeIndex)
            {
                IPolyhedron3DGraphNode node1 = graph.NodeList[nodeIndex];
                for(Int32 connIndex = 0; connIndex < node1.ConnectionList.Count; ++connIndex)
                {
                    IPolyhedron3DGraphNode node2 = node1.ConnectionList[connIndex];
                    // если ID узла 2 < ID узла 1, то данную связь уже проверили
                    if(node2.ID < node1.ID)
                    {
                        continue;
                    }
                    // узел 3; связь 1-3 предыдущая по отношению к связи 1-2
                    IPolyhedron3DGraphNode node3 = node1.ConnectionList.GetPrevItem(node2);
                    // узел 4; связь 1-4 следующая по отношению к связи 1-2
                    IPolyhedron3DGraphNode node4 = node1.ConnectionList.GetNextItem(node2);
                    //
                    if(!CheckConnConvexity(node1, node2, node3, node4))
                    {
                        connSet.AddConnection(node1, node2);
                        result = false;
                    }
                }
            }
            return result;
        }

        private Boolean CheckConnConvexity(IPolyhedron3DGraphNode node1,
                                           IPolyhedron3DGraphNode node2,
                                           IPolyhedron3DGraphNode node3,
                                           IPolyhedron3DGraphNode node4)
        {
            // решение системы лин. уравнений (3x3), используемое для проверки связи 1-2 на локальную выпуклость (см. алгоритм)
            Matrix cone123Solution = SolveCone123EquationSystem(node1, node2, node3);
            // проверка связи 1-2 на локальную выпуклость
            Double localConvexCriterion = cone123Solution[1, 1]*node4.NodeNormal.X +
                                          cone123Solution[2, 1]*node4.NodeNormal.Y +
                                          cone123Solution[3, 1]*node4.NodeNormal.Z;

            // if (localConvexCriterion <= node4.SupportFuncValue) то связь выпукла
            return approxComparer.LE(localConvexCriterion, node4.SupportFuncValue);
        }

        // метод SolveCone123EquationSystem решает систему уравнений ls*y = ksi(ls)
        // См. статью "Численное решение дифференциальной игры наведения третьего порядка" Зарх М.А., Пацко В.С.
        private Matrix SolveCone123EquationSystem(IPolyhedron3DGraphNode node1,
                                                  IPolyhedron3DGraphNode node2,
                                                  IPolyhedron3DGraphNode node3)
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = node1.NodeNormal.X;
            matrixA[1, 2] = node1.NodeNormal.Y;
            matrixA[1, 3] = node1.NodeNormal.Z;
            matrixA[2, 1] = node2.NodeNormal.X;
            matrixA[2, 2] = node2.NodeNormal.Y;
            matrixA[2, 3] = node2.NodeNormal.Z;
            matrixA[3, 1] = node3.NodeNormal.X;
            matrixA[3, 2] = node3.NodeNormal.Y;
            matrixA[3, 3] = node3.NodeNormal.Z;

            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = node1.SupportFuncValue;
            matrixB[2, 1] = node2.SupportFuncValue;
            matrixB[3, 1] = node3.SupportFuncValue;

            Matrix matrixError;

            Matrix solution = lesSolver.Solve(matrixA, matrixB, out matrixError);
            return solution;
        }

        private readonly ApproxComp approxComparer;

        private readonly ILinearEquationsSystemSolver lesSolver;
    }
}