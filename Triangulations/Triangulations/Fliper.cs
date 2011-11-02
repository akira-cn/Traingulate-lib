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
	/// Summary description for Fliper.
	/// </summary>
	public class Fliper
	{
		Polygon2D FirstPolygon;
		Polygon2D SecondPolygon;

		public Fliper(Polygon2D firstPolygon, Polygon2D secondPolygon)
		{
			if (firstPolygon.VertexCount != secondPolygon.VertexCount)
			{
				throw (new ArgumentException());
			}

			this.FirstPolygon = firstPolygon;
			this.SecondPolygon = secondPolygon;
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

		private void FlipGhostTriangle(GhostTriangle2D a, GhostTriangle2D b)
		{
			if (!isNeighbor(a, b))
			{
				throw (new ArgumentException());
			}

			Map(a, b);

			a.A = b.C;
			b.B = a.C;
		}

		private Polygon2D FlipArea(GhostTriangle2D a, GhostTriangle2D b, Polygon2D poly)
		{
			if (!isNeighbor(a, b))
			{
				throw (new ArgumentException());
			}

			Map(a, b);

			Point2DCollection points = new Point2DCollection(4);

			points.Add(poly.GetPoint(a.C));
			points.Add(poly.GetPoint(a.A));
			points.Add(poly.GetPoint(b.C));
			points.Add(poly.GetPoint(b.B));

			Polygon2D polygon = new Polygon2D(points);
			return polygon;
		}

		internal void Flipping(GhostTriangle2DCollection ghostTriangles)
		{
			for (int i = 0; i < ghostTriangles.Count; i++)
			{
				for (int j = i; j < ghostTriangles.Count; j++)
				{
					GhostTriangle2D t1 = ghostTriangles[i];
					GhostTriangle2D t2 = ghostTriangles[j];

					if (isNeighbor(t1, t2))
					{
						Polygon2D p1 = FlipArea(t1, t2, this.FirstPolygon);
						Polygon2D p2 = FlipArea(t1, t2, this.SecondPolygon);

						if (p1.isConvex && p2.isConvex)
						{
							double d1 = QualityDeterminer.CompatibleTriangleQuality(t1, this.FirstPolygon, this.SecondPolygon);
							double d2 = QualityDeterminer.CompatibleTriangleQuality(t2, this.FirstPolygon, this.SecondPolygon);

							double d = Math.Min(d1, d2);

							FlipGhostTriangle(t1, t2);

							d1 = QualityDeterminer.CompatibleTriangleQuality(t1, this.FirstPolygon, this.SecondPolygon);
							d2 = QualityDeterminer.CompatibleTriangleQuality(t2, this.FirstPolygon, this.SecondPolygon);

							if (d > Math.Min(d1, d2))
							{
								FlipGhostTriangle(t1, t2);
							}
						}
					}
				}
			}
		}
	}
}
