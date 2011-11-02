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
	/// <summary>
	/// Summary description for Matrix2D.
	/// </summary>
	public class Matrix2D : MatrixSet
	{

		#region Static members

		public static readonly Matrix2D Zero = new Matrix2D();
		public static readonly Matrix2D Identity = new Matrix2D(Vector2D.UnitX, Vector2D.UnitY);

		#endregion

		#region Constructors

		public Matrix2D() : base(2)
		{

		}

		public Matrix2D(Vector2D a, Vector2D b) : base(new Vector[]{(Vector)a, (Vector)b})
		{

		}

		public Matrix2D(double m11, double m12, double m21, double m22) 
			: base(new Vector[]{(Vector)new Vector2D(m11, m21),(Vector)new Vector2D(m12, m22)})
		{

		}

		public Matrix2D(Matrix2D m) : base(m)
		{

		}

		private Matrix2D(Vector[] vectors) : base(vectors)
		{
	
		}

		#endregion

		#region Properties
		/// <summary>The entry in the first row and first column of the matrix.</summary>
		/// <value>The entry in the first row and first column of the matrix.</value>
		public double M11
		{
			get
			{
				return this[0][0];
			}

			set
			{
				this.Val[0][0] = value;
			}
		}
		/// <summary>The entry in the first row and second column of the matrix.</summary>
		/// <value>The entry in the first row and second column of the matrix.</value>
		public double M12
		{
			get
			{
				return this[0][1];
			}
			set
			{
				this.Val[0][1] = value;
			}
		}
		/// <summary>The entry in the second row and first column of the matrix.</summary>
		/// <value>The entry in the second row and first column of the matrix.</value>
		public double M21
		{
			get
			{
				return this[1][0];
			}
			set
			{
				this.Val[1][0] = value;
			}
		}
		/// <summary>The entry in the second row and second column of the matrix.</summary>
		/// <value>The entry in the second row and second column of the matrix.</value>
		public double M22
		{
			get
			{
				return this[1][1];
			}
			set
			{
				this.Val[1][1] = value;
			}
		}

		/// <summary>The determinant of this matrix.</summary>
		/// <value>The determinant of this matrix.</value>
		public double Determinant
		{
			get { return this.M11 * this.M22 - this.M12 * this.M21; }
		}

		/// <summary>Indicates whether this matrix is invertible.</summary>
		/// <value><c>true</c> if this matrix is invertible; otherwise, <c>false</c>.</value>
		public bool IsInvertible
		{
			get { return this.Determinant != 0; }
		}

		#endregion

		#region Public methods

		/// <summary>The inverse of this matrix.</summary>
		/// <value>The inverse of this matrix.</value>
		/// <exception cref="InvalidOperationException">Thrown if this matrix is not invertible.</exception>
		public Matrix2D Inverse()
		{
			double determinant = this.Determinant;

			if (determinant == 0)
			{
				throw new InvalidOperationException("Cannot invert a singular matrix.");
			}

			Matrix2D inverse = new Matrix2D(this.M22 / determinant, -this.M12 / determinant, -this.M21 / determinant, this.M11 / determinant);

			return inverse;
		}
		#endregion

		#region Type converts

		/// <summary>
		/// Implicit convert a matrix2D to a matrix.
		/// </summary>
		/// <param name="m2d">a matrix2D.</param>
		/// <returns>a matrix.</returns>
		public static implicit operator Matrix(Matrix2D m2d)
		{
			return new Matrix(m2d.Cols);
		}

		/// <summary>
		/// Convert a matrix to a matrix2D.
		/// </summary>
		/// <param name="m">a matrix2D.</param>
		/// <returns>a matrix.</returns>
		public static Matrix2D ConvertFromMatrix(Matrix m)
		{
			if (m.Dimension == new Dimension2D(2, 2))
			{
				return new Matrix2D(m.Cols);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Explicitly convert a matrix2D to a squared matrix.
		/// </summary>
		/// <param name="m2d">a matrix2D.</param>
		/// <returns>a squared matrix.</returns>
		public static explicit operator SquaredMatrix(Matrix2D m2d)
		{
			return SquaredMatrix.ConvertFromMatrix((Matrix)m2d);
		}

		#endregion
	}
}
