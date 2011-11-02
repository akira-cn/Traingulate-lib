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
	using System;
	/// <summary>
	/// Summary description for Point2DCollection.
	/// </summary>
	public class Point2DCollection: Collection
	{
		#region Memeber fields
		private Point2D[] Points = null;
		#endregion

		#region Properties

		public Point2D FirstPoint
		{
			get
			{
				return Points[0];
			}
		}

		public Point2D LastPoint
		{
			get
			{
				return Points[Count - 1];
			}
		}

		#endregion

		#region Constructors
		public Point2DCollection()
		{
			Points = new Point2D[defaultCapacity];
			Capacity = defaultCapacity;
		}

		public Point2DCollection(int capacity)
		{
			Points = new Point2D[capacity];
			Capacity = capacity;
		}

		public Point2DCollection(Point2D[] points)
		{
			Capacity = points.Length;
			Points = points;
			CurrentCount = Capacity;
		}

		public Point2DCollection(Point2D[] points, int capacity)
		{
			Capacity = capacity;
			CurrentCount = points.Length;

			if (Capacity < CurrentCount)
			{
				throw (new CollectionCapacityException());
			}

			Points = points;
		}
		#endregion

		#region Public methods
		public void Add(Point2D point)
		{
			try
			{
				base.Add();
			}
			catch (CollectionCapacityException e)
			{
				string emsg = e.Message;
				this.Capacity *= 2;
				Point2D[] points = new Point2D[Capacity];

				this.Points.CopyTo(points, 0);
				this.Points = points;

				base.Add();
			}

			Points[CurrentCount - 1] = point;
		}

		public void Add(Point2D point, int index)
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

				Point2D[] points = new Point2D[Capacity];

				this.Points.CopyTo(points, 0);
				this.Points = points;
				base.Add();
			}
			for (int i = CurrentCount - 1; i > index; i--)
			{
				Points[i] = Points[i - 1];
			}

			Points[index] = point;
		}

		public void DistinctAdd(Point2D point)
		{
			for (int i = 0; i < Count; i++)
			{
				if (point == this[i])
				{
					return;
				}
			}

			Add(point);
		}
		public void Remove(int index)
		{
			if (index < 0 || index >= this.CurrentCount)
			{
				throw (new CollectionIndexException());
			}
			for (int i = index; i < this.CurrentCount - 1; i++)
			{
				Points[i] = Points[i + 1];
			}

			base.Remove();
		}

		public void Union(Point2DCollection point2DCollection)
		{
			Capacity += point2DCollection.Capacity;
		
			Point2D[] points = new Point2D[Capacity];

			this.Points.CopyTo(points, 0);
			point2DCollection.Points.CopyTo(points, this.Count);

			this.Points = points;

			CurrentCount += point2DCollection.Count;
		}

		public void Clone(Point2DCollection point2DCollection)
		{
			this.Capacity = point2DCollection.Capacity;

			this.Points = new Point2D[Capacity];

			point2DCollection.Points.CopyTo(this.Points, 0);

			CurrentCount = point2DCollection.Count;
		}

		public void Extend(int ExtraPoints)
		{
			if (this.Count < 2)
			{
				return;
			}

			while (ExtraPoints > 0)
			{
				int count = this.Count;

				for (int i = 0; i < count - 1; i++)
				{
					Point2D point = Point2D.MidPoint(this[i], this[i + 1]);
					this.Add(point, i + 1);
					ExtraPoints--;

					if (ExtraPoints <= 0)
					{
						break;
					}
				}
			}
		}
		#endregion

		#region Indexer
		public Point2D this[int i]
		{
			get
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}
				return this.Points[i];
			}
			set
			{
				if (i >= this.CurrentCount)
				{
					throw (new CollectionIndexException());
				}
				this.Points[i] = value;
			}
		}
		#endregion
	}
}

