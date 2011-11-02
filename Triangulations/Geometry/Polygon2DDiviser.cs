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
	using Akira.Collections;
	using System.Diagnostics;
	/// <summary>
	/// Summary description for PolygonDivision.
	/// </summary>
	public class Polygon2DDiviser
	{
		#region Member fields
		/// <summary>
		/// The source polygon to divide.
		/// </summary>
		public readonly Polygon2D Parent = null;
		/// <summary>
		/// The subDivisions of this polygon.
		/// </summary>
		public readonly Polygon2DCollection SubDivision = null;
		/// <summary>
		/// The line segment divisers of the division.
		/// </summary>
		public readonly LineSegment2DCollection Divisers = null;
		/// <summary>
		/// The inner points insert to the source polygon during the dividing actions.
		/// </summary>
		public readonly Point2DCollection InnerPoints = null;
		#endregion

		#region Constructors
		public Polygon2DDiviser(int n): this(new ClassicalPolygon2D(n))
		{
	
		}

		public Polygon2DDiviser(Polygon2D parent)
		{
			Parent = parent;
			SubDivision = new Polygon2DCollection();
			Divisers = new LineSegment2DCollection();
			InnerPoints = new Point2DCollection();
			SubDivision.Add(Parent);
		}
		#endregion

		#region Properties
		/// <summary>
		/// The polygon to divide.
		/// </summary>
		/// <value>the polygon.</value>
		public Polygon2D CurrentPolygon
		{
			get
			{
				return SubDivision[SubDivision.Count - 1];
			}
		}
		#endregion

		#region Private methods
		private bool isEdge(int a, int b,Polygon2D polygon)
		{
			if (Math.Abs(a - b) == 1 || Math.Abs(a - b) == polygon.VertexCount - 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Dividing methods

		/// <summary>
		/// Divide the polygon by the line segment between two vertices.
		/// </summary>
		/// <param name="a">the index of first vertex.</param>
		/// <param name="b">the index of second vertex.</param>
		public void DividedBy(int a, int b)
		{
			if (a < 0 || b < 0 || a > Parent.VertexCount || b > Parent.VertexCount)
			{
				throw (new ArgumentException());
			}

			LineSegment2D lineSegment = new LineSegment2D(Parent.GetPoint(a), Parent.GetPoint(b));

			DividedBy(lineSegment);
		}

		/// <summary>
		/// Divide the polygon by a line segment.
		/// </summary>
		/// <param name="lineSegment">the line segment.</param>
		public void DividedBy(LineSegment2D lineSegment)
		{
			Divisers.Add(lineSegment);

			int Count = SubDivision.Count;

			for (int i = 0; i < Count; i++)
			{
				Polygon2D poly = SubDivision[i];

				SubDivision[i] = DividedBy(lineSegment, poly);
			}			
		}

		/// <summary>
		/// Divide a sub polygon by a line segment.
		/// </summary>
		/// <param name="lineSegment">the line segment.</param>
		/// <param name="poly">the polygon to divide.</param>
		/// <returns>one of the sub polygon divided from the polygon.</returns>
		internal Polygon2D DividedBy(LineSegment2D lineSegment, Polygon2D poly)
		{
			if (poly.isDiagonal(lineSegment))
			{
				int a = poly.HasVertex(lineSegment.FirstPoint);
				int b = poly.HasVertex(lineSegment.LastPoint);

				if (a > b)
				{
					int tmp = a;
					a = b;
					b = tmp;
				}

				Point2DCollection p2 = new Point2DCollection(b - a + 1);
				Point2DCollection p1 = new Point2DCollection(poly.VertexCount - p2.Size + 2);

				for (int i = 0; i < poly.VertexCount; i++)
				{
					if (i <= a || i >= b)
					{
						p1.Add(poly.GetPoint(i));
					}
					if (i >= a && i <= b)
					{
						p2.Add(poly.GetPoint(i));
					}
				}

				if (p1.Count > p2.Count)
				{
					SubDivision.Add(new Polygon2D(p1));
					return new Polygon2D(p2);
				}
				else
				{
					SubDivision.Add(new Polygon2D(p2));
					return new Polygon2D(p1);
				}
			}
			else if (lineSegment.Intersects(poly))
			{
				Point2DCollection Points = new Point2DCollection(2);

				for (int i = 0; i < poly.VertexCount; i++)
				{
					LineSegment2D border = poly.GetEdge(i);

					Point2D p = lineSegment.GetIntersectPoint(border);

					if (p.isRegular)
					{
						Points.DistinctAdd(p);
					}
				}

				Debug.Assert(Points.Count == 2);

				if (poly.HasVertex(Points[0]) == Polygon2D.NoSuchPoint) 
				{
					poly.Add(Points[0], poly.OnEdge(Points[0]));
					Parent.AddInner(Points[0]);
					this.InnerPoints.DistinctAdd(Points[0]);
				}

				if (poly.HasVertex(Points[1]) == Polygon2D.NoSuchPoint)
				{
					poly.Add(Points[1], poly.OnEdge(Points[1]));
					Parent.AddInner(Points[1]);
					this.InnerPoints.DistinctAdd(Points[1]);
				}

				LineSegment2D line = new LineSegment2D(Points[0], Points[1]);

				return DividedBy(line, poly);
			}
			else
			{
				return poly;
			}
		}

		/// <summary>
		/// Divide a polygon by a path.(point2DCollection)
		/// </summary>
		/// <param name="path">the path.</param>
		/// <param name="polygon">the polygon to divide.</param>
		public void DividedBy(Point2DCollection path, Polygon2D polygon)
		{
			int a = polygon.HasVertex(path.FirstPoint);
			int b = polygon.HasVertex(path.LastPoint);

			if (a == Polygon2D.NoSuchPoint || b == Polygon2D.NoSuchPoint)
			{
				throw (new ArgumentException());
			}

			if (a > b)
			{
				int tmp = a;

				a = b;
				b = tmp;
			}

			Point2DCollection p2 = new Point2DCollection();
			Point2DCollection p1 = new Point2DCollection();

			for (int i = 0; i < polygon.VertexCount; i++)
			{
				if (i <= a || i >= b)
				{
					p1.Add(polygon.GetPoint(i));
				}

				if (i >= a && i <= b)
				{
					p2.Add(polygon.GetPoint(i));
				}
			}
            
			for (int i = 1; i < path.Count - 1; i++)
			{
				p1.Add(path[i], a + i);
				p2.Add(path[i], 0);
				Parent.AddInner(path[i]);
			}

			if (p1.Count > p2.Count)
			{
				SubDivision.Add(new Polygon2D(p1));
				SubDivision.Add(new Polygon2D(p2));
			}
			else
			{
				SubDivision.Add(new Polygon2D(p2));
				SubDivision.Add(new Polygon2D(p1));
			}
		}

		#region Triangulations
		/// <summary>
		/// Divide the polygon to triangles.
		/// </summary>
		public void Triangulation()
		{
			for (int i = 0; i < SubDivision.Count; i++)
			{
				Polygon2D poly = SubDivision[i];

				if (poly.PointCount > 3)
				{
					LineSegment2D lineSegment = new LineSegment2D(poly.GetPoint(0), poly.GetPoint(2));

					SubDivision[i] = DividedBy(lineSegment, poly);
				}
			}
		}
		#endregion

		#endregion

		#region Public methods

		public int GetSubIndex(int PointIndex, int SubIndex)
		{
			Polygon2D subPolygon = this.SubDivision[SubIndex];

			return subPolygon.GetPointIndex(this.Parent.GetPoint(PointIndex));
		}
		public int GetParentIndex(int PointIndex, int SubIndex)
		{
			Polygon2D subPolygon = this.SubDivision[SubIndex];
			return this.Parent.GetPointIndex(subPolygon.GetPoint(PointIndex));
		}

		#endregion
	}
}
