window.myFunctions = {
    getFormData: function (form) {
        const formData = new FormData(form);
        let data = {};

        for (let [key, value] of formData.entries()) {
            // Si tienes múltiples valores para la misma key (ej: checkbox), deberás manejarlos aquí.
            data[key] = value;
            console.log('key: ' + data[key]);
            console.log('value: ' + value);
        }

        return JSON.stringify(data);
    }
};