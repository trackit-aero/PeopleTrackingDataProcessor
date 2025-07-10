<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tmrScan = New System.Windows.Forms.Timer(Me.components)
        Me.btnStart = New System.Windows.Forms.Button()
        Me.bgProcess = New System.ComponentModel.BackgroundWorker()
        Me.tmrPost = New System.Windows.Forms.Timer(Me.components)
        Me.bgEmail = New System.ComponentModel.BackgroundWorker()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.lstLocation = New System.Windows.Forms.ListView()
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader12 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader15 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader13 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader14 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.btnClearPostList = New System.Windows.Forms.Button()
        Me.btnRefreshTag = New System.Windows.Forms.Button()
        Me.btnRefreshLoc = New System.Windows.Forms.Button()
        Me.btnRemovePostList = New System.Windows.Forms.Button()
        Me.btnAddPostList = New System.Windows.Forms.Button()
        Me.ListView4 = New System.Windows.Forms.ListView()
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnPost = New System.Windows.Forms.Button()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ColumnHeader16 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabControl1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(365, 45)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "EnTrackPPL Dashboard"
        '
        'tmrScan
        '
        Me.tmrScan.Interval = 5000
        '
        'btnStart
        '
        Me.btnStart.BackColor = System.Drawing.Color.FromArgb(CType(CType(66, Byte), Integer), CType(CType(133, Byte), Integer), CType(CType(244, Byte), Integer))
        Me.btnStart.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStart.ForeColor = System.Drawing.Color.White
        Me.btnStart.Location = New System.Drawing.Point(389, 42)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(91, 30)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = False
        '
        'bgProcess
        '
        '
        'tmrPost
        '
        Me.tmrPost.Interval = 30000
        '
        'bgEmail
        '
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Location = New System.Drawing.Point(19, 78)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1028, 416)
        Me.TabControl1.TabIndex = 8
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.ListView1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 37)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1020, 375)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Tag View"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader9, Me.ColumnHeader16})
        Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(3, 3)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(1014, 369)
        Me.ListView1.TabIndex = 2
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Tag ID"
        Me.ColumnHeader1.Width = 100
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Name"
        Me.ColumnHeader2.Width = 150
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Location"
        Me.ColumnHeader3.Width = 200
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "First Seen"
        Me.ColumnHeader4.Width = 178
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Last Seen"
        Me.ColumnHeader5.Width = 235
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Type"
        Me.ColumnHeader9.Width = 200
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.lstLocation)
        Me.TabPage4.Location = New System.Drawing.Point(4, 37)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(1020, 375)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Tag/Reader"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'lstLocation
        '
        Me.lstLocation.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader10, Me.ColumnHeader11, Me.ColumnHeader12, Me.ColumnHeader15, Me.ColumnHeader13, Me.ColumnHeader14})
        Me.lstLocation.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstLocation.HideSelection = False
        Me.lstLocation.Location = New System.Drawing.Point(3, 3)
        Me.lstLocation.Name = "lstLocation"
        Me.lstLocation.Size = New System.Drawing.Size(1014, 369)
        Me.lstLocation.TabIndex = 3
        Me.lstLocation.UseCompatibleStateImageBehavior = False
        Me.lstLocation.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Tag ID"
        '
        'ColumnHeader11
        '
        Me.ColumnHeader11.Text = "Name"
        '
        'ColumnHeader12
        '
        Me.ColumnHeader12.Text = "Reader MAC"
        '
        'ColumnHeader15
        '
        Me.ColumnHeader15.Text = "Location"
        '
        'ColumnHeader13
        '
        Me.ColumnHeader13.Text = "RSSI"
        '
        'ColumnHeader14
        '
        Me.ColumnHeader14.Text = "Last Seen"
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.btnClearPostList)
        Me.TabPage1.Controls.Add(Me.btnRefreshTag)
        Me.TabPage1.Controls.Add(Me.btnRefreshLoc)
        Me.TabPage1.Controls.Add(Me.btnRemovePostList)
        Me.TabPage1.Controls.Add(Me.btnAddPostList)
        Me.TabPage1.Controls.Add(Me.ListView4)
        Me.TabPage1.Controls.Add(Me.NumericUpDown1)
        Me.TabPage1.Controls.Add(Me.Label5)
        Me.TabPage1.Controls.Add(Me.btnPost)
        Me.TabPage1.Controls.Add(Me.ComboBox2)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.ComboBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 37)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(1020, 375)
        Me.TabPage1.TabIndex = 4
        Me.TabPage1.Text = "Simulate Read"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'btnClearPostList
        '
        Me.btnClearPostList.Location = New System.Drawing.Point(656, 123)
        Me.btnClearPostList.Name = "btnClearPostList"
        Me.btnClearPostList.Size = New System.Drawing.Size(312, 32)
        Me.btnClearPostList.TabIndex = 23
        Me.btnClearPostList.Text = "Clear Post List"
        Me.btnClearPostList.UseVisualStyleBackColor = True
        '
        'btnRefreshTag
        '
        Me.btnRefreshTag.Location = New System.Drawing.Point(656, 59)
        Me.btnRefreshTag.Name = "btnRefreshTag"
        Me.btnRefreshTag.Size = New System.Drawing.Size(312, 32)
        Me.btnRefreshTag.TabIndex = 22
        Me.btnRefreshTag.Text = "Refresh Tags"
        Me.btnRefreshTag.UseVisualStyleBackColor = True
        '
        'btnRefreshLoc
        '
        Me.btnRefreshLoc.Location = New System.Drawing.Point(656, 18)
        Me.btnRefreshLoc.Name = "btnRefreshLoc"
        Me.btnRefreshLoc.Size = New System.Drawing.Size(312, 32)
        Me.btnRefreshLoc.TabIndex = 21
        Me.btnRefreshLoc.Text = "Refresh Locations"
        Me.btnRefreshLoc.UseVisualStyleBackColor = True
        '
        'btnRemovePostList
        '
        Me.btnRemovePostList.Location = New System.Drawing.Point(406, 314)
        Me.btnRemovePostList.Name = "btnRemovePostList"
        Me.btnRemovePostList.Size = New System.Drawing.Size(192, 32)
        Me.btnRemovePostList.TabIndex = 19
        Me.btnRemovePostList.Text = "Remove from Post List"
        Me.btnRemovePostList.UseVisualStyleBackColor = True
        '
        'btnAddPostList
        '
        Me.btnAddPostList.Location = New System.Drawing.Point(406, 123)
        Me.btnAddPostList.Name = "btnAddPostList"
        Me.btnAddPostList.Size = New System.Drawing.Size(192, 32)
        Me.btnAddPostList.TabIndex = 18
        Me.btnAddPostList.Text = "Add Tag to Post List"
        Me.btnAddPostList.UseVisualStyleBackColor = True
        '
        'ListView4
        '
        Me.ListView4.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader8, Me.ColumnHeader6, Me.ColumnHeader7})
        Me.ListView4.HideSelection = False
        Me.ListView4.Location = New System.Drawing.Point(171, 188)
        Me.ListView4.Name = "ListView4"
        Me.ListView4.Size = New System.Drawing.Size(428, 97)
        Me.ListView4.TabIndex = 17
        Me.ListView4.UseCompatibleStateImageBehavior = False
        Me.ListView4.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "Location"
        Me.ColumnHeader8.Width = 141
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Tag Number"
        Me.ColumnHeader6.Width = 201
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "RSSI"
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.Location = New System.Drawing.Point(171, 90)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {30, 0, 0, -2147483648})
        Me.NumericUpDown1.Minimum = New Decimal(New Integer() {120, 0, 0, -2147483648})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(120, 33)
        Me.NumericUpDown1.TabIndex = 16
        Me.NumericUpDown1.Value = New Decimal(New Integer() {30, 0, 0, -2147483648})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(62, 92)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(65, 28)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Power"
        '
        'btnPost
        '
        Me.btnPost.Location = New System.Drawing.Point(171, 123)
        Me.btnPost.Name = "btnPost"
        Me.btnPost.Size = New System.Drawing.Size(229, 32)
        Me.btnPost.TabIndex = 14
        Me.btnPost.Text = "Post Tag"
        Me.btnPost.UseVisualStyleBackColor = True
        '
        'ComboBox2
        '
        Me.ComboBox2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBox2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBox2.Cursor = System.Windows.Forms.Cursors.Default
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(171, 59)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(229, 36)
        Me.ComboBox2.TabIndex = 13
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(62, 62)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(119, 28)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "Tag Number"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(62, 22)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 28)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Location"
        '
        'ComboBox1
        '
        Me.ComboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBox1.Cursor = System.Windows.Forms.Cursors.Default
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(171, 22)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(427, 36)
        Me.ComboBox1.TabIndex = 10
        '
        'btnSend
        '
        Me.btnSend.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnSend.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSend.ForeColor = System.Drawing.Color.White
        Me.btnSend.Location = New System.Drawing.Point(827, 42)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(91, 30)
        Me.btnSend.TabIndex = 15
        Me.btnSend.Text = "Send Email"
        Me.btnSend.UseVisualStyleBackColor = False
        '
        'btnClear
        '
        Me.btnClear.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnClear.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClear.ForeColor = System.Drawing.Color.White
        Me.btnClear.Location = New System.Drawing.Point(730, 42)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(91, 30)
        Me.btnClear.TabIndex = 14
        Me.btnClear.Text = "Clear All"
        Me.btnClear.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Button1.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.White
        Me.Button1.Location = New System.Drawing.Point(935, 42)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(150, 30)
        Me.Button1.TabIndex = 16
        Me.Button1.Text = "Config Details"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'ColumnHeader16
        '
        Me.ColumnHeader16.Text = "Battery(mV)"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(11.0!, 28.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1059, 506)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "frmMain"
        Me.Text = "EnTrackPPL Processor"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tmrScan As System.Windows.Forms.Timer
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents bgProcess As System.ComponentModel.BackgroundWorker
    Friend WithEvents tmrPost As Timer
    Friend WithEvents bgEmail As System.ComponentModel.BackgroundWorker
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents ListView1 As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents TabPage4 As TabPage
    Friend WithEvents lstLocation As ListView
    Friend WithEvents ColumnHeader10 As ColumnHeader
    Friend WithEvents ColumnHeader11 As ColumnHeader
    Friend WithEvents ColumnHeader12 As ColumnHeader
    Friend WithEvents ColumnHeader15 As ColumnHeader
    Friend WithEvents ColumnHeader13 As ColumnHeader
    Friend WithEvents ColumnHeader14 As ColumnHeader
    Friend WithEvents btnSend As Button
    Friend WithEvents btnClear As Button
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents btnRemovePostList As Button
    Friend WithEvents btnAddPostList As Button
    Friend WithEvents ListView4 As ListView
    Friend WithEvents ColumnHeader8 As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents Label5 As Label
    Friend WithEvents btnPost As Button
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Button1 As Button
    Friend WithEvents btnRefreshTag As Button
    Friend WithEvents btnRefreshLoc As Button
    Friend WithEvents btnClearPostList As Button
    Friend WithEvents ColumnHeader16 As ColumnHeader
End Class
