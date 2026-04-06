import '__EXPORT_SETTINGS__';
import { createLisners, IsValid } from './ExportScripts/export-utils.js';
import { HttpClient } from './httpclient.js';
import { Searcher as search } from './searcher.js';

class WebReport {
    ID = ``;
    route_base_path = ``;
    Outline;
    Searcher;
    client;
    observer;

    constructor(props) {
        this.ID = props.ID;
        this.route_base_path = props.RouteBasePath;
        this.Outline = props.Outline;
        this.Searcher = new search(this);
        search.ScrollOffsetTop = props.SearchScroolOffset;
        this.client = new HttpClient();
    };

    static Init() {
        var webReports = document.getElementsByClassName('webreport-script');
        for (var i = 0; i < webReports.length; i++) {
            var props = JSON.parse(webReports[i].dataset.config);
            var report = new WebReport(props);
            if (!window.Webreports || !window.Webreports.has(props.ID)) {
                window.Webreports = new Map();
                report.doc = document;
                window.Webreports.set(props.ID, report);
                report.load();
            }
        }
    }

    load() {
        this._reload();
    }

    initialize() {
        var calendars = document.getElementsByClassName('fr-monthcalendar');
        for (var i = 0; i < calendars.length; i++) {
            var props = JSON.parse(calendars[i].dataset.props);
            var elem = document.getElementById(props.ID);
            $(elem).datepicker();
            $(elem).datepicker("option", "dateFormat", props.DataFormat);
            $(elem).datepicker("setDate", props.SelectedDate, props.DataFormat);
        }

        this.initEventLiseners();
        this.getExportSettings();
        if (window.Split && this.Outline)
            this.outline();
        if ('MutationObserver' in window) {

            const callback = (mutationList) => {
                var container = this._findModalContainer();
                for (const mutation of mutationList) {
                    if (mutation.type == "childList" && mutation.target == container) {
                        if (!IsValid())
                            return;
                        createLisners();
                        this.initEventLiseners(true);
                    }
                }
            };

            this.observer = new MutationObserver(callback);

            const targetNode = document.querySelector(`#modalcontainers`);
            if (targetNode) {
                this.observer.observe(targetNode, {
                    childList: true,
                    subtree: true,
                    attributes: true,
                    characterData: true,
                    attributeOldValue: true,
                    characterDataOldValue: true
                });
            }
        }
    }

    initEventLiseners(inModalContainer) {
        // Create events from attached data to element
        var body = inModalContainer ? this._findModalContainer() : this._findContainer();
        const elemWithEvent = body.querySelectorAll('[data-event]');
        const contextMap = new Map();
        contextMap.set(`window.Webreports.get('${this.ID}')`, this);
        contextMap.set(`window.Webreports.get('${this.ID}').Searcher`, this.Searcher);
        contextMap.set(`frActions`, window.frActions);
        contextMap.set(`window`, window);

        for (var i = 0; i < elemWithEvent.length; i++) {
            let elem = elemWithEvent[i];
            let event = JSON.parse(elem.dataset.event);
            elem.addEventListener(event.Event, () => {
                if (contextMap.has(event.TargetObj)) {
                    var params = event.Params.map(param => {
                        return this.convertToObject(param, elem, contextMap);
                    });
                    var obj = contextMap.get(event.TargetObj);
                    obj[event.Func](...params);
                }
                else {
                    console.log(`Object ${event.TargetObj} not found`);
                }
            })
        }
    };

    convertToObject(param, elem, contextMap) {
        param = param.trim();
        if (param === 'true') return true;
        if (param === 'false') return false;
        if (param === 'null') return null;
        if (param.includes('this')) {
            let concatedStr = param.split('+').map(par => {
                par = par.trim();
                if (par === 'this.value.replace(/\\r?\\n/g, \'\\r\\n\')') return elem.value.replace(/\r?\n/g, '\r\n');
                if (par === 'this.value') return elem.value;
                if (par === 'this.checked') return elem.checked;
                if (par === 'this.selectedIndex') return elem.selectedIndex;
                if (par === 'this') return elem;
                return par.replace(new RegExp('^[\']+|[\']+$', 'g'), '');
            });
            if (concatedStr.length > 1)
                return concatedStr.join('');
            return concatedStr[0];
        }
        if (contextMap.has(param)) return contextMap.get(param);
        if (!isNaN(param) && param !== '') return Number(param);
        return param.replace(new RegExp('^[\']+|[\']+$', 'g'), '');
    }

