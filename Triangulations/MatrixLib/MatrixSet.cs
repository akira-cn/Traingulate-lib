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
	/// Summary description for MatrixSet.
	/// </summary>
	public abstract class MatrixSet
	{
		#region Static fields

		public static readonly double ZeroTolerance = Double.Epsilon;

		#endregion

		#region Member fields

		protected double[][] Val = null;
		protected int row = 0;
		protected int col = 0;

		#endregion

		#region Properties
	
		/// <summary>
		/// Determines whether this matrix is regular (finite and well-defined).
		/// </summary>
		/// <returns><c>true</c> if both coordinates are finite and well-defined; <c>false</c> otherwise.</returns>
		public bool isRegular
		{
			get
			{
				return this.Dimension.X > 0 && this.Dimension.Y > 0;
			}
		}

		/// <summary>
		/// Determines whether this matrix is squared.
		/// </summary>
		/// <returns><c>true</c> if this matrix is squared; <c>false</c> otherwise.</returns>
		public bool isSquared
		{
			get
			{
				return this.Dimension.X == this.Dimension.Y;
			}
		}

		/// <summary>
		/// The row vectors of this matrix.
		/// </summary>
		/// <value>the row vectors.</value>
		public Vector[] Rows
		{
			get
			{
				Vector[] vectors = new Vector[row];

				for (int i = 0; i < row; i++)
				{
					vectors[i] = new Vector(Val[i]);
				}

				return vectors;
			}
		}

		/// <summary>
		/// The col vectors of this matrix.
		/// </summary>
		/// <value>the col vectors.</value>
		public Vector[] Cols
		{
			get
			{
				Vector[] vectors = new Vector[col];

				for (int i = 0; i < col; i++)
				{
					vectors[i] = new Vector(row);
					for (int j = 0; j < row; j++)
					{
						vectors[i][j] = Val[j][i];
					}
				}

				return vectors;
			}
		}

		/// <summary>
		/// The number of rows in this matrix.
		/// </summary>
		/// <value>the number of rows.</value>
		public int Row
		{
			get
			{
				return row;
			}
		}

		/// <summary>
		/// The number of cols in this matrix.
		/// </summary>
		/// <value>the number of cols.</value>
		public int Col
		{
			get
			{
				return col;
			}
		}

		/// <summary>
		/// The Dimension of this matrix.
		/// </summary>
		/// <value>the Dimension.</value>		
		public Dimension2D Dimension
		{
			get
			{
				return new Dimension2D(row, col);
			}
		}

		/// <summary>
		/// The entropy of this matrix. H(Matrix(a,b)) = Vector[H(Vector(a),H(Vector(b)]
		/// </summary>
		/// <value>H(Matrix(a,b)).</value>
		public Vector Entropy
		{
			get
			{
				Vector entropy = new Vector(col);

				for (int i = 0; i < col; i++)
				{
					entropy[i] = this.Cols[i].Entropy;
				}

				return entropy;
			}
		}
		#endregion


		#region Indexer

		/// <summary>
		/// The <paramref name="index"/>th col of this matrix.
		/// </summary>
		/// <param name="index">The index of the col (must be less than the dimension.x).</param>
		/// <value>The <paramref name="index"/>th col of this matrix.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not less than the dimesion.x.</exception>
		public Vector this[int index]
		{
			get
			{
				return new Vector(Val[index]);
			}
			set
			{
				this[index] = value;
			}
		}

		#endregion

		#region Constructors

		protected MatrixSet()
		{
			Val = new double[1][];
			Val[0] = new double[1];
			row = 1;
			col = 1;
		}

		protected MatrixSet(int size)
		{
			Debug.Assert(size > 0);
			Val = new double[size][];

			for (int i = 0; i < size; i++)
			{
				Val[i] = new double[size];
			}

			row = size;
			col = size;		
		}

		protected MatrixSet(int m, int n)
		{
			Debug.Assert(m > 0 && n > 0);
			Val = new double[m][];
			
			for (int i = 0; i < m; i++)
			{
				Val[i] = new double[n];
			}
			row = m;
			col = n;
		}

		protected MatrixSet(Vector vector)
		{
			Val = new double[vector.Dimension][];

			for (int i = 0; i < vector.Dimension; i++)
			{
				Val[i] = new double[1];
				Val[i][0] = vector[i];
			}

			row = vector.Dimension;
			col = 1;
		}

		protected MatrixSet(Vector[] vectors)
		{
			col = vectors.Length;
			row = vectors[0].Dimension;
			
			Val = new double[row][];
			for (int i = 0; i < row; i++)
			{
				Val[i] = new double[col];
			}

			int j = 0;

			foreach (Vector vector in vectors)
			{
				for (int i = 0; i < vector.Dimension; i++)
				{
					Val[i][j] = vector[i];
				}

				j++;
			}
		}

		protected MatrixSet(MatrixSet m)
		{
			col = m.Col;
			row = m.Row;

			Val = new double[row][];
			for (int i = 0; i < row; i++)
			{
				Val[i] = new double[col];
			}

			int j = 0;

			foreach (Vector vector in m.Cols)
			{
				for (int i = 0; i < vector.Dimension; i++)
				{
					Val[i][j] = vector[i];
				}

				j++;
			}
		}

		#endregion

		#region Override methods

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			foreach (double[] Ponderance in Val)
			{
				stringBuilder.Append("[");
				foreach (double v in Ponderance)
				{
					stringBuilder.Append(v.ToString());
					stringBuilder.Append(",");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append("]");
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Determines whether the specified object is equal to this matrix.
		/// </summary>
		/// <param name="obj">The object to compare with this matrix.</param>
		/// <returns><c>true</c> if the specified object is equal to this matrix; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj)
		{
			bool isEqual = true;

			if (obj is MatrixSet)
			{
				if (this.Dimension != ((MatrixSet)obj).Dimension)
				{
					isEqual = false;
				}
				else
				{
					for (int i = 0; i < this.Dimension.X; i++)
					{
						if (this[i] != ((MatrixSet)obj)[i])
						{
							isEqual = false;
							break;
						}
					}
				}
			}
			else
			{
				isEqual = false;
			}

			return isEqual;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			int hashCode = 0;

			for (int i = 0; i < row; i++)
			{
				hashCode ^= this[i].GetHashCode();
			}
			return hashCode;
		}

		#endregion

		#region Public methods

		#region Primary translations
		/// <summary>
		/// Swap two rows.
		/// </summary>
		/// <param name="a">the first row.</param>
		/// <param name="b">the second row.</param>
		public void SwapRow(int a, int b)
		{
			if (a >= this.Dimension.X || b >= this.Dimension.X || a < 0 || b < 0)
			{
				throw (new ArgumentException());
			}

			if (a == b)
			{
				return;
			}

			for (int i = 0; i < this.Dimension.Y; i++)
			{
				double tmp = this[a][i];

				this[a][i] = this[b][i];
				this[b][i] = tmp;
			}
		}
		/// <summary>
		/// Swap two cols.
		/// </summary>
		/// <param name="a">the first col.</param>
		/// <param name="b">the second col.</param>
		public void SwapCol(int a, int b)
		{
			if (a >= this.Dimension.Y || b >= this.Dimension.Y || a < 0 || b < 0)
			{
				throw (new ArgumentException());
			}

			if (a == b)
			{
				return;
			}

			for (int i = 0; i < this.Dimension.X; i++)
			{
				double tmp = this[i][a];

				this[i][a] = this[i][b];
				this[i][b] = tmp;
			}
		}
		/// <summary>
		/// Add a row to another row. AddRow(a, b, t)=> a = a + b * t
		/// </summary>
		/// <param name="a">the first row.</param>
		/// <param name="b">the second row.</param>
		/// <param name="t">the ratio.</param>
		public void AddRow(int a, int b, double t)
		{
			if (a >= this.Dimension.X || b >= this.Dimension.X || a < 0 || b < 0)
			{
				throw (new ArgumentException());
			}
			
			for (int i = 0; i < this.Dimension.Y; i++)
			{
				this[a][i] += this[b][i] * t;
			}
		}
		/// <summary>
		/// Add a col to anther col. AddCol(a, b, t)=> a = a + b * t
		/// </summary>
		/// <param name="a">the first row.</param>
		/// <param name="b">the second row.</param>
		/// <param name="t">the ratio.</param>
		public void AddCol(int a, int b, double t)
		{
			if (a >= this.Dimension.Y || b >= this.Dimension.Y || a < 0 || b < 0)
			{
				throw (new ArgumentException());
			}

			for (int i = 0; i < this.Dimension.X; i++)
			{
				this[i][a] += this[i][b] * t;
			}
		}
		/// <summary>
		/// Multiply a row by a scalar.
		/// </summary>
		/// <param name="a">the row index.</param>
		/// <param name="t">the scalar.</param>
		public void MultiplyRow(int a, double t)
		{
			if (a >= this.Dimension.X || a < 0)
			{
				throw (new ArgumentException());
			}

			for (int i = 0; i < this.Dimension.Y; i++)
			{
				this[a][i] *= t;
			}
		}
		/// <summary>
		/// Multiply a col by a scalar.
		/// </summary>
		/// <param name="a">the col index.</param>
		/// <param name="t">the scalar.</param>
		public void MultiplyCol(int a, double t)
		{
			if (a >= this.Dimension.Y || a < 0)
			{
				throw (new ArgumentException());
			}

			for (int i = 0; i < this.Dimension.X; i++)
			{
				this[i][a] *= t;
			}
		}
		/// <summary>
		/// Translate this matrix.(to swap cols and rows.)
		/// </summary>
		/// <returns>translated matrix.</returns>
		public Matrix Translate()
		{
			Matrix m = new Matrix(col, row);

			for (int i = 0; i < row; i++)
			{
				for (int j = 0; j < col; j++)
				{
					m.Val[j][i] = this.Val[i][j];
				}
			}

			return m;
		}
		#endregion

		/// <summary>
		/// Get submatrix of this matrix.
		/// </summary>
		/// <param name="start_row">the first row to get.</param>
		/// <param name="start_col">the first col to get.</param>
		/// <param name="rows">the number of rows to get.</param>
		/// <param name="cols">the number of cols to get.</param>
		/// <returns>submatrix of this.</returns>
		public Matrix SubMatrix(int start_row, int start_col, int rows, int cols)
		{
			if (start_row < 0 || start_col < 0 || rows <= 0 || cols <= 0 ||  start_row + rows > this.Row || start_col + cols > this.Col)
			{
				throw (new ArgumentException());
			}

			Matrix m = new Matrix(rows, cols);

			for (int i = 0; i < m.Row; i++)
			{
				for (int j = 0; j < m.Col; j++)
				{
					m.Val[i][j] = Val[start_row + i][start_col + j];
				}
			}

			return m;
		}

		/// <summary>
		/// Get remain sub.
		/// </summary>
		/// <param name="start_row">the first row of sub.</param>
		/// <param name="start_col">the first col of sub.</param>
		/// <param name="rows">the number of rows of sub.</param>
		/// <param name="cols">the number of cols of sub.</param>
		/// <returns>remain submatrix of this.(null if no remain sub.)</returns>
		public Matrix RemainMatrix(int start_row, int start_col, int rows, int cols)
		{
			if (rows >= Row || cols >= Col)
			{
				return null;
				//throw (new ArgumentException());
			}
			if (start_row < 0 || start_col < 0 || rows <= 0 || cols <= 0 || start_row + rows > this.Row || start_col + cols > this.Col)
			{
				throw (new ArgumentException());
			}

			Matrix m = new Matrix(Row - rows, Col - cols);

			for (int i = 0; i < Row; i++)
			{
				for (int j = 0; j < Col; j++)
				{
					if (i < start_row && j < start_col)
					{
						m.Val[i][j] = Val[i][j];
					}
					else if (i < start_row && j >= start_col + cols)
					{
						m.Val[i][j - cols] = Val[i][j];
					}
					else if (i >= start_row + rows && j < start_col)
					{
						m.Val[i - rows][j] = Val[i][j];
					}
					else if (i >= start_row + rows && j >= start_col + cols)
					{
						m.Val[i - rows][j - cols] = Val[i][j];
					}
				}
			}

			return m;
		}

		/// <summary>
		/// Get remain sub.
		/// </summary>
		/// <param name="start_row">the row index of sub.</param>
		/// <param name="start_col">the col index of sub.</param>
		/// <returns>remain submatrix of this.(null if no remain sub.)</returns>
		public Matrix RemainMatrix(int row, int col)
		{
			return RemainMatrix(row, col, 1, 1);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Determines whether two vectors are exactly equal.
		/// </summary>
		/// <param name="a">The first vector.</param>			/// <param name="b">The second vector.</param>
		/// <returns><c>true</c> if the two vectors are equal; otherwise, <c>false</c>.</returns>
		public static bool Compare(MatrixSet a, MatrixSet b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			else if ((object)a == null && (object)b != null)
			{
				return false;
			}
			else
			{
				return a.Equals(b);
			}
		}
		
		public static bool operator ==(MatrixSet a, MatrixSet b)
		{
			return Compare(a, b);
		}

		public static bool operator !=(MatrixSet a, MatrixSet b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Adds two matrixes to produce a new matrix.
		/// </summary>
		/// <param name="a">The first matrix.</param>
		/// <param name="b">The second matrix.</param>
		/// <returns>The matrix <paramref name="a"/> + <paramref name="b"/>.</returns>
		public static Matrix Add(MatrixSet a, MatrixSet b)
		{
			if (a.Dimension != b.Dimension)
			{
				throw (new DimensionsNotEqualException());
			}

			Vector[] vectors = new Vector[a.Col];

			for (int i = 0; i < a.Col; i++)
			{
				vectors[i] = new Vector(a.Cols[i] + b.Cols[i]);
			}

			return new Matrix(vectors);
		}

		public static Matrix operator +(MatrixSet a, MatrixSet b)
		{
			return Add(a, b);
		}

		/// <summary>
		/// Multiplies a matrix by a scalar to produce a new matrix.
		/// </summary>
		/// <param name="a">A scalar.</param>
		/// <param name="b">A matrix.</param>
		/// <returns>The matrix <paramref name="a"/>*<paramref name="b"/>.</returns>		
		public static Matrix Multiply(double a, MatrixSet b)
		{
			Vector[] vectors = new Vector[b.Col];

			for (int i = 0; i < b.Col; i++)
			{
				vectors[i] = new Vector(a * b.Cols[i]);
			}

			return new Matrix(vectors);
		}

		public static Matrix operator *(double a, MatrixSet b)
		{
			return Multiply(a, b);
		}

		public static Matrix operator *(MatrixSet a, double b)
		{
			return Multiply(b, a);
		}

		public static Matrix operator /(MatrixSet a, double b)
		{
			return Multiply(1 / b, a);
		}

		public static Matrix Negative(MatrixSet a)
		{
			return Multiply(-1, a);
		}

		/// <summary>
		/// Subtracts a matrix from a matrix to produce a new matrix.
		/// </summary>
		/// <param name="a">The first matrix.</param>
		/// <param name="b">The second matrix.</param>
		/// <returns>The matrix <paramref name="a"/> - <paramref name="b"/>.</returns>
		public static Matrix Subtract(MatrixSet a, MatrixSet b)
		{
			if (a.Dimension != b.Dimension)
			{
				throw (new DimensionsNotEqualException());
			}

			Vector[] vectors = new Vector[a.Col];

			for (int i = 0; i < a.Col; i++)
			{
				vectors[i] = new Vector(a.Cols[i] - b.Cols[i]);
			}

			return new Matrix(vectors);
		}

		public static Matrix operator -(MatrixSet a)
		{
			return Negative(a);
		}

		public static Matrix operator -(MatrixSet a, MatrixSet b)
		{
			return Subtract(a, b);
		}

		/// <summary>
		/// Multiplies a matrix by a matrix to produce a new matrix.
		/// </summary>
		/// <param name="a">the first matrix.</param>
		/// <param name="b">the second matrix.</param>
		/// <returns>The matrix <paramref name="a"/>*<paramref name="b"/>.</returns>		
		public static Matrix Multiply(MatrixSet a, MatrixSet b)
		{
			if (a.Col != b.Row)
			{
				throw (new MatrixMultiplyException());
			}

			Matrix m = new Matrix(a.Row, b.Col);

			for (int i = 0; i < a.Row; i++)
			{
				for (int j = 0; j < b.Col; j++)
				{
					m.Val[i][j] = a.Rows[i] * b.Cols[j];
				}
			}

			return m;
		}

		public static Matrix operator *(MatrixSet a, MatrixSet b)
		{
			return Multiply(a, b);
		}

		public static Vector operator *(Vector a, MatrixSet b)
		{
			return Multiply(a.ToMatrix().Translate(), b).Rows[0];
		}

		public static Vector operator *(MatrixSet a, Vector b)
		{
			return Multiply(a, b.ToMatrix()).Cols[0];
		}

		#endregion
	}
}
