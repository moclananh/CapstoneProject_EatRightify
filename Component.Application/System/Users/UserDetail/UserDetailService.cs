using Component.Data.EF;
using Component.Data.Entities;
using Component.Data.Enums.UserDetailEnums;
using Component.Utilities.Exceptions;
using Component.ViewModels.System.Users;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.System.Users.UserDetail
{
    public class UserDetailService : IUserDetailService
    {
        private readonly ApplicationDbContext _context;
        public UserDetailService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<AppUserDetails> Create(UserDetailVm request)
        {
            var user = new AppUserDetails()
            {
                UserId = request.Id,
                Gender = request.Gender,
                AgeRange = request.AgeRange,
                Goal = request.Goal,
                BodyType = request.BodyType,
                BodyGoal = request.BodyGoal,
                TagetZone = request.TagetZone,
                TimeSpend = request.TimeSpend,
                LastPerfectWeight = request.LastPerfectWeight,
                DoWorkout = request.DoWorkout,
                FeelTired = request.FeelTired,
                Height = request.Height,
                CurrentWeight = request.CurrentWeight,
                GoalWeight = request.GoalWeight,
                TimeSleep = request.TimeSleep,
                WaterDrink = request.WaterDrink,
                Diet = request.Diet,
                ProductAllergies = request.ProductAllergies,
            };

            _context.AppUserDetails.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<int> Update(UserDetailVm request)
        {
            var userId = await _context.AppUserDetails.FindAsync(request.Id);

            if (userId == null) throw new EShopException($"Cannot find a userId with id: {request.Id}");
            userId.Gender = request.Gender;
            userId.AgeRange = request.AgeRange;
            userId.Goal = request.Goal;
            userId.BodyType = request.BodyType;
            userId.BodyGoal = request.BodyGoal;
            userId.TagetZone = request.TagetZone;
            userId.TimeSpend = request.TimeSpend;
            userId.LastPerfectWeight = request.LastPerfectWeight;
            userId.DoWorkout = request.DoWorkout;
            userId.FeelTired = request.FeelTired;
            userId.Height = request.Height;
            userId.CurrentWeight = request.CurrentWeight;
            userId.GoalWeight = request.GoalWeight;
            userId.TimeSleep = request.TimeSleep;
            userId.WaterDrink = request.WaterDrink;
            userId.Diet = request.Diet;
            userId.ProductAllergies = request.ProductAllergies;

            return await _context.SaveChangesAsync();
        }

        public async Task<UserDetailVm> GetById(Guid id)
        {
            var query = from ud in _context.AppUserDetails
                        where ud.UserId == id
                        select new { ud };
            return await query.Select(x => new UserDetailVm()
            {
               Id = x.ud.UserId,
               Gender= x.ud.Gender,
               AgeRange= x.ud.AgeRange,
               Goal = x.ud.Goal,
               BodyType= x.ud.BodyType,
               BodyGoal= x.ud.BodyGoal,
               TagetZone= x.ud.TagetZone,
               TimeSpend = x.ud.TimeSpend,
               LastPerfectWeight= x.ud.LastPerfectWeight,
               DoWorkout = x.ud.DoWorkout,
               FeelTired= x.ud.FeelTired,
               Height= x.ud.Height,
               CurrentWeight= x.ud.CurrentWeight,
               GoalWeight= x.ud.GoalWeight,
               TimeSleep  = x.ud.TimeSleep,
               WaterDrink= x.ud.WaterDrink,
               Diet = x.ud.Diet,    
               ProductAllergies= x.ud.ProductAllergies,
            }).FirstOrDefaultAsync();
        }

        
    }
}
