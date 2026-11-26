using System.Linq.Expressions;
using GePro.DataAccess.Extensions;
using GePro.Model.Base;
using Microsoft.EntityFrameworkCore;

namespace GePro.DataAccess.Repositories
{
    public class Repository<TEntity>(DbModelContext context) : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected DbSet<TEntity> DbSet { get; } = context.Set<TEntity>();

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct) =>
            await DbSet.FindAsync([id], ct);

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) =>
            await DbSet.FirstOrDefaultAsync(filter, ct);

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) =>
            await DbSet.AnyAsync(filter, ct);

        public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken ct)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();
            if (filter is not null)
                query = query.Where(filter);

            return await query.ToListAsync(ct);
        }

        public async Task<PagedDataResponse<TEntity>> GetPagedAsync(
            PagingSortFilterRequest request,
            Expression<Func<TEntity, bool>>? filter,
            CancellationToken ct)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();
            if (filter is not null)
                query = query.Where(filter);

            query = query.GetQueryFilterBy(request.FilterBy, request.FilterValue);

            var rowCount = await query.CountAsync(ct);

            query = query.GetQueryOrderBy(request.OrderBy).GetQueryPaged(request.PageSize, request.PageIndex, request.OrderBy);

            var results = await query.ToListAsync(ct);

            return new PagedDataResponse<TEntity>(request.PageIndex, request.PageSize, rowCount, results);
        }

        public async Task AddAsync(TEntity entity, CancellationToken ct) =>
            await DbSet.AddAsync(entity, ct);

        public void Remove(TEntity entity) => DbSet.Remove(entity);
    }
}
