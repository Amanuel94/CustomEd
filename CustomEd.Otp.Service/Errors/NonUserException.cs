using System;

namespace CustomEd.Otp.Service.Errors
{
    public class NonUserException : Exception
    {
        public NonUserException()
            : base("User not found")
        {
        }
    }
}
