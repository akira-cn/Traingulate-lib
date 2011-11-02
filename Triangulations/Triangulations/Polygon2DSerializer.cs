//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Text;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Summary description for PolygonSerializer.
	/// </summary>
	public class Polygon2DSerializer
	{
		public readonly Polygon2D FirstPolygon = null;
		public readonly Polygon2D LastPolygon = null;

		public Polygon2DSerializer(Polygon2D first, Polygon2D second)
		{
			this.FirstPolygon = first;
			this.LastPolygon = second;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < this.FirstPolygon.VertexCount; i++)
			{
				Point2D point = this.FirstPolygon.GetPoint(i);
					
				stringBuilder.Append(point.ToString());
				stringBuilder.Append("#");
			}

			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append("|");

			for (int i = 0; i < this.LastPolygon.VertexCount; i++)
			{
				Point2D point = this.LastPolygon.GetPoint(i);

				stringBuilder.Append(point.ToString());
				stringBuilder.Append("#");
			}

			stringBuilder.Remove(stringBuilder.Length - 1, 1);

			return stringBuilder.ToString();
		}

		public void FromString(string polygonString)
		{
			string[] str = polygonString.Split('|');

			string[] s = str[0].Split('#'); 

			for (int j = 0; j < s.Length; j++)
			{
				Polygon2DEditor polygonEditor = new Polygon2DEditor(this.FirstPolygon);

				polygonEditor.AddPoint(ConvertPoint2DFromString(s[j]));
			}

			s = str[1].Split('#');

			for (int j = 0; j < s.Length; j++)
			{
				Polygon2DEditor polygonEditor = new Polygon2DEditor(this.LastPolygon);

				polygonEditor.AddPoint(ConvertPoint2DFromString(s[j]));
			}
		}

		public Point2D ConvertPoint2DFromString(string point)
		{
			Point2D point2D = new Point2D();
			StringBuilder stringBuilder = new StringBuilder(point);

			stringBuilder.Remove(0, 1);
			stringBuilder.Remove(stringBuilder.Length - 1, 1);

			string[] s = stringBuilder.ToString().Split(',');

			point2D.X = double.Parse(s[0]);
			point2D.Y = double.Parse(s[1]);

			return point2D;
		}
	}
}
