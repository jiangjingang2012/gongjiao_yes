Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Xml
Imports System.Data
Imports System.Data.OleDb

Public Class Class2
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

    Public Function conndb1(ByVal command As String, ByVal tablename As String, ByVal tablename1 As String) As DataSet
        Dim strConn As String = "Provider=MSDAORA;Data Source=poldb;User ID=sximage;Password=sximage;"
        Dim conn As OleDbConnection = New OleDbConnection
        conn.ConnectionString = strConn
        conn.Open()
        Dim dataAdapter As OleDbDataAdapter = New OleDbDataAdapter(command, conn)
        Dim DataSet21 As DataSet = New DataSet
        dataAdapter.Fill(DataSet21, tablename)
        dataAdapter.Fill(DataSet21, tablename1)
        conn.Close()
        Return DataSet21
    End Function

    Public Function fetch_cilema_info() As String
        Dim command As String = "select cinema_id,cinema_name,address,phone  from cinema_info"
        Dim dataset As DataSet = conndb(command, "cinema_info")
        Dim count = dataset.Tables("cinema_info").Rows.Count
        Dim res As String = "你所在的地区有如下电影院："
        For i As Integer = 0 To count - 1
            res = res + vbCrLf + dataset.Tables("cinema_info").Rows(i)(0).ToString + ":" + dataset.Tables("cinema_info").Rows(i)(1).ToString

        Next i
        res = res + vbCrLf + vbCrLf + "直接回复  影院代码 快速获取该影院信息哦!!!" + vbCrLf + "如：回复 1 即刻获取 绍兴万达电影城  的影院信息！"
        dataset.Clear()
        Return res
    End Function

    Public Function fetch_film_info() As String
        Dim command As String = "select film_id,film_name  from film_info where film_status=1 order by film_id desc"
        Dim dataset As DataSet = conndb(command, "film_info")
        Dim count = dataset.Tables("film_info").Rows.Count
        Dim res As String = "当前正在热映的影片有："
        For i As Integer = 0 To count - 1
            res = res + vbCrLf + dataset.Tables("film_info").Rows(i)(0).ToString + ":" + dataset.Tables("film_info").Rows(i)(1).ToString
        Next i
        res = res + vbCrLf + vbCrLf + "回复 影片代码 可快速获取该影片信息哦！！！" + vbCrLf + "如：回复 " + dataset.Tables("film_info").Rows(0)(0).ToString + " 即刻获取 " + dataset.Tables("film_info").Rows(0)(1) + "  的影片信息！"
        ' res = res + vbCrLf + vbCrLf + "另外 直接回复 b 即可返回上一会话！！"
        dataset.Clear()
        Return res
    End Function


    Public Function fetch_specific_cinema_info(ByVal cinema_id As String) As String
        Dim today = Format(Now, "yyyyMMdd")
        Dim command As String = "select cinema_id,cinema_name,address,phone  from cinema_info where cinema_id=" + cinema_id
        Dim command1 As String = "select a.film_id, a.film_name,b.play_time from film_info  a ,(select film_id,count(*) play_time from film_show_time where  cinema_id=" + cinema_id + " and today = '" + today + "'group by film_id ) b where(a.film_id = b.film_id) order by b.play_time desc"
        Dim dataset As DataSet = conndb(command, "cinema_info")
        Dim res As String = ""
        Dim cinema_add = ""
        Dim cinema_phone = ""
        Dim cinema_name = ""
        If dataset.Tables("cinema_info").Rows.Count = 0 Then
            res = "没有找到对应的影院信息！！请核实！！"
        ElseIf dataset.Tables("cinema_info").Rows.Count = 1 Then
            cinema_name = dataset.Tables("cinema_info").Rows(0)(1)
            cinema_add = dataset.Tables("cinema_info").Rows(0)(2)
            cinema_phone = dataset.Tables("cinema_info").Rows(0)(3)
            res = "你输入的影院代码" + cinema_id + "为" + cinema_name + vbCrLf + "影院地址为：" + cinema_add + vbCrLf + "联系电话为：" + cinema_phone
            dataset.Clear()
            dataset = conndb1(command1, "a", "b")
            If dataset.Tables("a").Rows.Count > 0 Then
                Dim count As Integer = dataset.Tables("a").Rows.Count
                res = res + vbCrLf + vbCrLf + "该影院正在热映的电影共有" + count.ToString + "部"
                For i As Integer = 0 To count - 1
                    res = res + vbCrLf + dataset.Tables("a").Rows(i)(0).ToString + ":" + dataset.Tables("a").Rows(i)(1) + "  场次（" + dataset.Tables("a").Rows(i)(2).ToString + "场）"

                Next i

                res = res + vbCrLf + vbCrLf + "回复 影片代码，即可获取该影片在该影院的放映信息！" + vbCrLf + "如：回复" + dataset.Tables("a").Rows(0)(0).ToString + "  就可获取" + dataset.Tables("a").Rows(0)(1) + "影片在" + cinema_name + "的放映信息！！"
                res = res + vbCrLf + vbCrLf + "另外 直接回复 b 即可返回上一会话！！"
            Else
                res = res + vbCrLf + vbCrLf + "暂无数据，数据还没及时更新，请稍后再试！！" + vbCrLf + vbCrLf + "直接回复b 返回上一会话！"
            End If
        End If
        dataset.Clear()
        Return res
    End Function

    Public Function fetch_specific_film_info(ByVal cinema_id As String, ByVal film_id As String) As String
        Dim today As String = Format(Now, "yyyyMMdd")
        Dim command As String = "select film_name from film_info where film_id=" + film_id
        Dim command1 As String = "select play_time,ticket_price from film_show_time where cinema_id =" + cinema_id + " and film_id=" + film_id + " and today ='" + today + "'  order by play_time"
        Dim command2 As String = "select cinema_name from cinema_info where cinema_id=" + cinema_id
        Dim dataset As DataSet = conndb(command2, "cinema_info")
        Dim count1 As Integer = dataset.Tables("cinema_info").Rows.Count
        Dim film_name As String = ""
        Dim cinema_name As String = ""
        Dim res As String = ""
        If count1 = 0 Then
            res = "所指定的影院代码不存在，请重新指定，也可直接回复 c 查看当前的影院信息！！！"
        ElseIf count1 = 1 Then
            cinema_name = dataset.Tables("cinema_info").Rows(0)(0)
            dataset.Clear()
            dataset = conndb(command, "film_info")
            Dim count2 As Integer = dataset.Tables("film_info").Rows.Count
            If count2 = 0 Then
                res = "你所指定的影片代码不存在，请重新指定，也可回复 f 查看当前热映影片！！！"
            ElseIf count2 > 0 Then
                film_name = dataset.Tables("film_info").Rows(0)(0)
                dataset.Clear()
                dataset = conndb(command1, "film_show_time")
                Dim count3 As Integer = dataset.Tables("film_show_time").Rows.Count
                res = cinema_name + "上映的影片" + film_name + "场次如下" + vbCrLf
                If count3 = 0 Then
                    res = res + "无场次！！！"
                ElseIf count3 > 0 Then
                    For i As Integer = 0 To count3 - 1
                        res = res + vbCrLf + dataset.Tables("film_show_time").Rows(i)(0) + "      " + dataset.Tables("film_show_time").Rows(i)(1)
                    Next i
                    res = res + vbCrLf + vbCrLf + "另外 直接回复 b 即可返回上一会话！！"
                End If
                dataset.Clear()
            End If
        End If
        Return res
    End Function

    Public Function fetch_specific_film_info1(ByVal film_id As String) As String
        Dim today As String = Format(Now, "yyyyMMdd")
        Dim command As String = "SELECT film_name,film_direct,film_actor,film_show_time,film_time ,decode(film_status,1,'热映',2,'即将上映',3,'下映') FROM FILM_INFO where film_id=" + film_id
        Dim command1 As String = "select cinema_id,cinema_name from cinema_info where cinema_id in(select cinema_id  from film_show_time  where film_id =" + film_id + " and today='" + today + "' group by cinema_id)"
        Dim dataset As DataSet = conndb(command, "film_info")
        Dim film_name As String = ""
        Dim res As String = ""
        If dataset.Tables("film_info").Rows.Count = 0 Then
            res = "你输入的影片代码 没有找到对应的影片信息，你回复F 查看当前热映的影片！！！"
        ElseIf dataset.Tables("film_info").Rows.Count = 1 Then
            film_name = dataset.Tables("film_info").Rows(0)(0)
            Dim film_direct = dataset.Tables("film_info").Rows(0)(1)
            Dim film_actor = dataset.Tables("film_info").Rows(0)(2)
            Dim film_show_time = dataset.Tables("film_info").Rows(0)(3)
            Dim film_time = dataset.Tables("film_info").Rows(0)(4)
            Dim film_status = dataset.Tables("film_info").Rows(0)(5)
            res = "影片代码<" + film_id.ToString + ">为影片<" + film_name + ">:" + vbCrLf + vbCrLf + "导演：" + film_direct + vbCrLf + "主演：" + film_actor + vbCrLf + "上映时间：" + film_show_time + vbCrLf + "片长：" + film_time + vbCrLf + "影片状态：" + film_status + vbCrLf + vbCrLf
            dataset.Clear()
            dataset = conndb(command1, "cinema_info")
            Dim count As Integer = dataset.Tables("cinema_info").Rows.Count
            If count = 0 Then
                res = res + "当前无电影院该影片放映信息，请稍后再试！！！"
            Else
                res = res + "当前共有" + dataset.Tables("cinema_info").Rows.Count.ToString + "家影院放映该影片！" + vbCrLf + vbCrLf
                For i As Integer = 0 To dataset.Tables("cinema_info").Rows.Count - 1
                    res = res + dataset.Tables("cinema_info").Rows(i)(0).ToString + ":" + dataset.Tables("cinema_info").Rows(i)(1) + vbCrLf
                Next i
                res = res + vbCrLf + "回复影院代码即可获取该影片在该影院的放映信息！！！"

            End If
            res = res + vbCrLf + vbCrLf + "直接回复 b 即可返回上一会话！！"
        End If
        Return res
    End Function

End Class
