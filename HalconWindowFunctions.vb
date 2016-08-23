'CountDiceDots.sln
'HalconWindowFunctions.vb

Option Explicit On      'require explicit declaration of variables, this is NOT Python !!
Option Strict On        'restrict implicit data type conversions to only widening conversions

Imports HalconDotNet

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Module HalconWindowFunctions
    
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Function drawContoursOnColorImage(imgOriginal As HImage, contoursToDraw As HXLDCont, red As Double, green As Double, blue As Double, thickness As Integer) As HImage
        Dim regionsToDraw As New List(Of HRegion)
        Dim regionToDraw As New HRegion()
        
        'regionToDraw = contoursToDraw.GenRegionContourXld("filled")
        regionToDraw = contoursToDraw.GenRegionContourXld("margin")
        
        regionsToDraw.Add(regionToDraw)
 
        imgOriginal = drawRegionsOnColorImage(imgOriginal, regionsToDraw, red, green, blue, thickness)
 
        Return imgOriginal
    End Function
 
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Function drawRegionsOnColorImage(imgOriginal As HImage, ByVal regionsToDraw As List(Of HRegion), red As Double, green As Double, blue As Double, thickness As Integer) As HImage
        
        'if there are no regions to draw, just return the input image how it is
        If (regionsToDraw.Count <= 0) Then
            Return imgOriginal
        End If
         
        Dim allRegionsToDraw As New HRegion()
        allRegionsToDraw.GenEmptyRegion()        
        For Each regionToDraw As HRegion In regionsToDraw
            allRegionsToDraw = allRegionsToDraw.ConcatObj(regionToDraw)
        Next
 
        'convert pass pad regions to contours so we can get the borders, then convert back to regions
        Dim allContourMarginsToDraw As New HXLDCont()
        Dim allRegionMarginsToDraw As New HRegion()
 
        allContourMarginsToDraw = allRegionsToDraw.GenContourRegionXld("border")
        allRegionMarginsToDraw = allContourMarginsToDraw.GenRegionContourXld("margin")
         
        'dilate regions to make them thicker so they show better when painted        
        allRegionMarginsToDraw = allRegionMarginsToDraw.DilationRectangle1(thickness, thickness)
        
        Dim imgOrigRedChannelWithRegions As New HImage()
        Dim imgOrigGreenChannelWithRegions As New HImage()
        Dim imgOrigBlueChannelWithRegions As New HImage()
         
        If (imgOriginal.CountChannels.D = 1) Then
            imgOrigRedChannelWithRegions = imgOriginal.PaintRegion(allRegionMarginsToDraw, red, "fill")
            imgOrigGreenChannelWithRegions = imgOriginal.PaintRegion(allRegionMarginsToDraw, green, "fill")
            imgOrigBlueChannelWithRegions = imgOriginal.PaintRegion(allRegionMarginsToDraw, blue, "fill")
        ElseIf(imgOriginal.CountChannels.D = 3) Then
            imgOrigRedChannelWithRegions = imgOriginal.Decompose3(imgOrigGreenChannelWithRegions, imgOrigBlueChannelWithRegions)
            imgOrigRedChannelWithRegions = imgOrigRedChannelWithRegions.PaintRegion(allRegionMarginsToDraw, red, "fill")
            imgOrigGreenChannelWithRegions = imgOrigGreenChannelWithRegions.PaintRegion(allRegionMarginsToDraw, green, "fill")
            imgOrigBlueChannelWithRegions = imgOrigBlueChannelWithRegions.PaintRegion(allRegionMarginsToDraw, blue, "fill")
        End If
         
        Dim imgOrigWithRegions As HImage = imgOrigRedChannelWithRegions.Compose3(imgOrigGreenChannelWithRegions, imgOrigBlueChannelWithRegions)
         
        Return(imgOrigWithRegions)
    End Function
 
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Function drawTextOnColorImage(imgOriginal As HImage, textToWrite As String, fontSize As Integer, fontType As String, useBoldFont As Boolean, useItalicizedFont As Boolean,
                                  topLeftXPos As Integer, topLeftYPos As Integer, red As Double, green As Double, blue As Double) As HImage
         
        'check if input image is a color (3 channel) image, if not, show error message and bail
        Dim numChannelsInOriginal As HTuple = imgOriginal.CountChannels()
        If (numChannelsInOriginal.I <> 3) Then
            MsgBox("error, drawTextOnColorImage was called on an image that is not 3 channel")
            Return Nothing
        End If
         
        'get the original image width and height
        Dim imageWidth As Integer
        Dim imageHeight As Integer
        imgOriginal.GetImageSize(imageWidth, imageHeight)
 
        'create an invisible dummy HWindow to draw text on
        Dim hwDummy As New HWindow(100, 100, imageWidth, imageHeight, 0, "invisible", "")
         
        'set font size and type, and if bold and/or italicized
        setHalconFont(hwDummy, fontSize, fontType, useBoldFont, useItalicizedFont)
         
        'call function to write text on the hidden dummy window, note this writes on a window, NOT on the image, we will do that next . . .
        writeTextOnHalconWindow(hwDummy, textToWrite, "image", topLeftXPos, topLeftYPos, "#FFFFFF", "false")
         
        'dump the window contents onto a dummy image
        Dim imgDummy As HImage = hwDummy.DumpWindowImage()
 
        'threshold the dummy image to HRegions
        Dim regDummy As HRegion = imgDummy.BinaryThreshold("max_separability", "light", New HTuple())
 
        'declare an image for each of the 3 color channels (red, green, and blue)
        Dim imgOrigRedChannelWithRegions As New HImage()
        Dim imgOrigGreenChannelWithRegions As New HImage()
        Dim imgOrigBlueChannelWithRegions As New HImage()
         
        'breakout the original image into 3 color channels
        imgOrigRedChannelWithRegions = imgOriginal.Decompose3(imgOrigGreenChannelWithRegions, imgOrigBlueChannelWithRegions)
 
        'paint the regions onto each of the 3 images per the chosen colors
        imgOrigRedChannelWithRegions = imgOrigRedChannelWithRegions.PaintRegion(regDummy, red, "fill")
        imgOrigGreenChannelWithRegions = imgOrigGreenChannelWithRegions.PaintRegion(regDummy, green, "fill")
        imgOrigBlueChannelWithRegions = imgOrigBlueChannelWithRegions.PaintRegion(regDummy, blue, "fill")
         
        'combine the 3 images
        Dim imgOrigWithRegions As HImage = imgOrigRedChannelWithRegions.Compose3(imgOrigGreenChannelWithRegions, imgOrigBlueChannelWithRegions)
 
        Return imgOrigWithRegions        
    End Function
 
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub showHImage(hImage As HImage, Optional scaleFactor As Double = 1.0)
        Dim imageWidth, imageHeight As Integer
        hImage.GetImageSize(imageWidth, imageHeight)
 
        Dim hWindow As New HWindow(100, 100, CInt(CDbl(imageWidth) * scaleFactor), CInt(CDbl(imageHeight) * scaleFactor), 0, "visible", "")
        'hWindow.SetPart(0, 0, imageWidth - 1, imageWidth - 1)
        hWindow.DispObj(hImage)
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Function showHRegion(hRegion As HRegion, imageWidth As Integer, imageHeight As Integer, Optional thickness As Integer = 1, Optional scaleFactor As Double = 1.0, Optional writeToFileAsBinaryImage As Boolean = False) As HImage
        If (thickness <> 1) Then
            hRegion = hRegion.DilationRectangle1(thickness, thickness)
        End If
        Dim hImage As HImage = hRegion.RegionToBin(255, 0, imageWidth, imageHeight)
        showHImage(hImage, scaleFactor)
        If (writeToFileAsBinaryImage) Then hImage.WriteImage("png", 0, "HRegionAsBinaryImage.png")
        Return hImage           'NOTE: the only purpose of returning the binary image here is to accomidate the showHXLDCont() function
    End Function
    
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub showHXLDCont(hxldCont As HXLDCont, imageWidth As Integer, imageHeight As Integer, Optional thickness As Integer = 1, Optional scaleFactor As Double = 1.0, Optional writeToFileAsBinaryImage As Boolean = False)
        Dim hRegion As HRegion = hxldCont.GenRegionContourXld("margin")
        Dim hImage = showHRegion(hRegion, imageWidth, imageHeight, thickness, scaleFactor, False)
        If (writeToFileAsBinaryImage) Then hImage.WriteImage("png", 0, "HXLDContAsBinaryImage.png")
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Function resizeImageWhileMaintainingAspectRatio(hImage As HImage, intWindowControlWidth As Integer, intWindowControlHeight As Integer) As HImage
         
        Dim imgResized As HImage = Nothing          'this will be the return values
         
        Dim htOriginalImageWidth As HTuple = Nothing        'declare tuples for image width and height
        Dim htOriginalImageHeight As HTuple = Nothing
         
        hImage.GetImageSize(htOriginalImageWidth, htOriginalImageHeight)     'get the image width and height
         
        Dim dblDeltaWidth As Double
        Dim dblDeltaHeight As Double
         
        dblDeltaWidth = CDbl(intWindowControlWidth - htOriginalImageWidth) / CDbl(htOriginalImageWidth)
        dblDeltaHeight = CDbl(intWindowControlHeight - htOriginalImageHeight) / CDbl(htOriginalImageHeight)
         
        Dim dblDelta As Double = Math.Min(dblDeltaWidth, dblDeltaHeight)
         
        Dim intNewImageWidth As Integer
        Dim intNewImageHeight As Integer
         
        intNewImageWidth = htOriginalImageWidth.I + CInt(htOriginalImageWidth.D * dblDelta)
        intNewImageHeight = htOriginalImageHeight.I + CInt(htOriginalImageHeight.D * dblDelta)
         
        imgResized = hImage.ZoomImageSize(intNewImageWidth, intNewImageHeight, "constant")
         
        Return imgResized
    End Function
 
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub writeTextOnHalconWindow(hWindow As HWindow, strTextToWrite As String, strCoordSystem As String, intTopLeftXPos As Integer, intTopLeftYPos As Integer, strColor As String, strBox As String)
         
        If (strCoordSystem = "window") Then
            'add more here later
        ElseIf (strCoordSystem = "image") Then
            'add more here later
        Else
            'should never get here
        End If
         
        hWindow.SetTposition(intTopLeftYPos, intTopLeftXPos)
        hWindow.SetColor(strColor)
        'add strBox here later
        hWindow.WriteString(strTextToWrite)
    End Sub
 
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub setHalconFont(hWindow As HWindow, intFontSize As Integer, strFont As String, blnBold As Boolean, blnSlant As Boolean)
  
        Dim strFinalFontString As String = ""       'this will be the eventual fully build up string to pass into SetFont
          
                                                'the font passed into this function is the Halcon name for a font, ex. 'mono'
        Dim strConvertedFont As String = ""     'this variable holds the actual operating system name for the corresponding font
  
        If (strFont = "mono") Then
            strConvertedFont = "Consolas"
        Else
            'put more conditionals for font options other than mono here later
        End If
          
        strFinalFontString = "-" + strConvertedFont + "-" + intFontSize.ToString() + "-*-"        'add the choice for font and size
  
        If (blnSlant = True) Then                           'add the choice for slant (italicized)
            strFinalFontString = strFinalFontString + "1-*-*-"
        ElseIf (blnSlant = False) Then
            strFinalFontString = strFinalFontString + "0-*-*-"
        Else
            'should never get here
        End If
  
        If (blnBold = True) Then
            strFinalFontString = strFinalFontString + "1-"
        ElseIf (blnBold = False) Then
            strFinalFontString = strFinalFontString + "0-"
        Else
            'should never get here
        End If
          
        hWindow.SetFont(strFinalFontString)
  
    End Sub
 
End Module


