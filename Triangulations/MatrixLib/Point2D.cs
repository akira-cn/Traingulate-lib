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
	/// <summary>
	/// Summary description for Point2D.
	/// </summary>
	public class Point2D : PointSet
	{
		#region Static members

		public static readonly Point2D Zero = new Point2D();

		#endregion

		#region Constructors
		
		public Point2D() : base(2)
		{
			
		}

		private Point2D(double[] d) : base(d)
		{

		}

		public Point2D(Point2D p) : base(p)
		{

		}

		public Point2D(double x, double y) : base(new double[2]{x, y}) 
		{
			
		}

		#endregion

		#region Properties

		/// <summary>
		/// The horizontal coordinate of this point.
		/// </summary>
		/// <value>The horizontal coordinate of this point.</value>
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
		/// <summary>
		/// The vertical coordinate of this point.
		/// </summary>
		/// <value>The vertical coordinate of this point.</value>
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

		#endregion

		#region Public methods

		/// <summary>
		/// Sets this point using the given coordinates.
		/// </summary>
		/// <param name="x">The horizontal coordinate.</param>
		/// <param name="y">The vertical coordinate.</param>
		public void SetFromXY(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// Computes the shortest distance from this point to the line defined by the parameters.
		/// Points <paramref name="a"/> and <paramref name="b"/> must be distinct.
		/// </summary>
		/// <param name="a">The first point defining the line.</param>
		/// <param name="b">The second point defining the line.</param>
		/// <returns>The distance from this point to the line defined by the parameters.</returns>
		/// <exception cref="ArgumentException">
		///	The points defining the line are not distinct.
		/// </exception>
		public double DistanceToLine(Point2D a, Point2D b)
		{
			if (a == b)
			{
				throw new ArgumentException("The points defining the line are not distinct.");
			}
			else
			{
				double distanceAB = Distance(a, b);

				// dist(P, Line(A,B) = |det(AB,AP)| / d(A,B). 
				return Math.Abs(((b.X - a.X) * (this.Y - a.Y) - (this.X - a.X) * (b.Y - a.Y)) / distanceAB);
			}
		}

		public Point2D GetClosestPointOnLineSegment(Point2D a, Point2D b)
		{
			Point2D p = this.ParametrizationToLineSegment(a, b);

			return WeightedAverage(a, b, p.X);
		}

		#endregion

		#region Family methods

		/// <summary>
		/// Make a parametrization to a line segment specified by two other points.
		/// </summary>
		/// <param name="a">the first point of the line segment.</param>
		/// <param name="b">the second point of the line segment.</param>
		/// <returns></returns>
		protected Point2D ParametrizationToLineSegment(Point2D a, Point2D b)
		{
			if (a == b)
			{
				throw new ArgumentException("The points defining the line are not distinct.");
			}
			else
			{
				Vector2D AB = b - a;
				Vector2D AP = this - a;
				Point2D p = new Point2D();

				// x = AB * AP/|AB|^2 = |AP|cos<AB,AP>/|AB|
				p.X = Vector2D.Dot(AB, AP) / AB.SquaredLength;

				// y = |AB X AP|/|AB|^2 = |AP|sin<AB,AP>/|AB|
				p.Y = Vector2D.Determinant(AB, AP) / AB.SquaredLength;
				return p;
			}
		}

		#endregion

		#region Type Converts

		/// <summary>
		/// Convert a point to point2D.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the point2D. (If the dimension of the point is not 2,it returns null)</returns>
		public static Point2D ConvertFromPoint(Point point)
		{
			if (point.Dimension == 2)
			{
				return new Point2D(point.Values);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Implicitly convert a point2D to a point.
		/// </summary>
		/// <param name="point2D">the point2D.</param>
		/// <returns>the point.</returns>
		public static implicit operator Point(Point2D point2D)
		{
			return new Point(point2D.Values);
		}

		#endregion

		#region Operator

		/// <summary>
		/// Returns the weighted average of two points.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <param name="t">The weight.</param>
		/// <returns>The point (1 - <paramref name="t"/>)*<paramref name="a"/> + <paramref name="t"/>*<paramref name="b"/>.</returns>
		public static Point2D WeightedAverage(Point2D a, Point2D b, double t)
		{

			if (t <= 0)
			{
				return new Point2D(a);
			}
			else if (t >= 1)
			{
				return new Point2D(b);
			}
			else
			{
				double oneMinusT = 1 - t;

				return new Point2D((1 - t) * a + t * b.ToVector());
			}
		}

		/// <summary>
		/// Compares two points using lexicographic order.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns>-1 if <paramref name="a"/> is less than <paramref name="b"/>,
		/// 0 if <paramref name="a"/> equals <paramref name="b"/>,
		/// and 1 if <paramref name="a"/> is greater than <paramref name="b"/>.</returns>
		public static int Compare(Point2D a, Point2D b)
		{
			if (a.X > b.X || (a.X == b.X && a.Y > b.Y))
			{
				return 1;
			}
			else if (a.X < b.X || (a.X == b.X && a.Y < b.Y))
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}	
	
		/// <summary>
		/// Determines whether one point is lexicographically greater than a second point.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns><c>true</c> if <paramref name="a"/> is lexicographically greater than <paramref name="b"/>; otherwise, <c>false</c>.</returns>
		public static bool operator >(Point2D a, Point2D b)
		{
			return a.X > b.X || (a.X == b.X && a.Y > b.Y);
		}

		/// <summary>
		/// Determines whether one point is lexicographically less than a second point.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns><c>true</c> if <paramref name="a"/> is lexicographically less than <paramref name="b"/>; otherwise, <c>false</c>.</returns>
		public static bool operator <(Point2D a, Point2D b)
		{
			return a.X < b.X || (a.X == b.X && a.Y < b.Y);
		}

		/// <summary>
		/// Determines whether one point is lexicographically greater than or equal to a second point.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns><c>true</c> if <paramref name="a"/> is lexicographically greater than or equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
		public static bool operator >=(Point2D a, Point2D b)
		{
			return a.X > b.X || (a.X == b.X && a.Y >= b.Y);
		}

		/// <summary>
		/// Determines whether one point is lexicographically less than or equal to a second point.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		/// <returns><c>true</c> if <paramref name="a"/> is lexicographically less than or equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
		public static bool operator <=(Point2D a, Point2D b)
		{
			return a.X < b.X || (a.X == b.X && a.Y <= b.Y);
		}

		#endregion
	}
}
