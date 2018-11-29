﻿Public Class BtnElevationSNOTEL
  Inherits ESRI.ArcGIS.Desktop.AddIns.Button

  Public Sub New()
        Me.Enabled = False
    End Sub
    Public WriteOnly Property SelectedProperty As Boolean
        Set(ByVal value As Boolean)
            Me.Enabled = value
        End Set
    End Property
  Protected Overrides Sub OnClick()
        Dim Basin_Name As String = ""
        Dim cboSelectedAoi = ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID(Of cboTargetedAOI)(My.ThisAddIn.IDs.cboTargetedAOI)
        BA_DisplayMap(My.Document, 2, Basin_Name, cboSelectedAoi.getValue, Map_Display_Elevation_in_Meters, _
                                         "ELEVATION - SNOTEL SITES")
        BAGIS_ClassLibrary.BA_ZoomToAOI(My.Document, AOIFolderBase)
  End Sub

  Protected Overrides Sub OnUpdate()

  End Sub
End Class
