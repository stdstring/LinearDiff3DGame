using System;

namespace LinearDiff3DGame.AdvMath.Common
{
    public class AdvTrigonometry
    {
        public AdvTrigonometry() : this(DefaultThresholdValue)
        {
        }

        public AdvTrigonometry(Double thresholdValue)
        {
            ThresholdValue = thresholdValue;
        }

#warning Подумать про периодичность функции
        public Double Sin(Double x)
        {
            return (-ThresholdValue <= x && x <= ThresholdValue) ? x : Math.Sin(x);
        }

#warning Подумать про периодичность функции
        public Double Cos(Double x)
        {
            return (-ThresholdValue <= x && x <= ThresholdValue) ? 1 - x*x/2 : Math.Cos(x);
        }

        public Double ThresholdValue { get; set; }

        public const Double DefaultThresholdValue = 1e-5;
    }
}