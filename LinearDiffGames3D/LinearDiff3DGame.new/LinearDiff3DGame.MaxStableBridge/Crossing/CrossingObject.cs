using System;
using LinearDiff3DGame.Geometry3D.PolyhedronGraph;

namespace LinearDiff3DGame.MaxStableBridge.Crossing
{
    /// <summary>
    /// ������, �������������� ����������� (����� � ������ � ���� �������� ����� ...  �������� ������������� � � ����� ������ ???)
    /// </summary>
    internal class CrossingObject
    {
        /// <summary>
        /// ����������� ������ CrossingObject
        /// </summary>
        /// <param name="crossingType">��� �����������</param>
        /// <param name="positiveNode">���� ����� 1 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)</param>
        /// <param name="negativeNode">���� ����� 2 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)</param>
        public CrossingObject(CrossingObjectType crossingType,
                              IPolyhedron3DGraphNode positiveNode,
                              IPolyhedron3DGraphNode negativeNode)
        {
            CrossingObjectType = crossingType;
            PositiveNode = positiveNode;
            NegativeNode = negativeNode;
        }

        /// <summary>
        /// CrossingObjectType - ��� ����������� (���� ��� �����)
        /// </summary>
        public readonly CrossingObjectType CrossingObjectType;

        /// <summary>
        /// PositiveNode - ���� ����� 1 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)
        /// </summary>
        public readonly IPolyhedron3DGraphNode PositiveNode;

        /// <summary>
        /// NegativeNode - ���� ����� 2 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)
        /// </summary>
        public readonly IPolyhedron3DGraphNode NegativeNode;

        /// <summary>
        /// ����� ������������ ��� ������� ����������� (������ � otherCrossingObject)
        /// </summary>
        /// <param name="otherCrossingObject">���� �� �������� ����������� ��� ���������</param>
        /// <returns>true, ���� ��� ������� ����������� ��������; ����� - false</returns>
        public Boolean Equals(CrossingObject otherCrossingObject)
        {
            if (ReferenceEquals(otherCrossingObject, null))
            {
                return false;
            }

            if (CrossingObjectType != otherCrossingObject.CrossingObjectType)
            {
                return false;
            }

            return (ReferenceEquals(PositiveNode, otherCrossingObject.PositiveNode) &&
                    ReferenceEquals(NegativeNode, otherCrossingObject.NegativeNode));
        }

        /// <summary>
        /// ��������������� ������ Object.Equals
        /// </summary>
        /// <param name="obj">������ ��� ���������</param>
        /// <returns>true, ���� ������ � obj ������� ���������; ����� - false</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals((CrossingObject) obj);
        }

        /// <summary>
        /// ��������������� ������ Object.GetHashCode
        /// </summary>
        /// <returns>���-��� ������� �������</returns>
        public override Int32 GetHashCode()
        {
#warning need more effective realization !!!
            return base.GetHashCode();
        }

        /// <summary>
        /// �������� ��������� ���� �������� �����������
        /// </summary>
        /// <param name="crossingObject1">1-� ������ �����������</param>
        /// <param name="crossingObject2">2-� ������ �����������</param>
        /// <returns>true, ���� ��� ������� ����������� �����; ����� - false</returns>
        public static Boolean operator ==(CrossingObject crossingObject1, CrossingObject crossingObject2)
        {
            if (ReferenceEquals(crossingObject1, null))
            {
                return ReferenceEquals(crossingObject2, null);
            }

            return crossingObject1.Equals(crossingObject2);
        }

        /// <summary>
        /// �������� ����������� ���� �������� �����������
        /// </summary>
        /// <param name="crossingObject1">1-� ������ �����������</param>
        /// <param name="crossingObject2">2-� ������ �����������</param>
        /// <returns>true, ���� ��� ������� ����������� �������; ����� - false</returns>
        public static Boolean operator !=(CrossingObject crossingObject1, CrossingObject crossingObject2)
        {
            if (ReferenceEquals(crossingObject1, null))
            {
                return !ReferenceEquals(crossingObject2, null);
            }

            return !crossingObject1.Equals(crossingObject2);
        }
    }
}