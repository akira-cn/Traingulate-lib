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
	/// 
	/// </summary>
	internal class GhostPoint2D
	{
		public readonly int FirstIndex;
		public readonly int LastIndex;
		public readonly double LocalPosition;
		public GhostPoint2D(int a, int b, double c)
		{
			FirstIndex = a;
			LastIndex = b;
			LocalPosition = c;
		}
	}
	/// <summary>
	/// Summary description for GhostPoint2DCollection.
	/// </summary>
	internal class GhostPoint2DCollection: Collection
	{
		#region Memeber fields
		private GhostPoint2D[] GhostPoints = null;
		#endregion

		#region Constructors
		public GhostPoint2DCollection()
		{
			GhostPoints = new GhostPoint2D[defaultCapacity];
			Capacity = defaultCapacity;
		}
		public GhostPoint2DCollection(int capacity)
		{
			GhostPoints = new GhostPoint2D[capacity];
			Capacity = capacity;
		}
		public GhostPoint2DCollection(GhostPoint2D[] ghostPoints)
		{
			Capacity = ghostPoints.Length;
			GhostPoints = ghostPoints;
			CurrentCount = Capacity;
		}
		public GhostPoint2DCollection(GhostPoint2D[] ghostPoints, int capacity)
		{
			Capacity = capacity;
			CurrentCount = ghostPoints.Length;
			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			GhostPoints = ghostPoints;
		}
		#endregion

		#region Public methods
		public void Add(GhostPoint2D ghostPoint)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				GhostPoint2D[] ghostPoints = new GhostPoint2D[Capacity];

				this.GhostPoints.CopyTo(ghostPoints, 0);
				this.GhostPoints = ghostPoints;
				base.Add();
			}
			GhostPoints[CurrentCount - 1] = ghostPoint;
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}

			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				GhostPoints[i] = GhostPoints[i + 1];
			}

			base.Remove();
		}
		public void Union(GhostPoint2DCollection ghostPoint2DCollection)
		{
			Capacity += ghostPoint2DCollection.Capacity;

			GhostPoint2D[] ghostPoints = new GhostPoint2D[Capacity];

			this.GhostPoints.CopyTo(ghostPoints, 0);
			ghostPoint2DCollection.GhostPoints.CopyTo(ghostPoints, this.Count);
			this.GhostPoints = ghostPoints;
			CurrentCount += ghostPoint2DCollection.Count;
		}
		#endregion

		#region Indexer
		public GhostPoint2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				return this.GhostPoints[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				this.GhostPoints[i] = value;
			}
		}
		#endregion
	}
}
