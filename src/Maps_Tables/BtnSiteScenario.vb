﻿Imports ESRI.ArcGIS.esriSystem
Imports BAGIS_ClassLibrary
Imports ESRI.ArcGIS.Desktop.AddIns

Public Class BtnSiteScenario
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Public Sub New()
        Me.Enabled = False

        'Hide dockable window if there is no AOI selected
        If String.IsNullOrEmpty(AOIFolderBase) Then
            Dim dockWindow As ESRI.ArcGIS.Framework.IDockableWindow
            Dim dockWinID As UID = New UIDClass()
            dockWinID.Value = My.ThisAddIn.IDs.frmSiteScenario
            dockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(dockWinID)
            If dockWindow IsNot Nothing Then
                dockWindow.Show(False)
            End If
        End If
    End Sub

    Protected Overrides Sub OnClick()
        'Dim frmElevScenario As frmElevScenario = New frmElevScenario
        'frmElevScenario.ShowDialog()

        Dim dockWindow As ESRI.ArcGIS.Framework.IDockableWindow
        Dim dockWinID As UID = New UIDClass()
        dockWinID.Value = My.ThisAddIn.IDs.frmSiteScenario
        dockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(dockWinID)
        ' Get handle to UI (form) to reload lists
        Dim dockWindowAddIn = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of frmSiteScenario.AddinImpl)(My.ThisAddIn.IDs.frmSiteScenario)
        Dim siteScenarioForm As frmSiteScenario = dockWindowAddIn.UI
        dockWindow.Show((Not dockWindow.IsVisible()))
        ' Set dimensions of dockable window
        Dim windowPos As ESRI.ArcGIS.Framework.IWindowPosition = CType(dockWindow, ESRI.ArcGIS.Framework.IWindowPosition)
        windowPos.Height = 762
        windowPos.Width = 635

        Dim comboBox = AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        Dim aoiName As String = comboBox.getValue()

        If Len(aoiName) = 0 Then
            MsgBox("Please select an AOI!")
        Else
            If Not dockWindowAddIn.Ready(aoiName) Then
                dockWindow.Caption = "Site Scenario Analysis Tool (Current AOI --> " & aoiName & ")"
                siteScenarioForm.LoadAOIInfo(AOIFolderBase)
            End If
        End If

    End Sub

    Public WriteOnly Property selectedProperty As Boolean
        Set(ByVal value As Boolean)
            'Check to make sure the elevzone layer exists before enabling site scenario tool
            'elevzone is generated by Maps dialog but is a required layer for site scenario tool
            Dim folderPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True)
            Dim fullPath As String = folderPath & BA_EnumDescription(MapsFileName.ElevationZone)
            If value = True Then
                If Not BA_File_Exists(fullPath, WorkspaceType.Geodatabase, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                    Me.Enabled = False
                    Exit Property
                End If
            End If
            Me.Enabled = value
        End Set
    End Property

    Protected Overrides Sub OnUpdate()
        If String.IsNullOrEmpty(AOIFolderBase) Then
            'Hide dockable window if there is no AOI selected
            Dim dockWindow As ESRI.ArcGIS.Framework.IDockableWindow
            Dim dockWinID As UID = New UIDClass()
            dockWinID.Value = My.ThisAddIn.IDs.frmSiteScenario
            dockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(dockWinID)
            If dockWindow IsNot Nothing Then
                dockWindow.Show(False)
            End If
        End If
    End Sub
End Class