<%@ Control %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sitefinity" Namespace="Telerik.Sitefinity.Web.UI" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sfFields" Namespace="Telerik.Sitefinity.Web.UI.Fields" %>

<sitefinity:ResourceLinks ID="resourcesLinks" runat="server">
    <sitefinity:ResourceFile Name="Styles/Ajax.css" />
</sitefinity:ResourceLinks>
<div id="designerLayoutRoot" class="sfContentViews sfSingleContentView" style="max-height: 400px; overflow: auto;">
    <ol>
        <li class="sfFormCtrl">
            <asp:Label runat="server" AssociatedControlID="ItemsPerPage" CssClass="sfTxtLbl">Items Per Page</asp:Label>
            <asp:TextBox ID="ItemsPerPage" runat="server" CssClass="sfTxt" />
        </li>

        <li class="sfFormCtrl">
            <asp:Label runat="server" AssociatedControlID="Limit" CssClass="sfTxtLbl">Limit</asp:Label>
            <asp:TextBox ID="Limit" runat="server" CssClass="sfTxt" />
        </li>

        <li class="sfFormCtrl">
            <asp:Label runat="server" AssociatedControlID="ViewTemplate" CssClass="sfTxtLbl">View Template</asp:Label>
            <asp:DropDownList ID="ViewTemplate" runat="server" />
        </li>
        <li class="sfFormCtrl providers">
            <asp:Label runat="server" CssClass="sfTxtLbl">Providers</asp:Label>
            <asp:CheckBoxList runat="server" ID="Providers" RepeatLayout="Flow" RepeatDirection="Vertical" CssClass="sfCheckBox"/>

        </li>

        <li class="sfFormCtrl">
            <asp:Label ID="Label2" runat="server" AssociatedControlID="Limit" CssClass="sfTxtLbl">Categories</asp:Label>
            <sitefinity:HierarchicalTaxonField ID="CategoriesSelector" runat="server" DisplayMode="Write" Expanded="false" ExpandText="Click To Select Categories"
                ShowDoneSelectingButton="true" AllowMultipleSelection="true" BindOnServer="false" TaxonomyMetafieldName="Category"
                WebServiceUrl="~/Sitefinity/Services/Taxonomies/HierarchicalTaxon.svc" />
        </li>

    </ol>
</div>
