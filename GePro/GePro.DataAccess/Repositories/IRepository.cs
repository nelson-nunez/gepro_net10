using System.Linq.Expressions;
using GePro.Model.Base;

namespace GePro.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio genérico. No expone IQueryable: las consultas específicas
    /// se agregan como métodos con nombre de dominio en un IXxxRepository.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct);

        /// <summary>Lista sin seguimiento de cambios (solo lectura).</summary>
        Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken ct);

        Task<PagedDataResponse<TEntity>> GetPagedAsync(
            PagingSortFilterRequest request,
            Expression<Func<TEntity, bool>>? filter,
            CancellationToken ct);

        Task AddAsync(TEntity entity, CancellationToken ct);

        /// <summary>Marca la entidad como eliminada; el Unit of Work aplica el borrado lógico.</summary>
        void Remove(TEntity entity);
    }
}
