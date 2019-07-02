﻿Public Class BtnSlopeDist
  Inherits ESRI.ArcGIS.Desktop.AddIns.Button

  Public Sub New()
        Me.Enabled = False
  End Sub

  Protected Overrides Sub OnClick()
        Dim Basin_Name As String = ""
        Dim cboSelectedAoi = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        BA_DisplayMap(My.Document, 6, Basin_Name, cboSelectedAoi.getValue, Map_Display_Elevation_in_Meters, _
                                         "SLOPE DISTRIBUTION")
        BAGIS_ClassLibrary.BA_ZoomToAOI(My.Document, AOIFolderBase)

        Dim dockWindowAddIn = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of FrmPublishMapPackage.AddinImpl)(My.ThisAddIn.IDs.FrmPublishMapPackage)
        Dim frmMapPackage As FrmPublishMapPackage = dockWindowAddIn.UI
        frmMapPackage.CurrentMap = BAGIS_ClassLibrary.BA_ExportMapSlopePdf
    End Sub

    Public WriteOnly Property SelectedProperty As Boolean
        Set(ByVal value As Boolean)
            Me.Enabled = value
        End Set
    End Property

  Protected Overrides Sub OnUpdate()

  End Sub
End Class