    outline() {
        var sizes = sessionStorage.getItem(`fastreport-outline-split-sizes`);

        if (sizes) {
            sizes = JSON.parse(sizes);
        } else {
            sizes = [25, 75];
        }

        var that = this;
        var split = window.Split([`.fr-outline`, `.fr-report`], {
            sizes: sizes,
            minSize: [0, 50],
            snapOffset: 20,
            onDragEnd() {
                sessionStorage.setItem(`fastreport-outline-split-sizes`, JSON.stringify(split.getSizes()));
            },
            elementStyle(dimension, size, gutterSize) {
                return {
                    "flex-basis": `calc(` + size + `% - ` + gutterSize + `px)`
                }
            },
            gutterStyle(dimension, gutterSize) {
                return {
                    "flex-basis": gutterSize + `px`
                }
            },
            gutter(index, direction) {
                var gutter = document.createElement(`div`);
                gutter.className = `fr-gutter fr-gutter-` + direction;
                return gutter;
            }
        });

        var tree = sessionStorage.getItem(`fastreport-outline-tree`);
        if (tree) {
            tree = JSON.parse(tree);
            var that = this;
            var container = this._findContainer();
            Object.keys(tree).forEach(function (key) {
                var caret = container.querySelector(`[data-fr-outline-node-id="` + key + `"]`);
                if (caret) {
                    that.outlineOpenNode(caret, true);
                }
            });
        }
    };

    outlineOpenNode(caret, skipTreeStorage) {
        caret.parentNode.parentNode.getElementsByClassName(`fr-outline-children`)[0].style.display = ``;
        caret.parentNode.parentNode.getElementsByClassName(`fr-js-outline-open-node`)[0].style.display = `none`;
        caret.parentNode.parentNode.getElementsByClassName(`fr-js-outline-close-node`)[0].style.display = ``;

        if (skipTreeStorage === true) {
            return;
        }

        var tree = sessionStorage.getItem(`fastreport-outline-tree`);
        if (tree) {
            tree = JSON.parse(tree);
        } else {
            tree = {};
        }

        tree[caret.getAttribute(`data-fr-outline-node-id`)] = true;
        sessionStorage.setItem(`fastreport-outline-tree`, JSON.stringify(tree));
    };

    outlineCloseNode(caret) {
        caret.parentNode.parentNode.getElementsByClassName(`fr-outline-children`)[0].style.display = `none`;
        caret.parentNode.parentNode.getElementsByClassName(`fr-js-outline-open-node`)[0].style.display = ``;
        caret.parentNode.parentNode.getElementsByClassName(`fr-js-outline-close-node`)[0].style.display = `none`;

        var tree = sessionStorage.getItem(`fastreport-outline-tree`);
        if (tree) {
            tree = JSON.parse(tree);
        } else {
            tree = {};
        }

        delete tree[caret.getAttribute(`data-fr-outline-node-id`)];
        sessionStorage.setItem(`fastreport-outline-tree`, JSON.stringify(tree));
    };

    outlineGoto(page, offset, singelPage) {
        if (singelPage) {
            const rect = document.getElementById("PageN" + page).getBoundingClientRect();
            var topOfElement = window.scrollY + rect.top + offset;
            window.scroll({ top: topOfElement, behavior: `smooth` });
        }
        else {
            var that = this;
            this.goto(page, {
                onSuccess(xhr) {
                    that.outlineGoto(1, offset, true);
                }
            });
        }
    };

