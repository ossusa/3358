Type.registerNamespace("SitefinityWebApp.WidgetDesigners.AuthService");

SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner = function (element) {
    /* Initialize Message fields */
    this._message = null;
    this._showLoginForm = null;
    
    
    /* Calls the base constructor */
    SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner.initializeBase(this, [element]);
}

SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner.prototype = {
    /* --------------------------------- set up and tear down --------------------------------- */
    initialize: function () {
        /* Here you can attach to events or do other initialization */
        SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner.callBaseMethod(this, 'initialize');
    },
    dispose: function () {
        /* this is the place to unbind/dispose the event handlers created in the initialize method */
        SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner.callBaseMethod(this, 'dispose');
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
        jQuery(this.get_showLoginForm()).val(controlData.ShowLoginForm);
        if(controlData.ShowLoginForm){ jQuery(this.get_showLoginForm()).prop('checked', true); }
        
    },

    /* Called when the "Save" button is clicked. Here you can transfer the settings from the designer to the control */
    applyChanges: function () {
        var controlData = this._propertyEditor.get_control().Settings;

        /* ApplyChanges Message */
        controlData.Message = jQuery(this.get_message()).val();        
        controlData.ShowLoginForm = false;
        if(jQuery(this.get_showLoginForm()).is(':checked')){controlData.ShowLoginForm = true;}
        
        
    },

    /* --------------------------------- event handlers ---------------------------------- */

    /* --------------------------------- private methods --------------------------------- */

    /* --------------------------------- properties -------------------------------------- */

    /* Message properties */
    get_message: function () { return this._message; }, 
    set_message: function (value) { this._message = value; },

    get_showLoginForm: function(){ return this._showLoginForm;
        },
    set_showLoginForm: function(value){this._showLoginForm = value;}
}

SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner.registerClass('SitefinityWebApp.WidgetDesigners.AuthService.AuthServiceDesigner', Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);
