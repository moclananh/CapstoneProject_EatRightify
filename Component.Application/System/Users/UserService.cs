using Component.Application.AI;
using Component.Application.Common;
using Component.Application.Utilities.Mail;
using Component.Data.EF;
using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.System.Users;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Component.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IStorageService _storageService;

        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration config,
            ApplicationDbContext context,
            IEmailService emailService,
            IStorageService storageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
            _emailService = emailService;
            _storageService = storageService;
        }

        public async Task<LoginRespone<string>> Authencate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return new LoginErrorRespone<string>("Account is not exist");
            if (user.IsBanned == true) return new LoginErrorRespone<string>("Account has been banned");
            if (user.IsVerify == false) return new LoginErrorRespone<string>("Account is not verify");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
            {
                return new LoginErrorRespone<string>("Wrong username or password");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.GivenName, user.FirstName),
        new Claim(ClaimTypes.Role, string.Join(";", roles)),
        new Claim(ClaimTypes.Name, request.UserName),
        new Claim(ClaimTypes.Dsa, user.Id.ToString()),
    };

            var loginResult = new LoginResult
            {
                ID = user.Id,
            };
            Guid id = loginResult.ID;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new LoginRespone<string>(new JwtSecurityTokenHandler().WriteToken(token), id);
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("User is not exist");
            }
            var reult = await _userManager.DeleteAsync(user);
            if (reult.Succeeded)
                return new ApiSuccessResult<bool>();

            return new ApiErrorResult<bool>("Delete fail");
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User is not exist");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Dob = user.Dob,
                Id = user.Id,
                LastName = user.LastName,
                UserName = user.UserName,
                Roles = roles,
                IsBanned = user.IsBanned,
                VIP = user.VIP,
                AccumulatedPoints = user.AccumulatedPoints,
                Avatar = user.Avatar,
                AcceptedTermOfUse = user.AcceptedTermOfUse,
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UserName.Contains(request.Keyword)
                 || x.PhoneNumber.Contains(request.Keyword));
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    Id = x.Id,
                    Dob = x.Dob,
                    LastName = x.LastName,
                    IsBanned = x.IsBanned,
                    VIP = x.VIP,
                    AccumulatedPoints = x.AccumulatedPoints,
                    Avatar = x.Avatar,
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Account is exist");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Email is exist");
            }

            user = new AppUser()
            {
                Dob = request.Dob,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsVerify = false,
                CreatedDate = DateTime.UtcNow,
                AcceptedTermOfUse = false
                
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Register fail");
        }

        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Account is not exist");
            }
            var removedRoles = request.Roles.Where(x => x.Selected == false).Select(x => x.Name).ToList();
            foreach (var roleName in removedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            var addedRoles = request.Roles.Where(x => x.Selected).Select(x => x.Name).ToList();
            foreach (var roleName in addedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }

            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<string>> Update(Guid id, UserUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<string>("Email is exist");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.Dob = request.Dob;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(request.Avatar) && IsBase64String(request.Avatar))
            {
                user.Avatar = await _storageService.SaveImageAsync(request.Avatar);
            }
            else
            {
                request.Avatar = user.Avatar;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<string>();
            }
            return new ApiErrorResult<string>("Update fail");
        }
        private bool IsBase64String(string s)
        {
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiResult<bool>> BanAccount(Guid id, bool status)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("User is not exist");
            }
            user.IsBanned = status;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<string>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiErrorResult<string>("Email not found");
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            Random generator = new Random();
            string r = generator.Next(0, 1000000).ToString("D6");
            DateTime expire = DateTime.Now;
            user.RefeshToken = resetToken;
            user.RefeshCode = r;
            user.RefeshTokenExpire = expire;
            await _context.SaveChangesAsync();
            var subject = "Password Reset Request";
            var body = $"This is your refresh password code: <strong>{r}</strong>";
            try
            {
                // Call the EmailService to send the password reset email
                await _emailService.SendPasswordResetEmailAsync(email, subject, body);
                return new ApiSuccessMessage<string>("Password reset email sent");
            }
            catch
            {
                // Handle the exception as needed
                return new ApiErrorResult<string>("Error sending password reset email");
            }
        }

        public async Task<ApiResult<string>> ResetPassword(ResetPasswordRequest request)
        {
            string token = "";
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Handle case where email doesn't exist
                return new ApiErrorResult<string>("Email not found");
            }
            // Calculate the time difference
            DateTime tmp = DateTime.Now;
            TimeSpan timeDifference = (TimeSpan)(tmp - user.RefeshTokenExpire);

            Console.WriteLine("Current Time: " + tmp);
            Console.WriteLine("Expire Token Time: " + user.RefeshTokenExpire);
            Console.WriteLine("Time Difference: " + timeDifference);
            Console.WriteLine("Total Minutes Difference: " + timeDifference.TotalMinutes);
            if (request.VerifyCode == user.RefeshCode)
            {
                token = user.RefeshToken;
            }
            // Check if the time difference is greater than 1 minute
            if (timeDifference.TotalMinutes > 2)
            {
                return new ApiErrorResult<string>("Token has expired");
            }
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return new ApiErrorResult<string>("The new password and confirm password do not match");
            }

            // Validate the password reset token
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (result.Succeeded)
            {
                // Password successfully reset
                return new ApiSuccessMessage<string>("Password reset successful");
            }
            else
            {
                // Handle password reset failure
                return new ApiErrorResult<string>("Password reset failed. Make sure your refesh code is correct");
            }
        }


        public async Task<ApiResult<string>> UpdatePassword(Guid id, UpdatePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.oldPassword);
                if(!isOldPasswordCorrect)
                {
                    return new ApiErrorResult<string>("Old password not correct");
                }
                var result = await _userManager.ChangePasswordAsync(user, request.oldPassword, request.newPassword);
                if (result.Succeeded)
                {
                    return new ApiSuccessMessage<string>("Update successfull");
                }
            }
            return new ApiErrorResult<string>("Update fail, please make sure new password is valid format");
        }

        public async Task<ApiResult<string>> GetVerifyCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiErrorResult<string>("Email not found");
            }
            Random generator = new Random();
            string r = generator.Next(0, 1000000).ToString("D6");
            user.VerifyCode = r;
            await _context.SaveChangesAsync();
            var subject = "Account verify";
            var body = $"This is your verify code: <strong>{r}</strong>";
            try
            {
                await _emailService.SendPasswordResetEmailAsync(email, subject, body);
                return new ApiSuccessMessage<string>(" Verify email sent");
            }
            catch
            {
                // Handle the exception as needed
                return new ApiErrorResult<string>("Error sending verify email");
            }
        }

        public async Task<ApiResult<string>> VerifyAccount(VerifyAccountRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Handle case where email doesn't exist
                return new ApiErrorResult<string>("Email not found");
            }
            if (request.VerifyCode == user.VerifyCode)
            {
                user.IsVerify = true;
                await _context.SaveChangesAsync();
                return new ApiSuccessMessage<string>("Verify account successful");
            }
            return new ApiErrorResult<string>("Verify code not correct");
        }

        public async Task<List<UserVm>> GetAll(string keyword)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.UserName.Contains(keyword)
                                        || x.PhoneNumber.Contains(keyword));
            }

            var users = await query.Select(x => new UserVm()
            {
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                UserName = x.UserName,
                FirstName = x.FirstName,
                Id = x.Id,
                Dob = x.Dob,
                LastName = x.LastName,
                IsBanned = x.IsBanned,
                VIP = x.VIP,
                AccumulatedPoints = x.AccumulatedPoints,
                Avatar = x.Avatar,
                CreatedDate = x.CreatedDate,
                AcceptedTermOfUse = x.AcceptedTermOfUse,
            }).Distinct().ToListAsync();

            // Sort the users by CreatedDate after projection
            users = users.OrderByDescending(x => x.CreatedDate).ToList();

            return users;
        }

        public async Task<int> UpdateUserAvatar(UpdateUserAvatarRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (!string.IsNullOrWhiteSpace(request.AvatarImage) && IsBase64String(request.AvatarImage))
            {
                user.Avatar = await _storageService.SaveImageAsync(request.AvatarImage);
            }
            else
            {
                request.AvatarImage = user.Avatar;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAcceptedTermOfUse(AcceptedTermOfUseRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new Exception("User not exist!");
            }
            user.AcceptedTermOfUse = request.IsAccepted;
            return await _context.SaveChangesAsync();
        }
    }
}
