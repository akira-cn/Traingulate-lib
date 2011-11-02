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

namespace Akira.Geometry
{
	using System;
	using Akira.MatrixLib;
	using System.Diagnostics;

	/// <summary>
	/// A Line Discribed by Vector[A,B,C] : Ax + By + C = 0
	/// </summary>
	public class Line2D
	{
		#region Static fields
		static readonly double ZeroTolerance = 0.000001;
		#endregion

		#region Member fields
		public enum Position { Intersect, Parallel, Superposition };
		private Vector vector = new Vector(3);
		#endregion

		#region Constructors
		public Line2D(double a, double b, double c)
		{
			if (a == 0 && b == 0)
			{
				throw (new ArgumentException());
			}

			vector[0] = a;
			vector[1] = b;
			vector[2] = c;
		}

		public Line2D(Line2D line) : this(line.A, line.B, line.C)
		{
			
		}

		public Line2D(Vector v)
		{
			if (v.Dimension != 3)
			{
				throw (new ArgumentException());
			}

			if (v[0] == 0 && v[1] == 0)
			{
				throw (new ArgumentException());
			}

			vector[0] = v[0];
			vector[1] = v[1];
			vector[2] = v[2];
		}
		#endregion

		#region Properties
		/// <summary>
		/// The coefficient of x.
		/// </summary>
		/// <value>the coefficient of x.</value>
		public double A
		{
			get
			{
				return vector[0];
			}
		}
		/// <summary>
		/// The coefficient of y.
		/// </summary>
		/// <value>the coefficient of y.</value>
		public double B
		{
			get
			{
				return vector[1];
			}
		}
		/// <summary>
		/// The last coefficient.
		/// </summary>
		/// <value>the last coefficient.</value>
		public double C
		{
			get
			{
				return vector[2];
			}
		}

		/// <summary>
		/// The slope of the line.
		/// </summary>
		/// <value>the slope.</value>
		public double K
		{
			get
			{
				if (B == 0)
				{
					return double.PositiveInfinity;
				}
				else
				{
					return -A / B;
				}
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Discribe the position of the two lines.
		/// </summary>
		/// <param name="line">the other line.</param>
		/// <returns>the position.</returns>
		public Position GetPosition(Line2D line)
		{
			if (this.A == 0 && line.A == 0)
			{
				if (this.B * line.C != this.C * line.B)
				{
					return Position.Parallel;
				}
				else
				{
					return Position.Superposition;
				}
			}

			if (this.B == 0 && line.B == 0)
			{
				if (this.A * line.C != this.C * line.A)
				{
					return Position.Parallel;
				}
				else
				{
					return Position.Superposition;
				}
			}

			Vector v1 = null;
			Vector v2 = null;

			if (Math.Abs(this.A) > Math.Abs(line.A))
			{
				v1 = new Vector(this.vector);
				v2 = new Vector(line.vector);
			}
			else
			{
				v1 = new Vector(line.vector);
				v2 = new Vector(this.vector);
			}

			v1 = Vector.Multiply(v2[0] / v1[0], v1);
			v1 = Vector.Subtract(v1,v2);

			if (Math.Abs(v1[1]) < Line2D.ZeroTolerance && Math.Abs(v1[2]) < Line2D.ZeroTolerance)
			{
				return Position.Superposition;
			}
			else if (Math.Abs(v1[1]) < Line2D.ZeroTolerance)
			{
				return Position.Parallel;
			}
			else
			{
				return Position.Intersect;
			}
		}

		/// <summary>
		/// Get the intersect point of the two lines.
		/// </summary>
		/// <param name="line">the other line.</param>
		/// <returns>the point.</returns>
		public Point2D Intersects(Line2D line)
		{
			if (this.GetPosition(line) != Position.Intersect)
			{
				//throw (new ArgumentException("The two lines cannot Intersects!"));
				return new Point2D(double.PositiveInfinity, double.PositiveInfinity);
			}

			Vector v1 = null;
			Vector v2 = null;

			if (Math.Abs(this.A) > Math.Abs(line.A))
			{
				v1 = new Vector(this.vector);
				v2 = new Vector(line.vector);
			}
			else
			{
				v1 = new Vector(line.vector);
				v2 = new Vector(this.vector);
			}

			Vector v3 = new Vector(v1);

			v1 = Vector.Multiply(v2[0] / v1[0], v1);
			v1 = Vector.Subtract(v1, v2);
			v1 = Vector.Multiply(1/v1[1], v1);

			Point2D point = new Point2D(new Line2D(v3).GetX(-v1[2]), -v1[2]);

			if (!this.Contains(point) || !line.Contains(point))
			{
				throw (new ArgumentException());
			}

			return point;
		}

		/// <summary>
		/// Let y = b and get x.
		/// </summary>
		/// <param name="b">y = b.</param>
		/// <returns>x.</returns>
		public double GetX(double b)
		{
			if (this.A == 0)
			{
				//throw (new ApplicationException());
				return double.NaN;
			}

			return (-this.B * b - this.C) / this.A;
		}

		/// <summary>
		/// Let x = a and get y.
		/// </summary>
		/// <param name="a">x = a.</param>
		/// <returns>y</returns>
		public double GetY(double a)
		{
			if (this.B == 0)
			{
				//throw (new ApplicationException());
				return Double.NaN;
			}

			return (-this.A * a - this.C) / this.B;
		}

		/// <summary>
		/// Wether the line contains the point.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if contains.</returns>
		public bool Contains(Point2D point)
		{
			double d = point.X * this.A + point.Y * this.B + this.C;
			if (d < Line2D.ZeroTolerance)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
