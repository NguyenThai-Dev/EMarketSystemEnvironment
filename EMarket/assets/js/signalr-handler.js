var SignalRManager = (function () {
    var isInitialized = false;

    return {
        init: function (explicitBranchId) {
            if (isInitialized) return;

            let branchId = (explicitBranchId !== undefined) ? explicitBranchId : (window.CURRENT_BRANCH_ID || "");
            const isAdmin = window.IS_ADMIN === true || window.IS_ADMIN === "True";

            $.connection.hub.qs = {
                branchId: branchId.toString(),
                isAdmin: isAdmin
            };

            var orderHub = $.connection.orderHub;

            orderHub.client.orderChanged = null;

            orderHub.client.orderChanged = function (payload) {
                if (payload.isTest) {
                    console.log("[TEST SIGNALR]:", payload.message);
                    if (typeof toastr !== 'undefined') toastr.success(payload.message);
                    return;
                }

                console.warn("[SIGNALR] ĐÃ NHẬN TIN:", payload);

                const safeReload = (selector, label) => {
                    const $el = $(selector);
                    if ($el.length > 0 && $.fn.DataTable.isDataTable(selector)) {
                        try {
                            const table = $el.DataTable();
                            console.log(`🔄 Reloading ${label} (Server-side)...`);
                            table.ajax.reload(null, false);
                        } catch (err) {
                            console.warn(`⚠️ Chưa reload được ${label}, DataTable đang bận.`);
                        }
                    }
                };
                setTimeout(() => {
                    safeReload('#tblOrders', 'Orders');
                    safeReload('#tblStock', 'Stock');
                }, 1000);
            };
            $.connection.hub.start()
                .done(function () {
                    isInitialized = true;
                    console.log("✅ Kết nối Hub thành công!");
                    orderHub.server.whoAmI();
                    window.currentSignalRConnectionId = $.connection.hub.id;
                })
                .fail(function (err) {
                    console.error("❌ Lỗi start hub:", err);
                });

        }
    };
})();