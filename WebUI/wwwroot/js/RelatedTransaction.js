(function($){
    // Renders related transactions from a clicked person element into #transactions-list
    window.renderRelatedTransactions = function(el) {
        try {
            var $el = $(el);
            var txJson = $el.attr('data-transactions') || $el.data('transactions');
            var txs = [];
            if (txJson) {
                if (typeof txJson === 'object') {
                    txs = txJson;
                } else {
                    txs = JSON.parse(txJson);
                }
            }

            var $txList = $('#transactions-list');
            $txList.empty();

            if (txs && txs.length > 0) {
                txs.forEach(function(t){
                    var id = t.id || t.Id || '';
                    var title = t.title || t.Title || t.description || t.Description || 'Transaction';
                    var date = t.date || t.Date || '';
                    var amount = t.amount || t.Amount || 0;

                    var displayDate = date ? (new Date(date)).toLocaleDateString() : '';

                    var isPositive = Number(amount) >= 0;
                    var amountText = (isPositive ? '+ ' : '- ') + 'R ' + Math.abs(Number(amount || 0)).toLocaleString(undefined, {minimumFractionDigits:2, maximumFractionDigits:2});
                    var amountClass = isPositive ? 'text-success' : 'text-danger';

                    var $a = $("<a>")
                        .attr('href', '/Transaction/Details/' + id)
                        .addClass('list-group-item list-group-item-action d-flex justify-content-between align-items-start')
                        .append(
                            $('<div>').append(
                                $('<div>').addClass('fw-semibold').text(title),
                                $('<small>').addClass('text-muted').text(displayDate)
                            ),
                            $('<div>').addClass(amountClass).text(amountText)
                        );

                    $txList.append($a);
                });
            } else {
                $txList.append('<li class="list-group-item text-muted">No transactions available.</li>');
            }
        }
        catch (ex) {
            console.error('Failed to parse transactions for person', ex);
        }
    };
})(jQuery);
