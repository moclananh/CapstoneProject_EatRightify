using Component.Data.Enums.UserDetailEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class AppUserDetails
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public GenderEnum Gender { get; set; }
        public AgeRangeEnum AgeRange { get; set; }
        public GoalEnum Goal { get; set; }
        public BodyTypeEnum BodyType { get; set; }
        public BodyGoalEnum BodyGoal { get; set; }
        public TagetZoneEnum TagetZone { get; set; }
        public FrequencyEnum TimeSpend { get; set; }
        public TimeEnum LastPerfectWeight { get; set; }
        public StateEnum DoWorkout { get; set; }
        public StateEnum FeelTired { get; set; }
        public int Height { get; set; }
        public float CurrentWeight { get; set; }
        public float GoalWeight { get; set; }
        public FrequencyEnum TimeSleep { get; set; }
        public FrequencyEnum WaterDrink { get; set; }
        public DietEnum Diet { get; set; }
        public string? ProductAllergies { get; set; }

    }
}
