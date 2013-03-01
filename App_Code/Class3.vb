Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Xml
Imports System.Data
Imports System.Data.OleDb

Public Class Class3
    Public Function conndb(ByVal command As String, ByVal tablename As String) As DataSet
        Dim strConn As String = "Provider=MSDAORA;Data Source=poldb;User ID=sximage;Password=sximage;"
        Dim conn As OleDbConnection = New OleDbConnection
        conn.ConnectionString = strConn
        conn.Open()
        Dim dataAdapter As OleDbDataAdapter = New OleDbDataAdapter(command, conn)
        Dim DataSet21 As DataSet = New DataSet
        dataAdapter.Fill(DataSet21, tablename)
        conn.Close()
        Return DataSet21
    End Function

    Public Function insert_table(ByVal command As String) As Integer
        Dim strConn As String = "Provider=MSDAORA;Data Source=poldb;User ID=sximage;Password=sximage;"
        Dim conn As OleDbConnection = New OleDbConnection
        conn.ConnectionString = strConn
        conn.Open()

        Try
            Dim comm As OleDbCommand = New OleDbCommand(command, conn)

            comm.ExecuteNonQuery()
            conn.Close()
            Return 1
        Catch

            conn.Close()
            Return 0
        End Try

    End Function

    Public Sub update_sender_status_id(ByVal input1 As String, ByVal input2 As String, ByVal input3 As String, ByVal fromuserid As String)
        Dim command As String = "update sender_status set sender_status_id=" + input1 + ",sender_status_id1=" + input3 + ",sender_status='" + input2 + "',sender_time=sysdate where sender_id='" + fromuserid + "'"
        insert_table(command)
    End Sub

    Public Function check_sender_status(ByVal fromuserid As String) As String
        Dim command As String = "select sender_status,sender_status_id,sender_status_id1 from sender_status where sender_id='" + fromuserid + "'"
        Dim dataset As DataSet = conndb(command, "sender_status")
        Dim res As String = ""
        If dataset.Tables("sender_status").Rows.Count = 0 Then
            Dim insert_command As String = "insert into sender_status(sender_id,sender_status,sender_time) values('" + fromuserid + "','c',sysdate)"
            insert_table(insert_command)
            res = "c|0|0"
        ElseIf dataset.Tables("sender_status").Rows.Count = 1 Then
            res = dataset.Tables("sender_status").Rows(0)(0) + "|" + dataset.Tables("sender_status").Rows(0)(1).ToString + "|" + dataset.Tables("sender_status").Rows(0)(2).ToString

        End If
        Return res
    End Function

    Public Function insert_content(ByVal input As String, ByVal input1 As String) As Integer
        Dim command As String = "insert into message_store values(message_seq.nextval,'" + input + "','" + input1 + "',sysdate,1)"
        Return insert_table(command)
    End Function


End Class
