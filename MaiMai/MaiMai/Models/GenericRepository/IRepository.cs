using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace MaiMai.Models.GenericRepository
{
    interface IRepository<Table>
    {
        IEnumerable<Table> GetAll();

        void Create(Table _entity);

        void Update(Table _entity);

        Table GetbyID(int id);

        void Delete(int id);

         
    }
}
