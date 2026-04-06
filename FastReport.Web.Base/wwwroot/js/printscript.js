parent.focus();
parent.print();
window.addEventListener('afterprint', function (event) { window.close(); })