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
	/// Summary description for GhostWebBuilder.
	/// </summary>
	internal class GhostWeb 
	{
		private const double AreaTolerance = 0.000001;
		private Polygon2D Polygon;
		public readonly WebNode[] webNodes;
		private bool available = true;

		public GhostWeb(Polygon2D poly, GhostTriangle2DCollection ghostTriangles)
		{
			Polygon = poly;

			webNodes = new WebNode[poly.PointCount];

			this.BuildGhostWeb(ghostTriangles);
		}

		private GhostWeb(Polygon2D poly)
		{
			Polygon = poly;

			webNodes = new WebNode[poly.PointCount];
		}

		public void BuildGhostWeb(GhostTriangle2DCollection ghostTriangles)
		{
			this.available = true;

			for (int i = 0; i < ghostTriangles.Count;  i++)
			{
				int a = ghostTriangles[i].A;
				int b = ghostTriangles[i].B;
				int c = ghostTriangles[i].C;
				
				this.BuildWebNode(a, b, c);
				this.BuildWebNode(b, c, a);
				this.BuildWebNode(c, a, b);

				if (!this.available)
				{
					return;
				}
			}
		}

		private bool isRegular(int index, Point2D point)
		{
			WebNode Router = this.webNodes[index];

			while (Router != null)
			{
				if (this.GetArea(Router.firstIndex, Router.secondIndex, point.X, point.Y) < GhostWeb.AreaTolerance)
				{
					return false;
				}
				Router = Router.Next;
			}
			return true;
		}

		private void BuildWebNode(int a, int b, int c)
		{
			Point2D point = this.Polygon.GetPoint(a);

			if (this.GetArea(b, c, point.X, point.Y) < 0)
			{
				int tmp;
				tmp = b;
				b = c;
				c = tmp;
			}
			
			//Debug.Assert(this.GetArea(b, c, point.X, point.Y ) >= 0);

			//if (this.GetArea(b, c, point.X, point.Y) < 0)
			//{
			//	this.available = false;
			//	return;
			//}

			if (webNodes[a] == null)
			{
				webNodes[a] = new WebNode(b, c, a);
			}

			else
			{
				webNodes[a].Add(new WebNode(b, c, a));
			}

		}

		private double GetArea(int i,int j, double x, double y)
		{
			Point2D point = this.Polygon.GetPoint(i);

			double x1 = point.X;
			double y1 = point.Y;

			point = this.Polygon.GetPoint(j);

			double x2 = point.X;
			double y2 = point.Y;

			double s = x1 * y2 + x * y1 + x2 * y - y2 * x - y1 * x2 - x1 * y;
			s *= 0.5;

			return s;
		}

		private double GetPolygonArea(int index)
		{
			double d = 0.0;
			WebNode Router = this.webNodes[index];

			int count = 0;

			while (Router != null)
			{
				d += this.GetArea(Router.firstIndex, Router.secondIndex, 0.0, 0.0);
				Router = Router.Next;
				count++;
			}

			return d/count;
		}

		private double[] GetValue(int index)
		{
			double[] d = new double[8];

			WebNode Router = this.webNodes[index];

			int count = 0;

			while (Router != null)
			{
				Point2D p1 = this.Polygon.GetPoint(Router.firstIndex);
				Point2D p2 = this.Polygon.GetPoint(Router.secondIndex);

				double dx = p2.X - p1.X;
				double dy = p1.Y - p2.Y;

				d[0] += dx; // x2 - x1;
				d[1] += dy; // y1 - y2;
				d[2] += dx * dx; 
				d[3] += dy * dy;
				d[4] += dx * dy;
				d[5] += (p1.X * p2.Y - p2.X * p1.Y) * dy;
				d[6] += (p1.X * p2.Y - p2.X * p1.Y) * dx;
				d[7] += this.GetArea(Router.firstIndex, Router.secondIndex, 0.0, 0.0);

				Router = Router.Next;
				count++;
			}

			d[7] /= count;

			return d;
		}

		public void AreaBasedRemeshing()
		{
			if (!this.available)
			{
				return;
			}

			for (int i = this.Polygon.VertexCount; i < this.Polygon.PointCount;  i++)
			{
				if (this.Polygon.isOnEdge(this.Polygon.GetPoint(i)) || this.Polygon.isVertex(this.Polygon.GetPoint(i)))
				{
					continue;
				}
				
				double[] d = this.GetValue(i);
				
				Line2D line1 = null;
				Line2D line2 = null;
				Point2D point = null;
				
				try
				{
					line1 = new Line2D(0.5 * d[3], 0.5 * d[4], 0.5 * d[5] - d[7] * d[1]);
					line2 = new Line2D(0.5 * d[4], 0.5 * d[2], 0.5 * d[6] - d[7] * d[0]);
				}
				catch (ArgumentException)
				{
					return;
				}

				point = line1.Intersects(line2);

				if (point.isRegular && this.isRegular(i, point) && this.Polygon.Contains(point))
				{
					Polygon2DEditor polygonEditor = new Polygon2DEditor(this.Polygon);

					polygonEditor.SetPoint(point.X, point.Y, i);
				}
			}
		}
	}

	internal class WebNode
	{
		/// <summary>
		/// The Shared Vertex Index of the GhostTriangle.
		/// </summary>
		public readonly int vertexIndex;
		/// <summary>
		/// The First Index of the GhostTriangle.
		/// </summary>
		public readonly int firstIndex;
		/// <summary>
		/// The Second Index of the GhostTriangle.
		/// </summary>
		public readonly int secondIndex;

		public WebNode Next;
		public WebNode Last;

		public WebNode(int x, int y, int index)
		{
			vertexIndex = index;
			firstIndex = x;
			secondIndex = y;

			Next = null;
			Last = this;
		}

		public void Add(WebNode webNode)
		{
			Last.Next = webNode;
			Last = webNode;
		}
	}
}
