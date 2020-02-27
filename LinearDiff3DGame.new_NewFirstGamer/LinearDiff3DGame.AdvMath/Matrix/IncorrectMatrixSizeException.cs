using System;
using System.Runtime.Serialization;

namespace LinearDiff3DGame.AdvMath
{
	[Serializable]
	public class IncorrectMatrixSizeException : ArgumentException, ISerializable
	{
		public IncorrectMatrixSizeException()
		{
		}

		public IncorrectMatrixSizeException(String message)
			: base(message)
		{
		}

		public IncorrectMatrixSizeException(String message, Exception innerException)
			: base(message, innerException)
		{
		}

		public IncorrectMatrixSizeException(String message, String paramName)
			: base(message, paramName)
		{
		}

		public IncorrectMatrixSizeException(String message, String paramName, Exception innerException)
			: base(message, paramName, innerException)
		{
		}

		protected IncorrectMatrixSizeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}