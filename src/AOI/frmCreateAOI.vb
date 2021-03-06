﻿Imports ESRI.ArcGIS.Geoprocessing
Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
Imports BAGIS_ClassLibrary
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.GeoAnalyst
Imports ESRI.ArcGIS.SpatialAnalyst
Imports System.Windows.Forms
Imports ESRI.ArcGIS.Desktop.AddIns
Imports System.IO
Imports ESRI.ArcGIS.DataSourcesGDB
Imports System.ComponentModel
Imports System.Text

Public Class frmCreateAOI
    Private Sub CmbRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRun.Click
        Me.Hide()
        Dim nstep As Integer
        Dim ListLayerCount As Integer
        'ListLayerCount = frmSettings.lstLayers.listcount

        Dim comReleaser As ESRI.ArcGIS.ADF.ComReleaser = New ESRI.ArcGIS.ADF.ComReleaser()
        'Dim pDEUtility As IDEUtilities = New DEUtilities
        BA_SetSettingPath()
        BA_ReadBAGISSettings(BA_Settings_Filepath)
        ListLayerCount = BA_SystemSettings.listCount

        Dim internalLayerCount As Integer = 0

        If BA_SystemSettings.GenerateAOIOnly Then
            internalLayerCount = 6
        Else
            internalLayerCount = 25
        End If

        nstep = internalLayerCount + ListLayerCount 'step counter for frmmessage

        Dim cboSelectedAOI = AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        Dim AOIName As String = cboSelectedAOI.getValue()

        AOIFolderBase = BasinFolderBase & "\" & AOIName

        Dim sourceSurfGDB As String = BasinFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Surfaces)
        Dim sourceAOIGDB As String = BasinFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Aoi)
        Dim destSurfGDB As String = AOIFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Surfaces)
        Dim destAOIGDB As String = AOIFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Aoi)
        Dim destLayersGDB As String = AOIFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Layers)
        Dim destPRISMGDB As String = AOIFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Prism)

        Dim pProgD As IProgressDialog2 = BA_GetAnimationProgressor(My.ArcMap.Application.hWnd, "Initializing process...", "Creating AOI")
        System.Windows.Forms.Application.DoEvents()

        'verify AOI buffer distance
        If ChkAOIBuffer.Checked = True Then
            If Not IsNumeric(txtBufferD.Text) Then
                MsgBox("Buffer distance must be numeric! Program stopped!")
                pProgD.HideDialog()
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)
                Exit Sub
            Else
                BA_AOIClipBuffer = CDbl(txtBufferD.Text) 'Unit is Meter
                If BA_AOIClipBuffer <= 0 Then BA_AOIClipBuffer = 100 'default buffer distance
            End If
        End If
        'verify PRISM buffer distance
        If ChkAOIBuffer.Checked = True Then
            If Not IsNumeric(TxtPrismBufferD.Text) Then
                MsgBox("PRISM buffer distance must be numeric! Program stopped!")
                pProgD.HideDialog()
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)
                Exit Sub
            Else
                BA_PRISMClipBuffer = CDbl(TxtPrismBufferD.Text) 'Unit is Meter
                If BA_PRISMClipBuffer <= 0 Then BA_PRISMClipBuffer = 1500 'default buffer distance
            End If
        End If

        'The BA_Create_Output_Folders function can delete the file structure if it exists
        Dim response As Integer
        'response = BA_Create_Output_Folders(AOIFolderBase, True)
        For Each pName In [Enum].GetValues(GetType(GeodatabaseNames))
            Dim EnumConstant As [Enum] = pName
            Dim fi As Reflection.FieldInfo = EnumConstant.GetType().GetField(EnumConstant.ToString())
            Dim aattr() As DescriptionAttribute = _
                DirectCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
            Dim gdbName As String = aattr(0).Description
            Dim gdbpath As String = AOIFolderBase & "\" & gdbName

            If BA_Workspace_Exists(gdbpath) Then
                If BA_DeleteGeodatabase(AOIFolderBase & "\" & gdbName, My.ArcMap.Document) <> BA_ReturnCode.Success Then
                    MsgBox("Cannot delete existing Geodatabase folders", "Unknown Error")
                End If
            End If
        Next

        Dim success As BA_ReturnCode = BA_CreateGeodatabaseFolders(AOIFolderBase, FolderType.AOI)

        If success <> BA_ReturnCode.Success Then
            MsgBox("Unable to create GDBs! Please check disk space")
            pProgD.HideDialog()
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)
            Exit Sub
        End If

        pProgD.Description = "Creating GDB Folders"
        System.Windows.Forms.Application.DoEvents()

        'create maps folder to save MXDs
        Dim mappath As String = AOIFolderBase & "\maps"
        If Not BA_Workspace_Exists(mappath) Then
            mappath = BA_CreateFolder(AOIFolderBase, "maps")
        End If

        'set pourpoint filename and save pourpoint as a shapefile
        Dim unsnappedppname As String
        If ChkSnapPP.Checked Then
            unsnappedppname = "unsnappedpp"
        Else
            unsnappedppname = BA_POURPOINTCoverage
        End If

        If BA_Graphic2FeatureClass(destAOIGDB, unsnappedppname) <> 1 Then
            MsgBox("Unable to save Pour Point")
        End If

        pProgD.Description = "Delineating AOI Boundaries..."
        System.Windows.Forms.Application.DoEvents()

        If ChkSnapPP.Checked Then 'snap the pourpoint
            'Snap Pour Point
            Dim snapFileName As String = "tmpSnap"
            Dim extractFileName As String = "tmpExtract"
            success = BA_SnapPourPoint(sourceSurfGDB + "\" + BA_EnumDescription(MapsFileName.flow_accumulation_gdb),
                                       destAOIGDB + "\" + unsnappedppname,
                                       txtSnapD.Text, destAOIGDB + "\" + snapFileName)

            If success = BA_ReturnCode.Success Then
                'Query the Previous Raster to Include only the PP location
                'Set where_clause to > -1 (Pour Point Value) AGS 10.5 returns a different value for PP location than 10.2.2
                success = BA_ExtractByAttributes(destAOIGDB + "\" + snapFileName, Nothing,
                                                 destAOIGDB + "\" + extractFileName, "VALUE > -1")
                If success = BA_ReturnCode.Success Then
                    success = BA_RasterToPoint(destAOIGDB + "\" + extractFileName, destAOIGDB + "\" + BA_POURPOINTCoverage,
                                               BA_FIELD_VALUE)
                    If success = BA_ReturnCode.Success Then
                        success = BA_Watershed(sourceSurfGDB + "\" + BA_EnumDescription(MapsFileName.flow_direction_gdb),
                                               destAOIGDB + "\" + unsnappedppname, destAOIGDB + "\" + BA_AOIExtentRaster)
                    End If
                End If
            End If
            ' Delete temporary files
            If BA_File_Exists(destAOIGDB + "\" + snapFileName, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                BA_RemoveRasterFromGDB(destAOIGDB, snapFileName)
            End If
            If BA_File_Exists(destAOIGDB + "\" + extractFileName, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                BA_RemoveRasterFromGDB(destAOIGDB, extractFileName)
            End If
        Else
            success = BA_Watershed(sourceSurfGDB + "\" + BA_EnumDescription(MapsFileName.flow_direction_gdb),
                                   destAOIGDB + "\" + unsnappedppname, destAOIGDB + "\" + BA_AOIExtentRaster)
        End If

        'Add pourpoint to Map
        Dim pMColor As IRgbColor = New RgbColor
        pMColor.RGB = RGB(0, 255, 255)

        If BA_File_Exists(destAOIGDB & "\" & BA_POURPOINTCoverage, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
            response = BA_MapDisplayPointMarkers(My.ArcMap.Application, destAOIGDB & "\" & BA_POURPOINTCoverage, MapsLayerName.Pourpoint, pMColor, MapsMarkerType.Pourpoint)
        End If

        pProgD.Description = "Saving AOI Boundaries..."
        System.Windows.Forms.Application.DoEvents()

        Dim DisplayName As String
        Dim comboBox = AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        DisplayName = "AOI " & comboBox.getValue

        'Convert watershed to polygon
        success = BA_Raster2Polygon_GP(destAOIGDB + "\" + BA_AOIExtentRaster, destAOIGDB + "\" + BA_AOIExtentCoverage,
                                       destAOIGDB + "\" + BA_AOIExtentRaster)

        'update the attribute table of the AOI using basin name
        response = BA_AddAOIVectorAttributes(destAOIGDB, comboBox.getValue())

        'add AOI extent (aoi_v) to map
        Dim pColor As IRgbColor = pDisplayColor
        response = BA_AddExtentLayer(My.ArcMap.Document, destAOIGDB & "\" & BA_AOIExtentCoverage, pColor, True, DisplayName, 0, 2)
        If response < 0 Then 'error occurred
            pProgD.HideDialog()
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)
            MsgBox("Unable to add AOI polygon to ArcMap! Program stopped.")

            Me.Close()
            Exit Sub
        End If

        'get AOI area and prompt if the user wants to continue
        Dim AOIArea As Double, AOIArea_String As String
        AOIArea = BA_GetShapeArea(destAOIGDB & "\" & BA_AOIExtentCoverage) / 1000000 'the shape unit is in sq meters, converted to sq km

        If AOIArea < 0 Then 'error when getting the area of aoi
            pProgD.HideDialog()
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)
            MsgBox("Unable to get the area of the AOI! Program stopped.")

            Me.Close()
            Exit Sub
        End If

        If AOIArea < BA_MinimumAOIArea Then
            pProgD.HideDialog()
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)
            MsgBox("The size of the AOI is too small!" & Chr(13) & Chr(10) & "Please select a new pour point location or use the auto snapping option.")

            Me.Close()
            Exit Sub
        End If

        AOI_ReferenceArea = Format(AOI_ReferenceArea, "#0.00")
        Dim tempRefArea As String = CStr(AOI_ReferenceArea)
        If UCase(BA_SystemSettings.PourAreaField) = "NO DATA" Or AOI_ReferenceArea = 0 Then
            tempRefArea = "Not specified"
        End If

        Dim tempAreaUnit As String = BA_SystemSettings.PourAreaUnit
        If UCase(tempAreaUnit) = "UNKNOWN" Or AOI_ReferenceArea = 0 Then
            tempAreaUnit = ""
        End If

        AOIArea_String = "The area of AOI is:"
        AOIArea_String = AOIArea_String & Chr(13) & Chr(10) & Chr(9) & " " & Format(AOIArea, "#0.00") & Chr(9) & " Square Km"
        AOIArea_String = AOIArea_String & Chr(13) & Chr(10) & Chr(9) & " " & Format(AOIArea * 247.1044, "#0.00") & Chr(9) & " Acre"
        AOIArea_String = AOIArea_String & Chr(13) & Chr(10) & Chr(9) & " " & Format(AOIArea * 0.3861022, "#0.00") & Chr(9) & " Square Miles"
        AOIArea_String = AOIArea_String & Chr(13) & Chr(10) & Chr(9) & " " & Chr(10) & "Reference Area for AOI is: "
        AOIArea_String = AOIArea_String & Chr(13) & Chr(10) & Chr(9) & " " & tempRefArea & Chr(9) & " " & tempAreaUnit
        AOIArea_String = AOIArea_String & Chr(13) & Chr(10) & Chr(9) & " " & Chr(10) & "Do you want to use this AOI boundary?"

        pProgD.HideDialog()
        ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pProgD)

        response = MsgBox(AOIArea_String, vbYesNo)

        If response = vbNo Then 'user abandon the process
            Me.Close()
            Exit Sub
        End If

        'update the area information in the pourpoint shapefile
        AOI_ShapeArea = AOIArea
        AOI_ShapeUnit = "Square Km"
        AOI_ReferenceUnit = BA_SystemSettings.PourAreaUnit
        response = BA_UpdatePPAttributes(destAOIGDB)

        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, nstep)
        Dim progressDialog2 As IProgressDialog2 = BA_GetProgressDialog(pStepProg, "Creating AOI...", "Creating AOI")
        System.Windows.Forms.Application.DoEvents()

        'create PRISM clipping buffered polygon
        If BA_PRISMClipBuffer <= 0 Then BA_PRISMClipBuffer = 1000 '1000 Meters by default

        'use Buffer GP to perform buffer and save the result as a shapefile
        Dim GP As ESRI.ArcGIS.Geoprocessor.Geoprocessor = New ESRI.ArcGIS.Geoprocessor.Geoprocessor()
        Dim BufferTool As ESRI.ArcGIS.AnalysisTools.Buffer = New ESRI.ArcGIS.AnalysisTools.Buffer
        With BufferTool
            .in_features = destAOIGDB & "\" & BA_AOIExtentCoverage
            .buffer_distance_or_field = BA_PRISMClipBuffer
            .dissolve_option = "ALL"
            .out_feature_class = AOIFolderBase & "\" & BA_PRISMClipAOI & ".shp"
        End With
        GP.AddOutputsToMap = False
        GP.Execute(BufferTool, Nothing)

        'save the buffered AOI as a shapefile and then import it into the GDB
        'to prevent a bug when the buffer distance exceed the xy domain limits of the GDB
        'response = BA_Graphic2Shapefile(AOIFolderBase, BA_PRISMClipAOI)

        'Copy the temporary line shape file to the aoi.gdb
        Dim retVal As BA_ReturnCode = BA_ReturnCode.UnknownError
        retVal = BA_ConvertShapeFileToGDB(AOIFolderBase, BA_StandardizeShapefileName(BA_PRISMClipAOI, True, False), destAOIGDB, BA_PRISMClipAOI)
        'create a raster version of the buffered AOI
        Dim Cellsize As Double = 0
        Dim fullLayerPath As String = destAOIGDB & "\" & BA_AOIExtentRaster
        Dim rasterStat As IRasterStatistics = BA_GetRasterStatsGDB(fullLayerPath, Cellsize)
        retVal = BA_Feature2RasterGP(AOIFolderBase & BA_StandardizeShapefileName(BA_PRISMClipAOI, True, True), destAOIGDB & BA_EnumDescription(PublicPath.AoiPrismGrid), "ID", Cellsize, fullLayerPath)
        BA_Remove_Shapefile(AOIFolderBase, BA_StandardizeShapefileName(BA_PRISMClipAOI, False))

        If Not ChkAOIBuffer.Checked Then 'buffer the AOI polygon for clipping
            BA_AOIClipBuffer = 1 'one meter buffer to dissolve polygons connected at a point
        End If

        With BufferTool
            .in_features = destAOIGDB & "\" & BA_AOIExtentCoverage
            .buffer_distance_or_field = BA_AOIClipBuffer
            .dissolve_option = "ALL"
            .out_feature_class = AOIFolderBase & "\" & BA_BufferedAOIExtentCoverage & ".shp"
        End With
        GP.AddOutputsToMap = False
        GP.Execute(BufferTool, Nothing)

        BufferTool = Nothing
        GP = Nothing

        retVal = BA_ConvertShapeFileToGDB(AOIFolderBase, BA_StandardizeShapefileName(BA_BufferedAOIExtentCoverage, True, False), destAOIGDB, BA_BufferedAOIExtentCoverage)
        retVal = BA_Feature2RasterGP(AOIFolderBase & BA_StandardizeShapefileName(BA_BufferedAOIExtentCoverage, True, True), destAOIGDB & BA_EnumDescription(PublicPath.AoiBufferedGrid), "ID", Cellsize, fullLayerPath)
        'BA_Remove_Shapefile(AOIFolderBase, BA_StandardizeShapefileName(BA_BufferedAOIExtentCoverage, True))
        success = BA_DeleteLayer_GP(AOIFolderBase + BA_StandardizeShapefileName(BA_BufferedAOIExtentCoverage, True, True))

        '=========================
        'start the clipping preparation
        '=========================
        'pStepProg.Show()
        pStepProg.Message = "Clipping DEM layer... (step 1 of " & nstep & ")"
        pStepProg.Step()
        System.Windows.Forms.Application.DoEvents()

        Dim inputraster As String
        Dim DemFilePath As String
        DemFilePath = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.filled_dem_gdb)
        If Not BA_File_Exists(DemFilePath, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            pStepProg.Hide()
            progressDialog2.HideDialog()
            pStepProg = Nothing
            progressDialog2 = Nothing
            MsgBox("Source dem was not found. DEM statistics cannot be obtained!" & vbCrLf & "Clipping was faield", "Missing Data")

            BA_DeleteGeodatabase(AOIFolderBase, My.ArcMap.Document)
            If BA_Workspace_Exists(AOIFolderBase) Then 'cannot delete the folder
                MsgBox("Cannot delete the AOI! It's probably caused by a file lock in the Geodatabase. Please restart ArcGIS and repeat the process.")
            End If

            Me.Close()
            Exit Sub
        End If

        Dim pRasterStats As IRasterStatistics = BA_GetRasterStatsGDB(DemFilePath, Cellsize)
        pRasterStats = Nothing

        If Cellsize = 0 Then 'error reading raster dem file
            pStepProg.Hide()
            progressDialog2.HideDialog()
            pStepProg = Nothing
            progressDialog2 = Nothing
            MsgBox("Unable to read basin DEM information. Program stopped.")

            BA_DeleteGeodatabase(AOIFolderBase, My.ArcMap.Document)
            If BA_Workspace_Exists(AOIFolderBase) Then 'cannot delete the folder
                MsgBox("Cannot delete the AOI! It is probably caused by a file lock in the Geodatabase. Please restart ArcGIS and repeat the process.")
            End If

            Me.Close()
            Exit Sub
        End If

        '======================
        'Clip and save DEM and Filled DEM
        '======================
        ' Open and Clip DEM
        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.dem_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.dem_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage, False)
        If response <= 0 Then
            MsgBox("Clipping DEM failed! Return value = " & response & ".")
        End If

        ' Open and Clip Filled DEM
        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.filled_dem_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.filled_dem_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage, False)
        If response <= 0 Then
            MsgBox("Clipping FILLED DEM failed! Return value = " & response & ".")
        End If

        '___________________________________________________________________________________________________________
        '_____________________________________________________________________________________________________________
        'creating weasel source file type and rasters are no longer required
        'make the aoi compatiable with a basin file structure
        'read the basin dem status file
        'Dim demstring As String
        'demstring = Check_Folder_Type(BasinFolderBase, BA_Basin_Type)

        'response = BA_Create_FolderType_File(AOIFolderBase, BA_Basin_Type, demstring)
        'creating weasel source file type is no longer required
        'the identical filled DEM is the source dem to be used in weasel
        'SourceRasterPath = BA_GetPath(AOIFolderBase, "SOURCEDEM")
        'response = BA_SaveRasterDataset(pDEMClip, SourceRasterPath, "grid")
        '___________________________________________________________________________________________________________
        '_____________________________________________________________________________________________________________

        '======================
        'Clip and save ASPECT
        '======================
        pStepProg.Message = "Clipping ASPECT... (step 2 of " & nstep & ")"
        pStepProg.Step()
        System.Windows.Forms.Application.DoEvents()

        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.aspect_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.aspect_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage, False)
        If response <= 0 Then
            MsgBox("Clipping ASPECT failed! Return value = " & response & ".")
        End If

        '======================
        'Clip and save SLOPE
        '======================
        pStepProg.Message = "Clipping SLOPE... (step 3 of " & nstep & ")"
        pStepProg.Step()
        System.Windows.Forms.Application.DoEvents()

        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.slope_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.slope_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage, False)
        If response <= 0 Then
            MsgBox("Clipping SLOPE failed! Return value = " & response & ".")
        End If

        '======================
        'Clip and save FLOW DIRECTION
        '======================
        pStepProg.Message = "Clipping FLOW DIRECTION... (step 4 of " & nstep & ")"
        pStepProg.Step()
        System.Windows.Forms.Application.DoEvents()

        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.flow_direction_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.flow_direction_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage)
        If response <= 0 Then
            MsgBox("Clipping FLOW DIRECTION failed! Return value = " & response & ".")
        End If

        '======================
        'Clip and save FLOW ACCUMULATION
        '======================
        pStepProg.Message = "Clipping FLOW ACCUMULATION... (step 5 of " & nstep & ")"
        pStepProg.Step()
        System.Windows.Forms.Application.DoEvents()

        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.flow_accumulation_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.flow_accumulation_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage, False)
        If response <= 0 Then
            MsgBox("Clipping FLOW ACCUMULATION failed! Return value = " & response & ".")
        End If

        '======================
        'Clip and save Hillshade
        '======================
        pStepProg.Message = "Clipping Hillshade... (step 6 of " & nstep & ")"
        pStepProg.Step()
        System.Windows.Forms.Application.DoEvents()

        inputraster = sourceSurfGDB & "\" & BA_EnumDescription(MapsFileName.hillshade_gdb)
        response = BA_ClipAOIRaster(AOIFolderBase, inputraster, BA_EnumDescription(MapsFileName.hillshade_gdb), destSurfGDB, AOIClipFile.BufferedAOIExtentCoverage, False)
        If response <= 0 Then
            MsgBox("Clipping HILLSHADE failed! Return value = " & response & ".")
        End If

        'update the Z unit metadata of DEM, slope, and PRISM
        Dim inputFolder As String
        Dim inputFile As String
        Dim unitText As String

        'We need to update the elevation units
        inputFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces)
        inputFile = BA_EnumDescription(MapsFileName.filled_dem_gdb)

        If BA_SystemSettings.DEM_ZUnit_IsMeter Then
            unitText = BA_EnumDescription(MeasurementUnit.Meters)
        Else
            unitText = BA_EnumDescription(MeasurementUnit.Feet)
        End If

        Dim sb As StringBuilder = New StringBuilder
        sb.Clear()
        sb.Append(BA_BAGIS_TAG_PREFIX)
        sb.Append(BA_ZUNIT_CATEGORY_TAG & MeasurementUnitType.Elevation.ToString & "; ")
        sb.Append(BA_ZUNIT_VALUE_TAG & unitText & "; ")
        'Record buffer distance
        sb.Append(BA_BUFFER_DISTANCE_TAG + CStr(BA_AOIClipBuffer) + "; ")
        'Record buffer units
        sb.Append(BA_XUNIT_VALUE_TAG + lblBufferUnit.Text + "; \")
        sb.Append(BA_BAGIS_TAG_SUFFIX)
        BA_UpdateMetadata(inputFolder, inputFile, LayerType.Raster, BA_XPATH_TAGS, _
                          sb.ToString, BA_BAGIS_TAG_PREFIX.Length)

        'We need to update the slope units 'always set it to Degree slope
        inputFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces)
        inputFile = BA_GetBareName(BA_EnumDescription(PublicPath.Slope))
        sb.Clear()
        sb.Append(BA_BAGIS_TAG_PREFIX)
        sb.Append(BA_ZUNIT_CATEGORY_TAG & MeasurementUnitType.Slope.ToString & "; ")
        sb.Append(BA_ZUNIT_VALUE_TAG & BA_EnumDescription(SlopeUnit.PctSlope) & ";")
        sb.Append(BA_BAGIS_TAG_SUFFIX)
        BA_UpdateMetadata(inputFolder, inputFile, LayerType.Raster, BA_XPATH_TAGS, _
                          sb.ToString, BA_BAGIS_TAG_PREFIX.Length)

        'get vector clipping mask, raster clipping mask is created earlier, i.e., pWaterRDS
        Dim strInLayerBareName As String
        Dim strInLayerPath As String
        Dim strExtension As String = "please Return"
        Dim strParentName As String = "Please return"

        Dim sbErrorMessage As StringBuilder = New StringBuilder
        If Not BA_SystemSettings.GenerateAOIOnly Then
            'clip snotel layer
            strInLayerPath = BA_SystemSettings.SNOTELLayer
            Dim snoClipLayer As String = BA_EnumDescription(AOIClipFile.BufferedAOIExtentCoverage)  'Use for both Snotel and SC

            pStepProg.Message = "Clipping SNOTEL layer... (step 7 of " & nstep & ")"
            pStepProg.Step()
            System.Windows.Forms.Application.DoEvents()

            'clip snotel vector
            Dim wType As WorkspaceType = BA_GetWorkspaceTypeFromPath(strInLayerPath)
            If wType = WorkspaceType.Raster Then
                strInLayerBareName = BA_GetBareNameAndExtension(strInLayerPath, strParentName, strExtension)
                response = BA_ClipAOISNOTEL(AOIFolderBase, strParentName & "\" & strInLayerBareName, True, snoClipLayer)
            ElseIf wType = WorkspaceType.FeatureServer Then
                response = BA_ClipAOISnoWebServices(AOIFolderBase, strInLayerPath, True, snoClipLayer)
            End If

            'Display error message if appropriate
            If response <> 1 Then
                Select Case response
                    Case -1 '-1: unknown error
                        sbErrorMessage.Append("Error: Unable to clip the SNOTEL layer to the AOI!" + vbCrLf)
                    Case -2 '-2: output exists
                        sbErrorMessage.Append("Error: Output SNOTEL target layer exists in the AOI. Unable to clip data to AOI!" + vbCrLf)
                    Case -3 '-3: missing parameters
                        sbErrorMessage.Append("Error: Missing SNOTEL clipping parameters. Unable to clip data to AOI!" + vbCrLf)
                    Case -4 '-4: no input shapefile
                        sbErrorMessage.Append("Error: Missing the SNOTEL clipping shapefile. Unable to clip data to AOI!" + vbCrLf)
                    Case 0 '0: no intersect between the input and the clip layers
                        sbErrorMessage.Append("Warning: There are no SNOTEL sites within the AOI. The output SNOTEL layer was not created." + vbCrLf)
                End Select
            End If

            'Record buffer units in metadata if snotel layer exists
            inputFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers, True)
            inputFile = BA_GetBareName(BA_EnumDescription(MapsFileName.Snotel))
            If BA_File_Exists(inputFolder + inputFile, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
                sb.Clear()
                sb.Append(BA_BAGIS_TAG_PREFIX)
                sb.Append(BA_BUFFER_DISTANCE_TAG + CStr(BA_AOIClipBuffer) + "; ")
                sb.Append(BA_XUNIT_VALUE_TAG + lblBufferUnit.Text + ";")
                sb.Append(BA_BAGIS_TAG_SUFFIX)
                BA_UpdateMetadata(inputFolder, inputFile, LayerType.Vector, BA_XPATH_TAGS, _
                                  sb.ToString, BA_BAGIS_TAG_PREFIX.Length)
            End If

            'clip snow course layer
            strInLayerPath = BA_SystemSettings.SCourseLayer

            pStepProg.Message = "Clipping Snow Course layer... (step 8 of " & nstep & ")"
            pStepProg.Step()
            System.Windows.Forms.Application.DoEvents()

            'clip snow course vector
            wType = BA_GetWorkspaceTypeFromPath(strInLayerPath)
            If wType = WorkspaceType.Raster Then
                strInLayerBareName = BA_GetBareNameAndExtension(strInLayerPath, strParentName, strExtension)
                response = BA_ClipAOISNOTEL(AOIFolderBase, strParentName & "\" & strInLayerBareName, False, snoClipLayer)
            ElseIf wType = WorkspaceType.FeatureServer Then
                response = BA_ClipAOISnoWebServices(AOIFolderBase, strInLayerPath, False, snoClipLayer)
            End If

            'Display error message if appropriate
            If response <> 1 Then
                Select Case response
                    Case -1 '-1: unknown error
                        sbErrorMessage.Append("Error: Unable to clip the Snow Course layer to the AOI!" + vbCrLf)
                    Case -2 '-2: output exists
                        sbErrorMessage.Append("Error: Output Snow Course target layer exists in the AOI. Unable to clip data to AOI!" + vbCrLf)
                    Case -3 '-3: missing parameters
                        sbErrorMessage.Append("Error: Missing Snow Course clipping parameters. Unable to clip data to AOI!" + vbCrLf)
                    Case -4 '-4: no input shapefile
                        sbErrorMessage.Append("Error: Missing the Snow Course clipping shapefile. Unable to clip data to AOI!" + vbCrLf)
                    Case 0 '0: no intersect between the input and the clip layers
                        sbErrorMessage.Append("Warning: There are no Snow Course sites within the AOI. The output Snow Course layer was not created." + vbCrLf)
                End Select
            End If

            'Record buffer units in metadata if SC layer exists
            inputFile = BA_GetBareName(BA_EnumDescription(MapsFileName.SnowCourse))
            If BA_File_Exists(inputFolder + inputFile, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
                sb.Clear()
                sb.Append(BA_BAGIS_TAG_PREFIX)
                sb.Append(BA_BUFFER_DISTANCE_TAG + CStr(BA_AOIClipBuffer) + "; ")
                sb.Append(BA_XUNIT_VALUE_TAG + lblBufferUnit.Text + ";")
                sb.Append(BA_BAGIS_TAG_SUFFIX)
                BA_UpdateMetadata(inputFolder, inputFile, LayerType.Vector, BA_XPATH_TAGS, _
                                  sb.ToString, BA_BAGIS_TAG_PREFIX.Length)
            End If

            'clip PRISM raster data
            ''''strInLayerPath = frmSettings.txtPRISM.Text
            strInLayerPath = BA_SystemSettings.PRISMFolder
            'remove back slash if exists
            If Microsoft.VisualBasic.Right(strInLayerPath, 1) = "\" Then strInLayerPath = Microsoft.VisualBasic.Left(strInLayerPath, Len(strInLayerPath) - 1)

            'there are 17 prism rasters to be clipped
            BA_SetPRISMFolderNames()
            wType = BA_GetWorkspaceTypeFromPath(strInLayerPath)
            Dim prismServices As System.Array = Nothing
            If wType = WorkspaceType.ImageServer Then
                prismServices = System.Enum.GetValues(GetType(PrismServiceNames))
            End If

            Dim j As Integer
            For j = 0 To 16
                System.Windows.Forms.Application.DoEvents()

                'Local File
                strInLayerBareName = PRISMLayer(j)
                pStepProg.Message = "Clipping PRISM " & strInLayerBareName & " layer... (step " & j + 9 & " of " & nstep & ")"
                pStepProg.Step()

                If wType = WorkspaceType.ImageServer Then
                    Dim clipFilePath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) & BA_EnumDescription(AOIClipFile.PrismClipAOIExtentCoverage)
                    Dim webServiceUrl As String = strInLayerPath & "/" & prismServices(j).ToString & _
                         "/" & BA_Url_ImageServer
                    Dim newFilePath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism, True) & strInLayerBareName
                    response = BA_ClipAOIImageServer(AOIFolderBase, webServiceUrl, newFilePath, AOIClipFile.PrismClipAOIExtentCoverage)
                Else
                    response = BA_ClipAOIRaster(AOIFolderBase, strInLayerPath & "\" & strInLayerBareName & "\grid", strInLayerBareName, destPRISMGDB, AOIClipFile.PrismClipAOIExtentCoverage)
                End If
                If response <= 0 Then
                    sbErrorMessage.Append("Error: PRISM Clipping " & strInLayerPath & " failed! Return value = " & response & "." + vbCrLf)
                End If
            Next

            'update the Z unit metadata of PRISM
            'We need to update the depth units if AOI for Basin Analysis was created
            inputFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism)
            inputFile = AOIPrismFolderNames.annual.ToString

            If rbtnDepthInch.Checked Then
                unitText = BA_EnumDescription(MeasurementUnit.Inches)
            Else
                unitText = BA_EnumDescription(MeasurementUnit.Millimeters)
            End If

            sb.Clear()
            sb.Append(BA_BAGIS_TAG_PREFIX)
            sb.Append(BA_ZUNIT_CATEGORY_TAG & MeasurementUnitType.Depth.ToString & "; ")
            sb.Append(BA_ZUNIT_VALUE_TAG & unitText & "; ")
            'Record buffer distance and units
            sb.Append(BA_BUFFER_DISTANCE_TAG + CStr(BA_PRISMClipBuffer) + "; ")
            sb.Append(BA_XUNIT_VALUE_TAG + lblBufferUnit.Text + ";")
            sb.Append(BA_BAGIS_TAG_SUFFIX)
            BA_UpdateMetadata(inputFolder, inputFile, LayerType.Raster, BA_XPATH_TAGS, _
                              sb.ToString, BA_BAGIS_TAG_PREFIX.Length)
        End If

        'clip other participating layers
        Try
            If ListLayerCount > 0 Then
                For j = 0 To ListLayerCount - 1
                    System.Windows.Forms.Application.DoEvents()
                    strInLayerPath = BA_SystemSettings.OtherLayers(j)
                    strInLayerBareName = BA_GetBareNameAndExtension(strInLayerPath, strParentName, strExtension)

                    pStepProg.Message = "Clipping " & strInLayerBareName & " layer... (step " & j + internalLayerCount + 1 & " of " & nstep & ")"
                    pStepProg.Step()

                    Dim workspaceType As WorkspaceType = BA_GetWorkspaceTypeFromPath(strParentName)
                    If strExtension = "(Shapefile)" Then
                        Dim featureClassExists As Boolean = False
                        If workspaceType = BAGIS_ClassLibrary.WorkspaceType.Raster Then
                            featureClassExists = BA_Shapefile_Exists(strParentName & strInLayerBareName)
                        Else
                            featureClassExists = BA_File_Exists(strParentName & strInLayerBareName, workspaceType, esriDatasetType.esriDTFeatureClass)
                        End If
                        If featureClassExists = True Then
                            'If BA_Shapefile_Exists(strParentName & strInLayerBareName) Then
                            response = BA_ClipAOIVector(AOIFolderBase, strParentName & strInLayerBareName, strInLayerBareName, _
                                                        destLayersGDB, True) 'always use buffered aoi to clip the layers
                        Else
                            sbErrorMessage.Append("Error: " + strInLayerBareName & " does not exist. Clipping failed!" + vbCrLf)
                        End If

                    ElseIf strExtension = "(Raster)" Then
                        'If BA_File_ExistsRaster(strParentName, strInLayerBareName) Then
                        If BA_File_Exists(strParentName & strInLayerBareName, workspaceType, esriDatasetType.esriDTRasterDataset) Then
                            response = BA_ClipAOIRaster(AOIFolderBase, strParentName & strInLayerBareName, strInLayerBareName, destLayersGDB, AOIClipFile.BufferedAOIExtentCoverage)
                            If response <= 0 Then
                                sbErrorMessage.Append("Error: Clipping " & strInLayerBareName & " failed! Return value = " & response & "." + vbCrLf)
                            End If
                        Else
                            sbErrorMessage.Append("Error: " + strInLayerBareName & " does not exist. Clipping failed!" + vbCrLf)
                        End If
                    Else
                        sbErrorMessage.Append("Error: " + strInLayerBareName + " cannot be clipped. " + vbCrLf)
                    End If
                Next
            End If

        Catch ex As Exception
            MsgBox("Create AOI Exception: " & ex.Message, MsgBoxStyle.OkOnly)
        End Try

        pStepProg.Hide()
        progressDialog2.HideDialog()
        pStepProg = Nothing
        progressDialog2 = Nothing

        'copy the basinanalyst.def file to the aoi folder
        If BA_Save_Settings(AOIFolderBase & "\" & BA_Settings_Filename) <> BA_ReturnCode.Success Then
            sbErrorMessage.Append("Error: The definition was not copied to the AOI folder " + vbCrLf)
        End If
        If sbErrorMessage.Length < 1 Then
            MessageBox.Show("AOI for the selected gauge station was created!", "BAGIS", MessageBoxButtons.OK)
        Else
            MessageBox.Show("AOI for the selected gauge station was created with the following warnings: " + vbCrLf + vbCrLf + sbErrorMessage.ToString, _
            "BAGIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        'enable and disable relevant UI buttons
        Dim SelectedAOICombo = AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        ''''BA_SetAOI(ThisDocument.SelectedAOI.Text)
        Dim aoinamestring As String = AOIFolderBase & "\" & SelectedAOICombo.getValue()
        BA_SetAOI(aoinamestring, True)
        Dim createAOIbutton = AddIn.FromID(Of BtnCreateAOI)(My.ThisAddIn.IDs.BtnCreateAOI)
        createAOIbutton.selectedProperty = False
        ClearMap() 'remove the graphics elements from the map

        Me.Close()
    End Sub

    Private Sub ChkSnapPP_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkSnapPP.Click
        lblSnapD.Enabled = ChkSnapPP.Checked
        lblSnapUnit.Enabled = ChkSnapPP.Checked
        txtSnapD.Enabled = ChkSnapPP.Checked
    End Sub

    Private Sub ChkAOIBuffer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkAOIBuffer.Click
        lblBufferD.Enabled = ChkAOIBuffer.Checked
        lblBufferUnit.Enabled = ChkAOIBuffer.Checked
        txtBufferD.Enabled = ChkAOIBuffer.Checked
        lblPrismBufferD.Enabled = ChkAOIBuffer.Checked
        lblPrismBufferUnit.Enabled = ChkAOIBuffer.Checked
        TxtPrismBufferD.Enabled = ChkAOIBuffer.Checked
    End Sub

    Private Sub frmCreateAOI_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ChkSnapPP.Checked = True
        ChkAOIBuffer.Checked = True
        txtSnapD.Text = "15"
        txtBufferD.Text = CStr(BA_AOIClipBuffer)
        TxtPrismBufferD.Text = CStr(BA_PRISMClipBuffer)

        If BA_SystemSettings.DEM_ZUnit_IsMeter Then
            lblDEMUnit.Text = BA_EnumDescription(MeasurementUnit.Meters)
        Else
            lblDEMUnit.Text = BA_EnumDescription(MeasurementUnit.Feet)
        End If

        lblSlopeUnit.Text = BA_EnumDescription(SlopeUnit.PctSlope) 'BAGIS generates Slope in Degree

        grpboxPRISMUnit.Visible = Not BA_SystemSettings.GenerateAOIOnly 'when generate AOI only, no PRISM will be clipped to the AOI

    End Sub

    Private Sub lblWhyBuffer_Click(sender As System.Object, e As System.EventArgs) Handles lblWhyBuffer.Click
         MessageBox.Show(BA_WhyBufferText, "Why Buffer an AOI", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class