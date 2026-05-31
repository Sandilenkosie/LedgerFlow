(function($){
    // Renders related transactions from a clicked person element into #transactions-list
    window.renderRelatedTransactions = function(el) {
        try {
            var $el = (el && el.jquery) ? el : $(el);
            var txJson = $el.attr('data-transactions') || $el.data('transactions');
            var txs = [];
            if (txJson) {
                txs = (typeof txJson === 'object') ? txJson : JSON.parse(txJson);
            }

            var $txList = $('#transactions-list');
            $txList.empty();

            if (txs && !Array.isArray(txs)) {
                if (typeof txs === 'object') txs = Object.values(txs); else txs = [];
            }

            if (txs && txs.length > 0) {
                // add simple timestamp for sorting (newest first)
                txs.forEach(function(t){
                    var d = t.TransactionDate || t.transactionDate || t.Date || t.date || t.TransactionDateString || '';
                    t._ts = Date.parse(d) || 0;
                });
                txs.sort(function(a,b){ return b._ts - a._ts; });

                // render top 3
                for (var i=0; i<txs.length && i<3; i++){
                    var t = txs[i];
                    var id = t.id || t.Id || '';
                    var title = t.title || t.Title || t.description || t.Description || 'Transaction';
                    var date = t.TransactionDate || t.transactionDate || t.date || t.Date || '';
                    var amount = Number(t.amount || t.Amount || 0);

                    var displayDate = date ? (new Date(date)).toLocaleDateString() : '';
                    var isPositive = amount >= 0;
                    var amountText = (isPositive ? '+ ' : '- ') + 'R ' + Math.abs(amount).toLocaleString(undefined, {minimumFractionDigits:2, maximumFractionDigits:2});
                    var amountClass = isPositive ? 'text-success' : 'text-danger';

                    var $li = $('<li>').addClass('list-group-item p-0');
                    var $a = $('<a>').attr('href', '/person/transactions/' + id).addClass('d-flex justify-content-between align-items-start p-2 text-decoration-none w-100');
                    $a.append(
                        $('<div>').append($('<div>').addClass('fw-semibold text-truncate').text(title), $('<small>').addClass('text-muted').text(displayDate)),
                        $('<div>').addClass(amountClass).text(amountText)
                    );
                    $li.append($a);
                    $txList.append($li);
                }
            } else {
                $txList.append('<li class="list-group-item text-muted">No transactions available.</li>');
            }
        }
        catch (ex) {
            console.error('Failed to parse transactions for person', ex);
        }
    };
})(jQuery);
