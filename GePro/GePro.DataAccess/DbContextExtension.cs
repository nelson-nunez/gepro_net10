using System.Linq.Expressions;
using System.Reflection;
using GePro.Model.Base;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GePro.DataAccess
{
    internal static class DbContextExtension
    {
        private static readonly MethodInfo SoftDeleteFilterMethod =
            typeof(DbContextExtension).GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!;

        /// <summary>Oculta los registros con borrado lógico e indexa la columna Deleted.</summary>
        public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
        {
            var filter = (LambdaExpression)SoftDeleteFilterMethod.MakeGenericMethod(entityType.ClrType).Invoke(null, null)!;
            entityType.SetQueryFilter(filter);
            entityType.AddIndex(entityType.FindProperty(nameof(ISoftDelete.Deleted))!);
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.Deleted.HasValue;
            return filter;
        }
    }
}
