using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearDiff3DGame.AdvMath
{
    public sealed partial class Matrix : IElementaryOp
    {
        #region IElementaryOp Members

        IElementaryOp IElementaryOp.Addition(IElementaryOp op1, IElementaryOp op2)
        {
            throw new NotImplementedException();
        }

        IElementaryOp IElementaryOp.Subtraction(IElementaryOp op1, IElementaryOp op2)
        {
            throw new NotImplementedException();
        }

        IElementaryOp IElementaryOp.Multiplication(IElementaryOp op1, IElementaryOp op2)
        {
            throw new NotImplementedException();
        }

        IElementaryOp IElementaryOp.Scaling(IElementaryOp op, double rate)
        {
            throw new NotImplementedException();
        }

        IElementaryOp IElementaryOp.Inverse(IElementaryOp op)
        {
            throw new NotImplementedException();
        }

        bool IElementaryOp.CanAddition
        {
            get { throw new NotImplementedException(); }
        }

        bool IElementaryOp.CanSubtraction
        {
            get { throw new NotImplementedException(); }
        }

        bool IElementaryOp.CanMultiplication
        {
            get { throw new NotImplementedException(); }
        }

        bool IElementaryOp.CanScaling
        {
            get { throw new NotImplementedException(); }
        }

        bool IElementaryOp.CanInverse
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
