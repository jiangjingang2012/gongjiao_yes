<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.xml" %>
<%@ import Namespace ="system.data"  %>
<%@ Import Namespace="System.Data.OleDB"  %>
<%@ Import Namespace="System.Text.RegularExpressions" %>

<%
     ' Response.Write(Request("echostr"))
     ' Response.End()
    Dim calss1 As Class1 = New Class1
    Dim class2 As Class2 = New Class2
    Dim class3 As Class3 = New Class3

    Try
   
        Dim s As String = New StreamReader(Request.InputStream).ReadToEnd
        s = "<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>" + vbCrLf + "<table>" + vbCrLf + s + vbCrLf + "</table>"

        '    Dim s As String = "<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>" + vbCrLf + "<table>" + vbCrLf + "<xml><ToUserName><![CDATA[gh_30b1dd71c2c8]]></ToUserName>" + vbCrLf + "<FromUserName><![CDATA[ohGTnjgSq3x2ASQo64YBSTBJ2LIc]]></FromUserName>" + vbCrLf + "<CreateTime>1356686131</CreateTime>" + vbCrLf + "<MsgType><![CDATA[text]]></MsgType>" + vbCrLf + "<Content><![CDATA[斗门]]></Content>" + vbCrLf + "</xml>" + vbCrLf + " </table>"
  
        Dim FromUserName As String = calss1.read_xmlcontent(s, "FromUserName")
        Dim ToUserName As String = calss1.read_xmlcontent(s, "ToUserName")
        Dim content1 As String = calss1.read_xmlcontent(s, "Content")
        Dim finally_res As String
        Dim sender_info As String = class3.check_sender_status(FromUserName)
        Dim sender_status As String = sender_info.Split("|")(0)
        Dim sender_status_id As String = sender_info.Split("|")(1)
        Dim sender_status_id1 As String = sender_info.Split("|")(2)
       
        calss1.file_wrilte(FromUserName)
        calss1.file_wrilte(ToUserName)
        'content1 = "@b"
        'Dim riqi = Format(Now, "yyyyMMdd")
        If content1.ToLower = "b" Then
            If sender_status_id1 <> 0 Then
                class3.update_sender_status_id(sender_status_id, sender_status, "0", FromUserName)
                content1 = sender_status_id
                sender_status_id = "0"
            ElseIf sender_status_id1 = 0 Then
                class3.update_sender_status_id("0", sender_status, "0", FromUserName)
                content1 = sender_status
                sender_status_id = "0"
            Else
                content1 = sender_status
                finally_res = "无信息返回！！" + vbCrLf
            End If
        End If
        
        If content1 = "Hello2BizUser" Then

            finally_res = "hi,亲，欢迎关注 serverdiao 微信公众平台，此公众平台现在支持绍兴市公交路线查询和影讯查询功能，以后会陆续加入一些功能,敬请期待！由于上线不久，存在服务不稳定因素，请谅解！" + vbCrLf + vbCrLf + "使用方法:       公交线路查询" + vbCrLf + "直接输入 起始站&终点站" + vbCrLf + "例如: 斗门&绍兴图书馆" + vbCrLf + vbCrLf + "输入 c 获取当前地的影院信息" + vbCrLf + vbCrLf + " 输入 f 获取当前正在热映的影片" + vbCrLf + vbCrLf + "如果你有什么好的建议或着疑问也可留言给我" + vbCrLf + "留言方式：@留言内容" + vbCrLf + vbCrLf + "输入 h 或者 ？ 获取帮助信息！" + vbCrLf + vbCrLf + "还等什么，赶快发送一条指令 试一下吧，哈哈~~~~~~"
            '  Response.Write(calss1.xml_create(finally_res, FromUserName, ToUserName))
        ElseIf content1.ToLower = "h" Or content1.ToLower = "？" Or content1.ToLower = "?" Then
            finally_res = "hi,亲，欢迎使用help指令，本屌将竭尽所能服务大家，此公众平台功能如下：" + vbCrLf + vbCrLf + "公交线路查询：" + vbCrLf + "起始站&终点站" + vbCrLf + "例如: 斗门&绍兴图书馆" + vbCrLf + vbCrLf + "影讯查询：" + vbCrLf + "回复 c 查看当前地影院信息！" + vbCrLf + "回复 f 查看当前热映影片信息！！" + vbCrLf + "每天10点左右更新数据！" + vbCrLf + vbCrLf + "留言:" + vbCrLf + "@+留言内容" + vbCrLf + "如：@hello!" + vbCrLf + vbCrLf + "有没有其他功能了？其实是有的，但就不告诉你了，啦拉拉~~~~~~~"
            Response.Write(calss1.xml_create(finally_res, FromUserName, ToUserName))
        ElseIf content1.Split("&").Length = 2 Or content1.Split("﹠").Length = 2 Or content1.Split("＆").Length = 2 Then
            Dim str1 As String = ""
            Dim str2 As String = ""
            If content1.ToLower.Split("&").Length = 2 Then
                Dim string1 As String() = content1.ToLower.Split("&")
                str1 = string1(0).Trim
                str2 = string1(1).Trim
            ElseIf content1.Split("﹠").Length = 2 Then
                Dim string1 As String() = content1.ToLower.Split("﹠")
                str1 = string1(0).Trim
                str2 = string1(1).Trim
            ElseIf content1.Split("＆").Length = 2 Then
                Dim string1 As String() = content1.ToLower.Split("＆")
                str1 = string1(0).Trim
                str2 = string1(1).Trim
        
            End If

            Dim site_name_src As String
            Dim site_id_src As String
            Dim site_name_des As String
            Dim site_id_des As String
            Dim site_src_res, site_des_res As String
            site_src_res = calss1.site_name_res(str1)
            site_des_res = calss1.site_name_res(str2)
    
            If site_src_res.Split("|").Length = 2 Then
                If site_des_res.Split("|").Length = 2 Then
                    site_name_src = site_src_res.Split("|")(1).Split("%")(0)
                    site_id_src = site_src_res.Split("|")(1).Split("%")(1)
                    site_name_des = site_des_res.Split("|")(1).Split("%")(0)
                    site_id_des = site_des_res.Split("|")(1).Split("%")(1)
        
                    Dim commd_busline_src As String = "select distinct(busline_name) from busline_site_info where current_site=" + site_id_src
                    Dim commd_busline_des As String = "select distinct(busline_name) from busline_site_info where current_site=" + site_id_des
      
                    Dim dataset1 As DataSet = calss1.conndb(commd_busline_src, "busline_site_info")
                    Dim dataset2 As DataSet = calss1.conndb(commd_busline_des, "busline_site_info")
                    Dim counter_busline_src As Integer = dataset1.Tables("busline_site_info").Rows.Count
                    Dim counter_busline_des As Integer = dataset2.Tables("busline_site_info").Rows.Count
                    Dim busline_src_string As String = "top"
                    Dim busline_des_string As String = "top"
                    If counter_busline_src > 0 Then
                        While counter_busline_src > 0
                            busline_src_string = busline_src_string + "|" + dataset1.Tables("busline_site_info").Rows(counter_busline_src - 1)(0)
                            counter_busline_src = counter_busline_src - 1
                        End While
                    End If
    
                    If counter_busline_des > 0 Then
       
                        While counter_busline_des > 0
                            busline_des_string = busline_des_string + "|" + dataset2.Tables("busline_site_info").Rows(counter_busline_des - 1)(0)
                            counter_busline_des = counter_busline_des - 1
                        End While
                    End If
    
                    Dim strings_busline1 As String() = busline_src_string.Split("|")
                    Dim strings_busline2 As String() = busline_des_string.Split("|")
                    Dim busline_res As String = calss1.find_busline(strings_busline1, strings_busline2, site_id_src, site_id_des)
                    Dim mid_res As String
                    If busline_res.Split("|").Length > 1 Then
                        finally_res = calss1.work_strings(busline_res, site_name_src, site_name_des)
                    Else
                        mid_res = calss1.one_change(strings_busline1, strings_busline2, site_name_src, site_name_des, site_id_src, site_id_des)
                        If mid_res.Split("|").Length > 1 Then
                            finally_res = calss1.one_change_work_string(mid_res)
                        Else
                    
                            'finally_res = calss1.last_strings(strings_busline1, strings_busline2, site_id_src, site_name_src, site_id_des, site_name_des)
                            finally_res = calss1.query_two_change(site_name_src, site_name_des)
                            If finally_res = "aa" Then
                        
                                finally_res = calss1.last_strings(strings_busline1, strings_busline2, site_id_src, site_name_src, site_id_des, site_name_des)
                                If finally_res.Split("|")(0) = "top" Then
                                    finally_res = calss1.string_two_change(finally_res)
                        
                                ElseIf finally_res = "aa" Then
                                    finally_res = "没有找到对应的乘车路线！"
                                End If
                            End If
                        End If
                    End If
                ElseIf site_des_res.Split("|").Length = 1 Then
                    finally_res = site_des_res
                End If
            ElseIf site_src_res.Split("|").Length = 1 Then
                If site_des_res.Split("|").Length = 2 Then
                    finally_res = site_src_res + "2"
                ElseIf site_des_res.Split("|").Length = 1 Then
                    finally_res = site_src_res + vbCrLf + site_des_res
                End If
            End If
        ElseIf content1.ToLower = "c" Then
            finally_res = class2.fetch_cilema_info
            class3.update_sender_status_id("0", "c", "0", FromUserName)
        ElseIf content1.ToLower = "f" Then
            finally_res = class2.fetch_film_info
            class3.update_sender_status_id("0", "f", "0", FromUserName)
        ElseIf Regex.IsMatch(content1.ToLower, "^c[0-9]{1,3}$") Then 'Or content1.ToLower Like "c##" Or content1.ToLower Like "c###" Then
            Dim cinema_id = Regex.Replace(content1.ToLower, "c", "")
            finally_res = class2.fetch_specific_cinema_info(cinema_id)
            'Response.Write("kk" + cinema_id + "kk")
        ElseIf Regex.IsMatch(content1.ToLower, "^f[0-9]{1,5}$") Then
            Dim film_id = Regex.Replace(content1.ToLower, "b", "")
            'Response.Write("fff" + film_id + "ffff")
        ElseIf Regex.IsMatch(content1.ToLower, "c[0-9]{1,3}f[0-9]{1,5}$") Then
            Dim cinema_id = Regex.Replace(Regex.Replace(content1.ToLower, "c", ""), "f[0-9]{1,5}", "")
            Dim film_id = Regex.Replace(content1.ToLower, "c[0-9]{1,3}f", "")
            'Response.Write("ddd" + cinema_id + "ddd" + film_id + "dddd")
        ElseIf Regex.IsMatch(content1.ToLower, "f[0-9]{1,5}c[0-9]{1,3}$") Then
            Dim film_id = Regex.Replace(Regex.Replace(content1.ToLower, "f", ""), "c[0-9]{1,3}", "")
            Dim cinema_id = Regex.Replace(content1.ToLower, "f[0-9]{1,3}c", "")
            ' Response.Write("ddd" + cinema_id + "ddd" + film_id + "dddd")
        ElseIf Regex.IsMatch(content1.ToLower, "[0-9]{1,5}") Then
            If sender_status = "c" And sender_status_id = "0" Then
                finally_res = class2.fetch_specific_cinema_info(content1.ToLower)
                class3.update_sender_status_id(content1.ToLower, "c", "0", FromUserName)
            ElseIf sender_status = "c" And sender_status_id <> "0" Then
                finally_res = class2.fetch_specific_film_info(sender_status_id, content1.ToLower)
                class3.update_sender_status_id(sender_status_id, "c", content1.ToLower, FromUserName)
            ElseIf sender_status = "f" And sender_status_id = "0" Then
                finally_res = class2.fetch_specific_film_info1(content1.ToLower)
                class3.update_sender_status_id(content1.ToLower, "f", "0", FromUserName)
            ElseIf sender_status = "f" And sender_status_id <> "0" Then
                finally_res = class2.fetch_specific_film_info(content1.ToLower, sender_status_id)
                class3.update_sender_status_id(sender_status_id, "f", content1.ToLower, FromUserName)
            End If
        ElseIf Regex.IsMatch(content1.ToLower, "^@") Then
            Dim content_in As String = Regex.Replace(content1.ToLower, "@", "")
            Dim res As Integer = class3.insert_content(FromUserName, content_in)
            If res = 1 Then
                finally_res = "留言成功！！"
            ElseIf res = 0 Then
                finally_res = "留言失败！！请重试！！！"
            End If
        Else
            
            finally_res = "亲，我也不知道你输入的什么东东啊！回复 h 看下吧！！"
        End If
        Response.Write(calss1.xml_create(finally_res, FromUserName, ToUserName))
        Response.Write(finally_res + "11@")
        calss1.file_wrilte(finally_res)
        Response.Write(calss1.site_name_res("宋梅桥"))
    Catch
        Response.Write("hello!")
    End Try
        %>
