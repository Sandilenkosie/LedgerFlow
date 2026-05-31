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

            // Ensure accounts is an array
            if (accounts && !Array.isArray(accounts)) {
                if (typeof accounts === 'object') accounts = Object.values(accounts);
                else accounts = [];
            }

            if (accounts && accounts.length > 0) {
                // Compute simple timestamp and sort newest first
                accounts.forEach(function(a){
                    var created = a.Created || a.created || a.CreatedAt || a.createdAt || a.CreatedDate || a.createdDate || '';
                    a._ts = Date.parse(created) || 0;
                });
                accounts.sort(function(a,b){ return b._ts - a._ts; });

                // Show top 3 most recent (explicit loop to avoid unexpected push elsewhere)
                for (var i = 0; i < accounts.length && i < 3; i++) {
                    var acc = accounts[i];
                    var href = '/Account/Details/' + (acc.Id || acc.id || '');
                    var accountNumber = acc.AccountNumber || acc.accountNumber || 'Unknown';
                    var isClosed = acc.IsClosed || acc.isClosed || false;
                    var created = acc.Created || acc.created || acc.CreatedAt || acc.createdAt || acc.CreatedDate || acc.createdDate || '';
                    var createdText = '';
                    try { createdText = created ? (new Date(created)).toLocaleDateString() : ''; } catch(e) { createdText = ''; }

                    var $li = $('<li>').addClass('list-group-item p-0');
                    var $a = $('<a>').attr('href', href).addClass('d-flex justify-content-between align-items-start p-2 text-decoration-none w-100');
                    $a.append(
                        $('<div>').append($('<div>').addClass('fw-semibold text-truncate').text(accountNumber), $('<small>').addClass('text-muted').text('Account')),
                        $('<div>').addClass('text-end').append($('<div>').addClass('fw-semibold').text(isClosed ? 'Closed' : 'Open'), $('<small>').addClass('text-muted').text(createdText))
                    );
                    $li.append($a);
                    $accountsList.append($li);
                }
            } else {
                $accountsList.append('<li class="list-group-item text-muted">No accounts available.</li>');
            }
        }
        catch (ex) {
            console.error('Failed to parse accounts for person', ex);
        }
    };
})(jQuery);
