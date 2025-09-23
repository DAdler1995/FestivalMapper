// This file is loaded as an ES module via JS interop (no global <script> tag needed).
// Requires Bootstrap's JS bundle to already be loaded globally.

export function init(el, options) {
    // eslint-disable-next-line no-undef
    const instance = bootstrap.Modal.getOrCreateInstance(el, options || {});
    el._modalInstance = instance;
}

export function show(el) {
    el?._modalInstance?.show();
}

export function hide(el) {
    el?._modalInstance?.hide();
}

export function dispose(el) {
    if (el?._modalInstance) {
        el._modalInstance.dispose();
        delete el._modalInstance;
    }
}
