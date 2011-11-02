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
//Modified on 02-09-2004

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;
	using Akira.MatrixLib.Exceptions;
	/// <summary>
	/// Represents a new geometry vector.
	/// </summary>
	public class Vector : VectorSet
	{
		#region Static fields

		public static readonly Vector Zero = new Vector();
		public static readonly Vector Unit = new Vector(new double[] { 1 });
		public static readonly Vector Unit2D = new Vector(new double[] { 1, 1 });
		public static readonly Vector Unit3D = new Vector(new double[] { 1, 1, 1 });
		public static readonly Vector Unit3D_X = new Vector(new double[] { 1, 0, 0 });
		public static readonly Vector Unit3D_Y = new Vector(new double[] { 0, 1, 0 });
		public static readonly Vector Unit3D_Z = new Vector(new double[] { 0, 0, 1 });
        
		#endregion

		#region Constructor

		public Vector() : base()
		{

		}

		public Vector(int size) : base(size)
		{

		}

		public Vector(double[] pond) : base(pond)
		{

		}

		public Vector(VectorSet a) : base(a)
		{

		}

		#endregion

		#region Type converts

		/// <summary>
		/// Implicitly convert a Vector to Vector2D.
		/// </summary>
		/// <param name="vector">the vector.</param>
		/// <returns>the vector2D.</returns>
		public static implicit operator Vector2D(Vector vector)
		{
			if (vector.Dimension == 2)
			{
				return Vector2D.ConvertFromVector(vector);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}

		/// <summary>
		/// Implicitly convert a Vector to Vector3D.
		/// </summary>
		/// <param name="vector">the vector.</param>
		/// <returns>the vector3D.</returns>
		public static implicit operator Vector3D(Vector vector)
		{
			if (vector.Dimension == 3)
			{
				return Vector3D.ConvertFromVector(vector);
			}
			else
			{
				throw (new TypeConvertFailureException());
			}
		}
		#endregion
	}
}
