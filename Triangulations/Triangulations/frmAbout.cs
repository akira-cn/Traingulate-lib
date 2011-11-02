//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Summary description for frmAbout.
	/// </summary>
	public class frmAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.Label label2;

		private System.Windows.Forms.Label label3;

		private System.Windows.Forms.Label label4;

		private System.Windows.Forms.Label label5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();

			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(56, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(217, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Triangulator Version 1.00 Demo";

			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(56, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(223, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Copy Right (&C) 2004";

			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(56, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(199, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Internet Graphics Group";

			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(56, 103);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(182, 20);
			this.label4.TabIndex = 3;
			this.label4.Text = "Microsoft Research Asia";

			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(56, 126);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(217, 21);
			this.label5.TabIndex = 4;
			this.label5.Text = "vlwu@msrchina.research.microsoft.com";

			// 
			// frmAbout
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(286, 167);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmAbout";
			this.Text = "About Triangulator";
			this.Load += new System.EventHandler(this.frmAbout_Load);
			this.ResumeLayout(false);
		}
#endregion

		private void frmAbout_Load(object sender, System.EventArgs e)
		{
		}
	}
}
