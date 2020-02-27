using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    /// <summary>
    /// интерфейс для поддержки элементарных математических операций над объектами
    /// </summary>
    public interface IElementaryOp
    {
        /// <summary>
        /// операция сложения двух объектов
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        IElementaryOp Addition(IElementaryOp op1, IElementaryOp op2);
        /// <summary>
        /// операция разности двух объектов
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        IElementaryOp Subtraction(IElementaryOp op1, IElementaryOp op2);
        /// <summary>
        /// операция умножения двух объектов
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        IElementaryOp Multiplication(IElementaryOp op1, IElementaryOp op2);
        /// <summary>
        /// операция маштабирования (умножение на число) объекта
        /// </summary>
        /// <param name="op"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        IElementaryOp Scaling(IElementaryOp op, Double rate);
        /// <summary>
        /// операция получения обратного объекта (для данного)
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        IElementaryOp Inverse(IElementaryOp op);

        /// <summary>
        /// флаг, показывающий возможно ли сложение двух объектов
        /// </summary>
        Boolean CanAddition
        {
            get;
        }
        /// <summary>
        /// флаг, показывающий возможно ли вычитание двух объектов
        /// </summary>
        Boolean CanSubtraction
        {
            get;
        }
        /// <summary>
        /// флаг, показывающий возможно ли умножение двух объектов
        /// </summary>
        Boolean CanMultiplication
        {
            get;
        }
        /// <summary>
        /// флаг, показывающий возможно ли маштабтрование объекта
        /// </summary>
        Boolean CanScaling
        {
            get;
        }
        /// <summary>
        /// флаг, показывающий возможно ли получение обратного объекта
        /// </summary>
        Boolean CanInverse
        {
            get;
        }
    }
}