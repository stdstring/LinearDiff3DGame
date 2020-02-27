using System;
using System.Collections.Generic;

namespace LinearDiff3DGame.Common
{
    public static class IListExtensions
    {
        public static Int32 GetItemIndex<T>(this IList<T> list, Int32 itemIndex, Int32 shiftValue)
        {
            if(itemIndex < 0 || itemIndex >= list.Count)
                throw new ArgumentOutOfRangeException("itemIndex");
            if(shiftValue <= -list.Count || shiftValue >= list.Count)
                throw new ArgumentOutOfRangeException("shiftValue");
            if(shiftValue >= 0)
                return (itemIndex + shiftValue) % list.Count;
            return (itemIndex + shiftValue + list.Count) % list.Count;
        }

        public static Int32 GetItemIndex<T>(this IList<T> list, T item, Int32 shiftValue)
        {
            return GetItemIndex(list, list.IndexOf(item), shiftValue);
        }

        public static T GetItem<T>(this IList<T> list, Int32 itemIndex, Int32 shiftValue)
        {
            return list[GetItemIndex(list, itemIndex, shiftValue)];
        }

        public static T GetItem<T>(this IList<T> list, T item, Int32 shiftValue)
        {
            return list[GetItemIndex(list, item, shiftValue)];
        }

        public static Int32 GetNextItemIndex<T>(this IList<T> list, Int32 itemIndex)
        {
            return GetItemIndex(list, itemIndex, 1);
        }

        public static Int32 GetNextItemIndex<T>(this IList<T> list, T item)
        {
            return GetItemIndex(list, item, 1);
        }

        public static T GetNextItem<T>(this IList<T> list, Int32 itemIndex)
        {
            return GetItem(list, itemIndex, 1);
        }

        public static T GetNextItem<T>(this IList<T> list, T item)
        {
            return GetItem(list, item, 1);
        }

        public static Int32 GetPrevItemIndex<T>(this IList<T> list, Int32 itemIndex)
        {
            return GetItemIndex(list, itemIndex, -1);
        }

        public static Int32 GetPrevItemIndex<T>(this IList<T> list, T item)
        {
            return GetItemIndex(list, item, -1);
        }

        public static T GetPrevItem<T>(this IList<T> list, Int32 itemIndex)
        {
            return GetItem(list, itemIndex, -1);
        }

        public static T GetPrevItem<T>(this IList<T> list, T item)
        {
            return GetItem(list, item, -1);
        }
    }
}