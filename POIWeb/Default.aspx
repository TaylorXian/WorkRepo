<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POIWeb._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>POI Json</title>

    <script type="text/javascript" src="http://code.jquery.com/jquery-latest.js"></script>

    <script type="text/javascript">
        function cccclick() {
            jQuery.ajax({
                type: "GET",
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

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" onclick="cccclick();" value="click me" />
    </div>
    </form>
</body>
</html>
