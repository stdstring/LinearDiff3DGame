using System;
using System.Collections.Generic;
using System.Text;

namespace LinearDiff3DGame.Common
{
    /// <summary>
    /// циклический список элементов типа T
    /// </summary>
    /// <typeparam name="T">тип элементов, содержащихся в списке</typeparam>
    public class CyclicList<T> : List<T>, ICyclicList<T>
    {
        #region ICyclicList<T> Members

        public int GetItemIndex(int currentItemIndex, int shiftValue)
        {
            if (currentItemIndex < 0 || currentItemIndex >= this.Count)
            {
#warning может более специализированное исключение
                throw new ArgumentOutOfRangeException("Item's index must be between 0 and Count-1");
            }

            if (shiftValue <= -this.Count || shiftValue >= this.Count)
            {
#warning может более специализированное исключение
                throw new ArgumentOutOfRangeException("Item's index must be between -(Count-1) and Count-1");
            }

            if (shiftValue >= 0)
            {
                return (currentItemIndex + shiftValue) % this.Count;
            }
            else
            {
                return (currentItemIndex + shiftValue + this.Count) % this.Count;
            }
        }

        public int GetItemIndex(T currentItem, int shiftValue)
        {
            return GetItemIndex(this.IndexOf(currentItem), shiftValue);
        }

        public T GetItem(int currentItemIndex, int shiftValue)
        {
            return this[GetItemIndex(currentItemIndex, shiftValue)];
        }

        public T GetItem(T currentItem, int shiftValue)
        {
            return this[GetItemIndex(currentItem, shiftValue)];
        }

        public int GetNextItemIndex(int currentItemIndex)
        {
            return GetItemIndex(currentItemIndex, 1);
        }

        public int GetNextItemIndex(T currentItem)
        {
            return GetItemIndex(currentItem, 1);
        }

        public T GetNextItem(int currentItemIndex)
        {
            return GetItem(currentItemIndex, 1);
        }

        public T GetNextItem(T currentItem)
        {
            return GetItem(currentItem, 1);
        }

        public int GetPrevItemIndex(int currentItemIndex)
        {
            return GetItemIndex(currentItemIndex, -1);
        }

        public int GetPrevItemIndex(T currentItem)
        {
            return GetItemIndex(currentItem, -1);
        }

        public T GetPrevItem(int currentItemIndex)
        {
            return GetItem(currentItemIndex, -1);
        }

        public T GetPrevItem(T currentItem)
        {
            return GetItem(currentItem, -1);
        }

        #endregion
    }
}
