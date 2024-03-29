﻿Imports BAGIS_ClassLibrary
Imports System.Windows.Forms
Imports System.Text
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geodatabase

Public Class FrmPsuedoSite

    Private m_analysisFolder As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis)
    Private m_representedArea As String = BA_EnumDescription(MapsFileName.ActualRepresentedArea)
    Private m_precipFolder As String
    Private m_precipFile As String
    Private m_elevLayer As String = "ps_elev"
    Private m_siteFileName As String = "ps_site"
    Private m_proximityLayer As String = "ps_proximity"
    Private m_precipLayer As String = "ps_precip"
    Private m_locationLayer As String = "ps_location"
    Private m_cellStatsLayer As String = "ps_cellStat"
    Private m_demInMeters As Boolean    'Inherited from Site Scenario form; Controls elevation display/calculation
    Private m_usingElevMeters As Boolean    'Inherited from Site Scenario form; Controls elevation display/calculation
    Private m_usingXYUnits As esriUnits  'Inerited from Site Scenario form; Controls proximity display/calculation  
    Private m_aoiBoundary As String = BA_EnumDescription(AOIClipFile.AOIExtentCoverage)
    Private m_lastAnalysis As PseudoSite = Nothing  'The site currently loaded in form
    Private m_formLoaded As Boolean = False
    Private m_cellSize As Double
    Private m_demMax As Double
    Private m_demMin As Double
    Private m_precipMax As Double
    Private m_precipMin As Double
    Private m_idxLayer As Int16 = 0
    Private m_idxValues As Int16 = 1
    Private m_idxBufferDistance As Int16 = 1
    Private m_idxFullPaths As Int16 = 2
    Private m_sep As String = ","
    'These 2 collections hold the values for the location layer(s) in memory; The key is the layer path which should be unique
    Private m_dictLocationAllValues As IDictionary(Of String, IList(Of String))
    Private m_dictLocationIncludeValues As IDictionary(Of String, IList(Of String))
    Private m_displayCombinedLayer As Boolean
    Private m_siteId As Integer
    Private m_reuseConstraintLayersFlag As Boolean = False


    Public Sub New(ByVal demInMeters As Boolean, ByVal useMeters As Boolean, ByVal usingXYUnits As esriUnits,
                   ByVal siteScenarioToolTimeStamp As DateTime, ByVal autoSiteLog As PseudoSite)

        ' This call is required by the designer.
        InitializeComponent()

        ' Slight chance form may be access if AOI not selected because site scenario is a dockable window
        If String.IsNullOrEmpty(AOIFolderBase) Then
            MessageBox.Show("You cannot run the auto-site tool without selecting an AOI first!")
            Me.Close()
        End If

        Dim success As BA_ReturnCode = BA_SetDefaultProjection(My.ArcMap.Application)
        If success <> BA_ReturnCode.Success Then    'unable to set the default projection
            Exit Sub
        End If

        'Populate class-level variables
        m_usingElevMeters = useMeters
        m_demInMeters = demInMeters
        m_usingXYUnits = usingXYUnits

        ' Add any initialization after the InitializeComponent() call.
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

        'read dem min, max everytime the form is activated
        'display dem elevation stats
        Dim inputPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces, True) + BA_EnumDescription(MapsFileName.filled_dem_gdb)
        Dim pRasterStats As IRasterStatistics2 = BA_GetRasterStatsGDB(inputPath, m_cellSize)
        'Determine if Display ZUnit is the same as DEM ZUnit
        'AOI_DEMMin and AOI_DEMMax use internal system unit, i.e., meters
        Dim Conversion_Factor As Double = BA_SetConversionFactor(m_usingElevMeters, m_demInMeters) 'i.e., meters to meters
        'Cheat up so min is never outside of the actual range
        m_demMin = pRasterStats.Minimum * Conversion_Factor - 0.005
        m_demMax = pRasterStats.Maximum * Conversion_Factor + 0.005

        'Populate Boxes
        txtMinElev.Text = Convert.ToString(Math.Ceiling(m_demMin))
        TxtMaxElev.Text = Convert.ToString(Math.Floor(m_demMax))
        TxtRange.Text = Val(TxtMaxElev.Text) - Val(txtMinElev.Text)
        txtLower.Text = txtMinElev.Text
        TxtUpperRange.Text = TxtMaxElev.Text

        'Set DEM label; Default is meters when form loads
        If m_usingElevMeters = False Then
            lblElevation.Text = "DEM Elevation (Feet)"
            LblElevRange.Text = "Desired Range (Feet)"
        End If

        'Set proximity label; Default is meters when form loads
        Select Case m_usingXYUnits
            Case esriUnits.esriFeet
                LblBufferDistance.Text = "Feet"
                LblAddBufferDistance.Text = "Buffer Distance (Feet):"
            Case esriUnits.esriKilometers
                LblBufferDistance.Text = "Kilometers"
                LblAddBufferDistance.Text = "Buffer Distance (Km):"
            Case esriUnits.esriMiles
                LblBufferDistance.Text = "Miles"
                LblAddBufferDistance.Text = "Buffer Distance (Miles):"
        End Select

        'Set label of form
        Me.Text = "Add Pseudo Site: " + BA_GetBareName(AOIFolderBase)

        SuggestSiteName()
        LoadLayers()

        If autoSiteLog IsNot Nothing Then
            LoadAnalysisLog(autoSiteLog)
        End If

        'Locate location panel
        PnlLocation.Left = 6
        PnlLocation.Top = 21
        'Locate proximity panel
        PnlProximity.Left = 6
        PnlProximity.Top = 21

        m_formLoaded = True
    End Sub

    Private Sub BtnFindSite_Click(sender As System.Object, e As System.EventArgs) Handles BtnFindSite.Click
        '1. Check to make sure npactual exists before going further; It's a required layer
        If Not BA_File_Exists(m_analysisFolder + "\" + m_representedArea, WorkspaceType.Geodatabase,
                              ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass) Then

            MessageBox.Show("Unable to locate the Scenario 1 represented area. Calculate Scenario 1 using the Site Scenario tool and try again.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        'Logic for re-using contraint layers
        If BA_Last_PseudoSite IsNot Nothing AndAlso m_lastAnalysis IsNot Nothing Then
            If BA_Last_PseudoSite.ObjectId = m_lastAnalysis.ObjectId Then
                m_reuseConstraintLayersFlag = True
            End If
        End If
        If m_reuseConstraintLayersFlag = False Then
            'Delete any layers from the previous run
            DeletePreviousRun()
        End If

        If String.IsNullOrEmpty(TxtSiteName.Text) Then
            MessageBox.Show("Site name is required to find a site", "BAGIS-V3", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)
            TxtSiteName.Focus()
            Exit Sub
        End If

        'Don't allow the site name to be duplicated
        Dim psuedoList As IList(Of Site) = BA_ReadSiteAttributes(SiteType.Pseudo)
        TxtSiteName.Text = TxtSiteName.Text.Trim()
        For Each pSite As Site In psuedoList
            If pSite.Name.ToUpper.Trim.Equals(TxtSiteName.Text.ToUpper) Then
                MessageBox.Show("Site name " + TxtSiteName.Text + " is already in use. Please supply another name!",
                                "BAGIS V3", MessageBoxButtons.OK)
                TxtSiteName.Focus()
                Exit Sub
            End If
        Next

        If CkElev.Checked Then
            'Validate lower and upper elevations
            Dim sbElev As StringBuilder = New StringBuilder()
            Dim comps As Double
            Dim minElev As Double = CDbl(txtMinElev.Text)
            Dim upperElev As Double = 99999
            If Not String.IsNullOrEmpty(TxtUpperRange.Text) Then
                Double.TryParse(TxtUpperRange.Text, upperElev)
            End If
            'tryparse fails, doesn't get into comps < 0 comparison
            If Double.TryParse(txtLower.Text, comps) Then
                If comps < minElev Then
                    sbElev.Append("Desired range lower: Value greater than minimum elevation is required!" + vbCrLf)
                ElseIf comps > upperElev Then
                    sbElev.Append("Desired range lower: Value less than upper desired range is required!" + vbCrLf)
                End If
            Else
                sbElev.Append("Desired range lower: Numeric value required!" + vbCrLf)
            End If
            Dim maxElev As Double = CDbl(TxtMaxElev.Text)
            Dim lowerRange As Double = 0
            If Not String.IsNullOrEmpty(txtLower.Text) Then
                Double.TryParse(txtLower.Text, lowerRange)
            End If
            'tryparse fails, doesn't get into comps < 0 comparison
            If Double.TryParse(TxtUpperRange.Text, comps) Then
                If comps < lowerRange Then
                    sbElev.Append("Desired range upper: Value greater than the lower desired range is required!" + vbCrLf)
                ElseIf comps > maxElev Then
                    sbElev.Append("Desired range upper: Value less than maximum elevation is required!" + vbCrLf)
                End If
            Else
                sbElev.Append("Desired range upper: Numeric value required!" + vbCrLf)
            End If

            If m_reuseConstraintLayersFlag = True Then
                'Check for existence of elev constraint layer
                If Not BA_File_Exists(m_analysisFolder + "\" + m_elevLayer, WorkspaceType.Geodatabase,
                                      ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                    sbElev.Append("BAGIS needs to re-use constraint layers for the new site but the elevation layer " + m_elevLayer + " cannot be found! " + vbCrLf)
                End If
            End If

            If sbElev.Length > 0 Then
                Dim errMsg As String = "You selected the Elevation option but one or more of the parameters are invalid: " + vbCrLf + vbCrLf +
                    sbElev.ToString + vbCrLf +
                    "Click 'No' to fix the parameters, or 'Yes' to find a site without using the Elevation option"
                Dim res As DialogResult = MessageBox.Show(errMsg, "Invalid elevation values", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)
                If res <> DialogResult.Yes Then
                    Exit Sub
                Else
                    CkElev.Checked = False
                End If
            End If
        End If

        'If user selected proximity layer, did they choose a layer?
        If CkProximity.Checked Then
            If GrdProximity.Rows.Count = 0 Then
                Dim res As DialogResult = MessageBox.Show("You selected the Proximity option but failed to configure any layers. Do you wish to " +
                                                          "find a site without using the Proximity option", "Missing layer", MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question)
                If res <> DialogResult.Yes Then
                    Exit Sub
                Else
                    CkProximity.Checked = False
                End If
            End If
            If m_reuseConstraintLayersFlag = True Then
                'Check for existence of elev constraint layer
                If Not BA_File_Exists(m_analysisFolder + "\" + m_proximityLayer, WorkspaceType.Geodatabase,
                                      ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                    Dim res As DialogResult = MessageBox.Show("BAGIS needs to re-use constraint layers for the new site but the proximity layer " + m_proximityLayer + " cannot be found! Do you wish to " +
                                          "find a site without using the Proximity option", "Missing layer", MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question)
                    If res <> DialogResult.Yes Then
                        Exit Sub
                    Else
                        CkProximity.Checked = False
                    End If
                End If
            End If

        End If

        'If user selected PRISM layer, did they enter a valid range?
        If CkPrecip.Checked Then
            Dim sbPrism As StringBuilder = New StringBuilder()
            Dim comps As Double
            If Not Double.TryParse(txtMinPrecip.Text, comps) Then
                Dim errMsg As String = "You selected the Precipitation option but have not configured a valid range." + vbCrLf + vbCrLf +
                    "Click 'No' to add the desired range, or 'Yes' to find a site without using the Precipitation option."
                Dim res As DialogResult = MessageBox.Show(errMsg, "Invalid precipitation values", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)
                If res <> DialogResult.Yes Then
                    Exit Sub
                Else
                    CkPrecip.Checked = False
                End If
            End If
            'Is Precip option still selected?
            If CkPrecip.Checked Then
                Dim minPrecip As Double = CDbl(txtMinPrecip.Text)
                Dim upperPrecip As Double = 99999
                If Not String.IsNullOrEmpty(TxtPrecipUpper.Text) Then
                    Double.TryParse(TxtPrecipUpper.Text, upperPrecip)
                End If
                'tryparse fails, doesn't get into comps < 0 comparison
                If Double.TryParse(TxtPrecipLower.Text, comps) Then
                    If comps < minPrecip Then
                        sbPrism.Append("Desired range lower: Value greater than minimum precipitation is required!" + vbCrLf)
                    ElseIf comps > upperPrecip Then
                        sbPrism.Append("Desired range lower: Value less than upper desired range is required!" + vbCrLf)
                    End If
                Else
                    sbPrism.Append("Desired range lower: Numeric value required!" + vbCrLf)
                End If
                Dim maxPrecip As Double = CDbl(txtMaxPrecip.Text)
                Dim lowerRange As Double = comps
                'tryparse fails, doesn't get into comps < 0 comparison
                If Double.TryParse(TxtPrecipUpper.Text, comps) Then
                    If comps < lowerRange Then
                        sbPrism.Append("Desired range upper: Value greater than the lower desired range is required!" + vbCrLf)
                    ElseIf comps > maxPrecip Then
                        sbPrism.Append("Desired range upper: Value less than maximum precipitation is required!" + vbCrLf)
                    End If
                Else
                    sbPrism.Append("Desired range upper: Numeric value required!" + vbCrLf)
                End If
                If m_reuseConstraintLayersFlag = True Then
                    'Check for existence of precip constraint layer
                    If Not BA_File_Exists(m_analysisFolder + "\" + m_precipLayer, WorkspaceType.Geodatabase,
                                          ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                        sbPrism.Append("BAGIS needs to re-use constraint layers for the new site but the precipitation layer " + m_precipLayer + " cannot be found! " + vbCrLf)
                    End If
                End If

                If sbPrism.Length > 0 Then
                    Dim errMsg As String = "You selected the Precipitation option but one or more of the parameters are invalid: " + vbCrLf + vbCrLf +
                        sbPrism.ToString + vbCrLf +
                        "Click 'No' to fix the parameters, or 'Yes' to find a site without using the Precipitation option."
                    Dim res As DialogResult = MessageBox.Show(errMsg, "Invalid precipitation values", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)
                    If res <> DialogResult.Yes Then
                        Exit Sub
                    Else
                        CkPrecip.Checked = False
                    End If
                End If
            End If
        End If

        'User selected location layer; Are required fields populated?
        If CkLocation.Checked Then
            Dim sbLoc As StringBuilder = New StringBuilder()
            If GrdLocation.Rows.Count = 0 Then
                sbLoc.Append("No layers have been configured")
            End If
            For Each row As DataGridViewRow In GrdLocation.Rows
                Dim layerName As String = Convert.ToString(row.Cells(m_idxLayer).Value)
                Dim layerLocation As String = Convert.ToString(row.Cells(m_idxFullPaths).Value)
                If row.Cells(m_idxFullPaths).Value Is Nothing Then
                    sbLoc.Append("Missing layer path for layer " + layerName + vbCrLf)
                End If
                If row.Cells(m_idxValues).Value Is Nothing Then
                    sbLoc.Append("Missing selected values for layer " + layerName + vbCrLf)
                Else
                    Dim lstValues As IList(Of String) = m_dictLocationIncludeValues(layerLocation)
                    If lstValues.Count < 1 Then
                        sbLoc.Append("Missing selected values for layer " + layerName + vbCrLf)
                    End If
                End If
            Next
            If m_reuseConstraintLayersFlag = True Then
                'Check for existence of precip constraint layer
                If Not BA_File_Exists(m_analysisFolder + "\" + m_locationLayer, WorkspaceType.Geodatabase,
                                      ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                    sbLoc.Append("BAGIS needs to re-use constraint layers for the new site but the location layer " + m_locationLayer + " cannot be found! " + vbCrLf)
                End If
            End If
            If sbLoc.Length > 0 Then
                Dim errMsg As String = "You selected the Location option but one or more of the parameters are invalid: " + vbCrLf + vbCrLf +
                    sbLoc.ToString + vbCrLf +
                    "Click 'No' to fix the parameters, or 'Yes' to find a site without using the Location option."
                Dim res As DialogResult = MessageBox.Show(errMsg, "Invalid location values", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)
                If res <> DialogResult.Yes Then
                    Exit Sub
                Else
                    CkLocation.Checked = False
                End If
            End If
        End If

        Dim snapRasterPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi) & BA_EnumDescription(PublicPath.AoiGrid)
        Dim maskPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) & m_aoiBoundary

        BtnFindSite.Enabled = False
        BA_Last_PseudoSite = Nothing    'Clear session variable

        'Use this to hold the list of layers that we send to the cell statistics tool
        Dim sb As StringBuilder = New StringBuilder()

        ' Create/configure a step progressor
        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, 15)
        pStepProg.Show()
        ' Create/configure the ProgressDialog. This automatically displays the dialog
        Dim progressDialog2 As IProgressDialog2 = BA_GetProgressDialog(pStepProg, "Locating pseudo-site", "Locating...")
        progressDialog2.ShowDialog()

        Dim success As BA_ReturnCode = BA_ReturnCode.Success

        '2. Identify cells that are furthest from the represented area (Euclidean distance tool)
        Dim tempDistanceFileName As String = "tmpDistance"  'File name before extract to mask; Run initially against buffered AOI
        Dim tempMaskPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) & BA_EnumDescription(AOIClipFile.BufferedAOIExtentCoverage)
        Dim distanceFileName As String = "ps_distance"
        Dim furthestPixelInputFile As String = distanceFileName
        If success = BA_ReturnCode.Success Then
            pStepProg.Message = "Executing Euclidean distance tool"
            pStepProg.Step()
            success = BA_EuclideanDistance(m_analysisFolder + "\" + m_representedArea, m_analysisFolder + "\" + tempDistanceFileName,
                                           CStr(m_cellSize), tempMaskPath, snapRasterPath, tempMaskPath)
        End If
        If success = BA_ReturnCode.Success Then
            'Extract to Mask to clip off area outside AOI
            success = BA_ExtractByMask(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) & m_aoiBoundary,
                                       m_analysisFolder + "\" + tempDistanceFileName, snapRasterPath, m_analysisFolder + "\" + distanceFileName)
            If success = BA_ReturnCode.Success Then
                'Delete temp distance file
                BA_RemoveRasterFromGDB(m_analysisFolder, tempDistanceFileName)
            End If
        End If
        If CkElev.Checked = True Then
            success = GenerateElevationLayer(pStepProg, snapRasterPath)
            If success = BA_ReturnCode.Success Then
                sb.Append(m_analysisFolder + "\" + m_elevLayer + "; ")
            Else
                MessageBox.Show("An error occurred while generating the Elevation layer. Analysis stopped.")
                CleanUpAfterAnalysis(pStepProg, progressDialog2)
                Exit Sub
            End If
        End If

        If CkPrecip.Checked = True Then
            success = GeneratePrecipitationLayer(pStepProg, snapRasterPath)
            If success = BA_ReturnCode.Success Then
                sb.Append(m_analysisFolder + "\" + m_precipLayer + "; ")
            Else
                MessageBox.Show("An error occurred while generating the Precipitation layer. Analysis stopped.")
                CleanUpAfterAnalysis(pStepProg, progressDialog2)
                Exit Sub
            End If
        End If

        If CkProximity.Checked = True Then
            success = GenerateProximityLayer(pStepProg, snapRasterPath)
            If success = BA_ReturnCode.Success Then
                sb.Append(m_analysisFolder + "\" + m_proximityLayer + "; ")
            Else
                MessageBox.Show("An error occurred while generating the Proximity layer. Analysis stopped.")
                CleanUpAfterAnalysis(pStepProg, progressDialog2)
                Exit Sub
            End If
        End If

        If CkLocation.Checked = True Then
            success = GenerateLocationLayer(pStepProg, snapRasterPath)
            If success = BA_ReturnCode.Success Then
                sb.Append(m_analysisFolder + "\" + m_locationLayer + "; ")
            Else
                MessageBox.Show("An error occurred while generating the Location layer. Analysis stopped.")
                CleanUpAfterAnalysis(pStepProg, progressDialog2)
                Exit Sub
            End If
        End If

        Dim timesFileName As String = "ps_times"
        If sb.Length > 0 Then
            If success = BA_ReturnCode.Success Then
                '6. Get minimum for all of the constraint layers
                pStepProg.Message = "Calculating cell statistics for all constraint layers"
                pStepProg.Step()
                sb.Remove(sb.ToString().LastIndexOf("; "), "; ".Length)
                success = BA_GetCellStatistics(sb.ToString, snapRasterPath, "MINIMUM",
                                               m_analysisFolder + "\" + m_cellStatsLayer, "false")
            End If

            If BA_IsRasterEmpty(m_analysisFolder, m_cellStatsLayer) Then
                Dim errMsg As String = "The entire area of the AOI was excluded using the constraints you selected. " +
                    "No suitable site location could be found. "
                MessageBox.Show(errMsg, "No site location found", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                If progressDialog2 IsNot Nothing Then
                    progressDialog2.HideDialog()
                End If
                BtnFindSite.Enabled = True
                Exit Sub
            End If

            'Add 'NAME' field cell stats (all constraints) layer to be used as label for map
            If success = BA_ReturnCode.Success Then
                success = BA_AddUserFieldToRaster(m_analysisFolder, m_cellStatsLayer, BA_FIELD_NAME, esriFieldType.esriFieldTypeString,
                                              100, BA_MAPS_PS_ALL_CONSTRAINTS)
            End If

            furthestPixelInputFile = timesFileName
            If success = BA_ReturnCode.Success Then
                pStepProg.Message = "Executing Times tool with distance and cell statistics layers"
                pStepProg.Step()
                success = BA_Times(m_analysisFolder + "\" + distanceFileName, m_analysisFolder + "\" + m_cellStatsLayer,
                    m_analysisFolder + "\" + timesFileName)
            End If
        End If

        Dim furthestPixelFileName As String = "ps_furthest"
        If success = BA_ReturnCode.Success Then
            pStepProg.Message = "Finding furthest pixel"
            pStepProg.Step()
            'Get the maximum pixel value
            'Set everything to null that is smaller than that; should leave one pixel
            'Expression can be more precise; Rounding down works for now
            Dim cellSize As Double = -1
            Dim pRasterStats As IRasterStatistics = BA_GetRasterStatsGDB(m_analysisFolder + "\" + furthestPixelInputFile, cellSize)
            Dim pExpression As String = Nothing
            If pRasterStats IsNot Nothing Then
                'sample expression: SetNull('C:\Docs\Lesley\animas_AOI_prms_3\analysis.gdb\ps_distance' < 6259,'C:\Docs\Lesley\animas_AOI_prms_3\analysis.gdb\ps_distance')
                Dim targetPath As String = m_analysisFolder + "\" + furthestPixelInputFile
                pExpression = "SetNull('" + targetPath + "' < " +
                    CStr(Math.Floor(pRasterStats.Maximum)) +
                    ",'" + targetPath + "')"
                success = BA_RasterCalculator(m_analysisFolder + "\" + furthestPixelFileName, pExpression,
                                              snapRasterPath, maskPath)
            End If
        End If

        If success = BA_ReturnCode.Success Then
            success = BA_RasterToPoint(m_analysisFolder + "\" + furthestPixelFileName,
                                       m_analysisFolder + "\" + m_siteFileName, BA_FIELD_VALUE)
        End If

        m_siteId = -1
        If success = BA_ReturnCode.Success Then
            Dim numSites As Int16 = BA_CountPolygons(m_analysisFolder, m_siteFileName, BA_FIELD_GRIDCODE_GDB)
            Dim intOid As Integer = 1
            If numSites < 1 Then
                MessageBox.Show("No psuedo-sites were found. Please double-check your selection criteria")
            ElseIf numSites > 1 Then
                MessageBox.Show(numSites & " pseudo-sites were found. Currently BAGIS only knows how to deal with one, so it will randomly pick one.")
                Dim r As New Random()
                intOid = r.Next(1, numSites + 1)
                'Delete all sites except the first one
                Dim strSelect As String = " " & BA_FIELD_OBJECT_ID & " <> " & intOid
                success = BA_DeleteFeatures(m_analysisFolder, m_siteFileName, strSelect)
            End If

            'Create new psuedo_sites file or append auto-site to existing
            pStepProg.Message = "Integrating new pseudo-site into site selection layers"
            pStepProg.Step()
            Dim newSite As Site = PreparePointFileToAppend(snapRasterPath, intOid)
            If newSite IsNot Nothing Then
                Dim pseudoPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers, True) + BA_EnumDescription(MapsFileName.Pseudo)
                If BA_File_Exists(pseudoPath, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
                    'Check to make sure existing feature class has the site type field
                    success = CheckForSiteType()
                    If success <> BA_ReturnCode.Success Then
                        MessageBox.Show("An error occurred while trying to process the new pseudo-site layer!")
                        Exit Sub
                    End If
                    success = BA_AppendFeatures(m_analysisFolder + "\" + m_siteFileName, pseudoPath)
                Else
                    success = BA_CopyFeatures(m_analysisFolder + "\" + m_siteFileName, pseudoPath)
                End If

                If success = BA_ReturnCode.Success Then
                    'Query the OID of the new site
                    m_siteId = GetNewSiteObjectId(newSite.Elevation)
                    If m_siteId > 0 Then
                        newSite.ObjectId = m_siteId
                        'Adds the sites to 'existing sites' on the form
                        Dim dockWindowAddIn = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of frmSiteScenario.AddinImpl)(My.ThisAddIn.IDs.frmSiteScenario)
                        Dim siteScenarioForm As frmSiteScenario = dockWindowAddIn.UI
                        siteScenarioForm.AddNewPseudoSite(newSite)

                        'Set the global variable for pseudo-sites to true
                        AOI_HasPseudoSite = True
                    Else
                        MessageBox.Show("Unable to add psuedo-site to Site Scenario Tool. Reload Site Scenario Tool")
                    End If
                End If
            Else
                MessageBox.Show("An error occurred while trying to process the new pseudo-site layer!")
            End If
        End If

        If success = BA_ReturnCode.Success Then
            pStepProg.Message = "Saving pseudo-site log"
            pStepProg.Step()
            SavePseudoSiteLog()
            BtnRecalculate.Enabled = True
            BtnMap.Enabled = True
            MessageBox.Show("The new pseudo-site has been added to Scenario 1 in the Site Scenario Tool")
        End If
        CleanUpAfterAnalysis(pStepProg, progressDialog2)
    End Sub

    Private Sub CleanUpAfterAnalysis(ByVal pStepProg As IStepProgressor, ByVal progressDialog2 As IProgressDialog2)
        If progressDialog2 IsNot Nothing Then
            progressDialog2.HideDialog()
        End If
        progressDialog2 = Nothing
        pStepProg = Nothing
    End Sub

    Private Sub CkElev_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CkElev.CheckedChanged
        GrpElevation.Enabled = CkElev.Checked
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub CkPrecip_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CkPrecip.CheckedChanged
        GrpPrecipitation.Enabled = CkPrecip.Checked
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub CkProximity_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CkProximity.CheckedChanged
        GrpProximity.Enabled = CkProximity.Checked
        If Not CkProximity.Checked Then _
            GrdProximity.Rows.Clear()
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Function GenerateElevationLayer(ByVal pStepProg As IStepProgressor, ByVal snapRasterPath As String) As BA_ReturnCode
        '1. Reclass elevation raster according to upper and lower ranges
        If m_reuseConstraintLayersFlag = False Then
            pStepProg.Message = "Reclass DEM for elevation layer"
            pStepProg.Step()
            Dim sb As StringBuilder = New StringBuilder()
            'Set min/max of reclass to actual dem values
            Dim strMinElev As String = Convert.ToString(m_demMin)
            Dim strLower As String = txtLower.Text
            Dim strUpperRange As String = TxtUpperRange.Text
            Dim strMaxElev As String = Convert.ToString(m_demMax)
            'Convert the values to the DEM value, before composing the reclass string, if we need to
            If m_demInMeters <> m_usingElevMeters Then
                Dim converter As IUnitConverter = New UnitConverter
                Dim toElevUnits As esriUnits = esriUnits.esriMeters
                If Not m_demInMeters Then _
                    toElevUnits = esriUnits.esriFeet
                Dim fromElevUnits As esriUnits = esriUnits.esriFeet
                If m_usingElevMeters Then _
                    fromElevUnits = esriUnits.esriMeters
                strMinElev = Convert.ToString(Math.Round(converter.ConvertUnits(m_demMin, fromElevUnits, toElevUnits), 3) - 0.005)
                strLower = Convert.ToString(Math.Round(converter.ConvertUnits(Convert.ToDouble(txtLower.Text), fromElevUnits, toElevUnits), 3))
                strUpperRange = Convert.ToString(Math.Round(converter.ConvertUnits(Convert.ToDouble(TxtUpperRange.Text), fromElevUnits, toElevUnits), 3))
                strMaxElev = Convert.ToString(Math.Round(converter.ConvertUnits(m_demMax, fromElevUnits, toElevUnits), 3) + 0.005)
            End If
            sb.Append(strMinElev + " " + strLower + " NoData;")
            sb.Append(strLower + " " + strUpperRange + " 1;")
            sb.Append(strUpperRange + " " + strMaxElev + " NoData")
            Dim inputPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces, True) + BA_EnumDescription(MapsFileName.filled_dem_gdb)
            Dim reclassElevPath As String = m_analysisFolder & "\" & m_elevLayer
            Dim success As BA_ReturnCode = BA_ReclassifyRasterFromString(inputPath, BA_FIELD_VALUE, sb.ToString,
                                                                         reclassElevPath, snapRasterPath)
            'Add 'NAME' field to be used as label for map
            If success = BA_ReturnCode.Success Then
                success = BA_AddUserFieldToRaster(m_analysisFolder, m_elevLayer, BA_FIELD_NAME, esriFieldType.esriFieldTypeString,
                                              100, BA_MAPS_PS_ELEVATION)
            End If
            Return success
        Else
            If BA_File_Exists(m_analysisFolder & "\" & m_elevLayer, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                Return BA_ReturnCode.Success
            Else
                Return BA_ReturnCode.UnknownError
            End If
        End If
    End Function

    Private Function GeneratePrecipitationLayer(ByVal pStepProg As IStepProgressor, ByVal snapRasterPath As String) As BA_ReturnCode
        If m_reuseConstraintLayersFlag = False Then
            '1. Reclass precip raster according to upper and lower ranges
            pStepProg.Message = "Reclass precipitation layer"
            pStepProg.Step()
            Dim sb As StringBuilder = New StringBuilder()
            Dim strMinPrecip As String = Convert.ToString(m_precipMin)
            Dim strLowerRange As String = TxtPrecipLower.Text
            Dim strUpperRange As String = TxtPrecipUpper.Text
            Dim strMaxPrecip As String = Convert.ToString(m_precipMax)
            sb.Append(strMinPrecip + " " + strLowerRange + " NoData;")
            sb.Append(strLowerRange + " " + strUpperRange + " 1;")
            sb.Append(strUpperRange + " " + strMaxPrecip + " NoData")

            Dim inputFolder As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism, True)
            Dim prismRasterName As String
            If CmboxPrecipType.SelectedIndex = 0 Then
                prismRasterName = AOIPrismFolderNames.annual.ToString    'read direct Annual PRISM raster
            ElseIf CmboxPrecipType.SelectedIndex > 0 And CmboxPrecipType.SelectedIndex < 5 Then 'read directly Quarterly PRISM raster
                prismRasterName = BA_GetPrismFolderName(CmboxPrecipType.SelectedIndex + 12)
            Else 'sum individual monthly PRISM rasters
                Dim response As Integer = BA_PRISMCustom(My.Document, AOIFolderBase, Val(CmboxBegin.SelectedItem), Val(CmboxEnd.SelectedItem))
                If response = 0 Then
                    MsgBox("Unable to generate custom PRISM layer! Program stopped.")
                    Return BA_ReturnCode.UnknownError
                End If
                inputFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True)
                prismRasterName = BA_TEMP_PRISM
            End If
            Dim inputPath As String = inputFolder + prismRasterName
            Dim reclassPrismPath As String = m_analysisFolder & "\" & m_precipLayer
            Dim success As BA_ReturnCode = BA_ReclassifyRasterFromString(inputPath, BA_FIELD_VALUE, sb.ToString,
                                                                                 reclassPrismPath, snapRasterPath)
            'Add 'NAME' field to be used as label for map
            If success = BA_ReturnCode.Success Then
                success = BA_AddUserFieldToRaster(m_analysisFolder, m_precipLayer, BA_FIELD_NAME, esriFieldType.esriFieldTypeString,
                                              100, BA_MAPS_PS_PRECIPITATION)
            End If
            Return success
        Else
            If BA_File_Exists(m_analysisFolder & "\" & m_precipLayer, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                Return BA_ReturnCode.Success
            Else
                Return BA_ReturnCode.UnknownError
            End If
        End If
    End Function

    Private Function GenerateLocationLayer(ByVal pStepProg As IStepProgressor, ByVal snapRasterPath As String) As BA_ReturnCode
        If m_reuseConstraintLayersFlag = False Then
            Dim layerCount As Int16 = GrdLocation.Rows.Count
            Dim success As BA_ReturnCode
            Dim outputFolderPath As String = m_analysisFolder + "\" + m_locationLayer
            Dim inRasterPath2 As String = Nothing
            Dim timesOutputFolderPath As String = Nothing
            Dim lstRastersToDelete As IList(Of String) = New List(Of String)
            For i As Int16 = 0 To layerCount - 1
                pStepProg.Message = "Processing location layer " + Convert.ToString(GrdLocation.Rows(i).Cells(m_idxLayer).Value)
                pStepProg.Step()
                'Build reclassItem array
                Dim layerLocation As String = Convert.ToString(GrdLocation.Rows(i).Cells(m_idxFullPaths).Value)
                Dim lstAllValues As IList(Of String) = m_dictLocationAllValues(layerLocation)
                Dim lstIncludeValues As IList(Of String) = m_dictLocationIncludeValues(layerLocation)
                Dim reclassItems(lstAllValues.Count - 1) As ReclassItem
                For j As Integer = 0 To lstAllValues.Count - 1
                    Dim nextItem As ReclassItem = New ReclassItem()
                    Dim pValue As String = lstAllValues(j)
                    nextItem.FromValue = pValue
                    nextItem.ToValue = pValue
                    If lstIncludeValues.Contains(pValue) Then
                        nextItem.OutputValue = 1
                    Else
                        nextItem.OutputValue = -9999
                    End If
                    reclassItems(j) = nextItem
                Next
                If layerCount > 1 Then
                    outputFolderPath = m_analysisFolder + "\tempLocation" + CStr(i)
                    lstRastersToDelete.Add(outputFolderPath)
                    timesOutputFolderPath = m_analysisFolder + "\timesLocation" + CStr(i)
                End If
                success = BA_ReclassifyRasterFromTableWithNoData(layerLocation, BA_FIELD_VALUE, reclassItems,
                                                                 outputFolderPath, snapRasterPath)
                If success = BA_ReturnCode.Success AndAlso i > 0 Then
                    'inRasterPath1 always outputFolderPath
                    'inRasterPath2 see case statement below
                    'outRasterPath always timesOutputFolderPath
                    Select Case i
                        Case 1
                            'Multiplying first 2 reclass layers
                            inRasterPath2 = m_analysisFolder + "\tempLocation" + CStr(i - 1)
                        Case Is > 1
                            'Multiplying by previous times output layer
                            inRasterPath2 = m_analysisFolder + "\timesLocation" + CStr(i - 1)
                    End Select
                    success = BA_Times(outputFolderPath, inRasterPath2, timesOutputFolderPath)
                    lstRastersToDelete.Add(timesOutputFolderPath)
                End If
                ' Stop processing if there is an error
                If success <> BA_ReturnCode.Success Then
                    Return success
                End If
            Next
            ' Need to rename the final layer to the location output layer
            If layerCount > 1 Then
                success = BA_Copy(timesOutputFolderPath, m_analysisFolder + "\" + m_locationLayer)
            End If
            If success = BA_ReturnCode.Success Then
                For Each layerPath As String In lstRastersToDelete
                    Dim layerFolder As String = "PleaseReturn"
                    Dim layerName As String = BA_GetBareName(layerPath, layerFolder)
                    Dim retVal As Short = BA_RemoveRasterFromGDB(layerFolder, layerName)
                Next
                success = BA_AddUserFieldToRaster(m_analysisFolder, m_locationLayer, BA_FIELD_NAME, esriFieldType.esriFieldTypeString,
                                  100, BA_MAPS_PS_LOCATION)
            End If
            Return success
        Else
            If BA_File_Exists(m_analysisFolder & "\" & m_locationLayer, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                Return BA_ReturnCode.Success
            Else
                Return BA_ReturnCode.UnknownError
            End If
        End If
    End Function

    Private Sub txtLower_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtLower.Validating
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub TxtUpperRange_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TxtUpperRange.Validating
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub CmdPrism_Click(sender As System.Object, e As System.EventArgs) Handles CmdPrism.Click
        ' Create/configure a step progressor
        Dim pStepProg As IStepProgressor = BA_GetStepProgressor(My.ArcMap.Application.hWnd, 15)
        pStepProg.Show()
        ' Create/configure the ProgressDialog. This automatically displays the dialog
        Dim progressDialog2 As IProgressDialog2 = BA_GetProgressDialog(pStepProg, "Calculating PRISM precipitation values", "Calculating...")
        progressDialog2.ShowDialog()

        m_precipFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Prism, True)
        If CmboxPrecipType.SelectedIndex = 0 Then  'read direct Annual PRISM raster
            m_precipFile = AOIPrismFolderNames.annual.ToString
        ElseIf CmboxPrecipType.SelectedIndex > 0 And CmboxPrecipType.SelectedIndex < 5 Then 'read directly Quarterly PRISM raster
            m_precipFile = BA_GetPrismFolderName(CmboxPrecipType.SelectedIndex + 12)
        Else 'sum individual monthly PRISM rasters
            Dim response As Integer = BA_PRISMCustom(My.Document, AOIFolderBase, Val(CmboxBegin.SelectedItem), Val(CmboxEnd.SelectedItem))
            If response = 0 Then
                MessageBox.Show("Unable to generate custom PRISM layer! Program stopped.")
                Exit Sub
            End If
            m_precipFolder = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True)
            m_precipFile = BA_TEMP_PRISM
        End If

        Dim raster_res As Double
        Dim pRasterStats As IRasterStatistics = BA_GetRasterStatsGDB(m_precipFolder & m_precipFile, raster_res)

        'Populate Boxes
        m_precipMin = pRasterStats.Minimum - 0.005
        m_precipMax = pRasterStats.Maximum + 0.005
        txtMinPrecip.Text = Math.Ceiling((pRasterStats.Minimum - 0.005) * 100) / 100
        txtMaxPrecip.Text = Math.Floor((pRasterStats.Maximum - 0.005) * 100) / 100
        txtRangePrecip.Text = Val(txtMaxPrecip.Text) - Val(txtMinPrecip.Text)
        TxtPrecipLower.Text = txtMinPrecip.Text
        TxtPrecipUpper.Text = txtMaxPrecip.Text

        If progressDialog2 IsNot Nothing Then
            progressDialog2.HideDialog()
        End If
        progressDialog2 = Nothing
        pStepProg = Nothing

    End Sub

    Private Sub CmboxPrecipType_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxPrecipType.SelectedIndexChanged
        RaiseEvent FormInputChanged()
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
        txtMinPrecip.Text = "-"
        txtMaxPrecip.Text = "-"
        txtRangePrecip.Text = "-"
    End Sub

    Private Sub BtnMap_Click(sender As System.Object, e As System.EventArgs) Handles BtnMap.Click
        'Ensure default map frame name is set before trying to build map
        Dim response As Integer = BA_SetDefaultMapFrameName(BA_MAPS_DEFAULT_MAP_NAME, My.Document)
        response = BA_SetMapFrameDimension(BA_MAPS_DEFAULT_MAP_NAME, 1, 2, 7.5, 9, True)
        AddLayersToMapFrame(My.ThisApplication, My.Document)
        Dim aoiName As String = BA_GetBareName(AOIFolderBase)
        BA_AddMapElements(My.Document, aoiName, "Subtitle BAGIS")

        'Toggle the layers we want to see
        Dim LayerNames(11) As String
        LayerNames(1) = BA_MAPS_SCENARIO1_REPRESENTATION
        LayerNames(2) = BA_EnumDescription(MapsLayerName.NewPseudoSite)
        LayerNames(3) = BA_MAPS_PS_INDICATOR
        LayerNames(4) = BA_MAPS_AOI_BASEMAP

        If m_displayCombinedLayer = False Then
            LayerNames(5) = BA_MAPS_PS_PROXIMITY
            LayerNames(6) = BA_MAPS_PS_ELEVATION
            LayerNames(7) = BA_MAPS_PS_PRECIPITATION
            LayerNames(8) = BA_MAPS_PS_LOCATION
            LayerNames(9) = BA_MAPS_HILLSHADE
            LayerNames(10) = BA_MAPS_AOI_BOUNDARY
        Else
            ReDim Preserve LayerNames(8)
            LayerNames(5) = BA_MAPS_PS_ALL_CONSTRAINTS
            LayerNames(6) = BA_MAPS_HILLSHADE
            LayerNames(7) = BA_MAPS_AOI_BOUNDARY
        End If
        response = BA_ToggleLayersinMapFrame(My.Document, LayerNames)

        BA_RemoveLayersfromLegend(My.Document)
        'Note: these functions are called in BA_DisplayMap if we end up adding buttons
        Dim UnitText As String = Nothing    'Textbox above scale bar
        Dim subtitle As String = "PROPOSED PSEUDO SITE LOCATION"
        BA_MapUpdateSubTitle(My.Document, aoiName, subtitle, UnitText)
        Dim keyLayerName As String = Nothing
        BA_SetLegendFormat(My.Document, keyLayerName)

        'Clip data frame to aoi border
        'ClipDataFrameToAoiBorder()     //Disable this so we can show areas outside the AOI

        MessageBox.Show("Use ArcMap Table of Contents to view map.", "Map", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub AddLayersToMapFrame(ByVal pApplication As ESRI.ArcGIS.Framework.IApplication,
                                    ByVal pMxDoc As ESRI.ArcGIS.ArcMapUI.IMxDocument)
        Dim pColor As IColor = New RgbColor
        Dim success As BA_ReturnCode = BA_ReturnCode.UnknownError
        Dim retVal As Integer = -1

        Try
            'Scenario 1 Represented area
            Dim filepathname As String = m_analysisFolder & "\" & m_representedArea
            If Not BA_File_Exists(filepathname, WorkspaceType.Geodatabase, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass) Then
                MessageBox.Show("Unable to locate the represented area from the site scenario tool. Cannot load map.", "Error",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            'Re-adding the map so it is in the right position and transparent
            pColor.RGB = RGB(255, 0, 0) 'red
            success = BA_MapDisplayPolygon(pMxDoc, filepathname, BA_MAPS_SCENARIO1_REPRESENTATION, pColor, 30)

            'add pseudo site
            If Not LayerIsOnMap(MapsLayerName.NewPseudoSite) Then
                filepathname = m_analysisFolder & "\" & m_siteFileName
                If BA_File_Exists(filepathname, WorkspaceType.Geodatabase, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass) Then
                    pColor.RGB = RGB(169, 0, 230)    'Purple
                    success = BA_MapDisplayPointMarkers(pApplication, filepathname, MapsLayerName.NewPseudoSite, pColor, MapsMarkerType.PseudoSite)
                End If
            End If

            'draw circle around pseudo site
            If Not LayerIsOnMap(BA_MAPS_PS_INDICATOR) Then
                Dim siteLayerName As String = BA_EnumDescription(MapsLayerName.NewPseudoSite)
                Dim tempLayer As ILayer
                Dim pseudoSrc As IFeatureLayer = Nothing
                'Reset layer count in case layers were removed
                Dim nlayers As Int16 = pMxDoc.FocusMap.LayerCount

                For i = nlayers To 1 Step -1
                    tempLayer = CType(pMxDoc.FocusMap.Layer(i - 1), ILayer)   'Explicit cast
                    If TypeOf tempLayer Is FeatureLayer AndAlso tempLayer.Name = siteLayerName Then
                        pseudoSrc = CType(tempLayer, IFeatureLayer)
                        Exit For
                    End If
                Next

                Dim pActualColor As IColor = New RgbColor
                pActualColor.RGB = RGB(169, 0, 230)    'Purple
                Dim actualRenderer As ISimpleRenderer = BA_BuildRendererForPoints(pActualColor, 25)

                If pseudoSrc IsNot Nothing Then
                    Dim pFSele As IFeatureSelection = TryCast(pseudoSrc, IFeatureSelection)
                    Dim pQFilter As IQueryFilter = New QueryFilter
                    pFSele.SelectFeatures(pQFilter, esriSelectionResultEnum.esriSelectionResultNew, False)
                    Dim fLayerDef As IFeatureLayerDefinition = CType(pseudoSrc, IFeatureLayerDefinition)
                    Dim pseudoCopy As IFeatureLayer = fLayerDef.CreateSelectionLayer(BA_MAPS_PS_INDICATOR, True, Nothing, Nothing)
                    Dim pGFLayer As IGeoFeatureLayer = CType(pseudoCopy, IGeoFeatureLayer)
                    pGFLayer.Renderer = actualRenderer
                    My.Document.FocusMap.AddLayer(pGFLayer)
                    pFSele.Clear()
                End If
            End If

            'add aoi boundary and zoom to AOI
            If Not LayerIsOnMap(BA_MAPS_AOI_BOUNDARY) Then
                filepathname = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) & m_aoiBoundary
                retVal = BA_AddExtentLayer(pMxDoc, filepathname, Nothing, False, BA_MAPS_AOI_BOUNDARY, 1, 1.2, 2.0)
            End If

            'add aoib as base layer for difference of representation maps
            If Not LayerIsOnMap(BA_MAPS_AOI_BASEMAP) Then
                filepathname = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Aoi, True) & BA_BufferedAOIExtentRaster
                retVal = BA_DisplayRasterWithSymbol(pMxDoc, filepathname, BA_MAPS_AOI_BASEMAP,
                                                    MapsDisplayStyle.Cyan_Light_to_Blue_Dark, 30, WorkspaceType.Geodatabase)
            End If

            'Check to see if should display the combined map layer
            Dim constraintCount As Short = 0
            If m_lastAnalysis.UseProximity Then _
                constraintCount += 1
            If m_lastAnalysis.UseElevation Then _
                constraintCount += 1
            If m_lastAnalysis.UseLocation Then _
                constraintCount += 1
            If m_lastAnalysis.UsePrism Then _
                constraintCount += 1
            If constraintCount > 1 Then
                m_displayCombinedLayer = True
                filepathname = m_analysisFolder & "\" & m_cellStatsLayer
                If BA_File_Exists(filepathname, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    If Not LayerIsOnMap(BA_MAPS_PS_ALL_CONSTRAINTS) Then
                        retVal = BA_DisplayRasterWithSymbol(pMxDoc, filepathname, BA_MAPS_PS_ALL_CONSTRAINTS,
                                                            MapsDisplayStyle.Pink_to_Yellow_Green, 30, WorkspaceType.Geodatabase)
                    End If
                End If
            End If

            'Proximity if it exists
            If m_lastAnalysis.UseProximity Then
                filepathname = m_analysisFolder & "\" & m_proximityLayer
                If BA_File_Exists(filepathname, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    If Not LayerIsOnMap(BA_MAPS_PS_PROXIMITY) Then
                        retVal = BA_DisplayRasterWithSymbol(pMxDoc, filepathname, BA_MAPS_PS_PROXIMITY,
                                                            MapsDisplayStyle.Yellows, 30, WorkspaceType.Geodatabase)
                    End If
                End If
            End If

            'Elevation if it exists
            filepathname = m_analysisFolder & "\" & m_elevLayer
            If m_lastAnalysis.UseElevation Then
                If BA_File_Exists(filepathname, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    If Not LayerIsOnMap(BA_MAPS_PS_ELEVATION) Then
                        retVal = BA_DisplayRasterWithSymbol(pMxDoc, filepathname, BA_MAPS_PS_ELEVATION,
                                       MapsDisplayStyle.Slope, 30, WorkspaceType.Geodatabase)
                    End If
                End If
            End If

            'Precipitation if used
            filepathname = m_analysisFolder & "\" & m_precipLayer
            If m_lastAnalysis.UsePrism Then
                If BA_File_Exists(filepathname, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    If Not LayerIsOnMap(BA_MAPS_PS_PRECIPITATION) Then
                        retVal = BA_DisplayRasterWithSymbol(pMxDoc, filepathname, BA_MAPS_PS_PRECIPITATION,
                                    MapsDisplayStyle.Purple_Blues, 30, WorkspaceType.Geodatabase)
                    End If
                End If
            End If

            'Location if used
            filepathname = m_analysisFolder & "\" & m_locationLayer
            If m_lastAnalysis.UseLocation Then
                If BA_File_Exists(filepathname, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                    If Not LayerIsOnMap(BA_MAPS_PS_LOCATION) Then
                        retVal = BA_DisplayRasterWithSymbol(pMxDoc, filepathname, BA_MAPS_PS_LOCATION,
                                    MapsDisplayStyle.Brown_to_Blue_Green, 30, WorkspaceType.Geodatabase)
                    End If
                End If
            End If

            'add hillshade
            If Not LayerIsOnMap(BA_MAPS_HILLSHADE) Then
                filepathname = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces, True) &
                    BA_GetBareName(BA_EnumDescription(PublicPath.Hillshade))
                retVal = BA_MapDisplayRaster(pMxDoc, filepathname, BA_MAPS_HILLSHADE, 0)
            End If
            'Move hillshade to bottom
            Dim layerCount As Int16 = My.Document.FocusMap.LayerCount
            Dim idxHillshade As Integer = BA_GetLayerIndexByName(My.Document, BA_MAPS_HILLSHADE)
            Dim hLayer As ILayer = My.Document.FocusMap.Layer(idxHillshade)
            My.Document.FocusMap.MoveLayer(hLayer, layerCount)

            'zoom to the aoi boundary layer
            BA_ZoomToAOI(pMxDoc, AOIFolderBase)

        Catch ex As Exception
            Debug.Print("AddLayersToMapFrame Exception: " & ex.Message)
            MessageBox.Show("An error occurred while trying to load the map!")
        End Try

    End Sub

    Private Sub LoadLayers()
        Dim AOIVectorList() As String = Nothing
        Dim AOIRasterList() As String = Nothing
        Dim layerPath As String = AOIFolderBase & "\" & BA_EnumDescription(GeodatabaseNames.Layers)
        BA_ListLayersinGDB(layerPath, AOIRasterList, AOIVectorList)

        'display feature layers
        Dim FeatureClassCount As Integer = UBound(AOIVectorList)
        If FeatureClassCount > 0 Then
            For i = 1 To FeatureClassCount
                Dim fullLayerPath As String = layerPath & "\" & AOIVectorList(i)
                Dim item As LayerListItem = New LayerListItem(AOIVectorList(i), fullLayerPath, LayerType.Vector, True)
                LstVectors.Items.Add(item)
            Next
        End If

        'display raster layers
        Dim rasterCount As Integer = UBound(AOIRasterList)
        If rasterCount > 0 Then
            For i = 1 To rasterCount
                Dim fullLayerPath As String = layerPath & "\" & AOIRasterList(i)
                If BA_IsIntegerRasterGDB(fullLayerPath) AndAlso BA_HasAttributeTable(fullLayerPath) Then
                    Dim item As LayerListItem = New LayerListItem(AOIRasterList(i), fullLayerPath, LayerType.Raster, True)
                    LstRasters.Items.Add(item)
                End If
            Next
        End If

        'display zonal layers
        Dim lstZoneLayers As IList(Of String) = New List(Of String)
        lstZoneLayers.Add(BA_RasterElevationZones)
        lstZoneLayers.Add(BA_RasterPrecipitationZones)
        lstZoneLayers.Add(BA_RasterSlopeZones)
        lstZoneLayers.Add(BA_RasterAspectZones)
        lstZoneLayers.Add(BA_RasterSNOTELZones)
        lstZoneLayers.Add(BA_RasterSnowCourseZones)
        For Each zoneLayer As String In lstZoneLayers
            Dim zonePath As String = m_analysisFolder + "\" + zoneLayer
            If BA_File_Exists(zonePath, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                Dim item As LayerListItem = New LayerListItem(zoneLayer, zonePath, LayerType.Raster, True)
                LstRasters.Items.Add(item)
            End If
        Next

    End Sub

    Private Function GenerateProximityLayer(ByVal pStepProg As IStepProgressor, ByVal snapRasterPath As String) As BA_ReturnCode
        If m_reuseConstraintLayersFlag = False Then
            pStepProg.Message = "Generating proximity layer"
            pStepProg.Step()

            Dim success As BA_ReturnCode = BA_ReturnCode.UnknownError
            Dim lstVectorsToDelete As IList(Of String) = New List(Of String)
            Dim tempProximity As String = Nothing
            Dim outFeaturesPath As String = Nothing
            Dim count As Int16 = 0
            'Use this to hold the list of layers that we send to the merge tool
            Dim sb As StringBuilder = New StringBuilder()
            For Each dRow As DataGridViewRow In GrdProximity.Rows
                '--- Calculate correct buffer distance based on XY units ---
                Dim comps As Double = -1
                Dim bufferDistance As Double = 0
                Dim strBuffer As String = Convert.ToString(dRow.Cells(m_idxBufferDistance).Value)
                Dim isNumber As Boolean = Double.TryParse(strBuffer, comps)
                If isNumber Then
                    bufferDistance = comps
                End If
                strBuffer = strBuffer + " "
                Select Case m_usingXYUnits
                    Case esriUnits.esriFeet
                        strBuffer = strBuffer + MeasurementUnit.Feet.ToString
                    Case esriUnits.esriKilometers
                        strBuffer = strBuffer + MeasurementUnit.Kilometers.ToString
                    Case esriUnits.esriMiles
                        strBuffer = strBuffer + MeasurementUnit.Miles.ToString
                    Case Else
                        strBuffer = strBuffer + MeasurementUnit.Meters.ToString
                End Select

                tempProximity = "ps_prox_v" & count
                outFeaturesPath = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True) + tempProximity
                success = BA_Buffer(Convert.ToString(dRow.Cells(m_idxFullPaths).Value), outFeaturesPath, strBuffer, "ALL")
                If success = BA_ReturnCode.Success Then
                    success = BA_AddUserFieldToVector(m_analysisFolder, tempProximity, BA_FIELD_PSITE, esriFieldType.esriFieldTypeInteger,
                                                      -1, "1")
                    If success = BA_ReturnCode.Success Then
                        sb.Append(outFeaturesPath + "; ")
                    End If
                End If
                lstVectorsToDelete.Add(outFeaturesPath)
                count += 1
            Next
            If count > 1 AndAlso success = BA_ReturnCode.Success Then
                'Merge all features
                sb.Remove(sb.ToString().LastIndexOf("; "), "; ".Length)
                outFeaturesPath = m_analysisFolder + "\tmpMerge"
                lstVectorsToDelete.Add(outFeaturesPath)
                success = BA_Intersect(sb.ToString, outFeaturesPath)
            End If
            If success = BA_ReturnCode.Success Then
                success = BA_Feature2RasterGP(outFeaturesPath, m_analysisFolder + "\" + m_proximityLayer, BA_FIELD_PSITE, m_cellSize, snapRasterPath)
            End If
            If success = BA_ReturnCode.Success Then
                'Add 'NAME' field to be used as label for map
                success = BA_AddUserFieldToRaster(m_analysisFolder, m_proximityLayer, BA_FIELD_NAME, esriFieldType.esriFieldTypeString,
                                              100, BA_MAPS_PS_PROXIMITY)
                For Each aPath As String In lstVectorsToDelete
                    Dim folderName As String = "PleaseReturn"
                    Dim fileName As String = BA_GetBareName(outFeaturesPath, folderName)
                    Dim retVal As Int16 = BA_Remove_ShapefileFromGDB(folderName, fileName)
                Next
            End If

            If Not success = BA_ReturnCode.Success Then
                MessageBox.Show("An error occurred while generating the proximity layer. It will not be used in analysis")
            End If
            Return success
        Else
            If BA_File_Exists(m_analysisFolder & "\" & m_proximityLayer, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                Return BA_ReturnCode.Success
            Else
                Return BA_ReturnCode.UnknownError
            End If
        End If
    End Function

    Private Sub SuggestSiteName()
        Dim psuedoList As IList(Of Site) = BA_ReadSiteAttributes(SiteType.Pseudo)
        Dim pSitePrefix As String = "auto_site_"
        Dim pSiteId As Short = 0
        Dim bName As Boolean = False
        If psuedoList.Count > 0 Then
            Do While bName = False
                pSiteId += 1
                bName = True
                For Each pSite As Site In psuedoList
                    If pSite.Name.Equals(pSitePrefix & pSiteId) Then
                        bName = False
                        Exit For
                    End If
                Next
            Loop
        Else
            pSiteId += 1
        End If
        TxtSiteName.Text = pSitePrefix & pSiteId
    End Sub

    Private Sub SavePseudoSiteLog()
        TxtSiteName.Text = TxtSiteName.Text.Trim()
        m_lastAnalysis = New PseudoSite(m_siteId, TxtSiteName.Text, CkElev.Checked, CkPrecip.Checked, CkProximity.Checked,
                                        CkLocation.Checked)
        'Save Elevation data
        If m_lastAnalysis.UseElevation Then
            Dim elevUnits As esriUnits = esriUnits.esriMeters
            If m_usingElevMeters = False Then _
                elevUnits = esriUnits.esriFeet
            m_lastAnalysis.ElevationProperties(elevUnits, CDbl(txtLower.Text), CDbl(TxtUpperRange.Text))
        End If
        'Save Prism settings
        If m_lastAnalysis.UsePrism Then
            m_lastAnalysis.PrismProperties(CmboxPrecipType.SelectedIndex, CmboxBegin.SelectedIndex, CmboxEnd.SelectedIndex,
                                  CDbl(TxtPrecipUpper.Text), CDbl(TxtPrecipLower.Text))
        End If
        'Save Proximity settings
        If m_lastAnalysis.UseProximity Then
            For Each pRow As DataGridViewRow In GrdProximity.Rows
                Dim comps As Double = -1
                Dim filePath As String = Convert.ToString(pRow.Cells(m_idxFullPaths).Value)
                Dim layerName As String = Convert.ToString(pRow.Cells(m_idxLayer).Value)
                Dim isNumber As Boolean = Double.TryParse(pRow.Cells(m_idxBufferDistance).Value, comps)
                If isNumber Then
                    m_lastAnalysis.AddProximityProperties(layerName, filePath, comps, m_usingXYUnits)
                Else
                    m_lastAnalysis.AddProximityProperties(layerName, filePath, 0, m_usingXYUnits)
                End If
            Next
        End If
        'Save Location settings
        If m_lastAnalysis.UseLocation Then
            For Each pRow As DataGridViewRow In GrdLocation.Rows
                Dim filePath As String = Convert.ToString(pRow.Cells(m_idxFullPaths).Value)
                Dim selectedList As List(Of String) = m_dictLocationIncludeValues(filePath)
                Dim allValuesList As List(Of String) = m_dictLocationAllValues(filePath)
                Dim layerName As String = Convert.ToString(pRow.Cells(m_idxLayer).Value)
                m_lastAnalysis.AddLocationProperties(layerName, filePath, BA_FIELD_VALUE, selectedList, allValuesList,
                                                     BA_PS_LOCATION)
            Next
        End If
        Dim lstPseudoSites As PseudoSiteList = BA_LoadPseudoSitesFromXml(AOIFolderBase)
        If lstPseudoSites Is Nothing Then
            lstPseudoSites = New PseudoSiteList()
            lstPseudoSites.PseudoSites = New List(Of PseudoSite)
        Else
            'Set lastSiteAdded to false for all existing sites; Only the latest site, 
            'yet to be added, will be true
            For Each existingSite As PseudoSite In lstPseudoSites.PseudoSites
                existingSite.LastSiteAdded = False
            Next
        End If
        lstPseudoSites.PseudoSites.Add(m_lastAnalysis)
        Dim xmlOutputPath As String = BA_GetPath(AOIFolderBase, PublicPath.Maps) & BA_EnumDescription(PublicPath.PseudoSitesXml)
        lstPseudoSites.Save(xmlOutputPath)
    End Sub

    Public Function PreparePointFileToAppend(ByVal snapRasterPath As String, ByVal intOid As Integer) As Site
        Dim fClass As IFeatureClass = Nothing
        Dim aField As IField = Nothing
        Dim aCursor As IFeatureCursor = Nothing
        Dim aFeature As IFeature = Nothing
        Try
            '1. Delete any fields that aren't shape or objectid
            fClass = BA_OpenFeatureClassFromGDB(m_analysisFolder, m_siteFileName)
            If fClass IsNot Nothing Then
                For i As Short = fClass.Fields.FieldCount - 1 To 0 Step -1
                    aField = fClass.Fields.Field(i)
                    Select Case aField.Name
                        Case BA_FIELD_OBJECT_ID
                            'Do nothing
                        Case BA_FIELD_SHAPE
                            'Do nothing
                        Case Else
                            fClass.DeleteField(aField)
                    End Select
                Next
            End If
            '2. Calculate site elevation: Use extract values to points
            Dim filledDemPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Surfaces, True) + BA_EnumDescription(MapsFileName.filled_dem_gdb)
            Dim tempFileName As String = "tmpExtract"
            Dim success As BA_ReturnCode = BA_ExtractValuesToPoints(m_analysisFolder + "\" + m_siteFileName, filledDemPath,
                                                                    m_analysisFolder + "\" + tempFileName, snapRasterPath, False)
            Dim newSite As Site = Nothing
            If success = BA_ReturnCode.Success Then
                Dim elev As Double = 9999.0
                fClass = BA_OpenFeatureClassFromGDB(m_analysisFolder, tempFileName)
                Dim idxElev As Short = fClass.Fields.FindField(BA_FIELD_RASTERVALU)
                If idxElev > -1 Then
                    aCursor = fClass.Search(Nothing, False)
                    aFeature = aCursor.NextFeature
                    If aFeature IsNot Nothing Then
                        elev = Convert.ToDouble(aFeature.Value(idxElev))
                    End If
                End If
                BA_Remove_ShapefileFromGDB(m_analysisFolder, tempFileName)
                '3. Updates the site attributes
                TxtSiteName.Text = TxtSiteName.Text.Trim()
                newSite = New Site(intOid, TxtSiteName.Text, SiteType.Pseudo, elev, False)
                success = BA_UpdatePseudoSiteAttributes(m_analysisFolder, m_siteFileName, intOid, newSite)
            End If
            Return newSite
        Catch ex As Exception
            Debug.Print("PreparePointFileToAppend: " & ex.Message)
            Return Nothing
        Finally
            fClass = Nothing
            aField = Nothing
            aCursor = Nothing
            aFeature = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Function

    Public Function GetNewSiteObjectId(ByVal siteElev As Double) As Integer
        Dim fClass As IFeatureClass = Nothing
        Dim aCursor As IFeatureCursor = Nothing
        Dim aFeature As IFeature = Nothing
        Dim aQueryFilter As IQueryFilter = New QueryFilter()
        Try
            Dim objectId As Integer = -1
            fClass = BA_OpenFeatureClassFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers), BA_EnumDescription(MapsFileName.Pseudo))
            Dim idxOid As Short = fClass.Fields.FindField(BA_FIELD_OBJECT_ID)
            TxtSiteName.Text = TxtSiteName.Text.Trim()
            Dim queryElev As Integer = Math.Round(siteElev, 0)  'Round to avoid rounding differences with ArcMap
            aQueryFilter.WhereClause = " " & BA_SiteNameField & " = '" & TxtSiteName.Text &
                                       "' and ROUND(" & BA_SiteElevField & ",0) = " & queryElev
            If idxOid > -1 Then
                aCursor = fClass.Search(aQueryFilter, False)
                aFeature = aCursor.NextFeature
                Do While aFeature IsNot Nothing
                    If aFeature IsNot Nothing Then
                        objectId = Convert.ToInt16(aFeature.Value(idxOid))
                    End If
                    aFeature = aCursor.NextFeature
                Loop
            End If
            Return objectId
        Catch ex As Exception
            Debug.Print("GetNewSiteObjectId: " & ex.Message)
            Return -1
        End Try
    End Function

    Public Event FormInputChanged()

    Protected Sub Form_InputChanged() Handles Me.FormInputChanged
        BtnMap.Enabled = False
        BtnFindSite.Enabled = True
        BtnRecalculate.Enabled = False
        BtnDefineSiteSame.Enabled = False
        m_reuseConstraintLayersFlag = False
        m_lastAnalysis = Nothing
    End Sub

    Private Sub CmboxBegin_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxBegin.SelectedIndexChanged
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub CmboxEnd_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CmboxEnd.SelectedIndexChanged
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub TxtPrecipUpper_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TxtPrecipUpper.Validating
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub TxtPrecipLower_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TxtPrecipLower.Validating
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub LstVectors_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles LstVectors.SelectedIndexChanged
        txtBufferDistance.Text = Nothing
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub txtBufferDistance_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        RaiseEvent FormInputChanged()
    End Sub

    Private Function ValidBufferDistance() As String
        Dim item As LayerListItem = LstVectors.SelectedItem
        Dim sb As StringBuilder = New StringBuilder
        If item IsNot Nothing Then
            Dim fClass As IFeatureClass = BA_OpenFeatureClassFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers), item.Name)
            If fClass IsNot Nothing Then
                If Not fClass.ShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon Then
                    Dim comps As Double = -1
                    If Double.TryParse(txtBufferDistance.Text, comps) Then
                        If comps <= 0 Then
                            sb.Append("Value greater than 0 required for features that are not polygons!" + vbCrLf)
                        End If
                    Else
                        sb.Append("Numeric value required for features that are not polygons!" + vbCrLf)
                    End If
                End If
            Else
                sb.Append("Unable to open selected feature class!" + vbCrLf)
            End If
        End If
        Return sb.ToString
    End Function

    Private Sub BtnClear_Click(sender As System.Object, e As System.EventArgs) Handles BtnClear.Click
        SuggestSiteName()
        Me.EnableForm(True)
        CkElev.Checked = False
        txtLower.Text = txtMinElev.Text
        TxtUpperRange.Text = TxtMaxElev.Text
        CkPrecip.Checked = False
        CmboxPrecipType.SelectedIndex = 0
        CmboxBegin.SelectedIndex = 0
        CmboxEnd.SelectedIndex = 0
        txtMinPrecip.Text = "-"
        txtMaxPrecip.Text = "-"
        TxtPrecipUpper.Text = Nothing
        TxtPrecipLower.Text = Nothing
        CkProximity.Checked = False
        LstRasters.ClearSelected()
        LstVectors.ClearSelected()
        txtBufferDistance.Text = Nothing
        CkLocation.Checked = False
        GrdProximity.Rows.Clear()
        GrdLocation.Rows.Clear()
        If m_dictLocationAllValues IsNot Nothing Then
            m_dictLocationAllValues.Clear()
            m_dictLocationIncludeValues.Clear()
        End If
        BtnFindSite.Enabled = True
        BtnMap.Enabled = False
        BtnRecalculate.Enabled = False
        BtnDefineSiteSame.Enabled = False
        m_reuseConstraintLayersFlag = False
        m_lastAnalysis = Nothing
        BA_Last_PseudoSite = Nothing
    End Sub

    Private Sub txtMinPrecip_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtMinPrecip.TextChanged
        ManagePrecipRange()
    End Sub

    Private Sub txtMaxPrecip_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtMaxPrecip.TextChanged
        ManagePrecipRange()
    End Sub

    Private Sub ManagePrecipRange()
        Dim comps As Double = -1
        If Double.TryParse(txtMinPrecip.Text, comps) Then
            If Double.TryParse(txtMaxPrecip.Text, comps) Then
                TxtPrecipUpper.Enabled = True
                TxtPrecipLower.Enabled = True
            Else
                TxtPrecipUpper.Enabled = False
                TxtPrecipLower.Enabled = False
            End If
        Else
            TxtPrecipUpper.Enabled = False
            TxtPrecipLower.Enabled = False
        End If
    End Sub

    Private Sub CkLocation_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CkLocation.CheckedChanged
        GrpLocation.Enabled = CkLocation.Checked
        If CkLocation.Checked = False Then
            If m_dictLocationAllValues IsNot Nothing Then
                m_dictLocationAllValues.Clear()
                m_dictLocationIncludeValues.Clear()
            End If
            GrdLocation.Rows.Clear()
        End If
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub LstRasters_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles LstRasters.SelectedIndexChanged
        PopulateValuesList()
        RaiseEvent FormInputChanged()
    End Sub

    Private Sub ToggleLocationButtons(ByVal enabled As Boolean)
        BtnAddLocation.Enabled = enabled
        BtnDeleteLocation.Enabled = enabled
        BtnEditLocation.Enabled = enabled
        If GrdLocation.SelectedRows.Count = 0 Then
            BtnDeleteLocation.Enabled = False
            BtnEditLocation.Enabled = False
        End If
    End Sub

    Private Sub BtnAddLocation_Click(sender As System.Object, e As System.EventArgs) Handles BtnAddLocation.Click
        PnlLocation.Visible = True
    End Sub

    Private Sub PnlLocation_VisibleChanged(sender As Object, e As System.EventArgs) Handles PnlLocation.VisibleChanged
        ToggleLocationButtons(Not PnlLocation.Visible)
    End Sub

    Private Sub BtnCancelLocation_Click(sender As System.Object, e As System.EventArgs) Handles BtnCancelLocation.Click
        PnlLocation.Visible = False
        LstRasters.ClearSelected()
    End Sub

    Private Sub BtnSaveLocation_Click(sender As System.Object, e As System.EventArgs) Handles BtnSaveLocation.Click
        Dim sb As StringBuilder = New StringBuilder()
        Dim lstAllValues As IList(Of String) = New List(Of String)
        Dim lstSelectValues As IList(Of String) = New List(Of String)
        If m_dictLocationAllValues Is Nothing Then
            m_dictLocationAllValues = New Dictionary(Of String, IList(Of String))
            m_dictLocationIncludeValues = New Dictionary(Of String, IList(Of String))
        End If
        Dim rasterItem As LayerListItem = LstRasters.SelectedItem
        Dim overwriteExisting As Boolean = False
        If m_dictLocationAllValues.ContainsKey(rasterItem.Value) Then
            Dim strMsg As String = "Selected values have already been configured for this Location layer. Do you " +
            "wish to overwrite that configuration ?"
            Dim res As DialogResult = MessageBox.Show(strMsg, "Location exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If res <> DialogResult.Yes Then
                Exit Sub
            Else
                overwriteExisting = True
            End If
        End If
        If LstValues.SelectedItems.Count < 1 Then
            MessageBox.Show("You must select at least one value to use this layer in the analysis")
            Exit Sub
        Else
            For i As Integer = 0 To LstValues.Items.Count - 1
                Dim listItem As LayerListItem = LstValues.Items(i)
                lstAllValues.Add(listItem.Value)
                If LstValues.GetSelected(i) Then
                    sb.Append(listItem.Value + m_sep)
                    lstSelectValues.Add(listItem.Value)
                End If
            Next
            sb.Remove(sb.ToString().LastIndexOf(m_sep), m_sep.Length)
            If overwriteExisting = False Then
                m_dictLocationAllValues.Add(rasterItem.Value, lstAllValues)
                m_dictLocationIncludeValues.Add(rasterItem.Value, lstSelectValues)
            Else
                m_dictLocationAllValues(rasterItem.Value) = lstAllValues
                m_dictLocationIncludeValues(rasterItem.Value) = lstSelectValues
            End If
            Dim item As New DataGridViewRow
            If overwriteExisting = True Then
                For Each aRow As DataGridViewRow In GrdLocation.Rows
                    Dim fullPath As String = Convert.ToString(aRow.Cells(m_idxFullPaths).Value)
                    If fullPath.Equals(rasterItem.Value) Then
                        item = aRow
                        Exit For
                    End If
                Next
            End If
            If item.Cells.Count = 0 Then _
                item.CreateCells(GrdLocation)
            With item
                .Cells(m_idxLayer).Value = rasterItem.Name
                .Cells(m_idxValues).Value = sb.ToString
                .Cells(m_idxFullPaths).Value = rasterItem.Value
            End With
            '---add the row---
            If Not GrdLocation.Rows.Contains(item) Then _
                GrdLocation.Rows.Add(item)
            LstRasters.ClearSelected()
            GrdLocation.CurrentCell = Nothing
            BtnCancelLocation_Click(sender, e)
        End If
    End Sub

    Private Sub GrdLocation_SelectionChanged(sender As Object, e As System.EventArgs) Handles GrdLocation.SelectionChanged
        If PnlLocation.Visible = False Then
            ToggleLocationButtons(True)
        End If
    End Sub

    Private Sub BtnDeleteLocation_Click(sender As System.Object, e As System.EventArgs) Handles BtnDeleteLocation.Click
        If GrdLocation.SelectedRows.Count > 0 Then
            Dim res As DialogResult = MessageBox.Show("You are about to delete a row from the Location constraint list. " +
                                                      "This cannot be undone." + vbCrLf + vbCrLf + "Do you wish to continue ?",
                                                      "Delete", MessageBoxButtons.YesNo)
            If res = DialogResult.Yes Then
                Dim dRow As DataGridViewRow = GrdLocation.SelectedRows(0)
                Dim fullPath As String = Convert.ToString(dRow.Cells(m_idxFullPaths).Value)
                If m_dictLocationAllValues.ContainsKey(fullPath) Then _
                    m_dictLocationAllValues.Remove(fullPath)
                m_dictLocationIncludeValues.Remove(fullPath)
                GrdLocation.Rows.Remove(GrdLocation.SelectedRows(0))
            End If
        Else
            MessageBox.Show("You must select a row to delete")
        End If
    End Sub

    Private Sub DeletePreviousRun()
        Dim fileToDelete As String = m_analysisFolder + "\" + m_elevLayer
        If BA_File_Exists(fileToDelete, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(m_analysisFolder, m_elevLayer)
        End If
        fileToDelete = m_analysisFolder + "\" + m_precipLayer
        If BA_File_Exists(fileToDelete, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(m_analysisFolder, m_precipLayer)
        End If
        fileToDelete = m_analysisFolder + "\" + m_proximityLayer
        If BA_File_Exists(fileToDelete, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(m_analysisFolder, m_proximityLayer)
        End If
        fileToDelete = m_analysisFolder + "\" + m_locationLayer
        If BA_File_Exists(fileToDelete, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(m_analysisFolder, m_locationLayer)
        End If
        fileToDelete = m_analysisFolder + "\" + m_cellStatsLayer
        If BA_File_Exists(fileToDelete, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
            BA_RemoveRasterFromGDB(m_analysisFolder, m_cellStatsLayer)
        End If
        fileToDelete = m_analysisFolder + "\" + m_siteFileName
        If BA_File_Exists(fileToDelete, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
            BA_Remove_ShapefileFromGDB(m_analysisFolder, m_siteFileName)
        End If
    End Sub

    Private Sub BtnEditLocation_Click(sender As System.Object, e As System.EventArgs) Handles BtnEditLocation.Click
        If GrdLocation.SelectedRows.Count > 0 Then
            Dim dRow As DataGridViewRow = GrdLocation.SelectedRows(0)
            Dim layerPath As String = Convert.ToString(dRow.Cells(m_idxFullPaths).Value)
            Dim fullPath = layerPath
            'Fix the full path if the AOI was moved to another filefolder
            If Not BA_File_Exists(fullPath, WorkspaceType.Geodatabase, esriDatasetType.esriDTRasterDataset) Then
                Dim idxGdb As Integer = -1
                Dim arrGeodatabases() As String = {BA_EnumDescription(GeodatabaseNames.Layers), BA_EnumDescription(GeodatabaseNames.Analysis)}
                For Each geodatabase As String In arrGeodatabases
                    idxGdb = layerPath.IndexOf(geodatabase)
                    If idxGdb > 0 Then
                        fullPath = AOIFolderBase + "\" + fullPath.Substring(idxGdb)
                        If m_dictLocationIncludeValues.ContainsKey(layerPath) Then
                            m_dictLocationIncludeValues.Add(fullPath, m_dictLocationIncludeValues(layerPath))
                            m_dictLocationIncludeValues.Remove(layerPath)
                        End If
                        If m_dictLocationAllValues.ContainsKey(layerPath) Then
                            m_dictLocationAllValues.Add(fullPath, m_dictLocationAllValues(layerPath))
                            m_dictLocationAllValues.Remove(layerPath)
                        End If
                        Exit For
                    End If
                Next
            End If

            LstRasters.ClearSelected()
            For i As Int16 = 0 To LstRasters.Items.Count - 1
                Dim item As LayerListItem = LstRasters.Items(i)
                If item.Value.Equals(fullPath) Then
                    LstRasters.SelectedIndex = i
                    dRow.Cells(m_idxFullPaths).Value = fullPath
                    Exit For
                End If
            Next
            If LstRasters.SelectedIndex > -1 Then
                Dim lstSelectValues As List(Of String) = m_dictLocationIncludeValues(fullPath)
                For j As Int16 = 0 To LstValues.Items.Count - 1
                    Dim listItem As LayerListItem = LstValues.Items(j)
                    If lstSelectValues.Contains(listItem.Value) Then
                        LstValues.SetSelected(j, True)
                    End If
                Next
            End If
            PnlLocation.Visible = True
        Else
            MessageBox.Show("You must select a row to edit")
        End If
    End Sub

    Private Sub PopulateValuesList()
        LstValues.Items.Clear()
        Dim pGeodataset As IGeoDataset = Nothing
        Dim pRasterBandCollection As IRasterBandCollection = Nothing
        Dim pRasterBand As IRasterBand = Nothing
        Dim pTable As ITable = Nothing
        Dim valuesCursor As ICursor = Nothing
        Dim pRow As IRow = Nothing
        Try
            If LstRasters.SelectedIndex > -1 Then
                Dim selItem As LayerListItem = LstRasters.SelectedItem
                Dim folderName As String = "PleaseReturn"
                Dim fileName As String = BA_GetBareName(selItem.Value, folderName)
                pGeodataset = BA_OpenRasterFromGDB(folderName, fileName)
                If pGeodataset IsNot Nothing Then
                    pRasterBandCollection = CType(pGeodataset, IRasterBandCollection)
                    pRasterBand = pRasterBandCollection.Item(0)
                    pTable = pRasterBand.AttributeTable
                    If pTable IsNot Nothing Then
                        Dim idxName As Int16 = pTable.FindField(BA_FIELD_NAME)
                        Dim idxValue As Int16 = pTable.FindField(BA_FIELD_VALUE)
                        If idxValue < 0 Then
                            MessageBox.Show("The layer you selected does not have a 'value' field. It cannot be used as a Location constraint")
                            Exit Sub
                        End If
                        Dim item As LayerListItem = Nothing
                        valuesCursor = pTable.Search(Nothing, False)
                        pRow = valuesCursor.NextRow
                        Do While pRow IsNot Nothing
                            Dim strName As String = Nothing
                            Dim strValue As String = Nothing
                            If idxName > -1 Then _
                                strName = Convert.ToString(pRow.Value(idxName))
                            If idxValue > -1 Then _
                                strValue = Convert.ToString(pRow.Value(idxValue))
                            If Not String.IsNullOrEmpty(strName) Then
                                item = New LayerListItem(strValue + ": " + strName, strValue, LayerType.Raster, True)
                            Else
                                item = New LayerListItem(strValue, strValue, LayerType.Raster, True)
                            End If
                            LstValues.Items.Add(item)
                            pRow = valuesCursor.NextRow
                        Loop
                    End If
                End If
            End If
        Catch ex As Exception
            Debug.Print("PopulateValuesList Exception: " & ex.Message)
        Finally
            pGeodataset = Nothing
            pRasterBandCollection = Nothing
            pRasterBand = Nothing
            pTable = Nothing
            valuesCursor = Nothing
            pRow = Nothing
        End Try
    End Sub

    Private Sub BtnToggle_Click(sender As System.Object, e As System.EventArgs) Handles BtnToggle.Click
        For i = LstValues.Items.Count - 1 To 0 Step -1
            Dim newValue As Boolean = Not LstValues.GetSelected(i)
            LstValues.SetSelected(i, newValue)
        Next i
    End Sub

    Private Sub LstValues_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles LstValues.SelectedIndexChanged
        If LstValues.SelectedItems.Count > 0 Then
            BtnToggle.Enabled = True
        Else
            BtnToggle.Enabled = False
        End If
    End Sub

    Private Sub BtnCancelProximity_Click(sender As Object, e As System.EventArgs) Handles BtnCancelProximity.Click
        PnlProximity.Visible = False
        LstVectors.ClearSelected()
        txtBufferDistance.Text = Nothing
    End Sub

    Private Sub BtnAddProximity_Click(sender As System.Object, e As System.EventArgs) Handles BtnAddProximity.Click
        PnlProximity.Visible = True
    End Sub

    Private Sub PnlProximity_VisibleChanged(sender As Object, e As System.EventArgs) Handles PnlProximity.VisibleChanged
        ToggleProximityButtons(Not PnlProximity.Visible)
    End Sub

    Private Sub ToggleProximityButtons(ByVal enabled As Boolean)
        BtnAddProximity.Enabled = enabled
        BtnDeleteProximity.Enabled = enabled
        BtnEditProximity.Enabled = enabled
        If GrdProximity.SelectedRows.Count = 0 Then
            BtnDeleteProximity.Enabled = False
            BtnEditProximity.Enabled = False
        End If
    End Sub

    Private Sub GrdProximity_SelectionChanged(sender As Object, e As System.EventArgs) Handles GrdProximity.SelectionChanged
        If PnlProximity.Visible = False Then
            ToggleProximityButtons(True)
        End If
    End Sub

    Private Sub BtnEditProximity_Click(sender As System.Object, e As System.EventArgs) Handles BtnEditProximity.Click
        If GrdProximity.SelectedRows.Count > 0 Then
            Dim dRow As DataGridViewRow = GrdProximity.SelectedRows(0)
            Dim fullPath As String = Convert.ToString(dRow.Cells(m_idxFullPaths).Value)
            If Not BA_File_Exists(fullPath, WorkspaceType.Geodatabase, esriDatasetType.esriDTFeatureClass) Then
                Dim idxGdb As Integer = -1
                idxGdb = fullPath.IndexOf(BA_EnumDescription(GeodatabaseNames.Layers))
                If idxGdb > 0 Then
                    fullPath = AOIFolderBase + "\" + fullPath.Substring(idxGdb)
                End If
            End If
            LstVectors.ClearSelected()
            For i As Int16 = 0 To LstVectors.Items.Count - 1
                Dim item As LayerListItem = LstVectors.Items(i)
                If item.Value.Equals(fullPath) Then
                    LstVectors.SelectedIndex = i
                    dRow.Cells(m_idxFullPaths).Value = fullPath
                    Exit For
                End If
            Next
            txtBufferDistance.Text = dRow.Cells(m_idxBufferDistance).Value
            PnlProximity.Visible = True
        Else
            MessageBox.Show("You must select a row to edit")
        End If
    End Sub

    Private Sub BtnDeleteProximity_Click(sender As System.Object, e As System.EventArgs) Handles BtnDeleteProximity.Click
        If GrdProximity.SelectedRows.Count > 0 Then
            Dim res As DialogResult = MessageBox.Show("You are about to delete a row from the Proximity constraint list. " +
                                                      "This cannot be undone." + vbCrLf + vbCrLf + "Do you wish to continue ?",
                                                      "Delete", MessageBoxButtons.YesNo)
            If res = DialogResult.Yes Then
                Dim dRow As DataGridViewRow = GrdProximity.SelectedRows(0)
                Dim fullPath As String = Convert.ToString(dRow.Cells(m_idxFullPaths).Value)
                GrdProximity.Rows.Remove(GrdProximity.SelectedRows(0))
            End If
        Else
            MessageBox.Show("You must select a row to delete")
        End If
    End Sub

    Private Sub BtnSaveProximity_Click(sender As System.Object, e As System.EventArgs) Handles BtnSaveProximity.Click
        Dim rasterItem As LayerListItem = LstVectors.SelectedItem
        Dim overwriteExisting As Boolean = False
        For Each dRow As DataGridViewRow In GrdProximity.Rows
            Dim filePath As String = Convert.ToString(dRow.Cells(m_idxFullPaths).Value)
            If rasterItem.Value.Equals(filePath) Then
                Dim strMsg As String = "Selected values have already been configured for this Proximity layer. Do you " +
"wish to overwrite that configuration ?"
                Dim res As DialogResult = MessageBox.Show(strMsg, "Proximity exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If res <> System.Windows.Forms.DialogResult.Yes Then
                    Exit Sub
                Else
                    overwriteExisting = True
                End If
            End If
        Next
        Dim errorMessage As String = ValidBufferDistance()
        If Not String.IsNullOrEmpty(errorMessage) Then
            MessageBox.Show(errorMessage)
            Exit Sub
        Else
            Dim item As New DataGridViewRow
            If overwriteExisting = True Then
                For Each aRow As DataGridViewRow In GrdProximity.Rows
                    Dim fullPath As String = Convert.ToString(aRow.Cells(m_idxFullPaths).Value)
                    If fullPath.Equals(rasterItem.Value) Then
                        item = aRow
                        Exit For
                    End If
                Next
            End If
            If item.Cells.Count = 0 Then _
                item.CreateCells(GrdProximity)
            With item
                .Cells(m_idxLayer).Value = rasterItem.Name
                .Cells(m_idxBufferDistance).Value = txtBufferDistance.Text
                .Cells(m_idxFullPaths).Value = rasterItem.Value
            End With
            '---add the row---
            If Not GrdProximity.Rows.Contains(item) Then _
                GrdProximity.Rows.Add(item)
            LstVectors.ClearSelected()
            GrdProximity.CurrentCell = Nothing
            BtnCancelProximity_Click(sender, e)
        End If
    End Sub

    Private Sub LoadAnalysisLog(ByVal logSite As PseudoSite)
        m_siteId = logSite.ObjectId
        TxtSiteName.Text = logSite.SiteName
        If logSite.UseElevation Then
            CkElev.Checked = True
            Dim logUsingMeters = True
            If logSite.ElevUnits = esriUnits.esriFeet Then
                logUsingMeters = False
            End If
            Dim Conversion_Factor As Double = BA_SetConversionFactor(m_usingElevMeters, logUsingMeters)
            txtLower.Text = CStr(Math.Round(logSite.LowerElev * Conversion_Factor))
            TxtUpperRange.Text = CStr(Math.Round(logSite.UpperElev * Conversion_Factor))
        End If
        If logSite.UsePrism Then
            CkPrecip.Checked = True
            CmboxPrecipType.SelectedIndex = logSite.PrecipTypeIdx
            If CmboxPrecipType.SelectedIndex = 5 Then
                lblBeginMonth.Enabled = True
                CmboxBegin.Enabled = True
                CmboxBegin.SelectedIndex = logSite.PrecipBeginIdx
                lblEndMonth.Enabled = True
                CmboxEnd.Enabled = True
                CmboxEnd.SelectedIndex = logSite.PrecipEndIdx
            Else
                lblBeginMonth.Enabled = False
                CmboxBegin.Enabled = False
                lblEndMonth.Enabled = False
                CmboxEnd.Enabled = False
            End If
            CmdPrism_Click(Me, EventArgs.Empty)
            TxtPrecipUpper.Text = CStr(logSite.UpperPrecip)
            TxtPrecipLower.Text = CStr(logSite.LowerPrecip)
        End If
        If logSite.UseProximity = True Then
            CkProximity.Checked = True
            If logSite.ProximityLayers IsNot Nothing AndAlso logSite.ProximityLayers.Count > 0 Then
                For Each pLayer As PseudoSiteLayer In logSite.ProximityLayers
                    'Convert distance if units are different
                    Dim bufferDistance As Double = pLayer.BufferDistance
                    If pLayer.BufferUnits <> m_usingXYUnits Then
                        Dim converter As IUnitConverter = New UnitConverter
                        bufferDistance = Math.Round(converter.ConvertUnits(bufferDistance, pLayer.BufferUnits, m_usingXYUnits), 3)
                        If pLayer.BufferUnits = esriUnits.esriUnknownUnits Then
                            MessageBox.Show("The units for a proximity layer are missing. The distance displayed may not be correct!", "BAGIS")
                        End If
                    End If
                    Dim item As New DataGridViewRow
                    item.CreateCells(GrdLocation)
                    With item
                        .Cells(m_idxLayer).Value = pLayer.LayerName
                        .Cells(m_idxBufferDistance).Value = bufferDistance
                        .Cells(m_idxFullPaths).Value = pLayer.LayerPath
                    End With
                    '---add the row---
                    GrdProximity.Rows.Add(item)
                Next
                'Clear selection on grid
                If GrdProximity.Rows.Count > 0 Then
                    GrdProximity(1, 0).Selected = True
                    GrdProximity.ClearSelection()
                End If
            End If
        End If
        If logSite.UseLocation = True Then
            CkLocation.Checked = True
            If logSite.LocationLayers IsNot Nothing AndAlso logSite.LocationLayers.Count > 0 Then
                If m_dictLocationAllValues Is Nothing Then
                    m_dictLocationAllValues = New Dictionary(Of String, IList(Of String))
                    m_dictLocationIncludeValues = New Dictionary(Of String, IList(Of String))
                End If
                For Each pLayer As PseudoSiteLayer In logSite.LocationLayers
                    m_dictLocationAllValues.Add(pLayer.LayerPath, pLayer.AllValues)
                    m_dictLocationIncludeValues.Add(pLayer.LayerPath, pLayer.SelectedValues)
                    Dim valSb As StringBuilder = New StringBuilder()
                    For Each strValue As String In pLayer.SelectedValues
                        valSb.Append(strValue + m_sep)
                    Next
                    valSb.Remove(valSb.ToString().LastIndexOf(m_sep), m_sep.Length)
                    Dim item As New DataGridViewRow
                    item.CreateCells(GrdLocation)
                    With item
                        .Cells(m_idxLayer).Value = pLayer.LayerName
                        .Cells(m_idxValues).Value = valSb.ToString
                        .Cells(m_idxFullPaths).Value = pLayer.LayerPath
                    End With
                    '---add the row---
                    GrdLocation.Rows.Add(item)
                Next
                'Clear selection on grid
                If GrdLocation.Rows.Count > 0 Then
                    GrdLocation(1, 0).Selected = True
                    GrdLocation.ClearSelection()
                End If
            End If
        End If
        Me.Text = "Auto-site log: " + BA_GetBareName(AOIFolderBase)

        'Enable copying
        m_lastAnalysis = logSite
        BtnDefineSiteSame.Enabled = True
        BtnFindSite.Enabled = False
        If logSite.LastSiteAdded = True Then
            BtnMap.Enabled = True
        Else
            BtnMap.Enabled = False
        End If
    End Sub

    Private Function LayerIsOnMap(ByVal layername As String) As Boolean
        'check if a layer with the assigned name exists
        Dim i As Long
        Dim pMap As IMap = My.Document.FocusMap()
        Dim nlayers As Long = pMap.LayerCount
        Dim pTempLayer As ILayer
        For i = nlayers To 1 Step -1
            pTempLayer = pMap.Layer(i - 1)
            If layername = pTempLayer.Name Then 'remove the layer
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub BtnRecalculate_Click(sender As System.Object, e As System.EventArgs) Handles BtnRecalculate.Click
        'Get handle to the site scenario form
        Dim dockWindowAddIn = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of frmSiteScenario.AddinImpl)(My.ThisAddIn.IDs.frmSiteScenario)
        Dim siteScenarioForm As frmSiteScenario = dockWindowAddIn.UI
        '1. Check box on frmSiteScenario
        siteScenarioForm.SelectPseudoSite(m_siteId)
        '2. Disable site scenario maps after calculation for speed
        siteScenarioForm.DisplayScenarioMapsFlag = False
        '3. Click BtnCalculate on Site Scenario form
        siteScenarioForm.BtnCalculate.PerformClick()
        'Enable/disable buttons
        BtnRecalculate.Enabled = False      'We just recalculated; Don't need to do it again
        BtnDefineSiteSame.Enabled = True    'OK to create a new site with the same parameters
        If m_lastAnalysis IsNot Nothing Then    'Save the last analysis in the session so the constraint layers can be re-used
            BA_Last_PseudoSite = m_lastAnalysis
        End If
        BtnFindSite.Enabled = False         'Why would you want to find site with same parameters? It will be re-enabled if you change anything or clear the form
    End Sub

    Private Sub BtnDefineSiteSame_Click(sender As System.Object, e As System.EventArgs) Handles BtnDefineSiteSame.Click
        SuggestSiteName()
        TxtSiteName.Text = TxtSiteName.Text.Trim()
        Dim siteName As String = InputBox("Please enter name for new pseudo-site:", "BAGIS V3", TxtSiteName.Text)
        If String.IsNullOrEmpty(siteName) Then
            Exit Sub
        Else
            TxtSiteName.Text = siteName
        End If

        'The form was in read-only mode so we need to make it writeable
        If Me.Text.IndexOf("Auto-site log:") > -1 Then
            ' Disable/hide controls on read-only form
            Me.Text = "Add Pseudo Site: " + BA_GetBareName(AOIFolderBase)
            Me.EnableForm(True)
        End If

        'Validate and fix the paths for proximity layers
        Dim lstProximityLayers As IList(Of String) = New List(Of String)
        Dim lstIdxToDelete As IList(Of Integer) = New List(Of Integer)
        'For Each row As DataGridViewRow In GrdProximity.Rows
        For i As Integer = 0 To GrdProximity.RowCount - 1
            Dim row As DataGridViewRow = GrdProximity.Rows.Item(i)
            Dim layerName As String = Convert.ToString(row.Cells(m_idxLayer).Value)
            Dim layerLocation As String = Convert.ToString(row.Cells(m_idxFullPaths).Value)
            If BA_File_Exists(layerLocation, WorkspaceType.Geodatabase,
                              esriDatasetType.esriDTFeatureDataset) Then
                'Do nothing; The layer is fine
            Else
                Dim isValid = False
                Dim idxGdb As Integer = -1
                idxGdb = layerLocation.IndexOf(BA_EnumDescription(GeodatabaseNames.Layers))
                If idxGdb > 0 Then
                    Dim relPath As String = layerLocation.Substring(idxGdb)
                    If BA_File_Exists(AOIFolderBase + "\" + relPath, WorkspaceType.Geodatabase,
                        esriDatasetType.esriDTFeatureClass) Then
                        row.Cells(m_idxFullPaths).Value = AOIFolderBase + "\" + relPath
                        isValid = True
                        Exit For
                    End If
                End If
                If Not isValid Then
                    lstProximityLayers.Add(layerName)
                    lstIdxToDelete.Add(i)
                End If
            End If
        Next
        'One of more of the layers cannot be used
        If lstProximityLayers.Count > 0 Then
            For Each idx As Integer In lstIdxToDelete
                GrdProximity.Rows.RemoveAt(idx)
            Next
            Dim sb As StringBuilder = New StringBuilder
            sb.Append("The following proximity layers could not be located on your computer and were removed from the analysis: " + vbCrLf)
            For Each layerName As String In lstProximityLayers
                sb.Append(layerName + vbCrLf)
            Next
            sb.Append(vbCrLf + "Do you wish to continue?")
            Dim res As DialogResult = MessageBox.Show(sb.ToString, "BAGIS", MessageBoxButtons.YesNo)
            If res <> DialogResult.Yes Then
                Exit Sub
            End If
        End If

        'Validate and fix the paths for location layers
        Dim lstLocationLayers As IList(Of String) = New List(Of String)
        lstIdxToDelete.Clear()
        For i As Integer = 0 To GrdLocation.RowCount - 1
            Dim row As DataGridViewRow = GrdLocation.Rows.Item(i)
            Dim layerName As String = Convert.ToString(row.Cells(m_idxLayer).Value)
            Dim layerLocation As String = Convert.ToString(row.Cells(m_idxFullPaths).Value)
            If BA_File_Exists(layerLocation, WorkspaceType.Geodatabase,
                              esriDatasetType.esriDTRasterDataset) Then
                'Do nothing; The layer is fine
            Else
                Dim isValid = False
                Dim idxGdb As Integer = -1
                Dim arrGeodatabases() As String = {BA_EnumDescription(GeodatabaseNames.Layers), BA_EnumDescription(GeodatabaseNames.Analysis)}
                For Each geodatabase As String In arrGeodatabases
                    idxGdb = layerLocation.IndexOf(geodatabase)
                    If idxGdb > 0 Then
                        Dim relPath As String = layerLocation.Substring(idxGdb)
                        If BA_File_Exists(AOIFolderBase + "\" + relPath, WorkspaceType.Geodatabase,
                            esriDatasetType.esriDTRasterDataset) Then
                            row.Cells(m_idxFullPaths).Value = AOIFolderBase + "\" + relPath
                            ' Fix the full path in the location layer dictionaries
                            If m_dictLocationAllValues.ContainsKey(layerLocation) Then
                                m_dictLocationAllValues.Add(AOIFolderBase + "\" + relPath, m_dictLocationAllValues.Item(layerLocation))
                                m_dictLocationAllValues.Remove(layerLocation)
                            End If
                            If m_dictLocationIncludeValues.ContainsKey(layerLocation) Then
                                m_dictLocationIncludeValues.Add(AOIFolderBase + "\" + relPath, m_dictLocationIncludeValues.Item(layerLocation))
                                m_dictLocationIncludeValues.Remove(layerLocation)
                            End If
                            isValid = True
                            Exit For
                        End If
                    End If
                Next
                If Not isValid Then
                    lstLocationLayers.Add(layerName)
                    lstIdxToDelete.Add(i)
                    m_dictLocationAllValues.Remove(layerLocation)
                    m_dictLocationIncludeValues.Remove(layerLocation)
                End If
            End If
        Next
        'One of more of the layers cannot be used
        If lstLocationLayers.Count > 0 Then
            For Each idx As Integer In lstIdxToDelete
                GrdLocation.Rows.RemoveAt(idx)
            Next
            Dim sb As StringBuilder = New StringBuilder
            sb.Append("The following location layers could not be located on your computer and were removed from the analysis: " + vbCrLf)
            For Each layerName As String In lstLocationLayers
                sb.Append(layerName + vbCrLf)
            Next
            sb.Append(vbCrLf + "Do you wish to continue?")
            Dim res As DialogResult = MessageBox.Show(sb.ToString, "BAGIS", MessageBoxButtons.YesNo)
            If res <> DialogResult.Yes Then
                Exit Sub
            End If
        End If

        Me.BtnFindSite.Enabled = True
        Me.BtnFindSite.PerformClick()
    End Sub

    Private Sub EnableForm(ByVal bEnabled As Boolean)
        If bEnabled = True Then
            CkElev.Enabled = True
            CkPrecip.Enabled = True
            CkLocation.Enabled = True
            CkProximity.Enabled = True
            CmdPrism.Visible = True
            BtnAddProximity.Visible = True
            BtnDeleteProximity.Visible = True
            BtnEditProximity.Visible = True
            BtnAddLocation.Visible = True
            BtnDeleteLocation.Visible = True
            BtnEditLocation.Visible = True
            BtnMap.Visible = True
            BtnClear.Visible = True
            BtnFindSite.Visible = True
            BtnRecalculate.Visible = True
            TxtSiteName.Enabled = True
            txtLower.Enabled = True
            TxtUpperRange.Enabled = True
            CmboxPrecipType.Enabled = True
            TxtPrecipLower.Enabled = True
            TxtPrecipUpper.Enabled = True
            BtnDefineSiteSame.Enabled = False
            BtnRecalculate.Enabled = False
        Else
            CkElev.Enabled = False
            CkPrecip.Enabled = False
            CkLocation.Enabled = False
            CkProximity.Enabled = False
            CmdPrism.Visible = False
            BtnAddProximity.Visible = False
            BtnDeleteProximity.Visible = False
            BtnEditProximity.Visible = False
            BtnAddLocation.Visible = False
            BtnDeleteLocation.Visible = False
            BtnEditLocation.Visible = False
            BtnMap.Enabled = False
            BtnFindSite.Enabled = False
            BtnRecalculate.Enabled = False
            TxtSiteName.Enabled = False
            txtLower.Enabled = False
            TxtUpperRange.Enabled = False
            CmboxPrecipType.Enabled = False
            TxtPrecipLower.Enabled = False
            TxtPrecipUpper.Enabled = False
        End If
    End Sub

    Private Sub BtnMultipleHelp_Click(sender As System.Object, e As System.EventArgs) Handles BtnMultipleHelp.Click
        Dim helpPath As String = BA_GetAddInDirectory() + BA_EnumDescription(PublicPath.PseudoSiteHelp)
        If IO.File.Exists(helpPath) Then
            Process.Start(helpPath)
        Else
            MessageBox.Show("The help .html file could not be found!", "BAGIS V3")
        End If
    End Sub

    Private Function CheckForSiteType() As BA_ReturnCode
        Dim pFeatClass As IFeatureClass
        Try
            pFeatClass = BA_OpenFeatureClassFromGDB(BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Layers, True), BA_EnumDescription(MapsFileName.Pseudo))
            Dim idxType As Integer = pFeatClass.FindField(BA_SiteTypeField)
            If idxType < 0 Then
                Dim pFld As IFieldEdit = New Field
                pFld.Name_2 = BA_SiteTypeField
                pFld.Type_2 = esriFieldType.esriFieldTypeString
                pFld.Length_2 = 5
                pFld.Required_2 = False
                ' Add field
                pFeatClass.AddField(pFld)
                idxType = pFeatClass.FindField(BA_SiteTypeField)

                Dim queryFilter As IQueryFilter = New QueryFilter
                queryFilter.WhereClause = "OBJECTID > 0 "
                Dim fCursor As IFeatureCursor = pFeatClass.Update(queryFilter, False)
                Dim pFeature As IFeature = fCursor.NextFeature
                Do While pFeature IsNot Nothing
                    pFeature.Value(idxType) = BA_SitePseudo
                    fCursor.UpdateFeature(pFeature)
                    pFeature = fCursor.NextFeature
                Loop
            End If
            Return BA_ReturnCode.Success
        Catch ex As Exception
            Debug.Print("CheckForSiteType Exception: " + ex.Message)
            Return BA_ReturnCode.UnknownError
        Finally
            pFeatClass = Nothing
        End Try
    End Function
End Class