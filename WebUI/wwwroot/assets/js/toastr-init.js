// assets/js/toastr-init.js
(function(){
    try {
        var data = window.__serverToastr || null;
        if (!data || !data.msg) return;

        // configure toastr options
        toastr.options = {
            closeButton: true,
            progressBar: true,
            positionClass: 'toast-bottom-right',
            timeOut: '5000'
        };

        switch ((data.type || 'info').toLowerCase()) {
            case 'success': toastr.success(data.msg); break;
            case 'error': toastr.error(data.msg); break;
            case 'warning': toastr.warning(data.msg); break;
            default: toastr.info(data.msg); break;
        }
    }
    catch(e){
        // fail silently to avoid breaking page scripts
        console && console.error && console.error('toastr-init error', e);
    }
})();
