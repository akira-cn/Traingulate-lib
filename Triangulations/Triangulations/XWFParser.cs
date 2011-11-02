//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using System.Text;
using System.IO;
using Akira.Collections;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// XWFParser 的摘要说明。
	/// </summary>
	public class XWFParser
	{
		private string Path = null;
		private Point2D[] points = null;
		private int[] indices = null;
		private double Reverse = 0;
		private const int Margin = 30;

		public XWFParser(string FilePath)
		{
			Path = FilePath;
		}

		public void Write(Polygon2D Polygon)
		{
			this.SetReverse(Polygon);

			XmlTextWriter xmlWriter = new XmlTextWriter(Path, Encoding.UTF8);

			xmlWriter.WriteStartDocument(true);
				xmlWriter.WriteStartElement("pn", "Polygon", "urn:Microsoft.VS.Akira.Triangulations");
					xmlWriter.WriteStartElement("pn:Coordinate");
						xmlWriter.WriteStartAttribute("", "type", "");
							xmlWriter.WriteString("Reversed");
						xmlWriter.WriteEndAttribute();
						xmlWriter.WriteStartAttribute("", "top", "");
							xmlWriter.WriteString(this.Reverse.ToString());
						xmlWriter.WriteEndAttribute();
						xmlWriter.WriteStartAttribute("", "margin", "");
							xmlWriter.WriteString(XWFParser.Margin.ToString());
						xmlWriter.WriteEndAttribute();
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("pn:Vertices");
						xmlWriter.WriteStartAttribute("","count","");
						xmlWriter.WriteString(Polygon.VertexCount.ToString());
						xmlWriter.WriteEndAttribute();
						
			for(int i = 0; i < Polygon.VertexCount; i++)
			{
				Point2D point = new Point2D(Polygon.GetPoint(i));
				point.Y = this.Reverse + XWFParser.Margin - point.Y;

				xmlWriter.WriteStartElement("pn:Vertex");
				xmlWriter.WriteStartAttribute("","index","");
				xmlWriter.WriteString(i.ToString());
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteStartAttribute("","x","");
				xmlWriter.WriteString(point.X.ToString());
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteStartAttribute("","y","");
				xmlWriter.WriteString(point.Y.ToString());
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteStartAttribute("","z","");
				xmlWriter.WriteString("0");
				xmlWriter.WriteEndAttribute();

                xmlWriter.WriteEndElement();
			}

					xmlWriter.WriteEndElement();

					xmlWriter.WriteStartElement("pn:Edges");

			for (int i = 0; i < Polygon.VertexCount; i++)
			{
				xmlWriter.WriteStartElement("pn:Edge");
				xmlWriter.WriteStartAttribute("","from","");
				xmlWriter.WriteString(i.ToString());
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteStartAttribute("","to","");
				xmlWriter.WriteString(((i+1) % Polygon.VertexCount).ToString());
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteEndElement();
			}

					xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			
			xmlWriter.Close();
		}

		private Polygon2D BuildPolygon()
		{
			Polygon2D polygon = new Polygon2D();
			Polygon2DEditor polygonEditor = new Polygon2DEditor(polygon);
			int j = 0;

			for (int i = 0; i < this.indices.Length; i++)
			{
				this.points[j].Y = this.Reverse - this.points[j].Y + XWFParser.Margin;
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
		public Polygon2D Read()
		{
			XmlDocument xmlDocument = new XmlDocument();

			xmlDocument.Load(Path);
					
			XmlNamespaceManager xmlNamespace = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespace.AddNamespace("pn", "urn:Microsoft.VS.Akira.Triangulations");

			XmlNode xmlNode = xmlDocument.SelectSingleNode("/pn:Polygon/pn:Vertices", xmlNamespace);
			
		    int count = int.Parse(xmlNode.Attributes[0].Value);

			xmlNode = xmlDocument.SelectSingleNode("/pn:Polygon/pn:Coordinate", xmlNamespace);
			this.Reverse = int.Parse(xmlNode.Attributes[1].Value);
		
			this.points = new Point2D[count];
			this.indices = new int[count];

			for (int i = 0; i < count; i++)
			{
				this.indices[i] = -1;
			}

			XmlNodeList Vertices = xmlDocument.SelectNodes("/pn:Polygon/pn:Vertices/pn:Vertex", xmlNamespace);
			XmlNodeList Edges = xmlDocument.SelectNodes("/pn:Polygon/pn:Edges/pn:Edge", xmlNamespace);

			for (int i = 0; i < count; i++)
			{
				xmlNode = Vertices.Item(i);

				int index = int.Parse(xmlNode.Attributes[0].Value);
				int x = int.Parse(xmlNode.Attributes[1].Value);
				int y = int.Parse(xmlNode.Attributes[2].Value);

				this.points[index] = new Point2D(x, y);

				xmlNode = Edges.Item(i);

				int from = int.Parse(xmlNode.Attributes[0].Value);
				int to = int.Parse(xmlNode.Attributes [1].Value);

				if (this.indices[from] == -1)
				{
					this.indices[from] = to;
				}
				else
				{
					this.indices[to] = from;
				}
			}

			return this.BuildPolygon();
		}
	}
}
