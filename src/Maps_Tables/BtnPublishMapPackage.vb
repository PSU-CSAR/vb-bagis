﻿Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO
Imports System.Windows.Forms
Imports BAGIS_ClassLibrary
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Desktop.AddIns

Public Class BtnPublishMapPackage
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Public Sub New()
        Me.Enabled = False

        'Hide dockable window if there is no AOI selected
        If String.IsNullOrEmpty(AOIFolderBase) Then
            Dim dockWindow As ESRI.ArcGIS.Framework.IDockableWindow
            Dim dockWinID As UID = New UIDClass()
            dockWinID.Value = My.ThisAddIn.IDs.FrmPublishMapPackage
            dockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(dockWinID)
            If dockWindow IsNot Nothing Then
                dockWindow.Show(False)
            End If
        End If
    End Sub

    Protected Overrides Sub OnClick()

        Dim comboBox = AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        Dim aoiName As String = comboBox.getValue()
        If Len(aoiName) = 0 Then
            MsgBox("Please select an AOI!")
        Else
            Dim dockWindow As ESRI.ArcGIS.Framework.IDockableWindow
            Dim dockWinID As UID = New UIDClass()
            dockWinID.Value = My.ThisAddIn.IDs.FrmPublishMapPackage
            dockWindow = My.ArcMap.DockableWindowManager.GetDockableWindow(dockWinID)
            ' Get handle to UI (form) to reload lists
            Dim dockWindowAddIn = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of FrmPublishMapPackage.AddinImpl)(My.ThisAddIn.IDs.FrmPublishMapPackage)
            Dim frmMapPackage As FrmPublishMapPackage = dockWindowAddIn.UI
            dockWindow.Show((Not dockWindow.IsVisible()))
            ' Get handle to parent window so we can place the child
            Dim parentPos As ESRI.ArcGIS.Framework.IWindowPosition = CType(My.ArcMap.Application, ESRI.ArcGIS.Framework.IWindowPosition)
            ' Set dimensions of dockable window
            Dim windowPos As ESRI.ArcGIS.Framework.IWindowPosition = CType(dockWindow, ESRI.ArcGIS.Framework.IWindowPosition)
            windowPos.Height = 450
            windowPos.Width = 715
            ' Must be floating before calling any IWindowPosition members
            dockWindow.Dock(ESRI.ArcGIS.Framework.esriDockFlags.esriDockFloat)
            windowPos.Move(parentPos.Left + 30, parentPos.Top + 30, windowPos.Width, windowPos.Height)

            dockWindow.Caption = "Publish Map Package (Current AOI --> " & aoiName & ")"
            dockWindowAddIn.UI.InitializeForm(AOIFolderBase + BA_DefaultMapPackageFolder)

        End If
    End Sub

    Public WriteOnly Property selectedProperty As Boolean
        Set(ByVal value As Boolean)
            If value = True Then
                'Check to make sure the elevzone layer exists before enabling map export tool
                'elevzone is generated by Maps dialog And is used as a test to make sure the maps have been properly configured
                Dim folderPath As String = BA_GeodatabasePath(AOIFolderBase, GeodatabaseNames.Analysis, True)
                Dim fullPath As String = folderPath & BA_EnumDescription(MapsFileName.ElevationZone)
                If value = True Then
                    If Not BA_File_Exists(fullPath, WorkspaceType.Geodatabase, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset) Then
                        Me.Enabled = False
                        Exit Property
                    End If
                End If
            End If
            Me.Enabled = value
        End Set
    End Property

    'Protected Overrides Sub OnClick()
    '    'Dim parentPath As String = BA_GetPath(AOIFolderBase, PublicPath.Maps) + "\"
    'Dim parentPath As String = AOIFolderBase + "\" + "maps_export"
    'BA_ExportMapPackageFolder = parentPath

    '    '' Open the output document
    '    Dim outputDocument As PdfDocument = New PdfDocument()
    '    GenerateCharts(parentPath, outputDocument)
    '    'Dim sourceFolder As String = parentPath + "excel"
    '    'ConcatenatePdfFromFolder(sourceFolder, outputDocument)

    '    'Save the document...
    '    'Dim concatFileName As String = parentPath + BA_GetBareName(AOIFolderBase) + ".pdf"
    '    Dim concatFileName As String = parentPath + "\sample_charts.pdf"
    '    outputDocument.Save(concatFileName)
    '    MessageBox.Show("Document saved!")
    'End Sub

    Protected Overrides Sub OnUpdate()

    End Sub

    Private Sub ConcatenatePdfFromFolder(ByVal strFolderPath As String, ByRef outputDocument As PdfDocument)
        Dim arrFiles As String() = System.IO.Directory.GetFiles(strFolderPath)
        For Each strFullPath As String In arrFiles
            'Open the document to import pages from it.
            Dim inputDocument As PdfDocument = PdfReader.Open(strFullPath, PdfDocumentOpenMode.Import)
            'Iterate pages
            Dim count As Int16 = inputDocument.PageCount
            For idx As Int16 = 0 To count - 1
                'Get the page from the external document...
                Dim page As PdfPage = inputDocument.Pages(idx)
                '...And add it to the output document.
                outputDocument.AddPage(page)
            Next
        Next
    End Sub
End Class
