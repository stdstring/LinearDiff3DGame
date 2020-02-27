using System;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.AdvMath.Common
{
    [Immutable]
    // класс ApproxComp применяется для приблизительного сравнения двух действительных чисел
    public class ApproxComp
    {
        // Epsilon - величина, определяющая точность приблизительного сравнения
        public readonly Double Epsilon;

        public ApproxComp(Double epsilon)
        {
            Epsilon = epsilon;
        }

        // метод GreaterThan возвращает true, если число number1 больше, чем число number2
        // с учетом того, что операции сравнения приблизительные, number1 больше, чем number2, если (number1-number2) > Epsilon
        public Boolean GreaterThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta > Epsilon);
        }

        // краткая форма метода метода GreaterThan
        public Boolean GT(Double number1, Double number2)
        {
            return GreaterThan(number1, number2);
        }

        // метод GreaterOrEqual возвращает true, если число number1 больше или равно числу number2
        // с учетом того, что операции сравнения приблизительные, number1 больше или равно number2, если (number1-number2) >= -Epsilon
        public Boolean GreaterOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta >= -Epsilon);
        }

        // краткая форма метода GreaterOrEqual
        public Boolean GE(Double number1, Double number2)
        {
            return GreaterOrEqual(number1, number2);
        }

        // метод Equal возвращает true, если число number1 равно числу number2
        // с учетом того, что операции сравнения приблизительные, number1 равно number2, если величина (number1-number2) принадлежит [-Epsilon, Epsilon]
        public Boolean Equal(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return ((delta >= -Epsilon) && (delta <= Epsilon));
        }

        // краткая форма метода Equal
        public Boolean EQ(Double number1, Double number2)
        {
            return Equal(number1, number2);
        }

        // метод NotEqual возвращает true, если число number1 не равно числу number2
        // с учетом того, что операции сравнения приблизительные, number1 не равно number2, если величина (number1-number2) не принадлежит [-Epsilon, Epsilon]
        public Boolean NotEqual(Double number1, Double number2)
        {
            return !Equal(number1, number2);
        }

        // краткая форма метода NotEqual
        public Boolean NE(Double number1, Double number2)
        {
            return NotEqual(number1, number2);
        }

        // метод LessOrEqual возвращает true, если число number1 меньше или равно числу number2
        // с учетом того, что операции сравнения приблизительные, number1 меньше или равно number2, если Epsilon >= (number1-number2)
        public Boolean LessOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta <= Epsilon);
        }

        // краткая форма метода LessOrEqual
        public Boolean LE(Double number1, Double number2)
        {
            return LessOrEqual(number1, number2);
        }

        // метод LessThan возвращает true, если число number1 меньше, чем число number2
        // с учетом того, что операции сравнения приблизительные, number1 меньше, чем number2, если -Epsilon > (number1-number2)
        public Boolean LessThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta < -Epsilon);
        }

        // краткая форма метода LessThan
        public Boolean LT(Double number1, Double number2)
        {
            return LessThan(number1, number2);
        }
    }
}