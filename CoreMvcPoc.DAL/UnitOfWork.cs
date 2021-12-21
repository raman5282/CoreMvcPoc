using CoreMvcPoc.Entities;

namespace CoreMvcPoc.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<User> userRepo;
        private IRepository<Book> bookRepo;
        private ApplicationDbContext context;
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IRepository<User> UserRepo{
            get{
                if(userRepo == null)
                {
                    userRepo = new Repository<User>(context);
                }
                return userRepo;
            }
        }

        public IRepository<Book> BookRepo{
            get{
                if(bookRepo == null)
                {
                    bookRepo = new Repository<Book>(context);
                }
                return bookRepo;
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}