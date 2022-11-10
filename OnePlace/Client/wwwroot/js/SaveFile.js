/*aqui creamos un metodo para descargar el pdf*/
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

//Ver PDF en otra ventana si sirve pero no lo usamos
//function OpenPdfNewTab(filename, byteBase64) {
//    var blob = base64Blob(byteBase64);
//    var blobURL = URL.createObjectURL(blob);
//    window.open(blobURL);
//}
//function base64Blob(b64Data) {
//    sliceSize = 512;
//    var byteCharacters = atob(b64Data);
//    var byteArrays = [];
//    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
//        var slice = byteCharacters.slice(offset, offset + sliceSize);
//        var byteNumbers = new Array(slice.length);
//        for (var i = 0; i < slice.length; i++) {
//            byteNumbers[i] = slice.charCodeAt(i);
//        }
//        var byteArray = new Uint8Array(byteNumbers);
//        byteArrays.push(byteArray);
//    }
//    var blob = new Blob(byteArrays, { type: 'application/pdf' });
//    return blob;
//}

//funcion para descargar una imagen se convirtio de ts a js
function downloadFromUrl(options) {
    var _a;
    var anchorElement = document.createElement('a');
    anchorElement.href = options.url;
    anchorElement.download = (_a = options.fileName) !== null && _a !== void 0 ? _a : '';
    anchorElement.click();
    anchorElement.remove();
}
