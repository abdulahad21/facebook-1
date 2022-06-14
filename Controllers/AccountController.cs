using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Facebook.DataBaseContext;
using Facebook.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Facebook.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<CustomUserFields> _userManager;
        private readonly SignInManager<CustomUserFields> _signInManager;
        private readonly PostContext _postContext;
        private readonly CommentContext _commentContext;
        private readonly UserContext _userContext;

        public AccountController(UserManager<CustomUserFields> userManager,
                              SignInManager<CustomUserFields> signInManager, PostContext postContext, CommentContext commentContext, UserContext userContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _postContext = postContext;
            _commentContext = commentContext;
            _userContext = userContext;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new CustomUserFields
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Mobile
                };


                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserAccount(RegisterModel model)
        {
            if (model.Email != null)
            {
                return RedirectToAction("Profile", "Home");
            }

            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.Mobile;

            _userContext.Entry(user).State = EntityState.Modified;
            bool userUpdated = false;

            try
            {
                await _userContext.SaveChangesAsync();
                userUpdated = true;
            }
            catch (Exception)
            {
                userUpdated = false;
            }


            if (userUpdated)
            {
                var posts = await _postContext.Posts.ToListAsync();
                var comments = await _commentContext.Comments.ToListAsync();

                foreach (var post in posts)
                {
                    if (User.Identity.Name == post.Email)
                    {
                        post.FirstName = model.FirstName;
                        post.LastName = model.LastName;
                    }
                }

                foreach (var comment in comments)
                {
                    if (User.Identity.Name == comment.Email)
                    {
                        comment.FirstName = model.FirstName;
                        comment.LastName = model.LastName;
                    }
                }

                await _postContext.SaveChangesAsync();
                await _commentContext.SaveChangesAsync();

            }


            return RedirectToAction("Profile", "Home");
        }

        // GET: Account/SearchUser/string
        public async Task<IActionResult> SearchUser(string strToSearch)
        {
            var users = await _userContext.AspNetUsers.ToListAsync();
            List<SearchUserModel> usersMatched = new List<SearchUserModel>();
            
            if (strToSearch != null)
            {
                foreach (var user in users)
                {
                    if (user.Email == User.Identity.Name)
                    {
                        continue;
                    }

                    if (StringMatched(strToSearch, user.FirstName) || StringMatched(strToSearch, user.LastName) || user.Email == strToSearch || user.PhoneNumber == strToSearch)
                    {
                        bool isAdded = true;

                        if (usersMatched.Count > 0)
                        {
                            foreach (var item in usersMatched)
                            {
                                if (item.Email == user.Email)
                                {
                                    isAdded = false;
                                    continue;
                                }
                            }
                        }

                        if (isAdded)
                        {
                            SearchUserModel searchUser = new SearchUserModel();
                            searchUser.Email = user.Email;
                            searchUser.FirstName = user.FirstName;
                            searchUser.LastName = user.LastName;
                            searchUser.Phone = user.PhoneNumber;

                            usersMatched.Add(searchUser);
                        }
                        
                    }
                }
            }

            return View("~/Views/Home/SearchUser.cshtml", usersMatched);
        }

        public async Task<IActionResult> Profile(string email)
        {
            return View("~/Views/Home/UserProfile.cshtml", await _postContext.Posts.Where(x => x.Email == email).ToListAsync());
        }

        //to check if string contains given string or not
        private bool StringMatched(string strToSearch, string str)
        {
            return Regex.IsMatch(strToSearch, Regex.Escape(str), RegexOptions.IgnoreCase);
        }

    }
}