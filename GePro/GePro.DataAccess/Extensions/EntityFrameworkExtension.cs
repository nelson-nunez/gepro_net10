using System.Linq.Expressions;
using LinqKit;

namespace GePro.DataAccess.Extensions
{
    /// <summary>Filtro, orden y paginación dinámicos. Uso interno de los repositorios.</summary>
    internal static class EntityFrameworkExtension
    {
        /// <summary>
        /// Filtra con "contiene". <paramref name="filterBy"/> admite varias propiedades separadas
        /// por '|' (se combinan con OR) y rutas de navegación con '.', ej. "Name|Province.Name".
        /// </summary>
        public static IQueryable<T> GetQueryFilterBy<T>(this IQueryable<T> query, string? filterBy, string? filterValue)
        {
            if (string.IsNullOrEmpty(filterBy) || string.IsNullOrEmpty(filterValue))
                return query;

            Expression<Func<T, bool>>? predicate = null;
            foreach (var property in filterBy.Split('|'))
            {
                var condition = ExpressionBuilder.BuildPredicate<T>(filterValue, OperatorComparer.Contains, property.Split('.'));
                predicate = predicate is null ? condition : predicate.Or(condition);
            }

            return query.Where(predicate!);
        }

        /// <summary>Ordena por propiedad o ruta ("Province.Name"); prefijo '-' para descendente.</summary>
        public static IQueryable<T> GetQueryOrderBy<T>(this IQueryable<T> query, string? orderBy)
        {
            return string.IsNullOrEmpty(orderBy) ? query : OrderByPath(query, orderBy);
        }

        /// <summary>Pagina (pageIndex base 0). Si no hay orden, ordena por Id para que la página sea estable.</summary>
        public static IQueryable<T> GetQueryPaged<T>(this IQueryable<T> query, int pageSize, int pageIndex, string? orderBy)
        {
            if (pageSize <= 0)
                return query;

            if (string.IsNullOrEmpty(orderBy))
                query = OrderByPath(query, "Id");

            return query.Skip(pageSize * pageIndex).Take(pageSize);
        }

        private static IQueryable<T> OrderByPath<T>(IQueryable<T> query, string orderBy)
        {
            var descending = orderBy.StartsWith('-');
            var path = orderBy.TrimStart('-');

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body = parameter;
            try
            {
                foreach (var member in path.Split('.'))
                    body = Expression.PropertyOrField(body, member);
            }
            catch (ArgumentException)
            {
                // Propiedad inexistente: se ordena por Id en lugar de fallar por un dato de entrada.
                if (path == "Id")
                    throw;
                return OrderByPath(query, "Id");
            }

            var method = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
            var call = Expression.Call(
                typeof(Queryable),
                method,
                [typeof(T), body.Type],
                query.Expression,
                Expression.Quote(Expression.Lambda(body, parameter)));

            return query.Provider.CreateQuery<T>(call);
        }
    }
}
