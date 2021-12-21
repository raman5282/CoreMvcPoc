using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMvcPoc.Entities;
using CoreMvcPoc.DAL;
using System.Linq;

namespace CoreMvcPoc.BLL
{
    public class BookService : IBookService
    {
        //private IRepository<Book> bookRepository;
        private IUnitOfWork unitOfWork;
        public BookService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public bool AddUserBook(Book model)
        {
           return unitOfWork.BookRepo.Insert(model);
        }

        public bool DeleteUserBook(int id)
        {
            var book = unitOfWork.BookRepo.Get(id);
            if(book==null)
            {
                return false;
            }
            return unitOfWork.BookRepo.Delete(book);
        }

        public Book GetBook(int id)
        {
             return unitOfWork.BookRepo.Get(id);
        }

        public List<Book> GetUserBooks(int userId, string sortby)
        {
            var books = unitOfWork.BookRepo.GetAll(x=>x.UserId.Equals(userId));
           if(books!=null && books.Count() > 0)
           {
                switch(sortby)
                {
                    case "name_desc":
                            books = books.OrderByDescending(s => s.Name);
                            break;
                    case "price_desc":
                            books = books.OrderByDescending(s => s.Price);
                            break;  
                    case "price":
                            books = books.OrderBy(s => s.Price);
                            break;  
                    case "author_desc":
                            books = books.OrderByDescending(s => s.Author);
                            break;   
                    case "author":
                            books = books.OrderBy(s => s.Author);
                            break;   
                    default:
                            books = books.OrderBy(s => s.Name);
                            break;                
                }
           }
           return books !=null ? books.ToList() : new List<Book>();
        }

        public PagedList<Book> GetUserBooks(int userId, string sortby, PagingParams pagingParams, string filter)
        {
           var books = unitOfWork.BookRepo.GetAll(x=>x.UserId.Equals(userId));
           if(books!=null && books.Count()>0)
           {
               if(!string.IsNullOrEmpty(filter))
               {
                   books=books.Where(x=>x.Name.Contains(filter));
               }
                switch(sortby)
                {
                    case "name_desc":
                            books = books.OrderByDescending(s => s.Name);
                            break;
                    case "price_desc":
                            books = books.OrderByDescending(s => s.Price);
                            break;  
                    case "price":
                            books = books.OrderBy(s => s.Price);
                            break;  
                    case "author_desc":
                            books = books.OrderByDescending(s => s.Author);
                            break;   
                    case "author":
                            books = books.OrderBy(s => s.Author);
                            break;   
                    default:
                            books = books.OrderBy(s => s.Name);
                            break;                
                }
           }
           books = books ?? new List<Book>().AsEnumerable();
           return new PagedList<Book>(books.AsQueryable(), pagingParams.PageNumber, pagingParams.PageSize);
        }

        public bool UpdateUserBook(Book model)
        {
            return unitOfWork.BookRepo.Update(model);
        }
    }
}