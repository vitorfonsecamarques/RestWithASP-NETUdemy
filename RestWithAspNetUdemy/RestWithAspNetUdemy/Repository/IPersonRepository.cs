using RestWithAspNetUdemy.Model;
using RestWithAspNetUdemy.Repository.Generic;

namespace RestWithAspNetUdemy.Repository
{
    public interface IPersonRepository : IRepository<Person>
    {
        Person Disable(long id);
    }
}
