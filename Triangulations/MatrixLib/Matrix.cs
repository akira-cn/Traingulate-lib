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

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;
	using Akira.MatrixLib.Exceptions;

	/// <summary>
	/// Summary description for Matrix.
	/// </summary>
	public class Matrix : MatrixSet
	{

		#region Static members

		public static readonly Matrix Zero = new Matrix();
		public static readonly Matrix Identity = new Matrix(new Vector[] { Vector2D.UnitX, Vector2D.UnitY });

		#endregion

		#region Constructors

		public Matrix() : base()
		{

		}

		public Matrix(int size) : base(size)
		{

		}

		public Matrix(int m, int n) : base(m, n)
        {

		}

		public Matrix(Vector vector) : base(vector)
		{

		}

		public Matrix(Vector[] vectors) : base(vectors)
		{

		}

		public Matrix(MatrixSet m) : base(m)
		{

		}

		#endregion

		#region Type converts

		/// <summary>
		/// Implicitly convert a Matrix to a Matrix2D.
		/// </summary>
		/// <param name="m">a matrix.</param>
		/// <returns>a matrix2D.</returns>
		public static implicit operator Matrix2D(Matrix m)
		{
			if (m.Dimension.Equals(new Vector2D(2, 2)))
			{
				return Matrix2D.ConvertFromMatrix(m);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}

		/// <summary>
		/// Implicitly convert a Matrix to a SquaredMatrix.
		/// </summary>
		/// <param name="m">a matrix.</param>
		/// <returns>a squared matrix.</returns>
		public static implicit operator SquaredMatrix(Matrix m)
		{
			if (m.isSquared)
			{
				return SquaredMatrix.ConvertFromMatrix(m);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}

		#endregion
	}
}
