//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using Akira.Collections;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Diagnostics;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Summary description for Polygon2DTriangulator.
	/// </summary>
	public class Polygon2DTriangulator
	{
		private Polygon2DDiviser Diviser = null;
		private Polygon2DAdorner FirstPolygon = null;
		private Polygon2DAdorner SecondPolygon = null;
		private GhostPoint2DCollection ghostPoint = null;
		private GhostTriangle2DCollection ghostTriangle = null;

		public Polygon2DTriangulator(Polygon2DAdorner a,Polygon2DAdorner b)
		{
			Polygon2DEditor polygonEditor = null;

			if (!a.polygon.isSimple || !b.polygon.isSimple)
			{
				throw (new ArgumentException());
			}
			if (a.polygon.PointDirection != b.polygon.PointDirection)
			{
				polygonEditor = new Polygon2DEditor(a.polygon);	
				polygonEditor.Invert();
			}

			Debug.Assert(a.polygon.PointDirection == b.polygon.PointDirection);
			FirstPolygon = a;
			SecondPolygon = b;
			Diviser = new Polygon2DDiviser(a.Count);
			ghostPoint = new GhostPoint2DCollection();
			ghostTriangle = new GhostTriangle2DCollection();

			polygonEditor = new Polygon2DEditor(this.FirstPolygon.polygon);
			polygonEditor.Clear();
			polygonEditor = new Polygon2DEditor(this.SecondPolygon.polygon);
			polygonEditor.Clear();
		}

		public void CompatibleTriangulation()
		{
			this.FirstPolygon.Triangulation();
			this.SecondPolygon.Triangulation();
			Divide();
			Triangulation();
			AddGhostPoint();
			AddInnerPoints();
			AddGhostTriangle();
			ApplyGhostTriangle();
		}

		private void Divide()
		{
			int Count = FirstPolygon.internalSegments.Count;
			Vector2DCollection v = FirstPolygon.internalSegments;

			for (int i = 0; i < Count; i++)
			{
				Diviser.DividedBy((int)(v[i].X), (int)(v[i].Y));
			}

			Count = SecondPolygon.internalSegments.Count;
			v = SecondPolygon.internalSegments;

			for (int i = 0; i < Count; i++)
			{
				Diviser.DividedBy((int)(v[i].X), (int)(v[i].Y));
			}
		}

		private void Triangulation()
		{
			Diviser.Triangulation();
		}

		private void AddGhostPoint()
		{
			for (int i = 0; i < this.Diviser.InnerPoints.Count; i++)
			{
				Point2D point = this.Diviser.InnerPoints[i];

				for (int j = 0; j < this.Diviser.Divisers.Count; j++)
				{
					LineSegment2D lineSegment = this.Diviser.Divisers[j];
	
					if (lineSegment.Contains(point))
					{
						int a = this.Diviser.Parent.GetPointIndex(lineSegment.FirstPoint);
						int b = this.Diviser.Parent.GetPointIndex(lineSegment.LastPoint);

						Debug.Assert(a != -1 && b != -1);

						double t = lineSegment.GetPosition(point);

						this.ghostPoint.Add(new GhostPoint2D(a, b, t));
					}
				}
			}
		}

		private void ApplyGhostTriangle()
		{
			this.FirstPolygon.ApplyGhostTriangles(this.ghostTriangle);
			this.SecondPolygon.ApplyGhostTriangles(this.ghostTriangle);
		}

		public void Clear()
		{
			this.ghostTriangle = new GhostTriangle2DCollection();

			Polygon2DEditor polygonEditor = new Polygon2DEditor(this.FirstPolygon.polygon);
			polygonEditor.Clear();
			polygonEditor = new Polygon2DEditor(this.SecondPolygon.polygon);
			polygonEditor.Clear();

			this.FirstPolygon.ApplyGhostTriangles(this.ghostTriangle);
			this.SecondPolygon.ApplyGhostTriangles(this.ghostTriangle);
		}

		private void AddInnerPoints()
		{
			AddInnerPoints(this.FirstPolygon);
			AddInnerPoints(this.SecondPolygon);
		}

		private void AddInnerPoints(Polygon2DAdorner polygon)
		{
			for (int j = 0; j < this.ghostPoint.Count; j++)
			{
				for (int i = 0; i < polygon.internalSegments.Count; i++)
				{
					int a = (int)(polygon.internalSegments[i].X);
					int b = (int)(polygon.internalSegments[i].Y);

					if (a == this.ghostPoint[j].FirstIndex && b == this.ghostPoint[j].LastIndex || a == this.ghostPoint[j].LastIndex && b == this.ghostPoint[j].FirstIndex)
					{
						Point2D point = null;
						LineSegment2D lineSegment = null;

						lineSegment = new LineSegment2D(polygon.polygon.GetPoint(a), polygon.polygon.GetPoint(b));

						point = lineSegment.GetPoint(this.ghostPoint[j].LocalPosition);

						Polygon2DEditor polygonEditor = new Polygon2DEditor(polygon.polygon);

						polygonEditor.AddInnerPoint(point);

						break;
					}
				}
			}
		}

		private void AddGhostTriangle()
		{
			for (int i = 0; i < this.Diviser.SubDivision.Count; i++)
			{
				Polygon2D polygon = this.Diviser.SubDivision[i];

				Point2D point = polygon.GetPoint(0);

				int a = Diviser.Parent.GetPointIndex(point);
				int b = Diviser.Parent.GetPointIndex(polygon.GetPoint(1));
				int c = Diviser.Parent.GetPointIndex(polygon.GetPoint(2));

				this.ghostTriangle.Add(new GhostTriangle2D(a, b, c));
			}
		}
	}
}
