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
	/// Summary description for Polygon2DEditor.
	/// </summary>
	public class Polygon2DEditor
	{
		#region Member fieldes

		private Polygon2D Polygon = null;

		#endregion

		#region Constructors
		public Polygon2DEditor(Polygon2D polygon)
		{
			Polygon = polygon;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Add new point to the polygon.
		/// </summary>
		/// <param name="point">new point.</param>
		public void AddPoint(Point2D point)
		{
			Polygon.Add(point);
		}

		/// <summary>
		/// Add new point to the polygon.
		/// </summary>
		/// <param name="point">new point.</param>
		/// <param name="i">point index.</param>
		public void AddPoint(Point2D point, int i)
		{
			Polygon.Add(point, i);
		}

		/// <summary>
		/// Remove all inner points from the polygon.
		/// </summary>
		public void Clear()
		{
			Polygon.Clear();
		}

		/// <summary>
		/// Remove a point from the polygon.
		/// </summary>
		/// <param name="i">the index.</param>
		public void RemovePoint(int i)
		{
			try
			{
				Polygon.RemovePoint(i);
			}
			catch (Collections.CollectionIndexException)
			{

			}
		}

		/// <summary>
		/// Reset a point of the polygon.
		/// </summary>
		/// <param name="point">the new point.</param>
		/// <param name="i">the point index.</param>
		public void SetPoint(Point2D point, int i)
		{
			Polygon.SetPoint(point, i);
		}

		/// <summary>
		/// Reset a point of the polygon.
		/// </summary>
		/// <param name="x">the first coordinate.</param>
		/// <param name="y">the second coordinate.</param>
		/// <param name="i">the index.</param>
		public void SetPoint(double x, double y, int i)
		{
			Polygon.SetPoint(x, y, i);
		}

		/// <summary>
		/// Invert the polygon direction.
		/// </summary>
		public void Invert()
		{
			Polygon.Inverse();
		}

		/// <summary>
		/// Scroll the polygon.
		/// </summary>
		public void Scroll()
		{
			Polygon.Scroll();
		}

		/// <summary>
		/// Add an inner point to the polygon.
		/// </summary>
		/// <param name="point">the new inner point.</param>
		public void AddInnerPoint(Point2D point)
		{
			Polygon.AddInner(point);
		}
		#endregion
	}
}
