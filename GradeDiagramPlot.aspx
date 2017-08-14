<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GradeDiagramPlot.aspx.cs" Inherits="GradeDiagramTest.GradeDiagramPlot" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<script src="./d3-3.5.0/d3.min.js" charset="utf-8"></script>
<script src="./c3-0.4.15/c3.min.js"></script>  
<script src="./Scripts/jquery-1.10.2.min.js"></script>
<script src="./Scripts/bootstrap.min.js"></script>

<link rel="stylesheet" href="./c3-0.4.15/c3.css"/> 
<link rel="stylesheet" href="./Content/bootstrap.min.css"/>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

        <ul id="nav_control" class="nav nav-tabs" runat="server">

        </ul>


        <div id="tab_content_control" class="tab-content" runat="server">


            <%--<div id="home" class="tab-pane fade in active">
                <asp:table id="Table_div" runat="server"/>
                
            </div>

            <div id="menu1" class="tab-pane fade">
                <asp:table id="Table_div_test" runat="server"/>
            </div>--%>
            
        </div>

        <div id="chart" runat="server"/>
       
         <ul id="nav_control_button" class="nav nav-tabs" runat="server">
             <li ><asp:LinkButton  data-toggle="tab" class="btn" onClick="DownLoadToExl_Click" runat="server">Pie Chart</asp:LinkButton></li>
             <li ><a  data-toggle="tab" class="btn" href="#donut"  runat="server">Donut Chart</a></li>
             <li><asp:LinkButton id="DownLoadToExl" class="btn" onClick="DownLoadToExl_Click" runat="server">Download Excel</asp:LinkButton></li>
         </ul>

      
        <div id="chartPlotJs" runat="server"/>
    

    </form>
    
  <%--  <div class="container">
  <h2>Dynamic Tabs</h2>
  <p>To make the tabs toggleable, add the data-toggle="tab" attribute to each link. Then add a .tab-pane class with a unique ID for every tab and wrap them inside a div element with class .tab-content.</p>

  <ul class="nav nav-tabs">
    <li class="active"><a data-toggle="tab" href="#home">Home</a></li>
    <li><a data-toggle="tab" href="#menu1">Menu 1</a></li>
    <li><a data-toggle="tab" href="#menu2">Menu 2</a></li>
    <li><a data-toggle="tab" href="#menu3">Menu 3</a></li>
  </ul>

  <div class="tab-content">
    <div id="home" class="tab-pane fade in active">
      
    </div>

    <div id="menu1" class="tab-pane fade">

    </div>

    <div id="menu2" class="tab-pane fade">
      <h3>Menu 2</h3>
      <p>Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam.</p>
    </div>
    <div id="menu3" class="tab-pane fade">
      <h3>Menu 3</h3>
      <p>Eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.</p>
    </div>
  </div>
</div>--%>

    
</body>
</html>




