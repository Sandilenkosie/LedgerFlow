(function($){
    // jQuery-based person list interactions
    $(document).ready(function(){
        var $items = $('.person-item');

        function showNoSelection() {
            $('#no-selection').removeClass('d-none');
            $('#person-details').addClass('d-none');
        }

        function showDetails() {
            $('#no-selection').addClass('d-none');
            $('#person-details').removeClass('d-none');
        }

        // Start with no selection
        showNoSelection();

        // Helper to safely show a Bootstrap modal (supports Bootstrap 5 global or jQuery fallback)
        function showModalElement(modalEl) {
            if (!modalEl) return;
            try {
                if (typeof bootstrap !== 'undefined' && bootstrap && typeof bootstrap.Modal === 'function') {
                    var modal = new bootstrap.Modal(modalEl);
                    modal.show();
                    return;
                }
            } catch (ex) {
                console.warn('bootstrap.Modal failed, falling back to jQuery if available', ex);
            }

            // jQuery/bootstrap 4 fallback
            if (window.jQuery && typeof $(modalEl).modal === 'function') {
                $(modalEl).modal('show');
                return;
            }

            console.warn('No modal method available to show element', modalEl);
        }

        // helper to apply active text styling to an item
        function applyActiveTextStyles($el) {
            // reset all items
            $items.each(function() {
                var $it = $(this);
                $it.find('.fw-semibold').removeClass('fw-bold');
                $it.find('h6').removeClass('text-dark');
                $it.find('small').removeClass('text-dark').addClass('text-muted');
            });

            // apply to selected
            if ($el && $el.length) {
                $el.find('.fw-semibold').addClass('fw-bold');
                $el.find('h6').addClass('text-dark');
                $el.find('small').removeClass('text-muted').addClass('text-dark');
            }
        }

        $items.on('click', function(e){
            e.preventDefault();
            $items.removeClass('active');
            $(this).addClass('active');
            applyActiveTextStyles($(this));

            var id = $(this).data('id');
            var name = $(this).data('name');
            var username = $(this).data('username');
            var idnumber = $(this).data('idnumber');
            var email = $(this).data('email');
            var phone = $(this).data('phone');

            // populate fields
            $('#person-name').text(name || '');
            $('#person-username-text').text(username || '');
            $('#detail-fullname').text(name || '');
            $('#detail-idnumber').text(idnumber || '');
            $('#detail-email').text(email || '');
            $('#detail-phone').text(phone || '');

            // Render related accounts using shared helper if available
            if (window.renderRelatedAccounts) {
                window.renderRelatedAccounts(this);
            } else {
                console.warn('renderRelatedAccounts is not defined. Ensure RelatedAccount.js is loaded.');
            }

            // Render related transactions using shared helper if available
            if (window.renderRelatedTransactions) {
                window.renderRelatedTransactions(this);
            } else {
                console.warn('renderRelatedTransactions is not defined. Ensure RelatedTransaction.js is loaded.');
            }

            showDetails();
        });

        // Optional: clear selection when clicking outside (basic implementation)
        $(document).on('click', function(e){
            if ($(e.target).closest('.person-item, .details-panel').length === 0) {
                $items.removeClass('active');
                showNoSelection();
            }
        });

        // Expose a helper to open the Add Account modal and populate fields
        window.openAddAccountModal = function(person) {
            // support both camelCase and PascalCase properties
            var id = (person && (person.id || person.Id)) || '';
            $('#person-id').val(id);
            $('#account-type').val('Primary');

            var modalEl = document.getElementById('addAccountModal');
            showModalElement(modalEl);
        };

        // Listen for any button that targets the Add Account modal and has a data-userid attribute
        $(document).on('click', 'button[data-target="#addAccountModal"], button[data-bs-target="#addAccountModal"]', function(e){
            // allow bootstrap's default behavior to proceed for non-JS environments, but handle via JS here
            e.preventDefault();

            var $btn = $(this);
            var userId = $btn.data('userid') || $btn.attr('data-userid') || '';

            // Call the helper with a simple person-like object
            window.openAddAccountModal({ Id: userId});
        });

        // Expose a helper to open the Add Transaction modal and populate fields
        window.openAddTransactionModal = function (account) {
            // support both camelCase and PascalCase properties
            var id = (account && (account.id || account.Id)) || '';
            // account-id is the hidden field used by the Add Transaction form
            $('#account-id').val(id);

            // clear/reset transaction fields
            $('#transaction-id').val('');
            $('#transaction-amount').val('');
            // default date to today (local date string in yyyy-mm-dd)
            try {
                var today = new Date();
                var yyyy = today.getFullYear();
                var mm = String(today.getMonth() + 1).padStart(2, '0');
                var dd = String(today.getDate()).padStart(2, '0');
                $('#transaction-date').val(yyyy + '-' + mm + '-' + dd);
            } catch (ex) { $('#transaction-date').val(''); }

            var modalEl = document.getElementById('addTransactionModal');
            showModalElement(modalEl);
        };

        // Open Edit Transaction modal prefilled with data
        window.openEditTransactionModal = function(tx) {
            if (!tx) return;
            $('#transaction-id').val(tx.Id || tx.id || '');
            $('#account-id').val(tx.AccountId || tx.accountId || '');
            $('#transaction-amount').val(tx.Amount || tx.amount || '');
            // normalize date to yyyy-mm-dd
            var d = tx.TransactionDate || tx.transactionDate || tx.Date || tx.date || '';
            if (d) {
                try { var dt = new Date(d); var yyyy = dt.getFullYear(); var mm = String(dt.getMonth()+1).padStart(2,'0'); var dd = String(dt.getDate()).padStart(2,'0'); $('#transaction-date').val(yyyy + '-' + mm + '-' + dd); } catch (ex) { $('#transaction-date').val(d); }
            }
            $('#transaction-Description').val(tx.Description || tx.description || '');
            var modalEl = document.getElementById('addTransactionModal');
            showModalElement(modalEl);
        };

        // Listen for any button that targets the Add Transaction modal and has a data-userid or data-accountid attribute
        $(document).on('click', 'button[data-target="#addTransactionModal"]', function(e){
            e.preventDefault();
            var $btn = $(this);
            var userId = $btn.data('accountid') || $btn.attr('data-accountid') || '';
            window.openAddTransactionModal({ Id: userId });
        });

        // Helper to open the update modal for the currently selected person
        window.openUpdatePersonModalFromCurrent = function() {
            var $selected = $('.person-item.active');
            if (!$selected || !$selected.length) return;

            // Build person object from data attributes (support PascalCase too)
            var person = {
                id: $selected.data('id') || $selected.attr('data-id'),
                name: $selected.data('name') || $selected.attr('data-name'),
                fullName: $selected.data('name') || $selected.attr('data-name'),
                username: $selected.data('username') || $selected.attr('data-username'),
                idNumber: $selected.data('idnumber') || $selected.attr('data-idnumber') || '',
                email: $selected.data('email') || $selected.attr('data-email') || '',
                phone: $selected.data('phone') || $selected.attr('data-phone') || ''
            };

            if (window.openUpdatePersonModal) {
                window.openUpdatePersonModal(person);
            } else {
                console.warn('openUpdatePersonModal is not defined');
            }
        };

        // Helper to delete the currently selected person via AJAX POST
        window.deleteCurrentPerson = function() {
            var $selected = $('.person-item.active');
            if (!$selected || !$selected.length) return;
            var id = $selected.data('id') || $selected.attr('data-id');
            if (!id) return;

            if (!confirm('Are you sure you want to remove this person? This cannot be undone.')) return;

            // get the antiforgery token from the page
            var token = $('input[name="__RequestVerificationToken"]').first().val();

            $.post({
                url: '/Person/DeletePerson',
                data: { id: id, __RequestVerificationToken: token },
                success: function(resp) {
                    if (resp && resp.success) {
                        // remove from list and clear details
                        $selected.remove();
                        $('#person-name').text('');
                        $('#person-username-text').text('');
                        $('#detail-fullname').text('');
                        $('#detail-idnumber').text('');
                        $('#detail-email').text('');
                        $('#detail-phone').text('');
                        $('#no-selection').removeClass('d-none');
                        $('#person-details').addClass('d-none');
                    } else {
                        alert('Failed to remove person');
                    }
                },
                error: function() {
                    alert('Failed to remove person');
                }
            });
        };
    });
})(jQuery);
