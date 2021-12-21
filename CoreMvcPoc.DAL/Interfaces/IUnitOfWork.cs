using CoreMvcPoc.Entities;

namespace CoreMvcPoc.DAL
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepo { get; }
        IRepository<Book> BookRepo { get; }
        void Save();
    }
}