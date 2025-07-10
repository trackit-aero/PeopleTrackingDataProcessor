Partial Class DB
    Partial Public Class vwActiveTagDataTable
        Private Sub vwActiveTagDataTable_vwActiveTagRowChanging(sender As Object, e As vwActiveTagRowChangeEvent) Handles Me.vwActiveTagRowChanging

        End Sub

    End Class

    Partial Public Class NotificationsDataTable
        Private Sub NotificationsDataTable_NotificationsRowChanging(sender As Object, e As NotificationsRowChangeEvent) Handles Me.NotificationsRowChanging

        End Sub

    End Class

    Partial Public Class vwLogicalDevicesDataTable

    End Class

    Partial Public Class AssetTrackingLogDataTable

    End Class
End Class

Namespace DBTableAdapters
    Partial Public Class vwActiveTaggableAssetsTableAdapter
    End Class

    Partial Public Class BusinessRuleAssetTableAdapter
    End Class

    Partial Public Class vwCurrentTagsTableAdapter
    End Class
End Namespace
