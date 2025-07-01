using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.BaseConfig;

public interface IRepositoryBaseConfig<T> where T : class
{
    public IQueryable<T> GetByOdata();
    public T GetById(Guid Id);
    public Task<T> GetByIdAsync(Guid Id);
    public T Add(T genericTable);
    public Task<T> AddAsync(T genericTable);
    public bool Update(T genericTable);
    public Task<bool> UpdateAsync(T genericTable);
    public T Delete(T genericTable);
    public Task<T> DeleteAsync(T genericTable);
    public List<T> GetAll();
    public Task<List<T>> GetAllAsync();
}

