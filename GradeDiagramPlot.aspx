<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GradeDiagramPlot.aspx.cs" Inherits="GradeDiagramTest.GradeDiagramPlot" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<script src="./d3-3.5.0/d3.min.js" charset="utf-8"></script>
<script src="./c3-0.4.15/c3.min.js"></script>
<link href="./c3-0.4.15/c3.css" rel="stylesheet"/>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:table id="Table_div" runat="server"/>
        <asp:Button id="DownLoadToExl" runat="server" Text="Download Excel" OnClick="DownLoadToExl_Click" />
        <div id="chart" runat="server"/>

    </form>
    
    

    
</body>
</html>




