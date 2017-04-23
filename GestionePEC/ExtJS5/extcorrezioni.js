Ext.define('Ext.overrides.selection.CheckboxModel', {
    override: 'Ext.selection.CheckboxModel',
    privates: {
        selectWithEventMulti: function (record, e, isSelected) {
            var me = this;

            if (!e.shiftKey && !e.ctrlKey && e.getTarget(me.checkSelector)) {
                if (isSelected) {
                    me.doDeselect(record); // Second param here is suppress event, not "keep selection"
                } else {
                    me.doSelect(record, true);
                }
            } else {
                me.callParent([record, e, isSelected]);
            }
        }
    }
});



Ext.override(Ext.form.field.Base, {
    initEvents: function () {
        var me = this,
        inputEl = me.inputEl,
            onFieldMutation = me.onFieldMutation,
            events = me.checkChangeEvents,
            len = events.length,
            i, event;
        if (inputEl) {
            me.mon(inputEl, Ext.supports.SpecialKeyDownRepeat ? 'keydown' : 'keypress', me.fireKey, me);
            for (i = 0; i < len; ++i) {
                event = events[i];
                if (event === 'propertychange') {
                    me.usesPropertychange = true;
                }
                if (event === 'textInput') {
                    me.usesTextInput = true;
                }
                me.mon(inputEl, event, onFieldMutation, me);
            }
        }
        me.callParent();
    },

    bindPropertyChange: function (active) {
        var method = active ? 'resumeEvent' : 'suspendEvent',
            inputEl = this.inputEl;
        if (this.usesPropertychange) {
            inputEl[method]('propertychange');
        }
        if (this.usesTextInput) {
            inputEl[method]('textInput');
        }
    }
});