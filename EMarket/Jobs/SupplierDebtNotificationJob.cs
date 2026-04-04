using System;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Events.Interfaces; // Link tới Dispatcher nếu muốn dùng Event
using EMarket.Models; // Link tới DbContext của bạn

public class SupplierDebtNotificationJob
{
    private readonly EMarket_DBEntities _db;
    private readonly IEventDispatcher _dispatcher;

    // Hangfire sẽ lấy các tham số này từ Simple Injector
    public SupplierDebtNotificationJob(EMarket_DBEntities db, IEventDispatcher dispatcher)
    {
        _db = db;
        _dispatcher = dispatcher;
    }

    public async Task NotifyNearDueDebts()
    {
        var today = DateTime.Today;
        var thresholdDate = today.AddDays(3);

        var nearDueIds = _db.SupplierDebts
            .Where(x => x.unpaid_amount > 0 && x.due_date <= thresholdDate && x.due_date >= today)
            .Select(x => x.debt_id)
            .ToList();

        if (nearDueIds.Any())
        {
            _dispatcher.DispatchAsync(new SupplierDebtNearDueEvent(nearDueIds));
        }
    }

    public async Task NotifyOverdueDebts()
    {
        var today = DateTime.Today;

        var overdueIds = _db.SupplierDebts
            .Where(x => x.unpaid_amount > 0 && x.due_date < today)
            .Select(x => x.debt_id)
            .ToList();

        if (overdueIds.Any())
        {
            _dispatcher.DispatchAsync(new SupplierDebtOverdueEvent(overdueIds));
        }
    }
}