using static FastReport.Web.Constants;

namespace FastReport.Web
{
    partial class WebReport
    {

        string template_script()
        {
            var localization = new ToolbarLocalization(Res);
            return $@"
'use strict';

var {template_FR} = {{

        toggleSearchForm: function() {{
            var form = document.getElementById('{template_FR}-toolbar-search-form');
            form.classList.toggle('open');
            if (!form.classList.contains('open')) {{
                var searchText = sessionStorage.getItem('fastreport-search-text');
                var matchCase = sessionStorage.getItem('fastreport-search-match-case') === 'true';
                var wholeWord = sessionStorage.getItem('fastreport-search-whole-word') === 'true';
                this.findNext(
                    sessionStorage.getItem('fastreport-search-index'),
                    searchText,
                    matchCase,
                    wholeWord,
                    true
                );  
            }}
        }},

        saveSearchFormState: function() {{
            sessionStorage.setItem('fastreport-search-text-state', document.getElementById('fr-search-text').value);
            sessionStorage.setItem('fastreport-search-match-case-state', document.getElementById('fr-match-case').checked);
            sessionStorage.setItem('fastreport-search-whole-word-state', document.getElementById('fr-whole-word').checked);
        }},

        restoreSearchFormState: function(resetIndex = false, openAfter = false) {{
            document.getElementById('fr-search-text').value = sessionStorage.getItem('fastreport-search-text-state');
            document.getElementById('fr-match-case').checked = sessionStorage.getItem('fastreport-search-match-case-state') === 'true';
            document.getElementById('fr-whole-word').checked = sessionStorage.getItem('fastreport-search-whole-word-state') === 'true';
            this.onEnterSearchText();
            if (resetIndex)
                sessionStorage.setItem('fastreport-search-text', '');
            if (openAfter)
                document.getElementById('{template_FR}-toolbar-search-form').classList.toggle('open');
         }},

        onEnterSearchText: function () {{
            if(document.getElementById('fr-search-text').value == '') {{
                document.getElementById('fr-search-prev').setAttribute('disabled', 'disabled');
                document.getElementById('fr-search-next').setAttribute('disabled', 'disabled');
                document.getElementById('fr-match-case').setAttribute('disabled', 'disabled');
                document.getElementById('fr-whole-word').setAttribute('disabled', 'disabled');
                document.getElementById('clear-searchbox').setAttribute('hidden', 'hidden');
            }}
            else{{
                document.getElementById('fr-search-prev').removeAttribute('disabled');
                document.getElementById('fr-search-next').removeAttribute('disabled');
                document.getElementById('fr-match-case').removeAttribute('disabled');
                document.getElementById('fr-whole-word').removeAttribute('disabled');
                document.getElementById('clear-searchbox').removeAttribute('hidden');
            }}
        }},

        clearSearchText: function () {{
            document.getElementById('fr-search-text').value = '';
            this.onEnterSearchText();
        }},

        getSearchRanges: function (text, matchCase, wholeWord) {{
            var sel = window.getSelection();
            var ranges = []
            var container = document.getElementsByClassName('{template_FR}-body')[0];
            // find all occurrences in a page
            while (window.find(text, matchCase, false, false, wholeWord, false, false)) {{
                // filter out search results outside of a specific element
                if (container.contains(sel.anchorNode)) {{
                    ranges.push(sel.getRangeAt(sel.rangeCount - 1));
                }}
            }}
            return ranges
        }},

        findNext: function (index, text, matchCase, wholeWord, removeHighlight) {{
            var container = this._findContainer();
            // selection object
            var sel = window.getSelection();

            sel.collapse(container, 0)
            var ranges = this.getSearchRanges(text, matchCase, wholeWord);
            sel.collapse(container, 0);

            if (ranges.length == 0) {{
                return false;
            }}
            else if ((index < ranges.length && index >= 0) || removeHighlight) {{
                if (!removeHighlight) {{
                    ranges.sort((a, b) => {{
                        return a.startContainer.parentElement.getBoundingClientRect().top - b.startContainer.parentElement.getBoundingClientRect().top
                    }});
                    ranges = [ranges[index]];
                }}
                else {{
                    ranges = ranges.filter((r) => r.startContainer.parentElement.classList.contains('search-highlight'));
                }}

                for (var i = 0; i < ranges.length; i++) {{
                    var range = ranges[i]
                    if (range.startContainer == range.endContainer) {{
                        // Range includes just one node
                        if (removeHighlight)
                            this.clearHighlight(range)
                        else
                            this.highlight(range)
                        return true
                    }} else {{
                        // More complex case: range includes multiple nodes
                        // Get all the text nodes in the range
                        var textNodes = this.getTextNodesInRange(
                            range.commonAncestorContainer,
                            range.startContainer,
                            range.endContainer)

                        var startOffset = range.startOffset
                        var endOffset = range.endOffset
                        for (var j = 0; j < textNodes.length; j++) {{
                            var node = textNodes[j]
                            range.setStart(node, j == 0 ? startOffset : 0)
                            range.setEnd(node, j == textNodes.length - 1 ?
                                endOffset : node.nodeValue.length)
                            if (removeHighlight)
                                this.clearHighlight(range);
                            else
                                this.highlight(range);
                        }}
                    }}
                    return true;
                }}

            }}
            return false;
        }},

        search: function (backward) {{
            var searchText = document.getElementById('fr-search-text').value;
            var lastSearchText = sessionStorage.getItem('fastreport-search-text');
            var index = sessionStorage.getItem('fastreport-search-index');
            var matchCase = document.getElementById('fr-match-case').checked;
            var wholeWord = document.getElementById('fr-whole-word').checked;
            document.getElementById('fr-WebRepot-text-info').innerText = '';
            if (!index)
                index = -1

            if (lastSearchText) {{
                this.findNext(index, lastSearchText, sessionStorage.getItem('fastreport-search-match-case') === 'true', sessionStorage.getItem('fastreport-search-whole-word') === 'true', true)
            }}

            if (backward)
                index--
            else
                index++

            if (lastSearchText != searchText)
                index = 0;

            if(!this.findNext(index, searchText, matchCase, wholeWord, false))
            {{   
                var that = this;
                var body = this._findBody();
                var container = this._findContainer();
                // search on next pages
                this._fetch({{
                    method: 'POST',
                    url: '{template_ROUTE_BASE_PATH}/preview.getReport?reportId={ID}&skipPrepare=yes&renderBody=yes&backward=' + backward + '&searchText=' + searchText+ '&matchCase=' + matchCase+ '&wholeWord=' + wholeWord,
                    onSend: function () {{
                        that._activateSpinner();
                    }},
                    onSuccess: function (xhr) {{
                        container.outerHTML = xhr.responseText;
                        that._execScripts();
                        // get new container
                        container = that._findContainer();
                        var sel = window.getSelection();
                        sel.collapse(container, 0);
                        index = backward ?  that.getSearchRanges(searchText, matchCase, wholeWord).length - 1 : 0;
                        sessionStorage.setItem('fastreport-search-index', index);
                        document.getElementById('{template_FR}-toolbar-search-form').classList.toggle('open');
                        document.getElementById('fr-search-text').value = searchText;
                        that.onEnterSearchText();
                        if (!that.findNext(index, searchText, matchCase, wholeWord, false))
                            document.getElementById('fr-WebRepot-text-info').innerText = '{localization.searchNotFound}';
                    }},
                    onError: function (xhr) {{
                        that._deactivateSpinner();
                        index = backward ? 0 : index - 1;
                        sessionStorage.setItem('fastreport-search-index', index);
                        that.findNext(index, searchText, matchCase, wholeWord, false)
                        document.getElementById('fr-WebRepot-text-info').innerText = '{localization.searchNotFound}';
                    }}
                }});
            }}
            sessionStorage.setItem('fastreport-search-index', index);
            sessionStorage.setItem('fastreport-search-text', searchText);
            sessionStorage.setItem('fastreport-search-match-case', matchCase);
            sessionStorage.setItem('fastreport-search-whole-word', wholeWord);
        }},

         highlight: function (range) {{
            var newNode = document.createElement('span');
            newNode.className = 'search-highlight';
            range.surroundContents(newNode);
            const rect = newNode.getBoundingClientRect();
            const vWidth = window.innerWidth || doc.documentElement.clientWidth;
            const vHeight = window.innerHeight || doc.documentElement.clientHeight;

            // Check if the element is out of bounds
            if (rect.right < 0 || rect.bottom < 0 || rect.left > vWidth || rect.top > vHeight) {{
                newNode.scrollIntoView();
            }}
        }},

        clearHighlight: function (range) {{
            var selection = document.getSelection()
            selection.removeAllRanges()
            selection.addRange(range)
            const selParent = selection.anchorNode?.parentElement;
            const selectedElem = selParent?.nodeType == 1 && selParent?.children.length < 2 && selParent;
            if (selectedElem.tagName === 'SPAN' && selectedElem.classList.contains('search-highlight')) {{
                selectedElem.previousSibling.nodeValue += selectedElem.innerText;
                selectedElem.previousSibling.nodeValue += selectedElem.nextSibling.nodeValue;
                selectedElem.parentNode.removeChild(selectedElem.nextSibling);
                selectedElem.parentNode.removeChild(selectedElem);
            }}
        }},

        getTextNodesInRange: function (rootNode, firstNode, lastNode) {{
            var nodes = []
            var startNode = null, endNode = lastNode
            var walker = document.createTreeWalker(
                    rootNode,
                    // search for text nodes
                    NodeFilter.SHOW_TEXT,
                    // Logic to determine whether to accept, reject or skip node.
                    // In this case, only accept nodes that are between
                    // <code>firstNode</code> and <code>lastNode</code>
                    {{
                        acceptNode: function(node)
                        {{
                            if (!startNode) {{
                                if (firstNode == node){{
                                    startNode = node
                                    return NodeFilter.FILTER_ACCEPT
                                }}
                                return NodeFilter.FILTER_REJECT
                            }}

                            if (endNode)
                            {{
                                if (lastNode == node){{
                                    endNode = null
                                }}
                                return NodeFilter.FILTER_ACCEPT
                            }}

                            return NodeFilter.FILTER_REJECT
                         }}
                    }},
                    false
                )

            while(walker.nextNode()){{
                nodes.push(walker.currentNode)
            }}
            return nodes
        }},

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

    load: function () {{
        this._reload();
    }},

    refresh: function () {{
        var that = this;
        var searchForm = document.getElementById('{template_FR}-toolbar-search-form');
        var needRestoreSearch = false;
        var searchText = sessionStorage.getItem('fastreport-search-text');
        var matchCase = sessionStorage.getItem('fastreport-search-match-case') === 'true';
        var wholeWord = sessionStorage.getItem('fastreport-search-whole-word') === 'true';

        if (searchForm) {{
            needRestoreSearch = searchForm.classList.contains('open');
            this.saveSearchFormState();
        }}   
        this._reloadBase('&forceRefresh=yes', null,{{
             onSuccess: function (xhr) {{
                if(needRestoreSearch){{
                    that.restoreSearchFormState(false, true);
                    that.findNext(
                        sessionStorage.getItem('fastreport-search-index'),
                        searchText,
                        matchCase,
                        wholeWord,
                        false
                    );
                }}
            }}
        }});
    }},

    zoom: function (value) {{
        var that = this;
        var searchForm = document.getElementById('{template_FR}-toolbar-search-form');
        var needRestoreSearch = false;
        var searchText = sessionStorage.getItem('fastreport-search-text');
        var matchCase = sessionStorage.getItem('fastreport-search-match-case') === 'true';
        var wholeWord = sessionStorage.getItem('fastreport-search-whole-word') === 'true';

        if (searchForm) {{
            needRestoreSearch = searchForm.classList.contains('open');
            this.saveSearchFormState();
        }}   
        this._closeDropdowns();
        this._reloadBase('&skipPrepare=yes&zoom=' + value, null,{{
             onSuccess: function (xhr) {{
                if(needRestoreSearch){{
                    that.restoreSearchFormState(false, true);
                    that.findNext(
                        sessionStorage.getItem('fastreport-search-index'),
                        searchText,
                        matchCase,
                        wholeWord,
                        false
                    );
                }}
            }}
        }});
    }},

    goto: function (page) {{
        this._reload('&skipPrepare=yes&goto=' + page);
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

    customMethodInvoke: function(elementId, inputValue){{
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();

        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/preview.toolbarElementClick?reportId={ID}&elementId=' + elementId + '&inputValue=' + inputValue,
            onSend: function () {{
                that._activateSpinner();
            }},
            onSuccess: function (xhr) {{
                container.outerHTML = xhr.responseText;
                that._execScripts();
            }},
            onError: function (xhr) {{
                that._placeError(xhr, body);
                that._deactivateSpinner();
            }}
        }});
    }},


    settab: function (tab) {{
        this._reload('&skipPrepare=yes&settab=' + tab);
    }},

    closetab: function (tab) {{
        this._reload('&skipPrepare=yes&closetab=' + tab);
    }},

    _reload: function (params, form) {{
        var that = this;
        var searchForm = document.getElementById('{template_FR}-toolbar-search-form');
        var needRestoreSearch = false;

        if (searchForm) {{
            needRestoreSearch = searchForm.classList.contains('open');
            this.saveSearchFormState();
        }}        

        this._reloadBase(params, form, {{
             onSuccess: function (xhr) {{
                if(needRestoreSearch)
                    that.restoreSearchFormState(true, true);
            }}
        }});
    }},

    _reloadBase: function (params, form, options) {{
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();
        var onSuccess = options.onSuccess;
        var onError = options.onError;
        var onSend = options.onSend;
        var onFinally = options.onFinally;

        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/preview.getReport?reportId={ID}&renderBody=yes' + (params || ''),
            form: form,
            onSend: function () {{
                that._activateSpinner();
                //that._lockToolbar();
                if (typeof onSend === 'function') {{
                    onSend(xhr);
                }}
            }},
            onSuccess: function (xhr) {{
                container.outerHTML = xhr.responseText;
                that._execScripts();
                if (typeof onSuccess === 'function') {{
                    onSuccess(xhr);
                }}
            }},
            onError: function (xhr) {{
                that._placeError(xhr, body);
                that._deactivateSpinner();
                if (typeof onError === 'function') {{
                    onError(xhr);
                }}
            }},
            onFinally: function () {{
                //that._unlockToolbar();
                if (typeof onFinally === 'function') {{
                    onFinally(xhr);
                }}
            }}
        }});
    }},

