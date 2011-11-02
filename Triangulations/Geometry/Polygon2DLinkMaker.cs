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
	/// Construction of Minimum Link between two vertices.
	/// </summary>
	public class Polygon2DLinkMaker : Polygon2DDiviser
	{
		#region Member fields
		private Point2D FirstPoint = null;
		private Point2D LastPoint = null;
		/// <summary>
		/// The link path in the polygon.
		/// </summary>
		public readonly Point2DCollection LinkDivisers = null;
		#endregion

		#region Properties
		/// <summary>
		/// The link distance between the two vertices.
		/// </summary>
		/// <value></value>
		public int LinkDistance
		{
			get
			{
				if (this.SubDivision.Count > 1)
				{
					return this.SubDivision.Count;
				}
				else
				{
					LineSegment2D CurrentLine = new LineSegment2D(this.FirstPoint, this.LastPoint);

					if (this.Parent.Contains(CurrentLine))
					{
						return 1;
					}
					else
					{
						return 2;
					}
				}
			}
		}

		public int MinimumPart
		{
			get
			{
				int a = this.Parent.GetPointIndex(FirstPoint);
				int b = this.Parent.GetPointIndex(LastPoint);
				int c = Math.Abs(a - b);
				int d = this.Parent.VertexCount - c;

				int r = Math.Min(c, d);

				return r + this.Divisers.Count - 2;
			}
		}

		/// <summary>
		/// The index of the first vertex.
		/// </summary>
		/// <value>the index.</value>
		public int FirstPointIndex
		{
			get
			{
				return this.Parent.GetPointIndex(FirstPoint);
			}
		}

		/// <summary>
		/// The index of the last vertex.
		/// </summary>
		/// <value>the index.</value>
		public int LastPointIndex
		{
			get
			{
				return CurrentPolygon.GetPointIndex(LastPoint);
			}
		}

		/// <summary>
		/// The index of the current vertex.
		/// </summary>
		/// <value>the index.</value>
		public int CurrentPointIndex
		{
			get
			{
				return CurrentPolygon.GetPointIndex(this.LastDiviser.FirstPoint);
			}
		}

		/// <summary>
		/// The line segment of the patitions share.
		/// </summary>
		/// <value>the line segment.</value>
		public LineSegment2D LastDiviser
		{
			get
			{
				if (this.Divisers.Count > 0)
				{
					return this.Divisers[this.Divisers.Count - 1];
				}
				else
				{
					return new LineSegment2D(this.FirstPoint, this.FirstPoint);
				}
			}
		}
		#endregion

		#region Constructors
		public Polygon2DLinkMaker(Polygon2D polygon, int a, int b): base(polygon)
		{
			if (!polygon.isRegular)
			{
				throw (new ArgumentException());
			}

			FirstPoint = polygon.GetPoint(a);
			LastPoint = polygon.GetPoint(b);
			LinkDivisers = new Point2DCollection();
			//Partitions = new Polygon2DCollection();
			//Partitions.Add(polygon);
		}
		#endregion

		#region Private methods
		private LineSegment2D GetLineSegment(int a, int b)
		{
			Point2D first =	CurrentPolygon.GetPoint(a);
			Point2D second = CurrentPolygon.GetPoint(b);

			return new LineSegment2D(first, second);
		}

		private bool isApart(LineSegment2D lineSegment, int c, int d)
		{
			double a = CurrentPolygon.OnEdge(lineSegment.FirstPoint) - 0.5;
			double b = CurrentPolygon.OnEdge(lineSegment.LastPoint) - 0.5;

			if (a < 0)
			{
				a += CurrentPolygon.VertexCount;
			}
			if (b < 0)
			{
				b += CurrentPolygon.VertexCount;
			}

			return isApart(a, b, c, d);
		}

		private bool isApart(double a, double b, int c, int d)
		{
			double tmpd;
			int tmp;

			if (a > b)
			{
				tmpd = a;
				a = b;
				b = tmpd;
			}

			if (c > d)
			{
				tmp = c;
				c = d;
				d = tmp;
			}

			if (c > a && b > c && d > b)
			{
				return true;
			}

			if (a > c && d > a && b > d)
			{
				return true;
			}

			return false;
		}

		private LineSegment2D ChooseDiviser(LineSegment2D lineSegment)
		{
			for (int i = 0; i < CurrentPolygon.VertexCount; i++)
			{
				LineSegment2D Edge = CurrentPolygon.GetEdge(i);

				if (Edge == lineSegment || Edge.Intersects(lineSegment) || !Edge.LineIntersects(lineSegment))
				{
					continue;
				}

				Point2D divPoint = lineSegment.ToLine().Intersects(Edge.ToLine());

				if (lineSegment.Contains(divPoint))
				{
					continue;
				}

				LineSegment2D divLine = new LineSegment2D(lineSegment.LastPoint, divPoint);

				if (divLine.Contains(lineSegment.FirstPoint))
				{
					//divLine = new LineSegment2D(lineSegment.FirstPoint, divPoint);
					continue;
				}

				/*if (CurrentPolygon.InflectsEdge(divLine))
				{
					int ssi = 0;
				}*/
				if (!CurrentPolygon.Contains(divLine))
				{
					continue;
				}

				if (divPoint == this.LastPoint)
				{
					///need trace here
					return divLine;
				}

				if (this.isApart(divLine, this.CurrentPointIndex, this.LastPointIndex))
				{
					return divLine;
				}
			}

			return null;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Divide the source polygon to compute link distance.
		/// </summary>
		public void Divide()
		{
			LineSegment2D entrance = this.LastDiviser;
			LineSegment2D path = new LineSegment2D(entrance.FirstPoint, this.LastPoint);

			if (this.CurrentPolygon.Contains(path))
			{
				return;
			}

			path = new LineSegment2D(entrance.LastPoint, this.LastPoint);

			if (this.CurrentPolygon.Contains(path))
			{
				return;
			}

			for (int i = 0; i < this.CurrentPolygon.VertexCount; i++)
			{
				
				if (this.CurrentPolygon.GetPoint(i) == entrance.FirstPoint)
				{
					continue;
				}

				LineSegment2D lineSegment = new LineSegment2D(entrance.FirstPoint, this.CurrentPolygon.GetPoint(i));

				if(!this.CurrentPolygon.Contains(lineSegment) && !this.CurrentPolygon.isEdge(lineSegment))
				{
					continue;
				}

				LineSegment2D lineDiviser = this.ChooseDiviser(lineSegment);

				if (lineDiviser == null)
				{
					continue;
				}

				Polygon2D testPoly = new Polygon2D(this.CurrentPolygon);
				if (!testPoly.isVertex(lineDiviser.LastPoint))
				{
					testPoly.Add(lineDiviser.LastPoint, testPoly.OnEdge(lineDiviser.LastPoint));
				}

				Polygon2D polygon = this.DividedBy(lineDiviser, testPoly);

				if (polygon.isVertex(this.LastPoint))
				{
					testPoly = this.CurrentPolygon;
					this.SubDivision.Remove(this.SubDivision.Count - 1);
				}
				else
				{
					testPoly = polygon;
					polygon = this.CurrentPolygon;
					this.SubDivision.Remove(this.SubDivision.Count - 1);
				}

				if (entrance.FirstPoint == entrance.LastPoint)
				{
					this.SubDivision.Remove(this.SubDivision.Count - 1);
					this.SubDivision.Add(testPoly);
					this.SubDivision.Add(polygon);
				}
				else
				{
					LineSegment2D testLine = new LineSegment2D(entrance.LastPoint, lineDiviser.FirstPoint);

					if (!testLine.isRegular)
					{
						entrance = new LineSegment2D(entrance.LastPoint, entrance.LastPoint);
						continue;
					}

					if (this.CurrentPolygon.Contains(testLine))
					{
						testLine = this.ChooseDiviser(testLine);
						if (testLine != null && !testPoly.Contains(testLine))
						{
							lineDiviser = testLine;
							testPoly = new Polygon2D(this.CurrentPolygon);
							if (!testPoly.isVertex(lineDiviser.LastPoint))
							{
								testPoly.Add(lineDiviser.LastPoint, testPoly.OnEdge(lineDiviser.LastPoint));
							}

							polygon = this.DividedBy(lineDiviser, testPoly);
							testPoly = this.CurrentPolygon;
							this.SubDivision.Remove(this.SubDivision.Count - 1);
						}
						else if (testLine == null)
						{
							continue;
						}
					}

					if (!polygon.isVertex(this.LastPoint))
					{
						this.SubDivision.Remove(this.SubDivision.Count - 1);
						this.SubDivision.Add(polygon);
						this.SubDivision.Add(testPoly);
					}
					else
					{
						this.SubDivision.Remove(this.SubDivision.Count - 1);
						this.SubDivision.Add(testPoly);
						this.SubDivision.Add(polygon);
					}
				}

				this.Divisers.Add(lineDiviser);
				
				this.Divide();

				return;
			}
		}

		/// <summary>
		/// Construct a minimum link path of a polygon.
		/// </summary>
		public void BuildPath()
		{
			this.LinkDivisers.Clear();

			Point2D CurrentPoint = this.LastPoint;
			Point2D IntersectPoint = null;
			LineSegment2D CurrentLine = null;
			LineSegment2D IntersectLine = null;

			for (int i = this.Divisers.Count - 1; i >= 0; i--)
			{
				double position = -1.0;
				Polygon2D currentPolygon = this.SubDivision[i + 1];

				CurrentLine = this.Divisers[i];
				if (CurrentLine.Contains(CurrentPoint))
				{
					this.LinkDivisers.Add(CurrentPoint);

					Vector2D vector = CurrentLine.ToVector().Normal().Normalize();
					IntersectPoint = CurrentLine.MidPoint + vector;

					if (!this.SubDivision[i].Contains(IntersectPoint))
					{
						vector = -vector;
					}

					double length = CurrentLine.Length;
					
					LineSegment2D testLine = null;
					
					do
					{
						IntersectPoint = CurrentLine.MidPoint + vector * length;
						length /= 2;
						testLine = new LineSegment2D(CurrentPoint, IntersectPoint);
					} while (!this.SubDivision[i].Contains(testLine));

					continue;
				}

				for (int j = 0; j < currentPolygon.VertexCount; j++)
				{
					Point2D point = currentPolygon.GetPoint(j);

					if (CurrentPoint == point)
					{
						continue;
					}

					LineSegment2D lineSegment = new LineSegment2D(CurrentPoint, point);
	
					if (currentPolygon.Contains(lineSegment) || currentPolygon.isEdge(lineSegment))
					{
		
						if (CurrentLine.LineIntersects(lineSegment))
						{
							IntersectPoint = CurrentLine.ToLine().Intersects(lineSegment.ToLine());
							if (position < 0)
							{
								position = CurrentLine.GetPosition(IntersectPoint);
							}
							else
							{
								position += CurrentLine.GetPosition(IntersectPoint);
								position /= 2;
							}
						}
					}
				}

				if (position < -0.5)
				{
					CurrentPoint = IntersectPoint + IntersectLine.ToVector().Normalize();
					i++;
					continue;
				}

				this.LinkDivisers.Add(CurrentPoint);
				IntersectPoint = CurrentLine.GetPoint(position);
				IntersectLine = new LineSegment2D(CurrentPoint, IntersectPoint);
				double extend = IntersectLine.Length;

				do
				{
					extend /= 2;
					CurrentPoint = IntersectLine.Extend(extend);
				} while (!this.SubDivision[i].Contains(CurrentPoint));
				
				//CurrentPoint = CurrentLine.GetPoint(position);
			}
			
			CurrentLine = new LineSegment2D(CurrentPoint, this.FirstPoint);

			if (this.Parent.Contains(CurrentLine))
			{
				this.LinkDivisers.Add(CurrentPoint);
				this.LinkDivisers.Add(this.FirstPoint);
			}
			else if (IntersectLine != null)
			{
				CurrentPoint = IntersectPoint + IntersectLine.ToVector().Normalize();
				this.LinkDivisers.Add(CurrentPoint);
				this.LinkDivisers.Add(this.FirstPoint);
			}
			else
			{
				Vector2D vector = CurrentLine.ToVector().Normal().Normalize();

				IntersectPoint = CurrentLine.MidPoint + vector;

				if (!this.Parent.Contains(IntersectPoint))
				{
					vector = -vector;
				}

				IntersectPoint = CurrentLine.MidPoint;

				double length = CurrentLine.Length;

				do
				{
					length /= 2;
					CurrentPoint = IntersectPoint + vector * length;
				}
				while (!this.Parent.Contains(CurrentPoint));
                
				this.LinkDivisers.Add(this.LastPoint);
				this.LinkDivisers.Add(CurrentPoint);
				this.LinkDivisers.Add(this.FirstPoint);
			}
		}
		#endregion
	}
}
