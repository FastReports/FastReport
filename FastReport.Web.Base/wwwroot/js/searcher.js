export class Searcher {
    static ScrollOffsetTop;
    Webreport;

    constructor(webreport) {
        this.Webreport = webreport;
    }

    toggleSearchForm() {
        var form = document.getElementById(`fr-toolbar-search-form`);
        form.classList.toggle(`open`);
        if (!form.classList.contains(`open`)) {
            var searchText = sessionStorage.getItem(`fastreport-search-text`);
            var matchCase = sessionStorage.getItem(`fastreport-search-match-case`) === `true`;
            var wholeWord = sessionStorage.getItem(`fastreport-search-whole-word`) === `true`;
            this.findNext(
                sessionStorage.getItem(`fastreport-search-index`),
                searchText,
                matchCase,
                wholeWord,
                true
            );
        }
    };

    saveSearchFormState() {
        sessionStorage.setItem(`fastreport-search-text-state`, document.getElementById(`fr-search-text`).value);
        sessionStorage.setItem(`fastreport-search-match-case-state`, document.getElementById(`fr-match-case`).checked);
        sessionStorage.setItem(`fastreport-search-whole-word-state`, document.getElementById(`fr-whole-word`).checked);
    };

    restoreSearchFormState(resetIndex = false, openAfter = false) {
        document.getElementById(`fr-search-text`).value = sessionStorage.getItem(`fastreport-search-text-state`);
        document.getElementById(`fr-match-case`).checked = sessionStorage.getItem(`fastreport-search-match-case-state`) === `true`;
        document.getElementById(`fr-whole-word`).checked = sessionStorage.getItem(`fastreport-search-whole-word-state`) === `true`;
        this.onEnterSearchText();
        if (resetIndex)
            sessionStorage.setItem(`fastreport-search-text`, ``);
        if (openAfter)
            document.getElementById(`fr-toolbar-search-form`).classList.toggle(`open`);
    };

    onEnterSearchText() {
        if (document.getElementById(`fr-search-text`).value == ``) {
            document.getElementById(`fr-search-prev`).setAttribute(`disabled`, `disabled`);
            document.getElementById(`fr-search-next`).setAttribute(`disabled`, `disabled`);
            document.getElementById(`fr-match-case`).setAttribute(`disabled`, `disabled`);
            document.getElementById(`fr-whole-word`).setAttribute(`disabled`, `disabled`);
            document.getElementById(`clear-searchbox`).setAttribute(`hidden`, `hidden`);
        }
        else {
            document.getElementById(`fr-search-prev`).removeAttribute(`disabled`);
            document.getElementById(`fr-search-next`).removeAttribute(`disabled`);
            document.getElementById(`fr-match-case`).removeAttribute(`disabled`);
            document.getElementById(`fr-whole-word`).removeAttribute(`disabled`);
            document.getElementById(`clear-searchbox`).removeAttribute(`hidden`);
        }
    };

    clearSearchText() {
        document.getElementById(`fr-search-text`).value = ``;
        this.onEnterSearchText();
    };

    getSearchRanges(text, matchCase, wholeWord) {
        var curScrollY = window.scrollY;
        var curScrollX = window.scrollX;
        var sel = window.getSelection();
        var ranges = []
        var container = document.getElementsByClassName(`fr-report-body`)[0];
        // find all occurrences in a page
        while (window.find(text, matchCase, false, false, wholeWord, false, false)) {
            // filter out search results outside of a specific element
            if (container.contains(sel.anchorNode)) {
                ranges.push(sel.getRangeAt(sel.rangeCount - 1));
            }
        }
        window.scrollTo(curScrollX, curScrollY);
        return ranges
    };

    findNext(index, text, matchCase, wholeWord, removeHighlight) {
        var container = this.Webreport._findContainer();
        // selection object
        var sel = window.getSelection();

        sel.collapse(container, 0)
        var ranges = this.getSearchRanges(text, matchCase, wholeWord);
        sel.collapse(container, 0);

        if (ranges.length == 0) {
            return false;
        }
        else if ((index < ranges.length && index >= 0) || removeHighlight) {
            if (!removeHighlight) {
                ranges.sort((a, b) => {
                    return a.startContainer.parentElement.getBoundingClientRect().top - b.startContainer.parentElement.getBoundingClientRect().top
                });
                ranges = [ranges[index]];
            }
            else {
                ranges = ranges.filter((r) => r.startContainer.parentElement.classList.contains(`search-highlight`));
            }

            for (var i = 0; i < ranges.length; i++) {
                var range = ranges[i]
                if (range.startContainer == range.endContainer) {
                    // Range includes just one node
                    if (removeHighlight)
                        this.clearHighlight(range)
                    else
                        this.highlight(range)
                    return true
                } else {
                    // More complex case: range includes multiple nodes
                    // Get all the text nodes in the range
                    var textNodes = this.getTextNodesInRange(
                        range.commonAncestorContainer,
                        range.startContainer,
                        range.endContainer)

                    var startOffset = range.startOffset
                    var endOffset = range.endOffset
                    for (var j = 0; j < textNodes.length; j++) {
                        var node = textNodes[j]
                        range.setStart(node, j == 0 ? startOffset : 0)
                        range.setEnd(node, j == textNodes.length - 1 ?
                            endOffset : node.nodeValue.length)
                        if (removeHighlight)
                            this.clearHighlight(range);
                        else
                            this.highlight(range);
                    }
                }
                return true;
            }

        }
        return false;
    };

    search(backward, searchNotFoundText) {
        var searchText = document.getElementById(`fr-search-text`).value;
        var lastSearchText = sessionStorage.getItem(`fastreport-search-text`);
        var index = sessionStorage.getItem(`fastreport-search-index`);
        var matchCase = document.getElementById(`fr-match-case`).checked;
        var wholeWord = document.getElementById(`fr-whole-word`).checked;
        document.getElementById(`fr-searchform-text-info`).innerText = ``;
        if (!index)
            index = -1

        if (lastSearchText) {
            this.findNext(index, lastSearchText, sessionStorage.getItem(`fastreport-search-match-case`) === `true`, sessionStorage.getItem(`fastreport-search-whole-word`) === `true`, true)
        }

        if (backward)
            index--
        else
            index++

        if (lastSearchText != searchText)
            index = 0;

        if (!this.findNext(index, searchText, matchCase, wholeWord, false)) {
            var that = this;
            var container = this.Webreport._findContainer();
            // search on next pages
            this.Webreport.client.fetch({
                method: `POST`,
                url: `${this.Webreport.route_base_path}/preview.getReport?reportId=${this.Webreport.ID}&skipPrepare=yes&renderBody=yes&backward=` + backward + `&searchText=` + searchText + `&matchCase=` + matchCase + `&wholeWord=` + wholeWord,
                onSend() {
                    that.Webreport._activateSpinner();
                },
                onSuccess(xhr) {
                    container.outerHTML = xhr.responseText;
                    that.Webreport.initialize();
                    // get new container
                    container = that.Webreport._findContainer();
                    var sel = window.getSelection();
                    sel.collapse(container, 0);
                    index = backward ? that.getSearchRanges(searchText, matchCase, wholeWord).length - 1 : 0;
                    sessionStorage.setItem(`fastreport-search-index`, index);
                    document.getElementById(`fr-toolbar-search-form`).classList.toggle(`open`);
                    document.getElementById(`fr-search-text`).value = searchText;
                    that.onEnterSearchText();
                    if (!that.findNext(index, searchText, matchCase, wholeWord, false))
                        document.getElementById(`fr-searchform-text-info`).innerText = searchNotFoundText;
                },
                onError(xhr) {
                    that.Webreport._deactivateSpinner();
                    index = backward ? 0 : index - 1;
                    sessionStorage.setItem(`fastreport-search-index`, index);
                    that.findNext(index, searchText, matchCase, wholeWord, false)
                    document.getElementById(`fr-searchform-text-info`).innerText = searchNotFoundText;
                }
            });
        }
        sessionStorage.setItem(`fastreport-search-index`, index);
        sessionStorage.setItem(`fastreport-search-text`, searchText);
        sessionStorage.setItem(`fastreport-search-match-case`, matchCase);
        sessionStorage.setItem(`fastreport-search-whole-word`, wholeWord);
    };

    highlight(range) {
        var newNode = document.createElement(`span`);
        newNode.className = `search-highlight`;
        range.surroundContents(newNode);
        const rect = newNode.getBoundingClientRect();
        const vWidth = (window.innerWidth || document.documentElement.clientWidth) - rect.width;
        const vHeight = (window.innerHeight || document.documentElement.clientHeight) - rect.height;
        var topOfElement = window.scrollY;
        var leftOfElement = window.screenX;

        if (rect.bottom < rect.height || rect.top > vHeight)
            topOfElement = topOfElement + rect.top - Searcher.ScrollOffsetTop;
        if (rect.right < rect.width || rect.left > vWidth)
            leftOfElement = leftOfElement + rect.left;
        window.scroll({ top: topOfElement, left: leftOfElement, behavior: 'smooth' });
    };

    clearHighlight(range) {
        var selection = document.getSelection()
        selection.removeAllRanges()
        selection.addRange(range)
        const selParent = selection.anchorNode?.parentElement;
        const selectedElem = selParent?.nodeType == 1 && selParent?.children.length < 2 && selParent;
        if (selectedElem.tagName === `SPAN` && selectedElem.classList.contains(`search-highlight`)) {
            selectedElem.previousSibling.nodeValue += selectedElem.innerText;
            selectedElem.previousSibling.nodeValue += selectedElem.nextSibling.nodeValue;
            selectedElem.parentNode.removeChild(selectedElem.nextSibling);
            selectedElem.parentNode.removeChild(selectedElem);
        }
    };

    getTextNodesInRange(rootNode, firstNode, lastNode) {
        var nodes = []
        var startNode = null, endNode = lastNode
        var walker = document.createTreeWalker(
            rootNode,
            // search for text nodes
            NodeFilter.SHOW_TEXT,
            // Logic to determine whether to accept, reject or skip node.
            // In this.Webreport case, only accept nodes that are between
            // <code>firstNode</code> and <code>lastNode</code>
            {
                acceptNode(node) {
                    if (!startNode) {
                        if (firstNode == node) {
                            startNode = node
                            return NodeFilter.FILTER_ACCEPT
                        }
                        return NodeFilter.FILTER_REJECT
                    }

                    if (endNode) {
                        if (lastNode == node) {
                            endNode = null
                        }
                        return NodeFilter.FILTER_ACCEPT
                    }

                    return NodeFilter.FILTER_REJECT
                }
            },
            false)

        while (walker.nextNode()) {
            nodes.push(walker.currentNode)
        }
        return nodes
    };
}
