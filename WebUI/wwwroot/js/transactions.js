(function ($) {
    $(function () {
        // cache targets
        var $txDesc = $('#tx-description'), $txMeta = $('#tx-meta'), $txBadge = $('#tx-badge');
        var $txId = $('#tx-id'), $txDate = $('#tx-date'), $txAccount = $('#tx-account');
        var $txAmount = $('#tx-amount'), $txStatus = $('#tx-status-text');

        var $container = $('.card.sidebar-card');
        if (!$container.length) return;

        function parseNumber(v){
            if (v == null) return NaN;
            return Number(String(v).trim().replace(',', '.'));
        }

        function updateFrom($el){
            var titleEl = $el.find('h6, .fw-bold').first();
            var title = titleEl.length ? $.trim(titleEl.text()) : ($el.attr('data-description') || '');
            var date = $el.attr('data-transaction-date') || '';
            var acc = $el.attr('data-account-number') || '';
            var amountRaw = $el.attr('data-amount');
            var id = $el.attr('data-transaction-id') || '';

            $txDesc.text(title || 'No transaction selected');
            $txMeta.text(date ? date + ' • ' + acc : '-');
            // badge style like People page: green when selected, muted when none
            $txBadge.text(title ? 'Completed' : '');
            $txBadge.removeClass('bg-success bg-secondary');
            $txBadge.addClass(title ? 'bg-success' : 'bg-secondary');

            $txId.text(id);
            $txDate.text(date || '-');
            $txAccount.text(acc || '-');

            var num = parseNumber(amountRaw);
            $txAmount.removeClass('text-danger text-success');
            if (isFinite(num)){
                if (num < 0) {
                    $txAmount.addClass('text-danger').text('- R ' + Math.abs(num).toLocaleString(undefined,{minimumFractionDigits:2,maximumFractionDigits:2}));
                } else {
                    $txAmount.addClass('text-success').text('R ' + num.toLocaleString(undefined,{minimumFractionDigits:2,maximumFractionDigits:2}));
                }
            } else {
                $txAmount.text('-');
            }

            $txStatus.text(title ? 'Completed' : '-');
        }

        var $items = $container.find('.person-item');
        if ($items.length){
            $items.first().addClass('active');
            updateFrom($items.first());
        }

        $container.on('click','.person-item', function(e){
            e.preventDefault();
            $container.find('.person-item.active').removeClass('active');
            $(this).addClass('active');
            updateFrom($(this));
        });
    });
})(jQuery);
