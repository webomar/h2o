using h2o_library.Models.Base;

namespace h2o_library.Abstract.Repositories;

public interface IBaseRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetAsync(Guid id);
    Task AddAsync(TEntity entity);
    void Remove(TEntity entity);
}