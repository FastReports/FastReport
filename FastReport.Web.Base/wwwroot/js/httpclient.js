
export class HttpClient {
    _fetchQueue = []

     fetch(options) {
        var method = options.method;
        var url = options.url;
        var form = options.form;
        var onSuccess = options.onSuccess;
        var onError = options.onError;
        var onSend = options.onSend;
        var onFinally = options.onFinally;

        if (options.method == `POST` && options.form == null) {
            form = {};
        }

        var that = this;
        var xhr = new XMLHttpRequest();
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
            var f = this._fetchQueue[0];
            if (typeof f.__onSend === `function`) {
                f.__onSend(f);
            }
            f.send(f.__form);
        }
    };

    _nextFetch() {
        this._fetchQueue.shift();

        if (this._fetchQueue.length) {
            var f = this._fetchQueue[0];
            if (typeof f.__onSend === `function`) {
                f.__onSend(f);
            }
            f.send(f.__form);
        }
    };
}