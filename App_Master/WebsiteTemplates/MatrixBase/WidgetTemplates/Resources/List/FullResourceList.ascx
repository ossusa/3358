<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FullResourceList.ascx.cs" Inherits="SitefinityWebApp.App_Data.Sitefinity.WebsiteTemplates.MatrixBase.WidgetTemplates.Resources.List.FullResourceList" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.DynamicModules.Web.UI.Frontend" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.Fields" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI" TagPrefix="sf" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI.ContentUI" Assembly="Telerik.Sitefinity, Version=8.1.5820.0, Culture=neutral, PublicKeyToken=b28c218413bdf563" %>
<p>MG:</p>
<sf:SitefinityLabel id="title" runat="server" WrapperTagName="div" HideIfNoText="true" HideIfNoTextMode="Server" CssClass="sfitemFieldLbl" />
<telerik:RadListView ID="dynamicContentListView" ItemPlaceholderID="ItemsContainer" runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false">
    <LayoutTemplate>
    <div class="tags-featured-resources">
        <div class="carousel-section">
            <div class="mg-carousel" id="slider-carousel">
               <div class="sfitemsList sfitemsListTitleDateTmb sflist featured-carousel-ul">                    
                    <asp:PlaceHolder ID="ItemsContainer" runat="server" />
                </div>
            </div>
            <div class="section slider-range">
                <div class="slider-range__slider-range" id="custom-slider-range"></div>
            </div>
        </div>	
        </div>
  	</LayoutTemplate>
    <ItemTemplate>
		<div class="mg-carousel__item"> 
		<div class="featured-carousel-li">
			<div class="mg-card-thumb">
                  <a href="/topics-and-tools/resources/resource/<%# Eval("UrlName")%>">
                    <asp:Repeater runat="server" DataSource='<%# Eval("featuredimage") %>'>
                              <ItemTemplate>       
                            <img src="<%# Eval("ThumbnailUrl")%>" alt="<%# Eval("AlternativeText")%>" title="" />
                        
                    </ItemTemplate>
                </asp:Repeater>
                      </a>
                </div>                
                  <div class="mg-card">
                          <div class="mg-card-title protected bright">
                             <sf:DetailsViewHyperLink ID="DetailsViewHyperLink" TextDataField="Title" runat="server" data-sf-field="Title" data-sf-ftype="ShortText" data-name="detailLink"  />
                          </div>
                  
                    <div class="mg-card-text">
                      <a href="/topics-and-tools/resources/resource/<%# Eval("UrlName")%>"><%# Eval("shortsummary")%></a>
                      </div>
                         <div data-cats="category" style="display:none;">
                        <sitefinity:HierarchicalTaxonField data-topics="topicItem" ID="HierarchicalFieldControl" runat="server" TaxonomyId="E5CD6D69-1543-427b-AD62-688A99F5E7D4" DisplayMode="Read" 
          WebServiceUrl="~/Sitefinity/Services/Taxonomies/HierarchicalTaxon.svc" Expanded="false" TaxonomyMetafieldName="Category" BindOnServer="true" style="display:none;" />
                    </div>
                    </div>  
		</div>
	  </div>
                  
    </ItemTemplate>
    </telerik:RadListView>
    <sf:Pager id="pager" runat="server"></sf:Pager>
    <asp:PlaceHolder ID="socialOptionsContainer" runat="server"></asp:PlaceHolder>