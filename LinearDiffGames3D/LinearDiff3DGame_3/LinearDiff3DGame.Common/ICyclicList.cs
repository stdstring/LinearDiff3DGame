using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Common
{
    /// <summary>
    /// ����������� ������ ��������� ���� T (������� generic-���������� IList)
    /// </summary>
    /// <typeparam name="T">��� ���������, ������������ � ������</typeparam>
    public interface ICyclicList<T> : IList<T>
    {
        /// <summary>
        /// ���������� ������ ��������, ��������� ������������ �������� �� �������� shiftValue (� ������ ����������� ������)
        /// </summary>
        /// <param name="currentItemIndex">������ �������� ��������</param>
        /// <param name="shiftValue">�������� ������</param>
        /// <returns>������ ���������� �������� � ����������� ������</returns>
        Int32 GetItemIndex(Int32 currentItemIndex, Int32 shiftValue);
        /// <summary>
        /// ���������� ������ ��������, ��������� ������������ �������� �� �������� shiftValue (� ������ ����������� ������)
        /// </summary>
        /// <param name="currentItem">������� �������</param>
        /// <param name="shiftValue">�������� ������</param>
        /// <returns>������ ���������� �������� � ����������� ������</returns>
        Int32 GetItemIndex(T currentItem, Int32 shiftValue);

        /// <summary>
        /// ���������� �������, ��������� ������������ �������� �� �������� shiftValue (� ������ ����������� ������)
        /// </summary>
        /// <param name="currentItemIndex">������ �������� ��������</param>
        /// <param name="shiftValue">�������� ������</param>
        /// <returns>��������� ������� �� ����������� ������</returns>
        T GetItem(Int32 currentItemIndex, Int32 shiftValue);
        /// <summary>
        /// ���������� �������, ��������� ������������ �������� �� �������� shiftValue (� ������ ����������� ������)
        /// </summary>
        /// <param name="currentItem">������� �������</param>
        /// <param name="shiftValue">�������� ������</param>
        /// <returns>��������� �������� �� ����������� ������</returns>
        T GetItem(T currentItem, Int32 shiftValue);

        /// <summary>
        /// ���������� ������ ���������� �������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="currentItemIndex">������ �������� ��������</param>
        /// <returns>������ ���������� �������� � ����������� ������</returns>
        Int32 GetNextItemIndex(Int32 currentItemIndex);
        /// <summary>
        /// ���������� ������ ���������� �������� � ����������� ������ �� ��������� �������� ��������
        /// </summary>
        /// <param name="currentItem">������� �������</param>
        /// <returns>������ ���������� �������� � ����������� ������</returns>
        Int32 GetNextItemIndex(T currentItem);

        /// <summary>
        /// ���������� ��������� ������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="currentItemIndex">������ �������� ��������</param>
        /// <returns>��������� ������� � ����������� ������</returns>
        T GetNextItem(Int32 currentItemIndex);
        /// <summary>
        /// ���������� ��������� ������� � ����������� ������ �� ��������� �������� ��������
        /// </summary>
        /// <param name="currentItem">������� �������</param>
        /// <returns>��������� ������� � ����������� ������</returns>
        T GetNextItem(T currentItem);

        /// <summary>
        /// ���������� ������ ����������� �������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="currentItemIndex">������ �������� ��������</param>
        /// <returns>������ ����������� �������� � ����������� ������</returns>
        Int32 GetPrevItemIndex(Int32 currentItemIndex);
        /// <summary>
        /// ���������� ������ ����������� �������� � ����������� ������ �� ��������� �������� ��������
        /// </summary>
        /// <param name="currentItem">������� �������</param>
        /// <returns>������ ����������� �������� � ����������� ������</returns>
        Int32 GetPrevItemIndex(T currentItem);

        /// <summary>
        /// ���������� ���������� ������� � ����������� ������ �� ��������� ������� �������� ��������
        /// </summary>
        /// <param name="currentItemIndex">������ �������� ��������</param>
        /// <returns>���������� ������� � ����������� ������</returns>
        T GetPrevItem(Int32 currentItemIndex);
        /// <summary>
        /// ���������� ���������� ������� � ����������� ������ �� ��������� �������� ��������
        /// </summary>
        /// <param name="currentItem">������� �������</param>
        /// <returns>���������� ������� � ����������� ������</returns>
        T GetPrevItem(T currentItem);
    }
}
