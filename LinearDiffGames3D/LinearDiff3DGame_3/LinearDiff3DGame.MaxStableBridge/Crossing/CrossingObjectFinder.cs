using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// класс для нахождения пересечений графа с G(...Xi...), где Xi - некий отрезок, G(...Xi...) - большой круг, перпендикулярный Xi и проходящий через точку 0
    /// </summary>
    internal class CrossingObjectFinder
    {
        /// <summary>
        /// конструктор класса CrossingObjectFinder
        /// </summary>
        /// <param name="approxComparer">сравниватель, для приближенного сравнения действительных чисел</param>
        public CrossingObjectFinder(ApproxComp approxComparer)
        {
            m_ApproxComparer = approxComparer;
        }

        /// <summary>
        /// метод GetFirstCrossingObject возвращает первый объект пересечения графа с G(...Xi...)
        /// </summary>
        /// <param name="startNode">узел графа, с которого начинается поиск</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>первый объект пересечения графа с G(...Xi...)</returns>
        public CrossingObject GetFirstCrossingObject(Polyhedron3DGraphNode startNode, Vector3D directingVectorXi)
        {
            CrossingObject firstCrossingObject = null;

            // текущий узел
            Polyhedron3DGraphNode currentNode = startNode;
            // вычисляем скалярное произведение вектора, связанного с текущим узлом, и направляющего вектора отрезка Xi
            Double currentScalarProduct = Vector3D.ScalarProduct(currentNode.NodeNormal, directingVectorXi);
            // если скалярное произведение = 0, то текущий узел становится искомым объектом
            if (m_ApproxComparer.EQ(currentScalarProduct, 0))
            {
                firstCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, currentNode, currentNode);
            }

            // цикл пока не найден искомый первый объект пересечения
            while (Object.ReferenceEquals(firstCrossingObject, null))
            {
                Double bestScalarProduct = Double.NaN;
                Polyhedron3DGraphNode bestNode = null;

                // цикл по всем связям текущего узла
                for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
                {
                    // текущая связь текущего узла
                    Polyhedron3DGraphNode currentConnNode = currentNode.ConnectionList[connIndex];
                    // считаем скалярное произведение вектора, связанного с полученным выше узлом, и направляющего вектора отрезка Xi
                    Double scalarProduct = Vector3D.ScalarProduct(currentConnNode.NodeNormal, directingVectorXi);

                    // если скалярное произведение = 0, то полученный узел становится искомым объектом
                    if (m_ApproxComparer.EQ(scalarProduct, 0))
                    {
                        firstCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, currentConnNode, currentConnNode);
                        break;
                    }

                    // если скалярное произведение того же знака, что и для вектора, связанного с текущим узлом, то
                    // если по абсолютному значению величина скалярного произведения меньше запомненного, то запоминаем величину и полученный узел
                    if (Math.Sign(currentScalarProduct) == Math.Sign(scalarProduct))
                    {
                        if (Double.IsNaN(bestScalarProduct) ||
                            (Math.Abs(scalarProduct) < Math.Abs(bestScalarProduct)))
                        {
                            bestScalarProduct = scalarProduct;
                            bestNode = currentConnNode;
                        }
                    }
                    // если знак скалярного произведения для полученного (выше) вектора, отличается от знака скалярного произведения для вектора, связанного с текущим узлом, то
                    // связь, соединяющая текущий и полученный (выше) узлы становится искомым объектом
                    else
                    {
                        Polyhedron3DGraphNode plusNode = (currentScalarProduct > 0 ? currentNode : currentConnNode);
                        Polyhedron3DGraphNode minusNode = (currentScalarProduct < 0 ? currentNode : currentConnNode);

                        firstCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, minusNode);
                        break;
                    }
                }

                // текущим узлом становится запомненный узел
                currentNode = bestNode;
                currentScalarProduct = bestScalarProduct;
            }

            return firstCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject возвращает следующий по направлению движения объект пересечения
        /// при этом текущий объект пересечения currentCrossingObject может быть "испорчен" из-за наличия 
        /// внутри него узла (если текущий объект пересечения - связь) currentCrossingNode на пересечении объекта и G(...Xi...)
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>следующий по направлению движения объект пересечения</returns>
        public CrossingObject GetNextCrossingObject(CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // если текущий объект – узел
            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                nextCrossingObject = GetNextCrossingObject4GraphNode(currentCrossingObject, directingVectorXi);
            }
            // если текущий объект – связь
            else
            {
                nextCrossingObject = GetNextCrossingObject4GraphConn(currentCrossingObject, currentCrossingNode, directingVectorXi);
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject возвращает следующий по направлению движения объект пересечения
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>следующий по направлению движения объект пересечения</returns>
        public CrossingObject GetNextCrossingObject(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // если текущий объект – узел
            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphNode)
            {
                nextCrossingObject = GetNextCrossingObject4GraphNode(currentCrossingObject, directingVectorXi);
            }
            // если текущий объект – связь
            else
            {
                nextCrossingObject = GetNextCrossingObject4GraphConn(currentCrossingObject, directingVectorXi);
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject4GraphNode возвращает следующий по направлению движения объект пересечения, если текущий - узел
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>следующий по направлению движения объект пересечения</returns>
        private CrossingObject GetNextCrossingObject4GraphNode(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            Polyhedron3DGraphNode currentNode = currentCrossingObject.PositiveNode;
            // цикл по всем связям текущего узла
            for (Int32 connIndex = 0; connIndex < currentNode.ConnectionList.Count; ++connIndex)
            {
                // получаем узел (номер 1), связанный с текущим узлом текущей связью
                Polyhedron3DGraphNode node1 = currentNode.ConnectionList[connIndex];
                // получаем узел (номер 2), связанный с текущим узлом предыдущей связью
                Polyhedron3DGraphNode node2 = currentNode.ConnectionList.GetPrevItem(connIndex);

                // вычисляем скалярное произведение вектора 1 и направляющего вектора Xi
                Double scalarProduct1 = Vector3D.ScalarProduct(node1.NodeNormal, directingVectorXi);
                // вычисляем скалярное произведение вектора 2 и направляющего вектора Xi
                Double scalarProduct2 = Vector3D.ScalarProduct(node2.NodeNormal, directingVectorXi);

                // если скалярное произведение узла 1 и направляющего вектора Xi == 0
                if (m_ApproxComparer.EQ(scalarProduct1, 0))
                {
                    // если направление движения выбрано правильно, то узел номер 1 становится следующим по движению объектом
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, node1, node1);
                    if (CheckMoveDirection(nextCrossingObject, currentCrossingObject, directingVectorXi))
                    {
                        break;
                    }
                    else
                    {
                        nextCrossingObject = null;
                    }
                }

                // если скалярное произведение узла 2 и направляющего вектора Xi == 0
                if (m_ApproxComparer.EQ(scalarProduct2, 0))
                {
                    // если направление движения выбрано правильно, то узел номер 2 становится следующим по движению объектом
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, node2, node2);
                    if (CheckMoveDirection(nextCrossingObject, currentCrossingObject, directingVectorXi))
                    {
                        break;
                    }
                    else
                    {
                        nextCrossingObject = null;
                    }
                }

                // если скалярные произведения узлов 1 и 2 и направляющего вектора Xi имеют разный знак
                if (Math.Sign(scalarProduct1) != Math.Sign(scalarProduct2))
                {
                    // если направление движения выбрано правильно, то связь, соединяющая узлы 1 и 2, становится следующим по движению объектом
                    Polyhedron3DGraphNode plusNode = (scalarProduct1 > 0 ? node1 : node2);
                    Polyhedron3DGraphNode minusNode = (scalarProduct1 < 0 ? node1 : node2);
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, minusNode);
                    if (CheckMoveDirection(nextCrossingObject, currentCrossingObject, directingVectorXi))
                    {
                        break;
                    }
                    else
                    {
                        nextCrossingObject = null;
                    }
                }
            }
            // цикл по всем связям текущего узла

            return nextCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject4GraphConn возвращает следующий по направлению движения объект пересечения, если текущий - связь
        /// при этом текущий объект пересечения currentCrossingObject может быть "испорчен" из-за наличия 
        /// внутри него узла currentCrossingNode на пересечении объекта и G(...Xi...)
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="currentCrossingNode">узел на текущем пересечении графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>следующий по направлению движения объект пересечения</returns>
        private CrossingObject GetNextCrossingObject4GraphConn(CrossingObject currentCrossingObject, Polyhedron3DGraphNode currentCrossingNode, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // положительный узел текущей связи
            Polyhedron3DGraphNode plusNode = currentCrossingObject.PositiveNode;
            // отрицательный узел текущей связи
            Polyhedron3DGraphNode minusNode = currentCrossingObject.NegativeNode;

            // для положительного узла (currentCrossingObject.PositiveNode) берем следующую связь (относительно текущей)
            Polyhedron3DGraphNode nextNode1 = plusNode.ConnectionList.GetNextItem(currentCrossingNode);
            // для отрицательного узла (currentCrossingObject.NegativeNode) берем предыдущую связь (относительно текущей)
            Polyhedron3DGraphNode nextNode2 = minusNode.ConnectionList.GetPrevItem(currentCrossingNode);

            Double scalarProduct1 = Vector3D.ScalarProduct(nextNode1.NodeNormal, directingVectorXi);
            Double scalarProduct2 = Vector3D.ScalarProduct(nextNode2.NodeNormal, directingVectorXi);

            // если полученный узел (номер 1) нулевой
            if (m_ApproxComparer.EQ(scalarProduct1, 0))
            {
                // если полученный узел номер 2 нулевой
                if (m_ApproxComparer.EQ(scalarProduct2, 0))
                {
                    // полученный узел становится следующим по движению объектом
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, nextNode1, nextNode1);
                    // exit
                }
                // если полученный узел номер 2 ненулевой
                else
                {
                    // "связь" (это связь, с которой мы начинали движение; реально ее уже нет) соединяющая положительный узел и узел номер 2 становится следующим по движению объектом
                    // Связи с которой мы начинали движение нет из-за того, что на первом шаге между узлами связи был построен узел на пересечении этой связи и G(...Pi...)
                    nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, nextNode2);
                    // exit
                }
            }
            // если полученный узел (номер 1) положительный
            else if (m_ApproxComparer.GT(scalarProduct1, 0))
            {
                // связь, соединяющая новый положительный узел и старый отрицательный узел, становится следующим по движению объектом
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, nextNode1, minusNode);
                // exit
            }
            // если полученный узел (номер 1) отрицательный
            else if (m_ApproxComparer.LT(scalarProduct1, 0))
            {
                // связь, соединяющая новый отрицательный узел и старый положительный узел, становится следующим по движению объектом
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, nextNode2);
                // exit
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// метод GetNextCrossingObject4GraphConn возвращает следующий по направлению движения объект пересечения, если текущий - связь
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>следующий по направлению движения объект пересечения</returns>
        private CrossingObject GetNextCrossingObject4GraphConn(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            CrossingObject nextCrossingObject = null;

            // положительный узел текущей связи
            Polyhedron3DGraphNode plusNode = currentCrossingObject.PositiveNode;
            // отрицательный узел текущей связи
            Polyhedron3DGraphNode minusNode = currentCrossingObject.NegativeNode;

            // для положительного узла (CurrentCrossingObject.PositiveNode) берем следующую связь (относительно текущей)
            Polyhedron3DGraphNode nextNode1 = plusNode.ConnectionList.GetNextItem(minusNode);
            // для отрицательного узла (CurrentCrossingObject.NegativeNode) берем предыдущую связь (относительно текущей)
            Polyhedron3DGraphNode nextNode2 = minusNode.ConnectionList.GetPrevItem(plusNode);

            Double scalarProduct1 = Vector3D.ScalarProduct(nextNode1.NodeNormal, directingVectorXi);
            Double scalarProduct2 = Vector3D.ScalarProduct(nextNode2.NodeNormal, directingVectorXi);

            // если полученный узел (номер 1) нулевой
            if (m_ApproxComparer.EQ(scalarProduct1, 0))
            {
                // если полученный узел номер 2 не нулевой
                if (m_ApproxComparer.NE(scalarProduct2, 0))
                {
#warning может более специализированное исключение
                    throw new Exception("GetNextCrossingObject4GraphConn2 method incorrect work");
                }

                // полученный узел становится следующим по движению объектом
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphNode, nextNode1, nextNode1);
                // exit
            }
            // если полученный узел (номер 1) положительный
            else if (m_ApproxComparer.GT(scalarProduct1, 0))
            {
                // связь, соединяющая новый положительный узел и старый отрицательный узел, становится следующим по движению объектом
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, nextNode1, minusNode);
                // exit
            }
            // если полученный узел (номер 1) отрицательный
            else if (m_ApproxComparer.LT(scalarProduct1, 0))
            {
                // связь, соединяющая новый отрицательный узел и старый положительный узел, становится следующим по движению объектом
                nextCrossingObject = new CrossingObject(CrossingObjectType.GraphConnection, plusNode, nextNode2);
                // exit
            }

            return nextCrossingObject;
        }

        /// <summary>
        /// метод CalcCrossingNodeNormal вычисляет нормаль узла на текущем пересечении графа с G(...Xi...)
        /// Xi - некий отрезок, G(...Xi...) - большой круг, перпендикулярный Xi и проходящий через точку 0
        /// </summary>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>нормаль узла на текущем пересечении графа с G(...Xi...)</returns>
        private Vector3D CalcCrossingNodeNormal(CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            Vector3D crossingNodeNormal = Vector3D.ZeroVector3D;

            if (currentCrossingObject.CrossingObjectType == CrossingObjectType.GraphConnection)
            {
                Vector3D plusVector = currentCrossingObject.PositiveNode.NodeNormal;
                Vector3D minusVector = currentCrossingObject.NegativeNode.NodeNormal;
                // Строим вектор, перпендикулярный векторам, связанным текущей связью,
                // как векторное произведение положительного узла связи на отрицательный
                Vector3D npm = Vector3D.VectorProduct(plusVector, minusVector);
                // Вычисляем векторное произведение построенного вектора и направляющего вектора Xi
                crossingNodeNormal = Vector3D.VectorProduct(npm, directingVectorXi);
                crossingNodeNormal.Normalize();
            }
            else
            {
                crossingNodeNormal = currentCrossingObject.PositiveNode.NodeNormal;
            }

            return crossingNodeNormal;
        }

        /// <summary>
        /// метод CheckMoveDirection возвращает true, если направление движения по G(...Xi...) правильное, иначе возвращается false
        /// правильным считается направление движения против часовой стрелки, если смотреть с конца направляющего вектора Xi
        /// </summary>
        /// <param name="checkCrossingObject">проверяемый объект пересечения графа с G(...Xi...)</param>
        /// <param name="currentCrossingObject">текущий объект пересечения графа с G(...Xi...)</param>
        /// <param name="directingVectorXi">направляющий вектор отрезка Xi</param>
        /// <returns>true, если при построении объектов пересечения мы двигаемся правильное; иначе - false</returns>
        private Boolean CheckMoveDirection(CrossingObject checkCrossingObject, CrossingObject currentCrossingObject, Vector3D directingVectorXi)
        {
            // направляющий вектор Xi становится ортом оси OZ
            Vector3D directingVectorOZ = directingVectorXi;

            // строим пересечение текущего объекта и G(...Xi...); вектор, полученный при построении пересечения, становится ортом оси OX
            Vector3D directingVectorOX = CalcCrossingNodeNormal(currentCrossingObject, directingVectorXi);

            // строим орт оси OY правой СК XYZ (как векторное произведение орта оси OZ на орт оси OX)
            Vector3D directingVectorOY = Vector3D.VectorProduct(directingVectorOZ, directingVectorOX);

            // строим пересечение проверяемого объекта и G(...Xi...); вычисляем скалярное произведение вектора, полученного при построении пересечения, и орта оси OY
            Vector3D checkVector = CalcCrossingNodeNormal(checkCrossingObject, directingVectorXi);
            Double scalarProduct = Vector3D.ScalarProduct(checkVector, directingVectorOY);

            // если ScalarProductValue = 0 - это ошибка работы алгоритма
            if (m_ApproxComparer.EQ(scalarProduct, 0))
            {
#warning может более специализированное исключение
                throw new Exception("CheckMoveDirection method incorrect work");
            }

            // если вычисленное скалярное произведение > 0, то направление движения правильное, иначе направление движения неправильное
            return (scalarProduct > 0 ? true : false);
        }

        /// <summary>
        /// сравниватель, для приближенного сравнения действительных чисел
        /// </summary>
        private ApproxComp m_ApproxComparer;
    }
}
