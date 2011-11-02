using System;
using Akira.Collections;
//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using Akira.Geometry;
using Akira.MatrixLib;
using System.Diagnostics;

namespace Microsoft.VS.Akira.Triangulations
{
	internal class GhostTriangle2D
	{
		private int[] Indices;
		internal enum Position{Equal = 3, Neighbor = 2, Insect = 1, Faraway = 0};

		public GhostTriangle2D(int a, int b, int c)
		{
			Indices = new int[3];
			Indices[0] = a;
			Indices[1] = b;
			Indices[2] = c;
		}
		public int A
		{
			get
			{
				return Indices[0];
			}
			set
			{
				Indices[0] = value;
			}
		}
		public int B
		{
			get
			{
				return Indices[1];
			}
			set
			{
				Indices[1] = value;
			}
		}
		public int C
		{
			get
			{
				return Indices[2];
			}
			set
			{
				Indices[2] = value;
			}
		}
		public Triangle2D ToTriangle(Polygon2D polygon)
		{
			Point2D a = polygon.GetPoint(this.A);
			Point2D b = polygon.GetPoint(this.B);
			Point2D c = polygon.GetPoint(this.C);

			return new Triangle2D(a, b, c);
		}
		public static Position GetPosition(Triangle2D a, Triangle2D b)
		{
			int ret = 0;

			if (a.A == b.A || a.A == b.B || a.A == b.C)
			{
				ret++;	
			}
			if (a.B == b.A || a.B == b.B || a.B == b.C)
			{
				ret++;	
			}
			if (a.C == b.A || a.C == b.B || a.C == b.C)
			{
				ret++;	
			}
            
			return (Position)ret;
		}
	}
	/// <summary>
	/// Summary description for GhostPoint2DCollection.
	/// </summary>
	internal class GhostTriangle2DCollection: Collection
	{
		#region Memeber fields
		private GhostTriangle2D[] GhostTriangles = null;
		#endregion

		#region Constructors
		public GhostTriangle2DCollection()
		{
			GhostTriangles = new GhostTriangle2D[defaultCapacity];
			Capacity = defaultCapacity;
		}
		public GhostTriangle2DCollection(int capacity)
		{
			GhostTriangles = new GhostTriangle2D[capacity];
			Capacity = capacity;
		}
		public GhostTriangle2DCollection(GhostTriangle2D[] ghostTriangles)
		{
			Capacity = ghostTriangles.Length;
			GhostTriangles = ghostTriangles;
			CurrentCount = Capacity;
		}
		public GhostTriangle2DCollection(GhostTriangle2D[] ghostTriangles, int capacity)
		{
			Capacity = capacity;
			CurrentCount = ghostTriangles.Length;
			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			GhostTriangles = ghostTriangles;
		}
		#endregion

		#region Public methods
		public void Add(GhostTriangle2D ghostTriangle)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				GhostTriangle2D[] ghostTriangles = new GhostTriangle2D[Capacity];

				this.GhostTriangles.CopyTo(ghostTriangles, 0);
				this.GhostTriangles = ghostTriangles;
				base.Add();
			}
			GhostTriangles[CurrentCount - 1] = ghostTriangle;
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}

			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				GhostTriangles[i] = GhostTriangles[i + 1];
			}

			base.Remove();
		}
		public void Union(GhostTriangle2DCollection ghostTriangle2DCollection)
		{
			Capacity += ghostTriangle2DCollection.Capacity;

			GhostTriangle2D[] ghostTriangles = new GhostTriangle2D[Capacity];

			this.GhostTriangles.CopyTo(ghostTriangles, 0);
			ghostTriangle2DCollection.GhostTriangles.CopyTo(ghostTriangles, this.Count);
			this.GhostTriangles = ghostTriangles;
			CurrentCount += ghostTriangle2DCollection.Count;
		}
		#endregion

		#region Indexer
		public GhostTriangle2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				return this.GhostTriangles[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				this.GhostTriangles[i] = value;
			}
		}
		#endregion
	}
}
