Type.registerNamespace("SitefinityWebApp.WidgetDesigners");

SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner = function (element) {
    /* Initialize CollectionId fields */
    this._collectionId = null;
    
    /* Initialize DateRangeName fields */
    this._dateRangeName = null;
    
    /* Initialize FacetNames fields */
    this._facetNames = null;
    
    /* Initialize SearchBloxServerIp fields */
    this._searchBloxServerIp = null;
    
    /* Initialize ShowSearchBox fields */
    this._showSearchBox = null;
    
    /* Calls the base constructor */
    SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner.initializeBase(this, [element]);
}

SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner.prototype = {
    /* --------------------------------- set up and tear down --------------------------------- */
    initialize: function () {
        /* Here you can attach to events or do other initialization */
        SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner.callBaseMethod(this, 'initialize');
    },
    dispose: function () {
        /* this is the place to unbind/dispose the event handlers created in the initialize method */
        SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner.callBaseMethod(this, 'dispose');
    },

    /* --------------------------------- public methods ---------------------------------- */

    findElement: function (id) {
        var result = jQuery(this.get_element()).find("#" + id).get(0);
        return result;
    },

    /* Called when the designer window gets opened and here is place to "bind" your designer to the control properties */
    refreshUI: function () {
        var controlData = this._propertyEditor.get_control().Settings; /* JavaScript clone of your control - all the control properties will be properties of the controlData too */

        /* RefreshUI CollectionId */
        jQuery(this.get_collectionId()).val(controlData.CollectionId);

        /* RefreshUI DateRangeName */
        jQuery(this.get_dateRangeName()).val(controlData.DateRangeName);

        /* RefreshUI FacetNames */
        jQuery(this.get_facetNames()).val(controlData.FacetNames);

        /* RefreshUI SearchBloxServerIp */
        jQuery(this.get_searchBloxServerIp()).val(controlData.SearchBloxServerIp);

        /* RefreshUI ShowSearchBox */
        jQuery(this.get_showSearchBox()).attr("checked", controlData.ShowSearchBox);
    },

    /* Called when the "Save" button is clicked. Here you can transfer the settings from the designer to the control */
    applyChanges: function () {
        var controlData = this._propertyEditor.get_control().Settings;

        /* ApplyChanges CollectionId */
        controlData.CollectionId = jQuery(this.get_collectionId()).val();

        /* ApplyChanges DateRangeName */
        controlData.DateRangeName = jQuery(this.get_dateRangeName()).val();

        /* ApplyChanges FacetNames */
        controlData.FacetNames = jQuery(this.get_facetNames()).val();

        /* ApplyChanges SearchBloxServerIp */
        controlData.SearchBloxServerIp = jQuery(this.get_searchBloxServerIp()).val();

        /* ApplyChanges ShowSearchBox */
        controlData.ShowSearchBox = jQuery(this.get_showSearchBox()).is(":checked");
    },

    /* --------------------------------- event handlers ---------------------------------- */

    /* --------------------------------- private methods --------------------------------- */

    /* --------------------------------- properties -------------------------------------- */

    /* CollectionId properties */
    get_collectionId: function () { return this._collectionId; }, 
    set_collectionId: function (value) { this._collectionId = value; },

    /* DateRangeName properties */
    get_dateRangeName: function () { return this._dateRangeName; }, 
    set_dateRangeName: function (value) { this._dateRangeName = value; },

    /* FacetNames properties */
    get_facetNames: function () { return this._facetNames; }, 
    set_facetNames: function (value) { this._facetNames = value; },

    /* SearchBloxServerIp properties */
    get_searchBloxServerIp: function () { return this._searchBloxServerIp; }, 
    set_searchBloxServerIp: function (value) { this._searchBloxServerIp = value; },

    /* ShowSearchBox properties */
    get_showSearchBox: function () { return this._showSearchBox; }, 
    set_showSearchBox: function (value) { this._showSearchBox = value; }
}

SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner.registerClass('SitefinityWebApp.WidgetDesigners.SBAngularSearchResultsDesigner', Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);
