//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using System.IO;
using Akira.Collections;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Diagnostics;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Summary description for WFParser.
	/// </summary>
	public class WFParser
	{
		private string Path = null;
		private Point2D[] points = null;
		private int[] indices = null;
		private double Reverse = 0;
		private const int Margin = 30;
			
		public WFParser(string filePath)
		{
			Path = filePath;
		}
		public Polygon2D Read()
		{
			StreamReader Reader = new StreamReader(Path);
			string msg = null;
			int count = this.GetCount();

			indices = new int[count];
			points = new Point2D[count];
			while ((msg = Reader.ReadLine()) != null)
			{
				if (msg.Length > 0 && msg[0] == '<')
				{
					XWFParser xwfParser = new XWFParser(this.Path);

                    return xwfParser.Read();
				}
				this.Parse(msg);
			}

			Reader.Close();

			return this.BuildPolygon();
		}
		private int GetCount()
		{
			int Count = 0;
			StreamReader Reader = new StreamReader(Path);
			string msg = null;

			while ((msg = Reader.ReadLine()) != null)
			{
				if (msg.Length <= 0 || msg[0] == '#')
				{
					continue;
				}

				string[] s = msg.Split(' ');

				if (s.Length == 4)
				{
					Count++;
				}
			}
			Reader.Close();
			return Count;
		}
		private void Parse(string msg)
		{
			if (msg.Length <= 0 || msg[0] == '#')
			{
				return;
			}
			else
			{
				string[] s = msg.Split(' ');

				if (s.Length == 4)
				{
					double x = double.Parse(s[0]);
					double y = double.Parse(s[1]);
					int index = int.Parse(s[3]);

					this.points[index] = new Point2D(x, y);
					if (y > this.Reverse)
					{
						this.Reverse = y;
					}
				}
				else if (s.Length == 2)
				{
					int a = int.Parse(s[0]);
					int b = int.Parse(s[1]);

					this.indices[a] = b;
				}
			}
		}
		private Polygon2D BuildPolygon()
		{
			Polygon2D polygon = new Polygon2D();
			Polygon2DEditor polygonEditor = new Polygon2DEditor(polygon);
			int j = 0;

			for (int i = 0; i < this.indices.Length; i++)
			{
				this.points[j].Y = this.Reverse - this.points[j].Y + WFParser.Margin;
				polygonEditor.AddPoint(this.points[j]);
				j = this.indices[j];
			}

			return polygon;
		}
		private void SetReverse(Polygon2D polygon)
		{
			for (int i = 0; i < polygon.VertexCount; i++)
			{
				Point2D point = polygon.GetPoint(i);
				if (this.Reverse < point.Y)
				{
					this.Reverse = point.Y;
				}
			}
		}
		public void Write(Polygon2D polygon)
		{
			Stream stream = new FileStream(this.Path, FileMode.Create, FileAccess.Write, FileShare.None);

			string str = "# WFMESH2";
            
			stream.Write(this.GetBytes(str), 0, str.Length);
			stream.WriteByte((byte)'\r');
			stream.WriteByte((byte)'\n');

			str = "# Created by Triangulator";

			stream.Write(this.GetBytes(str), 0, str.Length);
			stream.WriteByte((byte)'\r');
			stream.WriteByte((byte)'\n');
	
			str = "# Vertices";
			
			stream.Write(this.GetBytes(str), 0, str.Length);
			stream.WriteByte((byte)'\r');
			stream.WriteByte((byte)'\n');

			this.SetReverse(polygon);

			for (int i = 0; i < polygon.VertexCount; i++)
			{
				Point2D point = new Point2D(polygon.GetPoint(i));
				point.Y = this.Reverse + WFParser.Margin - point.Y;

				str = point.X.ToString() + " " + point.Y.ToString() + " " + "0.00" + " " + i.ToString();

				stream.Write(this.GetBytes(str), 0, str.Length);
				stream.WriteByte((byte)'\r');
				stream.WriteByte((byte)'\n');
			}

			stream.WriteByte((byte)'\r');
			stream.WriteByte((byte)'\n');

			str = "# Edges";
			stream.Write(this.GetBytes(str), 0, str.Length);
			stream.WriteByte((byte)'\r');
			stream.WriteByte((byte)'\n');

			for (int i = 0; i < polygon.VertexCount - 1; i++)
			{
				str = i.ToString() + " " + (i + 1).ToString();

				stream.Write(this.GetBytes(str), 0, str.Length);
				stream.WriteByte((byte)'\r');
				stream.WriteByte((byte)'\n');
			}

			str = (polygon.VertexCount - 1).ToString() + " 0";

			stream.Write(this.GetBytes(str), 0, str.Length);
			stream.WriteByte((byte)'\r');
			stream.WriteByte((byte)'\n');

			stream.Close();
		}

		private byte[] GetBytes(string str)
		{
			byte[] b = new byte[str.Length];

			for (int i = 0; i < str.Length; i++)
			{
				b[i] = (byte)str[i];
			}

			return b;
		}
	}
}
