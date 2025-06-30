namespace Domain.Interfaces.Services.DbConfig
{
    public interface IServiceBaseConfig<T> where T : class
    {
        public IQueryable<T> GetByOdata();

        public T GetById(Guid Id);
        public Task<T> GetByIdAsync(Guid Id);

        public List<T> GetAll();
        public Task<List<T>> GetAllAsync();

        public T Add(T genericTable);
        public Task<T> AddAsync(T genericTable);

        public void Delete(T genericTable);
        public Task DeleteAsync(T genericTable);

        public bool Update(T genericTable);
        public Task<bool> UpdateAsync(T genericTable);

    }
}
