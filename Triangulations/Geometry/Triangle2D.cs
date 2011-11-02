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
	/// Summary description for Triangle.
	/// </summary>
	public class Triangle2D : Polygon2D
	{
		#region Constructors
		public Triangle2D(Point2D a, Point2D b, Point2D c) : base(new Point2DCollection(new Point2D[3]{a, b, c}))
		{

		}
		#endregion

		#region Properties
		/// <summary>
		/// The first point of the triangle.
		/// </summary>
		/// <value>the point.</value>
		public Point2D A
		{
			get
			{
				return this.Vertices[0];
			}
		}

		/// <summary>
		/// The second point of the triangle.
		/// </summary>
		/// <value>the point.</value>
		public Point2D B
		{
			get
			{
				return this.Vertices[1];
			}
		}

		/// <summary>
		/// The third point of the triangle.
		/// </summary>
		/// <value>the point.</value>
		public Point2D C
		{
			get
			{
				return this.Vertices[2];
			}
		}

		/// <summary>
		/// Angle of A
		/// </summary>
		/// <value>the angle</value>
		public double a
		{
			get
			{
				Vector2D v1 = B - A;
				Vector2D v2 = C - A;

				return Math.Abs(Vector2D.Angle(v1, v2));
			}
		}

		/// <summary>
		/// Angle of B
		/// </summary>
		/// <value>the angle</value>
		public double b
		{
			get
			{
				Vector2D v1 = C - B;
				Vector2D v2 = A - B;

				return Math.Abs(Vector2D.Angle(v1, v2));
			}
		}

		/// <summary>
		/// Angle of C
		/// </summary>
		/// <value>the angle</value>
		public double c
		{
			get
			{
				Vector2D v1 = A - C;
				Vector2D v2 = B - C;

				return Math.Abs(Vector2D.Angle(v1, v2));
			}
		}

		/// <summary>
		/// Edge AB
		/// </summary>
		/// <value>the Edge</value>
		public LineSegment2D AB
		{
			get
			{
				return new LineSegment2D(this.A, this.B);
			}
		}

		/// <summary>
		/// Edge AC
		/// </summary>
		/// <value>the Edge</value>
		public LineSegment2D AC
		{
			get
			{
				return new LineSegment2D(this.A, this.C);
			}
		}

		/// <summary>
		/// Edge BC
		/// </summary>
		/// <value>the Edge</value>
		public LineSegment2D BC
		{
			get
			{
				return new LineSegment2D(this.B, this.C);
			}
		}

		/// <summary>
		/// Whether the triangle is regular.
		/// </summary>
		/// <value>true if is regular.</value>
		new public bool isRegular
		{
			get
			{
				LineSegment2D lineSegment = new LineSegment2D(A,B);
				
				return A != B && B != C && C != A && lineSegment.isOnLine(C);
			}
		}
		#endregion
	}
}
