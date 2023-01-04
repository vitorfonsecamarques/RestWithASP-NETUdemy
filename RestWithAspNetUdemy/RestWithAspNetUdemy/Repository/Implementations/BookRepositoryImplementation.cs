using RestWithAspNetUdemy.Model;
using RestWithAspNetUdemy.Model.Context;

namespace RestWithAspNetUdemy.Repository.Implementations
{
    public class BookRepositoryImplementation : IBookRepository
    {
        private MySqlContext _context;

        public BookRepositoryImplementation(MySqlContext context) 
        {
            _context = context;    
        }

        public List<Book> FindAll()
        {
            return _context.Book.ToList();
        }

        public Book FindByID(long id)
        {
            return _context.Book.SingleOrDefault(p => p.Id.Equals(id));
        }

        public Book Create(Book book)
        {
            try
            {
                _context.Add(book);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return book;
        }

        public Book Update(Book book)
        {
            if (!Exists(book.Id)) return null;

            var result = _context.Book.SingleOrDefault(p => p.Id.Equals(book.Id));

            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(book);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return book;
        }

        public void Delete(long id)
        {
            var result = _context.Book.SingleOrDefault(p => p.Id.Equals(id));

            if (result != null)
            {
                try
                {
                    _context.Book.Remove(result);
                    _context.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private bool Exists(long id)
        {
            return _context.Book.Any(p => p.Id.Equals(id));
        }
    }
}
