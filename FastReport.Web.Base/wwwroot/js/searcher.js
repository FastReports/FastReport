export class Searcher {
    static ScrollOffsetTop;
    Webreport;

    constructor(webreport) {
        this.Webreport = webreport;
    }

    toggleSearchForm() {
        const form = document.getElementById(`fr-toolbar-search-form`);
        form.classList.toggle(`open`);
        if (!form.classList.contains(`open`)) {
            const searchText = sessionStorage.getItem(`fastreport-search-text`);
            const matchCase = sessionStorage.getItem(`fastreport-search-match-case`) === `true`;
            const wholeWord = sessionStorage.getItem(`fastreport-search-whole-word`) === `true`;
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
        const curScrollY = window.scrollY;
        const curScrollX = window.scrollX;
        const sel = globalThis.getSelection();
        const ranges = []
        const container = document.getElementsByClassName(`fr-report-body`)[0];
        // find all occurrences in a page
        while (globalThis.find(text, matchCase, false, false, wholeWord, false, false)) {
            // filter out search results outside of a specific element
            if (container.contains(sel.anchorNode)) {
                ranges.push(sel.getRangeAt(sel.rangeCount - 1));
            }
        }
        window.scrollTo(curScrollX, curScrollY);
        return ranges
    };

    findNext(index, text, matchCase, wholeWord, removeHighlight) {
        const container = this.Webreport._findContainer();
        const sel = globalThis.getSelection();

        sel.collapse(container, 0);
        let ranges = this.getSearchRanges(text, matchCase, wholeWord, container);
        sel.collapse(container, 0);

        if (ranges.length === 0) return false;

        const targetRanges = this._getTargetRanges(ranges, index, removeHighlight);
        if (targetRanges.length === 0) return false;

        const range = targetRanges[0];
        this._processRange(range, removeHighlight);

        return true;
    }

    _getTargetRanges(ranges, index, removeHighlight) {
        if (removeHighlight) {
            return ranges.filter(r => r.startContainer.parentElement.classList.contains('search-highlight'));
        }

        if (index >= 0 && index < ranges.length) {
            const sortedRanges = [...ranges].sort((a, b) => {
                return a.startContainer.parentElement.getBoundingClientRect().top -
                    b.startContainer.parentElement.getBoundingClientRect().top;
            });
            return [sortedRanges[index]];
        }

        return [];
    }

    _processRange(range, removeHighlight) {
        if (range.startContainer === range.endContainer) {
            this._applyHighlight(range, removeHighlight);
            return;
        }

        const textNodes = this.getTextNodesInRange(
            range.commonAncestorContainer,
            range.startContainer,
            range.endContainer
        );

        const startOffset = range.startOffset;
        const endOffset = range.endOffset;

        for (let j = 0; j < textNodes.length; j++) {
            const node = textNodes[j];
            const isFirst = j === 0;
            const isLast = j === textNodes.length - 1;

            range.setStart(node, isFirst ? startOffset : 0);
            range.setEnd(node, isLast ? endOffset : node.nodeValue.length);

            this._applyHighlight(range, removeHighlight);
        }
    }

    _applyHighlight(range, removeHighlight) {
        if (removeHighlight) {
            this.clearHighlight(range);
        } else {
            this.highlight(range);
        }
    }

    search(backward, searchNotFoundText) {
        const searchText = document.getElementById('fr-search-text').value;
        const lastSearchText = sessionStorage.getItem('fastreport-search-text');
        let index = this._getStoredIndex();
        const matchCase = document.getElementById(`fr-match-case`).checked;
        const wholeWord = document.getElementById(`fr-whole-word`).checked;
        document.getElementById(`fr-searchform-text-info`).innerText = ``;

        this._clearPreviousHighlights(lastSearchText);

        index = this._calculateNewIndex(index, backward, lastSearchText, searchText);

        if (!this.findNext(index, searchText, matchCase, wholeWord, false)) {
            this._handleNotFound(searchText, backward, matchCase, wholeWord, index, searchNotFoundText);
        }

        this._storeSearchState(index, searchText, matchCase, wholeWord);
    }

    _getStoredIndex() {
        const index = sessionStorage.getItem('fastreport-search-index');
        return index ? Number.parseInt(index) : -1;
    }

    _clearPreviousHighlights(lastSearchText) {
        if (lastSearchText) {
            this.findNext(
                -1,
                lastSearchText,
                sessionStorage.getItem('fastreport-search-match-case') === 'true',
                sessionStorage.getItem('fastreport-search-whole-word') === 'true',
                true
            );
        }
    }

    _calculateNewIndex(currentIndex, backward, lastSearchText, searchText) {
        let newIndex = currentIndex;

        if (backward) {
            newIndex--;
        } else {
            newIndex++;
        }

        if (lastSearchText !== searchText) {
            newIndex = 0;
        }

        return newIndex;
    }

    _handleNotFound(searchText, backward, matchCase, wholeWord, currentIndex, searchNotFoundText) {
        const that = this;
        let container = this.Webreport._findContainer();
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
                const sel = globalThis.getSelection();
                sel.collapse(container, 0);
                const index = backward ? that.getSearchRanges(searchText, matchCase, wholeWord).length - 1 : 0;
                sessionStorage.setItem(`fastreport-search-index`, index);
                document.getElementById(`fr-toolbar-search-form`).classList.toggle(`open`);
                document.getElementById(`fr-search-text`).value = searchText;
                that.onEnterSearchText();
                if (!that.findNext(index, searchText, matchCase, wholeWord, false))
                    document.getElementById(`fr-searchform-text-info`).innerText = searchNotFoundText;
            },
            onError(xhr) {
                that.Webreport._deactivateSpinner();
                const index = backward ? 0 : currentIndex - 1;
                sessionStorage.setItem(`fastreport-search-index`, index);
                that.findNext(index, searchText, matchCase, wholeWord, false)
                document.getElementById(`fr-searchform-text-info`).innerText = searchNotFoundText;
            }
        });

    }

    _storeSearchState(index, searchText, matchCase, wholeWord) {
        sessionStorage.setItem('fastreport-search-index', index);
        sessionStorage.setItem('fastreport-search-text', searchText);
        sessionStorage.setItem('fastreport-search-match-case', matchCase);
        sessionStorage.setItem('fastreport-search-whole-word', wholeWord);
    }

    highlight(range) {
        const newNode = document.createElement(`span`);
        newNode.className = `search-highlight`;
        range.surroundContents(newNode);
        const rect = newNode.getBoundingClientRect();
        const vWidth = (window.innerWidth || document.documentElement.clientWidth) - rect.width;
        const vHeight = (window.innerHeight || document.documentElement.clientHeight) - rect.height;
        let topOfElement = window.scrollY;
        let leftOfElement = window.screenX;

        if (rect.bottom < rect.height || rect.top > vHeight)
            topOfElement = topOfElement + rect.top - Searcher.ScrollOffsetTop;
        if (rect.right < rect.width || rect.left > vWidth)
            leftOfElement = leftOfElement + rect.left;
        window.scroll({ top: topOfElement, left: leftOfElement, behavior: 'smooth' });
    };

    clearHighlight(range) {
        const selection = document.getSelection()
        selection.removeAllRanges()
        selection.addRange(range)
        const selParent = selection.anchorNode?.parentElement;
        const selectedElem = selParent?.nodeType == 1 && selParent?.children.length < 2 && selParent;
        if (selectedElem.tagName === `SPAN` && selectedElem.classList.contains(`search-highlight`)) {
            selectedElem.previousSibling.nodeValue += selectedElem.innerText;
            selectedElem.previousSibling.nodeValue += selectedElem.nextSibling.nodeValue;
            selectedElem.nextSibling.remove();
            selectedElem.remove();
        }
    };

    getTextNodesInRange(rootNode, firstNode, lastNode) {
        const nodes = []
        let startNode = null, endNode = lastNode
        const walker = document.createTreeWalker(
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
