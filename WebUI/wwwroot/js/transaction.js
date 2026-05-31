(function($){
    $(function(){
        function showModalElement(modalEl) {
            if (!modalEl) return;
            if (window.bootstrap && bootstrap.Modal) {
                new bootstrap.Modal(modalEl).show();
                return;
            }
            if ($.fn.modal) $(modalEl).modal('show');
        }

        function hideModalElement(modalEl) {
            if (!modalEl) return;
            if (window.bootstrap && bootstrap.Modal) {
                var instance = bootstrap.Modal.getInstance(modalEl);
                if (instance) instance.hide();
                return;
            }
            if ($.fn.modal) $(modalEl).modal('hide');
        }

        function toInputDate(value) {
            var dt = value ? new Date(value) : new Date();
            if (isNaN(dt.getTime())) return value || '';
            var yyyy = dt.getFullYear();
            var mm = String(dt.getMonth() + 1).padStart(2, '0');
            var dd = String(dt.getDate()).padStart(2, '0');
            return yyyy + '-' + mm + '-' + dd;
        }

        function normalizeAmount(value) {
            var cleaned = String(value == null ? '' : value).trim().replace(',', '.').replace(/[^0-9.\-]/g, '');
            return cleaned.replace(/(\..*)\./g, '$1');
        }

        function getActiveAccountId() {
            return $('#account-id').val() || $('.person-item.active').attr('data-account-id') || '';
        }

        function getToastManager() {
            if (window._ledger_toast_manager) return window._ledger_toast_manager;
            if (window.Toaster && window.Toaster.ToastManager) {
                window._ledger_toast_manager = new Toaster.ToastManager();
                return window._ledger_toast_manager;
            }
            return null;
        }

        function notify(type, message) {
            var tm = getToastManager();
            var t = (type || 'success').toLowerCase();
            if (tm) {
                var mapped = t === 'warn' ? 'warning' : t;
                tm.addToast(message, mapped, { allowHtml: true });
                return;
            }
            if (!window.toastr) return;
            if (t === 'error') toastr.error(message);
            else if (t === 'warning') toastr.warning(message);
            else if (t === 'info') toastr.info(message);
            else toastr.success(message);
        }

        window.openAddTransactionModal = function (account) {
            var $modal = $('#addTransactionModal');
            var accountId = (account && (account.id || account.Id)) || getActiveAccountId();
            $modal.find('[name="Id"]').val('');
            $modal.find('[name="AccountId"]').val(accountId);
            $modal.find('[name="Amount"]').val('');
            $modal.find('[name="TransactionDate"]').val(toInputDate());
            $modal.find('[name="Description"]').val('');
            showModalElement(document.getElementById('addTransactionModal'));
        };

        window.openEditTransactionModal = function(tx) {
            if (!tx) return;
            var $modal = $('#updateTransactionModal');
            $modal.find('[name="Id"]').val(tx.Id || tx.id || '');
            $modal.find('[name="AccountId"]').val(tx.AccountId || tx.accountId || getActiveAccountId());
            $modal.find('[name="Amount"]').val(tx.Amount || tx.amount || '');
            $modal.find('[name="TransactionDate"]').val(toInputDate(tx.TransactionDate || tx.transactionDate || tx.Date || tx.date || ''));
            $modal.find('[name="Description"]').val(tx.Description || tx.description || '');
            showModalElement(document.getElementById('updateTransactionModal'));
        };

        $(document).on('click', 'button[data-target="#addTransactionModal"], button[data-bs-target="#addTransactionModal"]', function(e){
            e.preventDefault();
            var accountId = $(this).data('accountid') || $(this).attr('data-accountid') || getActiveAccountId();
            window.openAddTransactionModal({ Id: accountId });
        });

        $(document).on('click', '#make-transaction-link', function(e){
            e.preventDefault();
            window.openAddTransactionModal({ Id: getActiveAccountId() });
        });

        $(document).on('submit', '#add-transaction-form, #update-transaction-form', function(e){
            e.preventDefault();
            var $form = $(this);
            var $modal = $form.closest('.modal');
            var isUpdate = this.id === 'update-transaction-form';
            var accountId = $modal.find('[name="AccountId"]').val() || getActiveAccountId();
            var amount = normalizeAmount($modal.find('[name="Amount"]').val());

            $modal.find('[name="AccountId"]').val(accountId);
            $modal.find('[name="Amount"]').val(amount);

            if (!accountId) {
                notify('error', 'No account selected');
                return;
            }
            if (!amount) {
                notify('error', 'Amount is required');
                return;
            }

            // serialize form so token and names are included
            var payload = $form.serialize();
            $.ajax({
                type: 'POST',
                url: $form.attr('action') || '/Person/MakeTransaction',
                data: payload,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).done(function(resp){
                if (resp && resp.success) {
                    notify(resp.type || 'success', resp.message || (isUpdate ? 'Transaction updated' : 'Transaction saved'));
                    hideModalElement($modal.get(0));
                    setTimeout(function(){ window.location.reload(); }, 900);
                    return;
                }
                notify('error', (resp && resp.message) || (isUpdate ? 'Failed to update transaction' : 'Failed to save transaction'));
            }).fail(function(jqXHR, textStatus, errorThrown){
                // Try to extract a useful error message from JSON response or text
                var serverMessage = null;
                try {
                    if (jqXHR && jqXHR.responseJSON && jqXHR.responseJSON.message) serverMessage = jqXHR.responseJSON.message;
                    else if (jqXHR && jqXHR.responseText) {
                        // Attempt to parse JSON string if returned as text
                        try {
                            var parsed = JSON.parse(jqXHR.responseText);
                            if (parsed && parsed.message) serverMessage = parsed.message;
                        } catch (e) {
                            // not JSON, use raw text (trimmed)
                            serverMessage = jqXHR.responseText.substring(0, 1000);
                        }
                    }
                } catch (ex) {
                    // swallow
                }

                var message = serverMessage || errorThrown || textStatus || (isUpdate ? 'Failed to update transaction (network/server error)' : 'Failed to save transaction (network/server error)');
                notify('error', message);
                if (window.console && console.error) console.error('Transaction submit failed:', textStatus, errorThrown, jqXHR);
            });
        });
    });
})(jQuery);
