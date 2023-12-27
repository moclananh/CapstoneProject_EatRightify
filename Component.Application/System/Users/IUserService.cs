using Component.ViewModels.Common;
using Component.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.System.Users
{
    public interface IUserService
    {
        Task<LoginRespone<string>> Authencate(LoginRequest request, bool verifyRole = true);

        Task<ApiResult<bool>> Register(RegisterRequest request);

        Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request);

        Task<ApiResult<PagedResult<UserVm>>> GetUsersPaging(GetUserPagingRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<bool>> Delete(Guid id);

        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
        Task<ApiResult<bool>> BanAccount(Guid id, bool status);
        Task<ApiResult<string>> ForgotPassword(string email);
        Task<ApiResult<string>> ResetPassword(string email, string token, string newPassword, string confirmPassword);
        Task<ApiResult<string>> UpdatePassword(Guid id, string oldPassword, string newPassword);
    }
}
