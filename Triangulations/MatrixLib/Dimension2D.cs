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

//Created on 02-09-2004
//Modified on 02-09-2004

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;
	/// <summary>
	/// Summary description for Dimension.
	/// </summary>
	public class Dimension2D : Vector2D
	{
		#region Constructors

		public Dimension2D(int x, int y) : base(x, y)
		{
			
		}

		#endregion

		#region properties

		/// <summary>
		/// The rows of a matrix etc.
		/// </summary>
		/// <value>integer x.</value>
		new public int X
		{
			get
			{
				return (int)this.Values[0];
			}
		}

		/// <summary>
		/// The cols of a matrix etc.
		/// </summary>
		/// <value>integer y.</value>
		new public int Y
		{
			get
			{
				return (int)this.Values[1];
			}
		}

		#endregion

		#region Override Methods

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>	
		public override string ToString()
		{
			return this.X.ToString() + " X " + this.Y.ToString();
		}

		#endregion
	}
}
