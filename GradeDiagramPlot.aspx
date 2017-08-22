<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GradeDiagramPlot.aspx.cs" Inherits="GradeDiagramTest.GradeDiagramPlot" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="./d3-3.5.0/d3.min.js" charset="utf-8"></script>
    <script src="./c3-0.4.15/c3.min.js"></script>
    <script src="./Scripts/jquery-1.10.2.min.js"></script>
    <script src="./Scripts/bootstrap.min.js"></script>
    <script>
        var question;
        var type="pie";
        $(document).ready(function(){
            $(".pie").click(function () {
                type = "pie";
                chart[question].transform("pie");
            });
            $(".donut").click(function () {
                type = "donut";
                chart[question].transform("donut");
            });
        })
    </script>

    <link rel="stylesheet" href="./c3-0.4.15/c3.css" />
    <link rel="stylesheet" href="./Content/bootstrap.min.css" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>

<body>
    <form id="form1" runat="server">

        <%-- Dynamic add javascript for auto type with qusetion change --%>
        <div id="onclick_add" runat="server">

         </div>

        <%-- Dynamic add tab for question --%>
        <ul id="nav_control" class="nav nav-tabs" runat="server">
        </ul>


        <%-- Dynamic add tab-pane table --%>
        <div id="tab_content_control" class="tab-content" runat="server">
        </div>


        <%-- type tab and download --%>
        <ul id="nav_control_button" class="nav nav-tabs" runat="server">
            <li class="pie active"><a data-toggle="tab" href="#" runat="server">Pie Chart</a></li>
            <li class="donut"><a data-toggle="tab" href="#" runat="server">Donut Chart</a></li>
            <li>
                <asp:LinkButton ID="DownLoadToExl" class="btn" OnClick="DownLoadToExl_Click" runat="server">Download Excel</asp:LinkButton></li>
        </ul>


        <%-- Dynamic add tab-pane chart --%>
        <div id="tab_content_chart_control" class="tab-content" runat="server">

        </div>

        <div id="chartPlotJs" runat="server"></div>


    </form>

</body>
</html>




