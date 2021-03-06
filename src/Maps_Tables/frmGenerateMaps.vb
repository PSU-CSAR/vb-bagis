﻿Imports BAGIS_ClassLibrary
Imports System.IO
Imports System.Windows.Forms
Imports System.Text
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.SpatialAnalyst
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Desktop.AddIns
Imports Microsoft.Office.Interop.Excel
Imports ESRI.ArcGIS.GeoAnalyst
Imports ESRI.ArcGIS.Carto

Public Class frmGenerateMaps

    'silence the message
    Private Set_Silent_Mode As Boolean = True
    'flag to control the execution of analysis
    Private Flag_ElevationZone As Boolean
    Private Flag_PrecipitationZone As Boolean
    Private Flag_BasinTables As Boolean
    Private Flag_BasinMaps As Boolean
    Private Flag_ElevOrPrecipChange As Boolean
    'variables
    Private Elev_Interval As Integer
    Private Elev_Subdivision As Integer

    Private AnalysisPath As String
    Private PRISMPath As String
    Private ElvPath As String
    Private PrecipPath As String
    Private PRISMRasterName As String
    Private m_demInMeters As Boolean
    Private m_formInit As Boolean = False

    'partition raster variables from FrmElevPrecip
    Private m_partitionRasterPath As String
    Private m_zoneRasterPath As String

    Friend Const PARTITION_MODE As String = "PARTITION"
    Friend Const ZONE_MODE As String = "ZONE"


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim filepath As String, FileName As String
        Dim response As Integer

        If Len(Trim(AOIFolderBase)) = 0 Then
            MsgBox("AOI hasn't been specified! Please use the AOI Tools to set target AOI.")
            Me.Close()
        End If

        'Initialize the controls on the form
        InitForm()

        'read dem min, max everytime the form is activated
        'display dem elevation stats
        Dim pRasterStats As IRasterStatistics = BA_GetDemStatsGDB(AOIFolderBase)

        Dim DataConversion_Factor As Double
        Dim DisplayConversion_Factor As Double

        'convert unit to internal system unit, i.e., meters
        Dim elevUnit As MeasurementUnit = BA_GetElevationUnitsForAOI(AOIFolderBase)
        If elevUnit = MeasurementUnit.Missing Then
            Dim aoiName As String = BA_GetBareName(AOIFolderBase)
            Dim pAoi As Aoi = New Aoi(aoiName, AOIFolderBase, "", "")
            Dim frmDataUnits As FrmDataUnits = New FrmDataUnits(pAoi)
            If frmDataUnits.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                elevUnit = frmDataUnits.NewElevationUnit
            Else
                'Set elevUnit to system default and warn the user
                elevUnit = MeasurementUnit.Meters
                Dim sb As StringBuilder = New StringBuilder
                sb.Append("You did not define the elevation units for this" & vbCrLf)
                sb.Append("AOI. BAGIS assumes the elevation units are" & vbCrLf)
                sb.Append(BA_EnumDescription(MeasurementUnit.Meters) & ". If the elevation units are not " & BA_EnumDescription(MeasurementUnit.Meters) & vbCrLf)
                sb.Append("the results will be incorrect." & vbCrLf)
                MessageBox.Show(sb.ToString, "Elevation units not defined", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If
        If elevUnit = MeasurementUnit.Meters Then
            m_demInMeters = True
        End If
        DataConversion_Factor = BA_SetConversionFactor(True, m_demInMeters)
        AOI_DEMMin = Math.Round(pRasterStats.Minimum * DataConversion_Factor - 0.005, 2)
        AOI_DEMMax = Math.Round(pRasterStats.Maximum * DataConversion_Factor + 0.005, 2)

        'Get AOI area
        Dim aoiGdbPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True)
        Dim AOIArea As Double = BA_GetShapeAreaGDB(aoiGdbPath & BA_AOIExtentCoverage) / 1000000 'the shape unit is in sq meters, converted to sq km
        txtArea.Text = Format(AOIArea, "#0.00")
        txtAreaAcre.Text = Format(AOIArea * 247.1044, "#0.00")
        txtAreaSQMile.Text = Format(AOIArea * 0.3861022, "#0.00")

        'check to see if maps folder exists
        filepath = BA_GetPath(AOIFolderBase, PublicPath.Maps)
        If Not BA_Folder_ExistsWindowsIO(filepath) Then
            Dim newFolder As String = BA_CreateFolder(AOIFolderBase, BA_GetBareName(BA_EnumDescription(PublicPath.Maps)))
            If String.IsNullOrEmpty(newFolder) Then
                MessageBox.Show("Could not create maps folder in AOI. The Generate Maps screen is unavailable.",
                                "Failed to create folder", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If

        'check if map_parameters.txt file exists
        Set_Silent_Mode = True
        FileName = BA_MapParameterFile 'i.e., map_parameters.txt
        response = ReadMapParameters(filepath, FileName)

        Dim rvalues_arr() As BA_IntervalList = Nothing
        Dim subrvalues_arr() As BA_IntervalList = Nothing

        Dim ninterval As Integer
        Dim nsubinterval As Integer
        If response < 1 Then 'cannot read the parameter file correctly
            If m_demInMeters = True Then
                OptZMeters.Checked = True
                lblElevUnit.Text = "Elevation (m):"
            Else
                OptZFeet.Checked = True
                lblElevUnit.Text = "Elevation (ft):"
            End If

            'Populate Boxes
            DisplayConversion_Factor = BA_SetConversionFactor(OptZMeters.Checked, True)

            txtMinElev.Text = Math.Round(AOI_DEMMin * DisplayConversion_Factor - 0.005, 2)  'adjust value to include the actual min, max
            txtMaxElev.Text = Math.Round(AOI_DEMMax * DisplayConversion_Factor + 0.005, 2)
            txtRangeElev.Text = Val(txtMaxElev.Text) - Val(txtMinElev.Text)

            Elev_Interval = Val(CmboxElevInterval.SelectedItem)
            Elev_Subdivision = Val(ComboxSubDivide.SelectedItem)

            ninterval = BA_CreateRangeArray(Val(txtMinElev.Text), Val(txtMaxElev.Text), Elev_Interval, rvalues_arr)
            txtElevClassNumber.Text = ninterval
            Display_IntervalList(rvalues_arr)
            nsubinterval = Subdivide_IntervalList(rvalues_arr, Elev_Interval,
                subrvalues_arr, Elev_Subdivision)
            Display_ElevationRange(rvalues_arr)
        Else
            'Populate elevation Boxes
            DisplayConversion_Factor = BA_SetConversionFactor(OptZMeters.Checked, True)
            txtMinElev.Text = Math.Round(AOI_DEMMin * DisplayConversion_Factor - 0.005, 2)  'adjust value to include the actual min, max
            txtMaxElev.Text = Math.Round(AOI_DEMMax * DisplayConversion_Factor + 0.005, 2)
            txtRangeElev.Text = Val(txtMaxElev.Text) - Val(txtMinElev.Text)
            Elev_Interval = Val(CmboxElevInterval.SelectedItem)
            Elev_Subdivision = Val(ComboxSubDivide.SelectedItem)

            ninterval = BA_CreateRangeArray(Val(txtMinElev.Text), Val(txtMaxElev.Text), Elev_Interval, rvalues_arr)
            'txtElevClassNumber.Text = ninterval
            nsubinterval = Subdivide_IntervalList(rvalues_arr, Elev_Interval,
                subrvalues_arr, Elev_Subdivision)
            Display_ElevationRange(rvalues_arr)
            'Check for existence of elev-precip AOI table; If it doesn't exist, disable elevation-precip tool
            'This isn't perfect but we will assume if the maps config file was saved and the table doesn't exist,
            'Then the previous run didn't include elev-precip
            If Not BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis) + "\" _
                      + BA_TablePrecMeanElev, WorkspaceType.Geodatabase, esriDatasetType.esriDTTable) Then
                ChkRepresentedPrecip.Checked = False
            End If
        End If

        If AOI_DEMMax > 30000 Then 'elevation range value error
            MsgBox("DEM elevation value out of normal bound! Please check the DEM data.")
            Me.Close()
        End If

        AnalysisPath = BA_GetPath(AOIFolderBase, PublicPath.Analysis)
        PRISMPath = AnalysisPath & "\" & BA_RasterPrecipitationZones
        ElvPath = AnalysisPath & "\" & BA_RasterElevationZones

        Display_DataStatus()

        m_formInit = True
        Set_Silent_Mode = False
    End Sub

    Private Sub CmbClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbClose.Click
        Me.Close()
    End Sub

    Private Function ReadMapParameters(ByVal filepath As String, ByVal FileName As String) As Integer

        Dim filepathname As String
        Dim linestring As String
        Dim errormessage As String
        Dim listcount As Integer
        Dim listitem(0 To 3) As String
        Dim i As Integer, j As Integer
        Dim position As Integer

        If Len(Trim(filepath)) * Len(Trim(FileName)) = 0 Then Return -1

        filepathname = filepath & "\" & FileName

        Dim sr As StreamReader = Nothing


        Try
            'open file for input
            If BA_File_ExistsWindowsIO(filepathname) Then
                sr = File.OpenText(filepathname)
            Else
                'MsgBox("File " & filepathname & " does not exist and cannot be opened.")
                Return -1
            End If

            'read the version text
            linestring = sr.ReadLine

            'check version
            If Trim(linestring) <> BA_VersionText And Trim(linestring) <> BA_CompatibleVersion1Text Then
                sr.Close()
                errormessage = "The map parameter file's version doesn't match the version of the model!" & vbCrLf & "Please delete or rename the file and restart the model" & vbCrLf
                errormessage = errormessage & FileName
                MsgBox(errormessage)
                Return -1
            End If

            'read the map unit text
            linestring = sr.ReadLine
            If Trim(linestring) = "True" Then
                OptZMeters.Checked = True
                Map_Display_Elevation_in_Meters = True
            Else
                OptZFeet.Checked = True
                Map_Display_Elevation_in_Meters = False
            End If

            'prepare the elevation interval list
            linestring = sr.ReadLine
            CmboxElevInterval.SelectedIndex = Val(Trim(linestring))
            linestring = sr.ReadLine
            listcount = Val(Trim(linestring))
            txtElevClassNumber.Text = listcount
            lstintervals.Items.Clear()

            If listcount > 0 Then
                For i = 0 To listcount - 1
                    linestring = sr.ReadLine  'comma delimited
                    j = 0
                    Do While Len(linestring) > 0
                        position = InStr(1, linestring, ",", vbTextCompare)
                        If position > 0 Then
                            listitem(j) = Microsoft.VisualBasic.Left(linestring, position - 1)
                            linestring = Microsoft.VisualBasic.Right(linestring, Len(linestring) - position)
                        Else
                            listitem(j) = linestring
                            linestring = ""
                        End If
                        j = j + 1
                    Loop

                    With lstintervals
                        Dim pitem As New ListViewItem(listitem(0))
                        pitem.SubItems.Add(listitem(1))
                        pitem.SubItems.Add(listitem(2))
                        pitem.SubItems.Add(listitem(3))
                        .Items.Add(pitem)
                    End With
                Next
                'disable the apply button if the zone raster has been created
                cmdApplyElevInterval.Enabled = False
            Else
                'enable the apply button
                cmdApplyElevInterval.Enabled = True
            End If

            'prepare the PRISM list
            linestring = sr.ReadLine
            CmboxPrecipType.SelectedIndex = Val(linestring)
            linestring = sr.ReadLine
            CmboxBegin.SelectedIndex = Val(linestring)
            linestring = sr.ReadLine
            CmboxEnd.SelectedIndex = Val(linestring)
            linestring = sr.ReadLine
            txtMinPrecip.Text = Val(linestring)
            linestring = sr.ReadLine
            txtMaxPrecip.Text = Val(linestring)
            linestring = sr.ReadLine
            txtRangePrecip.Text = Val(linestring)

            linestring = sr.ReadLine
            If Val(linestring) <= 0 Then
                txtPrecipMapZoneInt.Text = 1
            Else
                txtPrecipMapZoneInt.Text = Val(linestring)
            End If

            linestring = sr.ReadLine
            txtPrecipMapZoneNo.Text = Val(linestring)

            'lstPrecipZones list box
            linestring = sr.ReadLine
            listcount = Val(Trim(linestring))
            lstPrecipZones.Items.Clear()
            If listcount > 0 Then
                For i = 1 To listcount
                    linestring = sr.ReadLine
                    lstPrecipZones.Items.Add(Trim(linestring))
                Next
                'disable the apply button if the zone raster has been created
                cmdPRISM.Enabled = False
                cmdApplyPRISMInterval.Enabled = False
            Else
                'enable the apply button
                cmdPRISM.Enabled = True
                cmdApplyPRISMInterval.Enabled = False
            End If

            linestring = sr.ReadLine 'number of subdivision
            ComboxSubDivide.SelectedIndex = Val(linestring) - 1     'Backwards compatibility; First index was one in VBA

            linestring = sr.ReadLine 'whether subrange analysis
            chkUseRange.Checked = linestring

            linestring = sr.ReadLine 'from elevation
            txtFromElev.Text = linestring

            linestring = sr.ReadLine 'to elevation
            txtToElev.Text = linestring

            If sr.Peek > -1 Then 'check if additional parameters were added after BAGIS Ver 1. Aspect was added in version 2
                linestring = sr.ReadLine 'skip the REVISION text
                linestring = sr.ReadLine 'aspect
                Dim tokenstring() As String = linestring.Split(New Char(), " "c)
                If tokenstring(0).ToUpper = "ASPECT" Then
                    Select Case tokenstring(1)
                        Case "4"
                            CmboxAspect.SelectedIndex = 0
                        Case "8"
                            CmboxAspect.SelectedIndex = 1
                        Case Else
                            CmboxAspect.SelectedIndex = 2
                    End Select
                Else
                    CmboxAspect.SelectedIndex = 2 'default value is set to 16 aspect classes
                End If
            Else
                CmboxAspect.SelectedIndex = 2 'default value is set to 16 aspect classes
            End If

            If sr.Peek > -1 Then 'check to see if we were partway through an analysis and need to set generate to true
                linestring = sr.ReadLine
                Dim tokenstring() As String = linestring.Split(New Char(), " "c)
                If tokenstring(0).ToUpper = "ENABLE_GENERATE" Then
                    If tokenstring(1) = "True" Then
                        Flag_BasinMaps = False
                        Flag_BasinTables = False
                        Flag_ElevOrPrecipChange = True
                    Else
                        Flag_BasinMaps = True
                        Flag_BasinTables = True
                        Flag_ElevOrPrecipChange = False
                    End If
                End If
            End If

            'Flag_PrecipitationZone = True
            'Flag_ElevationZone = True
            CmdGenerate.Enabled = False
            Return 1
        Catch ex As Exception
            Debug.Print("ReadMapParameters Exception: " & ex.Message)
            Return -1
        Finally
            'Don't forget to close the file handle
            If sr IsNot Nothing Then sr.Close()
        End Try
    End Function

    Private Function SaveMapParameters(filepath As String, FileName As String) As Integer
        Dim filepathname As String
        Dim i As Integer
        Dim tempstring As String
        Dim listcount As Integer

        If Len(Trim(filepath)) * Len(Trim(FileName)) = 0 Then Return -1

        filepathname = filepath & "\" & FileName
        Dim sw As StreamWriter = Nothing

        Try
            'open file for output
            sw = New StreamWriter(filepathname)
            sw.WriteLine(BA_VersionText)
            'map unit
            sw.WriteLine(OptZMeters.Checked)
            'elevation dist frame
            sw.WriteLine(CmboxElevInterval.SelectedIndex)
            listcount = lstintervals.Items.Count
            sw.WriteLine(listcount)

            'lstintervals list box
            If listcount > 0 Then
                For i = 0 To listcount - 1
                    With lstintervals
                        Dim pItem As ListViewItem = .Items(i)
                        tempstring = pItem.Text & "," & pItem.SubItems(1).Text & "," & pItem.SubItems(2).Text & "," & pItem.SubItems(3).Text
                        sw.WriteLine(tempstring)  'comma delimited
                    End With
                Next
            End If

            'precipitation dist frame
            sw.WriteLine(CmboxPrecipType.SelectedIndex)
            sw.WriteLine(CmboxBegin.SelectedIndex)
            sw.WriteLine(CmboxEnd.SelectedIndex)
            sw.WriteLine(txtMinPrecip.Text)
            sw.WriteLine(txtMaxPrecip.Text)
            sw.WriteLine(txtRangePrecip.Text)
            sw.WriteLine(txtPrecipMapZoneInt.Text)
            sw.WriteLine(txtPrecipMapZoneNo.Text)

            listcount = lstPrecipZones.Items.Count
            sw.WriteLine(listcount)

            'lstPrecipZones list box
            If listcount > 0 Then
                For i = 0 To listcount - 1
                    sw.WriteLine(lstPrecipZones.Items(i))
                Next
            End If

            sw.WriteLine(ComboxSubDivide.SelectedIndex + 1) 'number of subdivision 1 to 5; Backwards compatibility
            sw.WriteLine(chkUseRange.Checked) 'whether user enable the subrange analysis
            sw.WriteLine(txtFromElev.Text)  'from elevation
            sw.WriteLine(txtToElev.Text) 'to elevation

            'BAGIS V2 new parameter for aspect class
            sw.WriteLine("REVISION for BAGIS 2")
            sw.WriteLine("ASPECT " & 2 ^ (CmboxAspect.SelectedIndex + 2))
            sw.WriteLine("ENABLE_GENERATE " & Flag_ElevOrPrecipChange)

            sw.Flush()
            Return 1
        Catch ex As Exception
            Debug.Print("SaveMapParameters Exception: " & ex.Message)
            Return -1
        Finally
            If sw IsNot Nothing Then
                sw.Close()
            End If
        End Try
    End Function

    'This subroutine updates only the Elevation zone information in the map parameter file
    'return value:
    ' 0: fail
    ' 1: success
    Public Function UpdateMapParameters(ByVal filepath As String, ByVal FileName As String) As Integer
        Dim filepathname As String
        Dim linearray() As String
        Dim i As Integer
        Dim linestring As String, tempstring As String
        Dim listcount As Integer
        Dim nlines As Long, oldVersonLines As Long

        If Len(Trim(filepath)) * Len(Trim(FileName)) = 0 Then Return -1
        filepathname = filepath & "\" & FileName
        Dim sr As StreamReader = Nothing
        Dim sw As StreamWriter = Nothing

        Try
            'read parameters into memory
            'open file for input
            sr = New StreamReader(filepathname)
            linestring = sr.ReadLine 'read the version text

            'check version
            If Trim(linestring) <> BA_VersionText And Trim(linestring) <> BA_CompatibleVersion1Text Then
                sr.Close()
                'Delete the text file: map_parameters.txt
                BA_Remove_File(filepathname)
                Return 0
            End If

            nlines = 1
            oldVersonLines = 1
            Dim endofOldVersion As Boolean = False
            Do While linestring IsNot Nothing
                linestring = sr.ReadLine
                If linestring.Length > 8 Then
                    If linestring.Substring(0, 8) = "REVISION" Then
                        endofOldVersion = True
                    End If
                End If
                nlines = nlines + 1 'count the number of lines in the file
                If Not endofOldVersion Then oldVersonLines = oldVersonLines + 1
            Loop

            sr.Close() 'close the file and reopen to read
            sr = New StreamReader(filepathname)

            ReDim linearray(0 To nlines)
            For i = 0 To nlines - 1
                linestring = sr.ReadLine
                linearray(i) = linestring 'read the whole parameter file
            Next

            sr.Close() 'close the file and reopen for output

            'open file for output
            sw = New StreamWriter(filepathname)

            sw.WriteLine(linearray(0))
            sw.WriteLine(OptZMeters.Checked)

            'elevation dist frame
            Dim OldListCount As Long = Val(linearray(3))
            listcount = lstintervals.Items.Count
            sw.WriteLine(CmboxElevInterval.SelectedIndex)
            sw.WriteLine(listcount)

            'lstintervals list box
            If listcount > 0 Then
                For i = 0 To listcount - 1
                    With lstintervals
                        Dim pItem As ListViewItem = .Items(i)
                        tempstring = pItem.Text & "," & pItem.SubItems(1).Text & "," & pItem.SubItems(2).Text & "," & pItem.SubItems(3).Text
                        sw.WriteLine(tempstring)  'comma delimited
                    End With
                Next
            End If

            For i = OldListCount + 4 To oldVersonLines - 6
                sw.WriteLine(linearray(i))
            Next

            sw.WriteLine(ComboxSubDivide.SelectedIndex + 1) 'number of subdivision 1 to 5

            If chkUseRange.Checked = True Then
                'last 3 lines are for user-specified range analysis
                sw.WriteLine(chkUseRange.Checked) 'whether user enable the subrange analysis
                sw.WriteLine(txtFromElev.Text) 'from elevation
                sw.WriteLine(txtToElev.Text) 'to elevation
            Else
                sw.WriteLine(linearray(oldVersonLines - 4))
                sw.WriteLine(linearray(oldVersonLines - 3))
                sw.WriteLine(linearray(oldVersonLines - 2))
            End If

            If oldVersonLines < nlines Then
                For i = oldVersonLines To nlines
                    sw.WriteLine(linearray(i - 1))
                Next
            End If

            Return 1
        Catch ex As Exception
            Debug.Print("UpdateMapParameters Exception " & ex.Message)
            Return 0
        Finally
            'Close the file(s)
            If sw IsNot Nothing Then
                sw.Close()
            End If
            If sr IsNot Nothing Then
                sr.Close()
            End If
        End Try
    End Function

    'This subroutine updates only the Precipitation zone information in the map parameter file
    'return value:
    ' 0: fail
    ' 1: success
    Private Function UpdateMapParametersPRISM(ByVal filepath As String, ByVal FileName As String) As Integer
        Dim filepathname As String
        Dim linearray() As String
        Dim i As Integer
        Dim linestring As String
        Dim listcount As Integer
        Dim nlines As Long, oldVersonLines As Long

        If Len(Trim(filepath)) * Len(Trim(FileName)) = 0 Then Return 0
        Dim sr As StreamReader = Nothing
        Dim sw As StreamWriter = Nothing

        filepathname = filepath & "\" & FileName
        Try
            'read parameters into memory
            'open file for input
            sr = New StreamReader(filepathname)
            linestring = sr.ReadLine 'read the version text

            'check version
            If Trim(linestring) <> BA_VersionText And Trim(linestring) <> BA_CompatibleVersion1Text Then
                sr.Close()
                'Delete the text file: map_parameters.txt
                BA_Remove_File(filepathname)
                Return 0
            End If

            nlines = 1
            oldVersonLines = 1
            Dim endofOldVersion As Boolean = False
            linestring = sr.ReadLine
            Do While linestring IsNot Nothing
                If linestring.Length > 8 Then
                    If linestring.Substring(0, 8) = "REVISION" Then
                        endofOldVersion = True
                    End If
                End If
                nlines = nlines + 1 'count the number of lines in the file
                If Not endofOldVersion Then oldVersonLines = oldVersonLines + 1
                linestring = sr.ReadLine
            Loop
            sr.Close() 'close the file and reopen to read

            ReDim linearray(0 To nlines)

            'Open filepathname For Input
            sr = New StreamReader(filepathname)

            For i = 0 To nlines - 1
                linestring = sr.ReadLine
                linearray(i) = linestring 'read the whole parameter file
            Next
            sr.Close() 'close the file and reopen for output

            'open file for output
            sw = New StreamWriter(filepathname)
            'reproduce the first three lines
            For i = 0 To 2
                sw.WriteLine(linearray(i))
            Next

            'reproduce elevation dist frame
            listcount = Val(linearray(3))
            For i = 0 To listcount
                sw.WriteLine(linearray(i + 3))
            Next

            'update the precipitation settings and frame
            sw.WriteLine(CmboxPrecipType.SelectedIndex)
            sw.WriteLine(CmboxBegin.SelectedIndex)
            sw.WriteLine(CmboxEnd.SelectedIndex)
            sw.WriteLine(txtMinPrecip.Text)
            sw.WriteLine(txtMaxPrecip.Text)
            sw.WriteLine(txtRangePrecip.Text)
            sw.WriteLine(txtPrecipMapZoneInt.Text)
            sw.WriteLine(txtPrecipMapZoneNo.Text)

            listcount = lstPrecipZones.Items.Count
            sw.WriteLine(listcount)

            'lstPrecipZones list box
            If listcount > 0 Then
                For i = 0 To listcount - 1
                    sw.WriteLine(lstPrecipZones.Items(i))
                Next
            End If

            'last 4 lines are for user-specified range analysis
            sw.WriteLine(linearray(oldVersonLines - 5)) 'subdivision number
            sw.WriteLine(linearray(oldVersonLines - 4)) 'whether range analysis activated
            sw.WriteLine(linearray(oldVersonLines - 3)) 'from elev
            sw.WriteLine(linearray(oldVersonLines - 2)) 'to elev

            If oldVersonLines < nlines Then
                For i = oldVersonLines To nlines
                    sw.WriteLine(linearray(i - 1))
                Next
            End If

            sw.Close()    'Close the file
            Return 1
        Catch ex As Exception
            Debug.Print("UpdateMapParametersPRISM Exception: " & ex.Message)
            Return 0
        Finally
            If sr IsNot Nothing Then
                sr.Close()
            End If
            If sw IsNot Nothing Then
                sw.Close()
            End If
        End Try
    End Function

    Private Function Subdivide_IntervalList(ByVal In_List() As BA_IntervalList, ByVal In_Interval As Integer,
                                            ByRef Out_List() As BA_IntervalList, ByVal SubdivideNo As Integer) As Long
        Dim new_interval As Double
        Dim ninterval As Long
        Dim minval As Double
        Dim maxval As Double
        Dim i As Long

        ninterval = UBound(In_List)

        If SubdivideNo = 1 Then  'duplicate the interval list
            ReDim Out_List(0 To ninterval)

            For i = 1 To ninterval
                Out_List(i).Value = In_List(i).Value
                Out_List(i).LowerBound = In_List(i).LowerBound
                Out_List(i).UpperBound = In_List(i).UpperBound
                Out_List(i).Name = In_List(i).Name
            Next

            Subdivide_IntervalList = ninterval
            Exit Function
        End If

        new_interval = Math.Round(CDbl(In_Interval / SubdivideNo), 4)
        minval = In_List(1).LowerBound
        maxval = In_List(ninterval).UpperBound

        Subdivide_IntervalList = BA_CreateRangeArray(minval, maxval, new_interval, Out_List)

        'round the out_list values
        For i = 1 To Subdivide_IntervalList
            Out_List(i).LowerBound = Math.Round(Out_List(i).LowerBound)
            Out_List(i).UpperBound = Math.Round(Out_List(i).UpperBound)
        Next

        Out_List(1).LowerBound = minval
        Out_List(Subdivide_IntervalList).UpperBound = maxval
    End Function

    Private Sub Display_IntervalList(ByVal Interval_List() As BA_IntervalList)
        Dim i As Long
        Dim ninterval As Long = UBound(Interval_List)

        lstintervals.Items.Clear()

        Try
            Dim rangestring As String
            Dim pcntstring As String
            If ninterval = 0 Then
                Exit Sub    'no class interval
            Else
                For i = 0 To ninterval - 1
                    rangestring = Interval_List(i + 1).Name

                    'align the percentage number on the decimal point
                    pcntstring = Format(Interval_List(i + 1).Area, "000.00")
                    If Microsoft.VisualBasic.Left(pcntstring, 1) = "0" Then pcntstring = " " & Microsoft.VisualBasic.Right(pcntstring, Len(pcntstring) - 1)
                    If Mid(pcntstring, 2, 1) = "0" And Microsoft.VisualBasic.Left(pcntstring, 1) <> "1" Then pcntstring = "  " & Microsoft.VisualBasic.Right(pcntstring, Len(pcntstring) - 2)

                    With lstintervals
                        Dim pitem As New ListViewItem(rangestring)
                        If Not String.IsNullOrEmpty(pcntstring) Then
                            pitem.SubItems.Add(pcntstring & " %")
                        Else
                            pitem.SubItems.Add("?")
                        End If
                        If Not String.IsNullOrEmpty(Interval_List(i + 1).SNOTEL) Then
                            pitem.SubItems.Add(Interval_List(i + 1).SNOTEL)
                        Else
                            pitem.SubItems.Add("?")
                        End If
                        If Not String.IsNullOrEmpty(Interval_List(i + 1).SnowCourse) Then
                            pitem.SubItems.Add(Interval_List(i + 1).SnowCourse)
                        Else
                            pitem.SubItems.Add("?")
                        End If
                        .Items.Add(pitem)
                    End With
                Next
            End If

        Catch ex As Exception
            Debug.Print("Display_IntervalList Exception: " & ex.Message)
        End Try
    End Sub

    'BA_IntervalList array is 1-based
    Private Sub Display_PreciptIntervalList(ByVal Interval_List() As BA_IntervalList)
        Dim i As Long
        Dim ninterval As Long = UBound(Interval_List)

        lstPrecipZones.Items.Clear()

        Dim rangestring As String

        If ninterval = 0 Then
            Exit Sub    'no class interval
        Else
            For i = 0 To ninterval - 1
                rangestring = Interval_List(i + 1).Name
                lstPrecipZones.Items.Add(rangestring)
            Next
        End If
    End Sub

    Private Sub Display_ElevationRange(ByVal Interval_List() As BA_IntervalList)
        Dim i As Long
        Dim ninterval As Long

        ninterval = UBound(Interval_List)
        If ninterval <= 1 Then
            Exit Sub    'no class interval
        Else
            lstElevRange.Items.Clear()
            For i = 0 To ninterval - 1
                lstElevRange.Items.Add(Interval_List(i + 1).LowerBound)
            Next
            lstElevRange.Items.Add(Interval_List(ninterval).UpperBound)
            'lstElevRange.SelectedIndex = 0
        End If
    End Sub

    Private Sub Display_DataStatus()

        'number of data layers, determined by the program developer
        'layers to be checked
        'Note: If you add layers,make sure AOI_hasSNotel on line 891 still works; Based on the position in array
        Dim ndata As Integer = 7

        'prepare list
        Dim datastatus(ndata) As String
        Dim datadesc(ndata) As String
        Dim DataName(ndata) As String

        Dim DataCount As Integer

        datadesc(0) = BA_MapStream
        DataName(0) = BA_StandardizeShapefileName(BA_EnumDescription(PublicPath.AoiStreamsVector), False)

        datadesc(1) = BA_MapPRISMElevation
        DataName(1) = BA_RasterElevationZones

        datadesc(2) = BA_MapElevationZone
        DataName(2) = BA_SubElevationZones

        datadesc(3) = BA_MapSNOTELZone
        DataName(3) = BA_RasterSNOTELZones

        datadesc(4) = BA_MapSnowCourseZone
        DataName(4) = BA_RasterSnowCourseZones

        datadesc(5) = BA_MapAspect
        DataName(5) = BA_RasterAspectZones

        datadesc(6) = BA_MapSlope
        DataName(6) = BA_RasterSlopeZones

        Dim i As Long

        lstDataStatus.Items.Clear()

        Try
            'check AOI stream
            Dim gdbPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers, True)
            If BA_File_Exists(gdbPath & DataName(0), WorkspaceType.Geodatabase, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass) Then
                datastatus(0) = "Ready"
            Else
                datastatus(0) = " ?"
            End If

            'check zone rasters
            Flag_PrecipitationZone = False
            Flag_ElevationZone = False

            gdbPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True)
            DataCount = 0
            For i = 1 To ndata - 1
                If BA_File_Exists(gdbPath & DataName(i), WorkspaceType.Geodatabase, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                    datastatus(i) = "Ready"
                    DataCount = DataCount + 1
                    Select Case i
                        Case 1 'Flag_ElevationZone
                            Flag_ElevationZone = True
                        Case 2 'Flag_PrecipitationZone
                            Flag_PrecipitationZone = True
                    End Select
                Else
                    datastatus(i) = " ?"
                End If
            Next

            For i = 0 To ndata - 1
                With lstDataStatus
                    Dim pitem As New ListViewItem(datastatus(i))
                    pitem.SubItems.Add(datadesc(i))
                    pitem.SubItems.Add(DataName(i))
                    .Items.Add(pitem)
                End With
            Next

            'check snotel and snow course data, tables and maps are still allowed
            'to be generated without the presence of these layers
            If datastatus(3) = " ?" Then 'snotel
                AOI_HasSNOTEL = False
                ndata = ndata - 1
            Else
                AOI_HasSNOTEL = True
            End If

            If datastatus(4) = " ?" Then 'snow course
                AOI_HasSnowCourse = False
                ndata = ndata - 1
            Else
                AOI_HasSnowCourse = True
            End If

            'disable elevation zone apply button if the zone exist
            If datastatus(2) = "Ready" Then
                cmdApplyElevInterval.Enabled = False
            Else
                cmdApplyElevInterval.Enabled = True
            End If

            'configure UI for Elevation-Precipitation Correlation
            Dim ElevPrecipLayersReady As Boolean = True
            Dim Has_SNOTELLayer As Boolean, Has_SnowCourseLayer As Boolean
            CheckForSitesLayers(Has_SNOTELLayer, Has_SnowCourseLayer)
            If Not Has_SNOTELLayer AndAlso Not Has_SnowCourseLayer Then
                'Disable Elevation-Precipitation Correlation if no sites
                FrameRepresentedPrecipitation.Enabled = False
            Else
                'Only run this path if using Elevation/Precip Correlation
                If ChkRepresentedPrecip.Checked Then
                    If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis) + "\" _
                                          + BA_TablePrecMeanElev, WorkspaceType.Geodatabase, esriDatasetType.esriDTTable) Then
                        ChkPrecipAoiTable.Checked = True
                    End If
                    If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) +
                                      BA_VectorSnotelPrec, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
                        ChkPrecipSitesLayer.Checked = True
                    End If
                    If ChkPrecipAoiTable.Checked = True AndAlso ChkPrecipSitesLayer.Checked = True Then
                        ElevPrecipLayersReady = True
                    Else
                        ElevPrecipLayersReady = False
                    End If
                    Dim partitionFileName As String = BA_FindElevPrecipRasterName(BA_RasterPartPrefix)
                    If Not String.IsNullOrEmpty(partitionFileName) Then
                        LblPartitionLayer.Text = partitionFileName.Substring(BA_RasterPartPrefix.Length)
                        m_partitionRasterPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + partitionFileName
                    End If
                    Dim zonesFileName As String = BA_FindElevPrecipRasterName(BA_ZonesRasterPrefix)
                    If Not String.IsNullOrEmpty(zonesFileName) Then
                        m_zoneRasterPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + zonesFileName
                        OptAggZone.Checked = True
                    Else
                        OptAggPrism.Checked = True
                    End If
                End If
            End If

            'set UI control
            If DataCount = ndata - 1 AndAlso ElevPrecipLayersReady = True AndAlso
                Flag_ElevOrPrecipChange = False Then 'not counting the AOI stream layer
                CmdMaps.Enabled = True
                If BA_Excel_Available Then
                    CmdTables.Enabled = True
                    CmdPartition.Enabled = True
                Else
                    CmdTables.Enabled = False
                    CmdPartition.Enabled = False
                End If
            Else
                'if PRISM zone intervals are populated but precipitation zones don't exist, enable CMdApplyPrism
                If datastatus(2) = "Ready" And datastatus(3) = " ?" And lstPrecipZones.Items.Count > 0 Then cmdApplyPRISMInterval.Enabled = True
                'enable generate zones button when elevation and precipitation zones exist
                If datastatus(2) = "Ready" And datastatus(3) = "Ready" And ElevPrecipLayersReady = False Then CmdGenerate.Enabled = True
                CmdMaps.Enabled = False
                CmdTables.Enabled = False
            End If

        Catch ex As Exception
            Debug.Print("Display_DataStatus Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub OptZMeters_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles OptZMeters.CheckedChanged
        If OptZMeters.Checked = True Then
            'Determine if Display ZUnit is the same as DEM ZUnit
            'AOI_DEMMin and AOI_DEMMax use internal system unit, i.e., meters
            Dim Conversion_Factor As Double = BA_SetConversionFactor(True, True) 'i.e., meters to meters

            lblElevUnit.Text = "Elevation (m):"

            'Populate Boxes
            txtMinElev.Text = Math.Round(AOI_DEMMin * Conversion_Factor - 0.005, 2) 'adjust value to include the actual min, max
            txtMaxElev.Text = Math.Round(AOI_DEMMax * Conversion_Factor + 0.005, 2)
            txtRangeElev.Text = Val(txtMaxElev.Text) - Val(txtMinElev.Text)

            Dim rvalues_arr() As BA_IntervalList = Nothing
            Dim subrvalues_arr() As BA_IntervalList = Nothing
            Dim ninterval As Integer, nsubinterval As Long

            ninterval = BA_CreateRangeArray(Val(txtMinElev.Text), Val(txtMaxElev.Text), Val(CmboxElevInterval.SelectedItem), rvalues_arr)
            txtElevClassNumber.Text = ninterval

            Display_IntervalList(rvalues_arr)
            nsubinterval = Subdivide_IntervalList(rvalues_arr, Val(CmboxElevInterval.Items(CmboxElevInterval.SelectedIndex)),
                                                  subrvalues_arr, Val(ComboxSubDivide.Items(ComboxSubDivide.SelectedIndex)))
            Display_ElevationRange(rvalues_arr)
            ResetElevationRange()

            Map_Display_Elevation_in_Meters = True
            If Not Set_Silent_Mode Then
                MsgBox("You must reapply the change (i.e., click the 1. Apply button) on Elevation Distribution Map to update the change!" &
                vbCrLf & "Or, reopen this dialog window to load the previous result.")
            End If

        End If
    End Sub

    Private Sub OptZFeet_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles OptZFeet.CheckedChanged
        If OptZFeet.Checked = True Then
            'Determine if Display ZUnit is the same as DEM ZUnit
            'AOI_DEMMin and AOI_DEMMax use internal system unit, i.e., meters
            Dim Conversion_Factor As Double = BA_SetConversionFactor(False, True) 'i.e., meters to feet

            lblElevUnit.Text = "Elevation (ft):"

            'Populate Boxes
            txtMinElev.Text = Math.Round(AOI_DEMMin * Conversion_Factor - 0.005, 2) 'adjust value to include the actual min, max
            txtMaxElev.Text = Math.Round(AOI_DEMMax * Conversion_Factor + 0.005, 2)
            txtRangeElev.Text = Val(txtMaxElev.Text) - Val(txtMinElev.Text)

            Dim rvalues_arr() As BA_IntervalList = Nothing
            Dim subrvalues_arr() As BA_IntervalList = Nothing
            Dim ninterval As Integer, nsubinterval As Long
            ninterval = BA_CreateRangeArray(Val(txtMinElev.Text), Val(txtMaxElev.Text), Val(CmboxElevInterval.SelectedItem), rvalues_arr)
            txtElevClassNumber.Text = ninterval

            Display_IntervalList(rvalues_arr)
            nsubinterval = Subdivide_IntervalList(rvalues_arr, Val(CmboxElevInterval.Items(CmboxElevInterval.SelectedIndex)),
                subrvalues_arr, Val(ComboxSubDivide.Items(ComboxSubDivide.SelectedIndex)))
            Display_ElevationRange(rvalues_arr)
            ResetElevationRange()

            Map_Display_Elevation_in_Meters = False
            If Not Set_Silent_Mode Then
                MsgBox("You must reapply the change (i.e., click the 1. Apply button) on Elevation Distribution Map to update the change!" &
                vbCrLf & "Or, reopen this dialog window to load the previous result.")
            End If
        End If
    End Sub

    'disable elevation range
    Private Sub ResetElevationRange()
        chkUseRange.Checked = False
        txtFromElev.Text = txtMinElev.Text
        txtToElev.Text = txtMaxElev.Text
    End Sub

    Private Sub InitForm()
        Elev_Interval = 200
        Elev_Subdivision = 1

        ComboxSubDivide.Items.Clear()
        With ComboxSubDivide
            .Items.Add("1")
            .Items.Add("2")
            .Items.Add("3")
            .Items.Add("4")
            .Items.Add("5")
            .SelectedIndex = 0
        End With

        CmboxPrecipType.Items.Clear()
        With CmboxPrecipType
            .Items.Add("Annual Precipitation")
            .Items.Add("Jan - Mar Precipitation")
            .Items.Add("Apr - Jun Precipitation")
            .Items.Add("Jul - Sep Precipitation")
            .Items.Add("Oct - Dec Precipitation")
            .Items.Add("Custom")
            .SelectedIndex = 0
        End With

        CmboxElevInterval.Items.Clear()
        With CmboxElevInterval
            .Items.Add("50")
            .Items.Add("100")
            .Items.Add("200")
            .Items.Add("250")
            .Items.Add("500")
            .Items.Add("1000")
            .Items.Add("2500")
            .Items.Add("5000")
            .SelectedIndex = 2
        End With

        CmboxBegin.Items.Clear()
        With CmboxBegin
            .Items.Add("1")
            .Items.Add("2")
            .Items.Add("3")
            .Items.Add("4")
            .Items.Add("5")
            .Items.Add("6")
            .Items.Add("7")
            .Items.Add("8")
            .Items.Add("9")
            .Items.Add("10")
            .Items.Add("11")
            .Items.Add("12")
            .SelectedIndex = 0
        End With

        CmboxEnd.Items.Clear()
        With CmboxEnd
            .Items.Add("1")
            .Items.Add("2")
            .Items.Add("3")
            .Items.Add("4")
            .Items.Add("5")
            .Items.Add("6")
            .Items.Add("7")
            .Items.Add("8")
            .Items.Add("9")
            .Items.Add("10")
            .Items.Add("11")
            .Items.Add("12")
            .SelectedIndex = 11
        End With

        CmboxAspect.Items.Clear()
        With CmboxAspect
            .Items.Add("4")
            .Items.Add("8")
            .Items.Add("16")
            .SelectedIndex = 2 'default to 16 aspect classes
        End With

        'set the value of BA_Excel_Available
        BA_Excel_Available = BA_Excel_Installed()

        Flag_BasinTables = False
        Flag_BasinMaps = False
    End Sub

    Private Sub chkUseRange_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkUseRange.CheckedChanged
        lblFromElev.Enabled = chkUseRange.Checked
        lblToElev.Enabled = chkUseRange.Checked
        OptSelFrom.Enabled = chkUseRange.Checked
        OptSelTo.Enabled = chkUseRange.Checked
        lblSelectType.Enabled = chkUseRange.Checked
        lblSelNote.Enabled = chkUseRange.Checked
        lstElevRange.Enabled = chkUseRange.Checked
    End Sub

    Private Sub CmboxElevInterval_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxElevInterval.SelectedIndexChanged
        Elev_Interval = Val(CmboxElevInterval.SelectedItem)
        Dim rvalues_arr() As BA_IntervalList = Nothing
        Dim subrvalues_arr() As BA_IntervalList = Nothing
        Dim ninterval As Integer, nsubinterval As Long
        lstintervals.Items.Clear()
        ninterval = BA_CreateRangeArray(Val(txtMinElev.Text), Val(txtMaxElev.Text), Elev_Interval, rvalues_arr)
        txtElevClassNumber.Text = ninterval
        Display_IntervalList(rvalues_arr)

        nsubinterval = Subdivide_IntervalList(rvalues_arr, Elev_Interval, subrvalues_arr, Elev_Subdivision)
        Display_ElevationRange(rvalues_arr)
        ResetElevationRange()
        If m_formInit = True Then
            cmdApplyElevInterval.Enabled = True
            CmdGenerate.Enabled = False
            CmdTables.Enabled = False
            CmdMaps.Enabled = False
        End If
    End Sub

    Private Sub cmdApplyElevInterval_Click(sender As System.Object, e As System.EventArgs) Handles cmdApplyElevInterval.Click
        Dim response As Integer
        Dim MessageKey As String
        Dim LayerRemoved As Integer
        Dim DeleteStatus As Integer
        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, 15)
        Dim progressDialog2 As IProgressDialog2 = Nothing
        Dim pDEMGeoDataset As IGeoDataset = Nothing
        Dim pZoneRaster As IGeoDataset = Nothing
        Dim pRasterBand As IRasterBand = Nothing
        Dim pRasterBandCollection As IRasterBandCollection = Nothing
        Dim pTable As ITable
        Dim pCursor As ICursor
        Dim pRow As IRow
        Dim pZoneFeatureCursor As IFeatureCursor
        Dim pQueryFilter As IQueryFilter
        Dim pZoneFeature As IFeature
        Dim pExtractOp As IExtractionOp2 = New RasterExtractionOp
        Dim pAOIRaster As IGeoDataset = Nothing
        Dim pTempRaster As IGeoDataset = Nothing
        'Declarations for Spatial Filter
        Dim pGeo As IGeometry = Nothing
        Dim pSFilter As ISpatialFilter = Nothing
        Dim SnowCourseFeatureClass As IFeatureClass = Nothing
        Dim SNOTELFeatureClass As IFeatureClass = Nothing
        Dim pFeatureClass As IFeatureClass = Nothing

        '=====================================================================================
        '1. Get Min and Max Raster Value
        '=====================================================================================
        'Get Min and Max DEM Raster Value
        Dim interval As Object
        Dim IntervalList() As BA_IntervalList = Nothing
        Dim SubIntervalList() As BA_IntervalList = Nothing
        Dim ninterval As Integer, nsubinterval As Long
        Dim DisplayConversionFact As Double
        Dim DataConversionFact As Double
        Dim i As Integer, j As Integer

        Try
            interval = Val(CmboxElevInterval.SelectedItem)
            DisplayConversionFact = BA_SetConversionFactor(OptZMeters.Checked, True)

            'calculate range values for reclass in Display ZUnits
            ninterval = BA_CreateRangeArray(Math.Round(AOI_DEMMin * DisplayConversionFact - 0.005, 2),
                                            Math.Round(AOI_DEMMax * DisplayConversionFact + 0.005, 2), interval, IntervalList)

            Elev_Interval = interval
            Elev_Subdivision = Val(ComboxSubDivide.SelectedItem)

            'calculate the subelevation list
            nsubinterval = Subdivide_IntervalList(IntervalList, Elev_Interval, SubIntervalList, Elev_Subdivision)

            'convert the range values to the DEM Zunit
            DataConversionFact = BA_SetConversionFactor(m_demInMeters, OptZMeters.Checked)

            If DataConversionFact <> 1 Then
                For i = 1 To ninterval
                    IntervalList(i).LowerBound = IntervalList(i).LowerBound * DataConversionFact
                    IntervalList(i).UpperBound = IntervalList(i).UpperBound * DataConversionFact
                Next

                For i = 1 To nsubinterval
                    SubIntervalList(i).LowerBound = SubIntervalList(i).LowerBound * DataConversionFact
                    SubIntervalList(i).UpperBound = SubIntervalList(i).UpperBound * DataConversionFact
                Next
            End If

            '=====================================================================================
            '2. Reclass Rasters (prism raster and sub_elevation raster)
            'prism raster: for elevation map and for prism precipitation analysis
            'sub_elevation raster: for creating elevation curve in excel charts
            '=====================================================================================
            '========================================
            'create elevation zones for prism summary
            '========================================
            'prism elevation zone is the same as the elevation mapping zone
            'use the prism elevation zone for elevation map

            'set parameters
            'Open the DEM of the AOI
            Dim strSavePath As String

            If Len(AOIFolderBase) > 0 Then
                Dim surfacesGdbPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces)
                pDEMGeoDataset = BA_OpenRasterFromGDB(surfacesGdbPath, BA_EnumDescription(MapsFileName.filled_dem_gdb))
            Else
                MsgBox("Please select an AOI!")
                Exit Sub
            End If

            cmdApplyElevInterval.Enabled = False
            progressDialog2 = BA_GetProgressDialog(pStepProg, "Processing Elevation Data", "Running...")
            pStepProg.Show()
            progressDialog2.ShowDialog()
            pStepProg.Step()

            'check and remove the output folder if it already exists
            strSavePath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis)  'analysis folder
            LayerRemoved = BA_RemoveLayersInFolder(My.Document, strSavePath) 'remove layers from map

            'remove elevation zone
            If BA_File_Exists(strSavePath & "\" & BA_RasterElevationZones, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                'delete raster
                DeleteStatus = BA_RemoveRasterFromGDB(strSavePath, BA_RasterElevationZones)
                If DeleteStatus = 0 Then 'unable to delete the folder
                    MsgBox("Unable to remove the folder " & strSavePath & "\" & BA_RasterElevationZones & ". Program stopped.")
                    Exit Sub
                End If
            End If

            'remove sub elevation zone
            If BA_File_Exists(strSavePath & "\" & BA_SubElevationZones, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                'delete raster
                DeleteStatus = BA_RemoveRasterFromGDB(strSavePath, BA_SubElevationZones)
                If DeleteStatus = 0 Then 'unable to delete the folder
                    MsgBox("Unable to remove the folder " & strSavePath & "\" & BA_SubElevationZones & ". Program stopped.")
                    Exit Sub
                End If
            End If

            MessageKey = "PRISM Elevation"
            pStepProg.Message = "Creating " & MessageKey & " Zones ..."
            pStepProg.Step()

            'reclassify and save the reclassified raster
            response = BA_ReclassRasterFromIntervalList(IntervalList, pDEMGeoDataset, strSavePath, BA_RasterElevationZones)
            'propogate interval list data to the attribute table of the grid
            response = BA_UpdateReclassRasterAttributes(IntervalList, strSavePath, BA_RasterElevationZones)
            'Try to enable Site Scenario tool after elevation zone is created
            Dim SiteScenarioButton = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of BtnSiteScenario)(My.ThisAddIn.IDs.BtnSiteScenario)
            SiteScenarioButton.selectedProperty = True


            MessageKey = "Subdivided-Elevation"
            pStepProg.Message = "Creating " & MessageKey & " Zones ..."
            pStepProg.Step()

            'repeat the same procedures for sub elevation raster
            response = BA_ReclassRasterFromIntervalList(SubIntervalList, pDEMGeoDataset, strSavePath, BA_SubElevationZones)
            response = BA_UpdateReclassRasterAttributes(SubIntervalList, strSavePath, BA_SubElevationZones)

            'open the raster elevation zone dataset for use
            pZoneRaster = BA_OpenRasterFromGDB(strSavePath, BA_RasterElevationZones)

            '=====================================================================================
            '3. Get Percent Area
            '=====================================================================================
            'Get zone Raster attribute
            pRasterBandCollection = pZoneRaster
            pRasterBand = pRasterBandCollection.Item(0)

            'Get Total Count of Cells
            Dim AreaSUM As Long
            Dim classarea() As Long
            Dim piField As Integer

            pTable = pRasterBand.AttributeTable
            pCursor = pTable.Search(Nothing, True)
            pRow = pCursor.NextRow

            AreaSUM = 0
            piField = pCursor.FindField(BA_FIELD_COUNT)
            ReDim classarea(0 To ninterval)

            For j = 1 To ninterval
                classarea(j) = pRow.Value(piField)
                AreaSUM = AreaSUM + classarea(j)
                pRow = pCursor.NextRow
            Next

            'calculate Percent Area
            For j = 1 To ninterval
                IntervalList(j).Area = (classarea(j) / AreaSUM) * 100
            Next

            '=====================================================================================
            '4. Convert Raster to Vector
            '=====================================================================================
            'Silently Delete Shapefile if Exists
            If BA_File_Exists(strSavePath & "\" & BA_VectorElevationZones, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
                'Remove Layer
                response = BA_RemoveLayersInFolder(My.Document, strSavePath)
                'Delete Dataset
                response = BA_Remove_ShapefileFromGDB(strSavePath, BA_VectorElevationZones)
            End If

            'set mask on the pZoneRaster so that the vector version doesn't include the buffer
            'Use the AOI extent for analysis
            'Open AOI Polygon to set the analysis mask
            pAOIRaster = BA_OpenRasterFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi), BA_AOIExtentRaster)
            If pAOIRaster Is Nothing Then
                MsgBox("Cannot locate AOI boundary raster in the AOI.  Please re-run AOI Tool.")
                Exit Sub
            End If

            pTempRaster = pExtractOp.Raster(pZoneRaster, pAOIRaster)
            Dim tmpRasterName = "tmpMask"
            response = BA_SaveRasterDataset(pTempRaster, AOIFolderBase, tmpRasterName)
            If response = 1 Then
                Dim success As BA_ReturnCode = BA_Raster2Polygon_GP(AOIFolderBase + "\" + tmpRasterName, strSavePath + "\" + BA_VectorElevationZones,
                                               BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) + BA_AOIExtentRaster)
                If BA_File_Exists(AOIFolderBase + "\" + tmpRasterName, WorkspaceType.Raster, esriDatasetType.esriDTRasterDataset) Then
                    response = BA_Remove_Raster(AOIFolderBase, tmpRasterName)
                End If
            End If
            pExtractOp = Nothing    'Release object

            If response = 0 Then
                MsgBox("Unable to convert the elevation zone raster to vector! Program stopped.")
                Exit Sub
            End If

            'propogate interval list data to the attribute table of the shapefile
            response = BA_UpdateReclassVectorAttributes(IntervalList, strSavePath, BA_VectorElevationZones)

            '=====================================================================================
            '5. SNOTEL and Snow Course Analysis
            '=====================================================================================
            'Declarations for Within Array
            Dim nSTSite As Long
            Dim nSCSite As Long

            'Get BareName, ParentDirectory, and Extension
            'SNOTELBareName = BA_SNOTELSites 'BA_GetBareNameAndExtension(frmSettings.txtSNOTEL.Text, SNOTELParentName, SNOTELExtension)
            'SnowCourseBareName = BA_SnowCourseSites 'BA_GetBareNameAndExtension(frmSettings.txtSnowCourse.Text, SnowCourseParentName, SnowCourseExtension)

            Dim Has_SNOTELLayer As Boolean, Has_SnowCourseLayer As Boolean
            CheckForSitesLayers(Has_SNOTELLayer, Has_SnowCourseLayer)

            'Open Elevation zone file Files
            pFeatureClass = BA_OpenFeatureClassFromGDB(strSavePath, BA_VectorElevationZones)
            If Has_SNOTELLayer Then
                'Open SNOTEL, Snowcourse, and Zone vector Shapefiles
                SNOTELFeatureClass = BA_OpenFeatureClassFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers), BA_SNOTELSites)

                'Determine Number of Features Within Each Class
                pQueryFilter = New QueryFilter
                pZoneFeature = New Feature
                pSFilter = New SpatialFilter

                'Run Analysis
                For j = 1 To ninterval
                    pQueryFilter.WhereClause = BA_FIELD_GRIDCODE & " = " & j
                    pZoneFeatureCursor = pFeatureClass.Search(pQueryFilter, False)
                    pZoneFeature = pZoneFeatureCursor.NextFeature

                    'Build Filter
                    nSTSite = 0

                    Do Until pZoneFeature Is Nothing
                        'Create Spatial Filter
                        pGeo = pZoneFeature.Shape

                        With pSFilter
                            .Geometry = pGeo
                            .GeometryField = BA_FIELD_SHAPE
                            .SpatialRel = esriSpatialRelEnum.esriSpatialRelContains
                        End With

                        'Get Number of sites within Filter
                        nSTSite = nSTSite + SNOTELFeatureClass.FeatureCount(pSFilter)
                        pZoneFeature = pZoneFeatureCursor.NextFeature
                    Loop

                    'Call next SNOTEL Feature
                    IntervalList(j).SNOTEL = nSTSite
                Next
            Else
                'reset count to zero
                For j = 1 To ninterval
                    IntervalList(j).SNOTEL = 0
                Next
            End If

            If Has_SnowCourseLayer Then
                'Open Snowcourse, and Zone vector Shapefiles
                SnowCourseFeatureClass = BA_OpenFeatureClassFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers), BA_SnowCourseSites)

                'Determine Number of Features Within Each Class
                pQueryFilter = New QueryFilter
                pZoneFeature = New Feature
                pSFilter = New SpatialFilter

                'Run Analysis
                For j = 1 To ninterval
                    pQueryFilter.WhereClause = BA_FIELD_GRIDCODE & " = " & j
                    pZoneFeatureCursor = pFeatureClass.Search(pQueryFilter, False)
                    pZoneFeature = pZoneFeatureCursor.NextFeature

                    'Build Filter
                    nSCSite = 0

                    Do Until pZoneFeature Is Nothing
                        'Create Spatial Filter
                        pGeo = pZoneFeature.Shape

                        With pSFilter
                            .Geometry = pGeo
                            .GeometryField = BA_FIELD_SHAPE
                            .SpatialRel = esriSpatialRelEnum.esriSpatialRelContains
                        End With

                        'Get Number of sites within Filter
                        nSCSite = nSCSite + SnowCourseFeatureClass.FeatureCount(pSFilter)
                        pZoneFeature = pZoneFeatureCursor.NextFeature
                    Loop

                    'Call next SNOTEL Feature
                    IntervalList(j).SnowCourse = nSCSite
                Next
            Else
                'reset count to zero
                For j = 1 To ninterval
                    IntervalList(j).SnowCourse = 0
                Next
            End If

            '=======================================================================================================================
            '7. Add Values to Form
            '=======================================================================================================================
            Display_IntervalList(IntervalList)

            '=================================
            'Update map parameters file
            '=================================
            Dim filepath As String, FileName As String


            Flag_ElevOrPrecipChange = True
            'check if map_parameters.txt file exists
            filepath = BA_GetPath(AOIFolderBase, PublicPath.Maps)
            FileName = BA_MapParameterFile 'i.e., map_parameters.txt
            response = UpdateMapParameters(filepath, FileName)

            If response <= 0 Then
                response = SaveMapParameters(filepath, FileName)
                '    MsgBox "Error! Unable to update map parameter file. Please report the error to the developer."
            End If

            'set flags
            Flag_ElevationZone = True
            Flag_BasinTables = False
            Flag_BasinMaps = False

            If Flag_PrecipitationZone Then
                CmdGenerate.Enabled = True
            Else
                CmdGenerate.Enabled = False
            End If

            'We only want to enable button #2 after this completes if there is not a previous analysis
            'If there is a previous analysis, there will be items in lstPrecipZones
            If lstPrecipZones.Items.Count = 0 Then
                cmdPRISM.Enabled = True
            End If

            'Delete temporary files from AOI root
            BA_RemoveTempRasters(AOIFolderBase)

        Catch ex As Exception
            Debug.Print("cmdApplyElevInterval_Click Exception: " & ex.Message)
        Finally
            If pStepProg IsNot Nothing Then
                pStepProg.Hide()
                pStepProg = Nothing
            End If
            If progressDialog2 IsNot Nothing Then
                progressDialog2.HideDialog()
                progressDialog2 = Nothing
            End If

            pGeo = Nothing
            pSFilter = Nothing
            pZoneRaster = Nothing
            pZoneFeatureCursor = Nothing
            pZoneFeature = Nothing
            pQueryFilter = Nothing
            pFeatureClass = Nothing
            pZoneRaster = Nothing
            pTable = Nothing
            pCursor = Nothing
            pRow = Nothing
            pRasterBand = Nothing
            pRasterBandCollection = Nothing
            pDEMGeoDataset = Nothing
            pAOIRaster = Nothing
            SnowCourseFeatureClass = Nothing
            SNOTELFeatureClass = Nothing
        End Try
    End Sub

    Private Sub ComboxSubDivide_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboxSubDivide.SelectedIndexChanged
        Elev_Subdivision = Val(ComboxSubDivide.SelectedItem)
        Dim rvalues_arr() As BA_IntervalList = Nothing
        Dim subrvalues_arr() As BA_IntervalList = Nothing
        Dim ninterval As Integer, nsubinterval As Long
        ninterval = BA_CreateRangeArray(Val(txtMinElev.Text), Val(txtMaxElev.Text), Elev_Interval, rvalues_arr)
        nsubinterval = Subdivide_IntervalList(rvalues_arr, Elev_Interval, subrvalues_arr, Elev_Subdivision)
        Display_ElevationRange(rvalues_arr)
        ResetElevationRange()
        'Manage the form buttons if the form has already loaded
        If m_formInit = True Then
            cmdApplyElevInterval.Enabled = True
            CmdGenerate.Enabled = False
            CmdTables.Enabled = False 'disable the table button, users need to regenerate the elev zones
        End If
    End Sub

    Private Sub lstElevRange_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lstElevRange.SelectedIndexChanged
        If chkUseRange.Checked = True Then
            If OptSelFrom.Checked = True Then
                txtFromElev.Text = lstElevRange.SelectedItem
            Else
                txtToElev.Text = lstElevRange.SelectedItem
            End If
        End If
    End Sub

    Private Sub CmboxPrecipType_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxPrecipType.SelectedIndexChanged
        If m_formInit = True Then
            cmdPRISM.Enabled = True
            CmdGenerate.Enabled = False
            CmdGenerate.Enabled = False
            CmdTables.Enabled = False
            CmdMaps.Enabled = False
        End If

        If CmboxPrecipType.SelectedIndex = 5 Then
            lblBeginMonth.Enabled = True
            CmboxBegin.Enabled = True
            lblEndMonth.Enabled = True
            CmboxEnd.Enabled = True
        Else
            lblBeginMonth.Enabled = False
            CmboxBegin.Enabled = False
            lblEndMonth.Enabled = False
            CmboxEnd.Enabled = False
        End If

        'reset the PRISM Dialog window
        lstPrecipZones.Items.Clear()
        txtMinPrecip.Text = ""
        txtMaxPrecip.Text = ""
        txtRangePrecip.Text = ""
        txtPrecipMapZoneInt.Text = "0"
        If m_formInit = True Then
            cmdApplyPRISMInterval.Enabled = False
        End If

        'reset represented precipitation
        If ChkRepresentedPrecip.Checked = True Then
            'Need to re-run represented precip layers
            ChkPrecipAoiTable.Checked = False
            ChkPrecipSitesLayer.Checked = False
            FrameRepresentedPrecipitation.Enabled = True
        End If
    End Sub

    Private Sub cmdPRISM_Click(sender As System.Object, e As System.EventArgs) Handles cmdPRISM.Click
        'Get Number of Precipitation Zones
        Dim response As Integer
        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, 10)
        Dim progressDialog2 As IProgressDialog2 = Nothing
        Dim pRasterStats As IRasterStatistics

        'check for the presense of PRISM data before continue.
        Dim temppathname As String = AOIFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Prism) & "\Q4"
        If Not BA_File_Exists(temppathname, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            MsgBox("No PRISM data in the AOI! Please clip the data using the AOI Utility dialog.")
            Exit Sub
        End If

        Try
            progressDialog2 = BA_GetProgressDialog(pStepProg, "Compiling PRISM Precipitation Data", "Running...")
            pStepProg.Show()
            progressDialog2.ShowDialog()
            pStepProg.Step()

            PrecipPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism, True)
            AnalysisPath = BA_GetPath(AOIFolderBase, PublicPath.Analysis)

            'Determine Precipitation Path
            '    PRISMLayer(1) = "Jan"
            '    PRISMLayer(2) = "Feb"
            '    PRISMLayer(3) = "Mar"
            '    PRISMLayer(4) = "Apr"
            '    PRISMLayer(5) = "May"
            '    PRISMLayer(6) = "Jun"
            '    PRISMLayer(7) = "Jul"
            '    PRISMLayer(8) = "Aug"
            '    PRISMLayer(9) = "Sep"
            '    PRISMLayer(10) = "Oct"
            '    PRISMLayer(11) = "Nov"
            '    PRISMLayer(12) = "Dec"
            '    PRISMLayer(13) = "Q1"
            '    PRISMLayer(14) = "Q2"
            '    PRISMLayer(15) = "Q3"
            '    PRISMLayer(16) = "Q4"
            '    PRISMLayer(17) = "Annual"

            If CmboxPrecipType.SelectedIndex = 0 Then  'read direct Annual PRISM raster
                PRISMRasterName = AOIPrismFolderNames.annual.ToString
            ElseIf CmboxPrecipType.SelectedIndex > 0 And CmboxPrecipType.SelectedIndex < 5 Then 'read directly Quarterly PRISM raster
                PRISMRasterName = BA_GetPrismFolderName(CmboxPrecipType.SelectedIndex + 12)
            Else 'sum individual monthly PRISM rasters
                response = BA_PRISMCustom(My.Document, AOIFolderBase, Val(CmboxBegin.SelectedItem), Val(CmboxEnd.SelectedItem))
                If response = 0 Then
                    MsgBox("Unable to generate custom PRISM layer! Program stopped.")
                    Exit Sub
                End If
                PrecipPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True)
                PRISMRasterName = BA_TEMP_PRISM
            End If

            Dim raster_res As Double
            pRasterStats = BA_GetRasterStatsGDB(PrecipPath & PRISMRasterName, raster_res)

            'Populate Boxes
            txtMinPrecip.Text = Math.Round(pRasterStats.Minimum - 0.005, 2)
            txtMaxPrecip.Text = Math.Round(pRasterStats.Maximum + 0.005, 2)
            txtRangePrecip.Text = Val(txtMaxPrecip.Text) - Val(txtMinPrecip.Text)

            'create precipitation zones
            Dim minvalue As Object
            Dim maxvalue As Object
            Dim interval As Object
            Dim IntervalList() As BA_IntervalList = Nothing
            Dim ninterval As Integer

            minvalue = Val(txtMinPrecip.Text)
            maxvalue = Val(txtMaxPrecip.Text)

            'determine interval number based on map class #
            interval = (maxvalue - minvalue) / Val(txtPrecipMapZoneNo.Text)
            'round the number to 2 decimal place
            interval = Math.Round(interval, 2)

            'calculate range values for reclass
            ninterval = BA_CreateRangeArray(minvalue, maxvalue, interval, IntervalList)

            'display range information
            Display_PreciptIntervalList(IntervalList)
            txtPrecipMapZoneNo.Text = ninterval
            txtPrecipMapZoneInt.Text = interval

            '=================================
            'Update map parameters file
            '=================================
            Dim filepath As String, FileName As String

            'check if map_parameters.txt file exists
            filepath = BA_GetPath(AOIFolderBase, PublicPath.Maps)
            FileName = BA_MapParameterFile 'i.e., map_parameters.txt
            response = UpdateMapParametersPRISM(filepath, FileName)

            If response <= 0 Then
                response = SaveMapParameters(filepath, FileName)
            End If

            'set flags
            Flag_BasinTables = False
            Flag_BasinMaps = False

            cmdPRISM.Enabled = False
            cmdApplyPRISMInterval.Enabled = True
            'MsgBox("Change the Precipitation Zone Interval value to activate the Apply button!")
        Catch ex As Exception
            Debug.Print("cmdPRISM_Click Exception: " & ex.Message)
        Finally
            If pStepProg IsNot Nothing Then
                pStepProg.Hide()
                pStepProg = Nothing
            End If
            If progressDialog2 IsNot Nothing Then
                progressDialog2.HideDialog()
                progressDialog2 = Nothing
            End If
            pRasterStats = Nothing
        End Try
    End Sub

    Private Sub CmboxBegin_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxBegin.SelectedIndexChanged
        If m_formInit = True Then cmdPRISM.Enabled = True
    End Sub

    Private Sub CmboxEnd_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxEnd.SelectedIndexChanged
        If m_formInit = True Then cmdPRISM.Enabled = True
    End Sub

    Private Sub txtPrecipMapZoneInt_Validated(sender As Object, e As System.EventArgs) Handles txtPrecipMapZoneInt.Validated
        Dim minValue As Double = Val(txtMinPrecip.Text)
        Dim maxValue As Double = Val(txtMaxPrecip.Text)
        'determine interval number based on map class #
        Dim interval As Double = Val(txtPrecipMapZoneInt.Text)

        'Only run sub if min, max, interval values have been initialized
        If minValue + maxValue + interval <> 0 Then
            'create precipitation zones
            Dim IntervalList() As BA_IntervalList = Nothing
            Dim ninterval As Integer

            If interval <= 0 Or maxValue < minValue Then
                MsgBox("Invalid interval number or range value!")
                Exit Sub 'invalid parameters
            End If

            'calculate range values for reclass
            ninterval = BA_CreateRangeArray(minValue, maxValue, interval, IntervalList)

            'display range information
            Display_PreciptIntervalList(IntervalList)
            txtPrecipMapZoneNo.Text = ninterval
            cmdApplyPRISMInterval.Enabled = True
            CmdGenerate.Enabled = False
            CmdTables.Enabled = False
            CmdMaps.Enabled = False
        End If
    End Sub

    'Private Sub txtPrecipMapZoneInt_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtPrecipMapZoneInt.TextChanged
    '    Dim minValue As Double = Val(txtMinPrecip.Text)
    '    Dim maxValue As Double = Val(txtMaxPrecip.Text)
    '    'determine interval number based on map class #
    '    Dim interval As Double = Val(txtPrecipMapZoneInt.Text)

    '    'Only run sub if min, max, interval values have been initialized
    '    If minValue + maxValue + interval <> 0 Then
    '        'create precipitation zones
    '        Dim IntervalList() As BA_IntervalList = Nothing
    '        Dim ninterval As Integer

    '        If interval <= 0 Or maxValue < minValue Then
    '            MsgBox("Invalid interval number or range value!")
    '            Exit Sub 'invalid parameters
    '        End If

    '        'calculate range values for reclass
    '        ninterval = BA_CreateRangeArray(minValue, maxValue, interval, IntervalList)

    '        'display range information
    '        Display_PreciptIntervalList(IntervalList)
    '        txtPrecipMapZoneNo.Text = ninterval
    '        cmdApplyPRISMInterval.Enabled = True
    '    End If
    'End Sub

    Private Sub cmdApplyPRISMInterval_Click(sender As System.Object, e As System.EventArgs) Handles cmdApplyPRISMInterval.Click

        Dim response As Integer
        Dim PMinValue As Double
        Dim PMaxValue As Double
        Dim interval As Object
        Dim IntervalList() As BA_IntervalList = Nothing
        Dim ninterval As Integer
        Dim InputPath As String
        Dim InputName As String
        'Dim VectorName As String
        Const NO_VECTOR_NAME As String = ""
        Dim RasterName As String
        Dim strSavePath As String
        Dim pInputRaster As IGeoDataset
        Dim MessageKey As AOIMessageKey
        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, 10)
        Dim progressDialog2 As IProgressDialog2 = Nothing

        Try

            progressDialog2 = BA_GetProgressDialog(pStepProg, "Preparing Precipitation Zone Dataset", "Running...")
            pStepProg.Show()
            progressDialog2.ShowDialog()
            pStepProg.Step()
            cmdApplyPRISMInterval.Enabled = False

            '=================================
            'create precipitation zones
            '=================================
            'set parameters
            SetPrecipPathInfo()

            InputPath = PrecipPath
            InputName = PRISMRasterName
            'VectorName = BA_VectorPrecipitationZones
            RasterName = BA_EnumDescription(MapsFileName.PrecipZone)
            strSavePath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis)  'analysis fgdb
            MessageKey = AOIMessageKey.Precipitation

            pStepProg.Message = "Creating " & BA_EnumDescription(MessageKey) & " Zones ..."
            pStepProg.Step()

            'calculate range values for reclass
            PMinValue = Val(txtMinPrecip.Text)
            PMaxValue = Val(txtMaxPrecip.Text)
            interval = Val(txtPrecipMapZoneInt.Text)
            ninterval = BA_CreateRangeArray(PMinValue, PMaxValue, interval, IntervalList)

            'Create the zone raster and vector
            Dim maskFilePath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) + BA_EnumDescription(AOIClipFile.PrismClipAOIExtentCoverage)
            response = BA_MakeZoneDatasets(My.Document, InputPath + "\" + InputName, IntervalList,
                                           BA_FIELD_VALUE, strSavePath, RasterName, maskFilePath, NO_VECTOR_NAME, MessageKey)

            '=================================
            'Update map parameters file
            '=================================
            Dim filepath As String, FileName As String

            Flag_ElevOrPrecipChange = True

            'check if map_parameters.txt file exists
            filepath = BA_GetPath(AOIFolderBase, PublicPath.Maps)
            FileName = BA_MapParameterFile 'i.e., map_parameters.txt
            response = UpdateMapParametersPRISM(filepath, FileName)

            If response <= 0 Then
                response = SaveMapParameters(filepath, FileName)
                '    MsgBox "Error! Unable to update map parameter file. Please report the error to the developer."
            End If

            Flag_PrecipitationZone = True
            Flag_BasinTables = False
            Flag_BasinMaps = False

            If Flag_ElevationZone Then
                CmdGenerate.Enabled = True
            Else
                CmdGenerate.Enabled = False
            End If
        Catch ex As Exception
            Debug.Print(" Exception" & ex.Message)
            cmdApplyPRISMInterval.Enabled = True
            Display_DataStatus()
        Finally
            If pStepProg IsNot Nothing Then
                pStepProg.Hide()
                pStepProg = Nothing
            End If
            If progressDialog2 IsNot Nothing Then
                progressDialog2.HideDialog()
                progressDialog2 = Nothing
            End If
            pInputRaster = Nothing
        End Try
    End Sub

    Private Sub CmdGenerate_Click(sender As System.Object, e As System.EventArgs) Handles CmdGenerate.Click
        Dim response As Integer

        'generate all needed raster and vector datasets before generating Excel book
        'the layers to be generated are:
        'precipitation zone
        'elevation zone for precipitation distribution analysis
        'slope zones
        'aspect zones
        Dim i As Integer
        Dim IntervalList() As BA_IntervalList = Nothing
        Dim InputPath As String
        Dim InputName As String
        'Dim VectorName As String
        Const NO_VECTOR_NAME As String = ""
        Dim RasterName As String
        Dim strSavePath As String
        Dim MessageKey As AOIMessageKey
        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, 10)
        Dim progressDialog2 As IProgressDialog2 = Nothing

        Try
            progressDialog2 = BA_GetProgressDialog(pStepProg, "Preparing Zone Datasets", "Running...")
            pStepProg.Show()
            progressDialog2.ShowDialog()
            pStepProg.Step()

            '=================================
            'create aspect zones
            '=================================
            'set parameters
            InputPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces)
            InputName = BA_GetBareName(BA_EnumDescription(PublicPath.Aspect))
            'VectorName = BA_VectorAspectZones
            RasterName = BA_EnumDescription(MapsFileName.AspectZone)
            strSavePath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis)
            MessageKey = AOIMessageKey.Aspect

            pStepProg.Message = "Creating " & BA_EnumDescription(MessageKey) & " Zones ..."
            pStepProg.Step()

            'set reclass
            Dim AspectDirectionsNumber As Short = 2 ^ (CmboxAspect.SelectedIndex + 2) 'either 4, 8, or 16
            BA_SetAspectClasses(IntervalList, AspectDirectionsNumber)

            'Create the zone raster and vector
            Dim maskFolderPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) + BA_EnumDescription(AOIClipFile.BufferedAOIExtentCoverage)
            response = BA_MakeZoneDatasets(My.Document, InputPath + "\" + InputName, IntervalList, BA_FIELD_VALUE,
                                           strSavePath, RasterName, maskFolderPath, NO_VECTOR_NAME, MessageKey)

            '=================================
            'create slope zones
            '=================================
            'set parameters
            InputName = BA_GetBareName(BA_EnumDescription(PublicPath.Slope))
            'VectorName = BA_VectorSlopeZones
            RasterName = BA_EnumDescription(MapsFileName.SlopeZone)
            MessageKey = AOIMessageKey.Slope

            pStepProg.Message = "Creating " & BA_EnumDescription(MessageKey) & " Zones ..."
            pStepProg.Step()

            'set reclass
            BA_SetSlopeClasses(IntervalList)

            'Create the zone raster and vector
            response = BA_MakeZoneDatasets(My.Document, InputPath + "\" + InputName, IntervalList,
                                           BA_FIELD_VALUE, strSavePath, RasterName, maskFolderPath, NO_VECTOR_NAME, MessageKey)

            Dim DataConversionFact As Double = BA_SetConversionFactor(m_demInMeters, True)

            ' Open SNOTEL and Snow Course Files
            Dim SNOTELNamePath As String
            Dim SNOTELParentPath As String
            Dim SNOTELBareName As String
            Dim AOILayerPath As String
            Dim SnowCourseNamePath As String
            Dim SnowCourseParentPath As String
            Dim SnowCourseBareName As String

            AOILayerPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers) 'clipped SNOTEL and SnowCourse are in this folder
            SNOTELNamePath = BA_SystemSettings.SNOTELLayer
            SNOTELBareName = BA_EnumDescription(MapsFileName.Snotel)
            SNOTELParentPath = AOILayerPath

            SnowCourseNamePath = BA_SystemSettings.SCourseLayer
            SnowCourseBareName = BA_EnumDescription(MapsFileName.SnowCourse)
            SnowCourseParentPath = AOILayerPath

            '=================================
            'create snotel elevation zones
            '=================================
            'set parameters
            InputName = BA_EnumDescription(MapsFileName.filled_dem_gdb)
            'VectorName = BA_VectorSNOTELZones
            RasterName = BA_EnumDescription(MapsFileName.SnotelZone)
            MessageKey = AOIMessageKey.Snotel

            pStepProg.Message = "Creating " & BA_EnumDescription(MessageKey) & " Zones ..."
            pStepProg.Step()

            'SNOTEL and Snow Course elevation internal data units are always in meters
            'AOI_DEMMin and Max have the same unit as SNOTEL's ZUnit, i.e., in meters.
            response = BA_GetUniqueSortedValues(SNOTELParentPath, SNOTELBareName, BA_FIELD_SITE_NAME, BA_SiteElevField, AOI_DEMMin, AOI_DEMMax, IntervalList)

            'Converts SNOTEL integer into DEM ZUnit
            Dim ListMax As Object
            ListMax = UBound(IntervalList)
            For i = 1 To ListMax
                IntervalList(i).LowerBound = IntervalList(i).LowerBound * DataConversionFact
                IntervalList(i).UpperBound = IntervalList(i).UpperBound * DataConversionFact
            Next

            If response = 0 Then 'input attribute out of the elevation bounds
                MsgBox("Elevation data in the SNOTEL layer are out of the DEM elevation range!")
            ElseIf response > 0 Then
                'Create the zone raster and vector
                maskFolderPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) + BA_EnumDescription(AOIClipFile.AOIExtentCoverage)
                response = BA_MakeZoneDatasets(My.Document, InputPath + "\" + InputName, IntervalList,
                                               BA_FIELD_VALUE, strSavePath, RasterName, maskFolderPath, NO_VECTOR_NAME, MessageKey)
            End If

            '===================================
            'create snow course elevation zones
            '===================================
            'set parameters
            'VectorName = BA_VectorSnowCourseZones
            RasterName = BA_EnumDescription(MapsFileName.SnowCourseZone)
            MessageKey = AOIMessageKey.SnowCourse

            pStepProg.Message = "Creating " & BA_EnumDescription(MessageKey) & " Zones ..."
            pStepProg.Step()

            'Converts Display Range to SNOTEL ZUnit
            response = BA_GetUniqueSortedValues(SnowCourseParentPath, SnowCourseBareName, BA_FIELD_SITE_NAME, BA_SiteElevField, AOI_DEMMin, AOI_DEMMax, IntervalList)

            'Converts Snow course integer into DEM ZUnit
            ListMax = UBound(IntervalList)
            For i = 1 To ListMax
                IntervalList(i).LowerBound = IntervalList(i).LowerBound * DataConversionFact
                IntervalList(i).UpperBound = IntervalList(i).UpperBound * DataConversionFact
            Next

            If response = 0 Then 'input attribute out of the elevation bounds
                MsgBox("Elevation data in the snow course layer are out of the DEM elevation range!")
            ElseIf response > 0 Then
                'create the zone raster and vector
                response = BA_MakeZoneDatasets(My.Document, InputPath + "\" + InputName, IntervalList,
                                               BA_FIELD_VALUE, strSavePath, RasterName, maskFolderPath, NO_VECTOR_NAME, MessageKey)
            End If

            Dim success As BA_ReturnCode = BA_ReturnCode.UnknownError
            'Delete aggregated precip output table if it exists
            If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis) + "\" +
                    BA_TablePrecMeanElev, WorkspaceType.Geodatabase, esriDatasetType.esriDTTable) Then
                success = BA_Remove_TableFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), BA_TablePrecMeanElev)
            End If
            If ChkRepresentedPrecip.Checked = True Then
                pStepProg.Message = "Creating Precipitation Mean Elevation layer ..."
                pStepProg.Step()

                SetPrecipPathInfo()
                Dim AspIntervalList() As BA_IntervalList = Nothing
                BA_SetAspectClasses(AspIntervalList, AspectDirectionsNumber)
                Dim aspLayerPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_RasterAspectZones

                If OptAggZone.Checked = False Then
                    'AGGREGATING BY PRISM CELLS
                    'Resample DEM to PRISM resolution
                    Dim precipMeanPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis) + "\" + BA_RasterPrecMeanElev
                    success = BA_CreateElevPrecipLayer(AOIFolderBase, PrecipPath, PRISMRasterName, precipMeanPath)

                    'Resample Aspect to PRISM resolution
                    If success = BA_ReturnCode.Success Then
                        pStepProg.Message = "Creating Aspect layer for elev-precip analysis..."
                        pStepProg.Step()
                        aspLayerPath = CreateAspectLayerForElevPrecip(PrecipPath, PRISMRasterName)
                        If success = BA_ReturnCode.Success AndAlso Not String.IsNullOrEmpty(aspLayerPath) Then
                            'Run Sample tool to extract elevation/precipitation for PRISM cell locations; The output is a table
                            If success = BA_ReturnCode.Success Then
                                Dim sampleTablePath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis) + "\" + BA_TablePrecMeanElev
                                Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder()
                                sb.Append(aspLayerPath + ";")
                                If Not String.IsNullOrEmpty(m_partitionRasterPath) Then _
                                    sb.Append(m_partitionRasterPath + ";")
                                sb.Append(PrecipPath + "\" + PRISMRasterName & ";")
                                sb.Append(precipMeanPath)
                                pStepProg.Message = "Extracting DEM and PRISM values to table..."
                                pStepProg.Step()
                                Dim prismCellSize As Double = BA_CellSize(PrecipPath, PRISMRasterName)
                                success = BA_Sample(sb.ToString, PrecipPath + "\" + PRISMRasterName, sampleTablePath,
                                          PrecipPath + "\" + PRISMRasterName, BA_Resample_Nearest, CStr(prismCellSize))
                                If success = BA_ReturnCode.Success Then
                                    'set reclass
                                    Dim aspFieldName As String = BA_GetBareName(aspLayerPath)
                                    success = BA_UpdateTableAttributes(AspIntervalList, BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis),
                                                             BA_TablePrecMeanElev, BA_FIELD_ASPECT, aspFieldName, esriFieldType.esriFieldTypeString)
                                End If
                            End If
                        End If
                    End If
                Else
                    'AGGREGATING ACCORDING TO ZONE LAYER
                    success = GenerateZonePrecipElevLayers(pStepProg, AspectDirectionsNumber, AspIntervalList)
                End If
                If success = BA_ReturnCode.Success Then
                    ChkPrecipAoiTable.Checked = True
                End If
                Dim sitesPath As String = BA_CreateSitesLayer(AOIFolderBase, BA_MergedSites, BA_SiteTypeField,
                                                              BA_SiteSnotel, BA_SiteSnowCourse)
                'Extract values to sites; DEM comes from BA_SELEV
                If Not String.IsNullOrEmpty(sitesPath) Then
                    Dim snotelPrecipPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + BA_VectorSnotelPrec
                    Dim tempSnotelPrecipPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + "tmpSnoExtract"
                    'Extract PRISM values to sites
                    success = BA_ExtractValuesToPoints(sitesPath,
                                                       PrecipPath + "\" + PRISMRasterName, tempSnotelPrecipPath, PrecipPath + "\" + PRISMRasterName, True)
                    If success = BA_ReturnCode.Success Then
                        'Rename extracted precip field
                        Dim tempfileName As String = BA_GetBareName(tempSnotelPrecipPath)
                        BA_RenameRasterValuesField(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tempfileName, BA_FIELD_RASTERVALU,
                                                BA_FIELD_PRECIP, esriFieldType.esriFieldTypeDouble)
                        Dim aspectValuesInputPath As String = tempSnotelPrecipPath
                        Dim partitionFileName As String = "tmpPartition"
                        If Not String.IsNullOrEmpty(m_partitionRasterPath) Then
                            'Extract PARTITION values to sites
                            success = BA_ExtractValuesToPoints(tempSnotelPrecipPath, m_partitionRasterPath,
                                                              BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + partitionFileName,
                                                               PrecipPath + "\" + PRISMRasterName, True)
                            If success = BA_ReturnCode.Success Then
                                'Rename extracted partition field
                                Dim partFileName As String = BA_GetBareName(m_partitionRasterPath)
                                Dim partitionFieldName As String = partFileName.Substring(BA_RasterPartPrefix.Length)
                                BA_RenameRasterValuesField(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), partitionFileName, BA_FIELD_RASTERVALU,
                                                        partitionFieldName, esriFieldType.esriFieldTypeDouble)
                                aspectValuesInputPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + partitionFileName
                            End If
                        End If
                        Dim tmpZonesFileName As String = "tmpZones"
                        If Not String.IsNullOrEmpty(m_zoneRasterPath) Then
                            'Extract ZONES values to sites
                            success = BA_ExtractValuesToPoints(aspectValuesInputPath, m_zoneRasterPath,
                                  BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + tmpZonesFileName,
                                  PrecipPath + "\" + PRISMRasterName, True)
                            If success = BA_ReturnCode.Success Then
                                'Rename extracted partition field
                                Dim zonesFileName As String = BA_GetBareName(m_zoneRasterPath)
                                Dim zonesFieldName As String = zonesFileName.Substring(BA_ZonesRasterPrefix.Length)
                                BA_RenameRasterValuesField(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tmpZonesFileName, BA_FIELD_RASTERVALU,
                                                        zonesFieldName, esriFieldType.esriFieldTypeInteger)
                                aspectValuesInputPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + tmpZonesFileName
                            End If
                        End If
                        'Extract ASPECT values to sites
                        success = BA_ExtractValuesToPoints(aspectValuesInputPath, aspLayerPath, snotelPrecipPath,
                                                           PrecipPath + "\" + PRISMRasterName, True)
                        If Not String.IsNullOrEmpty(m_partitionRasterPath) Then
                            BA_Remove_ShapefileFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), partitionFileName)
                        End If
                        If Not String.IsNullOrEmpty(m_zoneRasterPath) Then
                            BA_Remove_ShapefileFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tmpZonesFileName)
                        End If
                        BA_Remove_ShapefileFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tempfileName)
                        If success = BA_ReturnCode.Success Then
                            success = BA_UpdateFeatureClassAttributes(AspIntervalList, BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis),
                                                                      BA_VectorSnotelPrec, BA_FIELD_ASPECT, BA_FIELD_RASTERVALU, esriFieldType.esriFieldTypeString)
                            If success = BA_ReturnCode.Success Then
                                ChkPrecipSitesLayer.Checked = True
                            End If
                        Else
                            MessageBox.Show("An error occurred while trying to process the site layers for represented precipitation!")
                        End If
                    Else
                        MessageBox.Show("An error occurred while trying to process the site layers for represented precipitation!")
                    End If
                Else
                    MessageBox.Show("An error occurred while trying to process the site layers for represented precipitation!")
                End If

            End If

            '=================================
            'Update map parameters file
            '=================================
            Dim filepath As String, FileName As String

            Flag_ElevOrPrecipChange = False

            'check if map_parameters.txt file exists
            filepath = BA_GetPath(AOIFolderBase, PublicPath.Maps)
            FileName = BA_MapParameterFile 'i.e., map_parameters.txt
            response = SaveMapParameters(filepath, FileName)
            Display_DataStatus()
            'set flags
            Flag_BasinMaps = True
            Flag_BasinTables = True

            CmdGenerate.Enabled = False
            If response <= 0 Then MsgBox("Error! Unable to update map parameter file. Please report the error to the developer.")
        Catch ex As Exception
            Debug.Print("CmdGenerate_Click Exception: " & ex.Message)
            Display_DataStatus()
            CmdGenerate.Enabled = False
        Finally
            If pStepProg IsNot Nothing Then
                pStepProg.Hide()
                pStepProg = Nothing
            End If
            If progressDialog2 IsNot Nothing Then
                progressDialog2.HideDialog()
                progressDialog2 = Nothing
            End If
        End Try
    End Sub

    Private Sub AddLayersToMap()
        Dim Basin_Name As String = ""
        Dim cboSelectedAoi = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)

        'BA_ActivateMapFrame BA_DefaultMapName
        Dim response As Integer = BA_AddLayerstoMapFrame(My.ThisApplication, My.Document, AOIFolderBase, AOI_HasSNOTEL, AOI_HasSnowCourse, Scenario1Map_Flag, Scenario2Map_Flag)
        BA_AddMapElements(My.Document, cboSelectedAoi.getValue, "Subtitle BAGIS")
        response = BA_DisplayMap(My.Document, 1, Basin_Name, cboSelectedAoi.getValue, Map_Display_Elevation_in_Meters,
                                  "Elevation Distribution")
        BA_RemoveLayersfromLegend(My.Document)
    End Sub

    Private Sub CmdMaps_Click(sender As System.Object, e As System.EventArgs) Handles CmdMaps.Click
        'Warn if there are maps in the viewer that may be overwritten
        Dim aMap As IMap = My.Document.FocusMap()
        If aMap.LayerCount > 0 Then
            Dim warnMessage As String = "Adding the maps to the display will overwrite the current arrangement of data layers. " +
                "This action cannot be undone." + vbCrLf + "Do you wish to continue ?"
            Dim res As DialogResult = MessageBox.Show(warnMessage, "BAGIS", MessageBoxButtons.YesNo)
            If res <> DialogResult.Yes Then
                Exit Sub
            End If
        End If
        'Ensure default map frame name is set before trying to build map
        Dim response As Integer = BA_SetDefaultMapFrameName(BA_MAPS_DEFAULT_MAP_NAME, My.Document)
        response = BA_SetMapFrameDimension(BA_MAPS_DEFAULT_MAP_NAME, 1, 2, 7.5, 9, True)
        AddLayersToMap()
        Call BA_Enable_MapFlags(True)
        'Set elevation distro as the currently displayed map
        Dim dockWindowAddIn = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of FrmPublishMapPackage.AddinImpl)(My.ThisAddIn.IDs.FrmPublishMapPackage)
        Dim frmMapPackage As FrmPublishMapPackage = dockWindowAddIn.UI
        frmMapPackage.CurrentMap = BAGIS_ClassLibrary.BA_ExportMapElevPdf
        'Enable publish map buttons
        Dim PublishMapButton As BtnPublishMap = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of BtnPublishMap)(My.ThisAddIn.IDs.BtnPublishMap)
        PublishMapButton.selectedProperty = True
        Dim PublishMapPackageButton As BtnPublishMapPackage =
            ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of BtnPublishMapPackage)(My.ThisAddIn.IDs.BtnPublishMapPackage)
        PublishMapPackageButton.selectedProperty = True
        MsgBox("Please use the menu items to view maps!")
        Me.Close()
    End Sub

    Private Sub CmdTables_Click(sender As System.Object, e As System.EventArgs) Handles CmdTables.Click

        '=================================
        'Update map parameters file
        '=================================
        Dim filepath As String, FileName As String

        'check if map_parameters.txt file exists
        filepath = BA_GetPath(AOIFolderBase, PublicPath.Maps)
        FileName = BA_MapParameterFile 'i.e., map_parameters.txt
        Dim response As Integer = SaveMapParameters(filepath, FileName)
        If response <= 0 Then
            MessageBox.Show("Error! Unable to update map parameter file. Please report the error to the developer.", "BAGIS")
            Exit Sub
        End If

        Dim oMapsSettings As MapsSettings = BA_ReadMapSettings()

        Dim EMinValue As Double = Convert.ToDouble(txtMinElev.Text)
        Dim EMaxValue As Double = Convert.ToDouble(txtMaxElev.Text)

        BA_GenerateTables(oMapsSettings, EMaxValue, EMinValue, True)
    End Sub

    Private Sub CmboxAspect_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxAspect.SelectedIndexChanged
        If Flag_ElevationZone And Flag_PrecipitationZone Then
            CmdGenerate.Enabled = True
        Else
            CmdGenerate.Enabled = False
        End If
        CmdMaps.Enabled = False
        CmdTables.Enabled = False
    End Sub

    Private Sub BtnPartition_Click(sender As System.Object, e As System.EventArgs) Handles CmdPartition.Click
        SetPrecipPathInfo()

        Dim cellSize As Double = BA_CellSize(PrecipPath, PRISMRasterName)
        Dim snapRasterPath As String = PrecipPath + "\" + PRISMRasterName
        Dim frmPartitionRaster As FrmElevPrecip = New FrmElevPrecip(m_partitionRasterPath, snapRasterPath, _
                                                                              cellSize, PARTITION_MODE, OptAggZone.Checked)
        frmPartitionRaster.ShowDialog()

        If Not String.IsNullOrEmpty(m_partitionRasterPath) AndAlso Not String.IsNullOrEmpty(frmPartitionRaster.RasterPath) Then
            'The new partition layer is different; Enable generate
            If Not m_partitionRasterPath.Equals(frmPartitionRaster.RasterPath) Then
                EnableGenerate(True)
            End If
        Else
            'The partition layer is new; Enable generate
            If Not String.IsNullOrEmpty(frmPartitionRaster.RasterPath) Then
                EnableGenerate(True)
            End If
        End If

        'If no new partition raster was selected
        If String.IsNullOrEmpty(frmPartitionRaster.RasterPath) Then
            ' Delete existing partition raster if it exists
            Dim deleteFileName As String = BA_FindElevPrecipRasterName(BA_RasterPartPrefix)
            If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + deleteFileName, _
                              WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                BA_RemoveRasterFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), deleteFileName)
            End If
        End If

        LblPartitionLayer.Text = frmPartitionRaster.RasterName
        m_partitionRasterPath = frmPartitionRaster.RasterPath

    End Sub

    Private Sub CmdClearPartition_Click(sender As System.Object, e As System.EventArgs) Handles CmdClearPartition.Click
        ' Delete existing partition raster if it exists
        Dim parentFolder As String = "PleaseReturn"
        Dim deleteFileName As String = BA_GetBareName(m_partitionRasterPath, parentFolder)
        If BA_File_Exists(m_partitionRasterPath, _
                          WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(parentFolder, deleteFileName)
        End If
        m_partitionRasterPath = Nothing
        LblPartitionLayer.Text = Nothing
    End Sub

    '** Note: this logic is duplicated two properties of the MapsSettings object **
    Private Sub SetPrecipPathInfo()
        If CmboxPrecipType.SelectedIndex = 0 Then  'read direct Annual PRISM raster
            PrecipPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism)
            PRISMRasterName = AOIPrismFolderNames.annual.ToString
        ElseIf CmboxPrecipType.SelectedIndex > 0 And CmboxPrecipType.SelectedIndex < 5 Then 'read directly Quarterly PRISM raster
            PrecipPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism)
            PRISMRasterName = BA_GetPrismFolderName(CmboxPrecipType.SelectedIndex + 12)
        Else 'sum individual monthly PRISM rasters
            PrecipPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis)
            PRISMRasterName = BA_TEMP_PRISM
        End If
    End Sub

    Private Function CreateAspectLayerForElevPrecip(ByVal PrecipPath As String, ByVal PRISMRasterName As String) As String
        Dim prismCellSize As Double = BA_CellSize(PrecipPath, PRISMRasterName)
        Dim aspectCellSize As Double = BA_CellSize(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), BA_RasterAspectZones)
        Dim outputFilePath As String = Nothing
        Try
            If aspectCellSize < prismCellSize Then
                'Execute focal statistics to account for differing cell sizes
                '"Rectangle 5 5 Cell"
                Dim neighborhood As String = "Rectangle " & prismCellSize & " " & prismCellSize & " Map"
                Dim strFilterType As String = StatisticsFieldName.MAJORITY.ToString
                Dim success As BA_ReturnCode = BA_FocalStatistics_CellSize(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_RasterAspectZones, _
                                                     BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_AspectPrec, _
                                                     neighborhood, strFilterType, PrecipPath & "\" & PRISMRasterName, Convert.ToString(prismCellSize))
                If success <> BA_ReturnCode.Success Then
                    MessageBox.Show("An error occurred while resampling the aspect layer for elevation precipitation")
                    Return Nothing
                Else
                    outputFilePath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_AspectPrec
                End If
            Else
                outputFilePath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_RasterAspectZones
            End If
            Return outputFilePath
        Catch ex As Exception
            Debug.Print("CreateAspectLayerForElevPrecip Exception: " & ex.Message)
            Return Nothing
        End Try
    End Function

    'sourceField = BA_RasterValu
    'targetField = BA_Precip
    'fieldType - esriFieldType.esriFieldTypeDouble
    Private Function RenameTableAttribute(ByVal filePath As String, ByVal fileName As String, ByVal sourceField As String,
                                          ByVal targetField As String, ByVal fieldType As esriFieldType) As BA_ReturnCode

        'open raster attribute table
        Dim pTable As ITable
        Dim pFld As IFieldEdit
        Dim success As BA_ReturnCode = BA_ReturnCode.OtherError

        'add field
        Try
            pTable = BA_OpenTableFromGDB(filePath, fileName)
            If pTable IsNot Nothing Then
                Dim idxTarget = pTable.FindField(targetField)
                If idxTarget < 0 Then
                    pFld = New Field
                    pFld.Name_2 = targetField
                    pFld.Type_2 = fieldType
                    pFld.Required_2 = False

                    ' Add field
                    pTable.AddField(pFld)
                End If

                Dim expressType As String = "VB"
                Dim expression As String = "[" + sourceField + "]"
                Dim outTable As String = filePath & "\" & fileName
                success = BA_CalculateField(outTable, targetField, expression, expressType)
                If success = BA_ReturnCode.Success Then
                    success = BA_DeleteFieldFromTable(filePath, fileName, sourceField)
                End If
            End If
            Return success
        Catch ex As Exception
            Debug.Print("RenameTableAttribute: " & ex.Message)
            Return BA_ReturnCode.UnknownError
        Finally
            pTable = Nothing
            pFld = Nothing
        End Try

    End Function

    Private Sub ChkRepresentedPrecip_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ChkRepresentedPrecip.CheckedChanged
        OptAggPrism.Enabled = ChkRepresentedPrecip.Checked
        OptAggZone.Enabled = ChkRepresentedPrecip.Checked
        CmdPartition.Enabled = ChkRepresentedPrecip.Checked
        CmdClearPartition.Enabled = ChkRepresentedPrecip.Checked
        CmdZonalAggregate.Enabled = ChkRepresentedPrecip.Checked
        If ChkRepresentedPrecip.Checked Then
            If Not String.IsNullOrEmpty(m_partitionRasterPath) Then
                Dim partitionFileName As String = BA_GetBareName(m_partitionRasterPath)
                LblPartitionLayer.Text = partitionFileName.Substring(BA_RasterPartPrefix.Length)
            End If
            If Not String.IsNullOrEmpty(m_zoneRasterPath) Then
                Dim zonesFileName As String = BA_GetBareName(m_zoneRasterPath)
                LblZonalLayer.Text = zonesFileName.Substring(BA_ZonesRasterPrefix.Length)
            End If
        Else
            LblPartitionLayer.Text = Nothing
            LblZonalLayer.Text = Nothing
        End If
    End Sub

    Private Sub OptAggPrism_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles OptAggPrism.CheckedChanged
        If OptAggPrism.Checked Then
            LblZonalLayer.Text = Nothing
            CmdZonalAggregate.Enabled = False
            'Delete zonal layer, if it exists
            If Not String.IsNullOrEmpty(m_zoneRasterPath) Then
                If BA_File_Exists(m_zoneRasterPath, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    Dim parentPath As String = "PleaseReturn"
                    Dim fileName As String = BA_GetBareName(m_zoneRasterPath, parentPath)
                    BA_RemoveRasterFromGDB(parentPath, fileName)
                    m_zoneRasterPath = Nothing
                    EnableGenerate(True)
                End If
            End If
        End If
    End Sub

    Private Sub OptAggZone_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles OptAggZone.CheckedChanged
        If OptAggZone.Checked Then
            If Not String.IsNullOrEmpty(m_zoneRasterPath) Then
                If BA_File_Exists(m_zoneRasterPath, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    Dim zonesFileName As String = BA_GetBareName(m_zoneRasterPath)
                    LblZonalLayer.Text = zonesFileName.Substring(BA_ZonesRasterPrefix.Length)
                    EnableGenerate(True)
                End If
            End If
            CmdZonalAggregate.Enabled = True
        End If
    End Sub

    Private Sub CmdClearZonal_Click(sender As System.Object, e As System.EventArgs)
        ' Delete existing partition raster if it exists
        Dim parentFolder As String = "PleaseReturn"
        Dim deleteFileName As String = BA_GetBareName(m_zoneRasterPath, parentFolder)
        If BA_File_Exists(m_zoneRasterPath, _
                          WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(parentFolder, deleteFileName)
        End If
        m_zoneRasterPath = Nothing
        LblZonalLayer.Text = Nothing
    End Sub

    Private Sub CmdZonalAggregate_Click(sender As System.Object, e As System.EventArgs) Handles CmdZonalAggregate.Click
        Dim frmElevPrecip As FrmElevPrecip = New FrmElevPrecip(m_zoneRasterPath, Nothing, _
                                                               -1, ZONE_MODE, OptAggZone.Checked)
        frmElevPrecip.ShowDialog()
        If Not String.IsNullOrEmpty(m_zoneRasterPath) Then
            'The new zones layer is different; Enable generate
            If Not m_zoneRasterPath.Equals(frmElevPrecip.RasterPath) Then
                EnableGenerate(True)
            End If
        Else
            'The zones layer is new; Enable generate
            If Not String.IsNullOrEmpty(frmElevPrecip.RasterPath) Then
                EnableGenerate(True)
            End If
        End If

        If String.IsNullOrEmpty(frmElevPrecip.RasterPath) Then
            ' Delete existing partition raster if it exists
            Dim deleteFileName As String = BA_FindElevPrecipRasterName(BA_ZonesRasterPrefix)
            If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + deleteFileName, _
                              WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                BA_RemoveRasterFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), deleteFileName)
            End If
            LblZonalLayer.Text = frmElevPrecip.RasterName
            m_zoneRasterPath = frmElevPrecip.RasterPath
        Else
            LblZonalLayer.Text = frmElevPrecip.RasterName
            m_zoneRasterPath = frmElevPrecip.RasterPath
        End If
    End Sub

    Private Function GenerateZonePrecipElevLayers(ByVal pStepProg As IStepProgressor, ByVal AspectDirectionsNumber As Integer, _
                                                  ByVal AspIntervalList As BA_IntervalList()) As BA_ReturnCode
        'Zonal statistics as table for DEM
        Dim zoneFileName As String = BA_GetBareName(m_zoneRasterPath)
        Dim demLayerPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces, True) + BA_EnumDescription(MapsFileName.filled_dem_gdb)

        Dim success As BA_ReturnCode = BA_ZonalStatisticsAsTable(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), zoneFileName, BA_FIELD_VALUE, _
                                                                 demLayerPath, BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), _
                                                                 BA_TablePrecMeanElev, demLayerPath, _
                                                                 StatisticsTypeString.MEAN)
        If success = BA_ReturnCode.Success Then
            'Rename MEAN field to precmeanelev
            success = RenameTableAttribute(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), BA_TablePrecMeanElev, _
                                           StatisticsTypeString.MEAN.ToString, BA_RasterPrecMeanElev, esriFieldType.esriFieldTypeDouble)

        End If
        If success = BA_ReturnCode.Success Then
            ' Zonal statistics as table for PRISM
            Dim tempPrismTableName As String = "tmpPrism"
            success = BA_ZonalStatisticsAsTable(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), zoneFileName, BA_FIELD_VALUE, _
                                                                 PrecipPath + "\" + PRISMRasterName, BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), _
                                                                 tempPrismTableName, demLayerPath, _
                                                                 StatisticsTypeString.MEAN)
            If success = BA_ReturnCode.Success Then
                'Join PRISM to DEM table and copy field
                success = BA_JoinField(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_TablePrecMeanElev, BA_FIELD_VALUE, _
                                       BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & tempPrismTableName, _
                                       BA_FIELD_VALUE, StatisticsTypeString.MEAN.ToString)
                If success = BA_ReturnCode.Success Then
                    'Rename MEAN field to PRISMRasterName + "_1"
                    success = RenameTableAttribute(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), BA_TablePrecMeanElev, _
                                                   StatisticsTypeString.MEAN.ToString, PRISMRasterName + "_1", esriFieldType.esriFieldTypeDouble)

                    'Delete temporary PRISM table
                    success = BA_Remove_TableFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tempPrismTableName)
                End If
            End If
            If success = BA_ReturnCode.Success Then
                pStepProg.Message = "Creating Aspect layer for elev-precip analysis..."
                pStepProg.Step()
                Dim tempAspectTableName As String = "tmpAspect"
                ' Zonal statistics as table for Aspect
                success = BA_ZonalStatisticsAsTable(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), zoneFileName, BA_FIELD_VALUE, _
                                                      BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_RasterAspectZones, _
                                                      BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tempAspectTableName, _
                                                      demLayerPath, StatisticsTypeString.MAJORITY)

                If success = BA_ReturnCode.Success Then
                    'Join ASPECT to DEM table and copy field
                    success = BA_JoinField(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_TablePrecMeanElev, BA_FIELD_VALUE, _
                                           BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & tempAspectTableName, _
                                           BA_FIELD_VALUE, StatisticsTypeString.MAJORITY.ToString)
                    'set reclass
                    success = BA_UpdateTableAttributes(AspIntervalList, BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), _
                                                       BA_TablePrecMeanElev, BA_FIELD_ASPECT, StatisticsTypeString.MAJORITY.ToString, esriFieldType.esriFieldTypeString)
                    'rename MAJORITY field in case there is a partition raster
                    success = RenameTableAttribute(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), BA_TablePrecMeanElev, _
                                                   StatisticsTypeString.MAJORITY.ToString, BA_AspectPrec, esriFieldType.esriFieldTypeDouble)

                    'Delete temporary ASPECT table
                    success = BA_Remove_TableFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tempAspectTableName)
                End If
            End If
            If success = BA_ReturnCode.Success AndAlso Not String.IsNullOrEmpty(LblPartitionLayer.Text) Then
                pStepProg.Message = "Creating attribute layer for elev-precip analysis..."
                pStepProg.Step()

                Dim tempAttribTableName As String = "tmpAttrib"
                ' Zonal statistics as table for attribute layer
                success = BA_ZonalStatisticsAsTable(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), zoneFileName, BA_FIELD_VALUE, _
                                                    m_partitionRasterPath, BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), _
                                                    tempAttribTableName, demLayerPath, StatisticsTypeString.MAJORITY)

                If success = BA_ReturnCode.Success Then
                    'Join attribute layer to DEM table and copy field
                    success = BA_JoinField(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & BA_TablePrecMeanElev, BA_FIELD_VALUE, _
                                           BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) & tempAttribTableName, _
                                           BA_FIELD_VALUE, StatisticsTypeString.MAJORITY.ToString)

                    'rename MAJORITY field
                    Dim partitionFileName As String = BA_UNKNOWN
                    If Not String.IsNullOrEmpty(LblPartitionLayer.Text) Then
                        partitionFileName = BA_GetBareName(m_partitionRasterPath)
                    End If
                    success = RenameTableAttribute(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), BA_TablePrecMeanElev, _
                                                   StatisticsTypeString.MAJORITY.ToString, partitionFileName, esriFieldType.esriFieldTypeDouble)
                    'Delete temporary attribute table
                    success = BA_Remove_TableFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis), tempAttribTableName)

                End If
            End If
        End If
        Return success
    End Function

    Private Sub ChkPrecipAoiTable_CheckedChanged(sender As Object, e As System.EventArgs) Handles ChkPrecipAoiTable.CheckedChanged
        If ChkPrecipAoiTable.Checked Then
            LblAttribLayerExists.Text = "Elev-Precip AOI table - Ready"
        Else
            LblAttribLayerExists.Text = "Elev-Precip AOI table - ?"
        End If
    End Sub


    Private Sub ChkPrecipSitesLayer_CheckedChanged(sender As Object, e As System.EventArgs) Handles ChkPrecipSitesLayer.CheckedChanged
        If ChkPrecipSitesLayer.Checked Then
            LblSitesLayerExists.Text = "Elev-Precip Sites layer - Ready"
        Else
            LblSitesLayerExists.Text = "Elev-Precip Sites layer - ?"
        End If
    End Sub

    Private Sub CheckForSitesLayers(ByRef hasSnotelLayer As Boolean, ByRef hasSnowCourseLayer As Boolean)
        'Check to see if SNOTEL Shapefile Exist
        If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers, True) & BA_SNOTELSites, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
            hasSnotelLayer = True
        Else
            hasSnotelLayer = False
        End If

        'Check to see if SnowCourse Shapefile Exist
        If BA_File_Exists(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers, True) & BA_SnowCourseSites, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
            hasSnowCourseLayer = True
        Else
            hasSnowCourseLayer = False
        End If
    End Sub

    Private Sub EnableGenerate(ByVal enable As Boolean)
        'We only want to override the normal order of buttons if maps/tables already activated
        If CmdTables.Enabled = enable Then
            CmdTables.Enabled = Not enable
            CmdMaps.Enabled = Not enable
            CmdGenerate.Enabled = enable
            'Also note that layers are not longer read
            ChkPrecipAoiTable.Checked = Not enable
            ChkPrecipSitesLayer.Checked = Not enable
        Else
            CmdGenerate.Enabled = enable
        End If
    End Sub

End Class