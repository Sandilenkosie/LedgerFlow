// Modern ToastManager: self-contained, injects styles, accessible, animated, and themeable.
// Attach to window.Toaster so other scripts can reuse it across the project.
window.Toaster = window.Toaster || {};

(() => {
    const defaultCSS = `
    .tf-toast-container{position:fixed;z-index:2147483647;pointer-events:none;display:flex;flex-direction:column;gap:10px;padding:12px;width:auto;max-width:calc(100% - 24px)}
    .tf-toast-top-right{top:12px;right:12px;align-items:flex-end}
    .tf-toast-top-left{top:12px;left:12px;align-items:flex-start}
    .tf-toast-bottom-right{bottom:12px;right:12px;align-items:flex-end}
    .tf-toast-bottom-left{bottom:12px;left:12px;align-items:flex-start}
    .tf-toast{min-width:280px;max-width:420px;pointer-events:auto;background:linear-gradient(135deg, rgba(255,255,255,0.95), rgba(250,250,250,0.95));color:#0f1724;border-radius:12px;box-shadow:0 8px 30px rgba(2,6,23,0.2);display:flex;gap:12px;align-items:center;padding:12px 14px;border-left:6px solid transparent;overflow:hidden;font-family:Inter,Segoe UI,Roboto,system-ui,-apple-system,"Helvetica Neue",Arial}
    .tf-toast .tf-icon{flex:0 0 36px;height:36px;width:36px;border-radius:8px;display:flex;align-items:center;justify-content:center;font-size:18px}
    .tf-toast .tf-body{flex:1;display:flex;flex-direction:column;gap:4px;align-items:center;justify-content:center}
    .tf-toast .tf-title{font-weight:600;font-size:14px;color:#071124;text-align:center}
    .tf-toast .tf-message{font-size:13px;color:#334155;text-align:center}
    .tf-toast .tf-actions{display:flex;gap:8px;align-items:center;margin-left:8px}
    .tf-toast .tf-close{background:transparent;border:none;color:#64748b;cursor:pointer;padding:6px;border-radius:6px}
    .tf-progress{height:4px;background:rgba(2,6,23,0.06);border-radius:999px;overflow:hidden;margin-top:8px}
    .tf-progress > i{display:block;height:100%;background:linear-gradient(90deg,#06b6d4,#3b82f6);width:100%;transform-origin:left;transition:transform linear}
    .tf-show{animation:tf-slide-in .36s cubic-bezier(.2,.9,.3,1) both}
    .tf-hide{animation:tf-fade-out .28s ease forwards}
    @keyframes tf-slide-in{from{transform:translateY(12px) scale(.98);opacity:0}to{transform:none;opacity:1}}
    @keyframes tf-fade-out{to{opacity:0;transform:translateY(-8px) scale(.98)}}
    .tf-success{border-left-color:#10b981}
    .tf-info{border-left-color:#06b6d4}
    .tf-warning{border-left-color:#f59e0b}
    .tf-error{border-left-color:#ef4444}
    `;

    // Inject styles once
    function injectStyles() {
        if (document.getElementById('tf-toast-styles')) return;
        const s = document.createElement('style');
        s.id = 'tf-toast-styles';
        s.appendChild(document.createTextNode(defaultCSS));
        document.head.appendChild(s);
    }

    class ToastManager {
        constructor(options = {}) {
            injectStyles();
            this.defaults = Object.assign({
                position: 'bottom-right', // top-right, top-left, bottom-right, bottom-left
                duration: 4500,
                autoClose: true,
                pauseOnHover: true,
                closeButton: true,
                allowHtml: false
            }, options);

            this.containers = {};
        }

        _getContainer(position) {
            if (this.containers[position]) return this.containers[position];
            const c = document.createElement('div');
            c.className = `tf-toast-container tf-toast-${position.replace(/_/g,'-')}`;
            document.body.appendChild(c);
            this.containers[position] = c;
            return c;
        }

        _sanitize(str) {
            const div = document.createElement('div');
            div.textContent = str;
            return div.innerHTML;
        }

        addToast(message, type = 'default', options = {}) {
            const opts = Object.assign({}, this.defaults, options);
            const pos = opts.position || this.defaults.position;
            const container = this._getContainer(pos);

            const toast = document.createElement('div');
            toast.className = `tf-toast tf-show ${type !== 'default' ? 'tf-' + type : ''}`;
            toast.setAttribute('role','status');
            toast.setAttribute('aria-live','polite');

            const icon = document.createElement('div');
            icon.className = 'tf-icon';
            const iconMap = {
                success: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6L9 17l-5-5"/></svg>',
                info: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>',
                warning: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>',
                error: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>',
                default: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/></svg>'
            };
            icon.innerHTML = iconMap[type] || iconMap.default;

            const body = document.createElement('div');
            body.className = 'tf-body';
            const titleEl = document.createElement('div');
            titleEl.className = 'tf-title';
            const msgEl = document.createElement('div');
            msgEl.className = 'tf-message';

            // If user provided HTML and allowed, set innerHTML; otherwise sanitize
            if (opts.allowHtml) {
                msgEl.innerHTML = message;
            } else {
                msgEl.innerHTML = this._sanitize(message);
            }

            body.appendChild(titleEl);
            body.appendChild(msgEl);

            const actions = document.createElement('div');
            actions.className = 'tf-actions';

            if (opts.closeButton) {
                const close = document.createElement('button');
                close.className = 'tf-close';
                close.setAttribute('aria-label','Close notification');
                close.innerHTML = '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>';
                close.addEventListener('click', () => this._removeToast(toast, opts.onClose));
                actions.appendChild(close);
            }

            toast.appendChild(icon);
            toast.appendChild(body);
            toast.appendChild(actions);

            // Progress bar
            let progressEl = null;
            if (opts.autoClose) {
                const progress = document.createElement('div');
                progress.className = 'tf-progress';
                const bar = document.createElement('i');
                bar.style.transform = 'scaleX(1)';
                progress.appendChild(bar);
                toast.appendChild(progress);
                progressEl = bar;
            }

            container.appendChild(toast);

            // Title usage: if options.title provided
            if (opts.title) titleEl.textContent = opts.title; else titleEl.style.display = 'none';

            let start = Date.now();
            let elapsed = 0;
            let rafId = null;

            const tick = () => {
                if (!progressEl) return;
                elapsed = Date.now() - start;
                const ratio = Math.max(0, 1 - elapsed / opts.duration);
                progressEl.style.transform = `scaleX(${ratio})`;
                if (ratio > 0) rafId = requestAnimationFrame(tick);
            };

            if (opts.autoClose && progressEl) {
                start = Date.now();
                rafId = requestAnimationFrame(tick);
            }

            let closeTimer = null;
            if (opts.autoClose) {
                closeTimer = setTimeout(() => this._removeToast(toast, opts.onClose), opts.duration);
            }

            const mouseHandlers = {};
            if (opts.pauseOnHover) {
                mouseHandlers.enter = () => {
                    if (closeTimer) { clearTimeout(closeTimer); closeTimer = null; }
                    if (rafId) { cancelAnimationFrame(rafId); rafId = null; }
                };
                mouseHandlers.leave = () => {
                    if (opts.autoClose) {
                        start = Date.now();
                        if (progressEl) rafId = requestAnimationFrame(tick);
                        closeTimer = setTimeout(() => this._removeToast(toast, opts.onClose), opts.duration);
                    }
                };
                toast.addEventListener('mouseenter', mouseHandlers.enter);
                toast.addEventListener('mouseleave', mouseHandlers.leave);
            }

            // Allow programmatic close via returned object
            const api = {
                close: () => this._removeToast(toast, opts.onClose)
            };

            return api;
        }

        _removeToast(toast, onClose) {
            if (!toast || toast._removing) return;
            toast._removing = true;
            toast.classList.remove('tf-show');
            toast.classList.add('tf-hide');
            setTimeout(() => {
                if (toast.parentNode) toast.parentNode.removeChild(toast);
                if (typeof onClose === 'function') {
                    try { onClose(); } catch (e) { console.error(e); }
                }
            }, 300);
        }
    }

    // Expose ToastManager
    window.Toaster.ToastManager = ToastManager;

})();



