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
	/// Rebuild 的摘要说明。
	/// </summary>
	public class Rebuild
	{
		public readonly Polygon2D polygon;
		internal readonly GhostTriangle2DCollection ghostTriangles;

		internal Rebuild(Polygon2D poly)
		{
			polygon = new Polygon2D(poly);
			ghostTriangles = new GhostTriangle2DCollection();
		}

	}
}
