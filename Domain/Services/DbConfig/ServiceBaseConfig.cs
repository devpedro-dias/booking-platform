using Domain.Interfaces.Repositories.BaseConfig;
using Domain.Interfaces.Services.DbConfig;

namespace ecommerce.Domain.Services.DbConfig
{
    public class ServiceBaseConfig<T> : IServiceBaseConfig<T> where T : class
    {
        private readonly IRepositoryBaseConfig<T> _repository;

        public ServiceBaseConfig(IRepositoryBaseConfig<T> repository)
        {
            _repository = repository;
        }

        public T Add(T genericTable)
        {
            try
            {
                return _repository.Add(genericTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> AddAsync(T genericTable)
        {
            try
            {
                return await _repository.AddAsync(genericTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Delete(T genericTable)
        {
            try
            {
                _repository.Delete(genericTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteAsync(T genericTable)
        {
            try
            {
                await _repository.DeleteAsync(genericTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<T> GetAll()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public T GetById(Guid Id)
        {
            try
            {
                return _repository.GetById(Id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> GetByIdAsync(Guid Id)
        {
            try
            {
                return await _repository.GetByIdAsync(Id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IQueryable<T> GetByOdata()
        {
            try
            {
                return _repository.GetByOdata();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Update(T genericTable)
        {
            try
            {
                return _repository.Update(genericTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> UpdateAsync(T genericTable)
        {
            try
            {
                return _repository.UpdateAsync(genericTable);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
