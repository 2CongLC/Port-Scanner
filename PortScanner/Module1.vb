Imports System
Imports System.Text
Module Module1

    Sub Main(args As String())
        Dim p As New PortScanner()
        Dim cmd As String

        'Sử dụng Unikey : Bảng mã Unicode dựng sẳn, kiểu gõ Telex để viết có dấu.
        Console.OutputEncoding = Encoding.UTF8

        Message("Điền địa chỉ IP hoặc Domain ví dụ (127.0.0.1) hoặc (localhost) ...", ConsoleColor.Yellow)
        Console.Write("Nhập IP hoặc Domain : ")
        cmd = Console.ReadLine()
        p.host = cmd

InMinPort:
        Message("Cổng quét đầu tiên có giá trị nhỏ nhất là 1", ConsoleColor.Yellow)
        Console.Write("Nhập Min Port : ")
        cmd = GetNum(Console.ReadLine())
        If cmd <> -1 Then p.min = cmd Else Message("Số nhập vào không đúng, nhập lại số !!!", ConsoleColor.DarkRed) : GoTo InMinPort
InMaxPort:
        Message("Cổng quét cuối cùng có giá trị lớn nhất là 65535", ConsoleColor.Yellow)
        Console.Write("Nhập Max Port : ")
        cmd = GetNum(Console.ReadLine())
        If cmd <> -1 Then p.max = cmd Else Message("Số nhập vào không đúng, nhập lại số !!!", ConsoleColor.DarkRed) : GoTo InMaxPort
InThreads:
        Message("Số lượng cổng cho mỗi lần chạy luồng. Giới hạn từ 1~50, giá trị càng thấp thì càng chính xác.", ConsoleColor.Yellow)
        Console.Write("Nhập số lượng : ")
        cmd = GetNum(Console.ReadLine())
        If cmd <> -1 Then p.Threads = cmd Else Message("Số nhập vào không đúng, nhập lại số !!!", ConsoleColor.DarkRed) : GoTo InThreads
InTimeout:
        Message("Thời gian chờ kết quả phản hồi đối với mỗi kết nối. Giới hạn từ 1~10s, giá trị càng cao thì càng tốt.", ConsoleColor.Yellow)
        Console.Write("Nhập thời gian : ")
        cmd = GetNum(Console.ReadLine())
        If cmd <> -1 Then p.timeout = cmd * 1000 Else Message("Số nhập vào không đúng, nhập lại số !!!", ConsoleColor.DarkRed) : GoTo InTimeout

        Console.WriteLine()
        p.start()


    End Sub

    Private Sub Message(str As String, cl As ConsoleColor)
        Console.ForegroundColor = cl
        Console.WriteLine(str)
        Console.ResetColor()
    End Sub

    Private Function GetNum(str As String) As Integer
        Dim num As Integer
        Dim IsNum As Boolean = Integer.TryParse(str, num)
        If num Then Return Integer.Parse(str)
        Return -1
    End Function
End Module
