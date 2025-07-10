
Imports System.Collections.Generic
Imports System.Text
Imports System.Net.Mail
Imports System.Web
Imports System.Configuration

Public Class MailUtils

    Public Shared EMAIL_HOST As String
    Public Shared EMAIL_PORT As Integer
    Public Shared EMAIL_FROM_ADDRESS As String
    Public Shared EMAIL_USER_NAME As String
    Public Shared EMAIL_PASSWORD As String
    Public Shared Sub InitializeEmailParameters()
        EMAIL_HOST = My.MySettings.Default.MailServer
        EMAIL_PORT = My.MySettings.Default.MailPort
        EMAIL_FROM_ADDRESS = My.MySettings.Default.MailUser
        EMAIL_USER_NAME = My.MySettings.Default.MailUser
        EMAIL_PASSWORD = My.MySettings.Default.MailPassword
    End Sub
    Public Shared Function SendEmail(toEmailAddresses As String(), subject As String, body As String) As Boolean
        If [String].IsNullOrEmpty(EMAIL_HOST) Then
            InitializeEmailParameters()
        End If
        Return SendEmail(EMAIL_FROM_ADDRESS, toEmailAddresses, subject, body, EMAIL_HOST, EMAIL_PORT, EMAIL_USER_NAME, EMAIL_PASSWORD)
    End Function
    Public Shared Function SendEmail(toEmailAddress As String, subject As String, body As String) As Boolean
        Dim toEmailAddresses As String() = New String(0) {}
        toEmailAddresses(0) = toEmailAddress

        Return SendEmail(toEmailAddresses, subject, body)
    End Function
    Public Shared Function SendEmail(fromEmailAddress As String, toEmailAddresses As String(), subject As String, Body As String, hostName As String, port As Integer,
        userName As String, password As String) As Boolean
        Try
            Dim message As New MailMessage()
            message.From = New MailAddress(fromEmailAddress)
            For Each toEMailAddress As String In toEmailAddresses
                message.[To].Add(toEMailAddress)
            Next
            message.Subject = subject
            message.IsBodyHtml = True
            message.Body = Body

            Dim smtpClient As New SmtpClient()
            smtpClient.UseDefaultCredentials = True

            smtpClient.Host = hostName
            smtpClient.Port = port
            smtpClient.EnableSsl = True
            smtpClient.Credentials = New System.Net.NetworkCredential(userName, password)
            smtpClient.Send(message)
            Return True
            'silently kill errors related to send emails without effecting the business transaction
        Catch ex As Exception
            WriteErrorLog(ex)
            Return False
        End Try
    End Function
    Public Shared Sub WriteErrorLog(ex As Exception)
        Dim sw As IO.StreamWriter = Nothing
        Try
            sw = New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\Email_LogFile.txt", True)
            If Not (ex.InnerException Is Nothing) Then
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.InnerException.ToString().Trim())
            Else
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim())
            End If
            sw.Flush()
            sw.Close()
        Catch
        End Try
    End Sub
    Public Shared Sub WriteErrorLog(Message As String)
        Dim sw As IO.StreamWriter = Nothing
        Try
            sw = New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\Email_LogFile.txt", True)
            sw.WriteLine(Convert.ToString(DateTime.Now.ToString() + ": ") & Message)
            sw.Flush()
            sw.Close()
        Catch
        End Try
    End Sub
End Class