    refresh() {
        var that = this;
        var searchForm = document.getElementById(`$fr-toolbar-search-form`);
        var needRestoreSearch = false;
        var searchText = sessionStorage.getItem(`fastreport-search-text`);
        var matchCase = sessionStorage.getItem(`fastreport-search-match-case`) === `true`;
        var wholeWord = sessionStorage.getItem(`fastreport-search-whole-word`) === `true`;

        if (searchForm) {
            needRestoreSearch = searchForm.classList.contains(`open`);
            this.Searcher.saveSearchFormState();
        }
        this._reloadBase(`&forceRefresh=yes`, null, {
            onSuccess(xhr) {
                if (needRestoreSearch) {
                    that.Searcher.restoreSearchFormState(false, true);
                    that.Searcher.findNext(
                        sessionStorage.getItem(`fastreport-search-index`),
                        searchText,
                        matchCase,
                        wholeWord,
                        false
                    );
                }
            }
        });
    };

    zoom(value) {
        var that = this;
        var searchForm = document.getElementById(`fr-toolbar-search-form`);
        var needRestoreSearch = false;
        var searchText = sessionStorage.getItem(`fastreport-search-text`);
        var matchCase = sessionStorage.getItem(`fastreport-search-match-case`) === `true`;
        var wholeWord = sessionStorage.getItem(`fastreport-search-whole-word`) === `true`;

        if (searchForm) {
            needRestoreSearch = searchForm.classList.contains(`open`);
            this.Searcher.saveSearchFormState();
        }
        this._closeDropdowns();
        this._reloadBase(`&skipPrepare=yes&zoom=` + value, null, {
            onSuccess(xhr) {
                if (needRestoreSearch) {
                    that.Searcher.restoreSearchFormState(false, true);
                    that.Searcher.findNext(
                        sessionStorage.getItem(`fastreport-search-index`),
                        searchText,
                        matchCase,
                        wholeWord,
                        false
                    );
                }
            }
        });
    };

    goto(page, options) {
        this._reload(`&skipPrepare=yes&goto=` + page, null, options);
    };

    click(el, kind, value) {
        var that = this;

        if (kind == `text_edit`) {
            if (that._win) {
                that._win.close();
            }
            that._win = this._popup(`${this.route_base_path}/preview.textEditForm?reportId=${this.ID}&click=` + value, `Text edit`, 400, 200);
            that._win.onmessage = function (e) {
                if (e.data == `submit`) {
                    var newText = that._win.document.querySelector(`textarea`).value;
                    var form = new FormData();
                    form.append(`text`, newText);
                    that._reload(`&skipPrepare=yes&` + kind + `=` + value, form);
                    that._win.close();
                }
            };
            return;
        }

        this._reload(`&skipPrepare=yes&` + kind + `=` + value);
    };

    customMethodInvoke(elementId, inputValue) {
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();

        this.client.fetch({
            method: `POST`,
            url: `${that.route_base_path}/preview.toolbarElementClick?reportId=${that.ID}&elementId=` + elementId + `&inputValue=` + inputValue,
            onSend() {
                that._activateSpinner();
            },
            onSuccess(xhr) {
                container.outerHTML = xhr.responseText;
                that.initialize();
            },
            onError(xhr) {
                that._placeError(xhr, body);
                that._deactivateSpinner();
            }
        });
    };


    settab(tab) {
        this._reload(`&skipPrepare=yes&settab=` + tab);
    };

    closetab(tab) {
        this._reload(`&skipPrepare=yes&closetab=` + tab);
    };

    _reload(params, form, options = {}) {
        var that = this;
        var onSuccess = options.onSuccess;
        var searchForm = document.getElementById(`fr-toolbar-search-form`);
        var needRestoreSearch = false;

        if (searchForm) {
            needRestoreSearch = searchForm.classList.contains(`open`);
            this.Searcher.saveSearchFormState();
        }

        this._reloadBase(params, form, {
            onSuccess(xhr) {
                if (typeof onSuccess === `function`) {
                    onSuccess(xhr);
                }
                if (needRestoreSearch)
                    that.Searcher.restoreSearchFormState(true, true);
            }
        });
    };

