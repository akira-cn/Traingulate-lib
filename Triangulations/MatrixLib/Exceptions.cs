//Copyright (C) 2004  Akira (akira_cn@msn.com)

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Library General Public
//License as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Library General Public License for more details.

//You should have received a copy of the GNU Library General Public
//License along with this library; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA  02111-1307, USA.

//Created on 02-06-2004
//Modified on 02-09-2004

namespace Akira.MatrixLib.Exceptions
{
	using System;

	public class ZeroVectorToUnitException: ApplicationException
	{
		public ZeroVectorToUnitException() : base("Cannot normalize a zero-length vector.")
		{
		}
		public ZeroVectorToUnitException(string message) : base(message)
		{
		}
		public ZeroVectorToUnitException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class TypeConvertFailureException : ApplicationException
	{
		public TypeConvertFailureException() : base("Cannot covert objects with different dimensions.")
		{
		}
		public TypeConvertFailureException(string message) : base(message)
		{
		}
		public TypeConvertFailureException(string message, Exception inner) : base(message, inner)
		{
		}
	}
	
	public class DimensionsNotEqualException : ApplicationException
	{
		public DimensionsNotEqualException() : base("The dimensions of this two elements are not equal.")
		{
		}
		public DimensionsNotEqualException(string message) : base(message)
		{
		}
		public DimensionsNotEqualException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class PointNotRegularException : ApplicationException
	{
		public PointNotRegularException() : base("Point is not regular.")
		{
		}
		public PointNotRegularException(string message) : base(message)
		{
		}
		public PointNotRegularException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class MatrixMultiplyException : ApplicationException
	{
		public MatrixMultiplyException() : base("Cannot multiply the two matrixes of this type.")
		{
		}
		public MatrixMultiplyException(string message) : base(message)
		{
		}
		public MatrixMultiplyException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class InvalidateEntropyException: ApplicationException
	{
		public InvalidateEntropyException() : base("Cannot caculate the entropy of this.")
		{
		}
		public InvalidateEntropyException(string message) : base(message)
		{
		}
		public InvalidateEntropyException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
