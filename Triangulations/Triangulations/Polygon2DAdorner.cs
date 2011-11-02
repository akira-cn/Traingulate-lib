//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Summary description for PolygonAdorner.
	/// </summary>
	public class Polygon2DAdorner
	{
		public readonly Polygon2D polygon = null;
		public readonly LineSegment2DCollection lineSegments = null;
		public readonly Vector2DCollection internalSegments = null;
		public bool ShowIndices = false;

		private GhostTriangle2DCollection ghostTriangles = null;
		private Triangle2DCollection triangles = null;
		private Polygon2DAdorner child = null;

		public Polygon2DAdorner(Polygon2D p)
		{
			polygon = p;
			triangles = new Triangle2DCollection(1);
			lineSegments = new LineSegment2DCollection(1);
			internalSegments = new Vector2DCollection(1);
			ghostTriangles = new GhostTriangle2DCollection(1);
		}

		internal GhostTriangle2DCollection GhostTriangles
		{
			get
			{
				return ghostTriangles;
			}
		}

		private PointF[] PolygonToPoints()
		{
			int Count = this.polygon.VertexCount;
			PointF[] pointF = new PointF[Count];

			for (int i = 0; i < Count; i++)
			{
				pointF[i] = new PointF((float)(this.polygon.GetPoint(i).X), (float)(this.polygon.GetPoint(i).Y));
			}

			return pointF;
		}

		private PointF[] LineSegmentToPoints(int i)
		{
			PointF[] pointF = new PointF[2];

			pointF[0] = new PointF((float)(this.lineSegments[i].FirstPoint.X), (float)(this.lineSegments[i].FirstPoint.Y));
			pointF[1] = new PointF((float)(this.lineSegments[i].LastPoint.X), (float)(this.lineSegments[i].LastPoint.Y));

			return pointF;
		}

		private PointF[] TriangleToPoints(int i)
		{
			PointF[] pointF = new PointF[3];

			pointF[0] = new PointF((float)(this.triangles[i].A.X), (float)(this.triangles[i].A.Y));
			pointF[1] = new PointF((float)(this.triangles[i].B.X), (float)(this.triangles[i].B.Y));
			pointF[2] = new PointF((float)(this.triangles[i].C.X), (float)(this.triangles[i].C.Y));

			return pointF;
		}

		/*private void SetOffset(ref PointF[] points, PointF point)
		{
			for (int i = 0; i < points.Length; i++)
			{
				points[i].X -= point.X;
				points[i].Y -= point.Y;
			}
		}

		private void SetOffset(ref PointF a, PointF b)
		{
			a.X -= b.X;
			a.Y -= b.Y;
		}*/

		public int HitTest(Point2D point)
		{
			for (int i = 0; i < this.polygon.VertexCount; i++)
			{
				Point2D p = this.polygon.GetPoint(i);

				if (Math.Abs(point.X - p.X) <= 3 && Math.Abs(point.Y - p.Y) <= 3)
				{
					return i;
				}
			}

			return -1;
		}

		public void Clear()
		{
			Polygon2DEditor polygonEditor = new Polygon2DEditor(this.polygon);
            
			polygonEditor.Clear();
			this.ghostTriangles.Clear();
			this.lineSegments.Clear();
			this.triangles.Clear();

			if (this.child != null)
			{
				this.child.Clear();
			}
		}

		public void Show(Graphics dc, PointF Offset, double ZoomRatio)
		{
			if (polygon.PointCount == 0)
			{
				return;
			}

			Pen redPen = new Pen(Color.Red, 1);

			for (int i = 0; i < this.polygon.VertexCount; i++)
			{
				PointF point = new PointF();
				point.X = (float)(this.polygon.GetPoint(i).X);
				point.Y = (float)(this.polygon.GetPoint(i).Y);

				this.SetZoom(ref point, Offset, ZoomRatio);

				dc.DrawRectangle(redPen, (int)point.X - 1, (int)point.Y - 1, 3, 3);

				if (this.ShowIndices)
				{
					SolidBrush brush = new SolidBrush(Color.Red);
					Font font = new Font(new FontFamily("Arial"), 8, FontStyle.Regular);

					dc.DrawString(i.ToString(), font, brush, (float)point.X + 8, (float)point.Y - 8);
				}
			}

			if (polygon.PointCount == 1)
			{
				return;
			}

			Pen bluePen = new Pen(Color.Blue, 1);
			PointF[] points = this.PolygonToPoints();

			this.SetZoom(ref points, Offset, ZoomRatio);
			
			if (polygon.isClosed)
			{
				dc.DrawPolygon(bluePen, points);
			}
			else
			{
				dc.DrawLines(bluePen, points);
			}

			bluePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			
			for (int i = 0; i < this.ghostTriangles.Count; i++)
			{
				PointF[] triangle = this.GhostTriangleToPoints(i);

				this.SetZoom(ref triangle, Offset, ZoomRatio);
				dc.DrawPolygon(bluePen, triangle);
				//break;
			}

			if (this.child != null)
			{
				child.Show(dc, Offset, ZoomRatio);
			}
		}

		internal void AppendChild(Polygon2DAdorner child)
		{
			Polygon2DAdorner currentAdorner = this;
			while(currentAdorner.child != null)
			{
				currentAdorner = currentAdorner.child;
			}
			currentAdorner.child = child;
		}

		internal void ApplyGhostTriangles(GhostTriangle2DCollection Triangles)
		{
			this.ghostTriangles = Triangles;
		}

		private PointF[] GhostTriangleToPoints(int i)
		{
			Point2D[] points = new Point2D[3];

			points[0] = this.polygon.GetPoint(this.ghostTriangles[i].A);
			points[1] = this.polygon.GetPoint(this.ghostTriangles[i].B);
			points[2] = this.polygon.GetPoint(this.ghostTriangles[i].C);

			PointF[] pointfs = new PointF[3];

			pointfs[0] = new PointF((float)(points[0].X), (float)(points[0].Y));
			pointfs[1] = new PointF((float)(points[1].X), (float)(points[1].Y));
			pointfs[2] = new PointF((float)(points[2].X), (float)(points[2].Y));

			return pointfs;
		}

		private void Triangulation(int a, int b, int c)
		{
			Debug.Assert(a != b && b != c && c != a);
			
			Triangle2D triangle = new Triangle2D(this.polygon.GetPoint(a), this.polygon.GetPoint(b), this.polygon.GetPoint(c));
			GhostTriangle2D ghost = new GhostTriangle2D(a, b, c);
			this.triangles.Add(triangle);
			this.ghostTriangles.Add(ghost);

			if (lineSegments.Count < polygon.VertexCount - 3)
			{
				Vector2D internalSegment = new Vector2D(a, c);
				LineSegment2D lineSegment = new LineSegment2D(this.polygon.GetPoint(a), this.polygon.GetPoint(c));
				this.lineSegments.Add(lineSegment);
				this.internalSegments.Add(internalSegment);
			}
		}

		public void Triangulation()
		{
			if (!this.polygon.isRegular || !this.polygon.isSimple)
			{
				throw (new ApplicationException("Can not triangulate this polygon!"));
			}

			//this.triangles.Clear();
			//this.lineSegments.Clear();
			this.internalSegments.Clear();
			this.ghostTriangles = new GhostTriangle2DCollection();
			this.Clear();
			
			int Count = this.polygon.VertexCount;
			int[] flag = new int[Count];

			for (int i = 0; i < Count; i++)
			{
				flag[i] = i;
			}

			int a = 0;
			int b = 1;
			int c = 2;

            while (Count > 3)
			{
				LineSegment2D lineSegment = new LineSegment2D(this.polygon.GetPoint(flag[a]), this.polygon.GetPoint(flag[c]));
	
				if (this.polygon.Contains(lineSegment))
				{
					Count--;
					this.Triangulation(flag[a], flag[b], flag[c]);

					for (int i = b; i < Count; i++)
					{
						flag[i] = flag[i + 1];
					}

					a = a % Count;
					b = (a + 1) % Count;
					c = (b + 1) % Count;
					continue;
				}
				else
				{
					a = (a + 1) % Count;
					b = (b + 1) % Count;
					c = (c + 1) % Count;
				}
			}
			this.Triangulation(flag[a],flag[b],flag[c]);

			if (this.child != null)
			{
				this.child.Triangulation();
			}
		}

		private bool isEdge(int a, int b)
		{
			if (Math.Abs(a - b) == 1 || Math.Abs(a - b) == this.polygon.VertexCount - 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				return this.polygon.VertexCount;
			}
		}

		private void SetZoom(ref PointF[] points, PointF point, double ratio)
		{
			for (int i = 0; i < points.Length; i++)
			{
				points[i].X -= point.X;
				points[i].Y -= point.Y;

				points[i].X *= (float)ratio;
				points[i].Y *= (float)ratio;
			}
		}

		private void SetZoom(ref PointF a, PointF b, double ratio)
		{
			a.X -= b.X;
			a.Y -= b.Y;
			a.X *= (float)ratio;
			a.Y *= (float)ratio;
		}

		public Polygon2DAdorner Next()
		{
			return this.child;
		}
	}
}
