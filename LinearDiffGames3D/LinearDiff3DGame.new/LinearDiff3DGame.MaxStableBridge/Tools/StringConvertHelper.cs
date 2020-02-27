using System;

namespace LinearDiff3DGame.MaxStableBridge.Tools
{
    internal static class StringConvertHelper
    {
        public static Double[] ToDoubleArray(String source, IFormatProvider provider)
        {
            String[] items = source.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            return Array.ConvertAll(items, item => Double.Parse(item, provider));
        }

        public static Int32[] ToInt32Array(String source, IFormatProvider provider)
        {
            String[] items = source.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            return Array.ConvertAll(items, item => Int32.Parse(item, provider));
        }

        private static readonly Char[] splitChars = new[] {' ', '\t'};
    }
}