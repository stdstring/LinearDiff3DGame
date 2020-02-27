using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
{
    /// <summary>
    /// объект, представляющий пересечение (графа с графом в виде большого круга ...  возможно использование и в общем случае ???)
    /// </summary>
    internal class CrossingObject
    {
        /// <summary>
        /// конструктор класса CrossingObject
        /// </summary>
        /// <param name="crossingType">тип пересечения</param>
        /// <param name="positiveNode">узел номер 1 объекта пересечения (положительный узел для связи; для узла сам узел)</param>
        /// <param name="negativeNode">узел номер 2 объекта пересечения (отрицательный узел для связи; для узла сам узел)</param>
        public CrossingObject(CrossingObjectType crossingType, Polyhedron3DGraphNode positiveNode, Polyhedron3DGraphNode negativeNode)
        {
            CrossingObjectType = crossingType;

            PositiveNode = positiveNode;
            NegativeNode = negativeNode;
            /* здесь должны быть проверки на корректность данных в PositiveNode и в NegativeNode в зависимости от типа объекта пересечения */
        }

        /// <summary>
        /// CrossingObjectType - тип пересечения (узел или связь)
        /// </summary>
        public readonly CrossingObjectType CrossingObjectType;
        /// <summary>
        /// PositiveNode - узел номер 1 объекта пересечения (положительный узел для связи; для узла сам узел)
        /// </summary>
        public readonly Polyhedron3DGraphNode PositiveNode;
        /// <summary>
        /// NegativeNode - узел номер 2 объекта пересечения (отрицательный узел для связи; для узла сам узел)
        /// </summary>
        public readonly Polyhedron3DGraphNode NegativeNode;

        /// <summary>
        /// метод сравнивающий два объекта пересечения (данный и otherCrossingObject)
        /// </summary>
        /// <param name="otherCrossingObject">один из объектов пересечения для сравнения</param>
        /// <returns>true, если два объекта пересечения однаковы; иначе - false</returns>
        public Boolean Equals(CrossingObject otherCrossingObject)
        {
            if (Object.ReferenceEquals(otherCrossingObject, null))
            {
                return false;
            }

            if (this.CrossingObjectType != otherCrossingObject.CrossingObjectType)
            {
                return false;
            }

            return (Object.ReferenceEquals(this.PositiveNode, otherCrossingObject.PositiveNode) &&
                    Object.ReferenceEquals(this.NegativeNode, otherCrossingObject.NegativeNode));
        }

        /// <summary>
        /// переопределение метода Object.Equals
        /// </summary>
        /// <param name="obj">объект для сравнения</param>
        /// <returns>true, если данный и obj объекты одинаковы; иначе - false</returns>
        public override Boolean Equals(Object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            CrossingObject otherCrossingObject = (obj as CrossingObject);
            return Equals(otherCrossingObject);
        }

        /// <summary>
        /// переопределение метода Object.GetHashCode
        /// </summary>
        /// <returns>хэш-код данного объекта</returns>
        public override Int32 GetHashCode()
        {
#warning need more effective realization !!!
            return base.GetHashCode();
        }

        /// <summary>
        /// оператор равенства двух объектов пересечения
        /// </summary>
        /// <param name="crossingObject1">1-й объект пересечения</param>
        /// <param name="crossingObject2">2-й объект пересечения</param>
        /// <returns>true, если два объекта пересечения равны; иначе - false</returns>
        public static Boolean operator ==(CrossingObject crossingObject1, CrossingObject crossingObject2)
        {
            if (Object.ReferenceEquals(crossingObject1, null))
            {
                return Object.ReferenceEquals(crossingObject2, null);
            }

            return crossingObject1.Equals(crossingObject2);
        }

        /// <summary>
        /// оператор неравенства двух объектов пересечения
        /// </summary>
        /// <param name="crossingObject1">1-й объект пересечения</param>
        /// <param name="crossingObject2">2-й объект пересечения</param>
        /// <returns>true, если два объекта пересечения неравны; иначе - false</returns>
        public static Boolean operator !=(CrossingObject crossingObject1, CrossingObject crossingObject2)
        {
            if (Object.ReferenceEquals(crossingObject1, null))
            {
                return !Object.ReferenceEquals(crossingObject2, null);
            }

            return !crossingObject1.Equals(crossingObject2);
        }
    }
}
