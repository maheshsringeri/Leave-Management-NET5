using System;
using System.Collections.Generic;
using System.Linq;
namespace Leave_Management_NET5.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        ICollection<T> findAll();
        T FindById(int id);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Save();
    }
}
