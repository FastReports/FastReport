
export class HttpClient {
    _fetchQueue = []

    fetch(options) {
        const method = options.method;
        const url = options.url;
        let form = options.form;
        const onSuccess = options.onSuccess;
        const onError = options.onError;
        const onSend = options.onSend;
        const onFinally = options.onFinally;

        if (options.method == `POST` && options.form == null) {
            form = {};
        }

        const that = this;
        const xhr = new XMLHttpRequest();
        xhr.__form = form;
        xhr.__onSend = onSend;
        xhr.open(method, url, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState != 4)
                return;

            if (xhr.status != 200) {
                if (typeof onError === `function`) {
                    onError(xhr);
                }
            } else {
                if (typeof onSuccess === `function`) {
                    onSuccess(xhr);
                }
            }

            if (typeof onFinally === `function`) {
                onFinally(xhr);
            }

            that._nextFetch();
        };

        this._fetchQueue.push(xhr);

        if (this._fetchQueue.length == 1) {
            const f = this._fetchQueue[0];
            if (typeof f.__onSend === `function`) {
                f.__onSend(f);
            }
            f.send(f.__form);
        }
    };

    _nextFetch() {
        this._fetchQueue.shift();

        if (this._fetchQueue.length) {
            const f = this._fetchQueue[0];
            if (typeof f.__onSend === `function`) {
                f.__onSend(f);
            }
            f.send(f.__form);
        }
    };
}