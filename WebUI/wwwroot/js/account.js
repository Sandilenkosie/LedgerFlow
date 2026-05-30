(function($){
    // Account-specific interactions: populate details panel and open transaction modal
    $(document).ready(function(){
        // safe fallbacks for helpers provided by person.js
        var showModal = window.showModalElement || function(el){ if (!el) return; try { if (window.jQuery && typeof $(el).modal === 'function') $(el).modal('show'); } catch(e){} };
        var applyActive = window.applyActiveTextStyles || function($el) { if (!$el) return; $el.addClass('active'); };

        $(document).on('click', '.person-item[data-account]', function(e){
            e.preventDefault();
            var $el = $(this);

            // manage active state
            $('.person-item').removeClass('active');
            $el.addClass('active');
            applyActive($el);

            // parse account data
            var accountJson = $el.attr('data-account') || $el.data('account');
            var account = {};
            try { account = (typeof accountJson === 'object') ? accountJson : JSON.parse(accountJson); } catch (ex) { console.error('Failed to parse account data', ex); return; }

            // populate right-side fields
            $('#account-number-title').text(account.accountNumber || account.AccountNumber || '');
            var ownerText = (account.isClosed ? 'Closed' : 'Active') + ' • ' + ((account.person && (account.person.name || account.person.Name)) || '');
            $('#account-owner').text(ownerText);
            $('#account-status-badge').removeClass('bg-success bg-secondary bg-danger');
            $('#account-status-badge').addClass(account.isClosed ? 'bg-secondary' : 'bg-success');

            $('#detail-account-number').text(account.accountNumber || account.AccountNumber || '');
            $('#detail-account-description').text(account.description || account.Description || '');
            $('#detail-account-owner').text((account.person && (account.person.name || account.person.Name)) || '');
            $('#detail-account-balance').text('R ' + ((account.balance !== undefined) ? Number(account.balance).toLocaleString(undefined, {minimumFractionDigits:2, maximumFractionDigits:2}) : '0.00'));
            $('#detail-account-created').text(account.createdAt || account.created || account.CreatedAt || '');

            // set Make Transaction link and hidden input for modal
            // keep Make Transaction link as a modal trigger (href '#') and set hidden account id for the form
            $('#make-transaction-link').attr('href', '#');
            $('#account-id').val(account.id || account.Id || '');

            // delegate transaction rendering to the shared RelatedTransaction.js helper
            try {
                var txs = account.transactions || account.Transactions || [];
                // ensure data-transactions is available for the renderer
                $el.attr('data-transactions', JSON.stringify(txs));
                if (typeof window.renderRelatedTransactions === 'function') {
                    window.renderRelatedTransactions($el);
                } else {
                    // fallback: clear list and show message
                    var $txListFallback = $('#transactions-list');
                    $txListFallback.empty();
                    if (txs && txs.length > 0) {
                        txs.forEach(function(t){
                            var id = t.id || t.Id || '';
                            var title = t.description || t.Description || t.title || t.Title || 'Transaction';
                            var date = t.transactionDate || t.TransactionDate || t.Date || t.date || '';
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
                            $txListFallback.append($a);
                        });
                    } else {
                        $txListFallback.append('<li class="list-group-item text-muted">No transactions available for this account.</li>');
                    }
                }
            }
            catch (ex) {
                console.error('Failed to render transactions for account', ex);
            }

            // show details
            $('#no-selection').addClass('d-none');
            $('#person-details').removeClass('d-none');
        });

        // open add transaction modal when Make Transaction link or buttons target modal
        $(document).on('click', '#make-transaction-link, button[data-bs-target="#addTransactionModal"], button[data-target="#addTransactionModal"]', function(e){
            // allow normal link behavior for href if it's a navigation; if href is #, open modal using hidden input
            var href = $(this).attr('href');
            if (href && href !== '#') {
                // navigate
                return;
            }
            e.preventDefault();
            var acctId = $('#account-id').val() || '';
            // ensure account-id is set before showing modal
            $('#account-id').val(acctId);
            showModal(document.getElementById('addTransactionModal'));
        });
    });
})(jQuery);
