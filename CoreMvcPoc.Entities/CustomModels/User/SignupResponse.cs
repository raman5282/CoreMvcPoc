using System;
using System.Collections.Generic;

namespace CoreMvcPoc.Entities
{
    public class SignupResponse
    {
         private string successMessage;
        public SignupResponse(string successMessage)
        {
            this.successMessage = successMessage;
        }

        public SignupResponse()
        {
        }

        public string SuccessMessage
        {
            get { return successMessage; }
            set { successMessage = value; }
        }
        public static implicit operator SignupResponse(string successMessage)
        {
            // While not technically a requirement; see below why this is done.
            if (successMessage == null)
                return null;

            return new SignupResponse(successMessage);
        }
        public List<string> DuplicateUserName { get; set; }
        public List<string> InvalidUserName { get; set; }
        public List<string> PasswordTooShort { get; set; }
        public List<string> PasswordRequiresUniqueChars { get; set; }
    }
}