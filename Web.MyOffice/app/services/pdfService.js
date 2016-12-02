(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('pdfService', [pdfService]);

    function pdfService() {
        return {
            createPDF: function (element, fileName, pdfOptions) {
                element = element || $('.exportable');
                fileName = fileName || 'export.pdf';
                pdfOptions = pdfOptions || {
                    format: 'a4',
                    orientation: 'l'
                };

                var width = element.width();
                var doc = new jsPDF(pdfOptions);
                element.width((doc.internal.pageSize.width * 2.8346 * 1.33333) - 80).css('max-width', 'none');
                $('.notPrintable').css('visibility', 'hidden');

                var useWidth = element.width();
                var useHeight = element.height();

                var crop = function crop(can, a, b) {
                    var ctx = can.getContext('2d');
                    var imageData = ctx.getImageData(a.x, a.y, b.x, b.y);
                    var newCan = document.createElement('canvas');
                    newCan.width = b.x - a.x;
                    newCan.height = b.y - a.y;
                    var newCtx = newCan.getContext('2d');
                    newCtx.putImageData(imageData, 0, 0);

                    return newCan;
                };

                html2canvas(element, {
                    imageTimeout: 2000,
                    removeContainer: true,
                    width: useWidth,
                    height: useHeight,
                    onrendered: function (canvas) {
                        element.width(width);
                        $('.notPrintable').css('visibility', '');

                        var ph = (doc.internal.pageSize.height * 2.8346) - 20;
                        for (var i = 0; i < Math.ceil(canvas.height / ph) ; i++) {
                            var a = { y: (ph * i * 1.2388), x: 0 };
                            var b = { y: (ph * (i + 1) * 1.2388), x: canvas.width };
                            var cnv = crop(canvas, a, b);
                            var img = cnv.toDataURL('image/png');

                            if (i !== 0) {
                                doc.addPage();
                            }
                            doc.addImage(img, 'JPEG', 20, 20);
                        }
                        if (typeof fileName === 'function') {
                            fileName(doc);
                        } else {
                            doc.save(fileName);
                        }
                    }
                });
            }
        };
    };
})();
