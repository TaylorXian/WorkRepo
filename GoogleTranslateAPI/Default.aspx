<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GoogleTranslateAPI._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=gbk" />
    <meta name="GENERATOR" content="Microsoft Visual Studio 8.0" />
    <title>Google 翻译</title>
    <link rel="stylesheet" type="text/css" href="./main.css" />
    <script type="text/javascript" src="https://www.google.com/jsapi?key=INSERT-YOUR-KEY"></script>
    <script type="text/javascript" src="./trans.js"></script>

</head>
<body onload="addjq();">
    <div id="container">
        <div id="header">
            <h2>
                Google Translate API Test System</h2>
        </div>
        <div id="main">
            <form id="form1" runat="server">
            <input type="hidden" value="" name="web" id="btnGetRequest" />
            <input type="hidden" value="" name="web" id="btn" />
            <div>zh
            <textarea rows="10" cols="50" id="source">zhongwen.</textarea>
            </div>
            <div>en
            <textarea rows="10" cols="50"></textarea>
            </div>
            <div><span id="result_box">this translate result</span>
            </div>
            <div id="dialog" class="invisible">
                <div style="margin: 16px auto; float: left;">
                    <input type="text" value="" id="txtKey" />=<input type="text" value="" id="txtVal" />
                </div>
                <div style="margin: auto auto 16px auto; clear: left; float: left;">
                    <input type="button" value="Modify" id="btnModify" /><input type="button" value="Close"
                        id="btnClose" />
                </div>
                <span id="lblInfo"></span>
            </div>
            <div class="invisible">
                <input type="button" value="translate" name="web" id="btnGetTable" />
            </div>
            </form>
        </div>
        <hr />
        <div id="footer">
            <span style="text-decoration: underline;">Google 翻译。</span>
        </div>
    </div>
</body>
</html>
