using RestWithAspNetUdemy.Data.VO;
using RestWithAspNetUdemy.Model;

namespace RestWithAspNetUdemy.Business
{
    public interface IBookBusiness
    {
        BookVO Create(BookVO book);
        BookVO FindByID(long id);
        List<BookVO> FindAll();
        BookVO Update(BookVO book);
        void Delete(long id);
    }
}
