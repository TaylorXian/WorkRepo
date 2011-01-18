<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GoogleTranslateAPI._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!--DOCTYPE HTML-->
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv='content-type' content='text/html; charset=gbk' />
    <meta name='GENERATOR' content='Microsoft Visual Studio 8.0' />
    <title>My name is gSoap Server Test.</title>
    <link rel='stylesheet' type='text/css' href='./main.css' />

    <script type='text/javascript' src='http://jquery.com/jquery-latest.js'></script>

    <script type='text/javascript'></script>

</head>
<body>
    <div id='container'>
        <div id='header'>
            <h2>
                WinCE Web Service ConfigFile Management</h2>
        </div>
        <div id='main'>
            <form id="form1" runat="server">
            <div id='dialog' class='invisible'>
                <div style='margin: 16px auto; float: left;'>
                    <input type='text' value='' id='txtKey' />=<input type='text' value='' id='txtVal' />
                </div>
                <div style='margin: auto auto 16px auto; clear: left; float: left;'>
                    <input type='button' value='Modify' id='btnModify' /><input type='button' value='Close'
                        id='btnClose' />
                </div>
                <span id='lblInfo'></span>
            </div>
            <input type='hidden' value='click me get soap data' name='web' id='btnGetRequestSoapData' />
            <input type='hidden' value='click me call web service' name='web' id='btn' />
            <div class='line'>
                <table>
                    <tr>
                        <td>
                            <span>attrname</span>
                        </td>
                        <td>
                            <input type='text' />
                        </td>
                    </tr>
                </table>
            </div>
            <div class='invisible'>
                <input type='button' value='click me get table' name='web' id='btnGetTable' />
            </div>
            </form>
        </div>
        <hr />
        <div id='footer'>
            <span style='text-decoration: underline;'>Email : taylorsaltfish@hotmail.com 提供技术支持。</span>
        </div>
    </div>
</body>
</html>
