﻿using Component.ViewModels.Common;
using Component.ViewModels.System.Users;

namespace Component.Application.System.Users
{
    public interface IUserService
    {
        Task<LoginRespone<string>> Authencate(LoginRequest request);
        Task<ApiResult<bool>> Register(RegisterRequest request);
        Task<ApiResult<string>> Update(Guid id, UserUpdateRequest request);
        Task<ApiResult<bool>> Create(UserCreateRequest request);
        Task<List<UserVm>> GetAll(string keyword);
        Task<int> UpdateUserAvatar(UpdateUserAvatarRequest request);
        Task<int> UpdateAcceptedTermOfUse(AcceptedTermOfUseRequest request);
        Task<ApiResult<UserVm>> GetById(Guid id);
        Task<ApiResult<bool>> Delete(Guid id);
        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
        Task<ApiResult<bool>> BanAccount(Guid id, bool status);
        Task<ApiResult<string>> ForgotPassword(string email);
        Task<ApiResult<string>> ResetPassword(ResetPasswordRequest request);
        Task<ApiResult<string>> UpdatePassword(Guid id, UpdatePasswordRequest request);
        Task<ApiResult<string>> GetVerifyCode(string email);
        Task<ApiResult<string>> VerifyAccount(VerifyAccountRequest request);
        Task<LoginRespone<string>> RefreshToken(string token);
        Task<int> TotalUser(DateTime? startDate, DateTime? endDate);
        //Task<ApiResult<PagedResult<UserVm>>> GetUsersPaging(GetUserPagingRequest request);
    }
}
