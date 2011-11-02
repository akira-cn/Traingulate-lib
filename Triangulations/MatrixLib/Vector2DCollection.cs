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

namespace Akira.MatrixLib
{
	using Akira.Collections;

	/// <summary>
	/// Summary description for Vector2DCollection.
	/// </summary>
	public class Vector2DCollection: Collection
	{
		#region Memeber fields
		private Vector2D[] Vectors = null;
		#endregion

		#region Properties

		public Vector2D FirstVector
		{
			get
			{
				return Vectors[0];
			}
		}

		public Vector2D LastVector
		{
			get
			{
				return Vectors[Count - 1];
			}
		}

		#endregion

		#region Constructors
		public Vector2DCollection()
		{
			Vectors = new Vector2D[defaultCapacity];
			Capacity = defaultCapacity;
		}
		public Vector2DCollection(int capacity)
		{
			Vectors = new Vector2D[capacity];
			Capacity = capacity;
		}
		public Vector2DCollection(Vector2D[] vectors)
		{
			Capacity = vectors.Length;
			Vectors = vectors;
			CurrentCount = Capacity;
		}
		public Vector2DCollection(Vector2D[] vectors, int capacity)
		{
			Capacity = capacity;
			CurrentCount = vectors.Length;
			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			Vectors = vectors;
		}
		#endregion

		#region Public methods
		public void Add(Vector2D vector)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				Vector2D[] vectors = new Vector2D[Capacity];

				this.Vectors.CopyTo(vectors, 0);
				this.Vectors = vectors;
				base.Add();
			}
			Vectors[CurrentCount - 1] = vector;
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}

			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				Vectors[i] = Vectors[i + 1];
			}

			base.Remove();
		}
		public void Union(Vector2DCollection vector2DCollection)
		{
			Capacity += vector2DCollection.Capacity;

			Vector2D[] vectors = new Vector2D[Capacity];

			this.Vectors.CopyTo(vectors, 0);
			vector2DCollection.Vectors.CopyTo(vectors, this.Count);
			this.Vectors = vectors;
			CurrentCount += vector2DCollection.Count;
		}
		public void Clone(Vector2DCollection vector2DCollection)
		{
			this.Capacity = vector2DCollection.Capacity;
			this.Vectors = new Vector2D[Capacity];
			vector2DCollection.Vectors.CopyTo(this.Vectors, 0);
			CurrentCount = vector2DCollection.Count;
		}
		#endregion

		#region Indexer
		public Vector2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				return this.Vectors[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				this.Vectors[i] = value;
			}
		}
		#endregion
	}
}


