//Microsoft Research Asia (2004)
//Copyright (C) 2004 Microsoft Corporation. All Rights Reserved.
//Triangulator Demo
//Written by v-lwu@research.msrchina.microsoft.com

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Akira.Geometry;
using Akira.MatrixLib;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.VS.Akira.Triangulations
{
	/// <summary>
	/// Form1 的摘要说明。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		#region UIController
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem newMenuItem;
		private System.Windows.Forms.MenuItem saveMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem saveMenuItem1;
		private System.Windows.Forms.MenuItem saveasMenuItem;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem exitMenuItem;
		private System.Windows.Forms.MenuItem controlMenuItem;
		private System.Windows.Forms.MenuItem clearMenuItem;
		private System.Windows.Forms.MenuItem scrollMenuItem;
		private System.Windows.Forms.MenuItem leftMenuItem1;
		private System.Windows.Forms.MenuItem rightMenuItem;
		private System.Windows.Forms.MenuItem triangulationMenuItem;
		private System.Windows.Forms.MenuItem optiMenuItem;
		private System.Windows.Forms.MenuItem areaBasedRemeshingMenuItem;
		private System.Windows.Forms.MenuItem flippingMenuItem1;
		private System.Windows.Forms.MenuItem subDivisionMenuItem;
		private System.Windows.Forms.MenuItem splitingMenuItem1;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem optimizationMenuItem;
		private System.Windows.Forms.MenuItem viewMenuItem;
		private System.Windows.Forms.MenuItem singleViewMenuItem;
		private System.Windows.Forms.MenuItem competibleTriangulationMenuItem;
		private System.Windows.Forms.MenuItem highQualityTriangulationMenuItem;
		private System.Windows.Forms.MenuItem showIndicesMenuItem;
		private System.Windows.Forms.MenuItem settingsMenuItem;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem helpMenuItem;
		private System.Windows.Forms.MenuItem aboutMenuItem;
		private System.Windows.Forms.MenuItem testMenuItem;
		private System.Windows.Forms.MenuItem linkMenuItem;
		private System.Windows.Forms.MenuItem triangulationMenuItem1;
		private System.Windows.Forms.MenuItem flipMenuItem;
		private System.Windows.Forms.MenuItem divideMenuItem;
		private System.Windows.Forms.MenuItem splitMenuItem;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.ComponentModel.IContainer components;
		#endregion

		#region Member fields
		private enum Partition { Left,Right };
		private enum TriangulationTech { Single, Simple, HighQuelity };
		private string AppName;
		private string FileName1;
		private string FileName2;

		private Polygon2DAdorner LeftPolygon = null;
		private Polygon2DAdorner RightPolygon = null;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Timers.Timer MoveTimer;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		private Image currentImage = null;
		#endregion

		#region Status Controller
		private struct StatusController
		{
			//public static bool isSingleView = false;
			public static TriangulationTech Triangulator = TriangulationTech.Simple;
			public static int CatchPoint = -1;
			public static bool canAddPoint = true;
			public static bool isZoomView
			{
				get
				{
					return LeftZoom.ratio != 1.0 || RightZoom.ratio != 1.0;
				}
			}
			public static ZoomControl LeftZoom = new ZoomControl();
			public static ZoomControl RightZoom = new ZoomControl();
			public static Partition availablePartition = Partition.Left;
			public static bool leftCanRestart = true;
			public static bool rightCanRestart = true;

			public static bool canRestart
			{
				get
				{
					if (availablePartition == Partition.Left)
					{
						return leftCanRestart;
					}
					else
					{
						return rightCanRestart;
					}
				}
				set
				{
					if (availablePartition == Partition.Left)
					{
						leftCanRestart = value;
					}
					else
					{
						rightCanRestart = value;
					}
				}
			}
		}

		private class ZoomControl
		{
			public ZoomControl()
			{
				ratio = 0.5;
				Offset = new PointF(0.0f, -30.0f);				
			}

			public double ratio;
			public PointF Offset;
		}
		#endregion

		#region Properties
		private Polygon2D FirstPolygon
		{
			get
			{
				return this.LeftPolygon.polygon;
			}
		}

		private Polygon2D SecondPolygon
		{
			get
			{
				return this.RightPolygon.polygon;
			}
		}
		private string FileName
		{
			get
			{
				if (StatusController.availablePartition == Partition.Left)
				{
					return FileName1;
				}
				else
				{
					return FileName2;
				}
			}
			set
			{
				if (StatusController.availablePartition == Partition.Left)
				{
					FileName1 = value;
				}
				else
				{
					FileName2 = value;
				}
			}
		}
		#endregion

		#region Constructors
		public Form1(string fileName1, string fileName2)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			this.FileName1 = fileName1;
			this.FileName2 = fileName2;

			this.AppName = "Triangulator Demo";
		}
		public Form1()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			this.FileName1 = null;
			this.FileName2 = null;

			this.AppName = "Triangulator Demo";
		}
		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.newMenuItem = new System.Windows.Forms.MenuItem();
			this.saveMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.saveMenuItem1 = new System.Windows.Forms.MenuItem();
			this.saveasMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.exitMenuItem = new System.Windows.Forms.MenuItem();
			this.controlMenuItem = new System.Windows.Forms.MenuItem();
			this.clearMenuItem = new System.Windows.Forms.MenuItem();
			this.scrollMenuItem = new System.Windows.Forms.MenuItem();
			this.leftMenuItem1 = new System.Windows.Forms.MenuItem();
			this.rightMenuItem = new System.Windows.Forms.MenuItem();
			this.triangulationMenuItem = new System.Windows.Forms.MenuItem();
			this.optiMenuItem = new System.Windows.Forms.MenuItem();
			this.areaBasedRemeshingMenuItem = new System.Windows.Forms.MenuItem();
			this.flippingMenuItem1 = new System.Windows.Forms.MenuItem();
			this.subDivisionMenuItem = new System.Windows.Forms.MenuItem();
			this.splitingMenuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.optimizationMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.viewMenuItem = new System.Windows.Forms.MenuItem();
			this.singleViewMenuItem = new System.Windows.Forms.MenuItem();
			this.competibleTriangulationMenuItem = new System.Windows.Forms.MenuItem();
			this.highQualityTriangulationMenuItem = new System.Windows.Forms.MenuItem();
			this.showIndicesMenuItem = new System.Windows.Forms.MenuItem();
			this.settingsMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.helpMenuItem = new System.Windows.Forms.MenuItem();
			this.aboutMenuItem = new System.Windows.Forms.MenuItem();
			this.testMenuItem = new System.Windows.Forms.MenuItem();
			this.linkMenuItem = new System.Windows.Forms.MenuItem();
			this.triangulationMenuItem1 = new System.Windows.Forms.MenuItem();
			this.flipMenuItem = new System.Windows.Forms.MenuItem();
			this.divideMenuItem = new System.Windows.Forms.MenuItem();
			this.splitMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem16 = new System.Windows.Forms.MenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton7 = new System.Windows.Forms.ToolBarButton();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
			this.MoveTimer = new System.Timers.Timer();
			this.menuItem17 = new System.Windows.Forms.MenuItem();
			this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MoveTimer)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.fileMenuItem,
																					  this.controlMenuItem,
																					  this.optiMenuItem,
																					  this.viewMenuItem,
																					  this.settingsMenuItem,
																					  this.helpMenuItem,
																					  this.testMenuItem});
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.Index = 0;
			this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.newMenuItem,
																						 this.saveMenuItem,
																						 this.menuItem1,
																						 this.saveMenuItem1,
																						 this.saveasMenuItem,
																						 this.menuItem17,
																						 this.menuItem2,
																						 this.exitMenuItem});
			this.fileMenuItem.Text = "&File";
			// 
			// newMenuItem
			// 
			this.newMenuItem.Index = 0;
			this.newMenuItem.Text = "&New";
			this.newMenuItem.Click += new System.EventHandler(this.newMenuItem_Click);
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Index = 1;
			this.saveMenuItem.Text = "&Open";
			this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// saveMenuItem1
			// 
			this.saveMenuItem1.Index = 3;
			this.saveMenuItem1.Text = "&Save";
			this.saveMenuItem1.Click += new System.EventHandler(this.saveMenuItem1_Click);
			// 
			// saveasMenuItem
			// 
			this.saveasMenuItem.Index = 4;
			this.saveasMenuItem.Text = "S&ave as";
			this.saveasMenuItem.Click += new System.EventHandler(this.saveasMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 6;
			this.menuItem2.Text = "-";
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Index = 7;
			this.exitMenuItem.Text = "&Exit";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// controlMenuItem
			// 
			this.controlMenuItem.Index = 1;
			this.controlMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.clearMenuItem,
																							this.scrollMenuItem,
																							this.triangulationMenuItem});
			this.controlMenuItem.Text = "&Control";
			// 
			// clearMenuItem
			// 
			this.clearMenuItem.Index = 0;
			this.clearMenuItem.Text = "&Clear";
			this.clearMenuItem.Click += new System.EventHandler(this.clearMenuItem_Click);
			// 
			// scrollMenuItem
			// 
			this.scrollMenuItem.Index = 1;
			this.scrollMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.leftMenuItem1,
																						   this.rightMenuItem});
			this.scrollMenuItem.Text = "&Scroll";
			// 
			// leftMenuItem1
			// 
			this.leftMenuItem1.Index = 0;
			this.leftMenuItem1.Text = "&Left";
			this.leftMenuItem1.Click += new System.EventHandler(this.leftMenuItem1_Click);
			// 
			// rightMenuItem
			// 
			this.rightMenuItem.Index = 1;
			this.rightMenuItem.Text = "&Right";
			this.rightMenuItem.Click += new System.EventHandler(this.rightMenuItem_Click);
			// 
			// triangulationMenuItem
			// 
			this.triangulationMenuItem.Index = 2;
			this.triangulationMenuItem.Text = "&Triangulation";
			this.triangulationMenuItem.Click += new System.EventHandler(this.triangulationMenuItem_Click);
			// 
			// optiMenuItem
			// 
			this.optiMenuItem.Index = 2;
			this.optiMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.areaBasedRemeshingMenuItem,
																						 this.flippingMenuItem1,
																						 this.subDivisionMenuItem,
																						 this.splitingMenuItem1,
																						 this.menuItem11,
																						 this.optimizationMenuItem,
																						 this.menuItem15});
			this.optiMenuItem.Text = "&Option";
			// 
			// areaBasedRemeshingMenuItem
			// 
			this.areaBasedRemeshingMenuItem.Index = 0;
			this.areaBasedRemeshingMenuItem.Text = "&Area Based Remeshing";
			this.areaBasedRemeshingMenuItem.Click += new System.EventHandler(this.areaBasedRemeshingMenuItem_Click);
			// 
			// flippingMenuItem1
			// 
			this.flippingMenuItem1.Index = 1;
			this.flippingMenuItem1.Text = "&Flipping";
			this.flippingMenuItem1.Click += new System.EventHandler(this.flippingMenuItem1_Click);
			// 
			// subDivisionMenuItem
			// 
			this.subDivisionMenuItem.Index = 2;
			this.subDivisionMenuItem.Text = "&SubDivision";
			this.subDivisionMenuItem.Click += new System.EventHandler(this.subDivisionMenuItem_Click);
			// 
			// splitingMenuItem1
			// 
			this.splitingMenuItem1.Index = 3;
			this.splitingMenuItem1.Text = "S&pliting";
			this.splitingMenuItem1.Click += new System.EventHandler(this.splitingMenuItem1_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 4;
			this.menuItem11.Text = "-";
			// 
			// optimizationMenuItem
			// 
			this.optimizationMenuItem.Index = 5;
			this.optimizationMenuItem.Text = "&Optimize >";
			this.optimizationMenuItem.Click += new System.EventHandler(this.optimizationMenuItem_Click);
			// 
			// menuItem15
			// 
			this.menuItem15.Index = 6;
			this.menuItem15.Text = "Surface Fitting >";
			this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
			// 
			// viewMenuItem
			// 
			this.viewMenuItem.Index = 3;
			this.viewMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.singleViewMenuItem,
																						 this.competibleTriangulationMenuItem,
																						 this.highQualityTriangulationMenuItem,
																						 this.showIndicesMenuItem});
			this.viewMenuItem.Text = "&Setting";
			// 
			// singleViewMenuItem
			// 
			this.singleViewMenuItem.Index = 0;
			this.singleViewMenuItem.Text = "&Single Triangulation";
			this.singleViewMenuItem.Click += new System.EventHandler(this.singleViewMenuItem_Click);
			// 
			// competibleTriangulationMenuItem
			// 
			this.competibleTriangulationMenuItem.Index = 1;
			this.competibleTriangulationMenuItem.Text = "&Competible Triangulation";
			this.competibleTriangulationMenuItem.Click += new System.EventHandler(this.competibleTriangulationMenuItem_Click);
			// 
			// highQualityTriangulationMenuItem
			// 
			this.highQualityTriangulationMenuItem.Index = 2;
			this.highQualityTriangulationMenuItem.Text = "&High Quality Triangulation";
			this.highQualityTriangulationMenuItem.Click += new System.EventHandler(this.highQualityTriangulationMenuItem_Click);
			// 
			// showIndicesMenuItem
			// 
			this.showIndicesMenuItem.Index = 3;
			this.showIndicesMenuItem.Text = "Show &Indices";
			this.showIndicesMenuItem.Click += new System.EventHandler(this.showIndicesMenuItem_Click);
			// 
			// settingsMenuItem
			// 
			this.settingsMenuItem.Index = 4;
			this.settingsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.menuItem3,
																							 this.menuItem4,
																							 this.menuItem5,
																							 this.menuItem6,
																							 this.menuItem7,
																							 this.menuItem8,
																							 this.menuItem9,
																							 this.menuItem10});
			this.settingsMenuItem.Text = "&Zoom";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "200%";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "400%";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "600%";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.Text = "800%";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 4;
			this.menuItem7.Text = "-";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 5;
			this.menuItem8.Text = "100%";
			this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 6;
			this.menuItem9.Text = "50%";
			this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 7;
			this.menuItem10.Text = "25%";
			this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
			// 
			// helpMenuItem
			// 
			this.helpMenuItem.Index = 5;
			this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.aboutMenuItem});
			this.helpMenuItem.Text = "&Help";
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Index = 0;
			this.aboutMenuItem.Text = "&About Triangulator";
			this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
			// 
			// testMenuItem
			// 
			this.testMenuItem.Index = 6;
			this.testMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.linkMenuItem,
																						 this.triangulationMenuItem1,
																						 this.flipMenuItem,
																						 this.divideMenuItem,
																						 this.splitMenuItem,
																						 this.menuItem12,
																						 this.menuItem13,
																						 this.menuItem14,
																						 this.menuItem16});
			this.testMenuItem.Text = "Test";
			// 
			// linkMenuItem
			// 
			this.linkMenuItem.Index = 0;
			this.linkMenuItem.Text = "Link";
			this.linkMenuItem.Click += new System.EventHandler(this.linkMenuItem_Click_1);
			// 
			// triangulationMenuItem1
			// 
			this.triangulationMenuItem1.Index = 1;
			this.triangulationMenuItem1.Text = "Triangulation";
			this.triangulationMenuItem1.Click += new System.EventHandler(this.triangulationMenuItem1_Click_1);
			// 
			// flipMenuItem
			// 
			this.flipMenuItem.Index = 2;
			this.flipMenuItem.Text = "Flip";
			this.flipMenuItem.Click += new System.EventHandler(this.flipMenuItem_Click_1);
			// 
			// divideMenuItem
			// 
			this.divideMenuItem.Index = 3;
			this.divideMenuItem.Text = "Divide";
			this.divideMenuItem.Click += new System.EventHandler(this.divideMenuItem_Click_1);
			// 
			// splitMenuItem
			// 
			this.splitMenuItem.Index = 4;
			this.splitMenuItem.Text = "Split";
			this.splitMenuItem.Click += new System.EventHandler(this.splitMenuItem_Click_1);
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 5;
			this.menuItem12.Text = "Divide Target";
			this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 6;
			this.menuItem13.Text = "Save XWF";
			this.menuItem13.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 7;
			this.menuItem14.Text = "Load XWF";
			this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
			// 
			// menuItem16
			// 
			this.menuItem16.Index = 8;
			this.menuItem16.Text = "Sub Triangulation";
			this.menuItem16.Click += new System.EventHandler(this.menuItem16_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.toolBarButton1,
																						this.toolBarButton2,
																						this.toolBarButton3,
																						this.toolBarButton4,
																						this.toolBarButton5,
																						this.toolBarButton6,
																						this.toolBarButton7});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(712, 28);
			this.toolBar1.TabIndex = 1;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick_1);
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.ImageIndex = 0;
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.ImageIndex = 1;
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.ImageIndex = 2;
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.ImageIndex = 3;
			this.toolBarButton4.Pushed = true;
			this.toolBarButton4.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			// 
			// toolBarButton5
			// 
			this.toolBarButton5.ImageIndex = 7;
			// 
			// toolBarButton6
			// 
			this.toolBarButton6.ImageIndex = 5;
			// 
			// toolBarButton7
			// 
			this.toolBarButton7.ImageIndex = 6;
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileName = "NewPolygon";
			this.saveFileDialog1.Filter = "WF Files|*.wf|XWF Files|*.xwf|All Files|*.*";
			this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk_1);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "WF Files|*.wf|XWF Files|*.xwf|All Files|*.*";
			this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk_1);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 424);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.statusBarPanel1,
																						  this.statusBarPanel3,
																						  this.statusBarPanel2});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(712, 24);
			this.statusBar1.TabIndex = 2;
			this.statusBar1.Text = "statusBar1";
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanel1.Width = 400;
			// 
			// statusBarPanel2
			// 
			this.statusBarPanel2.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
			this.statusBarPanel2.Width = 196;
			// 
			// MoveTimer
			// 
			this.MoveTimer.Enabled = true;
			this.MoveTimer.SynchronizingObject = this;
			this.MoveTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.MoveTimer_Elapsed);
			// 
			// menuItem17
			// 
			this.menuItem17.Index = 5;
			this.menuItem17.Text = "Save &Image";
			this.menuItem17.Click += new System.EventHandler(this.menuItem17_Click);
			// 
			// saveFileDialog2
			// 
			this.saveFileDialog2.Filter = "JPEG File(*.jpg)|*.jpg";
			this.saveFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog2_FileOk);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.BackColor = System.Drawing.SystemColors.Window;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(712, 448);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.toolBar1);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MoveTimer)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			string fileName1 = null;
			string fileName2 = null;

			if (args.Length > 0)
			{
				fileName1 = args[0];
			}
			if (args.Length > 1)
			{
				fileName2 = args[1];
			}

			Application.Run(new Form1(fileName1, fileName2));
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			this.Initialize();	

			if (this.FileName1 != null)
			{
				this.Open(this.FileName1);
			}
			
			if (this.FileName2 != null)
			{
				this.Open(this.FileName2);
			}
		}

		private void Initialize()
		{
			this.MoveTimer.Stop();
			this.splitingMenuItem1.Checked = true;
			this.subDivisionMenuItem.Checked = true;
			this.flippingMenuItem1.Checked = true;
			this.areaBasedRemeshingMenuItem.Checked = true;

			this.competibleTriangulationMenuItem.Checked = true;
			StatusController.Triangulator = TriangulationTech.Simple;

			Polygon2D firstPolygon = new Polygon2D(new Point2DCollection());
			LeftPolygon = new Polygon2DAdorner(firstPolygon);
			Polygon2D secondPolygon = new Polygon2D(new Point2DCollection());
			RightPolygon = new Polygon2DAdorner(secondPolygon);

			this.triangulationMenuItem.Enabled = false;
			StatusController.canAddPoint = true;
			this.toolBarButton4.Enabled = true;
			this.toolBarButton4.Pushed = true;
			this.toolBar1.Buttons[5].Enabled = false;
			StatusController.LeftZoom.ratio = 1.0;
			StatusController.LeftZoom.Offset = new PointF(0.0F, -30.0F);
			StatusController.RightZoom.ratio = 1.0;
			StatusController.RightZoom.Offset = new PointF(0.0F, -30.0F);
			this.statusBarPanel1.Text = this.GetPolygonStatus(new Point2D(1, 1));
			this.statusBarPanel2.Text = "Hint Index : -1 ";
			this.Invalidate();
		}

		#region Paint Events
		protected override void OnPaint(PaintEventArgs e)
		{
			if (testOnly)
			{
				testPaint(e);
				return;
			}

			if (polygonLinkAdorner != null)
			{
				e.Graphics.Clear(Color.White);
				polygonLinkAdorner.show(e.Graphics);
				return;
			}
			//test GDI+
			Graphics dc;
			Bitmap screenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
			Graphics screenGraphics = Graphics.FromImage(screenBitmap);

			//TODO: Drow Curves or Polygons
			if (FileName != null)
				this.Text = AppName + " - " + FileName;
			else
				this.Text = AppName;

			//Draw Right
			int Width = ClientSize.Width / 2 + 1;
			int Height = ClientSize.Height;

			Bitmap OffscreenBitmap = new Bitmap(Width, Height);

			dc = Graphics.FromImage(OffscreenBitmap);
			dc.Clear(System.Drawing.Color.White);

			for (int i = 0; i < Width; i += this.BackgroundImage.Width)
			{
				for (int j = 0; j < Height; j += this.BackgroundImage.Height)
				{
					dc.DrawImage(this.BackgroundImage, i, j);
				}
			}
			
			PointF point = new PointF();
			PointF midPoint = new PointF(Width / 2, Height / 2);
			double ratio = StatusController.RightZoom.ratio;
               
			point.X = StatusController.RightZoom.Offset.X + midPoint.X * (float)((ratio - 1) / ratio);
			point.Y = StatusController.RightZoom.Offset.Y + midPoint.Y * (float)((ratio - 1) / ratio);

			this.RightPolygon.Show(dc, point, ratio);

			if (StatusController.availablePartition == Partition.Right)
			{
				Pen bluePen = new Pen(Color.DarkBlue, 2);
				dc.DrawRectangle(bluePen, 1, 29, ClientSize.Width/2 - 4, ClientSize.Height - 55);
			}

			screenGraphics.DrawImage(OffscreenBitmap, ClientSize.Width / 2 + 2, 1, ClientSize.Width / 2, ClientSize.Height);

			//DrawLeft
			Width = ClientSize.Width / 2 + 1;
			Height = ClientSize.Height;

			OffscreenBitmap = new Bitmap(Width, Height);

			dc = Graphics.FromImage(OffscreenBitmap);
			dc.Clear(System.Drawing.Color.White);

			for (int i = 0; i < Width; i += this.BackgroundImage.Width)
			{
				for (int j = 0; j < Height; j += this.BackgroundImage.Height)
				{
					dc.DrawImage(this.BackgroundImage, i, j);
				}
			}

			Pen blackPen = new Pen(Color.Black, 3);
			PointF p1 = new PointF(Width - 2, 0);
			PointF p2 = new PointF(Width - 2, Height);

			dc.DrawLine(blackPen, p1, p2);

			point = new PointF();
			midPoint = new PointF(Width / 2, Height / 2);
			ratio = StatusController.LeftZoom.ratio;
			point.X = StatusController.LeftZoom.Offset.X + midPoint.X * (float)((ratio - 1) / ratio);
			point.Y = StatusController.LeftZoom.Offset.Y + midPoint.Y * (float)((ratio - 1) / ratio);

			this.LeftPolygon.Show(dc, point, ratio);

			if (StatusController.availablePartition == Partition.Left)
			{
				Pen bluePen = new Pen(Color.DarkBlue, 2);

				dc.DrawRectangle(bluePen, 1, 29, ClientSize.Width / 2 - 5, ClientSize.Height - 55);
			}

			screenGraphics.DrawImage(OffscreenBitmap, 1, 1, ClientSize.Width / 2, ClientSize.Height);
			e.Graphics.DrawImage(screenBitmap, 1, 1, ClientSize.Width, ClientSize.Height);

			if (this.CanTriangulate())
			{
				this.triangulationMenuItem.Enabled = true;
				this.toolBar1.Buttons[5].Enabled = true;
			}
			else
			{
				this.triangulationMenuItem.Enabled = false;
				this.toolBar1.Buttons[5].Enabled = false;
			}
			this.currentImage = screenBitmap;
		}


		// override it to avoid the 'image-flash' problem, use double buffer
		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		protected override void OnResize(EventArgs e)
		{
			this.Invalidate();
			base.OnResize (e);
		}

		#endregion

		#region Mouse Events

		protected override void OnMouseMove(MouseEventArgs e)
		{
			Point2D point = new Point2D(e.X, e.Y - 30);
			Polygon2DAdorner polygonAdorner = null;
			Partition part = OnPart(point);

			this.statusBarPanel1.Text = this.GetPolygonStatus(point);
			if (part == Partition.Left)
			{
				polygonAdorner = this.LeftPolygon;
			}
			else
			{
				point.X -= this.ClientSize.Width / 2;
				polygonAdorner = this.RightPolygon;
			}

			int i = StatusController.CatchPoint;

			if (i != -1)
			{
				this.Cursor = System.Windows.Forms.Cursors.Default;

				if (StatusController.availablePartition == part)
				{
					Polygon2DEditor pe = new Polygon2DEditor(polygonAdorner.polygon);

					pe.SetPoint(point, i);

					StatusController.canRestart = false;

				}

				this.Invalidate();
			}
			else
			{
				i = polygonAdorner.HitTest(point);
				if (i == -1)
				{
					this.Cursor = System.Windows.Forms.Cursors.Default;
				}
				else
				{
					this.Cursor = System.Windows.Forms.Cursors.Cross;
				}
				this.statusBarPanel2.Text =point.ToString() + "    " + "Hint Index : " + i.ToString() + " ";
			}

			if (StatusController.isZoomView)
			{
				PointF midPoint;
				if (part == Partition.Left)
				{
					midPoint = new PointF(this.ClientSize.Width / 4, this.ClientSize.Height / 2);

				}
				else
				{
					midPoint = new PointF(3 * this.ClientSize.Width / 4, this.ClientSize.Height / 2);
				}

				if (e.X < midPoint.X && e.Y  < midPoint.Y)
				{
					this.Cursor = System.Windows.Forms.Cursors.PanNW;
				}
				else if (e.X > midPoint.X && e.Y  < midPoint.Y)
				{
					this.Cursor = System.Windows.Forms.Cursors.PanNE;
				}
				else if (e.X < midPoint.X && e.Y > midPoint.Y)
				{
					this.Cursor = System.Windows.Forms.Cursors.PanSW;
				}
				else
				{
					this.Cursor = System.Windows.Forms.Cursors.PanSE;
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Point2D point = new Point2D(e.X, e.Y - 30);

			if (StatusController.availablePartition != OnPart(point))
			{
				StatusController.availablePartition = OnPart(point);
				this.Invalidate();
				return;
			}

			if (StatusController.isZoomView)
			{
				if (e.Button == MouseButtons.Left)
				{
					this.MoveTimer.Start();
					if (OnPart(point) == Partition.Left)
					{

						PointF midPoint = new PointF(this.ClientSize.Width / 4, this.ClientSize.Height / 2);

						if (e.X < midPoint.X)
						{
							StatusController.LeftZoom.Offset.X += (float)StatusController.LeftZoom.ratio;
						}
						else
						{
							StatusController.LeftZoom.Offset.X -= (float)StatusController.LeftZoom.ratio;
						}
						if (e.Y < midPoint.Y)
						{
							StatusController.LeftZoom.Offset.Y += (float)StatusController.LeftZoom.ratio;
						}
						else
						{
							StatusController.LeftZoom.Offset.Y -= (float)StatusController.LeftZoom.ratio;
						}

					}
					else
					{
						PointF midPoint = new PointF(3 * this.ClientSize.Width / 4, this.ClientSize.Height / 2);

						if (e.X < midPoint.X)
						{
							StatusController.RightZoom.Offset.X += (float)StatusController.RightZoom.ratio;
						}
						else
						{
							StatusController.RightZoom.Offset.X -= (float)StatusController.RightZoom.ratio;
						}

						if (e.Y < midPoint.Y)
						{
							StatusController.RightZoom.Offset.Y += (float)StatusController.RightZoom.ratio;
						}
						else
						{
							StatusController.RightZoom.Offset.Y -= (float)StatusController.RightZoom.ratio;
						}
					}
				}
				else if (e.Button == MouseButtons.Right)
				{
					this.MoveTimer.Stop();
					StatusController.LeftZoom.ratio = 1.0;
					StatusController.LeftZoom.Offset = new PointF(0.0F, -30.0F);

					StatusController.RightZoom.ratio = 1.0;
					StatusController.RightZoom.Offset = new PointF(0.0F, -30.0F);
				}
				this.Invalidate();
				return;
			}

			Polygon2DAdorner polygonAdorner = null;

			if (OnPart(point) == Partition.Left)
			{
				polygonAdorner = this.LeftPolygon;
			}
			else
			{
				point.X -= this.ClientSize.Width / 2;
				polygonAdorner = this.RightPolygon;
			}

			if (e.Button == MouseButtons.Left)
			{

				int i = polygonAdorner.HitTest(point);

				if (i == -1 && StatusController.canAddPoint)
				{
					if (StatusController.availablePartition == Partition.Left)
					{
						Polygon2DEditor pe = new Polygon2DEditor(FirstPolygon);

						pe.AddPoint(point);

						StatusController.canRestart = false;
					}
					else
					{
						Polygon2DEditor pe = new Polygon2DEditor(this.SecondPolygon);

						pe.AddPoint(new Point2D(point));

						StatusController.canRestart = false;
					}
				}

				else
				{
					StatusController.CatchPoint = i;
				}
			}
			else
			{
				int i = polygonAdorner.HitTest(point);

				if (i != -1)
				{
					if (StatusController.availablePartition == Partition.Left)
					{
						Polygon2DEditor pe = new Polygon2DEditor(this.FirstPolygon);

						pe.RemovePoint(i);

						StatusController.canRestart = false;
					}
					else
					{
						Polygon2DEditor pe = new Polygon2DEditor(this.SecondPolygon);

						pe.RemovePoint(i);

						StatusController.canRestart = false;
					}
				}
			}
			this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			Point2D point = new Point2D(e.X, e.Y);

			StatusController.CatchPoint = -1;

			this.statusBarPanel1.Text = this.GetPolygonStatus(point);
			this.MoveTimer.Stop();
		}

		#endregion

		#region Private methods

		private void PolygonScroll(Polygon2D polygon)
		{
			Polygon2DEditor pe = new Polygon2DEditor(polygon);

			pe.Scroll();
		}

		private bool CanTriangulate()
		{
			if (StatusController.Triangulator == TriangulationTech.Single)
			{
				if (this.FirstPolygon.isRegular && StatusController.availablePartition == Partition.Left)
				{
					return true;
				}
				if (this.SecondPolygon.isRegular && StatusController.availablePartition == Partition.Right)
				{
					return true;
				}
				return false;
			}

			if (!this.FirstPolygon.isRegular || !this.SecondPolygon.isRegular)
			{
				return false;
			}

			if (!this.FirstPolygon.isClosed || !this.SecondPolygon.isClosed)
			{
				return false;
			}

			if (!this.FirstPolygon.isSimple || !this.SecondPolygon.isSimple)
			{
				return false;
			}

			if (this.FirstPolygon.VertexCount != this.SecondPolygon.VertexCount)
			{
				return false;
			}

			return true;
		}
		private string GetPolygonStatus(Point2D point)
		{
			StringBuilder status = new StringBuilder();
			Polygon2D polygon = null;

			status.Append("Polygon:");

			if (this.OnPart(point) == Partition.Left)
			{
				status.Append("Left  ");
				polygon = this.FirstPolygon;
			}
			else
			{
				status.Append("Right  ");
				polygon = this.SecondPolygon;
			}

			status.Append(polygon.VertexCount.ToString());
			status.Append("(" + polygon.InnerPointCount.ToString() + ")");
			status.Append(" points");
			
			if (polygon.isRegular)
			{
				status.Append(" Regular");
			}
			else
			{
				status.Append(" Not regular");
			}

			if (polygon.isSimple)
			{
				status.Append(" Simple");
			}
			else
			{
				status.Append(" Complex");
			}

			if (polygon.isConvex)
			{
				status.Append(" Convex ");
			}
			else
			{
				status.Append(" Concave ");
			}

			status.Append(polygon.PointDirection.ToString());

			if (StatusController.isZoomView)
			{
				int r = 0;

				if (this.OnPart(point) == Partition.Left)
				{
					r = (int)(100 / StatusController.LeftZoom.ratio);
				}
				else
				{
					r = (int)(100 / StatusController.RightZoom.ratio);
				}
				
				status.Append(" ");
				status.Append(r.ToString());
				status.Append("% Zoom");
			}

			return status.ToString();
		}

		private Partition OnPart(Point2D point)
		{
			if (point.X < this.ClientSize.Width / 2)
			{
				return Partition.Left;
			}
			else
			{
				return Partition.Right;
			}
		}

		private void Triangulation()
		{
			Debug.Assert(this.CanTriangulate());

			if (StatusController.Triangulator == TriangulationTech.Simple)
			{
				Polygon2DTriangulator Triangulator = new Polygon2DTriangulator(LeftPolygon, RightPolygon);
				Triangulator.CompatibleTriangulation();
			}
			else if(StatusController.Triangulator == TriangulationTech.HighQuelity)
			{
				HighQualityTriangulator triangulator = new HighQualityTriangulator(this.FirstPolygon, this.SecondPolygon);

				triangulator.FastTriangluation();
				this.LeftPolygon.ApplyGhostTriangles(triangulator.GhostTriangle);
				this.RightPolygon.ApplyGhostTriangles(triangulator.GhostTriangle);
			}
			else
			{
				if(StatusController.availablePartition == Partition.Left)
				{
					LeftPolygon.Triangulation();
				}
				else
				{
					RightPolygon.Triangulation();
				}
			}
			StatusController.canAddPoint = false;
			this.toolBar1.Buttons[3].Enabled = false;
			this.Invalidate();
		}

		private void Clear()
		{
			this.LeftPolygon.Clear();
			this.RightPolygon.Clear();
			StatusController.canAddPoint = true;
			this.toolBarButton4.Enabled = true;
			this.toolBarButton4.Pushed = true;
			this.Invalidate();
		}

		private void Restart()
		{
			this.FileName = null;
			
			this.Clear();

			StatusController.canRestart = true;
			polygonLinkAdorner = null;

			if (StatusController.availablePartition == Partition.Left)
			{
				Polygon2D firstPolygon = new Polygon2D(new Point2DCollection());
				LeftPolygon = new Polygon2DAdorner(firstPolygon);
			}
			else
			{
				Polygon2D secondPolygon = new Polygon2D(new Point2DCollection());
				RightPolygon = new Polygon2DAdorner(secondPolygon);
			}
			this.triangulationMenuItem.Enabled = false;
			StatusController.canAddPoint = true;
			this.toolBarButton4.Enabled = true;
			this.toolBarButton4.Pushed = true;
			this.toolBar1.Buttons[5].Enabled = false;

			StatusController.LeftZoom.ratio = 1.0;
			StatusController.LeftZoom.Offset = new PointF(0.0F, -30.0F);
			StatusController.RightZoom.ratio = 1.0;
			StatusController.RightZoom.Offset = new PointF(0.0F, -30.0F);

			this.statusBarPanel1.Text = this.GetPolygonStatus(new Point2D(1,1));
			this.statusBarPanel2.Text = "Hint Index : -1 ";

			this.Invalidate();
		}

		private bool CanRestart()
		{
			if (!StatusController.canRestart)
			{
				if (!this.SaveFileWarning())
				{
					return false;
				}
			}
			return true;
		}
		
		#endregion

        #region Save & Load methods
		private bool SaveFileWarning()
		{
			System.Windows.Forms.DialogResult Result = System.Windows.Forms.MessageBox.Show("Save changes?", AppName, System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Warning);

			if (System.Windows.Forms.DialogResult.Yes == Result)
			{
				if (FileName != null)
				{
					this.Save(FileName);
				}
				else
				{
					if (this.saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
					{
						return false;
					}
				}
			}
			else if (System.Windows.Forms.DialogResult.Cancel == Result)
			{
				return false;
			}

			return true;
		}
		private void saveFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Save(this.saveFileDialog1.FileName);
		}
		private void openFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Restart();
			this.Open(this.openFileDialog1.FileName);
			this.Invalidate();
		}

		private void Save(string strFileName)
		{
			Polygon2D polygon = null;

			if (StatusController.availablePartition == Partition.Left)
			{
				polygon = this.FirstPolygon;
				this.FileName1 = strFileName;
			}
			else
			{
				polygon = this.SecondPolygon;
				this.FileName2 = strFileName;
			}

			string[] s = strFileName.Split('.');

			if (s[s.Length - 1] != "xwf")
			{
				WFParser wfParser = new WFParser(strFileName);
				bool isWritable = false;

				while (!isWritable)
				{
					try
					{
						wfParser.Write(polygon);
						isWritable = true;
					}
					catch (IOException)
					{
					}
				}
			}

			else
			{
				XWFParser xwfParser = new XWFParser(strFileName);
				bool isWritable = false;

				while (!isWritable)
				{
					try
					{
						xwfParser.Write(polygon);
						isWritable = true;
					}
					catch (IOException)
					{
					}
				}
			}
		}

		private void Open(string strFileName)
		{
			WFParser wfParser = new WFParser(strFileName);

			Polygon2D polygon = wfParser.Read();

			if (StatusController.availablePartition == Partition.Left)
			{
				this.LeftPolygon = new Polygon2DAdorner(polygon);
				this.FileName1 = strFileName;
			}
			else
			{
				this.RightPolygon = new Polygon2DAdorner(polygon);
				this.FileName2 = strFileName;
			}
			this.LeftPolygon.ShowIndices = this.showIndicesMenuItem.Checked;
			this.RightPolygon.ShowIndices = this.showIndicesMenuItem.Checked;
		}

		private void saveasMenuItem_Click(object sender, System.EventArgs e)
		{
			this.saveFileDialog1.ShowDialog();
		}
		private void saveMenuItem1_Click(object sender, System.EventArgs e)
		{
			if (this.FileName != null)
			{
				this.Save(this.FileName);
			}
			else
			{
				this.saveFileDialog1.ShowDialog();
			}
			
			this.Invalidate();
		}
		private void saveMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.CanRestart())
			{
				this.openFileDialog1.ShowDialog();
			}

			this.Invalidate();
		}
		#endregion

		#region Toolbar Events
		private void toolBar1_ButtonClick_1(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch (e.Button.ImageIndex)
			{
				case 0:
					if (this.CanRestart())
					{
						this.Restart();
					}
					break;
				case 1 :
					if (this.CanRestart())
					{
						this.openFileDialog1.ShowDialog();
					}
					break;
				case 2 :
					if (FileName != null)
					{
						this.Save(FileName);
					}
					else
					{
						this.saveFileDialog1.ShowDialog();
					}
					break;
				case 3 :
					StatusController.canAddPoint = this.toolBarButton4.Pushed;	
					break;
				case 7 :
					this.Optimize();
					break;
				case 5 :
					this.Triangulation();
					break;
				case 6 :
					this.Clear();
					break;
				default:
					break;
			}
		}
		#endregion

		#region Menu Events

		private void showIndicesMenuItem_Click(object sender, System.EventArgs e)
		{
			this.showIndicesMenuItem.Checked = !this.showIndicesMenuItem.Checked;
			this.LeftPolygon.ShowIndices = this.showIndicesMenuItem.Checked;
			this.RightPolygon.ShowIndices = this.showIndicesMenuItem.Checked;
			if (this.showIndicesMenuItem.Checked)
			{
				this.showIndicesMenuItem.Text = "Hide &Indices";
			}
			else
			{
				this.showIndicesMenuItem.Text = "Show &Indices";
			}

			this.Invalidate();
		}

		private void aboutMenuItem_Click(object sender, System.EventArgs e)
		{
			frmAbout formAbout = new frmAbout();

			formAbout.Show();
		}

		private void exitMenuItem_Click(object sender, System.EventArgs e)
		{
			StatusController.availablePartition = Partition.Left;
			if (this.CanRestart())
			{
				StatusController.availablePartition = Partition.Right;
				if (this.CanRestart())
				{
					Application.Exit();
				}
			}
		}
		private void triangulationMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Triangulation();
		}

		private void clearMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Clear();
		}

		private void newMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.CanRestart())
			{
				this.Restart();
			}
		}
		private void singleViewMenuItem_Click(object sender, System.EventArgs e)
		{
			this.singleViewMenuItem.Checked = true;
			this.competibleTriangulationMenuItem.Checked = false;
			this.highQualityTriangulationMenuItem.Checked = false;
			StatusController.Triangulator = TriangulationTech.Single;
		}

		private void competibleTriangulationMenuItem_Click(object sender, System.EventArgs e)
		{
			this.singleViewMenuItem.Checked = false; 
			this.competibleTriangulationMenuItem.Checked = true;
			this.highQualityTriangulationMenuItem.Checked = false;
			StatusController.Triangulator = TriangulationTech.Simple;
		}

		private void highQualityTriangulationMenuItem_Click(object sender, System.EventArgs e)
		{
			this.singleViewMenuItem.Checked = false;
			this.competibleTriangulationMenuItem.Checked = false;
			this.highQualityTriangulationMenuItem.Checked = true;
			StatusController.Triangulator = TriangulationTech.HighQuelity;
		}

		private void leftMenuItem1_Click(object sender, System.EventArgs e)
		{
			PolygonScroll(this.FirstPolygon);
			this.Triangulation();
		}

		private void rightMenuItem_Click(object sender, System.EventArgs e)
		{
			PolygonScroll(this.SecondPolygon);
			this.Triangulation();
		}
		#endregion

		#region Other Events
		protected override void OnClosing(CancelEventArgs e)
		{
			StatusController.availablePartition = Partition.Left;
			if (this.CanRestart())
			{
				StatusController.availablePartition = Partition.Right;
				if (this.CanRestart())
				{
					return;
				}
			}
			e.Cancel = true;
		}
		#endregion

		#region Zoom Controls
		private void Zooming(double r)
		{
			StatusController.LeftZoom.ratio = r;
			StatusController.RightZoom.ratio = r;
			this.Invalidate();
		}
		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			Zooming(2);
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			Zooming(4);
		}
		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			Zooming(6);
		}
		private void menuItem6_Click(object sender, System.EventArgs e)
		{
			Zooming(8);
		}
		private void menuItem8_Click(object sender, System.EventArgs e)
		{
			Zooming(1.0);
			StatusController.LeftZoom.Offset = new PointF(0.0F, -30.0F);
			StatusController.RightZoom.Offset = new PointF(0.0F, -30.0F);
		}
		private void menuItem9_Click(object sender, System.EventArgs e)
		{
			Zooming(0.5);
		}
		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			Zooming(0.25);
		}
		#endregion

		#region Test
		
		private PolygonLinkAdorner polygonLinkAdorner = null;

		private void linkMenuItem_Click_1(object sender, System.EventArgs e)
		{
			Polygon2DLinkMaker polygonLink = new Polygon2DLinkMaker(this.FirstPolygon, 0, 4);
			polygonLink.Divide();
			polygonLink.BuildPath();
			polygonLinkAdorner = new PolygonLinkAdorner(polygonLink);
			
			this.Invalidate();
		}

		private void triangulationMenuItem1_Click_1(object sender, System.EventArgs e)
		{
			HighQualityTriangulator triangulator = new HighQualityTriangulator(this.FirstPolygon, this.SecondPolygon);

			triangulator.Triangulation();

			this.LeftPolygon.ApplyGhostTriangles(triangulator.GhostTriangle);
			this.RightPolygon.ApplyGhostTriangles(triangulator.GhostTriangle);

			this.Invalidate();
		}

		private void flipMenuItem_Click_1(object sender, System.EventArgs e)
		{
			Fliper fliper = new Fliper(this.FirstPolygon, this.SecondPolygon);

			fliper.Flipping(this.LeftPolygon.GhostTriangles);

			this.Invalidate();
		}

		private void divideMenuItem_Click_1(object sender, System.EventArgs e)
		{
			TriangleDiviser triangleDiviser = new TriangleDiviser(this.FirstPolygon, this.SecondPolygon);

			triangleDiviser.Divide(this.LeftPolygon.GhostTriangles);

			this.Invalidate();
		}

		private void splitMenuItem_Click_1(object sender, System.EventArgs e)
		{
			TriangleDiviser triangleDiviser = new TriangleDiviser(this.FirstPolygon, this.SecondPolygon);

			triangleDiviser.Split(this.LeftPolygon.GhostTriangles);

			this.Invalidate();
		}

		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			Thread th = new Thread(new ThreadStart(DivideTarget));
			th.Start();
			//this.DivideTarget();
		}
		#endregion

		private void DivideTarget()
		{
			CompatibleDiviser cd = new CompatibleDiviser(this.FirstPolygon, this.SecondPolygon);
			cd.UpdateStatus += new Microsoft.VS.Akira.Triangulations.CompatibleDiviser.ShowStatus(surfaceFitter_UpdateStatus);
			cd.Divide();

            this.surfaceFitter_UpdateStatus(this, "Building...");
			this.LeftPolygon = new Polygon2DAdorner(cd.FirstPolygonDivisions[0]);
			this.RightPolygon  = new Polygon2DAdorner(cd.SecondPolygonDivisions[0]);

			for (int i = 1; i < cd.FirstPolygonDivisions.Count; i++)
			{
				Debug.Assert(cd.FirstPolygonDivisions[i].VertexCount == cd.SecondPolygonDivisions[i].VertexCount);
				this.LeftPolygon.AppendChild(new Polygon2DAdorner(cd.FirstPolygonDivisions[i]));
				this.RightPolygon.AppendChild(new Polygon2DAdorner(cd.SecondPolygonDivisions[i]));
			}

			this.LeftPolygon.Triangulation();
			
			this.surfaceFitter_UpdateStatus(this, "Done");
			this.Invalidate();
		}
		#region Optimization
		private void areaBasedRemeshingMenuItem_Click(object sender, System.EventArgs e)
		{
			this.areaBasedRemeshingMenuItem.Checked = !this.areaBasedRemeshingMenuItem.Checked;
		}
		private void flippingMenuItem1_Click(object sender, System.EventArgs e)
		{
			this.flippingMenuItem1.Checked = !this.flipMenuItem.Checked;
		}
		private void subDivisionMenuItem_Click(object sender, System.EventArgs e)
		{
			this.subDivisionMenuItem.Checked = !this.subDivisionMenuItem.Checked;
		}
		private void splitingMenuItem1_Click(object sender, System.EventArgs e)
		{
			this.splitingMenuItem1.Checked = !this.splitingMenuItem1.Checked;
		}
		private void Optimize()
		{
			if (this.flippingMenuItem1.Checked)
			{
				this.Flipping();
			}
			if (this.splitingMenuItem1.Checked)
			{
				this.Split();
			}
			if (this.subDivisionMenuItem.Checked)
			{
				this.Divide();
			}
			if (this.areaBasedRemeshingMenuItem.Checked)
			{
				this.Remeshing();
			}
		}
		private void optimizationMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Optimize();
			this.Invalidate();
		}
		private void Flipping()
		{
			Polygon2DAdorner leftPolygon = this.LeftPolygon;
			Polygon2DAdorner rightPolygon = this.RightPolygon;
            
			while(leftPolygon != null)
			{
				if (StatusController.Triangulator != TriangulationTech.Single)
				{
					Fliper fliper = new Fliper(leftPolygon.polygon, rightPolygon.polygon);

					fliper.Flipping(leftPolygon.GhostTriangles);
				}

				else if (StatusController.availablePartition == Partition.Left)
				{	
					Fliper fliper = new Fliper(leftPolygon.polygon, leftPolygon.polygon);

					fliper.Flipping(leftPolygon.GhostTriangles);
				}

				else
				{
					Fliper fliper = new Fliper(rightPolygon.polygon, rightPolygon.polygon);

					fliper.Flipping(rightPolygon.GhostTriangles);
				}
				
				leftPolygon = leftPolygon.Next();
				rightPolygon = rightPolygon.Next();
			}

			this.Invalidate();
		}
		private void Divide()
		{
			Polygon2DAdorner leftPolygon = this.LeftPolygon;
			Polygon2DAdorner rightPolygon = this.RightPolygon;

			while (leftPolygon != null)
			{
				if (StatusController.Triangulator != TriangulationTech.Single)
				{
					TriangleDiviser triangleDiviser = new TriangleDiviser(leftPolygon.polygon, rightPolygon.polygon);

					triangleDiviser.Divide(leftPolygon.GhostTriangles);
				}

				else if (StatusController.availablePartition == Partition.Left)
				{
					TriangleDiviser triangleDiviser = new TriangleDiviser(leftPolygon.polygon, leftPolygon.polygon);

					triangleDiviser.Divide(leftPolygon.GhostTriangles);
				}

				else
				{
					TriangleDiviser triangleDiviser = new TriangleDiviser(rightPolygon.polygon, rightPolygon.polygon);

					triangleDiviser.Divide(rightPolygon.GhostTriangles);
				}
				leftPolygon = leftPolygon.Next();
				rightPolygon = rightPolygon.Next();
			}

			this.Invalidate();
		}
		private void Remeshing()
		{
			Polygon2DAdorner leftPolygon = this.LeftPolygon;
			Polygon2DAdorner rightPolygon = this.RightPolygon;

			while (leftPolygon != null)
			{
				if (StatusController.Triangulator != TriangulationTech.Single || StatusController.availablePartition == Partition.Left)
				{
					GhostWeb ghostWeb = new GhostWeb(leftPolygon.polygon, leftPolygon.GhostTriangles);
	
					ghostWeb.AreaBasedRemeshing();
				}

				if(StatusController.Triangulator != TriangulationTech.Single || StatusController.availablePartition == Partition.Right)
				{
					GhostWeb ghostWeb = new GhostWeb(rightPolygon.polygon, rightPolygon.GhostTriangles);

					ghostWeb.AreaBasedRemeshing();
				}
				leftPolygon = leftPolygon.Next();
				rightPolygon = rightPolygon.Next();
			}
			this.Invalidate();
		}
		private void Split()
		{
			Polygon2DAdorner leftPolygon = this.LeftPolygon;
			Polygon2DAdorner rightPolygon = this.RightPolygon;

			while(leftPolygon != null)
			{
				if (StatusController.Triangulator != TriangulationTech.Single)
				{
					TriangleDiviser triangleDiviser = new TriangleDiviser(leftPolygon.polygon, rightPolygon.polygon);

					triangleDiviser.Split(leftPolygon.GhostTriangles);
				}
				else if (StatusController.availablePartition == Partition.Left)
				{
					TriangleDiviser triangleDiviser = new TriangleDiviser(leftPolygon.polygon, leftPolygon.polygon);

					triangleDiviser.Split(leftPolygon.GhostTriangles);
				}
				else
				{
					TriangleDiviser triangleDiviser = new TriangleDiviser(rightPolygon.polygon, rightPolygon.polygon);

					triangleDiviser.Split(rightPolygon.GhostTriangles);
				}

				leftPolygon = leftPolygon.Next();
				rightPolygon = rightPolygon.Next();
			}
			this.Invalidate();
		}
		#endregion

		private void menuItem13_Click(object sender, System.EventArgs e)
		{
			XWFParser parser = new XWFParser("test.xml");
			parser.Write(this.FirstPolygon);
		}

		private void menuItem14_Click(object sender, System.EventArgs e)
		{
			XWFParser parser = new XWFParser("test.xml");
			Polygon2D polygon = parser.Read();
			this.LeftPolygon = new Polygon2DAdorner(polygon);
			this.Invalidate();
		}

		#region Timer

		private void MoveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (StatusController.availablePartition == Partition.Left)
			{
				if (this.Cursor == System.Windows.Forms.Cursors.PanNW)
				{
					StatusController.LeftZoom.Offset.X += (float)StatusController.LeftZoom.ratio;
					StatusController.LeftZoom.Offset.Y += (float)StatusController.LeftZoom.ratio;
				}
				else if (this.Cursor == System.Windows.Forms.Cursors.PanNE)
				{
					StatusController.LeftZoom.Offset.X -= (float)StatusController.LeftZoom.ratio;
					StatusController.LeftZoom.Offset.Y += (float)StatusController.LeftZoom.ratio;
				}
				else if (this.Cursor == System.Windows.Forms.Cursors.PanSW)
				{
					StatusController.LeftZoom.Offset.X += (float)StatusController.LeftZoom.ratio;
					StatusController.LeftZoom.Offset.Y -= (float)StatusController.LeftZoom.ratio;
				}
				else if (this.Cursor == System.Windows.Forms.Cursors.PanSE)
				{
					StatusController.LeftZoom.Offset.X -= (float)StatusController.LeftZoom.ratio;
					StatusController.LeftZoom.Offset.Y -= (float)StatusController.LeftZoom.ratio;
				}
			}
			if (StatusController.availablePartition == Partition.Right)
			{
				if (this.Cursor == System.Windows.Forms.Cursors.PanNW)
				{
					StatusController.RightZoom.Offset.X += (float)StatusController.LeftZoom.ratio;
					StatusController.RightZoom.Offset.Y += (float)StatusController.LeftZoom.ratio;
				}
				else if (this.Cursor == System.Windows.Forms.Cursors.PanNE)
				{
					StatusController.RightZoom.Offset.X -= (float)StatusController.LeftZoom.ratio;
					StatusController.RightZoom.Offset.Y += (float)StatusController.LeftZoom.ratio;
				}
				else if (this.Cursor == System.Windows.Forms.Cursors.PanSW)
				{
					StatusController.RightZoom.Offset.X += (float)StatusController.LeftZoom.ratio;
					StatusController.RightZoom.Offset.Y -= (float)StatusController.LeftZoom.ratio;
				}
				else if (this.Cursor == System.Windows.Forms.Cursors.PanSE)
				{
					StatusController.RightZoom.Offset.X -= (float)StatusController.LeftZoom.ratio;
					StatusController.RightZoom.Offset.Y -= (float)StatusController.LeftZoom.ratio;
				}
			}
			this.Invalidate();
		}

		#endregion

		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			Thread th = new Thread(new ThreadStart(SurfaceFitting));
			th.Start();
		}

		private void SurfaceFitting()
		{
			Polygon2DAdorner leftPolygon = this.LeftPolygon;
			Polygon2DAdorner rightPolygon = this.RightPolygon;

			while (leftPolygon != null)
			{
				SurfaceFitter surfaceFitter = new SurfaceFitter(leftPolygon.polygon, rightPolygon.polygon, leftPolygon.GhostTriangles);
				surfaceFitter.UpdateStatus += new Microsoft.VS.Akira.Triangulations.SurfaceFitter.ShowStatus(surfaceFitter_UpdateStatus);
		
				surfaceFitter.Fitting();
			
				leftPolygon.ApplyGhostTriangles(surfaceFitter.ghostTriangles);
				rightPolygon.ApplyGhostTriangles(surfaceFitter.ghostTriangles);

				leftPolygon = leftPolygon.Next();
				rightPolygon = rightPolygon.Next();
			}

			this.Invalidate();
		}

		private void surfaceFitter_UpdateStatus(object sender, string msg)
		{
			this.statusBarPanel3.Text = msg;
		}

		private void menuItem16_Click(object sender, System.EventArgs e)
		{
		}
		private bool testOnly = false;
		private void testPaint(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}

		private void menuItem17_Click(object sender, System.EventArgs e)
		{
			this.saveFileDialog2.ShowDialog();
		}

		private void saveFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.currentImage.Save(this.saveFileDialog2.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
		}
	}
}
