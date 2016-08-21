<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.tableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.btnGetImageFromFile = New System.Windows.Forms.Button()
        Me.btnGetImageFromCamera = New System.Windows.Forms.Button()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.hWindowControl = New HalconDotNet.HWindowControl()
        Me.openFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.tableLayoutPanel.SuspendLayout
        Me.SuspendLayout
        '
        'tableLayoutPanel
        '
        Me.tableLayoutPanel.ColumnCount = 3
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.tableLayoutPanel.Controls.Add(Me.btnGetImageFromFile, 0, 0)
        Me.tableLayoutPanel.Controls.Add(Me.btnGetImageFromCamera, 1, 0)
        Me.tableLayoutPanel.Controls.Add(Me.lblInfo, 2, 0)
        Me.tableLayoutPanel.Controls.Add(Me.hWindowControl, 0, 1)
        Me.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.tableLayoutPanel.Name = "tableLayoutPanel"
        Me.tableLayoutPanel.RowCount = 2
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.tableLayoutPanel.Size = New System.Drawing.Size(1332, 939)
        Me.tableLayoutPanel.TabIndex = 0
        '
        'btnGetImageFromFile
        '
        Me.btnGetImageFromFile.AutoSize = true
        Me.btnGetImageFromFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnGetImageFromFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnGetImageFromFile.Location = New System.Drawing.Point(3, 3)
        Me.btnGetImageFromFile.Name = "btnGetImageFromFile"
        Me.btnGetImageFromFile.Size = New System.Drawing.Size(198, 35)
        Me.btnGetImageFromFile.TabIndex = 0
        Me.btnGetImageFromFile.Text = "Get Image From File"
        Me.btnGetImageFromFile.UseVisualStyleBackColor = true
        '
        'btnGetImageFromCamera
        '
        Me.btnGetImageFromCamera.AutoSize = true
        Me.btnGetImageFromCamera.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnGetImageFromCamera.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnGetImageFromCamera.Location = New System.Drawing.Point(207, 3)
        Me.btnGetImageFromCamera.Name = "btnGetImageFromCamera"
        Me.btnGetImageFromCamera.Size = New System.Drawing.Size(237, 35)
        Me.btnGetImageFromCamera.TabIndex = 1
        Me.btnGetImageFromCamera.Text = "Get Image From Camera"
        Me.btnGetImageFromCamera.UseVisualStyleBackColor = true
        '
        'lblInfo
        '
        Me.lblInfo.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lblInfo.AutoSize = true
        Me.lblInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblInfo.Location = New System.Drawing.Point(450, 8)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(879, 25)
        Me.lblInfo.TabIndex = 2
        Me.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'hWindowControl
        '
        Me.hWindowControl.BackColor = System.Drawing.Color.Black
        Me.hWindowControl.BorderColor = System.Drawing.Color.Black
        Me.tableLayoutPanel.SetColumnSpan(Me.hWindowControl, 3)
        Me.hWindowControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.hWindowControl.ImagePart = New System.Drawing.Rectangle(0, 0, 640, 480)
        Me.hWindowControl.Location = New System.Drawing.Point(3, 44)
        Me.hWindowControl.Name = "hWindowControl"
        Me.hWindowControl.Size = New System.Drawing.Size(1326, 892)
        Me.hWindowControl.TabIndex = 3
        Me.hWindowControl.WindowSize = New System.Drawing.Size(1326, 892)
        '
        'openFileDialog
        '
        Me.openFileDialog.FileName = "OpenFileDialog1"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8!, 16!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1332, 939)
        Me.Controls.Add(Me.tableLayoutPanel)
        Me.Name = "frmMain"
        Me.Text = "Form1"
        Me.tableLayoutPanel.ResumeLayout(false)
        Me.tableLayoutPanel.PerformLayout
        Me.ResumeLayout(false)

End Sub

    Friend WithEvents tableLayoutPanel As TableLayoutPanel
    Friend WithEvents btnGetImageFromFile As Button
    Friend WithEvents btnGetImageFromCamera As Button
    Friend WithEvents lblInfo As Label
    Friend WithEvents hWindowControl As HalconDotNet.HWindowControl
    Friend WithEvents openFileDialog As OpenFileDialog
End Class
