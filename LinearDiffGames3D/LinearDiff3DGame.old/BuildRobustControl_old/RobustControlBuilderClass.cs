using System;
using System.Collections.Generic;
using System.Text;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    public class RobustControlBuilderClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        /// <param name="deltaScale"></param>
        /// <returns></returns>
        public static RobustControlBuilderClass CreateRobustControlBuilder(RCInputDataReader reader)
        {
            Dictionary<String, Object> inputData = reader.InputData;

            Double startTime = (Double)inputData["StartTime"];
            Double finishTime = (Double)inputData["FinishTime"];
            Double timeInterval = finishTime - startTime;
            Double epsilon = (Double)inputData["Epsilon"];

            List<PolyhedronClass> maxStableBridge = new List<PolyhedronClass>();
            List<Matrix> fundKoshiMatrixList = new List<Matrix>();
            List<Double> timeValueList = new List<Double>();

            AlgorithmClass algorithm = new AlgorithmClass(reader.InputData);
            //maxStableBridge.Add(CreateAndFillPolyhedron(algorithm.GetSideList(), algorithm.GetVertexList()));

            // while ((timeInterval - algorithm.InverseTime) > 0)
            while ((timeInterval - algorithm.InverseTime) > epsilon)
            {
                //algorithm.NextSolutionIteration();
                maxStableBridge.Insert(0, CreateAndFillPolyhedron(algorithm.GetSideList(), algorithm.GetVertexList()));
                fundKoshiMatrixList.Insert(0, algorithm.GetFundKoshiMatrix().GetCurrentFundKoshiMatrix());
                timeValueList.Insert(0, finishTime - algorithm.InverseTime);

                algorithm.NextSolutionIteration();
            }

            // ���������� ��� ��������� ������� ������
            Double deltaMq = (Double)inputData["DeltaMq"];
            // ���������� ������������ �������������� ������������� ���������
            Double deltaFSKoeff = (Double)inputData["DeltaFSKoeff"];
            // ���������� ������
            Int32 bridgesCount = (Int32)inputData["BridgesCount"];

            RobustControlBuilderClass robustControlBuilder = new RobustControlBuilderClass();
            robustControlBuilder.m_MaxStableBridge = maxStableBridge.ToArray();
            robustControlBuilder.m_FundKoshiMatrixList = fundKoshiMatrixList.ToArray();
            robustControlBuilder.m_TimeValueList = timeValueList.ToArray();            

            robustControlBuilder.m_DeltaScale = (Double)inputData["DeltaScale"];

            robustControlBuilder.m_AMatrix = inputData["MatrixA"] as Matrix;//algorithm.GetAMatrix();
            robustControlBuilder.m_BMatrix = inputData["MatrixB"] as Matrix;//algorithm.GetBMatrix();
            robustControlBuilder.m_CMatrix = inputData["MatrixC"] as Matrix;//algorithm.GetCMatrix();
            robustControlBuilder.m_DeltaT = (Double)inputData["DeltaT"];//algorithm.DeltaT;
            robustControlBuilder.m_MaxUValue = (Double)inputData["Mp"];//algorithm.Mp;
            robustControlBuilder.m_Mq = (Double)inputData["Mq"];
            robustControlBuilder.m_MaxVValue = (Double)inputData["Mq2"];//algorithm.Mq;
            robustControlBuilder.m_Epsilon = (Double)inputData["Epsilon"];
            robustControlBuilder.m_MinVectorDistinguishAngle = (Double)inputData["MinVectorDistinguishAngle"];

            // ������� ������� ��������� ���� � ����� ������
            robustControlBuilder.m_MaxStableBridgeSystem = robustControlBuilder.CalcMaxStableBridgeSystem(deltaMq, deltaFSKoeff, bridgesCount);

            return robustControlBuilder;
        }

        /// <summary>
        /// ����� BuildRobustControl ������� ���������� ������� ������ ��� ���� ����� �� ��������� ������� [T���, T���] (�� m_TimeValueList)
        /// </summary>
        /// <param name="startPoint">��������� ����� � ������� ������������, � ������� ��������� �������</param>
        /// <param name="timeValueList"></param>
        /// <param name="secondGamerControlList"></param>
        /// <param name="scaleKoeffList"></param>
        /// <param name="nearestBottomBridgeIndicies"></param>
        /// <param name="checkPointLocationList"></param>
        /// <returns></returns>
        public Double[] BuildRobustControl(Point3D startPoint, out Double[] timeValueList, out Double[] secondGamerControlList, out Double[] scaleKoeffList, out Int32[] nearestBottomBridgeIndicies, out Boolean[] checkPointLocationList)
        {
            /*Double[] robustControl = new Double[m_MaxStableBridge.Length];
            secondGamerControlList = new Double[m_MaxStableBridge.Length];
            scaleKoeffList = new Double[m_MaxStableBridge.Length];
            nearestBottomBridgeIndicies = new Int32[m_MaxStableBridge.Length];
            checkPointLocationList = new Boolean[m_MaxStableBridge.Length];

            Point3D currentPoint = MatrixColumn2Point(m_FundKoshiMatrixList[0] * Point2MatrixColumn(startPoint));
            Point3D pointO = new Point3D(0, 0, 0);

            // sectionIndex - ������ T - ������� ������������� ����������� �����
            for (Int32 sectionIndex = 0; sectionIndex < m_MaxStableBridge.Length; sectionIndex++)
            {
                Matrix matrixD = m_FundKoshiMatrixList[sectionIndex] * m_BMatrix;
                Matrix matrixE = m_FundKoshiMatrixList[sectionIndex] * m_CMatrix;

                Int32 nearestBottomBridgeIndex = FindNearestBottomBridgeIndex(currentPoint, sectionIndex, -1);
                // ����� �������� ((( ??????? ... �������� �� ���������
                PolyhedronClass currentTSection = m_MaxStableBridgeSystem[nearestBottomBridgeIndex + 1][sectionIndex];

                Double scaleKoeff;
                if ((Math.Abs(currentPoint.XCoord) < m_Epsilon) &&
                    (Math.Abs(currentPoint.YCoord) < m_Epsilon) &&
                    (Math.Abs(currentPoint.ZCoord) < m_Epsilon))
                {
                    scaleKoeff = 0;
                }
                else if (nearestBottomBridgeIndex == -1)
                {
                    Point3D crossingPoint = currentTSection.GetCrossingPointWithRay_FullEmun(currentPoint);
                    scaleKoeff = AdvMathClass.DistanceBetween2Points(currentPoint, pointO) / AdvMathClass.DistanceBetween2Points(crossingPoint, pointO);
                }
                else
                {
                    scaleKoeff = 1;
                }

                scaleKoeffList[sectionIndex] = scaleKoeff;
                checkPointLocationList[sectionIndex] = currentTSection.IsPointInside(currentPoint);
                nearestBottomBridgeIndicies[sectionIndex] = nearestBottomBridgeIndex;

                //if (scaleKoeff > m_DeltaScale)
                if ((scaleKoeff - m_DeltaScale) > m_Epsilon)
                {
                    PolyhedronClass scaledPolyhedron = (scaleKoeff == 1 ?
                                                        currentTSection :
                                                        currentTSection.GetScaledPolyhedron(scaleKoeff - m_DeltaScale));
                    
                    Point3D nearestPoint = scaledPolyhedron.GetNearestPoint4Given_FullEmun(currentPoint);
                    Vector3D directionVector = new Vector3D(nearestPoint.XCoord - currentPoint.XCoord,
                                                            nearestPoint.YCoord - currentPoint.YCoord,
                                                            nearestPoint.ZCoord - currentPoint.ZCoord);

                    Vector3D vectorD = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
                    Double scalarProduct = Vector3D.ScalarProduct(directionVector, vectorD);
                    Double controlValue = 0;

                    //if (scalarProduct > 0)
                    if (Math.Abs(scalarProduct) > m_Epsilon)
                    {
                        // ��� controlValue = m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                        controlValue = m_MaxUValue * scaleKoeff;
                    }
                    //if (scalarProduct < 0)
                    if (Math.Abs(scalarProduct) < -m_Epsilon)
                    {
                        // ��� controlValue = -m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                        controlValue = -m_MaxUValue * scaleKoeff;
                    }

                    robustControl[sectionIndex] = controlValue;
                }
                else
                {
                    robustControl[sectionIndex] = 0;
                }

                Matrix oldCurrentPoint = Point2MatrixColumn(currentPoint);
                Double secondGamerControlValue = SecondGamerAction(m_TimeValueList[sectionIndex]);
                secondGamerControlList[sectionIndex] = secondGamerControlValue;
                Matrix delta = m_DeltaT * (robustControl[sectionIndex] * matrixD + secondGamerControlValue * matrixE);

                currentPoint = MatrixColumn2Point(oldCurrentPoint + delta);
            }

            timeValueList = m_TimeValueList;
            return robustControl;*/

            Double[] robustControl = new Double[m_MaxStableBridge.Length];
            secondGamerControlList = new Double[m_MaxStableBridge.Length];
            scaleKoeffList = new Double[m_MaxStableBridge.Length];
            nearestBottomBridgeIndicies = new Int32[m_MaxStableBridge.Length];
            checkPointLocationList = new Boolean[m_MaxStableBridge.Length];

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
                Matrix matrixD = m_FundKoshiMatrixList[sectionIndex] * m_BMatrix;
                Matrix matrixE = m_FundKoshiMatrixList[sectionIndex] * m_CMatrix;

                // ������ ������ ���������� ����� ����� ��� ������� ����� � ������ ������� 
                // ���������� �����, ������������ �� ���������� ��������
                nearestBottomBridgeIndex = FindNearestBottomBridgeIndex(currentPoint, sectionIndex, nearestBottomBridgeIndex);

                // ����������� �������������� �������� ������������� ���������� ������� ������ (0 <= scaleKoeff <= 1)
                Double scaleKoeff;
                // ��������� ����� ��� ������� �� �������������, �� ������� �� ���������
                Point3D nearestPoint;
                // ������ - ����������� �� ��������� �����
                Vector3D directionVector = Vector3D.ZeroVector3D;

                // ������� ����� ��������� ������ T-������� ������������� ����������� ����� �������� ������
                if (nearestBottomBridgeIndex == -1)
                {
                    // T-������� ������������� ����������� ����� �������� ������
                    PolyhedronClass currentTSection = m_MaxStableBridgeSystem[0][sectionIndex];

                    // ������� ����������� �������������� �������� ������������� ���������� ������� ������
                    if (Math.Abs(currentPoint.XCoord) < m_Epsilon && Math.Abs(currentPoint.YCoord) < m_Epsilon && Math.Abs(currentPoint.ZCoord) < m_Epsilon)
                    {
                        scaleKoeff = 0;
                    }
                    else
                    {
                        Point3D crossingPoint = currentTSection.GetCrossingPointWithRay_FullEmun(currentPoint);
                        scaleKoeff = AdvMathClass.DistanceBetween2Points(currentPoint, pointO) / AdvMathClass.DistanceBetween2Points(crossingPoint, pointO);
                    }

                    // ���� scaleKoeff < m_DeltaScale, �� ���������� ������� ������ �� ������
                    // (����� �� ���� ������ ��� ������ � ������ ��������������� ���������)
                    if ((scaleKoeff - m_DeltaScale) > m_Epsilon)
                    {
                        // ������������, �� ������� ���������� ���������
                        PolyhedronClass scaledPolyhedron = currentTSection.GetScaledPolyhedron(scaleKoeff - m_DeltaScale);

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
                    PolyhedronClass nearestBottomPolyhedron = m_MaxStableBridgeSystem[nearestBottomBridgeIndex][sectionIndex];

                    nearestPoint = nearestBottomPolyhedron.GetNearestPoint4Given_FullEmun(currentPoint);
                    directionVector = new Vector3D(nearestPoint.XCoord - currentPoint.XCoord,
                                                   nearestPoint.YCoord - currentPoint.YCoord,
                                                   nearestPoint.ZCoord - currentPoint.ZCoord);
                }

                Vector3D vectorD = new Vector3D(matrixD[1, 1], matrixD[2, 1], matrixD[3, 1]);
                Double scalarProduct = Vector3D.ScalarProduct(directionVector, vectorD);
                Double firstPlayerControlValue = 0;

                //if (scalarProduct > 0)
                if (scalarProduct > m_Epsilon)
                {
                    // ��� firstPlayerControlValue = m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                    firstPlayerControlValue = m_MaxUValue * scaleKoeff;
                }
                //if (scalarProduct < 0)
                if (scalarProduct < -m_Epsilon)
                {
                    // ��� firstPlayerControlValue = -m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                    firstPlayerControlValue = -m_MaxUValue * scaleKoeff;
                }

                Double secondPlayerControlValue = SecondGamerAction(m_TimeValueList[sectionIndex]);

                // �������� ������
                robustControl[sectionIndex] = firstPlayerControlValue;
                secondGamerControlList[sectionIndex] = secondPlayerControlValue;
                scaleKoeffList[sectionIndex] = scaleKoeff;
                nearestBottomBridgeIndicies[sectionIndex] = nearestBottomBridgeIndex;
                checkPointLocationList[sectionIndex] = false;
                // �������� ������

                // ����� ������� ����� �������
                Matrix oldCurrentPoint = Point2MatrixColumn(currentPoint);
                currentPoint = MatrixColumn2Point(oldCurrentPoint + m_DeltaT * (firstPlayerControlValue * matrixD + secondPlayerControlValue * matrixE));
            }
            // ���������� ���������� ���������� �� T - �������� ������ �� ��������� ���������� ������

            timeValueList = m_TimeValueList;
            return robustControl;
        }

        /// <summary>
        /// ����� CalcMaxStableBridgeSystem ������ ��������� ���� � ����� ������� ������
        /// ������ ���� � ���� ������� - ������������ ���������� ���� ��� �������� ������
        /// </summary>
        /// <param name="deltaMq">���������� ��� ��������� ������� ������</param>
        /// <param name="deltaFSKoeff">���������� ������������ �������������� ������������� ���������</param>
        /// <param name="bridgesCount">���������� ������</param>
        /// <returns>��������� ���� � ����� ������� ������</returns>
        public List<PolyhedronClass[]> CalcMaxStableBridgeSystem(Double deltaMq, Double deltaFSKoeff, Int32 bridgesCount)
        {
            // ������������ �������� ���������� ������� ������
            Double mq = m_Mq + deltaMq;
            // ������������ �������������� ������������� ���������
            Double fsKoeff = 1;
            // ������������ ���������
            PolyhedronClass finalSet = m_MaxStableBridge[m_MaxStableBridge.Length - 1];

            // ������� ������ ��� ������� ���������� ����� �� ���������
            Dictionary<String, Object> inputData = new Dictionary<String, Object>();
            inputData["MatrixA"] = m_AMatrix;
            inputData["MatrixB"] = m_BMatrix;
            inputData["MatrixC"] = m_CMatrix;
            inputData["FinalSet"] = null;
            inputData["Mp"] = m_MaxUValue;
            inputData["Mq"] = mq;
            inputData["DeltaT"] = m_DeltaT;
            inputData["MinVectorDistinguishAngle"] = m_MinVectorDistinguishAngle;
            inputData["Epsilon"] = m_Epsilon;

            // ������� ������
            List<PolyhedronClass[]> maxStableBridgesSystem = new List<PolyhedronClass[]>();
            maxStableBridgesSystem.Add(m_MaxStableBridge);

            m_MaxStableBridgeSystemParams = new List<DoublePair>();
            m_MaxStableBridgeSystemParams.Add(new DoublePair(m_Mq, fsKoeff));

            for (Int32 bridgeIndex = 0; bridgeIndex < bridgesCount; /*bridgeIndex++*/)
            {
                // ����������� ������������ ��������� ��� ���������� ����� �� ���������
                fsKoeff += deltaFSKoeff;
                PolyhedronClass scaledFinalSet = finalSet.GetScaledPolyhedron(fsKoeff);
                inputData["FinalSet"] = scaledFinalSet.ToPoint3DArray();

                // ����, ����������� �� ���������� ��������
                PolyhedronClass[] prevCalcBridge = maxStableBridgesSystem[bridgeIndex];

                // �������������� �������� ���������� ������������� ����������� �����
                AlgorithmClass algorithm = new AlgorithmClass(inputData);
                List<PolyhedronClass> maxStableBridge = new List<PolyhedronClass>();

                for (Int32 timePointIndex = 0; timePointIndex < m_TimeValueList.Length; timePointIndex++)
                {
                    PolyhedronClass currentTSection = CreateAndFillPolyhedron(algorithm.GetSideList(), algorithm.GetVertexList());
                    PolyhedronClass prevBridgeTSection = prevCalcBridge[prevCalcBridge.Length - timePointIndex - 1];

                    if (currentTSection.IsPolyhedronInside(prevBridgeTSection))
                    {
                        algorithm.NextSolutionIteration();
                        maxStableBridge.Insert(0, currentTSection);
                    }
                    else
                    {
                        break;
                    }
                }

                // ���� maxStableBridge.Count == m_TimeValueList.Length, �� ������ ���������� ���� ��������� ����� ������ ������������
                if (maxStableBridge.Count == m_TimeValueList.Length)
                {
                    /*Console.WriteLine("mq = {0}, FSKoeff = {1}", mq, fsKoeff);*/

                    maxStableBridgesSystem.Add(maxStableBridge.ToArray());
                    m_MaxStableBridgeSystemParams.Add(new DoublePair(mq, fsKoeff));

                    bridgeIndex++;
                    mq += deltaMq;
                    inputData["Mq"] = mq;
                }
                /*else
                {
                    Console.WriteLine("mq = {0}, FSKoeff = {1} is bad choice", mq, fsKoeff);
                }*/
            }

            return maxStableBridgesSystem;
        }

        public List<DoublePair> MaxStableBridgeSystemParams
        {
            get
            {
                return m_MaxStableBridgeSystemParams;
            }
        }

        /// <summary>
        /// ������������ ���������� ���� �� ��������� ������� [T���, T���]
        /// </summary>
        private PolyhedronClass[] m_MaxStableBridge;
        /// <summary>
        /// ��������� ���� � ����� ������� ������
        /// ������ ���� � ���� ������� - ������������ ���������� ���� ��� �������� ������ (m_MaxStableBridge)
        /// </summary>
        private List<PolyhedronClass[]> m_MaxStableBridgeSystem;
        /// <summary>
        /// 
        /// </summary>
        private List<DoublePair> m_MaxStableBridgeSystemParams;
        /// <summary>
        /// ������ �������� ��������������� ������� ����
        /// </summary>
        private Matrix[] m_FundKoshiMatrixList;
        /// <summary>
        /// ������ �������� �������, � ������� �� �������� T - ������� ������������� ����������� ����� � ��������������� ������� ����
        /// </summary>
        private Double[] m_TimeValueList;
        /// <summary>
        /// ������������ �������� ���������� ������� ������; ����������� = -������������
        /// </summary>
        private Double m_MaxUValue;
        /// <summary>
        /// 
        /// </summary>
        private Double m_Mq;
        /// <summary>
        /// ������������ �������� ���������� ������� ������ (������); ����������� = -������������
        /// </summary>
        private Double m_MaxVValue;
        /// <summary>
        /// 
        /// </summary>
        private Double m_DeltaScale;
        /// <summary>
        /// ������� A �������
        /// </summary>
        private Matrix m_AMatrix;
        /// <summary>
        /// ������� B �������
        /// </summary>
        private Matrix m_BMatrix;
        /// <summary>
        /// ������� C ������� 
        /// </summary>
        private Matrix m_CMatrix;
        /// <summary>
        /// 
        /// </summary>
        private Double m_DeltaT;
        /// <summary>
        /// 
        /// </summary>
        private Double m_MinVectorDistinguishAngle;
        /// <summary>
        /// Epsilon - ...
        /// </summary>
        private Double m_Epsilon;

        private Random m_RG;

        /// <summary>
        /// 
        /// </summary>
        private RobustControlBuilderClass()
        {
            m_RG = new Random();
        }

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
        }

        /// <summary>
        /// 
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
        /// 
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
        /// ����� FillPolyhedron ��������� ������������ polyhedron ������� �� ������ ������ vertexList � ������ sideList
        /// </summary>
        /// <param name="polyhedron">������������</param>
        /// <param name="sideList">������ ������</param>
        /// <param name="vertexList">������ ������</param>
        private static void FillPolyhedron(PolyhedronClass polyhedron, List<SideClass> sideList, List<VertexClass> vertexList)
        {
            // ��������� ������ ������ � �������������
            for (Int32 vertexIndex = 0; vertexIndex < vertexList.Count; vertexIndex++)
            {
                VertexClass currentVertex = vertexList[vertexIndex];
                polyhedron.m_VertexList.Add(new VertexClass(currentVertex.XCoord,
                                                            currentVertex.YCoord,
                                                            currentVertex.ZCoord,
                                                            currentVertex.ID));
            }

            // ��������� ������ ������ � �������������
            for (Int32 sideIndex = 0; sideIndex < sideList.Count; sideIndex++)
            {
                SideClass currentSide = sideList[sideIndex];

                List<VertexClass> sideVertexList = new List<VertexClass>();
                for (Int32 vertexIndex = 0; vertexIndex < currentSide.VertexCount; vertexIndex++)
                {
                    // ID ������� = ������ ������� + 1 ???
                    sideVertexList.Add(vertexList[currentSide[vertexIndex].ID - 1]);
                }

                polyhedron.m_SideList.Add(new SideClass(sideVertexList, currentSide.ID, currentSide.SideNormal));
            }
        }

        /// <summary>
        /// ����� CreateAndFillPolyhedron ������� ������������ � ��������� ��� ������� �� ������ ������ vertexList � ������ sideList
        /// </summary>
        /// <param name="sideList">������ ������</param>
        /// <param name="vertexList">������ ������</param>
        /// <returns>������������</returns>
        private static PolyhedronClass CreateAndFillPolyhedron(List<SideClass> sideList, List<VertexClass> vertexList)
        {
            PolyhedronClass polyhedron = new PolyhedronClass();

            FillPolyhedron(polyhedron, sideList, vertexList);

            return polyhedron;
        }

        /// <summary>
        /// ����� FindNearestBottomBridgeIndex ���������� ������ ���������� ����� ����� ��� �������� ����� position
        /// </summary>
        /// <param name="position">�������� �����</param>
        /// <param name="sectionIndex">������ T - ������� ������������� ����������� �����</param>
        /// <param name="lastNearestBottomBridgeIndex">������ ���������� ����� �����, ���������� � ���������� ���</param>
        /// <returns>������ ���������� ����� ����� ��� �������� ����� position</returns>
        private Int32 FindNearestBottomBridgeIndex(Point3D position, Int32 sectionIndex, Int32 lastNearestBottomBridgeIndex)
        {
            if (m_MaxStableBridgeSystem[0][sectionIndex].IsPointInside(position))
            {
                return -1;
            }

            for (Int32 bridgeIndex = 1; bridgeIndex < m_MaxStableBridgeSystem.Count; bridgeIndex++)
            {
                if (m_MaxStableBridgeSystem[bridgeIndex][sectionIndex].IsPointInside(position))
                {
                    return bridgeIndex - 1;
                }
            }

            return m_MaxStableBridgeSystem.Count - 1;
        }
    }
}
