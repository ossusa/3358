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
    <asp:Label runat="server" AssociatedControlID="Message" CssClass="sfTxtLbl">Message</asp:Label>
    <asp:TextBox ID="Message" runat="server" CssClass="sfTxt" />
    <div class="sfExample">The message to be displayed</div>
    </li>
    <li class="sfFormCtrl">
    <asp:Label runat="server" AssociatedControlID="ViewTemplate" CssClass="sfTxtLbl">View Template</asp:Label>
    <asp:TextBox ID="ViewTemplate" runat="server" CssClass="sfTxt" />
    <div class="sfExample">Set View Template</div>
    </li>
    <li class="sfFormCtrl">
        <label for="LoginRequired">Require login?</label>
        <asp:CheckBox runat="server" ID="LoginRequired" />
    </li>
</ol>
</div>
