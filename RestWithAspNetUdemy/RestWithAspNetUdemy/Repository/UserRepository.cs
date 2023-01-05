using RestWithAspNetUdemy.Data.VO;
using RestWithAspNetUdemy.Model;
using RestWithAspNetUdemy.Model.Context;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace RestWithAspNetUdemy.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlContext _context;

        public UserRepository(MySqlContext context)
        {
            _context = context;
        }

        public User ValidateCredentials(UserVO user)
        {
            var pass = ComputeHash(user.Password, SHA256.Create());

            return _context.Users.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == pass));
        }

        public User RefreshUserInfo(User user)
        {
            if (!_context.Users.Any(p => p.Id.Equals(user.Id)))
            {
                return null;
            }

            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));

            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return result;
        }

        public bool RevokeToken(string userName)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == userName);

            if (user is null) return false;

            user.RefreshToken = null;
            _context.SaveChanges();

            return true;
        }

        public User ValidateCredentials(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        private string ComputeHash(string input, SHA256 sHA512)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = sHA512.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }
    }
}
