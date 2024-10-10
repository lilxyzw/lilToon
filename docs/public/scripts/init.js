var scripts = ['/lilToon/scripts/jquery.min.js', '/lilToon/scripts/lightbox.min.js', '/lilToon/scripts/script.js'];
var i = 0;

function appendScript() {
    var script = document.createElement('script');
    script.src = scripts[i];
    document.body.appendChild(script);

    if (i++ < 2) {
        script.onload = appendScript;
    }
}

appendScript();