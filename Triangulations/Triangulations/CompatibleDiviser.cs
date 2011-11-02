//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Diagnostics;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// CompatibleDiviser 的摘要说明。
	/// </summary>
	public class CompatibleDiviser
	{
		private Polygon2D firstPolygon;
		private Polygon2D secondPolygon;
		private TargetDiviser targetDiviser;
		private Polygon2DLinkMaker linkMaker;
		private Polygon2DCollection firstPolygonCollection;
		private Polygon2DCollection secondPolygonCollection;
	
		public Polygon2DCollection FirstPolygonDivisions
		{
			get
			{
				return this.firstPolygonCollection;
			}
		}
		public Polygon2DCollection SecondPolygonDivisions
		{
			get
			{
				return this.secondPolygonCollection;
			}
		}

		public CompatibleDiviser(Polygon2D a, Polygon2D b)
		{
			this.firstPolygon = a;
			this.secondPolygon = b;

			this.firstPolygonCollection = new Polygon2DCollection();
			this.secondPolygonCollection = new Polygon2DCollection();

			targetDiviser = new TargetDiviser(b);
		}

		public void Divide()
		{
			UpdateStatus(this, "Dividing...");
			targetDiviser.UpdateStatus += new Microsoft.VS.Akira.Triangulations.TargetDiviser.ShowStatus(UpdateStatus);
			targetDiviser.Divide();
			Polygon2D poly = this.firstPolygon;
			this.firstPolygonCollection.Add(this.firstPolygon);
			this.secondPolygonCollection.Add(this.secondPolygon);
			UpdateStatus(this, "Mapping...");
			for (int i = 0; i < targetDiviser.Divisers.Count; i++)
			{
				int first_index = this.secondPolygon.GetPointIndex(targetDiviser.Divisers[i].FirstPoint);
				int second_index = this.secondPolygon.GetPointIndex(targetDiviser.Divisers[i].LastPoint);

				LineSegment2D line = new LineSegment2D(this.firstPolygon.GetPoint(first_index), this.firstPolygon.GetPoint(second_index));

				for (int j = 0; j < this.firstPolygonCollection.Count; j++)
				{
					if (this.firstPolygonCollection[j].HasVertex(line.FirstPoint) != Polygon2D.NoSuchPoint
						&&this.firstPolygonCollection[j].HasVertex(line.LastPoint) != Polygon2D.NoSuchPoint)
					{
						poly = this.firstPolygonCollection[j];
						this.firstPolygonCollection.Remove(j);
						break;
					}
				}
				int from = 0;
				int to = 0;

				from = poly.GetPointIndex(line.FirstPoint);
				to = poly.GetPointIndex(line.LastPoint);

				linkMaker = new Polygon2DLinkMaker(poly, from, to);

				linkMaker.Divide();
				int extraPoints = linkMaker.LinkDistance - 1;
				linkMaker.BuildPath();
				Polygon2DDiviser pd = new Polygon2DDiviser(poly);
				pd.DividedBy(linkMaker.LinkDivisers, poly);
				this.firstPolygonCollection.Add(pd.SubDivision[1]);
				this.firstPolygonCollection.Add(pd.SubDivision[2]);

				//divide target polygon
				line = targetDiviser.Divisers[i];

				for (int j = 0; j < this.secondPolygonCollection.Count; j++)
				{
					if (this.secondPolygonCollection[j].HasVertex(line.FirstPoint) != Polygon2D.NoSuchPoint
						&&this.secondPolygonCollection[j].HasVertex(line.LastPoint) != Polygon2D.NoSuchPoint)
					{
						poly = this.secondPolygonCollection[j];
						this.secondPolygonCollection.Remove(j);
						break;
					}
				}
				
				from = poly.GetPointIndex(line.FirstPoint);
				to = poly.GetPointIndex(line.LastPoint);

				Point2DCollection path = line.ToPath(extraPoints);

				pd = new Polygon2DDiviser(poly);
				pd.DividedBy(path, poly);

				this.secondPolygonCollection.Add(pd.SubDivision[1]);
				this.secondPolygonCollection.Add(pd.SubDivision[2]);
			}
		}
		public delegate void ShowStatus(object sender, string msg);
        public event ShowStatus UpdateStatus;
	}
}
