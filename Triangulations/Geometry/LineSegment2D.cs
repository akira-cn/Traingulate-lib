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

	/// <summary>
	/// Summary description for LineSegment.
	/// </summary>
	public class LineSegment2D
	{
		#region Static fields
		public static readonly double ZeroTolerance = 0.00001;
		#endregion

		#region Menber fields
		private Point2D[] PointPair = null;
		#endregion

		#region Constructors
		public LineSegment2D(Point2D a, Point2D b)
		{
			PointPair = new Point2D[2];

			PointPair[0] = a;
			PointPair[1] = b;
		}
		#endregion

		#region Properties

		/// <summary>
		/// The first Point of this line segment.
		/// </summary>
		/// <value>the point.</value>
		public Point2D FirstPoint
		{
			get
			{
				return PointPair[0];
			}
		}

		/// <summary>
		/// The last Point of this line segment.
		/// </summary>
		/// <value>the point.</value>
		public Point2D LastPoint
		{
			get
			{
				return PointPair[1];
			}
		}

		/// <summary>
		/// The mid Point of this line segment.
		/// </summary>
		/// <value>the point.</value>
		public Point2D MidPoint
		{
			get
			{
				return Point2D.MidPoint(this.FirstPoint, this.LastPoint);
			}
		}

		/// <summary>
		/// The Length of this line segment.
		/// </summary>
		/// <value>the length.</value>
		public double Length
		{
			get
			{
				double x = PointPair[1].X - PointPair[0].X;
				double y = PointPair[1].Y - PointPair[0].Y;

				return Math.Sqrt(x * x + y * y);
			}
		}

		/// <summary>
		/// Whether the line segment is regular or not.
		/// </summary>
		/// <value>true if is regular.</value>
		public bool isRegular
		{
			get
			{
				return FirstPoint.isRegular && LastPoint.isRegular && (FirstPoint != LastPoint);
			}
		}

		#endregion

		#region Internal and Private methods
		/// <summary>
		/// Whether the two extents intersect each other.
		/// (x1,y1)-----------------------------
		/// -         Extent                   -
		/// ------------------------------(x2,y2)
		/// </summary>
		/// <param name="line">the other line.</param>
		/// <returns>true if intersect.</returns>
		private bool ExtentIntersects(LineSegment2D line)
		{
			double x1, y1;
			double x2, y2;
			double x3, y3;
			double x4, y4;

			x1 = Math.Min(this.FirstPoint.X, this.LastPoint.X);
			x2 = Math.Max(this.FirstPoint.X, this.LastPoint.X);
			y1 = Math.Min(this.FirstPoint.Y, this.LastPoint.Y);
			y2 = Math.Max(this.FirstPoint.Y, this.LastPoint.Y);
			x3 = Math.Min(line.FirstPoint.X, line.LastPoint.X);
			x4 = Math.Max(line.FirstPoint.X, line.LastPoint.X);
			y3 = Math.Min(line.FirstPoint.Y, line.LastPoint.Y);
			y4 = Math.Max(line.FirstPoint.Y, line.LastPoint.Y);
			if (x1 <= x4 && x2 >= x3 && y1 <= y4 && y2 >= y3)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Whether the other line segment's line can intersects this line segment.
		/// </summary>
		/// <param name="line">the other line segment.</param>
		/// <returns>true if intersects.</returns>
		internal bool LineIntersects(LineSegment2D line)
		{
			/*Vector2D v1 = this.FirstPoint - line.FirstPoint;
			Vector2D v2 = line.LastPoint - line.FirstPoint;
			Vector2D v3 = this.LastPoint - line.FirstPoint;

			if (Vector2D.Determinant(v1, v2) * Vector2D.Determinant(v2, v3) >= 0)
			{
				return true;
			}
			else
			{
				return false;
			}*/
			double x1 = this.FirstPoint.X - line.FirstPoint.X;
			double y1 = this.FirstPoint.Y - line.FirstPoint.Y;
			double x2 = line.LastPoint.X - line.FirstPoint.X;
			double y2 = line.LastPoint.Y - line.FirstPoint.Y;
			double x3 = this.LastPoint.X - line.FirstPoint.X;
			double y3 = this.LastPoint.Y - line.FirstPoint.Y;
			double d1 = x1 * y2 - x2 * y1;
			double d2 = x2 * y3 - x3 * y2;

			if (d1 * d2 >= 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Whether the other line segment's line can truly intersects this line segment.
		/// </summary>
		/// <param name="line">the other line segment.</param>
		/// <returns>true if truly intersects.</returns>
		internal bool TrulyLineIntersects(LineSegment2D line)
		{
			/*Vector2D v1 = this.FirstPoint - line.FirstPoint;
			Vector2D v2 = line.LastPoint - line.FirstPoint;
			Vector2D v3 = this.LastPoint - line.FirstPoint;

			if (Vector2D.Determinant(v1, v2) * Vector2D.Determinant(v2, v3) >= ZeroTolerance)
			{
				double d = Vector2D.Determinant(v1, v2) * Vector2D.Determinant(v2, v3);
				return true;
			}
			else
			{
				return false;
			}*/
			double x1 = this.FirstPoint.X - line.FirstPoint.X;
			double y1 = this.FirstPoint.Y - line.FirstPoint.Y;
			double x2 = line.LastPoint.X - line.FirstPoint.X;
			double y2 = line.LastPoint.Y - line.FirstPoint.Y;
			double x3 = this.LastPoint.X - line.FirstPoint.X;
			double y3 = this.LastPoint.Y - line.FirstPoint.Y;
			double d1 = x1 * y2 - x2 * y1;
			double d2 = x2 * y3 - x3 * y2;

			if (d1 * d2 >= ZeroTolerance)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Convert the line segment to a vector.
		/// </summary>
		/// <returns></returns>
		public Vector2D ToVector()
		{
			return new Vector2D(this.LastPoint - this.FirstPoint);
		}

		/// <summary>
		/// Whether the line segment contains the point or not.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if contains.</returns>
		public bool Contains(Point2D point)
		{
			if (point == FirstPoint || point == LastPoint)
			{
				return true;
			}
			else
			{
				Vector2D v1 = point - FirstPoint;
				Vector2D v2 = LastPoint - point;
				double d1 = v1 * v2;
				double d2 = Vector2D.Determinant(v1, v2);

				if ((d1 >= 0) && Math.Abs(d2) < ZeroTolerance)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Whether the line segment contains the point and the point is not on the terminal.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if truly contains.</returns>
		public bool TrulyContains(Point2D point)
		{
			Vector2D v1 = point - FirstPoint;
			Vector2D v2 = LastPoint - point;
			double d1 = v1 * v2;
			double d2 = Vector2D.Determinant(v1, v2);

			if ((d1 > ZeroTolerance) && Math.Abs(d2) < ZeroTolerance)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Whether the two line segments inflect each other.
		/// </summary>
		/// <param name="lineSegment">the target linesegment</param>
		/// <returns>true if inflect</returns>
		public bool Inflects(LineSegment2D lineSegment)
		{
			if (this.TrulyContains(lineSegment.FirstPoint) || this.TrulyContains(lineSegment.LastPoint))
			{
				if (this.ToVector() * lineSegment.ToVector() < ZeroTolerance)
				{
					return true;
				}
			}

			if (this.Contains(lineSegment) || lineSegment.Contains(this))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Whether the two line segment truly intersect each other.
		/// </summary>
		/// <param name="lineSegment">the other line segment.</param>
		/// <returns>true if truly intersect.</returns>
		public bool TrulyIntersects(LineSegment2D lineSegment)
		{
			if (!this.ExtentIntersects(lineSegment))
			{
				return false;
			}
			return lineSegment.TrulyLineIntersects(this) && this.TrulyLineIntersects(lineSegment);
		}

		/// <summary>
		/// Whether the two line segment intersect each other.
		/// </summary>
		/// <param name="lineSegment">the other line segment.</param>
		/// <returns>true if intersect.</returns>
		public bool Intersects(LineSegment2D lineSegment)
		{
			if (!this.ExtentIntersects(lineSegment))
			{
				return false;
			}
			return lineSegment.LineIntersects(this) && this.LineIntersects(lineSegment);
		}

		/// <summary>
		/// Whether the two line segment share terminal point with each other.
		/// </summary>
		/// <param name="lineSegment"></param>
		/// <returns></returns>
		public bool SharePointWith(LineSegment2D lineSegment)
		{
			if (lineSegment == this)
			{
				return false;
			}
			return lineSegment.FirstPoint == this.FirstPoint || lineSegment.FirstPoint == this.LastPoint || lineSegment.LastPoint == this.FirstPoint || lineSegment.LastPoint == this.LastPoint;
		}

		/// <summary>
		/// Whether the point is on the line.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if is on the line.</returns>
		public bool isOnLine(Point2D point)
		{
			if (point == FirstPoint || point == LastPoint)
			{
				return true;
			}
			else
			{
				Vector2D v1 = point - FirstPoint;
				Vector2D v2 = LastPoint - point;

				return Vector2D.Determinant(v1, v2) < ZeroTolerance;
			}
		}

		/// <summary>
		/// Get the parametres of the points on line segment. (between 0 t0 1)
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the parametre of the point on line segment.</returns>
		public double GetPosition(Point2D point)
		{
			if (point == FirstPoint)
			{
				return 0;
			}
			else if (point == LastPoint)
			{
				return 1;
			}
			else
			{
				Vector2D v1 = point - FirstPoint;
				Vector2D v2 = this.ToVector();
				double d = Vector2D.Determinant(v1, v2);

				if (d < ZeroTolerance)
				{
					return v1.Length / v2.Length;
				}
				else if (d >= ZeroTolerance)
				{
					return double.PositiveInfinity;
				}
				else
				{
					return double.NegativeInfinity;
				}
			}
		}

		/// <summary>
		/// Get a new point according to a parametre.
		/// </summary>
		/// <param name="t">the parametre(between 0 to 1)</param>
		/// <returns>the point.</returns>
		public Point2D GetPoint(double t)
		{
			return Point2D.WeightedAverage(FirstPoint, LastPoint, t);
		}

		/// <summary>
		/// Convert line segment to a line.
		/// </summary>
		/// <returns>the line. (Ax + By + C = 0)</returns>
		public Line2D ToLine()
		{
			if (!this.isRegular)
			{
				return null; //TODO: Need Trace;
			}
			else
			{
				if (this.FirstPoint.X == this.LastPoint.X)
				{
					return new Line2D(1, 0, -this.FirstPoint.X);
				}
				else if (this.FirstPoint.Y == this.LastPoint.Y)
				{
					return new Line2D(0, 1, -this.FirstPoint.Y);
				}
				else
				{
					return new Line2D(FirstPoint.Y - LastPoint.Y, LastPoint.X - FirstPoint.X, FirstPoint.X * LastPoint.Y - LastPoint.X * FirstPoint.Y);
				}
			}
		}

		/// <summary>
		/// Get the intersect point of the two line segment.
		/// </summary>
		/// <param name="lineSegment">the ohter line segment.</param>
		/// <returns>the intersect point.</returns>
		public Point2D GetIntersectPoint(LineSegment2D lineSegment)
		{
			if (this.Intersects(lineSegment))
			{
				try
				{
					Point2D point = this.ToLine().Intersects(lineSegment.ToLine());

					return point;
				}
				catch (ArgumentException e)
				{
					string emsg = e.Message;

					return new Point2D(double.NaN, double.NaN);
				}
			}
			else
			{
				return new Point2D(double.NaN, double.NaN);
			}
		}

		/// <summary>
		/// Whether ths line segment intersects a polygon.
		/// </summary>
		/// <param name="polygon">the polygon.</param>
		/// <returns>true if intersects.</returns>
		public bool Intersects(Polygon2D polygon)
		{
			bool bResult = false;

			for (int i = 0; i < polygon.VertexCount; i++)
			{
				LineSegment2D lineSegment = polygon.GetEdge(i);

				//bResult |= this.TrulyIntersects(polygon.GetEdge(i));
				bResult |= (this.TrulyLineIntersects(lineSegment) && lineSegment.Intersects(this));
				if (bResult)
				{
					return true;
				}
			}

			return bResult;
		}

		/// <summary>
		/// Whether this line segment contains the other.
		/// </summary>
		/// <param name="lineSegment">the other line segment.</param>
		/// <returns>true if contains.</returns>
		public bool Contains(LineSegment2D lineSegment)
		{
			if (this.Contains(lineSegment.FirstPoint) && this.Contains(lineSegment.LastPoint))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Swap the two terminal.
		/// </summary>
		public void SwapPoints()
		{
			Point2D point = this.PointPair[0];

			this.PointPair[0] = this.PointPair[1];
			this.PointPair[1] = point;
		}

		/// <summary>
		/// Swap the two terminal.
		/// </summary>
		public void Inverse()
		{
			Point2D point = this.PointPair[0];

			this.PointPair[0] = this.PointPair[1];
			this.PointPair[1] = point;
		}

		/// <summary>
		/// Extend this line segment to get a point.
		/// </summary>
		/// <param name="length">the length to extend.</param>
		/// <returns>the new Point</returns>
		public Point2D Extend(double length)
		{
			return this.LastPoint + this.ToVector().Normalize() * length;
		}

		/// <summary>
		/// Convert this line segment to a path. Instert some new points.
		/// </summary>
		/// <param name="ExtraPoints">the number of extra points to insert.</param>
		/// <returns>the path.</returns>
		public Point2DCollection ToPath(int ExtraPoints)
		{
			if (ExtraPoints < 0)
			{
				throw (new ArgumentException());
			}

			Point2DCollection points = new Point2DCollection();

			points.Add(this.FirstPoint);
			for (int i = 1; i <= ExtraPoints; i++)
			{
				double d = (double) i / (double) (ExtraPoints + 1);

				points.Add(this.GetPoint(d));
			}

			points.Add(this.LastPoint);
			return points;
		}
		#endregion

		#region Static methods
		/// <summary>
		/// Whether the three points is on a line.
		/// </summary>
		/// <param name="a">the first point.</param>
		/// <param name="b">the second point.</param>
		/// <param name="c">the third point.</param>
		/// <returns>true if is on a line.</returns>
		static public bool isOnLine(Point2D a, Point2D b, Point2D c)
		{
			LineSegment2D line = new LineSegment2D(a, b);
                   
			return line.isOnLine(c);
		}
		#endregion

		#region Override methods
		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.FirstPoint.GetHashCode() ^ this.LastPoint.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified object is equal to this line segment.
		/// </summary>
		/// <param name="obj">The object to compare with this line segment.</param>
		/// <returns><c>true</c> if the specified object is equal to this linesegment; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			
			LineSegment2D lineSegment = (LineSegment2D)obj;

			if(lineSegment.FirstPoint == this.FirstPoint && lineSegment.LastPoint == this.LastPoint)
			{
				return true;
			}
			else if (lineSegment.FirstPoint == this.LastPoint && lineSegment.LastPoint == this.FirstPoint)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Operator

		public static bool operator ==(LineSegment2D a, LineSegment2D b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}

			if ((object)a == null && (object)b != null)
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(LineSegment2D a, LineSegment2D b)
		{
			return !(a==b);
		}
		#endregion
	}
}
