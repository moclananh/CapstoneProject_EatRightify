using Component.Data.Entities;
using Component.ViewModels.System.Users;
using Component.ViewModels.Utilities.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.System.Users.UserDetail
{
    public interface IUserDetailService
    {
        Task<UserDetailVm> GetById(Guid id);
        Task<AppUserDetails> Create(UserDetailVm request);
        Task<int> Update(UserDetailVm request);

    }
}
