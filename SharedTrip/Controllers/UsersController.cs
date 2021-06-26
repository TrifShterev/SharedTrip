using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyWebServer.Controllers;
using MyWebServer.Http;
using SharedTrip.Services;
using SharedTrip.ViewModels.Users;

namespace SharedTrip.Controllers
{
    public class UsersController :Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        public HttpResponse Login()
        {
            if (this.User.IsAuthenticated)
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost]
        public HttpResponse Login(LoginUserViewModel model)
        {
            if (this.User.IsAuthenticated)
            {
                return this.Redirect("/");
            }

            var userId = this.usersService.GetUserId(model.Username, model.Password);

            if (userId == null)
            {
                return this.Error("Invalid username ot password.");

            }

            this.SignIn(userId);

            return this.Redirect("/Trips/All");
        }
        public HttpResponse Register()
        {
            if (this.User.IsAuthenticated)
            {
                return this.Redirect("/");
            }
            return this.View();
        }

        [HttpPost]
        public HttpResponse Register(UserRegisterViewModel model)
        {
            if (this.User.IsAuthenticated)
            {
                return this.Redirect("/");
            }

            var modelErrors=  Validator(model);

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            this.usersService.Create(model.Username, model.Email, model.Password);

            return this.Redirect("/Users/Login");

        }

        public HttpResponse Logout()
        {

            if (this.User.IsAuthenticated)
            {
                this.SignOut();
            }
            return this.Redirect("/");
        }

        private ICollection<string> Validator(UserRegisterViewModel model)
        {
            var errors = new List<string>();

            
            if (model.Username.Length < 5 || model.Username.Length > 20)
            {
                errors.Add("Username should be between [5-20] characters.");
            }

            if (String.IsNullOrEmpty(model.Email) || !new EmailAddressAttribute().IsValid(model.Email))
            {
                errors.Add("Invalid email.");
            }

            if (model.Password.Length < 6 || model.Password.Length > 20)
            {
                errors.Add("The passwords should be between [6-20] characters.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                errors.Add("The passwords do not match.");

            }

            if (!this.usersService.IsUsernameAvailable(model.Username))
            {
                errors.Add("The username already exist.");
            }

            if (!this.usersService.IsEmailAvailable(model.Email))
            {
                errors.Add("Email already exist.");
            }

            return errors;
        }
    }
}