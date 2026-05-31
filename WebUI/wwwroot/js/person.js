 (function($){
    $(function(){
        var $items = $('.person-item');

        function clearDetails(){
            $('#person-name, #person-username-text, #detail-fullname, #detail-idnumber').text('');
            $('#no-selection').removeClass('d-none');
            $('#person-details').addClass('d-none');
            // disable delete action when no selection
            $('#delete-person-action').addClass('disabled-action').attr('aria-disabled','true').attr('tabindex','-1');
        }

        function showDetails(){
            $('#no-selection').addClass('d-none');
            $('#person-details').removeClass('d-none');
            // enable delete action when a person is selected
            $('#delete-person-action').removeClass('disabled-action').removeAttr('aria-disabled').removeAttr('tabindex');
        }

        // start empty
        clearDetails();

        // Toast helper using new Toaster when available
        function getToastManager() {
            if (window._ledger_toast_manager) return window._ledger_toast_manager;
            if (window.Toaster && window.Toaster.ToastManager) {
                window._ledger_toast_manager = new Toaster.ToastManager();
                return window._ledger_toast_manager;
            }
            return null;
        }

        function showToast(type, message, options) {
            var tm = getToastManager();
            var t = (type || 'success').toLowerCase();
            if (tm) {
                var mapped = t === 'warn' ? 'warning' : t;
                tm.addToast(message, mapped, Object.assign({ allowHtml: true }, options || {}));
            } else if (window.toastr) {
                if (t === 'error') toastr.error(message);
                else if (t === 'warning' || t === 'warn') toastr.warning(message);
                else if (t === 'info') toastr.info(message);
                else toastr.success(message);
            } else {
                console.log('Toast:', type, message);
            }
        }

        // simple selection
        $items.on('click', function(e){
            e.preventDefault();
            $items.removeClass('active');
            $(this).addClass('active');

            var $el = $(this);
            var id = $el.data('id');
            var name = $el.data('name') || '';
            var username = $el.data('username') || '';
            var idnumber = $el.data('idnumber') || '';

            $('#person-name').text(name);
            $('#person-username-text').text(username);
            $('#detail-fullname').text(name);
            $('#detail-idnumber').text(idnumber);

            // call shared renderers if present (keeps this file small)
            if (typeof window.renderRelatedAccounts === 'function') window.renderRelatedAccounts($el);
            if (typeof window.renderRelatedTransactions === 'function') window.renderRelatedTransactions($el);

            showDetails();
        });

        // open add account modal
        window.openAddAccountModal = function(person){
            var id = (person && (person.id || person.Id)) || '';
            $('#person-id').val(id);
            $('#account-type').val('Primary');
            var modal = document.getElementById('addAccountModal');
            if (window.bootstrap && bootstrap.Modal) new bootstrap.Modal(modal).show(); else $(modal).modal('show');
        };

        // ensure click on delete action triggers the modal (delegated)
        $(document).on('click', '#delete-person-action', function(e){
            e.preventDefault();
            var $this = $(this);
            if ($this.hasClass('disabled-action')) return;
            // call the same function
            window.deleteCurrentPerson();
        });

        // open add person modal (explicit) — fallback if data-bs toggle isn't working
        $(document).on('click', '#open-add-person', function(e){
            e.preventDefault();
            var modalEl = document.getElementById('addPersonModal');
            if (!modalEl) return;
            if (window.bootstrap && bootstrap.Modal) new bootstrap.Modal(modalEl).show(); else $('#addPersonModal').modal('show');
        });

        // submit add person via AJAX
        $(document).on('submit', '#add-person-form', function(e){
            e.preventDefault();
            var $form = $(this);
            $.ajax({
                type: 'POST',
                url: $form.attr('action') || '/Person/AddPerson',
                data: $form.serialize(),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).done(function(resp){
                if (resp && resp.success) {
                    showToast('success', resp.message || 'Person created');
                    var modalEl = document.getElementById('addPersonModal');
                    if (window.bootstrap && bootstrap.Modal) {
                        var instance = bootstrap.Modal.getInstance(modalEl);
                        if (instance) instance.hide();
                    } else {
                        $('#addPersonModal').modal('hide');
                    }
                    setTimeout(function(){ window.location.reload(); }, 1200);
                    return;
                }
                if (resp && resp.errors) showToast('error', (resp.errors || []).join('\n') || (resp && resp.message) || 'Failed to create person');
                else showToast('error', (resp && resp.message) || 'Failed to create person');
            }).fail(function(){
                showToast('error', 'Failed to create person (network/server error)');
            });
        });

        // open update modal from currently selected person
        window.openUpdatePersonModalFromCurrent = function(){
            var $sel = $('.person-item.active');
            if (!$sel.length) return;
            var person = {
                Id: $sel.data('id'),
                Name: $sel.data('name'),
                Username: $sel.data('username'),
                IdNumber: $sel.data('idnumber')
            };
            if (typeof openUpdatePersonModal === 'function') openUpdatePersonModal(person);
        };

        // submit update person via AJAX
        $(document).on('submit', '#update-person-form', function(e){
            e.preventDefault();
            var $form = $(this);
            $.ajax({
                type: 'POST',
                url: $form.attr('action') || '/Person/UpdatePerson',
                data: $form.serialize(),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).done(function(resp){
                if (resp && resp.success) {
                    showToast('success', resp.message || 'Person updated');
                    var modalEl = document.getElementById('updatePersonModal');
                    if (window.bootstrap && bootstrap.Modal) {
                        var instance = bootstrap.Modal.getInstance(modalEl);
                        if (instance) instance.hide();
                    } else {
                        $('#updatePersonModal').modal('hide');
                    }
                    setTimeout(function(){ window.location.reload(); }, 1200);
                    return;
                }
                if (resp && resp.errors) showToast('error', (resp.errors || []).join('\n') || (resp && resp.message) || 'Failed to update person');
                else showToast('error', (resp && resp.message) || 'Failed to update person');
            }).fail(function(){
                showToast('error', 'Failed to update person (network/server error)');
            });
        });

        // buttons that trigger add account
        $(document).on('click', 'button[data-bs-target="#addAccountModal"], button[data-target="#addAccountModal"]', function(e){
            e.preventDefault();
            var userId = $(this).data('userid') || $(this).attr('data-userid') || '';
            window.openAddAccountModal({ Id: userId });
        });

        // delete current person using confirmation modal
        window.deleteCurrentPerson = function(){
            var $sel = $('.person-item.active');
            if (!$sel.length) return;
            var id = $sel.data('id');
            if (!id) return;
            // set hidden input and show modal
            var $confirmForm = $('#confirmDeletePersonForm');
            var $confirmModal = $('#confirmDeletePersonModal');
            var $confirmBtn = $('#confirmDeletePersonConfirm');
            $confirmForm.find('#confirm-delete-person-id').val(id);
            var bs = $confirmModal.length ? new bootstrap.Modal($confirmModal[0]) : null;
            if (bs) bs.show(); else $confirmModal.modal('show');

            // attach one-time handler for confirm button
            $confirmBtn.off('click.confirm').on('click.confirm', function(){
                var token = $confirmForm.find('input[name="__RequestVerificationToken"]').first().val();
                $.ajax({
                    type: 'POST',
                    url: $confirmForm.attr('action') || '/Person/DeletePerson',
                    data: { id: id, __RequestVerificationToken: token },
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }).done(function(resp){
                if (resp && resp.success) {
                        showToast('success', resp.message || 'Person removed');
                        $sel.remove();
                        clearDetails();
                        if (bs) bs.hide(); else $confirmModal.modal('hide');
                        return;
                    }
                    showToast('error', (resp && resp.message) || 'Failed to remove person');
                    if (bs) bs.hide(); else $confirmModal.modal('hide');
                }).fail(function(){
                    showToast('error', 'Failed to remove person (network/server error)');
                    if (bs) bs.hide(); else $confirmModal.modal('hide');
                });
            });
        };
    });
})(jQuery);
