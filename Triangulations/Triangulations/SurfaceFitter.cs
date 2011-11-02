//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using Akira.Geometry;
using Akira.MatrixLib;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// SurfaceFitter 的摘要说明。
	/// </summary>
	public class SurfaceFitter
	{
		private Polygon2D sourcePolygon;
		private Polygon2D targetPolygon;
		internal readonly GhostTriangle2DCollection ghostTriangles;
		
		internal SurfaceFitter(Polygon2D a, Polygon2D b, GhostTriangle2DCollection ghost)
		{
			if (a.VertexCount != b.VertexCount)
			{
				throw (new ArgumentException());
			}

			sourcePolygon = a;
			targetPolygon = b;
			ghostTriangles = ghost;
		}

		public void Fitting()
		{
			GhostWeb ghostWeb = new GhostWeb(this.sourcePolygon, this.ghostTriangles);

			Vector[] vectors = new Vector[this.sourcePolygon.PointCount];

			for (int i = 0; i < vectors.Length; i++)
			{
				vectors[i] = new Vector(vectors.Length);
				vectors[i][i] = 1;

				if (i < this.sourcePolygon.VertexCount || this.sourcePolygon.isOnEdge(this.sourcePolygon.GetPoint(i)))
				{
					continue;
				}

				else
				{
					vectors[i][i] = 0;
					WebNode webNode = ghostWeb.webNodes[i];

					while (webNode!= null)
					{
						vectors[i][i] ++;
						vectors[i][webNode.firstIndex] = -1;
						vectors[i][webNode.secondIndex] = -1;

						webNode = webNode.Next;
					}
				}
			}

			Vector valx = new Vector(this.sourcePolygon.PointCount);
			Vector valy = new Vector(this.sourcePolygon.PointCount);

			for (int i = 0; i < this.targetPolygon.VertexCount; i++)
			{
				valx[i] = this.targetPolygon.GetPoint(i).X;
				valy[i] = this.targetPolygon.GetPoint(i).Y;
			}

			for (int i = this.sourcePolygon.VertexCount; i <this.sourcePolygon.PointCount; i++)
			{
				Point2D point = this.sourcePolygon.GetPoint(i);
				if(this.sourcePolygon.isOnEdge(point))
				{

					int x = this.sourcePolygon.OnEdge(point);
					LineSegment2D line = this.sourcePolygon.GetEdge(x);

					double pos = line.GetPosition(point);
					line = this.targetPolygon.GetEdge(x);

					Point2D newPoint = line.GetPoint(pos);

					valx[i] = newPoint.X;
					valy[i] = newPoint.Y;
				}
			}

			Matrix m = new Matrix(vectors).Translate();

			UpdateStatus(this, "Constructing...");

			LinearSystem lsx = new LinearSystem(new Matrix(m), valx);

			UpdateStatus(this, "Done");

			lsx.UpdateStatus += new Microsoft.VS.Akira.Triangulations.LinearSystem.ShowStatus(UpdateStatus);
			lsx.Gaussian(this.sourcePolygon.VertexCount);

			LinearSystem lsy = new LinearSystem(new Matrix(m), valy);
			lsy.UpdateStatus += new Microsoft.VS.Akira.Triangulations.LinearSystem.ShowStatus(UpdateStatus);
			lsy.Gaussian(this.sourcePolygon.VertexCount);

			Polygon2DEditor pe = new Polygon2DEditor(this.targetPolygon);

			UpdateStatus(this, "AddInner..");
			
			for (int i = this.sourcePolygon.VertexCount; i < this.sourcePolygon.PointCount; i++)
			{
				Point2D point = new Point2D(lsx.Value[i], lsy.Value[i]);
				pe.AddInnerPoint(point);
			}

			UpdateStatus(this, "Done");
		}

		public delegate void ShowStatus(object sender, string msg);
		public event ShowStatus UpdateStatus;
	}
}
