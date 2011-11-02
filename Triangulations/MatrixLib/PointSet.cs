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
//Modified on 02-08-2004

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;
	using Akira.MatrixLib.Exceptions;
	/// <summary>
	/// The base class of points.
	/// </summary>
	public abstract class PointSet
	{
		#region Static fields

		public static readonly double ZeroTolerance = 0.00000001;

		#endregion

		#region Member fields

		protected double[]	Ponderance = null;
		protected int Size = 0;
		
		#endregion
		
		#region Constructors
		
		protected PointSet() : this(1)
		{

		}

		protected PointSet(int size)
		{
			Ponderance = new double[size];
			Size = size;	
		}

		protected PointSet(double[] d)
		{
			Ponderance = d;
			if (d != null)
			{
				Size = d.Length;
			}
		}

		protected PointSet(PointSet p) : this(p.Size)
		{
			for (int i = 0; i < Size; i++)
			{
				Ponderance[i] = p.Ponderance[i];
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// The Dimension of this point.
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
		/// Determines whether this point is regular (finite and well-defined).
		/// </summary>
		/// <returns><c>true</c> if both coordinates are finite and well-defined; <c>false</c> otherwise.</returns>
		public bool isRegular
		{
			get
			{
				bool bRegular = true;

				if (Size > 0)
				{
					foreach (double d in Ponderance)
					{
						bRegular &= !(Double.IsNaN(d) || Double.IsInfinity(d));
					}
				}
				else
				{
					bRegular = false;
				}

				return bRegular;
			}
		}

		/// <summary>
		/// The entropy of this point. H(Point(a,b)) = -aloga-blogb
		/// </summary>
		/// <value>H(Point(a,b)).</value>
		public double Entropy
		{
			get
			{
				return this.ToVector().Entropy;
			}
		}

		/// <summary>
		/// The <paramref name="i"/>the coordinate array of this point.
		/// </summary>
		internal double[] Values
		{
			get
			{
				return this.Ponderance;
			}
		}

		#endregion

		#region Indexer

		/// <summary>
		/// The <paramref name="index"/>th coordinate of this point.
		/// </summary>
		/// <param name="index">The index of the coordinate (must be less than the dimension).</param>
		/// <value>The <paramref name="index"/>th coordinate of this point.</value>
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

		#region Override Methods

		/// <summary>
		/// Determines whether the specified object is equal to this point.
		/// </summary>
		/// <param name="obj">The object to compare with this point.</param>
		/// <returns><c>true</c> if the specified object is equal to this point; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj)
		{
			if (!isRegular)
			{
				throw (new PointNotRegularException());
			}

			bool isEqual = true;

			if (obj is PointSet)
			{
				if (this.Dimension != ((PointSet)obj).Dimension)
				{
					isEqual = false;
				}
				else
				{
					for (int i = 0; i < this.Dimension; i++)
					{
						if (Math.Abs(this[i] - ((PointSet)obj)[i]) > ZeroTolerance)
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

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			if (isRegular)
			{
				StringBuilder stringBuilder = new StringBuilder();

				stringBuilder.Append("(");
				foreach (double v in Ponderance)
				{
					stringBuilder.Append(v.ToString());
					stringBuilder.Append(",");
				}

				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
			else
			{
				return "PointNotRegular";
			}
		}


		#endregion

		#region Public Methods

		/// <summary>
		/// Convert a point to a vector. Vector[a,b]=Point(a,b)-Point(0,0)
		/// </summary>
		/// <returns>the vector specified by this point and point(0,0).</returns>
		public Vector ToVector()
		{
			if (isRegular)
			{
				return new Vector(this.Ponderance);
			}
			else
			{
				throw (new PointNotRegularException());
			}
		}
        
		/// <summary>
		/// Move a point to a new place throught a vector.
		/// </summary>
		/// <param name="v">The vector specified the path to move.</param>
		/// <returns>The new site of this point.</returns>
		public Point MoveForward(Vector v)
		{
			Add(this, v).Values.CopyTo(this.Values, 0);
			return (Point)this;
		}

		/// <summary>
		/// Move a point back to a previous place throught a vector
		/// </summary>
		/// <param name="v">The vector specified the path to move.</param>
		/// <returns>The new site of this point.</returns>
		public Point MoveBack(Vector v)
		{
			Subtract(this, v).Values.CopyTo(this.Values, 0);
			return (Point)this;
		}

		#endregion

		#region Operator

		/// <summary>
		/// The geometry distance between this two points.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns>The distance between them.</returns>
		public static double Distance(PointSet a, PointSet b)
		{
			return Subtract(a, b).Length;
		}

		/// <summary>
		/// Determines whether two pionts are exactly equal.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns><c>true</c> if the two vectors are equal; otherwise, <c>false</c>.</returns>
		public static bool Compare(PointSet a, PointSet b)
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
		/// Subtracts a point from a point to produce a vector.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns>The vector <paramref name="a"/> - <paramref name="b"/>.</returns>
		public static Vector Subtract(PointSet a, PointSet b)
		{
			if (!a.isRegular || !b.isRegular)
			{
				throw (new PointNotRegularException());
			}

			if (a.Dimension != b.Dimension)
			{
				throw (new DimensionsNotEqualException());
			}

			double[] d = new double[a.Dimension];

			for (int i = 0; i < a.Dimension; i++)
			{
				d[i] = a[i] - b[i];
			}

			return new Vector(d);
		}

		/// <summary>
		/// Subtracts a vector from a point to produce a new point.
		/// </summary>
		/// <param name="a">A point.</param>
		/// <param name="b">A vector.</param>
		/// <returns>The point <paramref name="a"/> - <paramref name="b"/>.</returns>		
		public static Point Subtract(PointSet a, Vector b)
		{
			return Subtract(a, b.ToPoint()).ToPoint();
		}

		public static Vector operator -(PointSet a, PointSet b)
		{
			return Subtract(a, b);
		}

		public static Point operator -(PointSet a, Vector b)
		{
			return Subtract(a, b.ToPoint()).ToPoint();
		}

		public static Point operator -(PointSet a)
		{
			return Multiply(-1, a);
		}

		/// <summary>
		/// Adds a point and a vector to produce a new point.
		/// </summary>
		/// <param name="a">A point.</param>
		/// <param name="b">A vector.</param>
		/// <returns>The point <paramref name="a"/> + <paramref name="b"/>.</returns>
		public static Point Add(PointSet a, Vector b)
		{
			return Vector.Add(a.ToVector(), b).ToPoint();
		}

		public static Point operator +(PointSet a, Vector b)
		{
			return Add(a, b);
		}

		public static bool operator ==(PointSet a, PointSet b)
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

		public static bool operator !=(PointSet a, PointSet b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Multiplies a point by a scalar to produce a new point.
		/// </summary>
		/// <param name="a">A scalar.</param>
		/// <param name="b">A point.</param>
		/// <returns>The image of <paramref name="b"/> in the scaling centered at the 
		/// origin with ratio <paramref name="a"/>.</returns>
		public static Point Multiply(double a, PointSet b)
		{
			if (!b.isRegular)
			{
				throw (new PointNotRegularException());
			}

			double[] d = new double[b.Dimension];

			for (int i = 0; i < b.Dimension; i++)
			{
				d[i] = a * b[i];
			}

			return new Point(d);
		}

		public static Point operator *(double a, PointSet b)
		{
			return Multiply(a, b);
		}

		public static Point operator *(PointSet a, double b)
		{
			return Multiply(b, a);
		}

		public static Point operator /(PointSet a, double b)
		{
			return Multiply(1 / b, a);
		}

		/// <summary>
		/// Returns the point midway between two points.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns>The point (<paramref name="a"/> + <paramref name="b"/>)/2.</returns>
		public static Point MidPoint(PointSet a, PointSet b)
		{
			if (!a.isRegular || !b.isRegular)
			{
				throw (new PointNotRegularException());
			}

			return ((a.ToVector() + b.ToVector()) / 2).ToPoint();
		}

		#endregion

		#region Type Converts

		/// <summary>
		/// Explicitly convert a point to a vector.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the vector.</returns>
		public static explicit operator Vector(PointSet point)
		{
			return point.ToVector();
		}

		/// <summary>
		/// Explicitly convert a point2D to a vector2D.
		/// </summary>
		/// <param name="point2D">the point.</param>
		/// <returns>the vector.</returns>
		public static explicit operator Vector2D(PointSet point2D)
		{
			if (point2D.Dimension == 2)
			{
				return (Vector2D)point2D.ToVector();
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}

		#endregion
	}
}
