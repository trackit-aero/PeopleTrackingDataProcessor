Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml

Public Module mdNotify
    Public Function getResponse(URL As String, body As String, token As String) As String
        Dim origResponse As HttpWebResponse
        Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(URL), HttpWebRequest)
        request.Method = "GET"
        Dim postBytes() As Byte = Encoding.UTF8.GetBytes(body)

        If body.Length > 0 Then
            Dim rtype = request.GetType()
            Dim currentMethod = rtype.GetProperty("CurrentMethod", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(request)

            Dim methodType = currentMethod.GetType()
            methodType.GetField("ContentBodyNotAllowed", BindingFlags.NonPublic Or BindingFlags.Instance).SetValue(currentMethod, False)


            request.ContentType = "application/json; charset=UTF-8"
            request.Accept = "application/json"
            request.ContentLength = postBytes.Length
            Dim requestStream As Stream = request.GetRequestStream()
            requestStream.Write(postBytes, 0, postBytes.Length)
            requestStream.Close()
        Else
            request.Headers.Add("Authorization", "Bearer " & token)
        End If
        Try
            origResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Dim Stream As Stream = origResponse.GetResponseStream()
            Dim sr As New StreamReader(Stream, Encoding.GetEncoding("utf-8"))
            Dim str As String = sr.ReadToEnd()
            Return str
        Catch ex As WebException
            log.Error("ER18:" & ex.Message, ex)
            Return ""
        End Try
    End Function
    Public Function postResponse(URL As String, body As String, token As String) As String
        Dim origResponse As HttpWebResponse
        Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(URL), HttpWebRequest)
        request.Method = "POST"
        Dim postBytes() As Byte = Encoding.UTF8.GetBytes(body)

        If body.Length > 0 Then
            Dim rtype = request.GetType()
            Dim currentMethod = rtype.GetProperty("CurrentMethod", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(request)

            Dim methodType = currentMethod.GetType()
            methodType.GetField("ContentBodyNotAllowed", BindingFlags.NonPublic Or BindingFlags.Instance).SetValue(currentMethod, False)


            request.ContentType = "application/json; charset=UTF-8"
            request.Accept = "application/json"
            request.ContentLength = postBytes.Length
            Dim requestStream As Stream = request.GetRequestStream()
            requestStream.Write(postBytes, 0, postBytes.Length)
            requestStream.Close()
        Else
            request.Headers.Add("Authorization", "Bearer " & token)
        End If
        Try
            origResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Dim Stream As Stream = origResponse.GetResponseStream()
            Dim sr As New StreamReader(Stream, Encoding.GetEncoding("utf-8"))
            Dim str As String = sr.ReadToEnd()
            Return str
        Catch ex As WebException
            log.Error("ER18:" & ex.Message, ex)
            Return ""
        End Try
    End Function
    Public Function JsonToXml(ByVal jsonString As String) As XDocument
        Using stream = New MemoryStream(Encoding.ASCII.GetBytes(jsonString))
            Dim quotas = New XmlDictionaryReaderQuotas()
            Return XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(stream, quotas))
        End Using
    End Function
End Module
