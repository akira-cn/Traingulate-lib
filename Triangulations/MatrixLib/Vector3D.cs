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

//Created on 03-03-2004

namespace Akira.MatrixLib
{
	using System;
	using System.Text;
	using System.Diagnostics;

	/// <summary>
	/// Represents a new geometry vector2D.
	/// </summary>
	public class Vector3D: VectorSet
	{
		#region Static fields

		public static readonly Vector3D Zero = new Vector3D();
		public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);
		public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);
		public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);

		#endregion

		#region Constructors

		public Vector3D() : base(3)
		{
		}
		public Vector3D(double x, double y, double z) : base(new double[] {x, y, z})
		{
		}
		public Vector3D(Vector3D v) : base(v)
		{
		}
		private Vector3D(double[] pond) : base(pond)
		{
		}

		#endregion

		#region Property
		
		public double X
		{
			get
			{
				return Ponderance[0];
			}
			set
			{
				Ponderance[0] = value;
			}
		}
		public double Y
		{
			get
			{
				return Ponderance[1];
			}
			set
			{
				Ponderance[1] = value;
			}
		}
		public double Z
		{
			get
			{
				return Ponderance[2];
			}
			set
			{
				Ponderance[2] = value;
			}
		}
		#endregion

		#region Public methods

		/// <summary>
		/// Sets this vector using the given coordinates.
		/// </summary>
		/// <param name="x">The horizontal coordinate.</param>
		/// <param name="y">The vertical coordinate.</param>
		public void SetFromXYZ(double x, double y, double z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		/// <summary>
		/// Returns a vector perpendicular to this vector. 
		/// </summary>
		/// <returns>A vector perpendicular to this vector.</returns>
		public Vector3D Normal(Vector3D vector)
		{
			if (vector == this)
			{
				throw (new ArgumentException());
			}

			double x = this.Y * vector.Z - this.Z * vector.Y;
			double y = this.Z * vector.X - this.X * vector.Z;
			double z = this.X * vector.Y - this.Y * vector.X;

			return new Vector3D(x,y,z);
		}
		
		/// <summary>
		/// Returns a normalized vector perpendicular to this vector. Equivalent to rotating this vector 90 degrees clockwise.
		/// </summary>
		/// <returns>A normalized vector perpendicular to this vector.</returns>
		public Vector3D UnitNormal(Vector3D vector)
		{
			return this.Normal(vector).Normalize();;
		}
		#endregion

		#region Operator
		
		/// <summary>
		/// Returns the cross product of two vectors. The cross product of the 3 3D vectors a and b is defined
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>The cross product of <paramref name="a"/> and <paramref name="b"/>.</returns>
		public static Vector3D Cross(Vector3D a, Vector3D b)
		{
			return a.Normal(b);
		}

		#endregion

		#region Type converts

		/// <summary>
		/// Implicitly convert a Vector3D to a Vector.
		/// </summary>
		/// <param name="vector2D">the vector2D.</param>
		/// <returns>the vector.</returns>
		public static implicit operator Vector(Vector3D vector3D)
		{
			return new Vector(vector3D.Values);
		}
		/// <summary>
		/// Convert a Vector to a Vector3D.
		/// </summary>
		/// <param name="vector">the vector.</param>
		/// <returns>the vector2D.</returns>
		public static Vector3D ConvertFromVector(Vector vector)
		{
			if (vector.Dimension == 3)
			{
				return new Vector3D(vector.Values);
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}