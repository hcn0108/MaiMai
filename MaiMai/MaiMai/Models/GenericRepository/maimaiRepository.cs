using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MaiMai.Models.GenericRepository;


/// <summary>
///  介面實作  CRUD
/// </summary>
namespace MaiMai.Models
{
    public class maimaiRepository<Table> : IRepository<Table> where Table : class
    {
        maimaiEntities  whdb = null;
        DbSet<Table> dbTable = null;

        public maimaiRepository()
        {
            whdb = new maimaiEntities();
            dbTable = whdb.Set<Table>();
        }
        public void Create(Table _entity)
        {
            dbTable.Add(_entity);
            whdb.SaveChanges();
        }

        public void Delete(int id)
        {
            dbTable.Remove(GetbyID(id));
            whdb.SaveChanges();
        }

        public IEnumerable<Table> GetAll()
        {
            return dbTable;
        }

        public Table GetbyID(int id)
        {
            return dbTable.Find(id);
        }

        public void Update(Table _entity)
        {
            whdb.Entry<Table>(_entity).State = EntityState.Modified;
            whdb.SaveChanges();
        }
    
    }
}