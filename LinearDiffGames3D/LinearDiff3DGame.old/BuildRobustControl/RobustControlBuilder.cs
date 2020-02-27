using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RobustControlBuilder : ISerializable
    {
        /// <summary>
        /// ����������� ������ RobustControlBuilder
        /// </summary>
        /// <param name="inputData">��������� � �������� �������</param>
        public RobustControlBuilder(DataContainer inputData)
        {
            Double startTime = (Double)inputData["StartTime"];
            Double finishTime = (Double)inputData["FinishTime"];
            Double timeInterval = finishTime - startTime;
            m_ApproxComparer = new ApproxComparer((Double)inputData["Epsilon"]);

            // ������������ ���������� ���� �������� ������
            List<ConvexPolyhedron3D> maxStableBridge = new List<ConvexPolyhedron3D>();

            List<Matrix> fundKoshiMatrixList = new List<Matrix>();
            List<Double> timeValueList = new List<Double>();

            // ���������� ������������� ����������� ����� �������� ������
            AlgorithmClass maxStableBridgeTask = new AlgorithmClass(inputData);
            // use GT or GE ???
            while (m_ApproxComparer.GE(timeInterval - maxStableBridgeTask.InverseTime, 0))
            {
                // T - ������� ������������� ����������� �����
                maxStableBridge.Insert(0, new ConvexPolyhedron3D(m_ApproxComparer,
                                                                 maxStableBridgeTask.GetSideList(),
                                                                 maxStableBridgeTask.GetVertexList()));
                // �������� ��������������� ������� ���� � ������ ������� finishTime - maxStableBridgeTask.InverseTime (???)
                fundKoshiMatrixList.Insert(0, maxStableBridgeTask.GetFundKoshiMatrix().GetCurrentFundKoshiMatrix());
                // �������� �������, ��� �������� �� ��� ��������
                timeValueList.Insert(0, finishTime - maxStableBridgeTask.InverseTime);

                maxStableBridgeTask.NextSolutionIteration();
            }
            // ���������� ������������� ����������� ����� �������� ������

            /*// �������� ������������� ����������� ����� �������� ������ �� ����������
            for (Int32 polyhedronIndex = 0; polyhedronIndex < maxStableBridge.Count; polyhedronIndex++)
            {
                if (!maxStableBridge[polyhedronIndex].IsPolyhedronConvex())
                {
                    throw new Exception("polyhedron must be convex !!!");
                }
            }
            // �������� ������������� ����������� ����� �������� ������ �� ����������*/

            m_TimeValueList = timeValueList.ToArray();
            m_FundKoshiMatrixList = fundKoshiMatrixList.ToArray();

            // ��������� �������� ������ (���������� ������������� ����������� �����)
            m_MatrixA = inputData["MatrixA"] as Matrix;
            m_MatrixB = inputData["MatrixB"] as Matrix;
            m_MatrixC = inputData["MatrixC"] as Matrix;
            m_DeltaT = (Double)inputData["DeltaT"];
            m_Mp = (Double)inputData["Mp"];
            m_Mq = (Double)inputData["Mq"];
            m_MinVectorDistinguishAngle = (Double)inputData["MinVectorDistinguishAngle"];
            // ��������� �������� ������ (���������� ������������� ����������� �����)

            // ��������� ������ ���������� ����������
            m_MaxVValue = (Double)inputData["MaxVValue"];
            m_DeltaScale = (Double)inputData["DeltaScale"];
            // ��������� ������ ���������� ����������

            // ��������� ��� ���������� ������� ���������� ������
            // ���������� ��� ��������� ������� ������
            Double deltaMq = (Double)inputData["DeltaMq"];
            // ���������� ������������ �������������� ������������� ���������
            Double deltaFSKoeff = (Double)inputData["DeltaFSKoeff"];
            // ���������� ������
            Int32 bridgesCount = (Int32)inputData["BridgesCount"];
            // ��������� ��� ���������� ������� ���������� ������

            // ������� ������� ��������� ���� � ����� ������
            m_StableBridgeSystem = CalcStableBridgeSystem(maxStableBridge, deltaMq, deltaFSKoeff, bridgesCount);

            m_RG = new Random();
        }

        /// <summary>
        /// ���������� robust - ���������� ��� ����� �������
        /// </summary>
        /// <param name="startPoint">��������� ����� ������� � ������� ������������</param>
        /// <returns>��������� � ��������� �������</returns>
        public DataContainer BuildRobustControl(Point3D startPoint)
        {
            // ���������� ������� ������ = robust control
            Double[] firstGamerControl = new Double[m_TimeValueList.Length];
            // ���������� ������� ������
            Double[] secondGamerControl = new Double[m_TimeValueList.Length];
            // ��������� ������� � ������� ������������
            Point3D[] systemPos = new Point3D[m_TimeValueList.Length];
            // ��������, ������������ ���������� �� ����� systemPos[...] ������ ������������� ����������� ����� �������� ������
            // Boolean[] isPosInMainBridge = new Boolean[m_TimeValueList.Length];
            // ������ ��������� �����
            Point3D[] nearestBridgePoint = new Point3D[m_TimeValueList.Length];
            // ������ �������� ���������� ����� �����
            Int32[] nearestBottomBridgeIndexList = new Int32[m_TimeValueList.Length];

            // ��������� ����� � ��������������� 3-� ������ ������������
            Point3D currentPoint = MatrixColumn2Point(m_FundKoshiMatrixList[0] * Point2MatrixColumn(startPoint));
            // ����� O � ��������������� 3-� ������ ������������
            Point3D pointO = new Point3D(0, 0, 0);

            // ������ ���������� ����� ����� ��� ������� �����, ���� -1, ���� ����� ����� ������ ������ ������� ����� �������
            Int32 nearestBottomBridgeIndex = -1;

            // ���������� ���������� ���������� �� T - �������� ������ �� ��������� ���������� ������
            // sectionIndex - ������ T - ������� ������ �� ��������� ���������� ������
            for (Int32 sectionIndex = 0; sectionIndex < m_TimeValueList.Length; sectionIndex++)
            {
                Matrix matrixD = m_FundKoshiMatrixList[sectionIndex] * m_MatrixB;
                Matrix matrixE = m_FundKoshiMatrixList[sectionIndex] * m_MatrixC;

                // ������ ������ ���������� ����� ����� ��� ������� ����� � ������ ������� 
                // ���������� �����, ������������ �� ���������� ��������
                nearestBottomBridgeIndex = FindNearestBottomBridgeIndex(currentPoint, sectionIndex, nearestBottomBridgeIndex);

                // ����������� �������������� �������� ������������� ���������� ������� ������ (0 <= scaleKoeff <= 1)
                Double scaleKoeff;
                // ��������� ����� ��� ������� �� �������������, �� ������� �� ���������
                Point3D nearestPoint = new Point3D(0, 0, 0);
                // ������ - ����������� �� ��������� �����
                Vector3D directionVector = Vector3D.ZeroVector3D;

                // ������� ����� ��������� ������ T-������� ������������� ����������� ����� �������� ������
                if (nearestBottomBridgeIndex == -1)
                {
                    // T-������� ������������� ����������� ����� �������� ������
                    ConvexPolyhedron3D currentTSection = m_StableBridgeSystem[0][sectionIndex];

                    // ������� ����������� �������������� �������� ������������� ���������� ������� ������
                    if (m_ApproxComparer.E(currentPoint.XCoord, 0) && m_ApproxComparer.E(currentPoint.YCoord, 0) && m_ApproxComparer.E(currentPoint.ZCoord, 0))
                    {
                        scaleKoeff = 0;
                    }
                    else
                    {
                        Point3D crossingPoint = currentTSection.GetCrossingPointWithRay_FullEmun(currentPoint);
                        scaleKoeff = AdvMath.DistanceBetween2Points(currentPoint, pointO) / AdvMath.DistanceBetween2Points(crossingPoint, pointO);
                    }

                    // ���� scaleKoeff < m_DeltaScale, �� ���������� ������� ������ �� ������
                    // (����� �� ���� ������ ��� ������ � ������ ��������������� ���������)
                    if (m_ApproxComparer.GT(scaleKoeff - m_DeltaScale, 0))
                    {
                        // ������������, �� ������� ���������� ���������
                        ConvexPolyhedron3D scaledPolyhedron = currentTSection.GetScaledPolyhedron(scaleKoeff - m_DeltaScale);

                        nearestPoint = scaledPolyhedron.GetNearestPoint4Given_FullEmun(currentPoint);
                        directionVector = new Vector3D(nearestPoint.XCoord - currentPoint.XCoord,
                                                       nearestPoint.YCoord - currentPoint.YCoord,
                                                       nearestPoint.ZCoord - currentPoint.ZCoord);
                    }
                }
                // ������� ����� ��������� ��� T-������� ������������� ����������� ����� �������� ������
                else
                {
                    scaleKoeff = 1;

                    // ������������, �� ������� ���������� ���������
                    ConvexPolyhedron3D nearestBottomPolyhedron = m_StableBridgeSystem[nearestBottomBridgeIndex][sectionIndex];

                    nearestPoint = nearestBottomPolyhedron.GetNearestPoint4Given_FullEmun(currentPoint);
                    directionVector = new Vector3D(nearestPoint.XCoord - currentPoint.XCoord,
                                                   nearestPoint.YCoord - currentPoint.YCoord,
                                                   nearestPoint.ZCoord - currentPoint.ZCoord);
                }

                Vector3D vectorD = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
                Double scalarProduct = Vector3D.ScalarProduct(directionVector, vectorD);
                Double firstPlayerControlValue = 0;

                //if (scalarProduct > 0)
                if (m_ApproxComparer.GT(scalarProduct, 0))
                {
                    // ��� firstPlayerControlValue = m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                    firstPlayerControlValue = m_Mp * scaleKoeff;
                }
                //if (scalarProduct < 0)
                if (m_ApproxComparer.LT(scalarProduct, 0))
                {
                    // ��� firstPlayerControlValue = -m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                    firstPlayerControlValue = -m_Mp * scaleKoeff;
                }

                Double secondPlayerControlValue = SecondGamerAction(m_TimeValueList[sectionIndex]);

                // �������� ������
                firstGamerControl[sectionIndex] = firstPlayerControlValue;
                secondGamerControl[sectionIndex] = secondPlayerControlValue;
                systemPos[sectionIndex] = currentPoint;
                nearestBridgePoint[sectionIndex] = nearestPoint;
                //isPosInMainBridge[sectionIndex] = m_StableBridgeSystem[0][sectionIndex].IsPointInside(currentPoint);
                nearestBottomBridgeIndexList[sectionIndex] = nearestBottomBridgeIndex;
                // �������� ������

                // ����� ������� ����� �������
                Matrix oldCurrentPoint = Point2MatrixColumn(currentPoint);
                currentPoint = MatrixColumn2Point(oldCurrentPoint + m_DeltaT * (firstPlayerControlValue * matrixD + secondPlayerControlValue * matrixE));
            }
            // ���������� ���������� ���������� �� T - �������� ������ �� ��������� ���������� ������

            // ��������� � ��������� ������� (� ������� ������� ����������)
            DataContainer outputData = new DataContainer();
            outputData.Add("TimeValueList", m_TimeValueList);
            outputData.Add("FirstGamerControl", firstGamerControl);
            outputData.Add("SecondGamerControl", secondGamerControl);
            outputData.Add("SystemPos", systemPos);
            outputData.Add("StableBridgeSystem", m_StableBridgeSystem);
            //outputData.Add("IsPosInMainBridge", isPosInMainBridge);
            outputData.Add("NearestBridgePoint", nearestBridgePoint);
            outputData.Add("NearestBottomBridgeIndex", nearestBottomBridgeIndexList);

            return outputData;
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ApproxComparer", m_ApproxComparer);
            info.AddValue("MatrixA", m_MatrixA);
            info.AddValue("MatrixB", m_MatrixB);
            info.AddValue("MatrixC", m_MatrixC);
            info.AddValue("Mp", m_Mp);
            info.AddValue("Mq", m_Mq);
            info.AddValue("DeltaT", m_DeltaT);
            info.AddValue("MinVectorDistinguishAngle", m_MinVectorDistinguishAngle);
            info.AddValue("MaxVValue", m_MaxVValue);
            info.AddValue("DeltaScale", m_DeltaScale);
            info.AddValue("TimeValueList", m_TimeValueList);
            info.AddValue("FundKoshiMatrixList", m_FundKoshiMatrixList);
            info.AddValue("StableBridgeSystem", m_StableBridgeSystem);
        }

        #endregion

        protected RobustControlBuilder(SerializationInfo info, StreamingContext context)
        {
            m_ApproxComparer = info.GetValue("ApproxComparer", typeof(ApproxComparer)) as ApproxComparer;
            m_MatrixA = info.GetValue("MatrixA", typeof(Matrix)) as Matrix;
            m_MatrixB = info.GetValue("MatrixB", typeof(Matrix)) as Matrix;
            m_MatrixC = info.GetValue("MatrixC", typeof(Matrix)) as Matrix;
            m_Mp = (Double)info.GetValue("Mp", typeof(Double));
            m_Mq = (Double)info.GetValue("Mq", typeof(Double));
            m_DeltaT = (Double)info.GetValue("DeltaT", typeof(Double));
            m_MinVectorDistinguishAngle = (Double)info.GetValue("MinVectorDistinguishAngle", typeof(Double));
            m_MaxVValue = (Double)info.GetValue("MaxVValue", typeof(Double));
            m_DeltaScale = (Double)info.GetValue("DeltaScale", typeof(Double));
            m_TimeValueList = info.GetValue("TimeValueList", typeof(Double[])) as Double[];
            m_FundKoshiMatrixList = info.GetValue("FundKoshiMatrixList", typeof(Matrix[])) as Matrix[];
            m_StableBridgeSystem = info.GetValue("StableBridgeSystem", typeof(List<ConvexPolyhedron3D[]>)) as List<ConvexPolyhedron3D[]>;

            m_RG = new Random();
        }

        /// <summary>
        /// ������ ��������� ���� � ����� ������� ���������� ������
        /// ������ ���� � ���� ������� - ������������ ���������� ���� ��� �������� ������
        /// </summary>
        /// <param name="maxStableBridge">������������ ���������� ���� ��� �������� ������</param>
        /// <param name="deltaMq">���������� ��� ��������� ������� ������</param>
        /// <param name="deltaFSKoeff">���������� ������������ �������������� ������������� ���������</param>
        /// <param name="bridgesCount">���������� ������</param>
        /// <returns>��������� ���� � ����� ������� ���������� ������</returns>
        private List<ConvexPolyhedron3D[]> CalcStableBridgeSystem(List<ConvexPolyhedron3D> maxStableBridge, Double deltaMq, Double deltaFSKoeff, Int32 bridgesCount)
        {
            // ������� ���������� ��������� ���� � ����� ������� ���������� ������:
            // ����� � ��� �������� ��� ���� � ������� i-1 (���� � ������� 0 - ������������ ���������� ���� ��� �������� ������)
            // ���������� ��������� i-� ����
            //
            // ����������� ������������ �������� ���������� ������� ������, ���������������� ��� ����������
            // (i-1)-�� �����, �� �������� deltaMq
            //
            // {
            //   ����������� ����������� �������������� ������������� ��������� ������������� ����������� �����
            //   �������� ������, ���������������� ��� ���������� (i-1)-�� �����, �� �������� deltaFSKoeff
            //
            //   ������ � ����������� ����������� ������������ ���������� ����
            //
            //   ���� ����������� ���� �������� � ���� (i-1)-� ���� (���� ��� T-������� (i-1)-�� ����� ����� ������ T-������� ������������)
            //   {
            //     ����������� ���� ���������� i-� ���������� ������,
            //   }
            // } (���� �� �������� i-� ���������� ����)

            // ������������ �������� ���������� ������� ������
            Double mq = m_Mq + deltaMq;
            // ������������ �������������� ������������� ���������
            Double fsKoeff = 1;
            // ������������ ���������
            ConvexPolyhedron3D finalSet = maxStableBridge[maxStableBridge.Count - 1];

            // ������� ������ ��� ������� ���������� ����� �� ���������
            DataContainer inputData = new DataContainer();
            inputData["MatrixA"] = m_MatrixA;
            inputData["MatrixB"] = m_MatrixB;
            inputData["MatrixC"] = m_MatrixC;
            inputData["FinalSet"] = null;
            inputData["Mp"] = m_Mp;
            inputData["Mq"] = mq;
            inputData["DeltaT"] = m_DeltaT;
            inputData["MinVectorDistinguishAngle"] = m_MinVectorDistinguishAngle;
            inputData["Epsilon"] = m_ApproxComparer.Epsilon;

            // ������� ������
            List<ConvexPolyhedron3D[]> stableBridgesSystem = new List<ConvexPolyhedron3D[]>();
            stableBridgesSystem.Add(maxStableBridge.ToArray());

            //m_MaxStableBridgeSystemParams = new List<DoublePair>();
            //m_MaxStableBridgeSystemParams.Add(new DoublePair(m_Mq, fsKoeff));

            for (Int32 bridgeIndex = 0; bridgeIndex < bridgesCount; )
            {
                // ����������� ������������ ��������� ��� ���������� ����� �� ���������
                fsKoeff += deltaFSKoeff;
                ConvexPolyhedron3D scaledFinalSet = finalSet.GetScaledPolyhedron(fsKoeff);
                inputData["FinalSet"] = scaledFinalSet.ToPoint3DArray();

                // ����, ����������� �� ���������� ��������
                ConvexPolyhedron3D[] prevCalcBridge = stableBridgesSystem[bridgeIndex];

                // �������������� �������� ���������� ������������� ����������� �����
                AlgorithmClass algorithm = new AlgorithmClass(inputData);
                List<ConvexPolyhedron3D> stableBridge = new List<ConvexPolyhedron3D>();

                for (Int32 timePointIndex = 0; timePointIndex < m_TimeValueList.Length; timePointIndex++)
                {
                    ConvexPolyhedron3D currentTSection = new ConvexPolyhedron3D(m_ApproxComparer, algorithm.GetSideList(), algorithm.GetVertexList());
                    ConvexPolyhedron3D prevBridgeTSection = prevCalcBridge[prevCalcBridge.Length - timePointIndex - 1];

                    if (currentTSection.IsPolyhedronInside(prevBridgeTSection))
                    {
                        algorithm.NextSolutionIteration();
                        stableBridge.Insert(0, currentTSection);
                    }
                    else
                    {
                        break;
                    }
                }

                // ���� maxStableBridge.Count == m_TimeValueList.Length, �� ������ ���������� ���� ��������� ����� ������ ������������
                if (maxStableBridge.Count == m_TimeValueList.Length)
                {
                    stableBridgesSystem.Add(stableBridge.ToArray());
                    //m_MaxStableBridgeSystemParams.Add(new DoublePair(mq, fsKoeff));

                    // ���� ������ ��������� ����, �� ����������� ������������ �������� ���������� ������� ������
                    mq += deltaMq;
                    inputData["Mq"] = mq;
                    bridgeIndex++;
                }
            }

            return stableBridgesSystem;
        }

        /// <summary>
        /// ��������������� �� ���� Point3D � ��� Matrix(3, 1)
        /// ������ ���� �� ����� !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Matrix Point2MatrixColumn(Point3D point)
        {
            Matrix matrixColumn = new Matrix(3, 1);

            matrixColumn[1, 1] = point.XCoord;
            matrixColumn[2, 1] = point.YCoord;
            matrixColumn[3, 1] = point.ZCoord;

            return matrixColumn;
        }

        /// <summary>
        /// ��������������� �� ���� Matrix(3, 1) � ��� Point3D
        /// ������ ���� �� ����� !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private Point3D MatrixColumn2Point(Matrix column)
        {
            if ((column.RowCount != 3) || (column.ColumnCount != 1))
            {
                throw new ArgumentException("incorrect matrix !!!");
            }

            return new Point3D(column[1, 1], column[2, 1], column[3, 1]);
        }

        /// <summary>
        /// ����� FindNearestBottomBridgeIndex ���������� ������ ���������� ����� ����� ��� �������� ����� position
        /// � ������, ���� ����� ����� ������ ������������� ����������� ����� �������� ������ (������ ������� �����) ������������ -1
        /// </summary>
        /// <param name="position">�������� �����</param>
        /// <param name="sectionIndex">������ T - ������� ������������� ����������� �����</param>
        /// <param name="lastNearestBottomBridgeIndex">������ ���������� ����� �����, ���������� � ���������� ���</param>
        /// <returns>������ ���������� ����� ����� ��� �������� ����� position, ���� -1, ���� ����� ����� ������ ������ ������� �����</returns>
        private Int32 FindNearestBottomBridgeIndex(Point3D position, Int32 sectionIndex, Int32 lastNearestBottomBridgeIndex)
        {
#warning ������ ������� !!!!!!!! �������� (((
            if (m_StableBridgeSystem[0][sectionIndex].IsPointInside(position))
            {
                return -1;
            }

            for (Int32 bridgeIndex = 1; bridgeIndex < m_StableBridgeSystem.Count; bridgeIndex++)
            {
                if (m_StableBridgeSystem[bridgeIndex][sectionIndex].IsPointInside(position))
                {
                    return bridgeIndex - 1;
                }
            }

            return m_StableBridgeSystem.Count - 1;
        }

        [Obsolete("���� ������ ������ !!!")]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private Double SecondGamerAction(Double time)
        {
            /*Double omega = 16 * Math.PI;
            Double fhi = 0;

            return m_MaxVValue * Math.Sin(omega * time + fhi);*/
            return m_MaxVValue * 2 * (m_RG.NextDouble() - 0.5);
            //return 0;
        }

        /// <summary>
        /// ������������ ��� �������� ���������������� ��������� �������������� �����
        /// </summary>
        private ApproxComparer m_ApproxComparer;

        // ��������� �������� ������ (���������� ������������� ����������� �����)
        /// <summary>
        /// ������� A �������
        /// </summary>
        private Matrix m_MatrixA;
        /// <summary>
        /// ������� B �������
        /// </summary>
        private Matrix m_MatrixB;
        /// <summary>
        /// ������� C �������
        /// </summary>
        private Matrix m_MatrixC;
        /// <summary>
        /// ��������� ��������� ������� ������
        /// </summary>
        private Double m_Mp;
        /// <summary>
        /// ��������� ��������� ������� ������
        /// </summary>
        private Double m_Mq;
        /// <summary>
        /// �������� ������������� t - ���
        /// </summary>
        private Double m_DeltaT;
        /// <summary>
        /// ����������� �������� ���� ����� ���������, ��� ������� �� ������� ��� ������� ���������� (�� ��������)
        /// </summary>
        private Double m_MinVectorDistinguishAngle;
        // ��������� �������� ������ (���������� ������������� ����������� �����)

        // ��������� ������ ���������� ����������
        /// <summary>
        /// ������������ �������� ���������� ������� ������
        /// </summary>
        private Double m_MaxVValue;
        /// <summary>
        /// ...
        /// </summary>
        private Double m_DeltaScale;
        // ��������� ������ ���������� ����������

        // ��������� ������ �������� ������
        /// <summary>
        /// ������ ���������� �������� �� t - ���
        /// </summary>
        private Double[] m_TimeValueList;
        /// <summary>
        /// ������ �������� ��������������� ������� ����
        /// </summary>
        private Matrix[] m_FundKoshiMatrixList;
        /// <summary>
        /// ��������� ���� � ����� ������� ���������������� ���������� ������
        /// ������ ���� � ���� ������� - ������������ ���������� ���� ��� �������� ������
        /// </summary>
        private List<ConvexPolyhedron3D[]> m_StableBridgeSystem;
        // ��������� ������ �������� ������

        private Random m_RG;
    }
}
