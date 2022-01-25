
export function prepareFieldsForm(fields, model, copyFields) {
    let fieldsForm = {};
    fieldsForm.model = {};
    fieldsForm.fields = fields;

    if (model && copyFields) {
        copyFields.map((fieldAlias) => {
            fieldsForm.model[fieldAlias] = model[fieldAlias];
        });
    }

    fieldsForm.fields.map((field) => {
        field.isBlur = false;
        field.error = null;
        if (field.mandatory === undefined)
            field.mandatory = false;

        if (model) {
            fieldsForm.model[field.alias] = model[field.alias];
        }

    });

    return fieldsForm;
}