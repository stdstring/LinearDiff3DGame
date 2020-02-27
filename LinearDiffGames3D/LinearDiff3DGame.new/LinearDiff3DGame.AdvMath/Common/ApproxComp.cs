using System;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.AdvMath.Common
{
    [Immutable]
    // ����� ApproxComp ����������� ��� ���������������� ��������� ���� �������������� �����
    public class ApproxComp
    {
        // Epsilon - ��������, ������������ �������� ���������������� ���������
        public readonly Double Epsilon;

        public ApproxComp(Double epsilon)
        {
            Epsilon = epsilon;
        }

        // ����� GreaterThan ���������� true, ���� ����� number1 ������, ��� ����� number2
        // � ������ ����, ��� �������� ��������� ���������������, number1 ������, ��� number2, ���� (number1-number2) > Epsilon
        public Boolean GreaterThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta > Epsilon);
        }

        // ������� ����� ������ ������ GreaterThan
        public Boolean GT(Double number1, Double number2)
        {
            return GreaterThan(number1, number2);
        }

        // ����� GreaterOrEqual ���������� true, ���� ����� number1 ������ ��� ����� ����� number2
        // � ������ ����, ��� �������� ��������� ���������������, number1 ������ ��� ����� number2, ���� (number1-number2) >= -Epsilon
        public Boolean GreaterOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta >= -Epsilon);
        }

        // ������� ����� ������ GreaterOrEqual
        public Boolean GE(Double number1, Double number2)
        {
            return GreaterOrEqual(number1, number2);
        }

        // ����� Equal ���������� true, ���� ����� number1 ����� ����� number2
        // � ������ ����, ��� �������� ��������� ���������������, number1 ����� number2, ���� �������� (number1-number2) ����������� [-Epsilon, Epsilon]
        public Boolean Equal(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return ((delta >= -Epsilon) && (delta <= Epsilon));
        }

        // ������� ����� ������ Equal
        public Boolean EQ(Double number1, Double number2)
        {
            return Equal(number1, number2);
        }

        // ����� NotEqual ���������� true, ���� ����� number1 �� ����� ����� number2
        // � ������ ����, ��� �������� ��������� ���������������, number1 �� ����� number2, ���� �������� (number1-number2) �� ����������� [-Epsilon, Epsilon]
        public Boolean NotEqual(Double number1, Double number2)
        {
            return !Equal(number1, number2);
        }

        // ������� ����� ������ NotEqual
        public Boolean NE(Double number1, Double number2)
        {
            return NotEqual(number1, number2);
        }

        // ����� LessOrEqual ���������� true, ���� ����� number1 ������ ��� ����� ����� number2
        // � ������ ����, ��� �������� ��������� ���������������, number1 ������ ��� ����� number2, ���� Epsilon >= (number1-number2)
        public Boolean LessOrEqual(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta <= Epsilon);
        }

        // ������� ����� ������ LessOrEqual
        public Boolean LE(Double number1, Double number2)
        {
            return LessOrEqual(number1, number2);
        }

        // ����� LessThan ���������� true, ���� ����� number1 ������, ��� ����� number2
        // � ������ ����, ��� �������� ��������� ���������������, number1 ������, ��� number2, ���� -Epsilon > (number1-number2)
        public Boolean LessThan(Double number1, Double number2)
        {
            Double delta = number1 - number2;
            return (delta < -Epsilon);
        }

        // ������� ����� ������ LessThan
        public Boolean LT(Double number1, Double number2)
        {
            return LessThan(number1, number2);
        }
    }
}