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
	/// Summary description for Class1.
	/// </summary>
	public class Polygon2D
	{
		#region Static fields
		public static readonly double ZeroTolerance = 0.00000001;
		#endregion

		#region Member fields
		public const int NoSuchPoint = -1;
		public enum Direction{Clockwise, CounterClockwise, Unknown};
		protected Point2DCollection Vertices = null;
		protected Point2DCollection InnerPoints = new Point2DCollection();
		protected bool bClosed = true;
		protected Direction eDirection = Direction.Unknown;
		protected bool bSimple = false;
		protected bool bConvex = false;
		#endregion

		#region Constructors
		public Polygon2D()
		{
			Vertices = new Point2DCollection();
		}

		public Polygon2D(Point2DCollection points)
		{
			Vertices = points;

			SetStatus();
		}

		public Polygon2D(Polygon2D polygon)
		{
			Vertices = new Point2DCollection();

			Vertices.Clone(polygon.Vertices);

			SetStatus();
		}
		#endregion

		#region Properties
		/// <summary>
		/// The direction of the simple polygon points.
		/// </summary>
		/// <value>the direction.</value>
 		public Direction PointDirection
		{
			get
			{
				return eDirection;
			}
		}

		/// <summary>
		/// The number of Sides of this polygon.
		/// </summary>
		/// <value>the number of sides.</value>
		public int Sides
		{
			get
			{
				return Vertices.Count;
			}
		}

		/// <summary>
		/// The number of Vertices of this polygon.
		/// </summary>
		/// <value>the number of vertices.</value>
		public int VertexCount
		{
			get
			{
				return Vertices.Count;
			}
		}

		/// <summary>
		/// The total number of points of this polygon. Inculding inner points.
		/// </summary>
		/// <value>the number of points.</value>
		public int PointCount
		{
			get
			{
				return Vertices.Count + InnerPoints.Count;
			}
		}

		/// <summary>
		/// The number of inner points of this polygon.
		/// </summary>
		/// <value>the number of inner points.</value>
		public int InnerPointCount
		{
			get
			{
				return InnerPoints.Count;
			}
		}

		/// <summary>
		/// Whether the polygon is regular.
		/// </summary>
		/// <value>true if regular.</value>
		public bool isRegular
		{
			get
			{
				return (Sides > 2) && bClosed;
			}
		}
		/// <summary>
		/// Whether the polygon is closed.
		/// </summary>
		/// <value>true if closed</value>
		public bool isClosed
		{
			get
			{
				return bClosed;
			}
		}
		/// <summary>
		/// Whether the polygon is simple.
		/// </summary>
		/// <value>true if simple.</value>
		public bool isSimple
		{
			get
			{
				return bSimple;
			}
		}

		/// <summary>
		/// Whether the polygon is convex.
		/// </summary>
		/// <value>true if convex.</value>
		public bool isConvex
		{
			get
			{
				return bConvex;
			}
		}

		/// <summary>
		/// The first point of the polygon.
		/// </summary>
		/// <value>the point.</value>
		public Point2D FirstPoint
		{
			get
			{
				return this.Vertices[0];
			}
		}

		/// <summary>
		/// The last point of the polygon.
		/// </summary>
		/// <value>the point.</value>
		public Point2D LastPoint
		{
			get
			{
				return this.Vertices[this.VertexCount - 1];
			}
		}

		public bool isConcave
		{
			get
			{
				if (!this.isRegular)
				{
					return false;
				}

				for (int i = 0; i < this.VertexCount; i++)
				{
					double angle = Angle(i);

					if (this.PointDirection == Direction.Clockwise && angle >= 0)
					{
						continue;
					}
					else if (this.PointDirection == Direction.CounterClockwise && angle <= 0)
					{
						continue;
					}
					else
					{
						return true;
					}
				}
				return false;
			}
		}
		#endregion

		#region Private and Family methods

		protected double GetAngle(Point2D point)
		{
			double angle = 0.0;

			for (int i = 0; i < this.VertexCount; i++)
			{
				Vector2D v1 = GetPoint(i) - point;
				Vector2D v2 = GetPoint((i + 1) % this.VertexCount) - point;
				double a = Vector2D.Angle(v1, v2);

				if (a > Math.PI)
				{
					a -= 2 * Math.PI;
				}
				else if (a < -Math.PI)
				{
					a += 2 * Math.PI;
				}

				angle += a;
			}

			return angle;
		}

		protected void GetDirection()
		{
			Vector2D v = GetPoint(1) - GetPoint(0);
			Point p = Point2D.MidPoint(GetPoint(1), GetPoint(0)) + v.UnitNormal();

			//GetDirection(p);
			//if(this.eDirection == Direction.Unknown)
			if (!this.Contains(p))
			{
				p = Point2D.MidPoint(GetPoint(1), GetPoint(0)) - v.UnitNormal();
				//GetDirection(p);
			}

			GetDirection(p);
		}

		protected void GetConvex()
		{
			if (!this.isRegular)
			{
				return;
			}

			for (int i = 0; i < this.VertexCount; i++)
			{
				double angle = Angle(i);

				if (this.PointDirection == Direction.Clockwise && angle > 0)
				{
					continue;
				}
				else if (this.PointDirection == Direction.CounterClockwise && angle < 0)
				{
					continue;
				}
				else
				{
					bConvex = false;
					return;
				}
			}

			bConvex = true;
		}

		protected double Angle(int i)
		{
			if (i < 0 || i >= this.VertexCount)
			{
				throw (new ArgumentException());
			}

			int a = (i - 1) % this.VertexCount;
			int b = i % this.VertexCount;
			int c = (i + 1) % this.VertexCount;

			if (a < 0)
			{
				a = this.VertexCount - 1;
			}

			Vector2D v1 = this.GetPoint(b) - this.GetPoint(a);
			Vector2D v2 = this.GetPoint(c) - this.GetPoint(b);
			double angle = Vector2D.Angle(v1, v2);

			return angle;
		}

		protected void GetShape()
		{
			if (!this.isRegular)
			{
				this.bSimple = false;
				return;
			}

			for (int i = 0; i < this.VertexCount; i++)
			{
				for (int j = i + 1; j < this.VertexCount; j++)
				{
					if (!this.GetEdge(i).SharePointWith(this.GetEdge(j)))
					{
						if (this.GetEdge(i).TrulyIntersects(this.GetEdge(j)))
						{
							bSimple = false;
							return;
						}
					}
				}
			}

			bSimple = true;
		}

		protected void GetDirection(Point2D point)
		{
			if (!this.isRegular)
			{
				return;
			}

			double angle = GetAngle(point);

			if (Math.Abs(angle - 2 * Math.PI) < ZeroTolerance)
			{
				this.eDirection = Direction.Clockwise;
			}
			else if (Math.Abs(angle + 2 * Math.PI) < ZeroTolerance)
			{
				this.eDirection = Direction.CounterClockwise;
			}
			else
			{
				this.eDirection = Direction.Unknown;
			}
		}

		private void SetStatus()
		{
			GetShape();
			if (bSimple)
			{
				GetDirection();
				GetConvex();
			}
			else
			{
				this.eDirection = Direction.Unknown;
				this.bConvex = false;
			}
		}
		#endregion

		#region Internal Editing methods

		/// <summary>
		/// Add a new point into the polygon.
		/// </summary>
		/// <param name="point"></param>
		internal void Add(Point2D point)
		{
			this.Vertices.Add(point);
			SetStatus();
		}
		/// <summary>
		/// Add a new inner point into the polygon.
		/// </summary>
		/// <param name="point"></param>
		internal void AddInner(Point2D point)
		{
			this.InnerPoints.DistinctAdd(point);
		}
		/// <summary>
		/// Add a new point according to the index.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <param name="index">the index.</param>
		internal void Add(Point2D point, int index)
		{
			this.Vertices.Add(point, index);
			SetStatus();
		}
		/// <summary>
		/// Remove all inner points from the polygon.
		/// </summary>
		internal void Clear()
		{
			this.InnerPoints.Clear();
		}
		/// <summary>
		/// Remove the point according to the index.
		/// </summary>
		/// <param name="index">the index.</param>
		internal void RemovePoint(int index)
		{
			if (index < this.VertexCount)
			{
				this.Vertices.Remove(index);
			}
			else
			{
				this.InnerPoints.Remove(index - this.VertexCount);
			}

			SetStatus();
		}
		/// <summary>
		/// Reset a current point according to the index.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <param name="index">the index.</param>
		internal void SetPoint(Point2D point, int index)
		{
			SetPoint(point.X, point.Y, index);
		}
		/// <summary>
		/// Reset a current point according to the index.
		/// </summary>
		/// <param name="x">the first coordinate.</param>
		/// <param name="y">the second coordinate.</param>
		/// <param name="index">the index.</param>
		internal void SetPoint(double x, double y, int index)
		{
			this.GetPoint(index).X = x;
			this.GetPoint(index).Y = y;
			SetStatus();
		}
		/// <summary>
		/// Scroll the polygon.(Sweep the indices of the vertices.)
		/// </summary>
		internal void Scroll()
		{
			int count = Vertices.Count - 1;
			Point2D point = Vertices[count];

			for (int i = count; i > 0; i--)
			{
				Vertices[i] = Vertices[i - 1];
			}

			Vertices[0] = point;
		}
		/// <summary>
		/// Invert the direction of this polygon.
		/// </summary>
		internal void Inverse()
		{
			Point2D point = null;
			int count = Vertices.Count;

			for (int i = 0; i < count / 2; i++)
			{
				point = Vertices[i];
				Vertices[i] = Vertices[count - 1 - i];
				Vertices[count - 1 - i] = point;
			}

			if (eDirection == Direction.Clockwise)
			{
				eDirection = Direction.CounterClockwise;
			}
			else if (eDirection == Direction.CounterClockwise)
			{
				eDirection = Direction.Clockwise;
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Get a point from a polygon.
		/// </summary>
		/// <param name="i">the index of this point.</param>
		/// <returns>a point.</returns>
		public Point2D GetPoint(int i)
		{
			if (i < this.VertexCount)
			{
				return this.Vertices[i];
			}
			else
			{
				return this.InnerPoints[i - this.VertexCount];
			}
		}

		/// <summary>
		/// Get an inner point form a polygon.
		/// </summary>
		/// <param name="i">the index of the inner point.</param>
		/// <returns>an inner point.</returns>
		public Point2D GetInnerPoint(int i)
		{
			return this.InnerPoints[i];
		}

		/// <summary>
		/// Whether the point is inside the polygon.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if contains.</returns>
		public bool Contains(Point2D point)
		{
			double angle = GetAngle(point);

			if (Math.Abs(Math.Abs(angle) - 2 * Math.PI) < ZeroTolerance)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Whether the line segment is inside the polygon.
		/// </summary>
		/// <param name="lineSegment">the line segment.</param>
		/// <returns>true if contains.</returns>
		public bool Contains(LineSegment2D lineSegment)
		{
			/*for (int i = 0; i < this.VertexCount; i++)
			{
				if (lineSegment.Contains(this.GetEdge(i)))
				{
					return false;
				}
			}*/
			if (this.InflectsEdge(lineSegment))
			{
				return false;
			}
			return !lineSegment.Intersects(this) && this.Contains(lineSegment.MidPoint) && !this.isOnEdge(lineSegment.MidPoint);
		}

		/// <summary>
		/// Whether the point is on the edge of the polygon.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if on the edge.</returns>
		public bool isOnEdge(Point2D point)
		{
			return this.OnEdge(point) != Polygon2D.NoSuchPoint;
		}

		/// <summary>
		/// Get the index of this point.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the index.</returns>
		public int GetPointIndex(Point2D point)
		{
			for (int i = 0; i < this.VertexCount; i++)
			{
				if (this.Vertices[i] == point)
				{
					return i;
				}
			}

			for (int i = 0; i < this.InnerPoints.Count; i++)
			{
				if (this.InnerPoints[i] == point)
				{
					return i + this.VertexCount;
				}
			}

			return NoSuchPoint;
		}

		/// <summary>
		/// Get the edge (i-1,i) of this polygon according to the index.
		/// </summary>
		/// <param name="i">the index.</param>
		/// <returns>the edge.</returns>
		public LineSegment2D GetEdge(int i)
		{
			if (i < 0 || i >= this.VertexCount)
			{
				throw (new ArgumentException());
			}

			int a = i - 1;

			if (a < 0)
			{
				a = this.VertexCount - 1;
			}

			int b = i;

			return new LineSegment2D(this.GetPoint(a), this.GetPoint(b));
		}

		/// <summary>
		/// Whether the line segment is a edge of this polygon.
		/// </summary>
		/// <param name="lineSegment">the line segment.</param>
		/// <returns>true if is edge.</returns>
		public bool isEdge(LineSegment2D lineSegment)
		{
			for (int i = 0; i < this.VertexCount; i++)
			{
				if (lineSegment == this.GetEdge(i))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Whether the line segment is the diagonal of this polygon.
		/// </summary>
		/// <param name="lineSegment">the line segment.</param>
		/// <returns>true if is diagonal.</returns>
		public bool isDiagonal(LineSegment2D lineSegment)
		{
			if (!lineSegment.isRegular)
			{
				return false;
			}
			if (this.isEdge(lineSegment))
			{
				return false;
			}
			if (!this.Contains(lineSegment))
			{
				return false;
			}

			for (int i = 0; i < this.VertexCount; i++)
			{
				if (lineSegment.TrulyContains(this.Vertices[i]))
				{
					return false;
				}
			}
			for (int i = 0; i < this.VertexCount; i++)
			{
				if (lineSegment.FirstPoint == this.Vertices[i])
				{
					for (int j = 0; j < this.VertexCount; j++)
					{
						if (lineSegment.LastPoint == this.Vertices[j])
						{
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// If the point is one of the vertices of this polygon, it returns the index. Otherwise it returns NoSuchPoint.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the index.</returns>
		public int HasVertex(Point2D point)
		{
			for (int i = 0; i < this.VertexCount; i++)
			{
				if (this.Vertices[i] == point)
				{
					return i;
				}
			}
			return NoSuchPoint;
		}

		/// <summary>
		/// If the point is on one of the edges of this polygon, it returns the index, otherwise it returns NoSuchPoint.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the index</returns>
		public int OnEdge(Point2D point)
		{
			for (int i = 0; i < this.VertexCount; i++)
			{
				LineSegment2D line = this.GetEdge(i);

				if (line.Contains(point))
				{
					return i;
				}
			}

			return NoSuchPoint;
		}

		/// <summary>
		/// Whether the angle of this polygon is convex.
		/// </summary>
		/// <param name="i">the index of this angle.</param>
		/// <returns>true if convex.</returns>
		public bool isConvexAngle(int i)
		{
			double angle = Angle(i);
			//ÅÐ¶Ï¶¥µãµÄ°¼Í¹
			//Ë³Ê±Õë¡¡¡ª¡ª¡¡angle > 0 Í¹ £¨×óÊÖ×ø±êÏµ£©
			//ÄæÊ±Õë  ¡ª¡ª  angle < 0 Í¹

			if ((this.PointDirection == Direction.Clockwise && angle > 0) || (this.PointDirection == Direction.CounterClockwise && angle < 0))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool isConcaveAngle(int i)
		{
			double angle = Angle(i);
			//ÅÐ¶Ï¶¥µãµÄ°¼Í¹
			//Ë³Ê±Õë¡¡¡ª¡ª¡¡angle > 0 Í¹ £¨×óÊÖ×ø±êÏµ£©
			//ÄæÊ±Õë  ¡ª¡ª  angle < 0 Í¹

			if (angle == 0)
			{
				return false;
			}
			else if ((this.PointDirection == Direction.Clockwise && angle > 0) || (this.PointDirection == Direction.CounterClockwise && angle < 0))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Whether the point is one of the vertices of the polygon.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>true if is a vertex.</returns>
		public bool isVertex(Point2D point)
		{
			return this.HasVertex(point) != NoSuchPoint;
		}

		/// <summary>
		/// Whether the line segment inflects the edges of the polygon.
		/// </summary>
		/// <param name="lineSegment">the line segment.</param>
		/// <returns>true if inflects.</returns>
		public bool InflectsEdge(LineSegment2D lineSegment)
		{
			for (int i = 0; i < this.VertexCount; i++)
			{
				if (lineSegment.Inflects(this.GetEdge(i)))
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