    _reloadBase(params, form, options) {
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();
        var onSuccess = options.onSuccess;
        var onError = options.onError;
        var onSend = options.onSend;
        var onFinally = options.onFinally;
        this.client.fetch({
            method: `POST`,
            url: `${this.route_base_path}/preview.getReport?reportId=${this.ID}&renderBody=yes${(params || ``)}`,
            form: form,
            onSend() {
                that._activateSpinner();
                if (typeof onSend === `function`) {
                    onSend(xhr);
                }
            },
            onSuccess(xhr) {
                container.outerHTML = xhr.responseText;
                that.initialize();
                if (typeof onSuccess === `function`) {
                    onSuccess(xhr);
                }
            },
            onError(xhr) {
                that._placeError(xhr, body);
                that._deactivateSpinner();
                if (typeof onError === `function`) {
                    onError(xhr);
                }
            },
            onFinally() {
                if (typeof onFinally === `function`) {
                    onFinally(xhr);
                }
            }
        });
    };

    _silentReload(params, form) {
        var that = this;
        var body = this._findBody();
        var container = this._findContainer();
        var searchForm = document.getElementById(`fr-toolbar-search-form`);
        var needRestoreSearch = false;
        var searchText = sessionStorage.getItem(`fastreport-search-text`);
        var matchCase = sessionStorage.getItem(`fastreport-search-match-case`) === `true`;
        var wholeWord = sessionStorage.getItem(`fastreport-search-whole-word`) === `true`;

        if (searchForm) {
            needRestoreSearch = searchForm.classList.contains(`open`);
            this.Searcher.saveSearchFormState();
        }

        this.client.fetch({
            method: `POST`,
            url: `${this.route_base_path}/preview.getReport?reportId=${this.ID}&renderBody=yes` + (params || ``),
            form: form,
            onSuccess(xhr) {
                container.outerHTML = xhr.responseText;
                that.initialize();
                if (needRestoreSearch) {
                    that.Searcher.restoreSearchFormState(false, true);
                    that.Searcher.findNext(
                        sessionStorage.getItem(`fastreport-search-index`),
                        searchText,
                        matchCase,
                        wholeWord,
                        false
                    );
                }
            },
            onError(xhr) {
                that._placeError(xhr, body);
            },
        });
    };


    _dialog(params, form) {
        this.client.fetch({
            method: `POST`,
            url: `${this.route_base_path}/dialog?reportId=${this.ID}` + (params || ``),
            form: form
        });
    };

    showEmailExportModal() {
        var modalcontainer = this._findModalContainer();
        const emailExportLink = document.getElementById(`emailexport`);
        const buttons = document.querySelectorAll(`.fr-settings-btn`);
        const Overlay = document.querySelector(`.modalcontainer-overlay`);
        var that = this;

        this.client.fetch({
            method: `POST`,
            url: `${this.route_base_path}/exportsettings.getSettings?reportId=${this.ID}&format=email`,
            onSuccess(xhr) {
                modalcontainer.innerHTML = xhr.responseText;
                that._execModalScripts();
                document.querySelector(`[data-target=email]`).classList.add(`modalcontainer--visible`);
                Overlay.classList.add(`modalcontainer-overlay--visible`);
            },
        })
    };

    showPopup(message, isSuccess) {
        var popup = document.createElement("div");
        popup.className = "fr-notification";
        if (isSuccess) {
            popup.classList.add("positive");
        } else {
            popup.classList.add("negative");
        }

        var content = document.createElement("div");
        content.className = "fr-notification-content";

        var image = document.createElement("img");
        image.src = "/_fr/resources.getResource?resourceName=notification-bell.svg&contentType=image%2Fsvg%2Bxml";

        var text = document.createElement("div");
        text.innerText = message;

        content.appendChild(image);
        content.appendChild(text);
        popup.appendChild(content);
        document.body.appendChild(popup);

        setTimeout(function () {
            popup.style.opacity = "0";
            setTimeout(function () {
                popup.remove();
            }, 500);
        }, 2000)
    };

