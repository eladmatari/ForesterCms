
export function prepareFieldsForm(fieldsForm) {
    fieldsForm.getModel = function (onComplete) {
        var model = {};

        var fieldValuesCount = 0;

        fieldsForm.fields.map((field) => {
            if (field.getValue) {
                field.getValue((val) => {
                    model[field.alias] = val;
                    fieldValuesCount++;
                })
            }
            else {
                model[field.alias] = field.value;
                fieldValuesCount++;
            }
        });

        var intervalId = setInterval(function () {
            if (fieldValuesCount == fieldsForm.fields.length) {
                clearInterval(intervalId);
                onComplete(model);
            }
        }, 50);
    }

    fieldsForm.fields.map((field) => {
        field.value = null;
        field.isBlur = false;
        field.error = null;
        if (field.mandatory === undefined)
            field.mandatory = false;
    });

    return fieldsForm;
}