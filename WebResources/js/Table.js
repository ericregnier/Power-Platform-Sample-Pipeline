"use strict";

var Sample = Sample || {};
Sample.Table = Sample.Table || {};

Sample.Table = {
    OnLoad(context) {
        let formContext = context.getFormContext();
        this.AddOnChangeHandlers(formContext);
    },

    AddOnChangeHandlers(formContext) {
        formContext.getAttribute("statuscode").addOnChange(this.OnStatusCodeChanged);
    },

    async OnStatusCodeChanged(context) {
        let formContext = context.getFormContext();
    }
}