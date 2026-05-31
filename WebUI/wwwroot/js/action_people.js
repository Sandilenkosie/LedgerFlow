(function($){
    $(function(){
        var $addAction = $('#add-account-action');

        function updateAddAccountAction() {
            var $sel = $('.person-item.active');
            if ($sel.length) {
                // enable
                $addAction.removeClass('disabled-action').removeAttr('aria-disabled').attr('tabindex', '0');
            } else {
                // disable
                $addAction.addClass('disabled-action').attr('aria-disabled','true').attr('tabindex','-1');
            }
        }

        // initialize state on load
        updateAddAccountAction();

        // when a person is selected (delegated) update the action
        $(document).on('click', '.person-item', function(e){
            // small timeout to let other handlers (that add .active) run first
            setTimeout(updateAddAccountAction, 0);
        });

        // handle click on the action link
        $(document).on('click', '#add-account-action', function(e){
            e.preventDefault();
            var $this = $(this);
            if ($this.hasClass('disabled-action')) return;
            var $sel = $('.person-item.active');
            var person = null;
            if ($sel.length) {
                person = { Id: $sel.data('id'), Name: $sel.data('name') };
            }
            if (typeof window.openAddAccountModal === 'function') {
                window.openAddAccountModal(person);
            }
        });

        // also observe mutations in case some other script toggles .active class
        try {
            var list = document.querySelector('.list-group');
            if (list && window.MutationObserver) {
                var mo = new MutationObserver(function(){ updateAddAccountAction(); });
                mo.observe(list, { attributes: true, subtree: true, attributeFilter: ['class'] });
            }
        } catch(e){ /* ignore */ }
    });
})(jQuery);
