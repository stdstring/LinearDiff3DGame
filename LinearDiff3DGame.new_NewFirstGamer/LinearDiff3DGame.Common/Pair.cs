﻿namespace LinearDiff3DGame.Common
{
    public struct Pair<T1, T2>
    {
        public Pair(T1 item1, T2 item2)
            : this()
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1
        {
            get; private set;
        }

        public T2 Item2
        {
            get; private set;
        }
    }
}
