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
	/// Summary description for HighQualityTriangulator.
	/// </summary>
	public class HighQualityTriangulator
	{
		private const int MAX_DIST = 10000;
		private Polygon2DLinkMaker FirstTarget = null;
		private Polygon2DLinkMaker SecondTarget = null;
		private Polygon2DDiviser FirstPolygons = null;
		private Polygon2DDiviser SecondPolygons = null;
		private Polygon2DCollection FirstResult = null;
		private Polygon2DCollection SecondResult = null;
		private GhostTriangle2DCollection ghostTriangle = null;
		private LinkDistance linkDistance = null;

		internal GhostTriangle2DCollection GhostTriangle
		{
			get
			{
				return ghostTriangle;
			}
		}

		public HighQualityTriangulator(Polygon2D a, Polygon2D b)
		{
			if (!a.isRegular && !b.isRegular)
			{
				throw (new ArgumentException());
			}
			if (a.VertexCount != b.VertexCount)
			{
				throw (new ArgumentException());
			}

			FirstPolygons = new Polygon2DDiviser(a);
			SecondPolygons = new Polygon2DDiviser(b);
		}

		private void InitLinkDistance()
		{
			int Count = this.FirstPolygons.Parent.VertexCount;
	
			Polygon2D firstPolygon = FirstPolygons.Parent;
			Polygon2D secondPolygon = SecondPolygons.Parent;

			for (int i = 0; i < Count; i++)
			{

				for (int j = i + 2; j < Count; j++)
				{
					if (i == j || Math.Abs(i - j) == 1 || Math.Abs(i - j) == Count - 1)
					{
						continue;
					}

					LineSegment2D testLine = new LineSegment2D(firstPolygon.GetPoint(i), firstPolygon.GetPoint(j));
					if (firstPolygon.InflectsEdge(testLine))
					{
						continue;
					}

					testLine = new LineSegment2D(secondPolygon.GetPoint(i), secondPolygon.GetPoint(j));

					if (secondPolygon.InflectsEdge(testLine))
					{
						continue;
					}

					FirstTarget = new Polygon2DLinkMaker(firstPolygon, i, j);
					SecondTarget = new Polygon2DLinkMaker(secondPolygon, i, j);
					FirstTarget.Divide();
					SecondTarget.Divide();

					int linkDistance = Math.Max(FirstTarget.LinkDistance, SecondTarget.LinkDistance);

					if (this.linkDistance == null)
					{
						this.linkDistance = new LinkDistance(i, j, 0, linkDistance);
					}
					else
					{
						this.linkDistance.Add(new LinkDistance(i, j, 0, linkDistance));
					}
				}
			}
		}

		public void FastTriangluation()
		{
			int from = 0;
			int to = 2;
			Polygon2DEditor polygonEditor = new Polygon2DEditor(this.FirstPolygons.Parent);

			polygonEditor.Clear();
			polygonEditor = new Polygon2DEditor(this.SecondPolygons.Parent);
			polygonEditor.Clear();

			this.InitLinkDistance();

			for (int i = 0; i < FirstPolygons.SubDivision.Count; i++)
			{
				int vertexCount = FirstPolygons.SubDivision[i].VertexCount;
				Polygon2D firstPolygon = FirstPolygons.SubDivision[i];
				Polygon2D secondPolygon = SecondPolygons.SubDivision[i];

				if (vertexCount <= 3)
				{
					continue;
				}

				int dist = MAX_DIST;

				for (int j = 0; j < vertexCount; j++)
				{
					for (int k = j + 2; k < vertexCount; k++)
					{
						if (j == k || Math.Abs(j - k) == 1 || Math.Abs(j - k) == vertexCount - 1)
						{
							continue;
						}

						LineSegment2D testLine = new LineSegment2D(firstPolygon.GetPoint(j), firstPolygon.GetPoint(k));

						if (firstPolygon.InflectsEdge(testLine))
						{
							continue;
						}

						testLine = new LineSegment2D(secondPolygon.GetPoint(j), secondPolygon.GetPoint(k));
						if (secondPolygon.InflectsEdge(testLine))
						{
							continue;
						}

						LinkDistance link = this.linkDistance.Contains(this.FirstPolygons.GetParentIndex(j, i), this.FirstPolygons.GetParentIndex(k, i), 0);
						
						int d;

						if (link != null)
						{
							d = link.linkDistance;
						}

						else
						{
							FirstTarget = new Polygon2DLinkMaker(firstPolygon, j, k);
							SecondTarget = new Polygon2DLinkMaker(secondPolygon, j, k);
							FirstTarget.Divide();
							SecondTarget.Divide();

							d = Math.Max(FirstTarget.LinkDistance, SecondTarget.LinkDistance);

							link = new LinkDistance(this.FirstPolygons.GetParentIndex(j, i), this.FirstPolygons.GetParentIndex(k, i), 0, d);
						}

						if (dist > d)
						{
							dist = d;
							from = j;
							to = k;
						}
						else if (dist == d)
						{
							if (Math.Abs(vertexCount / 2 - Math.Abs(j - k)) > Math.Abs(vertexCount / 2 - Math.Abs(from - to)))
							{
								from = j;
								to = k;
							}
						}
					}
				}

				FirstTarget = new Polygon2DLinkMaker(firstPolygon, from, to);
				SecondTarget = new Polygon2DLinkMaker(secondPolygon, from, to);
				FirstTarget.Divide();
				FirstTarget.BuildPath();
				SecondTarget.Divide();
				SecondTarget.BuildPath();

				if (dist != Math.Max(FirstTarget.LinkDistance, SecondTarget.LinkDistance))
				{
					dist = Math.Max(FirstTarget.LinkDistance, SecondTarget.LinkDistance);
					this.linkDistance.Set(this.FirstPolygons.GetParentIndex(from, i), this.FirstPolygons.GetParentIndex(to, i), 0, dist);
				}

				int Extra = dist - FirstTarget.LinkDistance;

				if (Extra > 0)
				{
					FirstTarget.LinkDivisers.Extend(Extra);
				}

				Extra = dist - SecondTarget.LinkDistance;
				if (Extra > 0)
				{
					SecondTarget.LinkDivisers.Extend(Extra);
				}

				FirstPolygons.DividedBy(FirstTarget.LinkDivisers, firstPolygon);
				SecondPolygons.DividedBy(SecondTarget.LinkDivisers, secondPolygon);
			}

			GetResult();
			BuildGhostTriangle();
		}

		public void Triangulation()
		{
			int from = 0;
			int to = 2;
			Polygon2DEditor polygonEditor = new Polygon2DEditor(this.FirstPolygons.Parent);
			polygonEditor.Clear();
			polygonEditor = new Polygon2DEditor(this.SecondPolygons.Parent);
			polygonEditor.Clear();

			for (int i = 0; i < FirstPolygons.SubDivision.Count; i++)
			{
				int vertexCount = FirstPolygons.SubDivision[i].VertexCount;
				Polygon2D firstPolygon = FirstPolygons.SubDivision[i];
				Polygon2D secondPolygon = SecondPolygons.SubDivision[i];

				if (vertexCount <= 3)
				{
					continue;
				}
				int dist = MAX_DIST;
				for (int j = 0; j < vertexCount; j++)
				{
					for (int k = j + 2; k < vertexCount; k++)
					{
						if (j == k || Math.Abs(j - k) == 1 || Math.Abs(j - k) == vertexCount - 1)
						{
							continue;
						}

						LineSegment2D testLine = new LineSegment2D(firstPolygon.GetPoint(j), firstPolygon.GetPoint(k));
						if (firstPolygon.InflectsEdge(testLine))
						{
							continue;
						}

						testLine = new LineSegment2D(secondPolygon.GetPoint(j), secondPolygon.GetPoint(k));

						if (secondPolygon.InflectsEdge(testLine))
						{
							continue;
						}

						FirstTarget = new Polygon2DLinkMaker(firstPolygon, j, k);
						SecondTarget = new Polygon2DLinkMaker(secondPolygon, j, k);
						FirstTarget.Divide();
						SecondTarget.Divide();

						int d = Math.Max(FirstTarget.LinkDistance, SecondTarget.LinkDistance);

						if (dist > d)
						{
							dist = d;
							from = j;
							to = k;
						}
						else if (dist == d)
						{
							if (Math.Abs(vertexCount/2 - Math.Abs(j - k)) > Math.Abs(vertexCount/2 - Math.Abs(from - to)))
							{
								from = j;
								to = k;
							}
						}
					}
				}

				FirstTarget = new Polygon2DLinkMaker(firstPolygon, from, to);
				SecondTarget = new Polygon2DLinkMaker(secondPolygon, from, to);
				FirstTarget.Divide();
				FirstTarget.BuildPath();
				SecondTarget.Divide();
				SecondTarget.BuildPath();
				int Extra = dist - FirstTarget.LinkDistance;
				if (Extra > 0)
				{
					FirstTarget.LinkDivisers.Extend(Extra);
				}
				Extra = dist - SecondTarget.LinkDistance;
				if (Extra > 0)
				{
					SecondTarget.LinkDivisers.Extend(Extra);
				}
				FirstPolygons.DividedBy(FirstTarget.LinkDivisers, firstPolygon);
				SecondPolygons.DividedBy(SecondTarget.LinkDivisers, secondPolygon);
			}
			GetResult();
			BuildGhostTriangle();
		}
		private void GetResult()
		{
			this.FirstResult = new Polygon2DCollection();
			this.SecondResult = new Polygon2DCollection();

			for (int i = 0; i < FirstPolygons.SubDivision.Count; i++)
			{
				if (FirstPolygons.SubDivision[i].VertexCount == 3)
				{
					this.FirstResult.Add(FirstPolygons.SubDivision[i]);
					this.SecondResult.Add(SecondPolygons.SubDivision[i]);
				}
			}
		}
		private void BuildGhostTriangle()
		{
			this.ghostTriangle = new GhostTriangle2DCollection();

			for (int i = 0; i < this.FirstResult.Count; i++)
			{
				Polygon2D polygon = this.FirstResult[i];
				
				int a = this.FirstPolygons.Parent.GetPointIndex(polygon.GetPoint(0));
				int b = this.FirstPolygons.Parent.GetPointIndex(polygon.GetPoint(1));
				int c = this.FirstPolygons.Parent.GetPointIndex(polygon.GetPoint(2));

				this.ghostTriangle.Add(new GhostTriangle2D(a, b, c));
			}
		}
	}
	internal class LinkDistance
	{
		private int subDivision;
		private int firstPointIndex;
		private int lastPointIndex;
		public int linkDistance;
		public LinkDistance Next;
		public LinkDistance Last;

		public LinkDistance(int a, int b, int index, int link)
		{
			subDivision = index;
			firstPointIndex = a;
			lastPointIndex = b;
			this.linkDistance = link;

			Next = null;
			Last = this;
		}

		public void Add(LinkDistance linkDistance)
		{
			Last.Next = linkDistance;
			Last = linkDistance;
		}

		public void Add(int a, int b, int index, int link)
		{
			LinkDistance newLinkDistance = new LinkDistance(a, b, index, link);
			Add(newLinkDistance);
		}

		public LinkDistance Contains(int a, int b, int index)
		{
			LinkDistance Router = this;

			while (Router != null)
			{
				if (Router.firstPointIndex == a && Router.lastPointIndex == b && Router.subDivision == index)
				{
					return Router;
				}
				Router = Router.Next;
			}

			return null;
		}

		public void Set(int a, int b, int index, int link)
		{
			LinkDistance Router = this;

			while (Router != null)
			{
				if (Router.firstPointIndex == a && Router.lastPointIndex == b && Router.subDivision == index)
				{
					Router.linkDistance = link;
					return;
				}

				Router = Router.Next;
			}
		}
	}
}
