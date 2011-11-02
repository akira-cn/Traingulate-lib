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
	/// Summary description for QualityDeterminer.
	/// </summary>
	public class QualityDeterminer
	{
		static public double TriangleQuality(Triangle2D triangle)
		{
			return Math.Min(triangle.a, Math.Min(triangle.b, triangle.c));
		}

		static internal double TriangleQuality(GhostTriangle2D ghostTriangle, Polygon2D polygon)
		{
			return TriangleQuality(ghostTriangle.ToTriangle(polygon));
		}

		static internal double CompatibleTriangleQuality(GhostTriangle2D ghostTriangle, Polygon2D firstPolygon, Polygon2D secondPolygon)
		{
			double d1 = TriangleQuality(ghostTriangle.ToTriangle(firstPolygon));
			double d2 = TriangleQuality(ghostTriangle.ToTriangle(secondPolygon));

			return Math.Min(d1, d2);
		}

		static internal double ShapeQuality(GhostTriangle2D ghostTriangle, Polygon2D polygon)
		{
			Triangle2D triangle = ghostTriangle.ToTriangle(polygon);
			
			return ShapeQuality(triangle);
		}

		static internal double ShapeQuality(Triangle2D triangle)
		{
			//double angle = TriangleQuality(triangle);
			//double Area = triangle.A.X * triangle.B.Y + triangle.B.X * triangle.C.Y + triangle.C.X * triangle.A.Y - triangle.A.Y * triangle.B.X - triangle.B.Y * triangle.C.X - triangle.C.Y * triangle.A.X;

			//Area /= 2;
			//return Math.Exp(2*(angle - Math.PI / 3)) / Math.Abs(Area);
			
			double val = 0.05 / Math.Max(triangle.AB.Length, Math.Max(triangle.AC.Length, triangle.BC.Length));
			return val;
		}

		static internal double AreaQuality(GhostTriangle2D ghostTriangle, Polygon2D polygon)
		{
			Triangle2D triangle = ghostTriangle.ToTriangle(polygon);
			
			return AreaQuality(triangle);
		}

		static internal double AreaQuality(Triangle2D triangle)
		{
			double Area = triangle.A.X * triangle.B.Y + triangle.B.X * triangle.C.Y + triangle.C.X * triangle.A.Y - triangle.A.Y * triangle.B.X - triangle.B.Y * triangle.C.X - triangle.C.Y * triangle.A.X;

			Area /= 2;

			return 1 / Math.Abs(Area);
		}
	}
}
