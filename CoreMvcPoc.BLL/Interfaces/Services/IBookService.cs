using System;
using System.Threading.Tasks;
using ServiceManager;
using System.Net.Http;
using CoreMvcPoc.Entities;
using System.Collections.Generic;

namespace CoreMvcPoc.BLL
{
    public interface IBookService
    {
         List<Book> GetUserBooks(int userId, string sortby);
         PagedList<Book> GetUserBooks(int userId, string sortby, PagingParams pagingParams, string filter);
         bool AddUserBook(Book model);
         bool UpdateUserBook(Book model);
         bool DeleteUserBook(int id);
         Book GetBook(int id);        
    }
}