    getExportSettings() {
        this._getExportSettings();
    };

    _getExportSettings(params, form) {
        var modalcontainer = this._findModalContainer();
        const buttons = document.querySelectorAll(`.fr-settings-btn`);
        const Overlay = document.querySelector(`.modalcontainer-overlay`);
        var fileformat;
        var that = this;
        buttons.forEach((el) => {
            el.addEventListener(`click`, (e) => {
                fileformat = e.currentTarget.getAttribute(`data-path`);

                this.client.fetch({
                    method: `POST`,
                    url: `${that.route_base_path}/exportsettings.getSettings?reportId=${that.ID}&format=` + fileformat + (params || ``),
                    form: form,
                    onSuccess(xhr) {
                        modalcontainer.innerHTML = xhr.responseText;
                        that._execModalScripts();
                        document.querySelector(`[data-target=${fileformat}]`).classList.add(`modalcontainer--visible`);
                        Overlay.classList.add(`modalcontainer-overlay--visible`);
                    },
                });
            })
        });
    };

    _execModalScripts() {
        var includeContainer = this._findModalContainer();
        var scripts = includeContainer.getElementsByTagName(`script`);
        for (var i = 0; i < scripts.length; i++) {
            for (var j = 0; j < scripts[i].attributes.length; j++) {
                if (scripts[i].attributes[j].name in HTMLScriptElement.prototype && scripts[i].attributes[j].name == "src") {
                    try {
                        if (import.meta.hot) {
                            import.meta.hot.invalidate();
                        }

                        const module = import(scripts[i].attributes[j].value.replace("_content/FastReport.Web/js/", ""));
                    } catch (error) {
                        console.error('Error on reloading:', error);
                    }
                }
            }
        }
    };

    _placeError(xhr, body) {
        body.innerHTML = `<div class="fr-error-container"><div class="fr-error-text">Error<br>` + xhr.status + ` - ` + xhr.statusText + `</div><div class="fr-error-response">` + xhr.responseText + `</div></div>`;
    };

    _activateSpinner() {
        document.getElementsByClassName(`fr-report-spinner`)[0].style.display = ``;
    };

    _deactivateSpinner() {
        document.getElementsByClassName(`fr-report-spinner`)[0].style.display = `none`;
    };

    _findContainer() {
        return document.getElementsByClassName(`fr-container`)[0];
    };

    _findModalContainer() {
        return document.getElementsByClassName(`content-modalcontainer`)[0];
    };

    _findBody() {
        return document.getElementsByClassName(`fr-report-body`)[0];
    };

    _closeDropdowns() {
        var dropdowns = document.getElementsByClassName(`fr-dropdown-content`);

        var func = function (dd) {
            setTimeout(function () {
                dd.style[`display`] = ``;
            }, 100);
        }

        for (var i = 0; i < dropdowns.length; i++) {
            var dd = dropdowns[i];
            dd.style[`display`] = `none`;
            func(dd);
        }
    };

    _popup(url, title, w, h) {
        // Fixes dual-screen position                         Most browsers       Firefox
        var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : window.screenX;
        var dualScreenTop = window.screenTop != undefined ? window.screenTop : window.screenY;

        var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        var left = ((width / 2) - (w / 2)) + dualScreenLeft;
        var top = ((height / 2) - (h / 2)) + dualScreenTop;

        var params = `menubar=0, toolbar=0, location=0, status=0, resizable=1, scrollbars=1`;
        var newWindow = window.open(url, title, params + `, width=` + w + `, height=` + h + `, top=` + top + `, left=` + left);

        if (newWindow.focus) {
            newWindow.focus();
        }

        return newWindow;
    }
}

if (window.top !== window.self) {
    const observer = new MutationObserver((mutations) => {
        if (document.readyState === 'complete') {
            var webReports = document.getElementsByClassName('webreport-script');
            if (webReports.length == 0)
                window.Webreports = new Map();
            WebReport.Init();
        }
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true,
    });
}

WebReport.Init();
