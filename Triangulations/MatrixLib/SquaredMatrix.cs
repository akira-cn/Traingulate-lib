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
	/// Summary description for SquaredMatrix.
	/// </summary>
	public class SquaredMatrix : MatrixSet
	{
		#region Constructors

		public SquaredMatrix() : base(2, 2)
		{

		}

		public SquaredMatrix(int size) : base(size)
		{

		}

		public SquaredMatrix(SquaredMatrix m) : base(m)
		{

		}

		private SquaredMatrix(Vector[] vectors) : base(vectors)
		{

		}

		#endregion

		#region Properties

		/// <summary>
		/// The size of this squared matrix.
		/// </summary>
		/// <value></value>
		public int Size
		{
			get
			{
				return this.Col;
			}
		}

		/// <summary>
		/// The identity square of this squared matrix.
		/// </summary>
		/// <value>identity square.</value>
		public SquaredMatrix Identity
		{
			get
			{
				SquaredMatrix s = new SquaredMatrix(Size);
                
				for (int i = 0; i < Size; i++)
				{
					s[i][i] = 1;
				}

				return s;
			}
		}

		/// <summary>The determinant of this matrix.</summary>
		/// <value>The determinant of this matrix.</value>
		public double Determinant
		{
			get 
			{
				if (this.Size == 1)
				{
					return this[0][0];
				}
				else if (this.Size == 2)
				{
					return this[0][0] * this[1][1] - this[0][1] * this[1][0];
				}
				else
				{
					double d = 0.0;

					for (int i = 0; i < this.Col; i++)
					{
						d += this[0][i] * this.AlgebraRemainDeterminant(0, i);
					}

					return d;
				}
			}
		}

		/// <summary>Indicates whether this matrix is invertible.</summary>
		/// <value><c>true</c> if this matrix is invertible; otherwise, <c>false</c>.</value>
		public bool IsInvertible
		{
			get { return this.Determinant != 0; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// The algebra remain determinant of this square.
		/// </summary>
		/// <param name="row">the row index of the sub.</param>
		/// <param name="col">the col index of the sub.</param>
		/// <returns>the algebra remain determinant of this square.</returns>
		public double AlgebraRemainDeterminant(int row, int col)
		{
			if (row < 0 || col < 0 || row >= Row || col >= Col)
			{
				throw (new ArgumentException());
			}

			SquaredMatrix s = (SquaredMatrix)this.RemainMatrix(row, col);

			return Math.Pow(-1, row + col) * s.Determinant;
		}

		/// <summary>
		/// The inversal matrix of this.
		/// </summary>
		/// <returns>the inversal matrix.</returns>
		public SquaredMatrix Inverse()
		{
			double determinant = this.Determinant;

			if (determinant == 0)
			{
				throw new InvalidOperationException("Cannot invert a singular matrix.");
			}

			SquaredMatrix s = new SquaredMatrix(Size);

			for (int i = 0; i < Row; i++)
			{
				for (int j = 0; j < Col; j++)
				{
					s[j][i] = this.AlgebraRemainDeterminant(i, j);
				}
			}

			return s/Determinant;
		}

		#endregion

		#region Type converts

		/// <summary>
		/// Implicitly convert a squared matrix to a matrix.
		/// </summary>
		/// <param name="s">a squared matrix.</param>
		/// <returns>a matrix.</returns>
		public static implicit operator Matrix(SquaredMatrix s)
		{
			return new Matrix(s.Cols);
		}

		/// <summary>
		/// Convert a matrix to a squared matrix.
		/// </summary>
		/// <param name="m">a matrix.</param>
		/// <returns>a squared matrix.(null if the matrix is not squared.)</returns>
		public static SquaredMatrix ConvertFromMatrix(Matrix m)
		{
			if (m.isSquared)
			{
				return new SquaredMatrix(m.Cols);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Explicitly convert a squared matrix to a matrix2D.
		/// </summary>
		/// <param name="s">a squared matrix.</param>
		/// <returns>a matrix2D.</returns>
		public static explicit operator Matrix2D(SquaredMatrix s)
		{
			if (s.Size == 2)
			{
				return Matrix2D.ConvertFromMatrix((Matrix)s);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}

		/// <summary>
		/// Explicitly convert a squared matrix to a matrix3D.
		/// </summary>
		/// <param name="s">a squared matrix.</param>
		/// <returns>a matrix3D.</returns>
		public static explicit operator Matrix3D(SquaredMatrix s)
		{
			if (s.Size == 3)
			{
				return Matrix3D.ConvertFromMatrix((Matrix)s);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}
		#endregion
	}
}
