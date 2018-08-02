module.exports = exporting;

function exporting(jQuery) {

    (function ($, window, document, undefined) {
        var SelectTree = {

            _classes: {
                container: "mtx-select-tree-container",
                val_container: "mtx-select-tree-values"
            },

            _init: function (options, elem) {
                var self = this;

                self.elem = elem;
                self.$elem = $(elem);

                self.options = $.extend({}, $.fn.selectTree.options, options);

                if (self.options.hideSelect) self.hideSelect();
                self.createTreeContainer();
                self.traverseAndAppendOptgroups();
                self.bindChangeToSelect();
                self.populateValuesContainer();
            },

            bindChangeToSelect: function () {
                var self = this;

                self.$elem.bind('change', function () {

                    var $select = $(this),
                        $options = $select.find('option');

                    $options.each(function () {

                        if ($(this).is(":selected")) {
                            self.setCheckboxState($(this).data("relLi"), "checked");
                        } else {
                            self.setCheckboxState($(this).data("relLi"), "unchecked");
                        }

                    });

                    self.populateValuesContainer();

                });
            },

            hideSelect: function () {
                var self = this;
                self.$elem.addClass('visuallyhidden');
            },

            createTreeContainer: function () {
                var self = this;

                if (self.options.treeContainer === null) {
                    self.$container = $('<div/>', {
                        "class": self._classes.container
                    });
                    self.$container.insertAfter(self.$elem);
                } else {
                    self.$container = self.options.treeContainer;
                    self.$container.addClass(self._classes.container);
                }

                if (self.options.valueContainer === null) {
                    self.$values = $('<div/>', {
                        "class": self._classes.val_container
                    });
                    self.$values.insertAfter(self.$container);
                } else {
                    self.$values = self.options.valuesContainer;
                    self.$values.addClass(self._classes.val_container);
                }

            },

            traverseAndAppendOptgroups: function () {
                var self = this,
                    $list = $('<ul/>');

                self.$elem.find("optgroup").each(function () {
                    var $groupLi = $('<li/>');

                    $('<a/>', {
                        href: "javascript:void(0);",
                        html: "[&#43;] ",
                        click: function () {
                            var $anchor = $(this);
                            $(this).siblings("ul").slideToggle(300, function () {
                                if ($(this).is(":visible")) {
                                    $anchor.html('[&ndash;] ');
                                } else {
                                    $anchor.html('[&#43;] ');
                                }
                            });
                        }
                    }).appendTo($groupLi);

                    $('<span/>', {
                        html: $(this).attr("label")
                    }).appendTo($groupLi);

                    $('<a/>', {
                        href: "javascript:void(0);",
                        text: "Check All",
                        "class": "toggle-checks",
                        data: {
                            isChecked: false
                        },
                        click: function (e) {
                            var $checksUl = $(this).siblings("ul");
                            e.preventDefault();

                            if (!$(this).data("isChecked")) {
                                $checksUl.find("li").each(function () {
                                    if (!$(this).hasClass("selected")) {
                                        $(this).children("a").click();
                                    }
                                });
                                $(this).data("isChecked", true);
                                $(this).text('Uncheck All');
                            } else {
                                $checksUl.find("li").each(function () {
                                    if ($(this).hasClass("selected")) {
                                        $(this).children("a").click();
                                    }
                                });
                                $(this).data("isChecked", false);
                                $(this).text('Check All');
                            }
                        }
                    }).appendTo($groupLi);

                    // find all options in the optgroup
                    self.traverseAndAppendOptions($(this), $groupLi);
                    $groupLi.appendTo($list);
                });

                $list.appendTo(self.$container);
            },

            traverseAndAppendOptions: function ($optgroup, $groupLi) {
                var self = this,
                    $newUl = $('<ul/>');

                $optgroup.find("option").each(function () {
                    var $optLi = $('<li/>').data('option', $(this)).data('optval', $(this).val());

                    // associate the new list item with the select option
                    $(this).data('relLi', $optLi);

                    $("<a/>", {
                        href: "javascript:void(0);",
                        text: $(this).text(),
                        click: function (e) {
                            self.toggleOptionSelection($(this));
                        }
                    }).appendTo($optLi);

                    $('<input type="checkbox"/>').prependTo($optLi)
                        .click(function (e) {
                            self.toggleOptionSelection($(this));
                        });

                    $optLi.appendTo($newUl);

                    if ($(this).is(':selected')) {
                        self.setCheckboxState($optLi, 'checked');
                    }

                });

                $newUl.appendTo($groupLi);
            },

            toggleOptionSelection: function ($anchor) {
                var self = this,
                    $li = $anchor.closest("li"),
                    $option = $li.data('option'),
                    $check = $li.find(":checkbox");

                $option.prop("selected", function (i, val) {
                    return !val;
                });

                if ($option.prop("selected")) {
                    self.setCheckboxState($li, 'checked');
                } else {
                    self.setCheckboxState($li, 'unchecked');
                }

                self.populateValuesContainer();
                self.updateCheckToggle($li);
            },

            setCheckboxState: function ($li, state) {
                var self = this,
                    $check = $li.find(":checkbox");

                if (state == "checked") {
                    $check.prop("checked", true);
                    $li.addClass("selected");
                } else if (state == "unchecked") {
                    $check.prop("checked", false);
                    $li.removeClass("selected");
                }
            },

            getSelectValue: function () {
                var self = this,
                    values = new Array();

                self.$elem.find(":selected").each(function () {
                    var selectedVal = $(this);
                    values.push(selectedVal);
                });

                return values;
            },

            populateValuesContainer: function () {
                var self = this,
                    values = self.getSelectValue();

                self.$values.empty();

                $.each(values, function (i, $option) {

                    $("<span/>", {
                        text: $option.text(),
                        click: function () {
                            var $anchor = self.$container.find("li").filter(function () {
                                return $(this).data('optval') === $option.val();
                            });

                            self.toggleOptionSelection($anchor);
                        }
                    }).appendTo(self.$values);

                });
            },

            updateCheckToggle: function ($listItem) {
                var $parentUl = $listItem.closest("ul");

                if ($parentUl.find(":checked").length === 0) {
                    $parentUl.siblings(".toggle-checks").data("isChecked", false).text("Check All");
                } else if ($parentUl.find(":checked").length === $parentUl.find(":checkbox").length) {
                    $parentUl.siblings(".toggle-checks").data("isChecked", true).text("Uncheck All");
                }
            },

            uncheckAll: function() {
                var self = this;
                var $options = this.$elem.find('option');
                var $ul = this.$container.find('ul');
                var $lis = this.$container.find('li');

                $options.each(function() {
                    this.selected = false;
                });
                $lis.each(function() {
                    self.setCheckboxState($(this), "unchecked");
                });
                $ul.siblings(".toggle-checks").data("isChecked", false).text("Check All");
            }

        };

        $.fn.selectTree = function (options) {
            return this.each(function () {
                if (typeof options === 'string' && 'selectTree' in this) {
                    this.selectTree[options]();
                    return;
                }

                this.selectTree = Object.create(SelectTree);
                this.selectTree._init(options, this);
            });
        };

        $.fn.selectTree.options = {
            hideSelect: false,
            treeContainer: null,
            valueContainer: null
        };
    })(jQuery, window, document);

    // Polyfill for Object.create method
    if (typeof Object.create !== 'function') {
        Object.create = function (obj) {
            function F() {};
            F.prototype = obj;
            return new F();
        };
    }
}
