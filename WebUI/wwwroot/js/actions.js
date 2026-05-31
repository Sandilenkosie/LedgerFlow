// Simplified jQuery-based actions.js
$(function() {
  var $close = $('#close-account-action');
  var $reopen = $('#reopen-account-action');
  var $items = $('.person-item');
  var $form = $('#confirmActionForm');
  var $modal = $('#confirmActionModal');
  var $modalBody = $('#confirmActionModalBody');
  var $confirmBtn = $('#confirmActionConfirm');
  var bsModal = $modal.length ? new bootstrap.Modal($modal[0]) : null;

  var selected = null;

  function setEnabled($el, enabled) {
    if (!$el || !$el.length) return;
    $el.toggleClass('disabled-action', !enabled);
    if (enabled) { $el.removeAttr('aria-disabled').removeAttr('tabindex'); }
    else { $el.attr('aria-disabled','true').attr('tabindex','-1'); }
  }

  // Create or reuse a Toast manager from toastr-init.js. Falls back to legacy window.toastr or alert.
  var _toastMgr = null;
  try {
    if (window.Toaster && window.Toaster._instance) _toastMgr = window.Toaster._instance;
    else if (window.Toaster && window.Toaster.ToastManager) _toastMgr = window.Toaster._instance = new window.Toaster.ToastManager();
  } catch (e) { _toastMgr = null; }

  function notify(type, message, title) {
    if (_toastMgr) {
      _toastMgr.addToast(message || '', type === 'error' ? 'error' : (type === 'warn' ? 'warning' : (type === 'info' ? 'info' : 'success')), { title: title });
      return;
    }
    if (window.toastr) {
      if (type === 'error') window.toastr.error(message);
      else if (type === 'warn' || type === 'warning') window.toastr.warning(message);
      else if (type === 'info') window.toastr.info(message);
      else window.toastr.success(message);
      return;
    }
    try { alert((title ? title + ': ' : '') + (message || '')); } catch (e){}
  }

  function refresh() {
    if (!selected) { setEnabled($close, false); setEnabled($reopen, false); return; }
    var closed = !!(selected.IsClosed === true || selected.isClosed === true);
    setEnabled($close, !closed);
    setEnabled($reopen, closed);
  }

  $items.on('click', function(e) {
    e.preventDefault();
    var $el = $(this);
    try { selected = JSON.parse($el.attr('data-account') || 'null'); } catch (err) { selected = null; }
    $items.removeClass('active');
    $el.addClass('active');
    refresh();
  });

  function submitForm() {
    var fd = new FormData($form[0]);
    return $.ajax({
      url: $form.attr('action') || '/Person/UpdateAccount',
      method: 'POST',
      data: fd,
      processData: false,
      contentType: false,
      headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });
  }

  function openConfirm(message, onConfirm) {
    if (!bsModal) { if (confirm(message)) onConfirm(); return; }
    $modalBody.text(message);
    var handler = function() { $confirmBtn.off('click', handler); bsModal.hide(); onConfirm(); };
    $confirmBtn.on('click', handler);
    bsModal.show();
  }

  function handle(close) {
    if (!selected || !selected.Id) { notify('error', 'No account selected'); return; }
    $form.find('#confirm-form-accountId').val(selected.Id);
    $form.find('#confirm-form-close').val(close ? 'true' : 'false');
    openConfirm((close ? 'Close' : 'Re-open') + ' account ' + (selected.AccountNumber || '' ) + '?', function() {
      setEnabled($close, false); setEnabled($reopen, false);
      submitForm().done(function(result) {
        if (!result || result.success !== true) { notify('error', result && result.message ? result.message : 'Action failed'); refresh(); return; }
        selected.IsClosed = !!close;
        refresh();
        var $badge = $('#account-status-badge');
        if ($badge.length) { $badge.toggleClass('bg-success', !close); $badge.toggleClass('bg-danger', close); $badge.text(close ? 'Closed' : 'Open'); }
        $items.each(function() {
          try {
            var $it = $(this);
            var obj = JSON.parse($it.attr('data-account')||'null');
            if (obj && (obj.Id == selected.Id || obj.id == selected.Id)) { obj.IsClosed = !!close; $it.attr('data-account', JSON.stringify(obj)); var s = $it.find('small'); if (s.length) s.text((close ? 'Closed' : 'Open') + ' • ' + (obj.OwnerName || 'Unknown')); }
          } catch (e) {}
        });
        notify('success', close ? 'Account closed' : 'Account re-opened');
      }).fail(function() { notify('error', 'Request failed'); refresh(); });
    });
  }

  $close.on('click', function(e) { e.preventDefault(); if (!$close.hasClass('disabled-action')) handle(true); });
  $reopen.on('click', function(e) { e.preventDefault(); if (!$reopen.hasClass('disabled-action')) handle(false); });

});
