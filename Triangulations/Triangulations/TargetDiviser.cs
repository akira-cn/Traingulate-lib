using System;
using Akira.Geometry;
using Akira.MatrixLib;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// TargetDiviser 的摘要说明。
	/// </summary>
	public class TargetDiviser : Polygon2DDiviser
	{
		public TargetDiviser(Polygon2D poly) : base(poly)
		{

		}

		public void Divide()
		{
			for (int i = 0; i < this.SubDivision.Count; i++)
			{
				Polygon2D poly = this.SubDivision[i];

				if (!poly.isConcave || poly.VertexCount == 3 || !poly.isSimple)
				{
					continue;
				}
				else
				{
					Divide(poly);
					i--;
				}
				UpdateStatus(this, "Dividing...(" + this.SubDivision.Count.ToString() + ")");
			}
		}

		private void Divide(Polygon2D poly)
		{
			for (int i = 0; i < poly.VertexCount; i++)  //Find Convex Angle
			{
				if (poly.isConvexAngle(i))
				{
					continue;
				}
				int index = i;
				double min_dist = 10000;

				for (int j = 0; j < poly.VertexCount; j++) //Divide Polygon
				{
					if (j == i)
					{
						continue;
					}
					LineSegment2D line = new LineSegment2D(poly.GetPoint(i), poly.GetPoint(j));

					if (poly.isDiagonal(line))
					{
						//double dist = line.Length;
						Vector2D v1 = line.ToVector();
						LineSegment2D line1 = poly.GetEdge(i);
						line1.SwapPoints();
						LineSegment2D line2 = poly.GetEdge((i + 1) % poly.VertexCount);
						Vector2D v2 = line1.ToVector();
						Vector2D v3 = line2.ToVector();

						double angle1 = Vector2D.Angle(v2, v1);
						double angle2 = Vector2D.Angle(v1, v3);  //选取划分角度最均匀的点作分割点

						if (angle1 < 0)
						{
							angle1 += Math.PI * 2;
						}
						if (angle2 < 0)
						{
							angle2 += Math.PI * 2;
						}

						double dist = Math.Max(angle1, angle2);
						if (!poly.isConvexAngle(j) && dist < Math.PI)
						{
							dist -= Math.PI;
						}
						
						if(dist < min_dist)
						{
							index = j;
							min_dist = dist;
						}
					}
				}
				if (index != i)
				{
					this.DividedBy(new LineSegment2D(poly.GetPoint(i), poly.GetPoint(index)));
					return;
				}
			}
			throw (new ArgumentException());
		}

		public delegate void ShowStatus(object sender, string msg);
		public event ShowStatus UpdateStatus;
	}
}
