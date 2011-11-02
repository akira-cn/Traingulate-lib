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
	/// Summary description for VectorSet.
	/// </summary>
	public abstract class VectorSet
	{
		#region Static fields

		public static readonly double ZeroTolerance = 0.00000001;

		#endregion

		#region Member fileds

		protected int Size = 1;
		protected double[] Ponderance = null;

        #endregion

		#region Properties

		/// <summary>
		/// The Dimension of this vector.
		/// </summary>
		/// <value>integer Dimension.</value>		
		public int Dimension
		{
			get
			{
				return Size;
			}
		}

		/// <summary>
		/// The Length of this vector.
		/// </summary>
		/// <value>double Length.</value>
		public double Length
		{
			get
			{
				double length = 0.0;

				foreach (double v in Ponderance)
				{
					length += v * v;
				}

				return Math.Sqrt(length);
			}
		}

		/// <summary>
		/// Returns the square of the length of this vector.
		/// </summary>
		/// <returns>The square of the length of this vector.</returns>
		public double SquaredLength
		{
			get
			{
				double length = 0.0;

				foreach (double v in Ponderance)
				{
					length += v * v;
				}

				return length;
			}
		}

		/// <summary>
		/// The entropy of this vector. H(Vector(a,b)) = -aloga-blogb
		/// </summary>
		/// <value>H(Vector(a,b)).</value>
		public double Entropy
		{
			get
			{
				double entropy = 0.0;

				foreach (double v in Ponderance)
				{
					try
					{
						if (v < 0)
						{
							throw (new InvalidateEntropyException());
						}
					}
					catch (InvalidateEntropyException)
					{
						return double.NaN;
					}
					if (v < ZeroTolerance)
					{
						continue;
					}
					entropy += -v * Math.Log(v, 2);
				}

				return entropy;
			}
		}

		/// <summary>
		/// Determines whether this vector is zero.
		/// </summary>
		/// <value>true if zero.</value>
		public bool isZero
		{
			get
			{
				return Length < ZeroTolerance;
			}
		}

		/// <summary>
		/// The <paramref name="i"/>the coordinate array of this vector.
		/// </summary>
		public double[] Values
		{
			get
			{
				return this.Ponderance;
			}
		}

		#endregion

		#region Indexer

		/// <summary>
		/// The <paramref name="index"/>th coordinate of this vector.
		/// </summary>
		/// <param name="index">The index of the coordinate (must be less than the dimension).</param>
		/// <value>The <paramref name="index"/>th coordinate of this vector.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not less than the dimesion.</exception>
		public double this[int index]
		{
			get
			{
				if (index < Dimension)
				{
					return Ponderance[index];
				}
				else
				{
					throw new ArgumentOutOfRangeException("index", index, "Index must be less than Dimension.");
				}
			}
			set
			{
				if (index < Dimension)
				{
					Ponderance[index] = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("index", index, "Index must be less than Dimension");
				}
			}
		}

		#endregion

		#region Constructor

		protected VectorSet()
		{
			Size = 1;
			Ponderance = new double[1];
			Ponderance[0] = 0;
		}

		protected VectorSet(int size)
		{
			Size = size;
			Ponderance = new double[Dimension];
		}

		protected VectorSet(double[] pond)
		{
			Size = pond.Length;
			Ponderance = pond;
		}

		protected VectorSet(VectorSet a)
		{
			Size = a.Dimension;
			Ponderance = new double[Dimension];
			for (int i = 0; i < Dimension; i++)
			{
				Ponderance[i] = a[i];
			}
		}

		#endregion

		#region Override Methods
		/// <summary>
		/// Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="obj">The object to compare with this vector.</param>
		/// <returns><c>true</c> if the specified object is equal to this vector; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj)
		{
			bool isEqual = true;

			if (obj is VectorSet)
			{
				if (this.Dimension != ((VectorSet)obj).Dimension)
				{
					isEqual = false;
				}
				else
				{
					for (int i = 0; i < this.Dimension; i++)
					{
						if (this[i] != ((VectorSet)obj)[i])
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
			
			foreach (double v in Ponderance)
			{
				hashCode ^= v.GetHashCode();
			}

			return hashCode;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");

			foreach (double v in Ponderance)
			{
				stringBuilder.Append(v.ToString());
				stringBuilder.Append(",");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append("]");

			return stringBuilder.ToString();
		}


		#endregion

		#region Public methods

		/// <summary>
		/// Extend this vector to a higher dimension vector.
		/// </summary>
		/// <param name="n">The level to extend.</param>
		/// <returns>new vector.</returns>
		public Vector Extend(int n)
		{
			double[] pond = new double[Dimension + n];

			this.Ponderance.CopyTo(pond, 0);

			return new Vector(pond);
		}

		/// <summary>
		/// Extend this vector to a higher dimension vector.
		/// </summary>
		/// <returns>new vector.</returns>
		public Vector Extend()
		{
			return Extend(1);
		}

		/// <summary>
		/// The opposite direction of the vector.
		/// </summary>
		/// <returns>new vector.</returns>
		public Vector OppositeDirections()
		{
			return VectorSet.Negate(this);
		}

		/// <summary>
		/// Get plumb of this vector. (e.g.Vector[1,2,3].ToPlumb(1) = Vector[0,2,0]
		/// </summary>
		/// <param name="index">The index of the coordinate.</param>
		/// <returns>new vector.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not less than the dimesion.</exception>
		public Vector ToPlumb(int index)
		{
			double[] d = new double[Dimension];

			if (index >= this.Dimension)
			{
				throw (new ArgumentOutOfRangeException());
			}
			
			for (int i = 0; i < Dimension; i++)
			{
				if (i == index)
				{
					d[i] = Ponderance[i];
					break;
				}
			}
			return new Vector(d);
		}

		/// <summary>
		/// Normalize this vector.
		/// </summary>
		/// <returns>the length of the vector.</returns>
		/// <exception cref="ZeroVectorToUnitException">Thrown if length is zero.</exception>
		public Vector ToUnit()
		{
			if (!isZero)
			{
				return Multiply(1 / this.Length, this);
			}
			else
			{
				throw new ZeroVectorToUnitException();
			}
		}

		/// <summary>
		/// Normalize this vector.
		/// </summary>
		/// <returns>the length of the vector.</returns>
		/// <exception cref="ZeroVectorToUnitException">Thrown if length is zero.</exception>
		public Vector Normalize()
		{
			return this.ToUnit();
		}

		/// <summary>
		/// Get unit plumb of this vector. (e.g.Vector[1,2,3].ToPlumb(1) = Vector[0,1,0]
		/// </summary>
		/// <param name="index">The index of the coordinate.</param>
		/// <returns>new vector.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not less than the dimesion.</exception>
		public Vector ToUnitPlumb(int index)
		{
			Vector v = ToPlumb(index);

			return v.ToUnit();
		}
		
		/// <summary>
		/// Convert the vector to a Matrix. (vector as cols.)
		/// </summary>
		/// <returns>the matrix.</returns>
		public Matrix ToMatrix()
		{
			return new Matrix(new Vector(this.Values));
		}

		/// <summary>
		/// Convert the vector to a Point.
		/// </summary>
		/// <returns>the point.</returns>
		public Point ToPoint()
		{
			return new Point(Values);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Determines whether two vectors are exactly equal.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns><c>true</c> if the two vectors are equal; otherwise, <c>false</c>.</returns>
		public static bool Compare(VectorSet a, VectorSet b)
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

		/// <summary>
		/// Determines whether two vectors are exactly equal.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns><c>true</c> if the two vectors are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(VectorSet a, VectorSet b)
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

		public static bool operator !=(VectorSet a, VectorSet b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Multiplies a vector by a scalar to produce a new vector.
		/// </summary>
		/// <param name="a">A scalar.</param>
		/// <param name="b">A vector.</param>
		/// <returns>The vector <paramref name="a"/>*<paramref name="b"/>.</returns>		
		public static Vector Multiply(double a, VectorSet b)
		{
			Vector v = new Vector(b.Dimension);

			for (int i = 0; i < b.Dimension; i++)
			{
				v[i] = b[i] * a;
			}

			return v;
		}

		public static Vector operator *(double a, VectorSet b)
		{
			return Multiply(a, b);
		}

		public static Vector operator *(VectorSet a, double b)
		{
			return b * a;
		}

		public static Vector operator /(VectorSet a, double b)
		{
			return 1 / b * a;
		}
		
		/// <summary>
		/// Returns the dot product of two vectors.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The dot product of <paramref name="a"/> and <paramref name="b"/>.</returns>
		public static double Dot(VectorSet a, VectorSet b)
		{
			if (a.Dimension != b.Dimension)
			{
				throw (new DimensionsNotEqualException());
			}
			
			double d = 0.0;
			for (int i = 0; i < a.Dimension; i++)
			{
				d += a[i] * b[i];
			}

			return d;
		}

		public static double operator *(VectorSet a, VectorSet b)
		{
			return Dot(a, b);
		}

		/// <summary>
		/// Negate a vector.
		/// </summary>
		/// <param name="v">A vector.</param>
		/// <returns>The vector -<paramref name="v"/>.</returns>
		public static Vector Negate(VectorSet a)
		{
			return Multiply(-1, a);
		}

		public static Vector operator -(VectorSet a)
		{
			return Negate(a);
		}

		/// <summary>
		/// Adds two vectors to produce a new vector.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The vector <paramref name="a"/> + <paramref name="b"/>.</returns>
		public static Vector Add(VectorSet a, VectorSet b)
		{
			if (a.Dimension != b.Dimension)
			{
				throw (new DimensionsNotEqualException());
			}

			Vector v = new Vector(a.Dimension);

			for (int i = 0; i < ((VectorSet)a).Dimension; i++)
			{
				v[i] = a[i] + b[i];
			}

			return v;
		}

		/// <summary>
		/// Subtracts a vector from a vector to produce a new vector.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The vector <paramref name="a"/> - <paramref name="b"/>.</returns>
		public static Vector Subtract(VectorSet a, VectorSet b)
		{
			return Add(a, Negate(b));
		}

		public static Vector operator +(VectorSet a, VectorSet b)
		{
			return Add(a, b);
		}

		public static Vector operator -(VectorSet a, VectorSet b)
		{
			return Subtract(a, b);
		}

		#endregion

		#region Type converts

		/// <summary>
		/// Explicitly convert a Vector to a Matrix.
		/// </summary>
		/// <param name="vector">the vector.</param>
		/// <returns>the matrix.</returns>
		public static explicit operator Matrix(VectorSet vector)
		{
			return new Matrix(new Vector(vector.Ponderance));
		}

		/// <summary>
		/// Explicitly convert a Vector to a Point.
		/// </summary>
		/// <param name="vector">the vector.</param>
		/// <returns>the point.</returns>
		public static explicit operator Point(VectorSet vector)
		{
			return vector.ToPoint();
		}

		#endregion
	}
}
