Type.registerNamespace("SitefinityWebApp.WidgetDesigners.Account");

SitefinityWebApp.WidgetDesigners.Account.AccountDesigner = function (element) {
    /* Initialize Message fields */
    this._message = null;
    this._viewtemplate = null;
    this._loginrequired = null;
    /* Calls the base constructor */
    SitefinityWebApp.WidgetDesigners.Account.AccountDesigner.initializeBase(this, [element]);
}

SitefinityWebApp.WidgetDesigners.Account.AccountDesigner.prototype = {
    /* --------------------------------- set up and tear down --------------------------------- */
    initialize: function () {
        /* Here you can attach to events or do other initialization */
        SitefinityWebApp.WidgetDesigners.Account.AccountDesigner.callBaseMethod(this, 'initialize');
    },
    dispose: function () {
        /* this is the place to unbind/dispose the event handlers created in the initialize method */
        SitefinityWebApp.WidgetDesigners.Account.AccountDesigner.callBaseMethod(this, 'dispose');
    },

    /* --------------------------------- public methods ---------------------------------- */

    findElement: function (id) {
        var result = jQuery(this.get_element()).find("#" + id).get(0);
        return result;
    },

    /* Called when the designer window gets opened and here is place to "bind" your designer to the control properties */
    refreshUI: function () {
        var controlData = this._propertyEditor.get_control().Settings; /* JavaScript clone of your control - all the control properties will be properties of the controlData too */

        /* RefreshUI Message */
        jQuery(this.get_message()).val(controlData.Message);
        //ViewTemplate
        jQuery(this.get_viewtemplate()).val(controlData.ViewTemplate);
        
        if(controlData.LoginRequired){ jQuery(this.get_loginrequired()).prop('checked', true); }
    },

    /* Called when the "Save" button is clicked. Here you can transfer the settings from the designer to the control */
    applyChanges: function () {
        var controlData = this._propertyEditor.get_control().Settings;

        /* ApplyChanges Message */
        controlData.Message = jQuery(this.get_message()).val();
        //ViewTemplate
        controlData.ViewTemplate = jQuery(this.get_viewtemplate()).val();
        controlData.LoginRequired = false;
        if(jQuery(this.get_loginrequired()).is(':checked')){controlData.LoginRequired = true;}
    },

    /* --------------------------------- event handlers ---------------------------------- */

    /* --------------------------------- private methods --------------------------------- */

    /* --------------------------------- properties -------------------------------------- */

    /* Message properties */
    get_message: function () { return this._message; }, 
    set_message: function (value) { this._message = value; },

    get_viewtemplate: function () { return this._viewtemplate; }, 
    set_viewtemplate: function (value) { this._viewtemplate = value; },


    get_loginrequired: function () { return this._loginrequired; }, 
    set_loginrequired: function (value) { this._loginrequired = value; }
}

SitefinityWebApp.WidgetDesigners.Account.AccountDesigner.registerClass('SitefinityWebApp.WidgetDesigners.Account.AccountDesigner', Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);
