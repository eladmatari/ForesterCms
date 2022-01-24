
export function prepareFieldsForm(fieldsForm, model) {
    fieldsForm.model = model || {};

    fieldsForm.fields.map((field) => {
        field.isBlur = false;
        field.error = null;
        if (field.mandatory === undefined)
            field.mandatory = false;
    });

    return fieldsForm;
}