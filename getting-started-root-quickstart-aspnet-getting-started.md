<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
<meta content="history" name="save"/>
<meta content="Innovasys Document! X (http://www.innovasys.com)" name="GENERATOR"/>
<meta name="ResourceType" content="Documentation" />
<meta name="ParentProductId" content="638" />
<meta name="ProductId" content="638" /> 
<title>Creating a New Telerik OpenAccess Web Application</title>
<link href="stylesheets/helpstudio.css" type="text/css" rel="stylesheet">
<link rel="stylesheet" type="text/css" href="stylesheets/customstyles.css">
<link rel="stylesheet" type="text/css" href="stylesheets/hs-boxes.css">
<link rel="stylesheet" type="text/css" href="Stylesheets/hs-expandcollapse.css">
<script src="script/helpstudio.js" type="text/javascript"></script>
<script src="script/hs-expandcollapse.js" type="text/javascript"></script>
<script src="script/hs-enlargeimage.js" type="text/javascript"></script>
</head>
<body id="hsbody">
<div style="DISPLAY: none">
<input class="userDataStyle" id="userDataCache" type="hidden" name="userDataCache"> <input id="hiddenScrollOffset" type="hidden" name="hiddenScrollOffset">
</div>
<img id="collapseImage" style="DISPLAY: none; WIDTH: 0px; HEIGHT: 0px" src="images/collapse.gif" name="collapseImage"> <img id="expandImage" style="DISPLAY: none; WIDTH: 0px; HEIGHT: 0px" src="images/expand.gif" name="expandImage">
<img id="copyImage" style="DISPLAY: none; WIDTH: 0px; HEIGHT: 0px"src="images/copycode.gif" name="copyImage"> <img id="copyHoverImage" style="DISPLAY: none; WIDTH: 0px; HEIGHT: 0px" src="images/copycodeHighlight.gif" name="copyHoverImage">
<div id="pagetop">
<table id="pagetoptable1" width="100%">
<tbody>
<tr id="pagetoptable1row1">
<td align="left"><span id="projecttitle">Telerik OpenAccess ORM</span></td>
<td align="right"><span id="feedbacklink"><a href="mailto:documentation@telerik.com?subject=Documentation Feedback:Telerik OpenAccess ORM-getting-started-root-quickstart-aspnet-getting-started">Send comments</a> on this topic.</span></td>
</tr>
<tr id="pagetoptable1row2">
<td align="left" colspan="2"><span id="pagetitle">Creating a New Telerik OpenAccess Web Application</span></td>
</tr>
<tr id="pagetoptable1row3">
<td colspan="2"><a href="#seealsobookmark">See Also</a></td>
</tr>
</tbody>
</table>
<table width="100%" id="pagetopbreadcrumbs" cellspacing="0" cellpadding="0">
<tr>
<td>Programmer's Guide > <a href="quickstart-overview.html">Quick-Start Scenarios</a> > ASP.NET > <a href="getting-started-root-quickstart-aspnet-overview.html">Web Application</a> > Creating a New Telerik OpenAccess Web Application</td>
</tr>
</table>
</div>
<div class="hspopupbubble" id="hsglossaryitembox">
<p>Glossary Item Box</p>
</div>
<div id="pagebody">
<div id="mainbody">
<DIV style="WIDTH: 280px; FLOAT: right">
<TABLE>
<TBODY>
<TR><TD><H4 align=center>RELATED VIDEOS</H4></TD></TR>
<TR><TD><H4>In this video you will learn how to get started building a web application based on an existing database using OpenAccess ORM reverse mapping. In addition you will learn how to extend the data model to include custom properties, and bind the data model to a RadGrid using the OpenAccessLinqDataSource.<BR><BR><A href="http://tv.telerik.com/watch/orm/creating-web-application-based-on-an-existing-database-using-openaccess-orm?seriesID=1537" target=_blank><IMG alt="Creating a Web Application with Telerik OpenAccess ORM Database First Development" align=middle src="images/1VideoImage.png"></A></H4></TD></TR>
</TBODY>
</TABLE>
</DIV>
<P>In this tutorial, you will create a new ASP.NET Web Application that you can use as a starting point for your application. You are going to use the <A href="developer-guide-integration-oa-templates.html#Telerik_OpenAccess_Web_Application">Telerik OpenAccess Web Application</A>&nbsp;project template. This template will create a new ASP.NET Web Application, and open the <STRONG>OpenAccess New Domain Model Wizard</STRONG>. Once you run through the wizard, the new domain model will be added to the web project.</P>
<H1>Creating&nbsp;a&nbsp;New&nbsp;ASP.NET Application</H1>
<P>In this task, you use an&nbsp;OpenAccess ORM&nbsp;project template in Microsoft Visual Studio to create a new ASP.NET Web Application that you can use as a starting point.</P>
<P><STRONG>To Create the&nbsp;ASP.NET Application:</STRONG></P>
<OL>
<LI>Start Visual Studio, on the <STRONG>File</STRONG> menu select <STRONG>New</STRONG>, and then select <STRONG>Project</STRONG>. 
<LI>In the <STRONG>New Project</STRONG> dialog box, select Visual Basic or Visual C# as the programming language. 
<LI>In the <STRONG>Templates</STRONG> pane, select&nbsp;<A href="developer-guide-integration-oa-templates.html#Telerik_OpenAccess_Web_Application">Telerik OpenAccess Web Application</A>.<BR>
<DIV style="WIDTH: 100%">
<TABLE class=hs-box>
<TBODY>
<TR>
<TD class=hs-box-icon vAlign=top><IMG src="images/hs-caution.gif"></TD>
<TD class=hs-box-content vAlign=top>The <STRONG>Telerik OpenAccess Web Application</STRONG> template is available only in <STRONG>Visual Studio 2010/2012</STRONG> for applications that use <STRONG>.NET version 4/4.5</STRONG>.</TD>
</TR>
</TBODY>
</TABLE>
</DIV>
<LI>Type <STRONG>CarRentWebSite</STRONG> as the name of the project.<BR><BR><IMG border=0 alt="" src="images/1GettingStarted-Root-Quickstart-AspNet-GettingStarted-010.png"></LI>
<LI>Click <STRONG>OK</STRONG>.</LI>
<LI>The <STRONG>ASP.NET Web</STRONG> <STRONG>Application</STRONG> is created.</LI>
<LI>The&nbsp;<A href="developemnt-environment-wizards-dialogs-model-tools-wizard-overview.html">Telerik OpenAccess New Domain Model Wizard</A> appears automatically. In the&nbsp;<A href="developemnt-environment-wizards-dialogs-model-tools-wizard-domainmodel-type.html">Select Domain Model Type</A> screen, select <STRONG>Populate from database</STRONG>. Enter <STRONG>SofiaCarRentalContext</STRONG> for <STRONG>Model name</STRONG>. Click Next to continue.</LI> 
<LI>In the<STRONG>&nbsp;</STRONG><A href="developemnt-environment-wizards-dialogs-model-tools-wizard-connection.html">Setup Database Connection</A> screen, create a data connection to the <EM>SofiaCarRental</EM> database and click Next.</LI>
<LI>In the&nbsp;<A href="developemnt-environment-wizards-dialogs-model-tools-wizard-database-objects.html">Choose Database Items</A> screen, select all tables.</LI>
<LI>Click Finish to generate your model. Persistent classes are generated for all tables from the <EM>SofiaCarRental</EM> database. The wizard adds the <EM>EntitiesModel.rlinq</EM> file to the&nbsp;<EM>CarRentWebSite</EM> project. Also references to the <STRONG>Telerik.OpenAccess.dll</STRONG> and <STRONG>Telerik.OpenAccess.35.Extensions.dll</STRONG> assemblies are added.<BR><BR><IMG border=0 alt="" src="images/1GettingStarted-Root-Quickstart-AspNet-GettingStarted-015.png"><BR><BR>And finally, a new connection string for the <EM>SofiaCarRental</EM> database&nbsp;is added to the <STRONG>Web.config</STRONG> file.</LI>
<DIV id=Syntax_XML class=LanguageSpecific>
<TABLE class=syntaxtable cellSpacing=0 cellPadding=0 width="100%">
<TBODY>
<TR>
<TH>XML</TH>
<TH><SPAN class=hs-onlineonly><SPAN class=copyCode onfocusin=changeCopyCodeIcon(this,true) onmouseover=changeCopyCodeIcon(this,true) onfocusout=changeCopyCodeIcon(this,false) tabIndex=0 onkeypress=CopyCode_CheckKey(this) onmouseout=changeCopyCodeIcon(this,false) onclick=copyCode(this)><IMG class=copyCodeImage name=ccImage align=absMiddle src="images/copycode.gif">Copy Code</SPAN></SPAN></TH></TR>
<TR>
<TD colSpan=2>
<DIV class=colorizedcode><FONT color=blue>&lt;</FONT><FONT color=black>?xml</FONT> <FONT color=red>version</FONT><FONT color=black>=</FONT><FONT color=blue>"1.0"</FONT><FONT color=black>?</FONT><FONT color=blue>&gt;<BR>&lt;</FONT><FONT color=maroon>configuration</FONT><FONT color=blue>&gt;<BR>&nbsp;</FONT><FONT color=blue>&lt;</FONT><FONT color=maroon>connectionStrings</FONT><FONT color=blue>&gt;<BR><BR>&nbsp;&nbsp;&nbsp;</FONT><FONT color=blue>&lt;</FONT><FONT color=maroon>add</FONT> <FONT color=red>name</FONT><FONT color=black>=</FONT><FONT color=blue>"SofiaCarRental21Connection"<BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</FONT><FONT color=red>connectionString</FONT><FONT color=black>=</FONT><FONT color=blue>"data source=.\sqlexpress;initial catalog=SofiaCarRental21;integrated security=True"<BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</FONT><FONT color=red>providerName</FONT><FONT color=black>=</FONT><FONT color=blue>"System.Data.SqlClient"</FONT> <FONT color=blue>/&gt;<BR><BR>&nbsp;</FONT><FONT color=blue>&lt;/</FONT><FONT color=maroon>connectionStrings</FONT><FONT color=blue>&gt;<BR>&lt;/</FONT><FONT color=maroon>configuration</FONT><FONT color=blue>&gt;</FONT></DIV></TD></TR></TBODY></TABLE></DIV>
<LI>Select the <STRONG>Telerik.OpenAccess.dll</STRONG> reference and press F4 to open the Properties pane. Set the <STRONG>Copy Local</STRONG> property to <STRONG>True</STRONG>.</LI> 
<LI><STRONG>Build the Solution</STRONG>. </LI>
</OL>
<P>In this task, you created a new ASP.NET Web Application by using the <A href="developer-guide-integration-oa-templates.html#Telerik_OpenAccess_Web_Application">Telerik OpenAccess Web Application</A> project template. Then, you created a new domain model based on the SofiaCarRental database and added it to the <EM>CarRentWebSite</EM> project. Next, you will start implementing the user interface.</P> 
<a id="seealsobookmark" name="seealsobookmark"></a>
<h1 class="heading"><span class="expandcollapse" tabindex="0"><img id="seealsoToggle" class="toggle" name="toggleSwitch" src="images/collapse.gif"></img>See Also</span></h1>
<div id="seealsoSection" class="section" name="collapseableSection"><a href="getting-started-root-quickstart-aspnet-querying.html">Reading and Displaying Data</a><br /><a href="getting-started-root-quickstart-aspnet-insert.html">Inserting Data</a>
</div>
</div>
<div id="pagefooter"><p>&nbsp;</p><p>&nbsp;</p><hr style="height: 1px" /><p>&copy; 2002-2012 Telerik. All Rights Reserved.</p></div>
</div>
</body>
</html>