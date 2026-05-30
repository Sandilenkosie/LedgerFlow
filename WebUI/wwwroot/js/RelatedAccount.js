(function($){
    // Renders related accounts from a clicked person element into #accounts-list
    window.renderRelatedAccounts = function(el) {
        try {
            var $el = $(el);
            var accountsJson = $el.attr('data-accounts') || $el.data('accounts');
            var accounts = [];
            if (accountsJson) {
                // If jQuery parsed it to object already, use it; otherwise parse JSON string
                if (typeof accountsJson === 'object') {
                    accounts = accountsJson;
                } else {
                    accounts = JSON.parse(accountsJson);
                }
            }

            var $accountsList = $('#accounts-list');
            $accountsList.empty();

            if (accounts && accounts.length > 0) {
                accounts.forEach(function(acc){
                    var href = '/Account/Details/' + (acc.id || acc.Id || '');
                    var accountNumber = acc.accountNumber || acc.AccountNumber || 'Unknown';
                    var isClosed = acc.isClosed || acc.IsClosed || acc.is_closed || false;

                    var $a = $("<a>")
                        .attr('href', href)
                        .addClass('list-group-item list-group-item-action d-flex justify-content-between align-items-start')
                        .append(
                            $('<div>').append(
                                $('<div>').addClass('fw-semibold').text(accountNumber),
                                $('<small>').addClass('text-muted').text('Account')
                            ),
                            $('<div>').addClass('text-end').append(
                                $('<div>').addClass('fw-semibold').text(isClosed),
                                $('<small>').addClass('text-muted').text('Available')
                            )
                        );

                    $accountsList.append($a);
                });
            } else {
                $accountsList.append('<li class="list-group-item text-muted">No accounts available.</li>');
            }
        }
        catch (ex) {
            console.error('Failed to parse accounts for person', ex);
        }
    };
})(jQuery);
