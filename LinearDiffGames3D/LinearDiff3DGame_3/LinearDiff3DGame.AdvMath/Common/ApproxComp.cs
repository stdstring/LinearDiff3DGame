using System;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// ����� ApproxComp ����������� ��� ���������������� ��������� ���� �������������� �����
    /// </summary>
    public class ApproxComp
    {
        /// <summary>
        /// Epsilon - ��������, ������������ �������� ���������������� ���������
        /// </summary>
        public readonly Double Epsilon;

        /// <summary>
        /// ����������� ������ ApproxCompClass; � ��� (� ������ � ���) �������� �������� Epsilon
        /// </summary>
        /// <param name="epsilon">��������, ������������ �������� ���������������� ���������</param>
        public ApproxComp(Double epsilon)
        {
            this.Epsilon = epsilon;
        }

        /// <summary>
        /// ����� GreaterThan ���������� true, ���� ����� number1 ������, ��� ����� number2
        /// � ������ ����, ��� �������� ��������� ���������������, number1 ������, ��� number2, ���� (number1-number2) > Epsilon
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ number2, ����� false</returns>
        public Boolean GreaterThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta > Epsilon);
        }

        /// <summary>
        /// ������� ����� ������ ������ GreaterThan
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>// <param name="number2"></param>
        /// <returns>true, ���� number1 ������ number2, ����� false</returns>
        public Boolean GT(Double number1, Double number2)
        {
            return GreaterThan(number1, number2);
        }

        /// <summary>
        /// ����� GreaterOrEqual ���������� true, ���� ����� number1 ������ ��� ����� ����� number2
        /// � ������ ����, ��� �������� ��������� ���������������, number1 ������ ��� ����� number2, ���� (number1-number2) >= -Epsilon
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ ��� ����� number2, ����� false</returns>
        public Boolean GreaterOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta >= -Epsilon);
        }

        /// <summary>
        /// ������� ����� ������ GreaterOrEqual
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ ��� ����� number2, ����� false</returns>
        public Boolean GE(Double number1, Double number2)
        {
            return GreaterOrEqual(number1, number2);
        }

        /// <summary>
        /// ����� Equal ���������� true, ���� ����� number1 ����� ����� number2
        /// � ������ ����, ��� �������� ��������� ���������������, number1 ����� number2, ���� �������� (number1-number2) ����������� [-Epsilon, Epsilon]
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ����� number2, ����� false</returns>
        public Boolean Equal(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return ((delta >= -Epsilon) && (delta <= Epsilon));
        }

        /// <summary>
        /// ������� ����� ������ Equal
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ����� number2, ����� false</returns>
        public Boolean EQ(Double number1, Double number2)
        {
            return Equal(number1, number2);
        }

        /// <summary>
        /// ����� NotEqual ���������� true, ���� ����� number1 �� ����� ����� number2
        /// � ������ ����, ��� �������� ��������� ���������������, number1 �� ����� number2, ���� �������� (number1-number2) �� ����������� [-Epsilon, Epsilon]
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 �� ����� number2, ����� false</returns>
        public Boolean NotEqual(Double number1, Double number2)
        {
            return !Equal(number1, number2);
        }

        /// <summary>
        /// ������� ����� ������ NotEqual
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 �� ����� number2, ����� false</returns>
        public Boolean NE(Double number1, Double number2)
        {
            return NotEqual(number1, number2);
        }

        /// <summary>
        /// ����� LessOrEqual ���������� true, ���� ����� number1 ������ ��� ����� ����� number2
        /// � ������ ����, ��� �������� ��������� ���������������, number1 ������ ��� ����� number2, ���� Epsilon >= (number1-number2)
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ ��� ����� number2, ����� false</returns>
        public Boolean LessOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta <= Epsilon);
        }

        /// <summary>
        /// ������� ����� ������ LessOrEqual
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ ��� ����� number2, ����� false</returns>
        public Boolean LE(Double number1, Double number2)
        {
            return LessOrEqual(number1, number2);
        }

        /// <summary>
        /// ����� LessThan ���������� true, ���� ����� number1 ������, ��� ����� number2
        /// � ������ ����, ��� �������� ��������� ���������������, number1 ������, ��� number2, ���� -Epsilon > (number1-number2)
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ number2, ����� false</returns>
        public Boolean LessThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;

            return (delta < -Epsilon);
        }

        /// <summary>
        /// ������� ����� ������ LessThan
        /// </summary>
        /// <param name="number1">�������� �������������� �����</param>
        /// <param name="number2">�������������� �����, � ������� ���������� ���������</param>
        /// <returns>true, ���� number1 ������ number2, ����� false</returns>
        public Boolean LT(Double number1, Double number2)
        {
            return LessThan(number1, number2);
        }
    }
}
