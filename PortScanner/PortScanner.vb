Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class PortScanner
    Public Property host As String
    Private port As Integer
    Private portList As Integer
    Public Property min As Integer
    Public Property max As Integer
    Public Property timeout As Integer
    Public Property Threads As Integer

    Private tcp As TcpClient
    Private count As Integer = 0
    Private turnOff As Boolean = True
    Public Sub New(Optional _host As String = "127.0.0.1",
                   Optional _min As Integer = 1,
                   Optional _max As Integer = 65535,
                   Optional _timeout As Integer = 5,
                   Optional _threads As Integer = 1)
        Me.host = _host
        min = _min
        max = _max
        portList = Nexport()
        timeout = _timeout
        Threads = _threads
    End Sub
    Public Sub start()
        For i As Integer = 0 To Threads - 1
            Dim th As Thread = New Thread(New ThreadStart(AddressOf runscan))
            th.Start()
        Next
    End Sub
    Private Function Nexport() As Integer
        Dim IsNum As Boolean = (max - min >= 0)
        If IsNum Then Return Math.Min(Interlocked.Increment(min), min - 1)
        Return -1
    End Function
    Private Sub runscan()
        While (__Assign(port, Nexport)) <> -1

            count = port
            Thread.Sleep(1)
            Console.Title = "Cổng đang xử lý : " + count.ToString()

            'Tiến hành kiểm tra cổng
            Try
                Connect()
            Catch
                Continue While
            End Try

            'Trả kết quả kiểm trả cổng
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine()
            Console.WriteLine("Cổng TCP " & port & " thì đang mở.")

            'Kiểm tra kiểu kết nối
            Try
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine(BannerGrab(host, port, timeout))
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Không thể kiểm tra kiểu kết nối ::Mã lỗi = " + ex.Message)
                Console.ResetColor()
            End Try

            'Kiểm tra dịch vụ kết nối

            Dim webpageTitle As String = GetPageTitle("http://" & host & ":" + port.ToString())
            If String.IsNullOrWhiteSpace(webpageTitle) = False Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Webpage Title = " & webpageTitle & "Found @ :: " & "http://" + host & ":" + port.ToString())
            Else
                Console.ForegroundColor = ConsoleColor.DarkMagenta
                Console.WriteLine("Có thể có cửa sổ dịch vụ @ :: " + host + ":" + port.ToString())
                Console.ResetColor()
            End If

            Console.ResetColor()
        End While

        'Kết thúc quá trình
        If (turnOff = True) Then
            turnOff = False
            Console.WriteLine()
            Console.WriteLine("Đã quét xong !!!")
            Console.ReadKey()
        End If
    End Sub
    Private Function __Assign(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

    Public Property IsRun As Boolean = True

    Private Function Connect() As TcpClient
        tcp = New TcpClient()
        Dim ar As IAsyncResult = tcp.BeginConnect(host, port, AddressOf AsyncCallback, Nothing)
        IsRun = ar.AsyncWaitHandle.WaitOne(timeout, False)
        If tcp.Connected = False OrElse IsRun = False Then
            Throw New Exception()
        End If
        Return tcp
    End Function
    Private Sub AsyncCallback(ar As IAsyncResult)
        Try
            tcp.EndConnect(ar)
        Catch ex As Exception
            Return
        End Try
        If tcp.Connected AndAlso IsRun Then
            Return
        End If
        tcp.Close()
    End Sub
    Private Function GetPageTitle(ip As String) As String
        Try
            Dim wc As WebClient = New WebClient()
            Dim str As String = wc.DownloadString(ip)
            Dim result As String = Regex.Match(str, "\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups("Title").Value
            Return result
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Public Function BannerGrab(ByVal hostName As String, ByVal port As Integer, ByVal timeout As Integer) As String
        Dim newClient = New TcpClient(hostName, port)
        newClient.SendTimeout = timeout
        newClient.ReceiveTimeout = timeout
        Dim ns As NetworkStream = newClient.GetStream()
        Dim sw As StreamWriter = New StreamWriter(ns)
        sw.Write("HEAD / HTTP/1.1" & vbCrLf & vbCrLf & "Connection: Closernrn")
        sw.Flush()
        Dim bytes As Byte() = New Byte(2047) {}
        Dim bytesRead As Integer = ns.Read(bytes, 0, bytes.Length)
        Dim response As String = Encoding.ASCII.GetString(bytes, 0, bytesRead)
        Return response
    End Function
End Class