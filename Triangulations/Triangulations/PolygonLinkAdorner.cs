using System;
using Akira.Geometry;
using Akira.MatrixLib;
//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;


namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Summary description for PolygonLinkAdorner.
	/// </summary>
	public class PolygonLinkAdorner
	{
		Polygon2DLinkMaker polygonLink = null;

		public PolygonLinkAdorner(Polygon2DLinkMaker link)
		{
			polygonLink = link;
		}

		public void show(Graphics dc)
		{
			for (int i = 0; i < polygonLink.SubDivision.Count; i++)
			{
				Polygon2DAdorner polygonAdorner = new Polygon2DAdorner(polygonLink.SubDivision[i]);
				polygonAdorner.Show(dc, new PointF(0.0F, -30.0F), 1.0);
			}

			if (polygonLink.LinkDivisers == null || polygonLink.LinkDivisers.Count == 0)
			{
				return;
			}

			PointF[] points = new PointF[polygonLink.LinkDivisers.Count];

			for (int i = 0; i < polygonLink.LinkDivisers.Count; i++)
			{
				points[i] = new PointF((float)polygonLink.LinkDivisers[i].X, (float)polygonLink.LinkDivisers[i].Y + 30);
			}

			Pen redPen = new Pen(Color.Red);

			dc.DrawLines(redPen, points);
		}
	}
}
