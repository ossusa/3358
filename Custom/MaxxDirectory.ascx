<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MaxxDirectory.ascx.cs" Inherits="SitefinityWebApp.Custom.MaxxDirectory" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI" Assembly="Telerik.Sitefinity" %>
<div class="searchDirectory">
<h4>Member</h4>
<h2>Directory</h2>
<p>Know who you are looking for?</p>
<fieldset id="directorySearch" class="sfsearchBox">
    <!-- <input name="anyDesc" type="text" id="directorySearchCompanyName" class="sfsearchTxt" placeholder="Search Directory"> -->
    <asp:TextBox runat="server" ID="anyName" CssClass="sfsearcTxt" placeholder="Search Directory"/>
	<asp:Button runat="server" ID="directorySearchSubmit" Text="Search" class="sfsearchSubmit"/>
	<!--<input type="submit" name="directorySearchSubmit" value="Search" onclick="return false;" id="directorySearchSubmit" class="sfsearchSubmit">-->
</fieldset>

<p>Need More Options?</p>
<p><a href="/forms/CompanyFormPublicMembers">Advanced Directory Search <span class="icon-bullet"></span></a></p>
</div>