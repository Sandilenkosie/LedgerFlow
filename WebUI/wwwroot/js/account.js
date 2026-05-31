(function($){
    $(function(){
        function parseAccount(data){
            if (!data) return {};
            if (typeof data === 'object') return data;
            try { return JSON.parse(data); } catch (e) { return {}; }
        }

        $(document).on('click', '.person-item[data-account]', function(e){
            e.preventDefault();
            var $item = $(this);
            $('.person-item').removeClass('active');
            $item.addClass('active');

            var account = parseAccount($item.attr('data-account') || $item.data('account'));
            var owner = (account.Person && account.Person.Name) || (account.person && account.person.Name) || account.OwnerName || account.ownerName || '';
            var number = account.AccountNumber || account.accountNumber || '';
            var type = account.AccountType || account.accountType || '';
            var balance = Number(account.Balance || account.balance || 0);
            var created = account.Created || account.created || account.CreatedAt || account.createdAt || '';

            $('#account-number-title').text(number);
            $('#account-owner').text((account.IsClosed || account.isClosed ? 'Closed' : 'Active') + ' • ' + owner);
            $('#detail-account-number').text(number);
            $('#detail-account-description').text(type);
            $('#detail-account-owner').text(owner);
            $('#detail-account-balance').text('R ' + balance.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
            $('#detail-account-created').text(created ? new Date(created).toLocaleString() : '');

            var $txList = $('#transactions-list').empty();
            var txs = account.Transactions || account.transactions || [];
            if (!txs.length) {
                $txList.append('<li class="list-group-item text-muted">No transactions available.</li>');
            } else {
                txs.forEach(function(t){
                    var amount = Number(t.Amount || t.amount || 0);
                    var text = t.Description || t.description || 'Transaction';
                    var date = t.TransactionDate || t.transactionDate || '';
                    var cls = amount >= 0 ? 'text-success' : 'text-danger';
                    var sign = amount >= 0 ? '+ ' : '- ';
                    $txList.append(
                        '<a href="/Transaction/Details/' + (t.Id || t.id || '') + '" class="list-group-item list-group-item-action d-flex justify-content-between align-items-start">' +
                            '<div><div class="fw-semibold">' + text + '</div><small class="text-muted">' + (date ? new Date(date).toLocaleDateString() : '') + '</small></div>' +
                            '<div class="' + cls + '">' + sign + 'R ' + Math.abs(amount).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</div>' +
                        '</a>'
                    );
                });
            }

            $('#account-id').val(account.Id || account.id || '');
            $('#update-account-id').val(account.Id || account.id || '');
            $('#no-selection').addClass('d-none');
            $('#person-details').removeClass('d-none');
        });

        // submit update account via AJAX (close-only)
        function getToastManager() {
            if (window._ledger_toast_manager) return window._ledger_toast_manager;
            if (window.Toaster && window.Toaster.ToastManager) {
                window._ledger_toast_manager = new Toaster.ToastManager();
                return window._ledger_toast_manager;
            }
            return null;
        }

        function showToast(type, message) {
            var tm = getToastManager();
            var t = (type || 'success').toLowerCase();
            if (tm) {
                var mapped = t === 'warn' ? 'warning' : t;
                tm.addToast(message, mapped, { allowHtml: true });
                return;
            }
            if (window.toastr) {
                if (t === 'error') toastr.error(message);
                else if (t === 'warning') toastr.warning(message);
                else if (t === 'info') toastr.info(message);
                else toastr.success(message);
            } else {
                console.log('Toast:', type, message);
            }
        }

        $(document).on('submit', '#update-account-form', function(e){
            e.preventDefault();
            var $form = $(this);
            $.ajax({
                type: 'POST',
                url: $form.attr('action') || '/Person/UpdateAccount',
                data: $form.serialize(),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).done(function(resp){
                if (resp && resp.success) {
                    showToast('success', resp.message || 'Account updated');
                    var modalEl = document.getElementById('updateAccountModal');
                    if (window.bootstrap && bootstrap.Modal) {
                        var instance = bootstrap.Modal.getInstance(modalEl);
                        if (instance) instance.hide();
                    } else {
                        $('#updateAccountModal').modal('hide');
                    }
                    setTimeout(function(){ window.location.reload(); }, 1200);
                    return;
                }
                showToast('error', (resp && resp.message) || 'Failed to update account');
            }).fail(function(){
                showToast('error', 'Failed to update account (network/server error)');
            });
        });

        $(document).on('click', '#make-transaction-link', function(e){
            var href = $(this).attr('href');
            if (href && href !== '#') return;
            e.preventDefault();
            var modalEl = document.getElementById('addTransactionModal');
            if (window.bootstrap && bootstrap.Modal) new bootstrap.Modal(modalEl).show();
            else $('#addTransactionModal').modal('show');
        });
    });
})(jQuery);
