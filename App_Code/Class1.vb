Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Xml
Imports System.Data
Imports System.Data.OleDb

Public Class Class1
    '连接数据库函数
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
    '从XML 中读取键值
    Public Function read_xmlcontent(ByVal s As String, ByVal content As String) As String
        Dim kk As StringReader = New StringReader(s)
        Dim result As String
        Dim xmlread As XmlTextReader = New XmlTextReader(kk)
        Dim R As Boolean = False
        While xmlread.Read()
            If R = False Then
                If xmlread.Name = content Then
                    xmlread.Read()
                    result = xmlread.Value.ToString().Trim()
                    R = True
                End If
            End If

        End While
        xmlread.Close()
        Return result
    End Function

    Public Function site_id(ByVal site_name As String) As String
        Dim command As String = "select site_name,site_id from site_name where site_name = '" + site_name + "'"
        Dim dataset As DataSet = conndb(command, "site_name")
        Dim resule As String
        If dataset.Tables("site_name").Rows.Count = 1 Then
            resule = dataset.Tables("site_name").Rows(0)(1)
        Else
            resule = "无信息！"
        End If
        dataset.Clear()
        Return resule
    End Function

    '获取开始站点到终止站点所需的战数
    '获取开始站点到终止站点所需的战数
    Public Function site_nums(ByVal busline As String, ByVal site_id_beg As String, ByVal site_id_end As String) As Integer
        Dim sql_command As String = "select * from busline_site_info start with current_site=" + site_id_beg + " and busline_name ='" + busline + "' connect by prior next_site= current_site and  busline_name ='" + busline + "'"
        Dim sql_command1 As String = "select * from busline_site_info start with current_site=" + site_id_end + " and busline_name ='" + busline + "' connect by prior next_site= current_site and  busline_name ='" + busline + "'"
        Dim dataset As DataSet = conndb(sql_command, "busline_site_info")
        Dim dataset1 As DataSet = conndb(sql_command1, "busline_site_info")
        Dim count1 As Integer = dataset.Tables("busline_site_info").Rows.Count
        Dim count2 As Integer = dataset1.Tables("busline_site_info").Rows.Count
        'Dim count1 As Integer = 3
        'Dim count2 As Integer = 5
        Dim result As Integer
        If count1 > count2 Then
            result = count1 - count2
        Else
            result = count2 - count1
        End If
        dataset.Clear()
        dataset1.Clear()
        Return result

    End Function

    '找到匹配的公交线路
    Public Function find_busline(ByVal strings1 As String(), ByVal strings2 As String(), ByVal site_beg As String, ByVal site_end As String) As String
        Dim counter_str1 As Integer = strings1.Length
        Dim counter_str2 As Integer = strings2.Length
        Dim result As String = "top" 'counter_str1 + counter_str2
        For n As Integer = 1 To counter_str1 - 1
            For j As Integer = 1 To counter_str2 - 1
                If strings2(j) = strings1(n) Then
                    result = result + "|" + strings2(j) + "%" + site_nums(strings2(j), site_beg, site_end).ToString
                End If
            Next j
        Next n

        Return result
    End Function

    '处理返回线路信息
    Public Function work_strings(ByVal input As String, ByVal site_name_beg As String, ByVal site_name_end As String) As String
        Dim strings As String() = input.Split("|")
        Dim strings_counter As Integer = strings.Length
        Dim result As String = "你可以选择的路线有:"
        If strings_counter > 1 Then
            For n As Integer = 1 To strings_counter - 1

                '  result = result + vbCrLf + "乘坐［" + strings(n).Split("%")(0) + "］从［" + site_name_beg + "］上车，" + "经过" + strings(n).Split("%")(1) + "站，从［" + site_name_end + "］下车"
                result = result + vbCrLf + vbCrLf + "从［" + site_name_beg + "］上车，乘坐［" + strings(n).Split("%")(0) + "/" + strings(n).Split("%")(1) + "站］从［" + site_name_end + "］下车，到达目的地！"
            Next n
        End If
        Return result
    End Function

    '生成所需的语句
    Public Function have_sql(ByVal input1 As String, ByVal input2 As String) As String
        Return "select busline_id,site_name,rela_busline from  relate_busline where busline_id='" + input1 + "' and rela_busline='" + input2 + "'"
    End Function


    '处理一次换乘公交的返回信息
    Public Function string_one_change(ByVal input As String, ByVal site_name_beg As String, ByVal site_name_end As String, ByVal site_id_beg As String, ByVal site_id_end As String) As String
        Dim count_num As Integer = input.Split("|").Length
        Dim string_split As String() = input.Split("|")
        Dim result As String
        Dim site_num1 As Integer
        Dim site_num2 As Integer
        Dim sum_num As Integer
        If count_num > 1 Then
            For n As Integer = 1 To count_num - 1
                site_num1 = site_nums(string_split(n).Split("%")(0), site_id_beg, site_id(string_split(n).Split("%")(1)))
                site_num2 = site_nums(string_split(n).Split("%")(2), site_id(string_split(n).Split("%")(1)), site_id_end)
                sum_num = site_num1 + site_num2
                If n = 1 Then
                    result = sum_num.ToString + "%" + string_split(n).Split("%")(0) + "%" + site_name_beg + "%" + string_split(n).Split("%")(1) + "%" + site_num1.ToString + "%" + string_split(n).Split("%")(2) + "%" + site_name_end + "%" + site_num2.ToString
                Else

                    result = result + "|" + sum_num.ToString + "%" + string_split(n).Split("%")(0) + "%" + site_name_beg + "%" + string_split(n).Split("%")(1) + "%" + site_num1.ToString + "%" + string_split(n).Split("%")(2) + "%" + site_name_end + "%" + site_num2.ToString
                    'result = result + "|" + string_split(n).Split("%")(0) + "$" + string_split(n).Split("%")(1) + "$" + string_split(n).Split("%")(2)

                End If
            Next n

        End If
        Return result
    End Function
    Public Function sort_num(ByVal input As String) As String
        Dim count_num As Integer = input.Split("|").Length
        Dim stringa As String() = input.Split("|")
        Dim tmp As String
        Dim res As String = "top|"
        If count_num > 2 Then
            For n As Integer = 1 To count_num - 1
                For j As Integer = n + 1 To count_num - 1
                    If stringa(n).Split("%")(0) > stringa(j).Split("%")(0) Then
                        tmp = stringa(n)
                        stringa(n) = stringa(j)
                        stringa(j) = tmp

                    End If
                Next j
            Next n
        End If
        If stringa.Length > 6 Then
            For i As Integer = 1 To 5
                If i = 1 Then
                    res = res + stringa(i)
                Else
                    res = res + "|" + stringa(i)
                End If
            Next i
        ElseIf stringa.Length > 2 Then
            For j As Integer = 1 To stringa.Length - 1
                If j = 1 Then
                    res = res + stringa(j)
                Else
                    res = res + "|" + stringa(j)
                End If
            Next j
        ElseIf stringa.Length = 2 Then
            res = res + stringa(1)
        End If
        Return res
    End Function

    Public Function sort_num1(ByVal input As String, ByVal num As Integer) As String
        Dim count_num As Integer = input.Split("|").Length
        Dim stringa As String() = input.Split("|")
        Dim tmp As String
        Dim res As String = "top|"
        If count_num > 3 Then
            For n As Integer = 1 To count_num - 2
                For j As Integer = 1 To count_num - n - 1
                    If stringa(j).Split("%")(0) > stringa(j + 1).Split("%")(0) Then
                        tmp = stringa(j)
                        stringa(j) = stringa(j + 1)
                        stringa(j + 1) = tmp

                    End If
                Next j
            Next n
        End If
        If count_num - 2 > num Then
            For i As Integer = 1 To num
                res = res + stringa(i) + "|"
            Next i
        Else
            For k As Integer = 1 To count_num - 2
                res = res + stringa(k) + "|"
            Next k
        End If
        Return res
    End Function

    '找出一次换乘的公交线路信息
    Public Function one_change(ByVal input1 As String(), ByVal input2 As String(), ByVal site_name_beg As String, ByVal site_name_end As String, ByVal site_id_beg As String, ByVal site_id_end As String) As String
        Dim counter_str1 As Integer = input1.Length
        Dim counter_str2 As Integer = input2.Length
        Dim mid_res As String = "top"
        Dim sort_res As String
        Dim result As String = "top"
        For n As Integer = 1 To counter_str1 - 1
            For j As Integer = 1 To counter_str2 - 1
                Dim command As String = have_sql(input1(n), input2(j))
                Dim dataset As DataSet = conndb(command, "relate_busline")
                Dim count As Integer = dataset.Tables("relate_busline").Rows.Count
                If count > 1 Then

                    mid_res = mid_res + "|" + dataset.Tables("relate_busline").Rows(count - 1)(0)
                    mid_res = mid_res + "%" + dataset.Tables("relate_busline").Rows(count - 1)(1)
                    mid_res = mid_res + "%" + dataset.Tables("relate_busline").Rows(count - 1)(2)


                ElseIf count = 1 Then
                    mid_res = mid_res + "|" + dataset.Tables("relate_busline").Rows(0)(0)
                    mid_res = mid_res + "%" + dataset.Tables("relate_busline").Rows(0)(1)
                    mid_res = mid_res + "%" + dataset.Tables("relate_busline").Rows(0)(2)

                End If
                dataset.Clear()
            Next j
        Next n
        '  sort_res = sort_num(mid_res)
        'result = result + string_one_change(mid_res, site_name_beg, site_name_end, site_id_beg, site_id_end)
        'sort_res = sort_num(result)
        If string_one_change(mid_res, site_name_beg, site_name_end, site_id_beg, site_id_end) = "" Then
            result = result + string_one_change(mid_res, site_name_beg, site_name_end, site_id_beg, site_id_end)
            sort_res = result
        Else
            result = result + "|" + string_one_change(mid_res, site_name_beg, site_name_end, site_id_beg, site_id_end)
            sort_res = sort_num(result)
        End If

        Return sort_res
    End Function

    Public Function one_change_work_string(ByVal input As String) As String
        Dim res = "建议的乘坐线路："
        Dim count As Integer = input.Split("|").Length
        If count > 1 Then
            For n As Integer = 1 To count - 1
                ' res = res + vbCrLf + "乘坐［" + input.Split("|")(n).Split("%")(1) + "］,从［" + input.Split("|")(n).Split("%")(2) + "］上车，途径［" + input.Split("|")(n).Split("%")(4) + "］站，从［" + input.Split("|")(n).Split("%")(3) + "］下车，在乘坐[" + input.Split("|")(n).Split("%")(5) + "],经过［" + input.Split("|")(n).Split("%")(7) + "］站，从［" + input.Split("|")(n).Split("%")(6) + "］下车，一共经过［" + input.Split("|")(n).Split("%")(0) + "］站，到达目的地!"
                res = res + vbCrLf + vbCrLf + n.ToString + ":从［" + input.Split("|")(n).Split("%")(2) + "］上车,乘坐［" + input.Split("|")(n).Split("%")(1) + "/" + input.Split("|")(n).Split("%")(4) + "站］从［" + input.Split("|")(n).Split("%")(3) + "］下车，再乘坐[" + input.Split("|")(n).Split("%")(5) + "/" + input.Split("|")(n).Split("%")(7) + "站］，从［" + input.Split("|")(n).Split("%")(6) + "］下车，一共经过［" + input.Split("|")(n).Split("%")(0) + "］站，到达目的地!"
                If n < count - 1 Then
                    res = res + vbCrLf + "或者"
                End If
            Next n
        End If
        Return res
    End Function

    '获取可中转的公交线路
    Public Function mid_busline(ByVal input1 As String, ByVal input2 As String) As String
        Dim sql As String = "select  distinct(a.rela_busline) from relate_busline a ,relate_busline b where a.rela_busline=b.busline_id and a.busline_id='" + input1 + "' and b.rela_busline='" + input2 + "'"
        Dim dataset As DataSet = conndb(sql, "a")
        Dim count As Integer = dataset.Tables("a").Rows.Count
        Dim res As String = "top"
        If count > 0 Then
            For i As Integer = 1 To count
                res = res + "|" + dataset.Tables("a").Rows(i - 1)(0)
            Next i
        End If
        dataset.Clear()
        Return res
    End Function

    '获取中间转换站的站名和站名id
    Public Function mid_mid_site(ByVal site_beg As String, ByVal site_end As String) As String
        Dim comm As String = "select  b.site_name, b.site_id  from relate_busline a ,site_name b where a.site_name=b.site_name  and a.rela_busline='" + site_end + "' and a.busline_id='" + site_beg + "'"
        Dim dataset As DataSet = conndb(comm, "b")
        Dim count As Integer = dataset.Tables("b").Rows.Count
        Dim res As String = "top"
        If count > 0 Then
            For i As Integer = 1 To count
                res = res + "|" + dataset.Tables("b").Rows(i - 1)(0) + "%" + dataset.Tables("b").Rows(i - 1)(1).ToString

            Next i
        End If
        dataset.Clear()
        Return res
    End Function


    Public Function mid_site(ByVal mid_buslines As String, ByVal busline_beg As String, ByVal busline_end As String, ByVal site_beg As String, ByVal site_beg_name As String, ByVal site_end As String, ByVal site_end_name As String) As String
        Dim count As Integer = mid_buslines.Split("|").Length
        Dim res As String = "top"
        Dim mid_res1, mid_res2 As String
        If count > 1 Then
            For n As Integer = 1 To 1 'count - 1
                mid_res1 = mid_mid_site(busline_beg, mid_buslines.Split("|")(n))
                mid_res2 = mid_mid_site(mid_buslines.Split("|")(n), busline_end)
                ' res = res + "|" + mid_res1 + mid_res2
                If mid_res1.Split("|").Length > 1 And mid_res2.Split("|").Length > 1 Then
                    res = res + "|" + compare_site_num(busline_beg, busline_end, mid_buslines.Split("|")(n), site_beg, site_beg_name, site_end, site_end_name, mid_res1, mid_res2)
                End If
            Next n
            'Else
            '    For j As Integer = 1 To count - 1
            '        mid_res1 = mid_mid_site(busline_beg, mid_buslines.Split("|")(j), conn)
            '        mid_res2 = mid_mid_site(mid_buslines.Split("|")(j), busline_end, conn)
            '        ' res = res + "|" + mid_res1 + mid_res2
            '        res = res + "|" + compare_site_num(busline_beg, busline_end, mid_buslines.Split("|")(j), site_beg, site_beg_name, site_end, site_end_name, mid_res1, mid_res2, conn)
            '        'res = res + "|" + compare_site_num(busline_beg, busline_end, "52路", site_beg, site_beg_name, site_end, site_end_name, mid_res1, mid_res2)
            '    Next j
        End If
        'Return res
        Return res
    End Function
    Public Function compare_site_num(ByVal busline_beg As String, ByVal busline_end As String, ByVal busline_mid As String, ByVal site_beg As String, ByVal site_beg_name As String, ByVal site_end As String, ByVal site_end_name As String, ByVal site_mid1 As String, ByVal site_mid2 As String) As String
        Dim count1 As String = site_mid1.Split("|").Length
        Dim count2 As String = site_mid2.Split("|").Length
        Dim site_num1 As Integer
        Dim site_num2 As Integer
        Dim site_num3 As Integer
        Dim sum_num As Integer
        Dim mid_res As String = "top"
        Dim res As String
        If count1 > 1 Then
            For i As Integer = 1 To 1 'count1 - 1
                If count2 > 1 Then
                    For j As Integer = 1 To 1 'count2 - 1

                        site_num1 = site_nums(busline_beg, site_beg, site_mid1.Split("|")(i).Split("%")(1))
                        site_num2 = site_nums(busline_mid, site_mid1.Split("|")(i).Split("%")(1), site_mid2.Split("|")(j).Split("%")(1))
                        site_num3 = site_nums(busline_end, site_mid2.Split("|")(j).Split("%")(1), site_end)
                        sum_num = site_num1 + site_num2 + site_num3
                        ' If i = 1 Then
                        mid_res = mid_res + "|" + sum_num.ToString + "%" + busline_beg + "%" + site_beg_name + "%" + site_mid1.Split("|")(i).Split("%")(0) + "%" + site_num1.ToString + "%" + busline_mid + "%" + site_mid2.Split("|")(j).Split("%")(0) + "%" + site_num2.ToString + "%" + busline_end + "%" + site_end_name + "%" + site_num3.ToString
                        'Else
                        'mid_res = mid_res + sum_num.ToString + "%" + busline_beg + "%" + site_beg_name + "%" + site_mid1.Split("|")(i).Split("%")(0) + "%" + site_num1.ToString + "%" + busline_mid + "%" + site_mid2.Split("|")(j).Split("%")(0) + "%" + site_num2.ToString + "%" + busline_end + "%" + site_end_name + "%" + site_num3.ToString + "|"
                        'End If
                        '  Dim sort_mid_res = sort_num1(mid_res, 1)
                        '  res = "top|" + sort_mid_res + "|"
                    Next j
                End If
            Next i

        End If
        Dim sore_string As String = sort_num(mid_res)
        If sore_string.Split("|").Length > 1 Then
            res = sore_string.Split("|")(1)
        End If
        Return res
    End Function
    Public Function insert_string(ByVal input As String) As String
        Dim count As Integer = input.Split("|").Length
        Dim strings As String() = input.Split("|")
        Dim count1 As Integer = 0
        Dim sql_string As String
        If count > 2 Then
            For n As Integer = 2 To count - 1
                If strings(n).Split("%").Length = 11 Then

                    sql_string = "insert into busline_two_change(SUM_SITES , BUSLINE_BEG ,SITE_BEG ,SITE_MID1 ,SITE_NUM1 ,BUSLINE_MID ,SITE_MID2 ,SITE_NUM2,BUSLINE_ENG ,SITE_END ,SITE_NUM3) values('" + strings(n).Split("%")(0) + "','" + strings(n).Split("%")(1) + "','" + strings(n).Split("%")(2) + "','" + strings(n).Split("%")(3) + "','" + strings(n).Split("%")(4) + "','" + strings(n).Split("%")(5) + "','" + strings(n).Split("%")(6) + "','" + strings(n).Split("%")(7) + "','" + strings(n).Split("%")(8) + "','" + strings(n).Split("%")(9) + "','" + strings(n).Split("%")(10) + "')"
                    '  insert_table(sql_string)
                    file_wrilte(sql_string + ";")
                    count1 = count1 + 1
                End If
            Next n
        End If
        Return count1.ToString + sql_string
    End Function

    Public Function last_strings(ByVal input As String(), ByVal input1 As String(), ByVal site_id_src As String, ByVal site_Name_src As String, ByVal site_id_des As String, ByVal site_name_des As String) As String
        Dim count1 As Integer = input.Length
        Dim count2 As Integer = input1.Length
        Dim res, res1, res2 As String
        If count1 > 1 Then
            For n As Integer = 1 To count1 - 1
                If count2 > 1 Then
                    For j As Integer = 1 To count2 - 1
                        res1 = mid_busline(input(n), input1(j))
                        If res1.Split("|").Length > 1 Then
                            res2 = mid_site(res1, input(n), input1(j), site_id_src, site_Name_src, site_id_des, site_name_des)
                            'res = insert_string(res2)
                            'res = res + "|" + res2
                        Else
                            res2 = "aa"
                        End If
                    Next j
                Else
                    res2 = "aa"
                End If
            Next n
        Else
            res2 = "aa"
        End If
        Return res2
    End Function

    Public Function site_name_res(ByVal site_name As String) As String
        Dim res, site_name_rel, site_id As String
        Dim command As String = "select site_name,site_id from site_name where site_name = '" + site_name + "'"

        Dim command_like As String = "select site_name,site_id from site_name where site_name like '%" + site_name + "%'"
        Dim dataset1 As DataSet = conndb(command, "site_name")


        If dataset1.Tables("site_name").Rows.Count = 1 Then
            site_name_rel = dataset1.Tables("site_name").Rows(0)(0)
            site_id = dataset1.Tables("site_name").Rows(0)(1)
            dataset1.Clear()
            res = "aa|" + site_name_rel + "%" + site_id
        ElseIf dataset1.Tables("site_name").Rows.Count = 0 Then
            dataset1.Clear()
            dataset1 = conndb(command_like, "site_name")
            If dataset1.Tables("site_name").Rows.Count = 1 Then
                site_name_rel = dataset1.Tables("site_name").Rows(0)(0)
                site_id = dataset1.Tables("site_name").Rows(0)(1)
                dataset1.Clear()
                res = "aa|" + site_name_rel + "%" + site_id
            ElseIf dataset1.Tables("site_name").Rows.Count > 1 Then
                Dim count_src = dataset1.Tables("site_name").Rows.Count
                res = "你输入的站点［" + site_name + "］存在多个结果，是以下几个的其中一个吗？"
                For n As Integer = 0 To count_src - 1
                    res = res + vbCrLf + dataset1.Tables("site_name").Rows(n)(0)
                Next n
                dataset1.Clear()
            ElseIf dataset1.Tables("site_name").Rows.Count = 0 Then
                res = "不存在你所输入的［" + site_name + "］站点名！"
                dataset1.Clear()

            End If
        End If
        Return res
    End Function

    Public Function xml_create(ByVal finally_res As String, ByVal FromUserName As String, ByVal ToUserName As String) As String
        Dim strresponse As String
        strresponse = "<xml>"
        strresponse = strresponse & "<ToUserName><![CDATA[" & FromUserName & "]]></ToUserName>"   'tousername是指接受的微信账号。即前面我们获取到的发送者账号
        strresponse = strresponse & "<FromUserName><![CDATA[" & ToUserName & "]]></FromUserName>" 'FromUserName是发送者账号，即我们的工种平台账号。
        strresponse = strresponse & "<CreateTime>" & Now & "</CreateTime>"  '时间。
        strresponse = strresponse & "<MsgType><![CDATA[text]]></MsgType>"  '发送类型。text是文本型。具体可以参考微信帮助手册
        strresponse = strresponse & "<Content><![CDATA[" & finally_res & "]]></Content>" '发送内容
        strresponse = strresponse & "<FuncFlag>0<FuncFlag>"
        strresponse = strresponse & "</xml>"

        Return strresponse
    End Function


    Public Function file_wrilte(ByVal comtent As String) As String
        Dim file As New System.IO.StreamWriter("c:\test.txt", True)
        file.WriteLine(comtent)
        file.Flush()
        file.Close()
    End Function

    Public Function query_two_change(ByVal site_beg As String, ByVal site_end As String) As String
        Dim comm As String = "select sum_sites,busline_beg,site_beg,site_mid1,site_num1,busline_mid,site_mid2,site_num2,busline_eng,site_end,site_num3  from (select * from busline_two_change where site_beg= '" + site_beg + "' and site_end='" + site_end + "'  order by sum_sites) a where rownum < 4"
        Dim comm1 As String = "select sum_sites,busline_beg,site_beg,site_mid1,site_num1,busline_mid,site_mid2,site_num2,busline_eng,site_end,site_num3  from (select * from busline_two_change where site_beg= '" + site_end + "' and site_end='" + site_beg + "'  order by sum_sites) a where rownum < 4"
        Dim res As String
        Dim dataset As DataSet = conndb(comm, "a")
        Dim count As Integer = dataset.Tables("a").Rows.Count
        If count > 0 Then
            res = "你可以选择的路线有[" + count.ToString + "]种"
            For i As Integer = 1 To count
                res = res + vbCrLf + i.ToString + ":从［" + dataset.Tables("a").Rows(i - 1)(2) + "］上车，乘坐［" + dataset.Tables("a").Rows(i - 1)(1) + "/" + dataset.Tables("a").Rows(i - 1)(4).ToString + "站］，到［" + dataset.Tables("a").Rows(i - 1)(3) + "］下车,再乘坐［" + dataset.Tables("a").Rows(i - 1)(5) + "/" + dataset.Tables("a").Rows(i - 1)(7).ToString + "站］，到［" + dataset.Tables("a").Rows(i - 1)(6) + "］下车，再坐［" + dataset.Tables("a").Rows(i - 1)(8) + "/" + dataset.Tables("a").Rows(i - 1)(7).ToString + "站］到［" + dataset.Tables("a").Rows(i - 1)(9) + "］下车，到达目的地！"
            Next i
        ElseIf count = 0 Then
            dataset.Clear()
            dataset = conndb(comm1, "a")
            count = dataset.Tables("a").Rows.Count
            If count > 0 Then
                res = "你可以选择的路线有[" + count.ToString + "]种"
                For i As Integer = 1 To count
                    res = res + vbCrLf + i.ToString + ":从［" + dataset.Tables("a").Rows(i - 1)(9) + "］上车，乘坐［" + dataset.Tables("a").Rows(i - 1)(8) + "/" + dataset.Tables("a").Rows(i - 1)(10).ToString + "］， 到［" + dataset.Tables("a").Rows(i - 1)(6) + "］下车,再乘坐［" + dataset.Tables("a").Rows(i - 1)(5) + "/" + dataset.Tables("a").Rows(i - 1)(7).ToString + "］，到［" + dataset.Tables("a").Rows(i - 1)(3) + "］下车，再坐［" + dataset.Tables("a").Rows(i - 1)(1) + "/" + dataset.Tables("a").Rows(i - 1)(4).ToString + "］到［" + dataset.Tables("a").Rows(i - 1)(2) + "］下车，到达目的地！"
                Next i
            Else
                res = "aa"
                'res = "没有找到对应的乘车路线！"
            End If

        End If
        Return res
    End Function
    Public Function string_two_change(ByVal input As String) As String
        Dim busline_string As String = input.Split("|")(1)
        Dim res As String
        res = "可乘坐的线路：从[" + busline_string.Split("%")(2) + "]上车，乘坐［" + busline_string.Split("%")(1) + "/" + busline_string.Split("%")(4) + "站］到［" + busline_string.Split("%")(3) + "］下车，在坐［" + busline_string.Split("%")(5) + "/" + busline_string.Split("%")(7) + "站］" + busline_string.Split("%")(6) + "下车，再坐［" + busline_string.Split("%")(8) + "/" + busline_string.Split("%")(10) + "站］到［" + busline_string.Split("%")(9) + "］下车，到达目的地！"
        Return res
    End Function


End Class
