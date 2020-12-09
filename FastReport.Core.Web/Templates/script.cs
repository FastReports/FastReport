using static FastReport.Web.Constants;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_script() => $@"
'use strict';

var {template_FR} = {{

    outline: function () {{
        var sizes = sessionStorage.getItem('fastreport-outline-split-sizes');

        if (sizes) {{
            sizes = JSON.parse(sizes);
        }} else {{
            sizes = [25, 75];
        }}

        var split = this.Split(['.{template_FR}-outline', '.{template_FR}-report'], {{
            sizes: sizes,
            minSize: [0, 50],
            snapOffset: 20,
            onDragEnd: function () {{
                sessionStorage.setItem('fastreport-outline-split-sizes', JSON.stringify(split.getSizes()));
            }},
            elementStyle: function (dimension, size, gutterSize) {{
                return {{
                    'flex-basis': 'calc(' + size + '% - ' + gutterSize + 'px)'
                }}
            }},
            gutterStyle: function (dimension, gutterSize) {{
                return {{
                    'flex-basis':  gutterSize + 'px'
                }}
            }},
            gutter: function (index, direction) {{
                var gutter = document.createElement('div');
                gutter.className = '{template_FR}-gutter {template_FR}-gutter-' + direction;
                return gutter;
            }}
        }});

        var tree = sessionStorage.getItem('fastreport-outline-tree');
        if (tree) {{
            tree = JSON.parse(tree);
            var that = this;
            var container = this._findContainer();
            Object.keys(tree).forEach(function (key) {{
                var caret = container.querySelector('[data-fr-outline-node-id=""' + key + '""]');
                if (caret) {{
                    that.outlineOpenNode(caret, true);
                }}
            }});
        }}
    }},

    outlineOpenNode: function (caret, skipTreeStorage) {{
        caret.parentNode.parentNode.getElementsByClassName('{template_FR}-outline-children')[0].style.display = '';
        caret.parentNode.parentNode.getElementsByClassName('{template_FR}-js-outline-open-node')[0].style.display = 'none';
        caret.parentNode.parentNode.getElementsByClassName('{template_FR}-js-outline-close-node')[0].style.display = '';

        if (skipTreeStorage === true) {{
            return;
        }}

        var tree = sessionStorage.getItem('fastreport-outline-tree');
        if (tree) {{
            tree = JSON.parse(tree);
        }} else {{
            tree = {{}};
        }}

        tree[caret.getAttribute('data-fr-outline-node-id')] = true;
        sessionStorage.setItem('fastreport-outline-tree', JSON.stringify(tree));
    }},

    outlineCloseNode: function (caret) {{
        caret.parentNode.parentNode.getElementsByClassName('{template_FR}-outline-children')[0].style.display = 'none';
        caret.parentNode.parentNode.getElementsByClassName('{template_FR}-js-outline-open-node')[0].style.display = '';
        caret.parentNode.parentNode.getElementsByClassName('{template_FR}-js-outline-close-node')[0].style.display = 'none';

        var tree = sessionStorage.getItem('fastreport-outline-tree');
        if (tree) {{
            tree = JSON.parse(tree);
        }} else {{
            tree = {{}};
        }}

        delete tree[caret.getAttribute('data-fr-outline-node-id')];
        sessionStorage.setItem('fastreport-outline-tree', JSON.stringify(tree));
    }},

    outlineGoto: function (page, offset) {{
        this.goto(page);
    }},

    refresh: function () {{
        this._reload();
    }},

    zoom: function (value) {{
        this._closeDropdowns();
        this._reload('&skipPrepare=yes&zoom=' + value);
    }},

    goto: function (page) {{
        this._reload('&skipPrepare=yes&goto=' + page);"+
        (ShowBottomToolbar? $@"document.getElementsByClassName('{template_FR}-body')[0].scrollIntoView({{behavior:""smooth""}});" : "")+$@"
    }},

    click: function (el, kind, value) {{
        var that = this;

        if (kind == 'text_edit') {{
            if (that._win) {{
                that._win.close();
            }}
            that._win = this._popup('{template_ROUTE_BASE_PATH}/preview.textEditForm?reportId={ID}&click=' + value, 'Text edit', 400, 200);
            that._win.onmessage = function (e) {{
                if (e.data == 'submit') {{
                    var newText = that._win.document.querySelector('textarea').value;
                    var form = new FormData();
                    form.append('text', newText);
                    that._reload('&skipPrepare=yes&' + kind + '=' + value, form);
                    that._win.close();
                }}
            }};
            return;
        }}

        this._reload('&skipPrepare=yes&' + kind + '=' + value);
    }},

    settab: function (tab) {{
        this._reload('&skipPrepare=yes&settab=' + tab);
    }},

    closetab: function (tab) {{
        this._reload('&skipPrepare=yes&closetab=' + tab);
    }},

    _reload: function (params, form) {{
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();

        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/preview.getReport?reportId={ID}&renderBody=yes' + (params || ''),
            form: form,
            onSend: function () {{
                that._activateSpinner();
                //that._lockToolbar();
            }},
            onSuccess: function (xhr) {{
                container.outerHTML = xhr.responseText;
                that._execScripts();
            }},
            onError: function (xhr) {{
                that._placeError(xhr, body);
                that._deactivateSpinner();
            }},
            onFinally: function () {{
                //that._unlockToolbar();
            }}
        }});
    }},

    {SILENT_RELOAD}: function (params, form) {{
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();

        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/preview.getReport?reportId={ID}&renderBody=yes' + (params || ''),
            form: form,
            onSuccess: function (xhr) {{
                container.outerHTML = xhr.responseText;
                that._execScripts();
            }},
            onError: function (xhr) {{
                that._placeError(xhr, body);
            }},
        }});
    }},


    {DIALOG}: function (params, form) {{
        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/dialog?reportId={ID}' + (params || ''),
            form: form
        }});
    }},

    _execScripts: function () {{
        var container = this._findContainer();
        var scripts = container.getElementsByTagName('script');
        for (var i = 0; i < scripts.length; i++) {{
            eval(scripts[i].text);
        }}
    }},

    _placeError: function (xhr, body) {{
        //var iframe = document.createElement('iframe');
        //iframe.src = 'data:text/html;charset=utf-8,' + escape(xhr.responseText);
        //body.innerHTML = '<div class=""{template_FR}-error-container""><div class=""{template_FR}-error"">Error<br>' + xhr.status + ' - ' + xhr.statusText + '</div>' + iframe.outerHTML + '</div>';
        body.innerHTML = '<div class=""{template_FR}-error-container""><div class=""{template_FR}-error-text"">Error<br>' + xhr.status + ' - ' + xhr.statusText + '</div><div class=""{template_FR}-error-response"">' + xhr.responseText + '</div></div>';
    }},

    _activateSpinner: function () {{
        document.getElementsByClassName('{template_FR}-spinner')[0].style.display = '';
    }},

    _deactivateSpinner: function () {{
        document.getElementsByClassName('{template_FR}-spinner')[0].style.display = 'none';
    }},

    _findContainer: function () {{
        return document.getElementsByClassName('{template_FR}-container')[0];
    }},

