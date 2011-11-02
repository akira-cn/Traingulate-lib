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
	/// Summary description for LineSegment2DCollection.
	/// </summary>
	public class LineSegment2DCollection: Collection
	{
		#region Memeber fields
		private LineSegment2D[] LineSegments = null;
		#endregion

		#region Constructors
		public LineSegment2DCollection()
		{
			LineSegments = new LineSegment2D[defaultCapacity];
			Capacity = defaultCapacity;
		}
		public LineSegment2DCollection(int capacity)
		{
			LineSegments = new LineSegment2D[capacity];
			Capacity = capacity;
		}
		public LineSegment2DCollection(LineSegment2D[] lineSegments)
		{
			Capacity = lineSegments.Length;
			LineSegments = lineSegments;
			CurrentCount = Capacity;
		}
		public LineSegment2DCollection(LineSegment2D[] lineSegments, int capacity)
		{
			Capacity = capacity;
			CurrentCount = lineSegments.Length;
			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			LineSegments = lineSegments;
		}
		#endregion

		#region Public methods
		public void Add(LineSegment2D lineSegment)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;

				this.Capacity *= 2;

				LineSegment2D[] lineSegments = new LineSegment2D[Capacity];

				this.LineSegments.CopyTo(lineSegments, 0);
				this.LineSegments = lineSegments;
				base.Add();
			}
			LineSegments[CurrentCount - 1] = lineSegment;
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}

			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				LineSegments[i] = LineSegments[i + 1];
			}

			base.Remove();
		}
		public void Union(LineSegment2DCollection lineSegment2DCollection)
		{
			Capacity += lineSegment2DCollection.Capacity;

			LineSegment2D[] lineSegments = new LineSegment2D[Capacity];

			this.LineSegments.CopyTo(lineSegments, 0);
			lineSegment2DCollection.LineSegments.CopyTo(lineSegments, this.Count);
			this.LineSegments = lineSegments;
			CurrentCount += lineSegment2DCollection.Count;
		}
		#endregion

		#region Indexer
		public LineSegment2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				return this.LineSegments[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}

				this.LineSegments[i] = value;
			}
		}
		#endregion
	}
}

