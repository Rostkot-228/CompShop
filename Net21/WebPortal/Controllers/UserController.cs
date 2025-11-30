using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using WebPortal.Controllers.CustomAuthorizeAttributes;
using WebPortal.DbStuff.Models;
using WebPortal.DbStuff.Models.Notifications;
using WebPortal.DbStuff.Repositories;
using WebPortal.DbStuff.Repositories.Interfaces;
using WebPortal.Enum;
using WebPortal.Hubs;
using WebPortal.Models.Users;
using WebPortal.Services;

namespace WebPortal.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IUserRepositrory _userRepositrory;
        private IFileService _fileService;
        private readonly IAuthService _authService;
        private readonly INotificationRepository _notificationRepository;

        public UserController(
            IUserRepositrory userRepositrory,
            IAuthService authService,
            IFileService fileService,
            INotificationRepository notificationRepository)
        {
            _userRepositrory = userRepositrory;
            _authService = authService;
            _fileService = fileService;
            _notificationRepository = notificationRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var usersViewModels = _userRepositrory
                .GetAll()
                .Select(x => new UserViewModel
                {
                    UserName = x.UserName,
                    Role = x.Role
                })
                .ToList();

            return View(usersViewModels);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserViewModel userViewModel)
        {
            var userDb = new User
            {
                UserName = userViewModel.UserName,
                Password = userViewModel.Password,
                AvatarUrl = userViewModel.AvatarUrl,
                Money = userViewModel.Money,
            };
            _userRepositrory.Add(userDb);
            return RedirectToAction("Index");
        }

        public IActionResult ChangeLanguage(Language lang)
        {
            var user = _authService.GetUser();
            user.Language = lang;
            _userRepositrory.Update(user);
            return RedirectToAction("Index", "Home");
        }

        
        public IActionResult CompShopProfil()
        {
            var userDb = _authService.GetUser();

            var userViewModel = new UserViewModel
            {
                UserName = userDb.UserName,
            };

            return View(userViewModel);
        }

        [HttpPost]
        public IActionResult UpdateLanguage(Language language)
        {
            if (language == 0) return NotFound();

            var user = _authService.GetUser();

            if (user is null) return NotFound();

            user.Language = language;

            _userRepositrory.Update(user);

            return RedirectToAction("CompShopProfil");
        }

        [HttpPost]
        public IActionResult UpdateAvatar(IFormFile avatar)
        {
            _fileService.UploadAvatar(avatar);
            return RedirectToAction("Profile");
        }

        [Role(Role.Admin)]
        public IActionResult AllAvatars()
        {
            var users = _userRepositrory
                .GetAll()
                .Select(x => new UserIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.UserName,
                })
                .ToList();
            return View(users);
        }

        [Role(Role.Admin)]
        public IActionResult DeleteAvatar(int userId)
        {
            _fileService.ReplaceAvatarToDefault(userId);
            return RedirectToAction("AllAvatars");
        }

        [Role(Role.Admin)]
        [HttpGet]
        public IActionResult SetRoleUser()
        {
            var setRoleUsersViewModel = FillSetRoleUsersViewModel();

            return View(setRoleUsersViewModel);
        }

        private SetRoleUsersViewModel FillSetRoleUsersViewModel()
        {
            var meUser = _authService.GetUser();
            var allUser = _userRepositrory.GetAll().Where(user => user != meUser);

            var allUserVM = allUser.Select(user => new SelectListItem
            {
                Text = user.UserName,
                Value = user.Id.ToString(),
            }).ToList();

            var allRoles = System.Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = ((int)x).ToString()
                }).ToList();


            var setRoleUsersViewModel = new SetRoleUsersViewModel
            {
                AllRoles = allRoles,
                AllUsers = allUserVM,
            };

            return setRoleUsersViewModel;
        }

        [Role(Role.Admin)]
        [HttpPost]
        public IActionResult SetRoleUser(int userId, int roleId)
        {
            if (!ModelState.IsValid)
            {
                var setRoleUsersViewModel = FillSetRoleUsersViewModel();
                return View(setRoleUsersViewModel);
            }

            var user = _userRepositrory.GetFirstById(userId);

            if(user == null)
            {
                throw new ArgumentNullException("User can't be null", nameof(user));
            }

            user.Role = (Role)roleId;

            _userRepositrory.Update(user);

            //send message
            var author = _authService.GetUser();
            var message = $"User '{user.UserName}' became a {user.Role}"; 

            var notification = new Notification
            {
                CreateAt = DateTime.Now,
                Message = message,
                Author = author,
                LevelNotification = Role.Admin
            };
            _notificationRepository.Add(notification);

            return RedirectToAction("Index");
        }
    }
}
