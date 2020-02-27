using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Common
{
    /// <summary>
    /// циклический список элементов типа T (потомок generic-интерфейса IList)
    /// </summary>
    /// <typeparam name="T">тип элементов, содержащихся в списке</typeparam>
    public interface ICyclicList<T> : IList<T>
    {
        /// <summary>
        /// возвращает индекс элемента, сдвинутый относительно текущего на величину shiftValue (с учетом цикличности списка)
        /// </summary>
        /// <param name="currentItemIndex">индекс текущего элемента</param>
        /// <param name="shiftValue">величина сдвига</param>
        /// <returns>индекс сдвинутого элемента в циклическом списке</returns>
        Int32 GetItemIndex(Int32 currentItemIndex, Int32 shiftValue);
        /// <summary>
        /// возвращает индекс элемента, сдвинутый относительно текущего на величину shiftValue (с учетом цикличности списка)
        /// </summary>
        /// <param name="currentItem">текущий элемент</param>
        /// <param name="shiftValue">величина сдвига</param>
        /// <returns>индекс сдвинутого элемента в циклическом списке</returns>
        Int32 GetItemIndex(T currentItem, Int32 shiftValue);

        /// <summary>
        /// возвращает элемент, сдвинутый относительно текущего на величину shiftValue (с учетом цикличности списка)
        /// </summary>
        /// <param name="currentItemIndex">индекс текущего элемента</param>
        /// <param name="shiftValue">величина сдвига</param>
        /// <returns>сдвинутый элемент из циклическом списке</returns>
        T GetItem(Int32 currentItemIndex, Int32 shiftValue);
        /// <summary>
        /// возвращает элемент, сдвинутый относительно текущего на величину shiftValue (с учетом цикличности списка)
        /// </summary>
        /// <param name="currentItem">текущий элемент</param>
        /// <param name="shiftValue">величина сдвига</param>
        /// <returns>сдвинутый элемента из циклическом списке</returns>
        T GetItem(T currentItem, Int32 shiftValue);

        /// <summary>
        /// возвращает индекс следующего элемента в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="currentItemIndex">индекс текущего элемента</param>
        /// <returns>индекс следующего элемента в циклическом списке</returns>
        Int32 GetNextItemIndex(Int32 currentItemIndex);
        /// <summary>
        /// возвращает индекс следующего элемента в циклическом списке по заданному текущему элементу
        /// </summary>
        /// <param name="currentItem">текущий элемент</param>
        /// <returns>индекс следующего элемента в циклическом списке</returns>
        Int32 GetNextItemIndex(T currentItem);

        /// <summary>
        /// возвращает следующей элемент в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="currentItemIndex">индекс текущего элемента</param>
        /// <returns>следующей элемент в циклическом списке</returns>
        T GetNextItem(Int32 currentItemIndex);
        /// <summary>
        /// возвращает следующей элемент в циклическом списке по заданному текущему элементу
        /// </summary>
        /// <param name="currentItem">текущий элемент</param>
        /// <returns>следующей элемент в циклическом списке</returns>
        T GetNextItem(T currentItem);

        /// <summary>
        /// возвращает индекс предыдущего элемента в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="currentItemIndex">индекс текущего элемента</param>
        /// <returns>индекс предыдущего элемента в циклическом списке</returns>
        Int32 GetPrevItemIndex(Int32 currentItemIndex);
        /// <summary>
        /// возвращает индекс предыдущего элемента в циклическом списке по заданному текущему элементу
        /// </summary>
        /// <param name="currentItem">текущий элемент</param>
        /// <returns>индекс предыдущего элемента в циклическом списке</returns>
        Int32 GetPrevItemIndex(T currentItem);

        /// <summary>
        /// возвращает предыдущий элемент в циклическом списке по заданному индексу текущего элемента
        /// </summary>
        /// <param name="currentItemIndex">индекс текущего элемента</param>
        /// <returns>предыдущий элемент в циклическом списке</returns>
        T GetPrevItem(Int32 currentItemIndex);
        /// <summary>
        /// возвращает предыдущий элемент в циклическом списке по заданному текущему элементу
        /// </summary>
        /// <param name="currentItem">текущий элемент</param>
        /// <returns>предыдущий элемент в циклическом списке</returns>
        T GetPrevItem(T currentItem);
    }
}
