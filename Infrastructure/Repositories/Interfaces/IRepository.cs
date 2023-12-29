namespace TaskCircle.GroupManagerApi.Infrastructure.Repositories.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(int id);
    Task<T> Create(T Entity);
    Task<T> Update(T Entity);
    Task<T> Delete(int id);
}
