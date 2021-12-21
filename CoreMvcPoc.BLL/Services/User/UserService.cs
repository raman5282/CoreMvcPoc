using System.Threading.Tasks;
using CoreMvcPoc.DAL;
using CoreMvcPoc.Entities;

namespace CoreMvcPoc.BLL
{
    public class UserService : IUserService
    {
        //private IRepository<User> userRepository;
        private IUnitOfWork unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public bool CheckUserToken(int userId, string token)
        {
            return unitOfWork.UserRepo.Get(x=>x.Id.Equals(userId)&& x.Token.Equals(token))!=null;
        }

        public User Login(string username, string password)
        {
            return unitOfWork.UserRepo.Get(x=>x.Email.Equals(username)&& BCrypt.Net.BCrypt.Verify(password, x.Password));
        }

        public bool Logout(int id)
        {
            bool returnVal=false;
            try
            {
                var user = unitOfWork.UserRepo.Get(x=>x.Id.Equals(id));
                if(user!=null)
                {
                    user.Token = string.Empty;
                    unitOfWork.UserRepo.Update(user);                    
                    returnVal=true;
                }               
            }
            catch
            {
                returnVal = false;
            }
            return returnVal;   
        }

        public ApiResponse SignUp(User model)
        {
            ApiResponse returnVal = new ApiResponse{Status=false};
            try
            {
                if(unitOfWork.UserRepo.Get(x=>x.Email.Equals(model.Email))!=null)
                {
                    returnVal.Status=false;
                    returnVal.Message = "Email already exist.";
                }
                else
                {
                    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    returnVal.Status = unitOfWork.UserRepo.Insert(model);
                }
            }
            catch
            {
                returnVal.Status = false;
                returnVal.Message = "Error in Sign up";
            }
            return returnVal;
        }

        public bool UpdateUser(User model)
        {
            bool returnVal;
            try
            {
                returnVal = unitOfWork.UserRepo.Update(model);
            }
            catch
            {
                returnVal = false;
            }
            return returnVal;   
        }
    }
}