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

//Created on 03-03-2004

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for Matrix3D.
	/// </summary>
	public class Matrix3D: MatrixSet
	{

		#region Static members

		public static readonly Matrix3D Zero = new Matrix3D();
		public static readonly Matrix3D Identity = new Matrix3D(Vector3D.UnitX, Vector3D.UnitY, Vector3D.UnitZ);

		#endregion

		#region Constructors

		public Matrix3D() : base(3, 3)
		{
		}
		public Matrix3D(Vector3D a, Vector3D b, Vector3D c) : base(new Vector[]{(Vector)a, (Vector)b, (Vector)c})
		{
		}
		public Matrix3D(Matrix3D m) : base(m)
		{
		}
		private Matrix3D(Vector[] vectors) : base(vectors)
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
		/// <summary>The entry in the first row and third column of the matrix.</summary>
		/// <value>The entry in the first row and third column of the matrix.</value>
		public double M13
		{
			get
			{
				return this[0][2];
			}
			set
			{
				this.Val[0][2] = value;
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
		/// <summary>The entry in the second row and third column of the matrix.</summary>
		/// <value>The entry in the second row and third column of the matrix.</value>
		public double M23
		{
			get
			{
				return this[1][2];
			}
			set
			{
				this.Val[1][2] = value;
			}
		}
		/// <summary>The entry in the third row and first column of the matrix.</summary>
		/// <value>The entry in the third row and first column of the matrix.</value>
		public double M31
		{
			get
			{
				return this[2][0];
			}
			set
			{
				this.Val[2][0] = value;
			}
		}
		/// <summary>The entry in the third row and second column of the matrix.</summary>
		/// <value>The entry in the third row and second column of the matrix.</value>
		public double M32
		{
			get
			{
				return this[2][1];
			}
			set
			{
				this.Val[2][1] = value;
			}
		}
		/// <summary>The entry in the third row and third column of the matrix.</summary>
		/// <value>The entry in the third row and third column of the matrix.</value>
		public double M33
		{
			get
			{
				return this[2][2];
			}
			set
			{
				this.Val[2][2] = value;
			}
		}
		/// <summary>The determinant of this matrix.</summary>
		/// <value>The determinant of this matrix.</value>
		public double Determinant
		{
			get { 
					double a = M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32;
					double b = M13 * M22 * M31 + M12 * M21 * M33 + M11 * M23 * M32;
					return a - b;
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

		/// <summary>The inverse of this matrix.</summary>
		/// <value>The inverse of this matrix.</value>
		/// <exception cref="InvalidOperationException">Thrown if this matrix is not invertible.</exception>
		public Matrix3D Inverse()
		{
			return (Matrix3D)((SquaredMatrix)this).Inverse();
		}
		#endregion

		#region Type converts

		/// <summary>
		/// Implicit convert a matrix2D to a matrix.
		/// </summary>
		/// <param name="m2d">a matrix2D.</param>
		/// <returns>a matrix.</returns>
		public static implicit operator Matrix(Matrix3D m3d)
		{
			return new Matrix(m3d.Cols);
		}
		/// <summary>
		/// Convert a matrix to a matrix2D.
		/// </summary>
		/// <param name="m">a matrix2D.</param>
		/// <returns>a matrix.</returns>
		public static Matrix3D ConvertFromMatrix(Matrix m)
		{
			if (m.Dimension == new Dimension2D(3, 3))
			{
				return new Matrix3D(m.Cols);
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
		public static explicit operator SquaredMatrix(Matrix3D m3d)
		{
			return SquaredMatrix.ConvertFromMatrix((Matrix)m3d);
		}

		#endregion
	}
}