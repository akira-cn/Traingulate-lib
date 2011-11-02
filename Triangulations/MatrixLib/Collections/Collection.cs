namespace Akira.Collections
{
	using System;

	/// <summary>
	/// Summary description for ICollection.
	/// </summary>
	public abstract class Collection
	{
		#region Member fields
		protected const int defaultCapacity = 8;
		protected int CurrentCount = 0;
		protected int Capacity = 0;
		#endregion

		#region Properties

		public virtual int Count
		{
			get
			{
				return CurrentCount;
			}
		}

		public virtual int Size
		{
			get
			{
				return Capacity;
			}
		}

		public bool isEmpty
		{
			get
			{
				return CurrentCount == 0;
			}
		}

		public bool isFull
		{
			get
			{
				return CurrentCount == Capacity;
			}
		}

		#endregion

		#region Family methods
		protected virtual void Add()
		{
			if (!isFull)
			{
				CurrentCount++;
			}
			else
			{
				throw (new CollectionCapacityException());
			}
		}

		protected virtual void Remove()
		{
			if (!isEmpty)
			{
				CurrentCount--;
			}
			else
			{
				throw (new CollectionIndexException());
			}
		}

		public virtual void Clear()
		{
			CurrentCount = 0;
		}

		protected virtual void Union(Collection c)
		{
			this.Capacity += c.Capacity;
			this.CurrentCount += c.CurrentCount;
		}
		#endregion
	}


	public class CollectionCapacityException: ApplicationException
	{
		public CollectionCapacityException(): base("The Capacity is not available or the Collection is full.")
		{
		}
	}
	public class CollectionIndexException: ApplicationException
	{
		public CollectionIndexException(): base("The index is not available or the Collection is empty.")
		{
		}
	}
}
