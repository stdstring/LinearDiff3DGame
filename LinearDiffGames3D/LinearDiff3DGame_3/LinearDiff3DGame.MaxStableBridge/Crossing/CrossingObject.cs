using System;
using System.Collections.Generic;
using System.Text;

using LinearDiff3DGame.Geometry3D;

namespace LinearDiff3DGame.MaxStableBridge
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
        public CrossingObject(CrossingObjectType crossingType, Polyhedron3DGraphNode positiveNode, Polyhedron3DGraphNode negativeNode)
        {
            CrossingObjectType = crossingType;

            PositiveNode = positiveNode;
            NegativeNode = negativeNode;
            /* ����� ������ ���� �������� �� ������������ ������ � PositiveNode � � NegativeNode � ����������� �� ���� ������� ����������� */
        }

        /// <summary>
        /// CrossingObjectType - ��� ����������� (���� ��� �����)
        /// </summary>
        public readonly CrossingObjectType CrossingObjectType;
        /// <summary>
        /// PositiveNode - ���� ����� 1 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)
        /// </summary>
        public readonly Polyhedron3DGraphNode PositiveNode;
        /// <summary>
        /// NegativeNode - ���� ����� 2 ������� ����������� (������������� ���� ��� �����; ��� ���� ��� ����)
        /// </summary>
        public readonly Polyhedron3DGraphNode NegativeNode;

        /// <summary>
        /// ����� ������������ ��� ������� ����������� (������ � otherCrossingObject)
        /// </summary>
        /// <param name="otherCrossingObject">���� �� �������� ����������� ��� ���������</param>
        /// <returns>true, ���� ��� ������� ����������� ��������; ����� - false</returns>
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
        /// ��������������� ������ Object.Equals
        /// </summary>
        /// <param name="obj">������ ��� ���������</param>
        /// <returns>true, ���� ������ � obj ������� ���������; ����� - false</returns>
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
            if (Object.ReferenceEquals(crossingObject1, null))
            {
                return Object.ReferenceEquals(crossingObject2, null);
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
            if (Object.ReferenceEquals(crossingObject1, null))
            {
                return !Object.ReferenceEquals(crossingObject2, null);
            }

            return !crossingObject1.Equals(crossingObject2);
        }
    }
}
