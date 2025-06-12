using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppMeals.Validations
{
    public class Validator : IValidator
    {
        public string NameError{ get; set; } = "";
        public string EmailError { get; set; } = "";
        public string ContactError{ get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string EmptyNameErrorMSg = "Please enter your name.";
        private const string EmptyNameErrorMsg = "Please enter a valid name.";
        private const string EmptyEmailErrorMSg = "Please enter your email.";
        private const string InvalidEmailErrorMsg = "Please enter a valid email.";
        private const string EmptyContactErrorMSg = "Please enter your phone number.";
        private const string InvalidContactErrorMsg = "Please enter a valid phone number.";
        private const string EmptyPasswordErrorMSg = "Please enter a password.";
        private const string InvalidPasswordErrorMsg = "The password must be at least 8 characters long and include letters and numbers.";

        public Task<bool> Validate(string name, string email, string contact, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isContactValid = ValidateContact(contact);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isContactValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMSg;
                return false;
            }

            if(name.Length < 3)
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }
            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                NameError = EmptyEmailErrorMSg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidateContact(string contact)
        {
            if (string.IsNullOrEmpty(contact))
            {
                ContactError = EmptyContactErrorMSg;
                return false;
            }

            if (contact.Length <9)
            {
                ContactError = InvalidContactErrorMsg;
                return false;
            }

            ContactError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMSg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
