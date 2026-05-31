(function($){
    $(function(){
        var $addAction = $('#add-account-action');
        var $personId = $('#person-id');
        var $addForm = $('#add-account-form');
        var $modal = $('#addAccountModal');

        function updateAddAccountAction() {
            var has = $('.person-item.active').length > 0;
            $addAction.toggleClass('disabled-action', !has)
                      .attr('aria-disabled', !has ? 'true' : null)
                      .attr('tabindex', !has ? '-1' : '0');
        }

        updateAddAccountAction();

        // when a person is selected update action state
        $(document).on('click', '.person-item', function(){
            // allow other handlers to run that set .active
            setTimeout(updateAddAccountAction, 0);
        });

        // small helper to resolve selected person id
        function resolveSelectedPersonId() {
            var id = $personId.val() || '';
            if (!id) {
                var $sel = $('.person-item.active');
                if ($sel.length) id = $sel.data('id') || $sel.attr('data-id') || '';
            }
            return id || '';
        }

        // open add account: set hidden inputs then show modal
        $(document).on('click', '#add-account-action', function(e){
            e.preventDefault();
            if ($(this).hasClass('disabled-action')) return;
            var id = resolveSelectedPersonId();
            $personId.val(id);
            if ($addForm.length) $addForm.find('input[name="userId"]').val(id);

            if (typeof window.openAddAccountModal === 'function') {
                window.openAddAccountModal({ Id: id });
                return;
            }
            if ($modal.length) {
                if (window.bootstrap && bootstrap.Modal) new bootstrap.Modal($modal[0]).show(); else $modal.modal('show');
            }
        });

        // ensure hidden input in form is set when modal opens
        $modal.on('show.bs.modal', function(){
            var id = resolveSelectedPersonId();
            $personId.val(id);
            if ($addForm.length) $addForm.find('input[name="userId"]').val(id);
        });

        // submit add-account via AJAX
        $addForm.on('submit', function(e){
            e.preventDefault();
            var id = resolveSelectedPersonId();
            if (!id) {
                showToast('error', 'No person selected. Please select a person before creating an account.');
                return;
            }
            $personId.val(id);
            $addForm.find('input[name="userId"]').val(id);

            $.ajax({
                type: 'POST',
                url: $addForm.attr('action') || '/Person/AddAccount',
                data: $addForm.serialize(),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).done(function(resp){
                if (resp && resp.success) {
                    showToast('success', resp.message || 'Account created');
                    if (window.bootstrap && bootstrap.Modal) {
                        var instance = bootstrap.Modal.getInstance($modal[0]);
                        if (instance) instance.hide();
                    } else {
                        $modal.modal('hide');
                    }
                    setTimeout(function(){ window.location.reload(); }, 1200);
                    return;
                }
                showToast('error', (resp && resp.message) || 'Failed to create account');
            }).fail(function(){
                showToast('error', 'Failed to create account (network/server error)');
            });
        });
    });
})(jQuery);
