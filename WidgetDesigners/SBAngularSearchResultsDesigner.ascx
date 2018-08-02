<%@ Control %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sitefinity" Namespace="Telerik.Sitefinity.Web.UI" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sfFields" Namespace="Telerik.Sitefinity.Web.UI.Fields" %>

<sitefinity:ResourceLinks ID="resourcesLinks" runat="server">
    <sitefinity:ResourceFile Name="Styles/Ajax.css" />
</sitefinity:ResourceLinks>
<div id="designerLayoutRoot" class="sfContentViews sfSingleContentView" style="max-height: 400px; overflow: auto; ">
<ol>        
    
    <li class="sfFormCtrl">
    <asp:Label runat="server" AssociatedControlID="SearchBloxServerIp" CssClass="sfTxtLbl">SearchBlox Server Address</asp:Label>
    <asp:TextBox ID="SearchBloxServerIp" runat="server" CssClass="sfTxt" placeholder="e.g.: http://10.236.57.201/"/>
    <div class="sfExample">The internet address where the SearchBlox collection resides.</div>
    </li>

    <li class="sfFormCtrl">
    <asp:Label runat="server" AssociatedControlID="CollectionId" CssClass="sfTxtLbl">Collection IDs</asp:Label>
    <asp:TextBox ID="CollectionId" runat="server" CssClass="sfTxt" placeholder="e.g.: 1, 2, 8"/>
    <div class="sfExample">A list of numbers indicating which collections to search through, separated by commas.</div>
    </li>
    
    <li class="sfFormCtrl">
    <asp:Label runat="server" AssociatedControlID="FacetNames" CssClass="sfTxtLbl">Facet Names</asp:Label>
        <asp:TextBox ID="FacetNames" runat="server" CssClass="sfTxt" rows="5" TextMode="MultiLine" placeholder="e.g.: contenttype = File Type"/>
    <div class="sfExample">A list of categories that can be applied to a search, separated by commas or line breaks. If you want to display a different name than the default, put a '=' after it and type the name you want to use.</div>
    </li>
    
    <li class="sfFormCtrl">
        <asp:Label runat="server" AssociatedControlID="DateRangeName" CssClass="sfTxtLbl">Name of Date Range Facet</asp:Label>
        <asp:TextBox ID="DateRangeName" runat="server" CssClass="sfTxt" placeholder="e.g.: Date Range"/>
        <asp:Panel ID="Panel1" class="sfExample" runat="server">The name to use for the facet that filters by date ranges. If left empty, facet will not be displayed.</asp:Panel>
    </li>
    
    <li class="sfFormCtrl">
    <asp:CheckBox runat="server" ID="ShowSearchBox" Text="Show Search Box" CssClass="sfCheckBox"/>        
    <div class="sfExample">Check to display the search box next to the search results.</div>
    </li>
    
</ol>
</div>
