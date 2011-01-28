<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POIWeb._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>POI Json</title>

    <script type="text/javascript" src="http://code.jquery.com/jquery-latest.js"></script>

    <script type="text/javascript">
        function cccclick() {
            jQuery.ajax({
                type: "POST",
                url: "PoiServe.asmx/HelloWorld",
                contentType: "application/json",
                success: function(msg) {
                    alert(msg);
                },
                error: function(err) {
                }
            });
        }
    </script>
    <style type="text/css">
        div h2
        {
        	text-align: center;
        }
        ul li
        {
        	list-style-type: decimal;
        }
        ul li p
        {
        	padding: 5px 5px 5px 5px;
        	border: 1px solid gray;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="display: none;">
        <input type="button" onclick="cccclick();" value="click me" />
    </div>
    <div>
        <h2>
            接口使用说明</h2>
        <ul>
            <li>
                <h4>
                    获取签到数</h4>
                <p>
                    请求: <a href="http://220.113.40.164/api/getsign?poiid=123">http://220.113.40.164/api/getsign?poiid=123</a><br />
                    poiid poi的ID
                    <br />
                    应答:
                    <br />
                    成功： { result:"success", signCount:"999" } 失败： { result:"failure", error:"no such
                    user!" }</p>
            </li>
            <li>
                <h4>
                    签到</h4>
                <p>
                    根据poi的ID以及用户ID向该poi进行签到，如果成功则返回签到数，失败返回错误结果<br />
                    请求: <a href="http://220.113.40.164/api/sign?poiid=123&userid=456">http://220.113.40.164/api/sign?poiid=123&userid=456</a><br />
                    poiid poi的ID<br />
                    userid 用户ID
                    <br />
                    应答:
                    <br />
                    成功： { result:"success", signCount:"999" } 失败： { result:"failure", error:"no such
                    user!" }</p>
            </li>
            <li>
                <h4>
                    获取评论列表</h4>
                <p>
                    根据poi的ID以及用户ID获取该poi对应的评论信息列表，需要进行分页，如果成功则返回制定数量的评论，失败返回错误结果
                    <br />
                    请求： <a href="http://220.113.40.164/api/getreview?poiid=123&userid=456&start=0&count=30">
                        http://220.113.40.164/api/getreview?poiid=123&userid=456&start=0&count=30</a>
                    <br />
                    poiid poi的ID<br />
                    userid 用户ID<br />
                    start 记录集的起始位置，用于分页<br />
                    count 制定返回记录的数量，用于分页
                    <br />
                    应答：
                    <br />
                    成功： { result:"success", totalCount:"1000" start:"0"; count:"30"; [
                    <br />
                    { userid:"123", username:"liutao", starlevel:"8", date:"2010-11-01", content:"very
                    good!" },<br />
                    { userid:"123", username:"liutao", starlevel:"8", date:"2010-11-01", content:"very
                    good!" },<br />
                    .........<br />
                    ] }
                    <br />
                    totalCount是该poi的所有评论的数量<br />
                    start是该次请求所对应的start参数，表示记录游标<br />
                    count是盖茨请求所对应的count参数，表示请求记录数<br />
                    []中是一个json数组，里面包含评论内容<br />
                    userid是用户ID<br />
                    username是用户名称<br />
                    starlevel是评分等级 date是日期<br />
                    content是评论内容<br />
                    其它相关信息，可以根据情况继续进行扩展。
                    <br />
                    失败： { result:"failure", error:"no such user!" }</p>
            </li>
            <li>
                <h4>
                    评论</h4>
                <p>
                    根据poi的ID以及用户ID对poi进行评论提交评分及文字评论内容等
                    <br />
                    请求: <a href="http://220.113.40.164/api/review?poiid=123&userid=456&starlevel=8&content=good">
                        http://220.113.40.164/api/review?poiid=123&userid=456&starlevel=8&content=good</a>
                    <br />
                    poiid poi的ID<br />
                    userid 用户ID<br />
                    starlevel 评分等级<br />
                    content 评论内容
                    <br />
                    应答:
                    <br />
                    成功: { result:"success", } 失败: { result:"failure", error:"no such user!" }</p>
            </li>
        </ul>
    </div>
    </form>
</body>
</html>
