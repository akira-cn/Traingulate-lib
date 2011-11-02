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
	/// Represents a new geometry vector2D.
	/// </summary>
	public class Vector2D : VectorSet
	{
		#region Static fields

		public static readonly Vector2D Zero = new Vector2D();
		public static readonly Vector2D UnitX = new Vector2D(1, 0);
		public static readonly Vector2D UnitY = new Vector2D(0, 1);

		#endregion

		#region Constructors

		public Vector2D() : base(2)
		{

		}

		public Vector2D(double x, double y) : base(new double[] {x, y})
		{
			
		}

		public Vector2D(Vector2D v) : base(v)
		{

		}

		private Vector2D(double[] pond) : base(pond)
		{

		}

		#endregion

		#region Property
		
		public double X
		{
			get
			{
				return Ponderance[0];
			}
			set
			{
				Ponderance[0] = value;
			}
		}

		public double Y
		{
			get
			{
				return Ponderance[1];
			}
			set
			{
				Ponderance[1] = value;
			}
		}
		/// <summary>
		/// [0,2*Pi]
		/// </summary>
		/// <value></value>
		public double Arg
		{
			get
			{
				double arg = 0.0;

				if (Math.Abs(X) > Vector.ZeroTolerance)
				{
					arg = Math.Atan(Y / X);
					if (X < -Vector.ZeroTolerance)
					{
						arg += Math.PI;
					}
					else if (arg < 0)
					{
						arg += 2 * Math.PI;
					}
				}
				else if (Y > Vector.ZeroTolerance)
				{
					arg = (Math.PI / 2);
				}
				else if (Y < -Vector.ZeroTolerance)
				{
					arg = (3 * Math.PI / 2);
				}

				return arg;
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// A translate of the Vector2D. (e.g. Vector[2,1].Translate() = Vector[1,2])
		/// </summary>
		/// <returns>new vector.</returns>
		public Vector2D Translate()
		{
			return new Vector2D(Y, X);
		}

		/// <summary>
		/// Sets this vector using the given coordinates.
		/// </summary>
		/// <param name="x">The horizontal coordinate.</param>
		/// <param name="y">The vertical coordinate.</param>
		public void SetFromXY(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// Rotates this vector counter-clockwise about the origin by <paramref name="radians"/> radians
		/// </summary>
		/// <param name="radians">Number of radians to rotate by</param>
		public Vector2D Rotate(double radians)
		{
			Vector2D v = new Vector2D(this);

			double cos = Math.Cos(radians);
			double sin = Math.Sin(radians);
			v.X = this.X * cos - this.Y * sin;
			v.Y = this.X * sin + this.Y * cos;

			return v;
		}

		/// <summary>
		/// Returns a vector perpendicular to this vector. Equivalent to rotating this vector 90 degrees clockwise.
		/// </summary>
		/// <returns>A vector perpendicular to this vector.</returns>
		public Vector2D Normal()
		{
			return new Vector2D(Y, -X);
		}		
		
		/// <summary>
		/// Returns a normalized vector perpendicular to this vector. Equivalent to rotating this vector 90 degrees clockwise.
		/// </summary>
		/// <returns>A normalized vector perpendicular to this vector.</returns>
		public Vector2D UnitNormal()
		{
			Vector2D normal = new Vector2D(Y, -X);

			return normal.Normalize();;
		}
		#endregion

		#region Operator

		/// <summary>
		/// Returns the cross product of two vectors. The cross product of the 2 2D vectors a and b is defined
		/// as the determinant of the 2 by 2 matrix (a, b) where the vectors are written in row or column order.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The cross product of <paramref name="a"/> and <paramref name="b"/>.</returns>
		public static Vector2D Cross(Vector2D a, Vector2D b)
		{
			return new Vector2D(a.X * b.Y , - a.Y * b.X);
		}

		/// <summary>
		/// Returns the determinant of two vectors, that is, the determinant of the 2 x 2 matrix they form.
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The determinant of <paramref name="a"/> and <paramref name="b"/>.</returns>
		public static double Determinant(Vector2D a, Vector2D b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		/// <summary>
		/// Returns the angle between two vectors. (-pi, pi]
		/// </summary>
		/// <param name="a">the first vector.</param>
		/// <param name="b">the second vector.</param>
		/// <returns>the angle.</returns>
		public static double Angle(Vector2D a, Vector2D b)
		{
			double d = b.Arg - a.Arg;

			if (d > Math.PI)
			{
				d -= 2 * Math.PI;
			}
			else if (d < -Math.PI)
			{
				d += 2 * Math.PI;
			}

			return d;
		}

		/// <summary>
		/// Returns the angle by rotate a around b. [0, 2pi)
		/// </summary>
		/// <param name="a">the first vector.</param>
		/// <param name="b">the second vector.</param>
		/// <returns>the angle.</returns>
		public static double RotateAngle(Vector2D a, Vector2D b)
		{
			double d = b.Arg - a.Arg;

			if (d < 0)
			{
				d += 2 * Math.PI;
			}
			return d;
		}
		#endregion

		#region Type converts

		/// <summary>
		/// Implicitly convert a Vector2D to a Vector.
		/// </summary>
		/// <param name="vector2D">the vector2D.</param>
		/// <returns>the vector.</returns>
		public static implicit operator Vector(Vector2D vector2D)
		{
			return new Vector(vector2D.Values);
		}

		/// <summary>
		/// Convert a Vector to a Vector2D.
		/// </summary>
		/// <param name="vector">the vector.</param>
		/// <returns>the vector2D.</returns>
		public static Vector2D ConvertFromVector(Vector vector)
		{
			if (vector.Dimension == 2)
			{
				return new Vector2D(vector.Values);
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}
