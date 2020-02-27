using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// класс ApproxComp применяется для приблизительного сравнения двух действительных чисел
    /// </summary>
    public sealed class ApproxComp
    {
        /// <summary>
        /// Epsilon - величина, определяющая точность приблизительного сравнения
        /// </summary>
        public readonly Double Epsilon;

        /// <summary>
        /// конструктор класса ApproxCompClass; в нем (и только в нем) задается величина Epsilon
        /// </summary>
        /// <param name="epsilon">величина, определяющая точность приблизительного сравнения</param>
        public ApproxComp(Double epsilon)
        {
            this.Epsilon = epsilon;
        }

        /// <summary>
        /// метод GreaterThan возвращает true, если число number1 больше, чем число number2
        /// с учетом того, что операции сравнения приблизительные, number1 больше, чем number2, если (number1-number2) > Epsilon
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 больше number2, иначе false</returns>
        public Boolean GreaterThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta > Epsilon);
        }

        /// <summary>
        /// краткая форма метода метода GreaterThan
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>// <param name="number2"></param>
        /// <returns>true, если number1 больше number2, иначе false</returns>
        public Boolean GT(Double number1, Double number2)
        {
            return GreaterThan(number1, number2);
        }

        /// <summary>
        /// метод GreaterOrEqual возвращает true, если число number1 больше или равно числу number2
        /// с учетом того, что операции сравнения приблизительные, number1 больше или равно number2, если (number1-number2) >= -Epsilon
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 больше или равно number2, иначе false</returns>
        public Boolean GreaterOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta >= -Epsilon);
        }

        /// <summary>
        /// краткая форма метода GreaterOrEqual
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 больше или равно number2, иначе false</returns>
        public Boolean GE(Double number1, Double number2)
        {
            return GreaterOrEqual(number1, number2);
        }

        /// <summary>
        /// метод Equal возвращает true, если число number1 равно числу number2
        /// с учетом того, что операции сравнения приблизительные, number1 равно number2, если величина (number1-number2) принадлежит [-Epsilon, Epsilon]
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 равно number2, иначе false</returns>
        public Boolean Equal(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return ((delta >= -Epsilon) && (delta <= Epsilon));
        }

        /// <summary>
        /// краткая форма метода Equal
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 равно number2, иначе false</returns>
        public Boolean EQ(Double number1, Double number2)
        {
            return Equal(number1, number2);
        }

        /// <summary>
        /// метод NotEqual возвращает true, если число number1 не равно числу number2
        /// с учетом того, что операции сравнения приблизительные, number1 не равно number2, если величина (number1-number2) не принадлежит [-Epsilon, Epsilon]
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 не равно number2, иначе false</returns>
        public Boolean NotEqual(Double number1, Double number2)
        {
            return !Equal(number1, number2);
        }

        /// <summary>
        /// краткая форма метода NotEqual
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 не равно number2, иначе false</returns>
        public Boolean NE(Double number1, Double number2)
        {
            return NotEqual(number1, number2);
        }

        /// <summary>
        /// метод LessOrEqual возвращает true, если число number1 меньше или равно числу number2
        /// с учетом того, что операции сравнения приблизительные, number1 меньше или равно number2, если Epsilon >= (number1-number2)
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 меньше или равно number2, иначе false</returns>
        public Boolean LessOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta <= Epsilon);
        }

        /// <summary>
        /// краткая форма метода LessOrEqual
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 меньше или равно number2, иначе false</returns>
        public Boolean LE(Double number1, Double number2)
        {
            return LessOrEqual(number1, number2);
        }

        /// <summary>
        /// метод LessThan возвращает true, если число number1 меньше, чем число number2
        /// с учетом того, что операции сравнения приблизительные, number1 меньше, чем number2, если -Epsilon > (number1-number2)
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 меньше number2, иначе false</returns>
        public Boolean LessThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta < -Epsilon);
        }

        /// <summary>
        /// краткая форма метода LessThan
        /// </summary>
        /// <param name="number1">исходное действительное число</param>
        /// <param name="number2">действительное число, с которым происходит сравнение</param>
        /// <returns>true, если number1 меньше number2, иначе false</returns>
        public Boolean LT(Double number1, Double number2)
        {
            return LessThan(number1, number2);
        }
    }
}
