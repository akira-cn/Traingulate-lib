//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

//Created on 03-30-2004
//Modified on 03-30-2004

using System;
using System.Text;
using System.Diagnostics;
using Akira.MatrixLib;
using Akira.MatrixLib.Exceptions;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Defined the linear system Ax=b
	/// </summary>
	public class LinearSystem
	{
		public static double percent = 0.0;

		#region Members

		private Matrix LMatrix = null;
		private int size = 0;

		#endregion

		#region Properties
		
		public int Size
		{
			get
			{
				return this.size;
			}
		}
		public SquaredMatrix Coefficient
		{
			get
			{
				Vector[] vectors = new Vector[this.LMatrix.Col - 1];

				for (int i = 0; i < this.LMatrix.Col - 1; i++)
				{
					vectors[i] = this.LMatrix.Cols[i];
				}

				Matrix s = new Matrix(vectors);
				return (SquaredMatrix)s;
			}
		}
		public Vector Value
		{
			get
			{
				return this.LMatrix.Cols[this.LMatrix.Col - 1];
			}
		}
		#endregion

		#region Constructors
		
		public LinearSystem(SquaredMatrix coe, Vector v)
		{
			if (coe.Size != v.Dimension)
			{
				throw (new ArgumentException());
			}

			Vector[] vectors = new Vector[coe.Size + 1];

			for (int i = 0; i < coe.Size; i++)
			{
				vectors[i] = coe.Cols[i];
			}
			vectors[coe.Size] = v;

			this.LMatrix = new Matrix(vectors);

			this.size = this.LMatrix.Col - 1;
		}

		#endregion

		public void Gaussian(int VertexCount)
		{
			int size = this.LMatrix.Col - 1;
			
			UpdateStatus(this, "Step1");
			for (int i = VertexCount; i < size; i++)
			{
				for (int j = 0; j < i; j++)
				{
					if (this.LMatrix[i][j] != 0)
					{
						Vector v = this.LMatrix.Rows[i] - this.LMatrix[i][j] * this.LMatrix.Rows[j] / this.LMatrix[j][j];
						
						for (int k = 0; k < size + 1; k++)
						{
							this.LMatrix[i][k] = v[k];
						}
					}
				}
			}

			UpdateStatus(this, "Step2");
			for (int i = VertexCount; i < size; i++)
			{
				for (int j = i + 1; j <= size; j++)
				{
					this.LMatrix[i][j] /= this.LMatrix[i][i];
				}
				this.LMatrix[i][i] = 1;
			}
			
			UpdateStatus(this, "Step3");
			double cost = (size - VertexCount) * (size - VertexCount - 1);
			if(percent >= 100)
			{
				percent = 0;
			}
            
			for (int i = VertexCount; i < size; i++)
			{
				for (int j = i + 1; j < size; j++)
				{
					if (this.LMatrix[i][j] != 0)
					{
						for (int k = j + 1; k < size + 1; k++)
						{
							this.LMatrix[i][k] -= this.LMatrix[j][k] * this.LMatrix[i][j];
						}
					}
					this.LMatrix[i][j] = 0;
					percent += (100.0 / cost);
					UpdateStatus(this, ((int) percent).ToString() + "%");
				}
			}
		}

		public delegate void ShowStatus(object sender, string msg);
		public event ShowStatus UpdateStatus;
	}
}
