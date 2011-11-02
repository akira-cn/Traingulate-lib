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
	/// Summary description for TriangleDiviser.
	/// </summary>
	public class TriangleDiviser
	{
		private const double AreaTolerance = 0.002;
		private const double AngleTolerance = Math.PI/6;
		private const double ShapeTolerance = 0.001;
		private Polygon2D firstPolygon = null;
		private Polygon2D secondPolygon = null;

		public TriangleDiviser(Polygon2D a, Polygon2D b)
		{
			firstPolygon = a;
			secondPolygon = b;
		}

		private void Divide(GhostTriangle2D ghostTriangle)
		{
			Triangle2D triangle = ghostTriangle.ToTriangle(firstPolygon);

			Point2D point = (triangle.A + triangle.B.ToVector() + triangle.C.ToVector()) / 3;

			Polygon2DEditor polygonEditor = new Polygon2DEditor(firstPolygon);

			polygonEditor.AddInnerPoint(point);

			triangle = ghostTriangle.ToTriangle(secondPolygon);

			point = (triangle.A + triangle.B.ToVector() + triangle.C.ToVector()) / 3;

			polygonEditor = new Polygon2DEditor(secondPolygon);

			polygonEditor.AddInnerPoint(point);
		}

		internal void Divide(GhostTriangle2DCollection ghostTriangles)
		{
			int Count = ghostTriangles.Count;

			for (int i = 0; i < Count; i++)
			{
				if(QualityDeterminer.AreaQuality(ghostTriangles[i], firstPolygon) < AreaTolerance|| QualityDeterminer.AreaQuality(ghostTriangles[i], secondPolygon) < AreaTolerance)
				{
					Divide(ghostTriangles[i]);

					GhostTriangle2D ghostTriangle = new GhostTriangle2D(ghostTriangles[i].A, ghostTriangles[i].B, this.firstPolygon.PointCount - 1);
					
					ghostTriangles.Add(ghostTriangle);

					ghostTriangle = new GhostTriangle2D(ghostTriangles[i].A, this.firstPolygon.PointCount - 1, ghostTriangles[i].C);

					ghostTriangles.Add(ghostTriangle);

					ghostTriangles[i].A = this.firstPolygon.PointCount - 1;
				}
			}
		}

		private bool isNeighbor(GhostTriangle2D a, GhostTriangle2D b)
		{
			int i = 0;

			if (a.A == b.A || a.A == b.B || a.A == b.C)
			{
				i++;
			}

			if (a.B == b.A || a.B == b.B || a.B == b.C)
			{
				i++;
			}

			if (a.C == b.A || a.C == b.B || a.C == b.C)
			{
				i++;
			}

			return i == 2;
		}

		/// <summary>
		/// Make a.A == b.A && a.B == b.B && a.C != b.C
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		private void Map(GhostTriangle2D a, GhostTriangle2D b)
		{
			if (!isNeighbor(a, b))
			{
				return;
			}

			int tmp;

			if (a.A != b.A && a.A != b.B && a.A != b.C)
			{
				tmp = a.A;
				a.A = a.C;
				a.C = tmp;
			}

			if (b.B == a.A)
			{
				tmp = b.B;
				b.B = b.A;
				b.A = tmp;
			}
			else if (b.C == a.A)
			{
				tmp = b.C;
				b.C = b.A;
				b.A = tmp;
			}

			if (a.B != b.A && a.B != b.B && a.B != b.C)
			{
				tmp = a.B;
				a.B = a.C;
				a.C = tmp;
			}

			if (b.C == a.B)
			{
				tmp = b.C;
				b.C = b.B;
				b.B = tmp;
			}
		}

		private void SplitGhostTriangle(GhostTriangle2DCollection ghostTriangles, GhostTriangle2D ghostTriangle)
		{
			int Count = ghostTriangles.Count;

			for (int i = 0; i < Count; i++)
			{
				if (isNeighbor(ghostTriangles[i], ghostTriangle))
				{
					Map(ghostTriangles[i], ghostTriangle);

					GhostTriangle2D triangle = new GhostTriangle2D(ghostTriangles[i].A, ghostTriangles[i].C, ghostTriangle.C);

					ghostTriangles.Add(triangle);

					ghostTriangles[i].A = ghostTriangle.C;
				}
			}
		}

		internal void Split(GhostTriangle2DCollection ghostTriangles)
		{
			int Count = ghostTriangles.Count;

			for (int i = 0; i < Count; i++)
			{
				Triangle2D t1 = ghostTriangles[i].ToTriangle(firstPolygon);
				Triangle2D t2 = ghostTriangles[i].ToTriangle(secondPolygon);

				double d1 = QualityDeterminer.ShapeQuality(t1);
				double d2 = QualityDeterminer.ShapeQuality(t2);
				
				if (d1 < TriangleDiviser.ShapeTolerance || d2 < TriangleDiviser.ShapeTolerance)
				{
					GhostTriangle2D ghostTriangle = Split(ghostTriangles[i]);
					SplitGhostTriangle(ghostTriangles, ghostTriangle);
				}
			}
		}

		private GhostTriangle2D Split(GhostTriangle2D ghostTriangle)
		{
			if (QualityDeterminer.TriangleQuality(ghostTriangle, firstPolygon) < QualityDeterminer.TriangleQuality(ghostTriangle, secondPolygon))
			{
				return Split(this.firstPolygon, this.secondPolygon, ghostTriangle);
			}
			else
			{
				return Split(this.secondPolygon, this.firstPolygon, ghostTriangle);
			}
		}

		private GhostTriangle2D Split(Polygon2D first, Polygon2D second, GhostTriangle2D ghostTriangle)
		{
			Triangle2D triangle = ghostTriangle.ToTriangle(first);
			LineSegment2D splitLine = Max(triangle.AB, triangle.BC, triangle.AC);
			int a = first.GetPointIndex(splitLine.FirstPoint);
			int b = first.GetPointIndex(splitLine.LastPoint);
			Point2D p1 = splitLine.MidPoint;
			splitLine = new LineSegment2D(second.GetPoint(a), second.GetPoint(b));
			Point2D p2 = splitLine.MidPoint;

			Polygon2DEditor polygonEditor = new Polygon2DEditor(first);
			polygonEditor.AddInnerPoint(p1);
			polygonEditor = new Polygon2DEditor(second);
			polygonEditor.AddInnerPoint(p2);

			return new GhostTriangle2D(a, b, first.PointCount - 1);
		}

		private LineSegment2D Max(LineSegment2D a, LineSegment2D b, LineSegment2D c)
		{
			LineSegment2D lineSegment = a;

			if (lineSegment.Length < b.Length)
			{
				lineSegment = b;
			}

			if (lineSegment.Length < c.Length)
			{
				lineSegment = c;
			}

			return lineSegment;
		}
	}
}
