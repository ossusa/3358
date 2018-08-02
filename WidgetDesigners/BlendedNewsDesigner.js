Type.registerNamespace("SitefinityWebApp.WidgetDesigners");

SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner = function (element) {
    /* Initialize ItemsPerPage fields */
    this._itemsPerPage = null;
    
    /* Initialize Limit fields */
    this._limit = null;
    
    this._CategoriesSelector = null;
    this._providers = null;
    this._viewTemplate = null;
    
    /* Calls the base constructor */
    SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner.initializeBase(this, [element]);
}

SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner.prototype = {
    /* --------------------------------- set up and tear down --------------------------------- */
    initialize: function () {
        /* Here you can attach to events or do other initialization */
        SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner.callBaseMethod(this, 'initialize');
    },
    dispose: function () {
        /* this is the place to unbind/dispose the event handlers created in the initialize method */
        SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner.callBaseMethod(this, 'dispose');
    },

    /* --------------------------------- public methods ---------------------------------- */

    findElement: function (id) {
        var result = jQuery(this.get_element()).find("#" + id).get(0);
        return result;
    },

    /* Called when the designer window gets opened and here is place to "bind" your designer to the control properties */
    refreshUI: function () {
        var controlData = this._propertyEditor.get_control().Settings; /* JavaScript clone of your control - all the control properties will be properties of the controlData too */

        /* RefreshUI ItemsPerPage */
        jQuery(this.get_itemsPerPage()).val(controlData.ItemsPerPage);

        /* RefreshUI Limit */
        jQuery(this.get_limit()).val(controlData.Limit);
        
        var pNames = controlData.ProviderNames;
        if (pNames != null) {
            jQuery(".providers input[type='checkbox']").each(function (i, item) {
                var value = $(item).val();
                var checked = pNames.split(',').indexOf(value) > -1;
                if (checked) {
                    $(item).attr("checked", "checked");
                }
            });
        }

        jQuery(this.get_viewTemplate()).val(controlData.ViewTemplate);

        // resize control designer on category selector close
        var c = this.get_CategoriesSelector();
        var cs = c.get_taxaSelector();
        cs.add_selectionDone(this._resizeControlDesigner);

        // resize control designer on category selector open]
        var csb = c.get_changeSelectedTaxaButton();
        this._resizeControlDesignerDelegate = Function.createDelegate(this, this._resizeControlDesigner);
        $addHandler(csb, "click", this._resizeControlDesignerDelegate);

        /* RefreshUI Categories */
        var cats = controlData.CategoryIds;
        if (cats != null && cats != "00000000-0000-0000-0000-000000000000")
            c.set_value(controlData.CategoryIds.split(","));
    },

    /* Called when the "Save" button is clicked. Here you can transfer the settings from the designer to the control */
    applyChanges: function () {
        var controlData = this._propertyEditor.get_control().Settings;

        /* ApplyChanges ItemsPerPage */
        controlData.ItemsPerPage = jQuery(this.get_itemsPerPage()).val();

        var pNames = [];
        jQuery(".providers input[type='checkbox']").each(function (i, item) {
            var value = $(item).val();
            var checked = $(item).is(":checked");
            if (checked) {
                pNames.push(value);
            }
        });

        controlData.ProviderNames = pNames.join();

        controlData.ViewTemplate = jQuery(this.get_viewTemplate()).val();

        /* ApplyChanges Limit */
        controlData.Limit = jQuery(this.get_limit()).val();      

        var c = this.get_CategoriesSelector();
        var cats = c.get_value();
        if (cats != null)
            controlData.CategoryIds = c.get_value().join();
    },

    /* --------------------------------- event handlers ---------------------------------- */

    /* --------------------------------- private methods --------------------------------- */

    /* --------------------------------- properties -------------------------------------- */

    /* ItemsPerPage properties */
    get_itemsPerPage: function () { return this._itemsPerPage; }, 
    set_itemsPerPage: function (value) { this._itemsPerPage = value; },

    /* Limit properties */
    get_limit: function () { return this._limit; }, 
    set_limit: function (value) { this._limit = value; },
    
    get_providers: function () { return this._providers; },
    set_providers: function(value) { this._providers = value; },

    get_viewTemplate: function() { return this._viewTemplate; },
    set_viewTemplate: function(value) { this._viewTemplate = value; },

    get_CategoriesSelector: function () {
        return this._CategoriesSelector;
    },
    set_CategoriesSelector: function (value) {
        this._CategoriesSelector = value;
    },
    // function to initialize resizer methods and handlers
    _resizeControlDesigner: function () {
        setTimeout("dialogBase.resizeToContent()", 100);
    }
}

SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner.registerClass('SitefinityWebApp.WidgetDesigners.BlendedNewsDesigner', Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);