    {SILENT_RELOAD}: function (params, form) {{
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();
        var searchForm = document.getElementById('{template_FR}-toolbar-search-form');
        var needRestoreSearch = false;
        var searchText = sessionStorage.getItem('fastreport-search-text');
        var matchCase = sessionStorage.getItem('fastreport-search-match-case') === 'true';
        var wholeWord = sessionStorage.getItem('fastreport-search-whole-word') === 'true';

        if (searchForm) {{
            needRestoreSearch = searchForm.classList.contains('open');
            this.saveSearchFormState();
        }}   

        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/preview.getReport?reportId={ID}&renderBody=yes' + (params || ''),
            form: form,
            onSuccess: function (xhr) {{
                container.outerHTML = xhr.responseText;
                that._execScripts();
                if(needRestoreSearch){{
                    that.restoreSearchFormState(false, true);
                    that.findNext(
                        sessionStorage.getItem('fastreport-search-index'),
                        searchText,
                        matchCase,
                        wholeWord,
                        false
                    );
                }}
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
    
    showEmailExportModal: function() {{
        var modalcontainer = this._findModalContainer();
        const emailExportLink = document.getElementById('emailexport');
        const buttons = document.querySelectorAll('.fr-webreport-settings-btn');
        const Overlay = document.querySelector('.modalcontainer-overlay');
        var that = this;

        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/exportsettings.getSettings?reportId={ID}&format=email',
            onSuccess: function (xhr) {{
                modalcontainer.innerHTML = xhr.responseText;
                that._execModalScripts();
                document.querySelector(`[data-target=email]`).classList.add('modalcontainer--visible');
                Overlay.classList.add('modalcontainer-overlay--visible');
            }},
        }})
    }},

showPopup: function (message, isSuccess) {{
    var popup = document.createElement(""div"");
    popup.className = ""fr-notification"";
    if (isSuccess) {{
        popup.classList.add(""positive"");
    }} else {{
        popup.classList.add(""negative"");
    }}

    var content = document.createElement(""div"");
    content.className = ""fr-notification-content"";

    var image = document.createElement(""img"");
    image.src = ""/_fr/resources.getResource?resourceName=notification-bell.svg&contentType=image%2Fsvg%2Bxml"";

    var text = document.createElement(""div"");
    text.innerText = message;

    content.appendChild(image);
    content.appendChild(text);
    popup.appendChild(content);
    document.body.appendChild(popup);

    setTimeout(function () {{
        popup.style.opacity = ""0"";
        setTimeout(function () {{
            popup.remove();
        }}, 500);
    }}, 2000);
}},

    getExportSettings: function () {{
        this._getExportSettings();
    }},

    _getExportSettings: function (params, form) {{
        var modalcontainer = this._findModalContainer();
        const buttons = document.querySelectorAll('.fr-webreport-settings-btn');
        const Overlay = document.querySelector('.modalcontainer-overlay');
        var fileformat;
        var that = this;
        buttons.forEach((el) => {{
        el.addEventListener('click', (e) => {{
        fileformat = e.currentTarget.getAttribute('data-path');
       
        this._fetch({{
            method: 'POST',
            url: '{template_ROUTE_BASE_PATH}/exportsettings.getSettings?reportId={ID}&format='+ fileformat + (params || ''),
            form: form,
            onSuccess: function (xhr) {{
                modalcontainer.innerHTML = xhr.responseText;
                that._execModalScripts();
                document.querySelector(`[data-target=${{fileformat}}]`).classList.add('modalcontainer--visible'); 
                Overlay.classList.add('modalcontainer-overlay--visible');
            }},
        }});
       }})
      }});
    }},

    _execScripts: function () {{
        var container = this._findContainer();
        var scripts = container.getElementsByTagName('script');
        for (var i = 0; i < scripts.length; i++) {{
            eval(scripts[i].text);
        }}
    
    }},
    _execModalScripts: function() {{
        var includeContainer = this._findModalContainer();
        var scripts = includeContainer.getElementsByTagName('script');
        for(var i = 0; i<scripts.length; i++) {{
            var script = document.createElement('script');
            if(scripts[i].text) {{
                script.text = scripts[i].text;
            }} else {{
                for(var j = 0; j<scripts[i].attributes.length; j++) {{
                    if(scripts[i].attributes[j].name in HTMLScriptElement.prototype) {{
                        script[scripts[i].attributes[j].name] = scripts[i].attributes[j].value;
                    }}
                }}
            }}
            scripts[i].parentNode.replaceChild(script, scripts[i]);
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

    _findModalContainer: function () {{
        return document.getElementsByClassName('content-modalcontainer')[0];
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
}