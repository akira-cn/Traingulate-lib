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

//Created on 02-06-2004
//Modified on 02-08-2004

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;
	using Akira.MatrixLib.Exceptions;
	/// <summary>
	/// Represents a new point.
	/// </summary>		
	public class Point : PointSet
	{
		#region Static members

		public static readonly Point Zero = new Point(1);

		#endregion

		#region Constructors


		public Point() : base()
		{
			
		}

		public Point(int size) : base(size)
		{

		}

		public Point(double[] d) : base(d)
		{

		}

		public Point(PointSet p) : base(p)
		{

		}

		#endregion

		#region Type Converts

		/// <summary>
		/// Implicitly convert a point to a point2D.
		/// </summary>
		/// <param name="point">the point.</param>
		/// <returns>the point2D.</returns>
		public static implicit operator Point2D(Point point)
		{
			if (point.Dimension == 2)
			{
				return Point2D.ConvertFromPoint(point);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}		

		#endregion
	}
}
