using Component.Data.Entities;
using Component.Data.Enums.UserDetailEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class UserDetailVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Giới tính")]
        public GenderEnum Gender { get; set; }

        [Display(Name = "Tuổi")]
        public AgeRangeEnum AgeRange { get; set; }

        [Display(Name = "Mục tiêu")]
        public GoalEnum Goal { get; set; }

        [Display(Name = "Thể trạng")]
        public BodyTypeEnum BodyType { get; set; }

        [Display(Name = "Thể trạng mong muốn")]
        public BodyGoalEnum BodyGoal { get; set; }

        [Display(Name = "Vùng cơ thể tập trung tập luyện")]
        public TagetZoneEnum TagetZone { get; set; }

        [Display(Name = "Thời gian tập luyên")]
        public FrequencyEnum TimeSpend { get; set; }

        [Display(Name = "Cân nặng hoàn hảo gần đây nhất")]
        public TimeEnum LastPerfectWeight { get; set; }

        [Display(Name = "Cường độ tập thể dục")]
        public StateEnum DoWorkout { get; set; }

        [Display(Name = "Có dễ mệt mỏi không")]
        public StateEnum FeelTired { get; set; }

        [Display(Name = "Chiều cao hiện tại")]
        [Range(0.1, double.MaxValue, ErrorMessage = ("Height must be greater than 0"))]
        public int Height { get; set; }

        [Display(Name = "Cân nặng hiện tại")]
        [Range(0.1, double.MaxValue, ErrorMessage = ("Weight must be greater than 0"))]
        public float CurrentWeight { get; set; }

        [Display(Name = "Cân nặng mong muốn")]
        [Range(0.1, double.MaxValue, ErrorMessage = ("Weight must be greater than 0"))]
        public float GoalWeight { get; set; }

        [Display(Name = "Thời gian ngủ trung bình")]
        public FrequencyEnum TimeSleep { get; set; }

        [Display(Name = "Lượng nước uống mỗi ngày")]
        public FrequencyEnum WaterDrink { get; set; }

        [Display(Name = "Chế độ ăn mong muốn")]
        public DietEnum Diet { get; set; }

        [Display(Name = "Sản phẩm bị dị ứng")]
        public string? ProductAllergies { get; set; }
    }
}
