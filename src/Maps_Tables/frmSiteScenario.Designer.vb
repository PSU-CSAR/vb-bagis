﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSiteScenario
  Inherits System.Windows.Forms.UserControl

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing AndAlso components IsNot Nothing Then
      components.Dispose()
    End If
    MyBase.Dispose(disposing)
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSiteScenario))
        Me.GrdScenario1 = New System.Windows.Forms.DataGridView()
        Me.Selected = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.ObjectId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Site_Type = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Site_Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Elevation = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Upper_Elev = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Lower_Elev = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DefaultElevation = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GrdScenario2 = New System.Windows.Forms.DataGridView()
        Me.SObjectId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ScenarioType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ScenarioName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ScenarioElevation = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ScenarioUpper_Elev = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ScenarioLower_Elev = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SDefaultElevation = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BtnAddSite = New System.Windows.Forms.Button()
        Me.BtnRemoveSite = New System.Windows.Forms.Button()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.lblElevation = New System.Windows.Forms.Label()
        Me.txtMaxElev = New System.Windows.Forms.TextBox()
        Me.txtMinElev = New System.Windows.Forms.TextBox()
        Me.OptZFeet = New System.Windows.Forms.RadioButton()
        Me.OptZMeters = New System.Windows.Forms.RadioButton()
        Me.LblElevUnit = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.BtnCalculate = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.BtnNewSite = New System.Windows.Forms.Button()
        Me.BtnPreview = New System.Windows.Forms.Button()
        Me.BtnRemoveAll = New System.Windows.Forms.Button()
        Me.BtnAddAll = New System.Windows.Forms.Button()
        Me.BtnDeleteSite = New System.Windows.Forms.Button()
        Me.BtnToggleSel = New System.Windows.Forms.Button()
        Me.BtnViewResult = New System.Windows.Forms.Button()
        Me.BtnReload = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtBufferDistance = New System.Windows.Forms.TextBox()
        Me.TxtUpperRange = New System.Windows.Forms.TextBox()
        Me.TxtLowerRange = New System.Windows.Forms.TextBox()
        Me.LblBufferDistance = New System.Windows.Forms.Label()
        Me.LblUpperRange = New System.Windows.Forms.Label()
        Me.LblLowerRange = New System.Windows.Forms.Label()
        Me.CmboxDistanceUnit = New System.Windows.Forms.ComboBox()
        Me.BtnAbout = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtReportTitle = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ChkBufferDistance = New System.Windows.Forms.CheckBox()
        Me.ChkUpperRange = New System.Windows.Forms.CheckBox()
        Me.ChkLowerRange = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtScenario1 = New System.Windows.Forms.TextBox()
        Me.TxtScenario2 = New System.Windows.Forms.TextBox()
        Me.BtnMaps = New System.Windows.Forms.Button()
        Me.BtnAutoPseudo = New System.Windows.Forms.Button()
        Me.BtnAutoLog = New System.Windows.Forms.Button()
        Me.BtnTables = New System.Windows.Forms.Button()
        Me.BtnTableHelp = New System.Windows.Forms.Button()
        CType(Me.GrdScenario1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GrdScenario2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GrdScenario1
        '
        Me.GrdScenario1.AllowUserToAddRows = False
        Me.GrdScenario1.AllowUserToDeleteRows = False
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.GrdScenario1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        Me.GrdScenario1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.GrdScenario1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Selected, Me.ObjectId, Me.Site_Type, Me.Site_Name, Me.Elevation, Me.Upper_Elev, Me.Lower_Elev, Me.DefaultElevation})
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.GrdScenario1.DefaultCellStyle = DataGridViewCellStyle6
        Me.GrdScenario1.Location = New System.Drawing.Point(1, 186)
        Me.GrdScenario1.Margin = New System.Windows.Forms.Padding(2)
        Me.GrdScenario1.Name = "GrdScenario1"
        Me.GrdScenario1.RowHeadersVisible = False
        Me.GrdScenario1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.GrdScenario1.Size = New System.Drawing.Size(432, 147)
        Me.GrdScenario1.TabIndex = 0
        '
        'Selected
        '
        Me.Selected.HeaderText = "□"
        Me.Selected.Name = "Selected"
        Me.Selected.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Selected.Width = 30
        '
        'ObjectId
        '
        Me.ObjectId.HeaderText = "ObjectId"
        Me.ObjectId.Name = "ObjectId"
        Me.ObjectId.Visible = False
        '
        'Site_Type
        '
        Me.Site_Type.HeaderText = "Type"
        Me.Site_Type.Name = "Site_Type"
        Me.Site_Type.ReadOnly = True
        Me.Site_Type.Width = 90
        '
        'Site_Name
        '
        Me.Site_Name.HeaderText = "Name"
        Me.Site_Name.Name = "Site_Name"
        Me.Site_Name.ReadOnly = True
        Me.Site_Name.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Site_Name.Width = 220
        '
        'Elevation
        '
        Me.Elevation.HeaderText = "Elevation"
        Me.Elevation.Name = "Elevation"
        Me.Elevation.ReadOnly = True
        Me.Elevation.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Upper_Elev
        '
        Me.Upper_Elev.HeaderText = "Upper Elevation"
        Me.Upper_Elev.Name = "Upper_Elev"
        Me.Upper_Elev.ReadOnly = True
        '
        'Lower_Elev
        '
        Me.Lower_Elev.HeaderText = "Lower Elevation"
        Me.Lower_Elev.Name = "Lower_Elev"
        Me.Lower_Elev.ReadOnly = True
        '
        'DefaultElevation
        '
        Me.DefaultElevation.HeaderText = "Default Elevation"
        Me.DefaultElevation.Name = "DefaultElevation"
        Me.DefaultElevation.Visible = False
        '
        'GrdScenario2
        '
        Me.GrdScenario2.AllowUserToAddRows = False
        Me.GrdScenario2.AllowUserToDeleteRows = False
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.GrdScenario2.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.GrdScenario2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.GrdScenario2.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.SObjectId, Me.ScenarioType, Me.ScenarioName, Me.ScenarioElevation, Me.ScenarioUpper_Elev, Me.ScenarioLower_Elev, Me.SDefaultElevation})
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.GrdScenario2.DefaultCellStyle = DataGridViewCellStyle8
        Me.GrdScenario2.Location = New System.Drawing.Point(-3, 463)
        Me.GrdScenario2.Margin = New System.Windows.Forms.Padding(2)
        Me.GrdScenario2.Name = "GrdScenario2"
        Me.GrdScenario2.RowHeadersVisible = False
        Me.GrdScenario2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.GrdScenario2.Size = New System.Drawing.Size(436, 147)
        Me.GrdScenario2.TabIndex = 1
        '
        'SObjectId
        '
        Me.SObjectId.HeaderText = "SObjectId"
        Me.SObjectId.Name = "SObjectId"
        Me.SObjectId.Visible = False
        '
        'ScenarioType
        '
        Me.ScenarioType.HeaderText = "Type"
        Me.ScenarioType.Name = "ScenarioType"
        Me.ScenarioType.ReadOnly = True
        '
        'ScenarioName
        '
        Me.ScenarioName.HeaderText = "Name"
        Me.ScenarioName.Name = "ScenarioName"
        Me.ScenarioName.ReadOnly = True
        Me.ScenarioName.Width = 235
        '
        'ScenarioElevation
        '
        Me.ScenarioElevation.HeaderText = "Elevation"
        Me.ScenarioElevation.Name = "ScenarioElevation"
        Me.ScenarioElevation.ReadOnly = True
        '
        'ScenarioUpper_Elev
        '
        Me.ScenarioUpper_Elev.HeaderText = "Upper Elevation"
        Me.ScenarioUpper_Elev.Name = "ScenarioUpper_Elev"
        Me.ScenarioUpper_Elev.ReadOnly = True
        '
        'ScenarioLower_Elev
        '
        Me.ScenarioLower_Elev.HeaderText = "Lower Elevation"
        Me.ScenarioLower_Elev.Name = "ScenarioLower_Elev"
        Me.ScenarioLower_Elev.ReadOnly = True
        '
        'SDefaultElevation
        '
        Me.SDefaultElevation.HeaderText = "DefaultElevation"
        Me.SDefaultElevation.Name = "SDefaultElevation"
        Me.SDefaultElevation.Visible = False
        '
        'BtnAddSite
        '
        Me.BtnAddSite.Image = CType(resources.GetObject("BtnAddSite.Image"), System.Drawing.Image)
        Me.BtnAddSite.Location = New System.Drawing.Point(200, 360)
        Me.BtnAddSite.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnAddSite.Name = "BtnAddSite"
        Me.BtnAddSite.Size = New System.Drawing.Size(35, 35)
        Me.BtnAddSite.TabIndex = 37
        Me.BtnAddSite.UseVisualStyleBackColor = True
        '
        'BtnRemoveSite
        '
        Me.BtnRemoveSite.Image = CType(resources.GetObject("BtnRemoveSite.Image"), System.Drawing.Image)
        Me.BtnRemoveSite.Location = New System.Drawing.Point(239, 360)
        Me.BtnRemoveSite.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnRemoveSite.Name = "BtnRemoveSite"
        Me.BtnRemoveSite.Size = New System.Drawing.Size(35, 35)
        Me.BtnRemoveSite.TabIndex = 38
        Me.BtnRemoveSite.UseVisualStyleBackColor = True
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.Location = New System.Drawing.Point(6, 47)
        Me.Label24.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(42, 20)
        Me.Label24.TabIndex = 42
        Me.Label24.Text = "Max:"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(9, 27)
        Me.Label23.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(38, 20)
        Me.Label23.TabIndex = 39
        Me.Label23.Text = "Min:"
        '
        'lblElevation
        '
        Me.lblElevation.AutoSize = True
        Me.lblElevation.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblElevation.Location = New System.Drawing.Point(6, 4)
        Me.lblElevation.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblElevation.Name = "lblElevation"
        Me.lblElevation.Size = New System.Drawing.Size(114, 20)
        Me.lblElevation.TabIndex = 40
        Me.lblElevation.Text = "DEM Elevation"
        '
        'txtMaxElev
        '
        Me.txtMaxElev.BackColor = System.Drawing.SystemColors.Menu
        Me.txtMaxElev.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtMaxElev.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMaxElev.ForeColor = System.Drawing.Color.Blue
        Me.txtMaxElev.Location = New System.Drawing.Point(45, 49)
        Me.txtMaxElev.Margin = New System.Windows.Forms.Padding(2)
        Me.txtMaxElev.Name = "txtMaxElev"
        Me.txtMaxElev.ReadOnly = True
        Me.txtMaxElev.Size = New System.Drawing.Size(70, 19)
        Me.txtMaxElev.TabIndex = 44
        Me.txtMaxElev.Text = "0"
        '
        'txtMinElev
        '
        Me.txtMinElev.BackColor = System.Drawing.SystemColors.Menu
        Me.txtMinElev.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtMinElev.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMinElev.ForeColor = System.Drawing.Color.Blue
        Me.txtMinElev.Location = New System.Drawing.Point(45, 29)
        Me.txtMinElev.Margin = New System.Windows.Forms.Padding(2)
        Me.txtMinElev.Name = "txtMinElev"
        Me.txtMinElev.ReadOnly = True
        Me.txtMinElev.Size = New System.Drawing.Size(70, 19)
        Me.txtMinElev.TabIndex = 43
        Me.txtMinElev.Text = "0"
        '
        'OptZFeet
        '
        Me.OptZFeet.AutoSize = True
        Me.OptZFeet.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OptZFeet.Location = New System.Drawing.Point(388, 46)
        Me.OptZFeet.Margin = New System.Windows.Forms.Padding(2)
        Me.OptZFeet.Name = "OptZFeet"
        Me.OptZFeet.Size = New System.Drawing.Size(60, 24)
        Me.OptZFeet.TabIndex = 46
        Me.OptZFeet.Text = "Feet"
        Me.OptZFeet.UseVisualStyleBackColor = True
        '
        'OptZMeters
        '
        Me.OptZMeters.AutoSize = True
        Me.OptZMeters.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OptZMeters.Location = New System.Drawing.Point(388, 26)
        Me.OptZMeters.Margin = New System.Windows.Forms.Padding(2)
        Me.OptZMeters.Name = "OptZMeters"
        Me.OptZMeters.Size = New System.Drawing.Size(76, 24)
        Me.OptZMeters.TabIndex = 47
        Me.OptZMeters.Text = "Meters"
        Me.OptZMeters.UseVisualStyleBackColor = True
        '
        'LblElevUnit
        '
        Me.LblElevUnit.AutoSize = True
        Me.LblElevUnit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblElevUnit.Location = New System.Drawing.Point(362, 4)
        Me.LblElevUnit.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LblElevUnit.Name = "LblElevUnit"
        Me.LblElevUnit.Size = New System.Drawing.Size(107, 20)
        Me.LblElevUnit.TabIndex = 45
        Me.LblElevUnit.Text = "Elevation Unit"
        '
        'BtnClose
        '
        Me.BtnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.Location = New System.Drawing.Point(383, 130)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(55, 22)
        Me.BtnClose.TabIndex = 49
        Me.BtnClose.Text = "Close"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'BtnCalculate
        '
        Me.BtnCalculate.Enabled = False
        Me.BtnCalculate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnCalculate.Location = New System.Drawing.Point(202, 105)
        Me.BtnCalculate.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnCalculate.Name = "BtnCalculate"
        Me.BtnCalculate.Size = New System.Drawing.Size(79, 22)
        Me.BtnCalculate.TabIndex = 48
        Me.BtnCalculate.Text = "Calculate"
        Me.BtnCalculate.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(7, 159)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(84, 18)
        Me.Label6.TabIndex = 50
        Me.Label6.Text = "Scenario1"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(-4, 431)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(89, 18)
        Me.Label5.TabIndex = 51
        Me.Label5.Text = "Scenario 2"
        '
        'BtnNewSite
        '
        Me.BtnNewSite.Image = CType(resources.GetObject("BtnNewSite.Image"), System.Drawing.Image)
        Me.BtnNewSite.Location = New System.Drawing.Point(6, 360)
        Me.BtnNewSite.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnNewSite.Name = "BtnNewSite"
        Me.BtnNewSite.Size = New System.Drawing.Size(35, 35)
        Me.BtnNewSite.TabIndex = 54
        Me.BtnNewSite.UseVisualStyleBackColor = True
        '
        'BtnPreview
        '
        Me.BtnPreview.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPreview.Location = New System.Drawing.Point(191, 402)
        Me.BtnPreview.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnPreview.Name = "BtnPreview"
        Me.BtnPreview.Size = New System.Drawing.Size(165, 22)
        Me.BtnPreview.TabIndex = 55
        Me.BtnPreview.Text = "Preview Site Buffers"
        Me.BtnPreview.UseVisualStyleBackColor = True
        '
        'BtnRemoveAll
        '
        Me.BtnRemoveAll.Image = CType(resources.GetObject("BtnRemoveAll.Image"), System.Drawing.Image)
        Me.BtnRemoveAll.Location = New System.Drawing.Point(318, 360)
        Me.BtnRemoveAll.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnRemoveAll.Name = "BtnRemoveAll"
        Me.BtnRemoveAll.Size = New System.Drawing.Size(35, 35)
        Me.BtnRemoveAll.TabIndex = 56
        Me.BtnRemoveAll.UseVisualStyleBackColor = True
        '
        'BtnAddAll
        '
        Me.BtnAddAll.Image = CType(resources.GetObject("BtnAddAll.Image"), System.Drawing.Image)
        Me.BtnAddAll.Location = New System.Drawing.Point(278, 360)
        Me.BtnAddAll.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnAddAll.Name = "BtnAddAll"
        Me.BtnAddAll.Size = New System.Drawing.Size(35, 35)
        Me.BtnAddAll.TabIndex = 57
        Me.BtnAddAll.UseVisualStyleBackColor = True
        '
        'BtnDeleteSite
        '
        Me.BtnDeleteSite.Image = CType(resources.GetObject("BtnDeleteSite.Image"), System.Drawing.Image)
        Me.BtnDeleteSite.Location = New System.Drawing.Point(46, 360)
        Me.BtnDeleteSite.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnDeleteSite.Name = "BtnDeleteSite"
        Me.BtnDeleteSite.Size = New System.Drawing.Size(35, 35)
        Me.BtnDeleteSite.TabIndex = 58
        Me.BtnDeleteSite.UseVisualStyleBackColor = True
        '
        'BtnToggleSel
        '
        Me.BtnToggleSel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnToggleSel.Location = New System.Drawing.Point(5, 130)
        Me.BtnToggleSel.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnToggleSel.Name = "BtnToggleSel"
        Me.BtnToggleSel.Size = New System.Drawing.Size(120, 23)
        Me.BtnToggleSel.TabIndex = 59
        Me.BtnToggleSel.Text = "Toggle Selection"
        Me.BtnToggleSel.UseVisualStyleBackColor = True
        '
        'BtnViewResult
        '
        Me.BtnViewResult.Enabled = False
        Me.BtnViewResult.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnViewResult.Location = New System.Drawing.Point(290, 130)
        Me.BtnViewResult.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnViewResult.Name = "BtnViewResult"
        Me.BtnViewResult.Size = New System.Drawing.Size(63, 22)
        Me.BtnViewResult.TabIndex = 60
        Me.BtnViewResult.Text = "Report"
        Me.BtnViewResult.UseVisualStyleBackColor = True
        '
        'BtnReload
        '
        Me.BtnReload.Image = CType(resources.GetObject("BtnReload.Image"), System.Drawing.Image)
        Me.BtnReload.Location = New System.Drawing.Point(371, 360)
        Me.BtnReload.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnReload.Name = "BtnReload"
        Me.BtnReload.Size = New System.Drawing.Size(35, 35)
        Me.BtnReload.TabIndex = 61
        Me.BtnReload.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(137, 4)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(190, 20)
        Me.Label1.TabIndex = 62
        Me.Label1.Text = "Representation Definition"
        '
        'TxtBufferDistance
        '
        Me.TxtBufferDistance.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBufferDistance.Location = New System.Drawing.Point(241, 23)
        Me.TxtBufferDistance.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtBufferDistance.Name = "TxtBufferDistance"
        Me.TxtBufferDistance.Size = New System.Drawing.Size(43, 20)
        Me.TxtBufferDistance.TabIndex = 63
        Me.TxtBufferDistance.Text = "0"
        Me.TxtBufferDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtUpperRange
        '
        Me.TxtUpperRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtUpperRange.Location = New System.Drawing.Point(241, 41)
        Me.TxtUpperRange.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtUpperRange.Name = "TxtUpperRange"
        Me.TxtUpperRange.Size = New System.Drawing.Size(43, 20)
        Me.TxtUpperRange.TabIndex = 64
        Me.TxtUpperRange.Text = "0"
        Me.TxtUpperRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtLowerRange
        '
        Me.TxtLowerRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLowerRange.Location = New System.Drawing.Point(241, 58)
        Me.TxtLowerRange.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtLowerRange.Name = "TxtLowerRange"
        Me.TxtLowerRange.Size = New System.Drawing.Size(43, 20)
        Me.TxtLowerRange.TabIndex = 65
        Me.TxtLowerRange.Text = "0"
        Me.TxtLowerRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LblBufferDistance
        '
        Me.LblBufferDistance.AutoSize = True
        Me.LblBufferDistance.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblBufferDistance.ForeColor = System.Drawing.Color.Blue
        Me.LblBufferDistance.Location = New System.Drawing.Point(152, 24)
        Me.LblBufferDistance.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LblBufferDistance.Name = "LblBufferDistance"
        Me.LblBufferDistance.Size = New System.Drawing.Size(80, 13)
        Me.LblBufferDistance.TabIndex = 66
        Me.LblBufferDistance.Text = "Buffer Distance"
        '
        'LblUpperRange
        '
        Me.LblUpperRange.AutoSize = True
        Me.LblUpperRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUpperRange.ForeColor = System.Drawing.Color.Blue
        Me.LblUpperRange.Location = New System.Drawing.Point(142, 41)
        Me.LblUpperRange.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LblUpperRange.Name = "LblUpperRange"
        Me.LblUpperRange.Size = New System.Drawing.Size(95, 13)
        Me.LblUpperRange.TabIndex = 67
        Me.LblUpperRange.Text = "Elev Upper Range"
        '
        'LblLowerRange
        '
        Me.LblLowerRange.AutoSize = True
        Me.LblLowerRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLowerRange.ForeColor = System.Drawing.Color.Blue
        Me.LblLowerRange.Location = New System.Drawing.Point(142, 58)
        Me.LblLowerRange.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LblLowerRange.Name = "LblLowerRange"
        Me.LblLowerRange.Size = New System.Drawing.Size(95, 13)
        Me.LblLowerRange.TabIndex = 68
        Me.LblLowerRange.Text = "Elev Lower Range"
        '
        'CmboxDistanceUnit
        '
        Me.CmboxDistanceUnit.FormattingEnabled = True
        Me.CmboxDistanceUnit.Location = New System.Drawing.Point(287, 23)
        Me.CmboxDistanceUnit.Margin = New System.Windows.Forms.Padding(2)
        Me.CmboxDistanceUnit.Name = "CmboxDistanceUnit"
        Me.CmboxDistanceUnit.Size = New System.Drawing.Size(58, 21)
        Me.CmboxDistanceUnit.TabIndex = 69
        '
        'BtnAbout
        '
        Me.BtnAbout.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAbout.Location = New System.Drawing.Point(139, 105)
        Me.BtnAbout.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnAbout.Name = "BtnAbout"
        Me.BtnAbout.Size = New System.Drawing.Size(55, 22)
        Me.BtnAbout.TabIndex = 70
        Me.BtnAbout.Text = "About"
        Me.BtnAbout.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(4, 77)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(95, 20)
        Me.Label2.TabIndex = 71
        Me.Label2.Text = "Report Title:"
        '
        'TxtReportTitle
        '
        Me.TxtReportTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtReportTitle.Location = New System.Drawing.Point(105, 79)
        Me.TxtReportTitle.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtReportTitle.Name = "TxtReportTitle"
        Me.TxtReportTitle.Size = New System.Drawing.Size(334, 22)
        Me.TxtReportTitle.TabIndex = 72
        Me.TxtReportTitle.Text = "Comparisons of actual and pseudo site representations"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(56, 342)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(54, 13)
        Me.Label3.TabIndex = 73
        Me.Label3.Text = "Site Tools"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(234, 342)
        Me.Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(78, 13)
        Me.Label4.TabIndex = 74
        Me.Label4.Text = "Scenario Tools"
        '
        'ChkBufferDistance
        '
        Me.ChkBufferDistance.AutoSize = True
        Me.ChkBufferDistance.Checked = True
        Me.ChkBufferDistance.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkBufferDistance.Location = New System.Drawing.Point(126, 25)
        Me.ChkBufferDistance.Margin = New System.Windows.Forms.Padding(2)
        Me.ChkBufferDistance.Name = "ChkBufferDistance"
        Me.ChkBufferDistance.Size = New System.Drawing.Size(15, 14)
        Me.ChkBufferDistance.TabIndex = 75
        Me.ChkBufferDistance.UseVisualStyleBackColor = True
        '
        'ChkUpperRange
        '
        Me.ChkUpperRange.AutoSize = True
        Me.ChkUpperRange.Checked = True
        Me.ChkUpperRange.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkUpperRange.Location = New System.Drawing.Point(126, 41)
        Me.ChkUpperRange.Margin = New System.Windows.Forms.Padding(2)
        Me.ChkUpperRange.Name = "ChkUpperRange"
        Me.ChkUpperRange.Size = New System.Drawing.Size(15, 14)
        Me.ChkUpperRange.TabIndex = 76
        Me.ChkUpperRange.UseVisualStyleBackColor = True
        '
        'ChkLowerRange
        '
        Me.ChkLowerRange.AutoSize = True
        Me.ChkLowerRange.Checked = True
        Me.ChkLowerRange.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkLowerRange.Location = New System.Drawing.Point(126, 59)
        Me.ChkLowerRange.Margin = New System.Windows.Forms.Padding(2)
        Me.ChkLowerRange.Name = "ChkLowerRange"
        Me.ChkLowerRange.Size = New System.Drawing.Size(15, 14)
        Me.ChkLowerRange.TabIndex = 77
        Me.ChkLowerRange.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(369, 342)
        Me.Label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(41, 13)
        Me.Label7.TabIndex = 78
        Me.Label7.Text = "Reload"
        '
        'TxtScenario1
        '
        Me.TxtScenario1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtScenario1.Location = New System.Drawing.Point(101, 159)
        Me.TxtScenario1.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtScenario1.Name = "TxtScenario1"
        Me.TxtScenario1.Size = New System.Drawing.Size(337, 22)
        Me.TxtScenario1.TabIndex = 79
        Me.TxtScenario1.Text = "ACTUAL SITE REPRESENTATION"
        '
        'TxtScenario2
        '
        Me.TxtScenario2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtScenario2.Location = New System.Drawing.Point(98, 430)
        Me.TxtScenario2.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtScenario2.Name = "TxtScenario2"
        Me.TxtScenario2.Size = New System.Drawing.Size(337, 22)
        Me.TxtScenario2.TabIndex = 80
        Me.TxtScenario2.Text = "PSEUDO SITE REPRESENTATION"
        '
        'BtnMaps
        '
        Me.BtnMaps.Enabled = False
        Me.BtnMaps.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnMaps.Location = New System.Drawing.Point(383, 105)
        Me.BtnMaps.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnMaps.Name = "BtnMaps"
        Me.BtnMaps.Size = New System.Drawing.Size(55, 22)
        Me.BtnMaps.TabIndex = 81
        Me.BtnMaps.Text = "Maps"
        Me.BtnMaps.UseVisualStyleBackColor = True
        '
        'BtnAutoPseudo
        '
        Me.BtnAutoPseudo.Image = CType(resources.GetObject("BtnAutoPseudo.Image"), System.Drawing.Image)
        Me.BtnAutoPseudo.Location = New System.Drawing.Point(123, 360)
        Me.BtnAutoPseudo.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnAutoPseudo.Name = "BtnAutoPseudo"
        Me.BtnAutoPseudo.Size = New System.Drawing.Size(35, 35)
        Me.BtnAutoPseudo.TabIndex = 82
        Me.BtnAutoPseudo.UseVisualStyleBackColor = True
        '
        'BtnAutoLog
        '
        Me.BtnAutoLog.Image = CType(resources.GetObject("BtnAutoLog.Image"), System.Drawing.Image)
        Me.BtnAutoLog.Location = New System.Drawing.Point(85, 360)
        Me.BtnAutoLog.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnAutoLog.Name = "BtnAutoLog"
        Me.BtnAutoLog.Size = New System.Drawing.Size(35, 35)
        Me.BtnAutoLog.TabIndex = 83
        Me.BtnAutoLog.UseVisualStyleBackColor = True
        '
        'BtnTables
        '
        Me.BtnTables.Enabled = False
        Me.BtnTables.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTables.Location = New System.Drawing.Point(290, 105)
        Me.BtnTables.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnTables.Name = "BtnTables"
        Me.BtnTables.Size = New System.Drawing.Size(63, 22)
        Me.BtnTables.TabIndex = 84
        Me.BtnTables.Text = "Tables"
        Me.BtnTables.UseVisualStyleBackColor = True
        '
        'BtnTableHelp
        '
        Me.BtnTableHelp.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnTableHelp.ForeColor = System.Drawing.Color.Blue
        Me.BtnTableHelp.Location = New System.Drawing.Point(354, 105)
        Me.BtnTableHelp.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnTableHelp.Name = "BtnTableHelp"
        Me.BtnTableHelp.Size = New System.Drawing.Size(25, 22)
        Me.BtnTableHelp.TabIndex = 85
        Me.BtnTableHelp.Text = "?"
        Me.BtnTableHelp.UseVisualStyleBackColor = True
        '
        'frmSiteScenario
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.BtnTableHelp)
        Me.Controls.Add(Me.BtnTables)
        Me.Controls.Add(Me.BtnAutoLog)
        Me.Controls.Add(Me.BtnAutoPseudo)
        Me.Controls.Add(Me.BtnMaps)
        Me.Controls.Add(Me.TxtScenario2)
        Me.Controls.Add(Me.TxtScenario1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.ChkLowerRange)
        Me.Controls.Add(Me.ChkUpperRange)
        Me.Controls.Add(Me.ChkBufferDistance)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtReportTitle)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.BtnAbout)
        Me.Controls.Add(Me.CmboxDistanceUnit)
        Me.Controls.Add(Me.LblLowerRange)
        Me.Controls.Add(Me.LblUpperRange)
        Me.Controls.Add(Me.LblBufferDistance)
        Me.Controls.Add(Me.TxtLowerRange)
        Me.Controls.Add(Me.TxtUpperRange)
        Me.Controls.Add(Me.TxtBufferDistance)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnReload)
        Me.Controls.Add(Me.BtnViewResult)
        Me.Controls.Add(Me.BtnToggleSel)
        Me.Controls.Add(Me.BtnDeleteSite)
        Me.Controls.Add(Me.BtnAddAll)
        Me.Controls.Add(Me.BtnRemoveAll)
        Me.Controls.Add(Me.BtnPreview)
        Me.Controls.Add(Me.BtnNewSite)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.BtnCalculate)
        Me.Controls.Add(Me.OptZFeet)
        Me.Controls.Add(Me.OptZMeters)
        Me.Controls.Add(Me.LblElevUnit)
        Me.Controls.Add(Me.txtMaxElev)
        Me.Controls.Add(Me.txtMinElev)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.lblElevation)
        Me.Controls.Add(Me.BtnRemoveSite)
        Me.Controls.Add(Me.BtnAddSite)
        Me.Controls.Add(Me.GrdScenario2)
        Me.Controls.Add(Me.GrdScenario1)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmSiteScenario"
        Me.Size = New System.Drawing.Size(471, 612)
        CType(Me.GrdScenario1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GrdScenario2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GrdScenario1 As System.Windows.Forms.DataGridView
    Friend WithEvents GrdScenario2 As System.Windows.Forms.DataGridView
    Friend WithEvents BtnAddSite As System.Windows.Forms.Button
    Friend WithEvents BtnRemoveSite As System.Windows.Forms.Button
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents lblElevation As System.Windows.Forms.Label
    Friend WithEvents txtMaxElev As System.Windows.Forms.TextBox
    Friend WithEvents txtMinElev As System.Windows.Forms.TextBox
    Friend WithEvents OptZFeet As System.Windows.Forms.RadioButton
    Friend WithEvents OptZMeters As System.Windows.Forms.RadioButton
    Friend WithEvents LblElevUnit As System.Windows.Forms.Label
    Friend WithEvents BtnClose As System.Windows.Forms.Button
    Friend WithEvents BtnCalculate As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents BtnNewSite As System.Windows.Forms.Button
    Friend WithEvents BtnPreview As System.Windows.Forms.Button
    Friend WithEvents BtnRemoveAll As System.Windows.Forms.Button
    Friend WithEvents BtnAddAll As System.Windows.Forms.Button
    Friend WithEvents BtnDeleteSite As System.Windows.Forms.Button
    Friend WithEvents BtnToggleSel As System.Windows.Forms.Button
    Friend WithEvents BtnViewResult As System.Windows.Forms.Button
    Friend WithEvents BtnReload As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtBufferDistance As System.Windows.Forms.TextBox
    Friend WithEvents TxtUpperRange As System.Windows.Forms.TextBox
    Friend WithEvents TxtLowerRange As System.Windows.Forms.TextBox
    Friend WithEvents LblBufferDistance As System.Windows.Forms.Label
    Friend WithEvents LblUpperRange As System.Windows.Forms.Label
    Friend WithEvents LblLowerRange As System.Windows.Forms.Label
    Friend WithEvents CmboxDistanceUnit As System.Windows.Forms.ComboBox
    Friend WithEvents BtnAbout As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtReportTitle As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ChkBufferDistance As System.Windows.Forms.CheckBox
    Friend WithEvents ChkUpperRange As System.Windows.Forms.CheckBox
    Friend WithEvents ChkLowerRange As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TxtScenario1 As System.Windows.Forms.TextBox
    Friend WithEvents TxtScenario2 As System.Windows.Forms.TextBox
    Friend WithEvents BtnMaps As System.Windows.Forms.Button
    Friend WithEvents Selected As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ObjectId As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Site_Type As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Site_Name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Elevation As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Upper_Elev As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Lower_Elev As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DefaultElevation As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SObjectId As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ScenarioType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ScenarioName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ScenarioElevation As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ScenarioUpper_Elev As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ScenarioLower_Elev As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SDefaultElevation As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BtnAutoPseudo As System.Windows.Forms.Button
    Friend WithEvents BtnAutoLog As System.Windows.Forms.Button
    Friend WithEvents BtnTables As System.Windows.Forms.Button
    Friend WithEvents BtnTableHelp As System.Windows.Forms.Button

End Class
