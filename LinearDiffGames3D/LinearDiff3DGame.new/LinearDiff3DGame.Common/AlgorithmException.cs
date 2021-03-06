﻿using System;
using System.Runtime.Serialization;

namespace LinearDiff3DGame.Common
{
	[Serializable]
	public class AlgorithmException : Exception
	{
		public AlgorithmException()
		{
		}

		public AlgorithmException(string message) : base(message)
		{
		}

		public AlgorithmException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected AlgorithmException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}