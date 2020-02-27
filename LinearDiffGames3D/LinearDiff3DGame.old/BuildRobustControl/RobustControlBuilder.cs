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
        /// конструктор класса RobustControlBuilder
        /// </summary>
        /// <param name="inputData">контейнер с входными данными</param>
        public RobustControlBuilder(DataContainer inputData)
        {
            Double startTime = (Double)inputData["StartTime"];
            Double finishTime = (Double)inputData["FinishTime"];
            Double timeInterval = finishTime - startTime;
            m_ApproxComparer = new ApproxComparer((Double)inputData["Epsilon"]);

            // максимальный стабильный мост исходной задачи
            List<ConvexPolyhedron3D> maxStableBridge = new List<ConvexPolyhedron3D>();

            List<Matrix> fundKoshiMatrixList = new List<Matrix>();
            List<Double> timeValueList = new List<Double>();

            // построение максимального стабильного моста исходной задачи
            AlgorithmClass maxStableBridgeTask = new AlgorithmClass(inputData);
            // use GT or GE ???
            while (m_ApproxComparer.GE(timeInterval - maxStableBridgeTask.InverseTime, 0))
            {
                // T - сечение максимального стабильного моста
                maxStableBridge.Insert(0, new ConvexPolyhedron3D(m_ApproxComparer,
                                                                 maxStableBridgeTask.GetSideList(),
                                                                 maxStableBridgeTask.GetVertexList()));
                // значение фундоментальной матрицы Коши в момент времени finishTime - maxStableBridgeTask.InverseTime (???)
                fundKoshiMatrixList.Insert(0, maxStableBridgeTask.GetFundKoshiMatrix().GetCurrentFundKoshiMatrix());
                // значение времени, для которого мы все получаем
                timeValueList.Insert(0, finishTime - maxStableBridgeTask.InverseTime);

                maxStableBridgeTask.NextSolutionIteration();
            }
            // построение максимального стабильного моста исходной задачи

            /*// проверка максимального стабильного моста исходной задачи на выпуклость
            for (Int32 polyhedronIndex = 0; polyhedronIndex < maxStableBridge.Count; polyhedronIndex++)
            {
                if (!maxStableBridge[polyhedronIndex].IsPolyhedronConvex())
                {
                    throw new Exception("polyhedron must be convex !!!");
                }
            }
            // проверка максимального стабильного моста исходной задачи на выпуклость*/

            m_TimeValueList = timeValueList.ToArray();
            m_FundKoshiMatrixList = fundKoshiMatrixList.ToArray();

            // параметры исходной задачи (построения максимального стабильного моста)
            m_MatrixA = inputData["MatrixA"] as Matrix;
            m_MatrixB = inputData["MatrixB"] as Matrix;
            m_MatrixC = inputData["MatrixC"] as Matrix;
            m_DeltaT = (Double)inputData["DeltaT"];
            m_Mp = (Double)inputData["Mp"];
            m_Mq = (Double)inputData["Mq"];
            m_MinVectorDistinguishAngle = (Double)inputData["MinVectorDistinguishAngle"];
            // параметры исходной задачи (построения максимального стабильного моста)

            // параметры задачи построения управления
            m_MaxVValue = (Double)inputData["MaxVValue"];
            m_DeltaScale = (Double)inputData["DeltaScale"];
            // параметры задачи построения управления

            // параметры для построения системы стабильных мостов
            // приращение для множества второго игрока
            Double deltaMq = (Double)inputData["DeltaMq"];
            // приращение коэффициента маштабирования терминального множества
            Double deltaFSKoeff = (Double)inputData["DeltaFSKoeff"];
            // количество мостов
            Int32 bridgesCount = (Int32)inputData["BridgesCount"];
            // параметры для построения системы стабильных мостов

            // считаем систему вложенных друг в друга мостов
            m_StableBridgeSystem = CalcStableBridgeSystem(maxStableBridge, deltaMq, deltaFSKoeff, bridgesCount);

            m_RG = new Random();
        }

        /// <summary>
        /// построение robust - управления для нашей системы
        /// </summary>
        /// <param name="startPoint">начальная точка системы в фазовом пространстве</param>
        /// <returns>контейнер с выходными данными</returns>
        public DataContainer BuildRobustControl(Point3D startPoint)
        {
            // управление первого игрока = robust control
            Double[] firstGamerControl = new Double[m_TimeValueList.Length];
            // управление второго игрока
            Double[] secondGamerControl = new Double[m_TimeValueList.Length];
            // положение системы в фазовом пространстве
            Point3D[] systemPos = new Point3D[m_TimeValueList.Length];
            // критерий, показывающий находиться ли точка systemPos[...] внутри максимального стабильного моста исходной задачи
            // Boolean[] isPosInMainBridge = new Boolean[m_TimeValueList.Length];
            // список ближайших точек
            Point3D[] nearestBridgePoint = new Point3D[m_TimeValueList.Length];
            // список индексов ближайшего снизу моста
            Int32[] nearestBottomBridgeIndexList = new Int32[m_TimeValueList.Length];

            // начальная точка в преобразованном 3-х мерном пространстве
            Point3D currentPoint = MatrixColumn2Point(m_FundKoshiMatrixList[0] * Point2MatrixColumn(startPoint));
            // точка O в преобразованном 3-х мерном пространстве
            Point3D pointO = new Point3D(0, 0, 0);

            // индекс ближайшего снизу моста для текущей точки, либо -1, если точка лежит внутри самого нижнего моста системы
            Int32 nearestBottomBridgeIndex = -1;

            // построение робастного управления на T - сечениях одного из семейства стабильных мостов
            // sectionIndex - индекс T - сечения одного из семейства стабильных мостов
            for (Int32 sectionIndex = 0; sectionIndex < m_TimeValueList.Length; sectionIndex++)
            {
                Matrix matrixD = m_FundKoshiMatrixList[sectionIndex] * m_MatrixB;
                Matrix matrixE = m_FundKoshiMatrixList[sectionIndex] * m_MatrixC;

                // строим индекс ближайшего снизу моста для текущей точки с учетом индекса 
                // ближайшего моста, построенного на предыдущей итерации
                nearestBottomBridgeIndex = FindNearestBottomBridgeIndex(currentPoint, sectionIndex, nearestBottomBridgeIndex);

                // коэффициент маштабирования величины максимального управления первого игрока (0 <= scaleKoeff <= 1)
                Double scaleKoeff;
                // ближайшая точка для текущей на многограннике, на который мы наводимся
                Point3D nearestPoint = new Point3D(0, 0, 0);
                // вектор - направление на ближайшую точку
                Vector3D directionVector = Vector3D.ZeroVector3D;

                // текущая точка находится внутри T-сечения максимального стабильного моста исходной задачи
                if (nearestBottomBridgeIndex == -1)
                {
                    // T-сечение максимального стабильного моста исходной задачи
                    ConvexPolyhedron3D currentTSection = m_StableBridgeSystem[0][sectionIndex];

                    // считаем коэффициент маштабирования величины максимального управления первого игрока
                    if (m_ApproxComparer.E(currentPoint.XCoord, 0) && m_ApproxComparer.E(currentPoint.YCoord, 0) && m_ApproxComparer.E(currentPoint.ZCoord, 0))
                    {
                        scaleKoeff = 0;
                    }
                    else
                    {
                        Point3D crossingPoint = currentTSection.GetCrossingPointWithRay_FullEmun(currentPoint);
                        scaleKoeff = AdvMath.DistanceBetween2Points(currentPoint, pointO) / AdvMath.DistanceBetween2Points(crossingPoint, pointO);
                    }

                    // если scaleKoeff < m_DeltaScale, то управление первого игрока не строим
                    // (чтобы не было ошибок при работе с малыми геометрическими объектами)
                    if (m_ApproxComparer.GT(scaleKoeff - m_DeltaScale, 0))
                    {
                        // многогранник, на который происходит наведение
                        ConvexPolyhedron3D scaledPolyhedron = currentTSection.GetScaledPolyhedron(scaleKoeff - m_DeltaScale);

                        nearestPoint = scaledPolyhedron.GetNearestPoint4Given_FullEmun(currentPoint);
                        directionVector = new Vector3D(nearestPoint.XCoord - currentPoint.XCoord,
                                                       nearestPoint.YCoord - currentPoint.YCoord,
                                                       nearestPoint.ZCoord - currentPoint.ZCoord);
                    }
                }
                // текущая точка находится вне T-сечения максимального стабильного моста исходной задачи
                else
                {
                    scaleKoeff = 1;

                    // многогранник, на который происходит наведение
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
                    // или firstPlayerControlValue = m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                    firstPlayerControlValue = m_Mp * scaleKoeff;
                }
                //if (scalarProduct < 0)
                if (m_ApproxComparer.LT(scalarProduct, 0))
                {
                    // или firstPlayerControlValue = -m_MaxUValue * (scaleKoeff - m_DeltaScale); ???
                    firstPlayerControlValue = -m_Mp * scaleKoeff;
                }

                Double secondPlayerControlValue = SecondGamerAction(m_TimeValueList[sectionIndex]);

                // выходные данные
                firstGamerControl[sectionIndex] = firstPlayerControlValue;
                secondGamerControl[sectionIndex] = secondPlayerControlValue;
                systemPos[sectionIndex] = currentPoint;
                nearestBridgePoint[sectionIndex] = nearestPoint;
                //isPosInMainBridge[sectionIndex] = m_StableBridgeSystem[0][sectionIndex].IsPointInside(currentPoint);
                nearestBottomBridgeIndexList[sectionIndex] = nearestBottomBridgeIndex;
                // выходные данные

                // новая текущая точка системы
                Matrix oldCurrentPoint = Point2MatrixColumn(currentPoint);
                currentPoint = MatrixColumn2Point(oldCurrentPoint + m_DeltaT * (firstPlayerControlValue * matrixD + secondPlayerControlValue * matrixE));
            }
            // построение робастного управления на T - сечениях одного из семейства стабильных мостов

            // контейнер с выходными данными (с данными расчета управления)
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
        /// строим вложенную друг в друга систему стабильных мостов
        /// нижний мост в этой системе - максимальный стабильный мост для исходной задачи
        /// </summary>
        /// <param name="maxStableBridge">максимальный стабильный мост для исходной задачи</param>
        /// <param name="deltaMq">приращение для множества второго игрока</param>
        /// <param name="deltaFSKoeff">приращение коэффициента маштабирования терминального множества</param>
        /// <param name="bridgesCount">количество мостов</param>
        /// <returns>вложенная друг в друга система стабильных мостов</returns>
        private List<ConvexPolyhedron3D[]> CalcStableBridgeSystem(List<ConvexPolyhedron3D> maxStableBridge, Double deltaMq, Double deltaFSKoeff, Int32 bridgesCount)
        {
            // принцип построения вложенной друг в друга системы стабильных мостов:
            // пусть у нас построен уже мост с номером i-1 (мост с номером 0 - максимальный стабильный мост для исходной задачи)
            // необходимо построить i-й мост
            //
            // увеличиваем максимальное значение управления второго игрока, использовавшееся при построении
            // (i-1)-го моста, на величину deltaMq
            //
            // {
            //   увеличиваем коэффициент маштабирования терминального множества максимального стабильного моста
            //   исходной задачи, использовавшееся при построении (i-1)-го моста, на величину deltaFSKoeff
            //
            //   строим с полученными параметрами максимальный стабильный мост
            //
            //   если построенный мост содержит в себе (i-1)-й мост (если все T-сечения (i-1)-го моста лежит внутри T-сечений построенного)
            //   {
            //     построенный мост становится i-м стабильным мостом,
            //   }
            // } (пока не построен i-й стабильный мост)

            // максимальное значение управления второго игрока
            Double mq = m_Mq + deltaMq;
            // коэффициента маштабирования терминального множества
            Double fsKoeff = 1;
            // терминальное множество
            ConvexPolyhedron3D finalSet = maxStableBridge[maxStableBridge.Count - 1];

            // входные данные для расчета очередного моста из семейства
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

            // система мостов
            List<ConvexPolyhedron3D[]> stableBridgesSystem = new List<ConvexPolyhedron3D[]>();
            stableBridgesSystem.Add(maxStableBridge.ToArray());

            //m_MaxStableBridgeSystemParams = new List<DoublePair>();
            //m_MaxStableBridgeSystemParams.Add(new DoublePair(m_Mq, fsKoeff));

            for (Int32 bridgeIndex = 0; bridgeIndex < bridgesCount; )
            {
                // увеличиваем терминальное множество для очередного моста из семейства
                fsKoeff += deltaFSKoeff;
                ConvexPolyhedron3D scaledFinalSet = finalSet.GetScaledPolyhedron(fsKoeff);
                inputData["FinalSet"] = scaledFinalSet.ToPoint3DArray();

                // мост, посчитанный на предыдущей итерации
                ConvexPolyhedron3D[] prevCalcBridge = stableBridgesSystem[bridgeIndex];

                // инициализируем алгоритм построения максимального стабильного моста
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

                // если maxStableBridge.Count == m_TimeValueList.Length, то значит предыдущий мост полностью лежит внутри построенного
                if (maxStableBridge.Count == m_TimeValueList.Length)
                {
                    stableBridgesSystem.Add(stableBridge.ToArray());
                    //m_MaxStableBridgeSystemParams.Add(new DoublePair(mq, fsKoeff));

                    // если смогли построить мост, то увеличиваем максимальное значение управления второго игрока
                    mq += deltaMq;
                    inputData["Mq"] = mq;
                    bridgeIndex++;
                }
            }

            return stableBridgesSystem;
        }

        /// <summary>
        /// преобразователь из типа Point3D в тип Matrix(3, 1)
        /// должен быть не здесь !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
        /// преобразователь из типа Matrix(3, 1) в тип Point3D
        /// должен быть не здесь !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
        /// метод FindNearestBottomBridgeIndex возвращает индекс ближайшего снизу моста для заданной точки position
        /// в случае, если точка лежит внутри максимального стабильного моста исходной задачи (самого нижнего моста) возвращается -1
        /// </summary>
        /// <param name="position">заданная точка</param>
        /// <param name="sectionIndex">индекс T - сечения максимального стабильного моста</param>
        /// <param name="lastNearestBottomBridgeIndex">индекс ближайшего снизу моста, полученный в предыдущий раз</param>
        /// <returns>индекс ближайшего снизу моста для заданной точки position, либо -1, если точка лежит внутри самого нижнего моста</returns>
        private Int32 FindNearestBottomBridgeIndex(Point3D position, Int32 sectionIndex, Int32 lastNearestBottomBridgeIndex)
        {
#warning полный перебор !!!!!!!! нехорошо (((
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

        [Obsolete("надо отсюда убрать !!!")]
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
        /// сравниватель для операций приблизительного сравнения действительных чисел
        /// </summary>
        private ApproxComparer m_ApproxComparer;

        // параметры исходной задачи (построения максимального стабильного моста)
        /// <summary>
        /// матрица A системы
        /// </summary>
        private Matrix m_MatrixA;
        /// <summary>
        /// матрица B системы
        /// </summary>
        private Matrix m_MatrixB;
        /// <summary>
        /// матрица C системы
        /// </summary>
        private Matrix m_MatrixC;
        /// <summary>
        /// описатель множества первого игрока
        /// </summary>
        private Double m_Mp;
        /// <summary>
        /// описатель множества второго игрока
        /// </summary>
        private Double m_Mq;
        /// <summary>
        /// величина дискретизации t - оси
        /// </summary>
        private Double m_DeltaT;
        /// <summary>
        /// минимальное значение угла между векторами, при котором мы считаем два вектора различными (не близкими)
        /// </summary>
        private Double m_MinVectorDistinguishAngle;
        // параметры исходной задачи (построения максимального стабильного моста)

        // параметры задачи построения управления
        /// <summary>
        /// максимальное значение управления второго игрока
        /// </summary>
        private Double m_MaxVValue;
        /// <summary>
        /// ...
        /// </summary>
        private Double m_DeltaScale;
        // параметры задачи построения управления

        // результат работы исходной задачи
        /// <summary>
        /// список дискретных значений на t - оси
        /// </summary>
        private Double[] m_TimeValueList;
        /// <summary>
        /// список значений фундаментальной матрицы Коши
        /// </summary>
        private Matrix[] m_FundKoshiMatrixList;
        /// <summary>
        /// вложенная друг в друга система непересекающихся стабильных мостов
        /// нижний мост в этой системе - максимальный стабильный мост для исходной задачи
        /// </summary>
        private List<ConvexPolyhedron3D[]> m_StableBridgeSystem;
        // результат работы исходной задачи

        private Random m_RG;
    }
}
