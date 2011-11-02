//Copyright (C) 2004  Akira (akira_cn@msn.com)

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Library General Public
//License as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Library General Public License for more details.

//You should have received a copy of the GNU Library General Public
//License along with this library; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA  02111-1307, USA.

namespace Akira.Geometry
{
	using System;
	using Akira.Collections;

	/// <summary>
	/// Summary description for Polygon2DCollection.
	/// </summary>
	public class Polygon2DCollection: Collection
	{
		#region Memeber fields
		private Polygon2D[] Polygons = null;
		#endregion

		#region Constructors
		public Polygon2DCollection()
		{
			Polygons = new Polygon2D[defaultCapacity];
			Capacity = defaultCapacity;
		}
		public Polygon2DCollection(int capacity)
		{
			Polygons = new Polygon2D[capacity];
			Capacity = capacity;
		}
		public Polygon2DCollection(Polygon2D[] polygons)
		{
			Capacity = polygons.Length;
			Polygons = polygons;
			CurrentCount = Capacity;
		}
		public Polygon2DCollection(Polygon2D[] polygons, int capacity)
		{
			Capacity = capacity;
			CurrentCount = polygons.Length;
			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			Polygons = polygons;
		}
		#endregion

		#region Public methods
		public void Add(Polygon2D polygon)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				Polygon2D[] polygons = new Polygon2D[Capacity];

				this.Polygons.CopyTo(polygons, 0);
				this.Polygons = polygons;
				base.Add();
			}
			Polygons[CurrentCount - 1] = polygon;
		}
		public void Add(Polygon2D polygon, int index)
		{
			if (index < 0 || index > this.Count)
			{
				throw (new ArgumentException());
			}
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				Polygon2D[] polygons = new Polygon2D[Capacity];

				this.Polygons.CopyTo(polygons, 0);
				this.Polygons = polygons;
				base.Add();
			}

			for (int i = CurrentCount - 1; i > index; i++)
			{
				Polygons[i] = Polygons[i - 1];
			}

			Polygons[index] = polygon;
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}

			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				Polygons[i] = Polygons[i + 1];
			}

			base.Remove();
		}
		public void Union(Polygon2DCollection polygon2DCollection)
		{
			Capacity += polygon2DCollection.Capacity;

			Polygon2D[] polygons = new Polygon2D[Capacity];

			this.Polygons.CopyTo(polygons, 0);
			polygon2DCollection.Polygons.CopyTo(polygons, this.Count);
			this.Polygons = polygons;
			CurrentCount += polygon2DCollection.Count;
		}
		#endregion

		#region Indexer
		public Polygon2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				return this.Polygons[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				this.Polygons[i] = value;
			}
		}
		#endregion
	}
}
