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
	using Akira.Collections;

	/// <summary>
	/// Summary description for Triangle2DCollection.
	/// </summary>
	public class Triangle2DCollection: Collection
	{
		#region Memeber fields
		private Triangle2D[] Triangles = null;
		#endregion

		#region Constructors
		public Triangle2DCollection()
		{
			Triangles = new Triangle2D[defaultCapacity];
			Capacity = defaultCapacity;
		}
		public Triangle2DCollection(int capacity)
		{
			Triangles = new Triangle2D[capacity];
			Capacity = capacity;
		}
		public Triangle2DCollection(Triangle2D[] triangles)
		{
			Capacity = triangles.Length;
			Triangles = triangles;
			CurrentCount = Capacity;
		}
		public Triangle2DCollection(Triangle2D[] triangles, int capacity)
		{
			Capacity = capacity;
			CurrentCount = triangles.Length;
			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			Triangles = triangles;
		}
		#endregion

		#region Public methods
		public void Add(Triangle2D triangle)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				Triangle2D[] triangles = new Triangle2D[Capacity];

				this.Triangles.CopyTo(triangles, 0);
				this.Triangles = triangles;
				base.Add();
			}
			Triangles[CurrentCount - 1] = triangle;
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}

			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				Triangles[i] = Triangles[i + 1];
			}

			base.Remove();
		}
		public void Union(Triangle2DCollection triangle2DCollection)
		{
			Capacity += triangle2DCollection.Capacity;

			Triangle2D[] triangles = new Triangle2D[Capacity];

			this.Triangles.CopyTo(triangles, 0);
			triangle2DCollection.Triangles.CopyTo(triangles, this.Count);
			this.Triangles = triangles;
			CurrentCount += triangle2DCollection.Count;
		}
		#endregion

		#region Indexer
		public Triangle2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				return this.Triangles[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				this.Triangles[i] = value;
			}
		}
		#endregion
	}
}

