﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmPsuedoSite
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LstVectors = New System.Windows.Forms.ListBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtBufferDistance = New System.Windows.Forms.TextBox()
        Me.CkElev = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.CkPrecip = New System.Windows.Forms.CheckBox()
        Me.GrpElevation = New System.Windows.Forms.GroupBox()
        Me.lblElevation = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.txtMinElev = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtMaxElev = New System.Windows.Forms.TextBox()
        Me.txtLower = New System.Windows.Forms.TextBox()
        Me.TxtRange = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtUpperRange = New System.Windows.Forms.TextBox()
        Me.GrpPrecipitation = New System.Windows.Forms.GroupBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.TextBox7 = New System.Windows.Forms.TextBox()
        Me.CmdPrism = New System.Windows.Forms.Button()
        Me.CmboxEnd = New System.Windows.Forms.ComboBox()
        Me.CmboxBegin = New System.Windows.Forms.ComboBox()
        Me.CmboxPrecipType = New System.Windows.Forms.ComboBox()
        Me.lblEndMonth = New System.Windows.Forms.Label()
        Me.lblBeginMonth = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.GrpProximity = New System.Windows.Forms.GroupBox()
        Me.CkProximity = New System.Windows.Forms.CheckBox()
        Me.BtnFindSite = New System.Windows.Forms.Button()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.GrpElevation.SuspendLayout()
        Me.GrpPrecipitation.SuspendLayout()
        Me.GrpProximity.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(9, 18)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(77, 16)
        Me.Label5.TabIndex = 69
        Me.Label5.Text = "Data Layer:"
        '
        'LstVectors
        '
        Me.LstVectors.FormattingEnabled = True
        Me.LstVectors.ItemHeight = 16
        Me.LstVectors.Items.AddRange(New Object() {"FS_Roads", "snotel_sites", "snowcourse_sites"})
        Me.LstVectors.Location = New System.Drawing.Point(12, 40)
        Me.LstVectors.Name = "LstVectors"
        Me.LstVectors.Size = New System.Drawing.Size(197, 52)
        Me.LstVectors.TabIndex = 70
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(215, 42)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(153, 16)
        Me.Label6.TabIndex = 72
        Me.Label6.Text = "Buffer Distance (Meters):"
        '
        'txtBufferDistance
        '
        Me.txtBufferDistance.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBufferDistance.Location = New System.Drawing.Point(372, 42)
        Me.txtBufferDistance.Margin = New System.Windows.Forms.Padding(2)
        Me.txtBufferDistance.Name = "txtBufferDistance"
        Me.txtBufferDistance.Size = New System.Drawing.Size(41, 20)
        Me.txtBufferDistance.TabIndex = 71
        Me.txtBufferDistance.Text = "500"
        Me.txtBufferDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CkElev
        '
        Me.CkElev.AutoSize = True
        Me.CkElev.Checked = True
        Me.CkElev.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkElev.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CkElev.Location = New System.Drawing.Point(24, 49)
        Me.CkElev.Name = "CkElev"
        Me.CkElev.Size = New System.Drawing.Size(15, 14)
        Me.CkElev.TabIndex = 74
        Me.CkElev.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(3, 9)
        Me.Label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(58, 16)
        Me.Label7.TabIndex = 75
        Me.Label7.Text = "Include"
        '
        'CkPrecip
        '
        Me.CkPrecip.AutoSize = True
        Me.CkPrecip.Checked = True
        Me.CkPrecip.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkPrecip.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CkPrecip.Location = New System.Drawing.Point(24, 156)
        Me.CkPrecip.Name = "CkPrecip"
        Me.CkPrecip.Size = New System.Drawing.Size(15, 14)
        Me.CkPrecip.TabIndex = 77
        Me.CkPrecip.UseVisualStyleBackColor = True
        '
        'GrpElevation
        '
        Me.GrpElevation.Controls.Add(Me.lblElevation)
        Me.GrpElevation.Controls.Add(Me.Label23)
        Me.GrpElevation.Controls.Add(Me.Label24)
        Me.GrpElevation.Controls.Add(Me.txtMinElev)
        Me.GrpElevation.Controls.Add(Me.Label1)
        Me.GrpElevation.Controls.Add(Me.Label4)
        Me.GrpElevation.Controls.Add(Me.TxtMaxElev)
        Me.GrpElevation.Controls.Add(Me.txtLower)
        Me.GrpElevation.Controls.Add(Me.TxtRange)
        Me.GrpElevation.Controls.Add(Me.Label3)
        Me.GrpElevation.Controls.Add(Me.Label2)
        Me.GrpElevation.Controls.Add(Me.TxtUpperRange)
        Me.GrpElevation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GrpElevation.Location = New System.Drawing.Point(62, 24)
        Me.GrpElevation.Name = "GrpElevation"
        Me.GrpElevation.Size = New System.Drawing.Size(487, 80)
        Me.GrpElevation.TabIndex = 79
        Me.GrpElevation.TabStop = False
        Me.GrpElevation.Text = "Elevation"
        '
        'lblElevation
        '
        Me.lblElevation.AutoSize = True
        Me.lblElevation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblElevation.Location = New System.Drawing.Point(5, 22)
        Me.lblElevation.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblElevation.Name = "lblElevation"
        Me.lblElevation.Size = New System.Drawing.Size(149, 16)
        Me.lblElevation.TabIndex = 70
        Me.lblElevation.Text = "DEM Elevation (Meters)"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(177, 22)
        Me.Label23.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(32, 16)
        Me.Label23.TabIndex = 69
        Me.Label23.Text = "Min:"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.Location = New System.Drawing.Point(264, 22)
        Me.Label24.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(36, 16)
        Me.Label24.TabIndex = 71
        Me.Label24.Text = "Max:"
        '
        'txtMinElev
        '
        Me.txtMinElev.BackColor = System.Drawing.SystemColors.Menu
        Me.txtMinElev.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtMinElev.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMinElev.ForeColor = System.Drawing.Color.Blue
        Me.txtMinElev.Location = New System.Drawing.Point(213, 22)
        Me.txtMinElev.Margin = New System.Windows.Forms.Padding(2)
        Me.txtMinElev.Name = "txtMinElev"
        Me.txtMinElev.ReadOnly = True
        Me.txtMinElev.Size = New System.Drawing.Size(47, 15)
        Me.txtMinElev.TabIndex = 72
        Me.txtMinElev.Text = "1816"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(358, 22)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(52, 16)
        Me.Label1.TabIndex = 73
        Me.Label1.Text = "Range:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(243, 47)
        Me.Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(47, 16)
        Me.Label4.TabIndex = 80
        Me.Label4.Text = "Lower:"
        '
        'TxtMaxElev
        '
        Me.TxtMaxElev.BackColor = System.Drawing.SystemColors.Menu
        Me.TxtMaxElev.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtMaxElev.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtMaxElev.ForeColor = System.Drawing.Color.Blue
        Me.TxtMaxElev.Location = New System.Drawing.Point(307, 22)
        Me.TxtMaxElev.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtMaxElev.Name = "TxtMaxElev"
        Me.TxtMaxElev.ReadOnly = True
        Me.TxtMaxElev.Size = New System.Drawing.Size(53, 15)
        Me.TxtMaxElev.TabIndex = 74
        Me.TxtMaxElev.Text = "3536"
        '
        'txtLower
        '
        Me.txtLower.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLower.Location = New System.Drawing.Point(292, 45)
        Me.txtLower.Margin = New System.Windows.Forms.Padding(2)
        Me.txtLower.Name = "txtLower"
        Me.txtLower.Size = New System.Drawing.Size(41, 20)
        Me.txtLower.TabIndex = 79
        Me.txtLower.Text = "2000"
        Me.txtLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TxtRange
        '
        Me.TxtRange.BackColor = System.Drawing.SystemColors.Menu
        Me.TxtRange.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtRange.ForeColor = System.Drawing.Color.Blue
        Me.TxtRange.Location = New System.Drawing.Point(411, 22)
        Me.TxtRange.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtRange.Name = "TxtRange"
        Me.TxtRange.ReadOnly = True
        Me.TxtRange.Size = New System.Drawing.Size(52, 15)
        Me.TxtRange.TabIndex = 75
        Me.TxtRange.Text = "2722"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(132, 45)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(49, 16)
        Me.Label3.TabIndex = 78
        Me.Label3.Text = "Upper:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(5, 45)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 16)
        Me.Label2.TabIndex = 76
        Me.Label2.Text = "Desired Range"
        '
        'TxtUpperRange
        '
        Me.TxtUpperRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtUpperRange.Location = New System.Drawing.Point(180, 44)
        Me.TxtUpperRange.Margin = New System.Windows.Forms.Padding(2)
        Me.TxtUpperRange.Name = "TxtUpperRange"
        Me.TxtUpperRange.Size = New System.Drawing.Size(41, 20)
        Me.TxtUpperRange.TabIndex = 77
        Me.TxtUpperRange.Text = "3000"
        Me.TxtUpperRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GrpPrecipitation
        '
        Me.GrpPrecipitation.Controls.Add(Me.Label9)
        Me.GrpPrecipitation.Controls.Add(Me.Label10)
        Me.GrpPrecipitation.Controls.Add(Me.Label11)
        Me.GrpPrecipitation.Controls.Add(Me.TextBox3)
        Me.GrpPrecipitation.Controls.Add(Me.Label12)
        Me.GrpPrecipitation.Controls.Add(Me.Label13)
        Me.GrpPrecipitation.Controls.Add(Me.TextBox4)
        Me.GrpPrecipitation.Controls.Add(Me.TextBox5)
        Me.GrpPrecipitation.Controls.Add(Me.TextBox6)
        Me.GrpPrecipitation.Controls.Add(Me.Label14)
        Me.GrpPrecipitation.Controls.Add(Me.Label15)
        Me.GrpPrecipitation.Controls.Add(Me.TextBox7)
        Me.GrpPrecipitation.Controls.Add(Me.CmdPrism)
        Me.GrpPrecipitation.Controls.Add(Me.CmboxEnd)
        Me.GrpPrecipitation.Controls.Add(Me.CmboxBegin)
        Me.GrpPrecipitation.Controls.Add(Me.CmboxPrecipType)
        Me.GrpPrecipitation.Controls.Add(Me.lblEndMonth)
        Me.GrpPrecipitation.Controls.Add(Me.lblBeginMonth)
        Me.GrpPrecipitation.Controls.Add(Me.Label8)
        Me.GrpPrecipitation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GrpPrecipitation.Location = New System.Drawing.Point(62, 110)
        Me.GrpPrecipitation.Name = "GrpPrecipitation"
        Me.GrpPrecipitation.Size = New System.Drawing.Size(487, 146)
        Me.GrpPrecipitation.TabIndex = 80
        Me.GrpPrecipitation.TabStop = False
        Me.GrpPrecipitation.Text = "Precipitation"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(7, 94)
        Me.Label9.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(132, 16)
        Me.Label9.TabIndex = 82
        Me.Label9.Text = "Precipitation (Inches)"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(179, 94)
        Me.Label10.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(32, 16)
        Me.Label10.TabIndex = 81
        Me.Label10.Text = "Min:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(260, 94)
        Me.Label11.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(36, 16)
        Me.Label11.TabIndex = 83
        Me.Label11.Text = "Max:"
        '
        'TextBox3
        '
        Me.TextBox3.BackColor = System.Drawing.SystemColors.Menu
        Me.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox3.ForeColor = System.Drawing.Color.Blue
        Me.TextBox3.Location = New System.Drawing.Point(215, 94)
        Me.TextBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.ReadOnly = True
        Me.TextBox3.Size = New System.Drawing.Size(38, 15)
        Me.TextBox3.TabIndex = 84
        Me.TextBox3.Text = "18.98"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(345, 94)
        Me.Label12.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(45, 16)
        Me.Label12.TabIndex = 85
        Me.Label12.Text = "Mean:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(245, 119)
        Me.Label13.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(47, 16)
        Me.Label13.TabIndex = 92
        Me.Label13.Text = "Lower:"
        '
        'TextBox4
        '
        Me.TextBox4.BackColor = System.Drawing.SystemColors.Menu
        Me.TextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox4.ForeColor = System.Drawing.Color.Blue
        Me.TextBox4.Location = New System.Drawing.Point(303, 94)
        Me.TextBox4.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.ReadOnly = True
        Me.TextBox4.Size = New System.Drawing.Size(38, 15)
        Me.TextBox4.TabIndex = 86
        Me.TextBox4.Text = "54.29"
        '
        'TextBox5
        '
        Me.TextBox5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox5.Location = New System.Drawing.Point(294, 117)
        Me.TextBox5.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(41, 20)
        Me.TextBox5.TabIndex = 91
        Me.TextBox5.Text = "30.0"
        Me.TextBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox6
        '
        Me.TextBox6.BackColor = System.Drawing.SystemColors.Menu
        Me.TextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox6.ForeColor = System.Drawing.Color.Blue
        Me.TextBox6.Location = New System.Drawing.Point(398, 94)
        Me.TextBox6.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.ReadOnly = True
        Me.TextBox6.Size = New System.Drawing.Size(35, 15)
        Me.TextBox6.TabIndex = 87
        Me.TextBox6.Text = "36.67"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(134, 117)
        Me.Label14.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(49, 16)
        Me.Label14.TabIndex = 90
        Me.Label14.Text = "Upper:"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(7, 117)
        Me.Label15.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(100, 16)
        Me.Label15.TabIndex = 88
        Me.Label15.Text = "Desired Range"
        '
        'TextBox7
        '
        Me.TextBox7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox7.Location = New System.Drawing.Point(182, 116)
        Me.TextBox7.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBox7.Name = "TextBox7"
        Me.TextBox7.Size = New System.Drawing.Size(41, 20)
        Me.TextBox7.TabIndex = 89
        Me.TextBox7.Text = "45.0"
        Me.TextBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CmdPrism
        '
        Me.CmdPrism.Location = New System.Drawing.Point(307, 20)
        Me.CmdPrism.Name = "CmdPrism"
        Me.CmdPrism.Size = New System.Drawing.Size(93, 30)
        Me.CmdPrism.TabIndex = 15
        Me.CmdPrism.Text = "Get Values"
        Me.CmdPrism.UseVisualStyleBackColor = True
        '
        'CmboxEnd
        '
        Me.CmboxEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmboxEnd.Enabled = False
        Me.CmboxEnd.FormattingEnabled = True
        Me.CmboxEnd.Location = New System.Drawing.Point(224, 54)
        Me.CmboxEnd.Name = "CmboxEnd"
        Me.CmboxEnd.Size = New System.Drawing.Size(67, 24)
        Me.CmboxEnd.TabIndex = 12
        '
        'CmboxBegin
        '
        Me.CmboxBegin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmboxBegin.Enabled = False
        Me.CmboxBegin.FormattingEnabled = True
        Me.CmboxBegin.Location = New System.Drawing.Point(96, 54)
        Me.CmboxBegin.Name = "CmboxBegin"
        Me.CmboxBegin.Size = New System.Drawing.Size(67, 24)
        Me.CmboxBegin.TabIndex = 13
        '
        'CmboxPrecipType
        '
        Me.CmboxPrecipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmboxPrecipType.FormattingEnabled = True
        Me.CmboxPrecipType.Location = New System.Drawing.Point(96, 22)
        Me.CmboxPrecipType.Name = "CmboxPrecipType"
        Me.CmboxPrecipType.Size = New System.Drawing.Size(195, 24)
        Me.CmboxPrecipType.TabIndex = 14
        '
        'lblEndMonth
        '
        Me.lblEndMonth.AutoSize = True
        Me.lblEndMonth.Enabled = False
        Me.lblEndMonth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEndMonth.Location = New System.Drawing.Point(191, 59)
        Me.lblEndMonth.Name = "lblEndMonth"
        Me.lblEndMonth.Size = New System.Drawing.Size(28, 16)
        Me.lblEndMonth.TabIndex = 10
        Me.lblEndMonth.Text = "To:"
        '
        'lblBeginMonth
        '
        Me.lblBeginMonth.AutoSize = True
        Me.lblBeginMonth.Enabled = False
        Me.lblBeginMonth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBeginMonth.Location = New System.Drawing.Point(48, 59)
        Me.lblBeginMonth.Name = "lblBeginMonth"
        Me.lblBeginMonth.Size = New System.Drawing.Size(42, 16)
        Me.lblBeginMonth.TabIndex = 9
        Me.lblBeginMonth.Text = "From:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(6, 24)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(82, 16)
        Me.Label8.TabIndex = 11
        Me.Label8.Text = "PRISM Data"
        '
        'GrpProximity
        '
        Me.GrpProximity.Controls.Add(Me.Label5)
        Me.GrpProximity.Controls.Add(Me.LstVectors)
        Me.GrpProximity.Controls.Add(Me.txtBufferDistance)
        Me.GrpProximity.Controls.Add(Me.Label6)
        Me.GrpProximity.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GrpProximity.Location = New System.Drawing.Point(62, 271)
        Me.GrpProximity.Name = "GrpProximity"
        Me.GrpProximity.Size = New System.Drawing.Size(487, 100)
        Me.GrpProximity.TabIndex = 81
        Me.GrpProximity.TabStop = False
        Me.GrpProximity.Text = "Proximity"
        '
        'CkProximity
        '
        Me.CkProximity.AutoSize = True
        Me.CkProximity.Checked = True
        Me.CkProximity.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkProximity.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CkProximity.Location = New System.Drawing.Point(24, 300)
        Me.CkProximity.Name = "CkProximity"
        Me.CkProximity.Size = New System.Drawing.Size(15, 14)
        Me.CkProximity.TabIndex = 82
        Me.CkProximity.UseVisualStyleBackColor = True
        '
        'BtnFindSite
        '
        Me.BtnFindSite.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnFindSite.Location = New System.Drawing.Point(337, 385)
        Me.BtnFindSite.Name = "BtnFindSite"
        Me.BtnFindSite.Size = New System.Drawing.Size(93, 23)
        Me.BtnFindSite.TabIndex = 83
        Me.BtnFindSite.Text = "Find Site"
        Me.BtnFindSite.UseVisualStyleBackColor = True
        '
        'BtnClose
        '
        Me.BtnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.Location = New System.Drawing.Point(446, 386)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(2)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(55, 22)
        Me.BtnClose.TabIndex = 73
        Me.BtnClose.Text = "Close"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'FrmPsuedoSite
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(561, 422)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.BtnFindSite)
        Me.Controls.Add(Me.CkProximity)
        Me.Controls.Add(Me.GrpProximity)
        Me.Controls.Add(Me.GrpPrecipitation)
        Me.Controls.Add(Me.GrpElevation)
        Me.Controls.Add(Me.CkPrecip)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.CkElev)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "FrmPsuedoSite"
        Me.ShowIcon = False
        Me.Text = "Add Pseudo Site: animas_AOI_prms_3"
        Me.GrpElevation.ResumeLayout(False)
        Me.GrpElevation.PerformLayout()
        Me.GrpPrecipitation.ResumeLayout(False)
        Me.GrpPrecipitation.PerformLayout()
        Me.GrpProximity.ResumeLayout(False)
        Me.GrpProximity.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LstVectors As System.Windows.Forms.ListBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtBufferDistance As System.Windows.Forms.TextBox
    Friend WithEvents CkElev As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents CkPrecip As System.Windows.Forms.CheckBox
    Friend WithEvents GrpElevation As System.Windows.Forms.GroupBox
    Friend WithEvents lblElevation As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents txtMinElev As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtMaxElev As System.Windows.Forms.TextBox
    Friend WithEvents txtLower As System.Windows.Forms.TextBox
    Friend WithEvents TxtRange As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtUpperRange As System.Windows.Forms.TextBox
    Friend WithEvents GrpPrecipitation As System.Windows.Forms.GroupBox
    Friend WithEvents CmboxEnd As System.Windows.Forms.ComboBox
    Friend WithEvents CmboxBegin As System.Windows.Forms.ComboBox
    Friend WithEvents CmboxPrecipType As System.Windows.Forms.ComboBox
    Friend WithEvents lblEndMonth As System.Windows.Forms.Label
    Friend WithEvents lblBeginMonth As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents TextBox7 As System.Windows.Forms.TextBox
    Friend WithEvents CmdPrism As System.Windows.Forms.Button
    Friend WithEvents GrpProximity As System.Windows.Forms.GroupBox
    Friend WithEvents CkProximity As System.Windows.Forms.CheckBox
    Friend WithEvents BtnFindSite As System.Windows.Forms.Button
    Friend WithEvents BtnClose As System.Windows.Forms.Button
End Class
