using Domain.Interfaces.Repositories.BaseConfig;

using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository.DbConfig;

public class RepositoryBaseConfig<T> : IRepositoryBaseConfig<T> where T : class
{
    readonly AppDbContext db;

    public RepositoryBaseConfig(AppDbContext context)
    {
        db = context ?? throw new ArgumentNullException(nameof(context));
    }

    public T Add(T genericTable)
    {
        try
        {
            db.Set<T>().Add(genericTable);
            db.SaveChanges();
            return genericTable;
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
            await db.Set<T>().AddAsync(genericTable);
            await db.SaveChangesAsync();
            return genericTable;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public T Delete(T genericTable)
    {
        try
        {
            db.Set<T>().Remove(genericTable);
            db.SaveChanges();
            return genericTable;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<T> DeleteAsync(T genericTable)
    {
        db.Set<T>().Remove(genericTable);
        await db.SaveChangesAsync();
        return genericTable;
    }

    public List<T> GetAll()
    {
        try
        {
            return db.Set<T>().ToList();
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
            return await db.Set<T>().ToListAsync();
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
            var response = db.Set<T>().Find(Id);

            if (response != null)
            {
                db.Entry(response).State = EntityState.Detached;
            }
            return response;
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
            var retorno = await db.Set<T>().FindAsync(Id);
            if (retorno != null)
            {
                db.Entry(retorno).State = EntityState.Detached;
            }
            return retorno;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public IQueryable<T> GetByOdata()
    {
        return db.Set<T>().AsQueryable();
    }

    public bool Update(T genericTable)
    {
        db.Entry(genericTable).State = EntityState.Modified;
        return db.SaveChanges() > 0;
    }

    public async Task<bool> UpdateAsync(T genericTable)
    {
        db.Set<T>().Update(genericTable);
        return await db.SaveChangesAsync() > 0;
    }
}
