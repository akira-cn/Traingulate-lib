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
	/// Summary description for ClassicPolygon.
	/// </summary>
	public class ClassicalPolygon2D : Polygon2D
	{
		#region Constructors
		public ClassicalPolygon2D(int n)
		{
			if (n <= 2)
			{
				throw (new ArgumentException());
			}

			this.Vertices = new Point2DCollection(n);

			for (int i = 0; i < n; i++)
			{
				this.Vertices.Add(new Point2D(Math.Cos(i * 2 * Math.PI / n), Math.Sin(i * 2 * Math.PI / n)));
			}

			this.GetDirection();
			this.bSimple = true;
			this.bClosed = true;
			this.bConvex = true;
		}
		#endregion
	}
}
