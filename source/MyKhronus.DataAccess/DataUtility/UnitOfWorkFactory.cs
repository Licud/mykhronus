namespace MyKhronus.DataAccess.DataUtility;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MyKhronus.DataAccess.Context;

internal class UnitOfWorkFactory(
    ILoggerFactory loggerFactory,
    IDbContextFactory<MyKhronusContext> contextFactory)
    : IUnitOfWorkFactory
{
    public IUnitOfWork Create()
    {
        return new MyKhronusContextUnitOfWork(loggerFactory, contextFactory);
    }
}
