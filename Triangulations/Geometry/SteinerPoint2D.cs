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
	using Akira.MatrixLib;

	/// <summary>
	/// Summary description for SteinerPoint2D.
	/// </summary>
	public class SteinerPoint2D
	{
		#region Member fields
		private int ID = 0;
		Vector2D[] parent = null;
		#endregion

		#region Constructors
		public SteinerPoint2D(Vector2D a, Vector2D b, int i)
		{
			ID = i;
			parent = new Vector2D[2];
			parent[0] = a;
			parent[1] = b;
		}
		#endregion

		#region Properties
		public int Index
		{
			get
			{
				return ID;
			}
		}

		public Vector2D Father
		{
			get
			{
				return parent[0];
			}
		}

		public Vector2D Mother
		{
			get
			{
				return parent[1];
			}
		}
		#endregion
	}
}