/*
    _findToolbar: function () {{
        return document.getElementsByClassName('{template_FR}-toolbar')[0];
    }},
*/

    _findBody: function () {{
        return document.getElementsByClassName('{template_FR}-body')[0];
    }},

    _closeDropdowns: function () {{
        var dropdowns = document.getElementsByClassName('{template_FR}-dropdown-content');

        var func = function (dd) {{
            setTimeout(function () {{
                dd.style['display'] = '';
            }}, 100);
        }}

        for (var i = 0; i < dropdowns.length; i++) {{
            var dd = dropdowns[i];
            dd.style['display'] = 'none';
            func(dd);
        }}
    }},

/*
    _lockToolbar: function () {{
        var toolbar = this._findToolbar();
        if (toolbar) {{
            toolbar.style['pointer-events'] = 'none';
        }}
    }},

    _unlockToolbar: function () {{
        var toolbar = this._findToolbar();
        if (toolbar) {{
            toolbar.style['pointer-events'] = '';
        }}
    }},
*/

    _fetchQueue: [],

    _fetch: function (options) {{
        var method = options.method;
        var url = options.url;
        var form = options.form;
        var onSuccess = options.onSuccess;
        var onError = options.onError;
        var onSend = options.onSend;
        var onFinally = options.onFinally;

        var that = this;
        var xhr = new XMLHttpRequest();
        xhr.__form = form;
        xhr.__onSend = onSend;
        xhr.open(method, url, true);
        xhr.onreadystatechange = function () {{
            if (xhr.readyState != 4)
                return;

            if (xhr.status != 200) {{
                if (typeof onError === 'function') {{
                    onError(xhr);
                }}
            }} else {{
                if (typeof onSuccess === 'function') {{
                    onSuccess(xhr);
                }}
            }}

            if (typeof onFinally === 'function') {{
                onFinally(xhr);
            }}

            that._nextFetch();
        }};

        this._fetchQueue.push(xhr);

        if (this._fetchQueue.length == 1) {{
            var f = this._fetchQueue[0];
            if (typeof f.__onSend === 'function') {{
                f.__onSend(f);
            }}
            f.send(f.__form);
        }}
    }},

    _nextFetch: function () {{
        this._fetchQueue.shift();

        if (this._fetchQueue.length) {{
            var f = this._fetchQueue[0];
            if (typeof f.__onSend === 'function') {{
                f.__onSend(f);
            }}
            f.send(f.__form);
        }}
    }},

    _popup: function (url, title, w, h) {{
        // Fixes dual-screen position                         Most browsers       Firefox
        var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : window.screenX;
        var dualScreenTop = window.screenTop != undefined ? window.screenTop : window.screenY;

        var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        var left = ((width / 2) - (w / 2)) + dualScreenLeft;
        var top = ((height / 2) - (h / 2)) + dualScreenTop;

        var params = 'menubar=0, toolbar=0, location=0, status=0, resizable=1, scrollbars=1';
        var newWindow = window.open(url, title, params + ', width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);

        if (newWindow.focus) {{
            newWindow.focus();
        }}

        return newWindow;
    }}

}};
";
    }
}