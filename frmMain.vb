'CountDiceDots.sln
'frmMain.vb

Option Explicit On      'require explicit declaration of variables, this is NOT Python !!
Option Strict On        'restrict implicit data type conversions to only widening conversions

Imports HalconDotNet

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class frmMain

    ' member variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Dim hfCamera As HFramegrabber = Nothing


    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub btnGetImageFromFile_Click(sender As Object, e As EventArgs) Handles btnGetImageFromFile.Click
        hWindowControl.Dock = DockStyle.Fill        're-doc the HWindowControl (in case the function was already called once, its necessary to un-dock, see later in the function)
 
        Dim hWindow As HWindow                  'declare an HWindow object, we will connect this to the HWindowControl on the form in a minute . . .
        Dim hfFile As HFramegrabber      'declare our frame grabber
        Dim imgOriginal As HImage = Nothing     'declare our original image
        
        Dim intInitialWindowControlWidth As Integer = hWindowControl.Width      'get the initial HWindowControl width and height
        Dim intInitialWindowControlHeight As Integer = hWindowControl.Height
        
        hWindow = hWindowControl.HalconWindow         'associate HWindow object to HWindowControl object on the form
        
        Dim drChosenFile As DialogResult

        drChosenFile = openFileDialog.ShowDialog()                 'open file dialog
 
        If (drChosenFile <> DialogResult.OK Or openFileDialog.FileName = "") Then    'if user chose Cancel or filename is blank . . .
            lblInfo.Text = "file not chosen"              'show error message on label
            Return
        End If
        
        Try             'try to instiantiate the HFramegrabber object
            hfFile = New HFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", openFileDialog.FileName, "default", 1, -1)
        Catch ex As Exception
            lblInfo.Text = "unable to read image, error: " + ex.Message
            Return
        End Try

        lblInfo.Text = openFileDialog.FileName        'update the info label on the form showing which file was opened
 
        imgOriginal = hfFile.GrabImage()     'grab the frame
        
        processImageAndUpdateGUI(imgOriginal, hWindow, intInitialWindowControlWidth, intInitialWindowControlHeight)
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub btnGetImageFromCamera_Click(sender As Object, e As EventArgs) Handles btnGetImageFromCamera.Click
        hWindowControl.Dock = DockStyle.Fill        're-doc the HWindowControl (in case the function was already called once, its necessary to un-dock, see later in the function)
 
        Dim hWindow As HWindow                  'declare an HWindow object, we will connect this to the HWindowControl on the form in a minute . . .
        'Dim hFramegrabber As HFramegrabber      'declare our frame grabber
        Dim imgOriginal As HImage = Nothing     'declare our original image
        
        Dim intInitialWindowControlWidth As Integer = hWindowControl.Width      'get the initial HWindowControl width and height
        Dim intInitialWindowControlHeight As Integer = hWindowControl.Height
        
        hWindow = hWindowControl.HalconWindow         'associate HWindow object to HWindowControl object on the form

        If (hfCamera Is Nothing) Then
            Try             'try to instiantiate the HFramegrabber object
                'downstairs camera
                hfCamera = New HFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", "000f315baa32_AlliedVisionTechnologies_MakoG503B", 0, -1)

                'upstairs camera
                'hfCamera = New HFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", "000f315baa31_AlliedVisionTechnologies_MakoG503B", 0, -1)
            Catch ex As Exception
                lblInfo.Text = "unable to read from camera, error: " + ex.Message
                hfCamera = Nothing
                Return
            End Try
        End If
        
        Try
            imgOriginal = hfCamera.GrabImage()
        Catch ex As Exception
            lblInfo.Text = "unable to grab frame from camera, error: " + ex.Message
            Return
        End Try
        
        lblInfo.Text = "image grab from camera successful"

        processImageAndUpdateGUI(imgOriginal, hWindow, intInitialWindowControlWidth, intInitialWindowControlHeight)
    End Sub

    
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub processImageAndUpdateGUI(imgOriginal As HImage, hWindow As HWindow, intInitialWindowControlWidth As Integer, intInitialWindowControlHeight As Integer)

        'goal is to get imgOriginal to a color image (3 channels) and imgOrigGrayscale to a grayscale image (1 channel)
        'first get the number of channels in imgOriginal
        Dim imgOrigGrayscale As HImage
        Dim numChannelsInOriginal As Integer = CInt(imgOriginal.CountChannels())
        'if original is grayscale (1 channel)
        If (numChannelsInOriginal = 1) Then
            'copy original image to the grayscale image,
            imgOrigGrayscale = imgOriginal.Clone()
            'then build up color image and assign to imgOrignal
            Dim imgRedChannel As HImage
            Dim imgGreenChannel As HImage
            Dim imgBlueChannel As HImage
            imgRedChannel = imgOriginal
            imgGreenChannel = imgOriginal
            imgBlueChannel = imgOriginal
            imgOriginal = imgRedChannel.Compose3(imgGreenChannel, imgBlueChannel)
        ElseIf (numChannelsInOriginal = 3) Then
            'get grayscale image from original
            imgOrigGrayscale = imgOriginal.Rgb1ToGray()
        Else
            MsgBox("error, num channels was not 1 or 3")
            Return
        End If
        
        'blur
        Dim imgBlurred As HImage = imgOrigGrayscale.GaussFilter(5)

        'threshold to a region
        Dim regThresh As HRegion = imgBlurred.BinaryThreshold("max_separability", "light", New HTuple())

        'erode the thresholded image so the pins on the inside are not touching the inner ring of the pads
        regThresh = regThresh.ErosionRectangle1(3, 3)
        
        'separate out the single region into an array of regions, then convert to contours
        Dim dieRegions As HRegion = regThresh.Connection()

        Dim htNumDotsOnEachDie As HTuple = Nothing          'declare tuple for number of dots on each die

        dieRegions.ConnectAndHoles(htNumDotsOnEachDie)      'find the number of holes, which is the same as the number of dots, in each die region

        Dim dieContours As New HXLDCont                        'declare contours
        Dim dieAndDotContours As New HXLDCont
        Dim dotContours As New HXLDCont

        dieContours = dieRegions.GenContourRegionXld("border")                      'first get just the die contours

        dieAndDotContours = dieRegions.GenContourRegionXld("border_holes")          'next get die and dot contours

                'finally subtract the die contours from the die and dot contours to get just the dot contours
        dotContours = dieAndDotContours.SymmDifferenceClosedContoursXld(dieContours)
        
        imgOriginal = drawContoursOnColorImage(imgOriginal, dieContours, 0.0, 200.0, 0.0)
        
        imgOriginal = drawContoursOnColorImage(imgOriginal, dotContours, 255.0, 0.0, 0.0)
        
        imgOriginal.WriteImage("png", 0, "imgOriginal.png")

        Dim htDieCenterXs As HTuple = Nothing
        Dim htDieCenterYs As HTuple = Nothing
        
        dieRegions.AreaCenter(htDieCenterYs, htDieCenterXs)         'for each die get the center X and Y values
 
        Dim htDieRectX1s As HTuple = Nothing
        Dim htDieRectY1s As HTuple = Nothing
        Dim htDieRectX2s As HTuple = Nothing
        Dim htDieRectY2s As HTuple = Nothing
        
                'get the bounding rect for each die, this function gives the top left point X and Y, and the bottom right point X and Y        
        dieRegions.SmallestRectangle1(htDieRectY1s, htDieRectX1s, htDieRectY2s, htDieRectX2s)

        'NOTE: alternatively, HOperatorSet can be used for Halcon functions like this:
        'HOperatorSet.SmallestRectangle1(dieRegions, htDieRectY1s, htDieRectX1s, htDieRectY2s, htDieRectX2s)
        'however, calling directly from an instantiated Halcon object is preferable due to more specific object types

        Dim textToWrite As String
        
        Dim imageWidth, imageHeight As Integer
        imgOriginal.GetImageSize(imageWidth, imageHeight)

        Dim fontSize As Integer = CInt(imageHeight * 0.06)
        Dim intTopLeftXPos As Integer
        Dim intTopLeftYPos As Integer

        For i As Integer = 0 To htNumDotsOnEachDie.Length - 1
            textToWrite = htNumDotsOnEachDie(i).I.ToString()
            intTopLeftXPos = CInt(htDieCenterXs(i).D + ((htDieRectX2s(i).D - htDieRectX1s(i)).D * 0.65))
            intTopLeftYPos = CInt(htDieCenterYs(i).D)
            imgOriginal = drawTextOnColorImage(imgOriginal, textToWrite, fontSize, "mono", True, False, intTopLeftXPos, intTopLeftYPos, 255.0, 0.0, 0.0)
        Next

        Dim htTotalDotCount As HTuple = Nothing

        htTotalDotCount = htNumDotsOnEachDie.TupleSum()

        textToWrite = "Sum: " + htTotalDotCount.ToString()
        intTopLeftYPos = CInt(imageHeight * 0.015)
        intTopLeftXPos = CInt(imageWidth * 0.015)

        imgOriginal = drawTextOnColorImage(imgOriginal, textToWrite, fontSize, "mono", True, False, intTopLeftXPos, intTopLeftYPos, 0.0, 200.0, 0.0)

        Dim imgResized As HImage = Nothing          'declare a resized image

        'get the resized image, as large as possible to fit the HWindowControl while still maintaining the image aspect ratio
        imgResized = resizeImageWhileMaintainingAspectRatio(imgOriginal, intInitialWindowControlWidth, intInitialWindowControlHeight)
        
        Dim htResizedImageWidth As HTuple = Nothing
        Dim htResizedImageHeight As HTuple = Nothing
 
        imgResized.GetImageSize(htResizedImageWidth, htResizedImageHeight)      'get the resized image width and height

        hWindowControl.Dock = DockStyle.None                    'undock the HWindowControl so we can change the size

        hWindowControl.Width = htResizedImageWidth              'change the HWindowControl size to match the resized image,
        hWindowControl.Height = htResizedImageHeight            'this is necessary b/c if we don't the HWindowControl will skew the image to fill itself entirely
 
        hWindow.SetPart(0, 0, htResizedImageHeight.I - 1, htResizedImageWidth.I - 1)        'set the HWindow object to show the the size of the resized image
        
        imgResized.DispObj(hWindow)                'display the image
    End Sub
    
End Class


