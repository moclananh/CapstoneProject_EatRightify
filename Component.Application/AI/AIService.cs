using Component.Application.System.Users;
using Component.Application.Utilities.Mail;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Data.Enums;
using Component.Utilities.Exceptions;
using Component.ViewModels.AI;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.System.Users;
using Component.ViewModels.Utilities.Promotions;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Prng;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Component.Application.AI
{
    public class AIService : IAIService
    {
        private readonly ApplicationDbContext _context;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;
        public AIService(ApplicationDbContext context, IEmailService emailService, UserManager<AppUser> userManager, IUserService userService)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<UpdateResultRequest> Update(int id, UpdateResultRequest request)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) throw new EShopException($"Cannot find a result with id: {id}");
            result.Title = request.Title;
            result.Description = request.Description;
            result.Status = request.Status;
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<UpdateStatusResult> UpdateStatus(int id, UpdateStatusResult status)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) throw new EShopException($"Cannot find a result with id: {id}");
            result.Status = status.Status;
            await _context.SaveChangesAsync();
            return status;
        }

        public async Task<List<ResultVM>> GetAll(string keyword, ResultStatus? status)
        {
            var query = from r in _context.Results
                        join u in _context.AppUsers on r.UserId equals u.Id
                        select new { r, u };

            //filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.r.Title.Contains(keyword));

            if (status != null)
            {
                query = query.Where(x => x.r.Status == status);
            }

            var result = await query.Select(x => new ResultVM()
            {
                Id = x.r.ResultId,
                Title = x.r.Title,
                Email = x.u.Email,
                Description = x.r.Description,
                ResultDate = x.r.ResultDate,
                Status = x.r.Status,
                IsSend = x.r.IsSended
            }).Distinct().ToListAsync();
            var sortResult = result.OrderByDescending(x => x.ResultDate).ToList();
            return sortResult;
        }

        public async Task<PagedResult<ResultVM>> GetAllPaging(ResultPagingRequest request)
        {
            var query = from r in _context.Results
                        join u in _context.AppUsers on r.UserId equals u.Id
                        select new ResultVM
                        {
                            Id = r.ResultId,
                            Email = u.Email,
                            Title = r.Title,
                            Description = r.Description,
                            Status = r.Status,
                            ResultDate = r.ResultDate,
                            IsSend = r.IsSended,
                        };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Email.Contains(request.Keyword));
            }
            List<ResultVM> result = query.ToList();
            int totalRow = result.Count();
            var data = result
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            var pagedResult = new PagedResult<ResultVM>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };

            return pagedResult;
        }

        public async Task<ResultVM> GetById(int id)
        {
            var query = from r in _context.Results
                        join u in _context.AppUsers on r.UserId equals u.Id
                        where r.ResultId == id
                        select new ResultVM
                        {
                            Id = r.ResultId,
                            Email = u.Email,
                            Title = r.Title,
                            Description = r.Description,
                            Status = r.Status,
                            ResultDate = r.ResultDate,
                            IsSend = r.IsSended,
                        };

            // Execute the query and retrieve the result
            var result = await query.FirstOrDefaultAsync();
            // Return the result
            return result;
        }


        public async Task<int> Delete(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) throw new EShopException($"Cannot find a product: {id}");

            _context.Results.Remove(result);
            return await _context.SaveChangesAsync();
        }

        public async Task<ResultVM> GetByUserId(Guid userId)
        {
            var query = from r in _context.Results
                        join u in _context.AppUsers on r.UserId equals u.Id
                        where r.UserId == userId
                        orderby r.ResultDate descending
                        select new ResultVM
                        {
                            Id = r.ResultId,
                            Email = u.Email,
                            Title = r.Title,
                            Description = r.Description,
                            Status = r.Status,
                            ResultDate = r.ResultDate,
                            IsSend = r.IsSended,
                        };

            // Execute the query and retrieve the result
            var result = await query.FirstOrDefaultAsync();
            // Return the result
            return result;
        }

        public async Task<Result> Create(CreateResultRequest request)
        {
            string userDetail = await GetResultByUserIdAsync(request.UserId);
            string gptResult = await ChatGPTService.GetGPTResult(userDetail);
            var user = await _userService.GetById(request.UserId);
            var result = new Result()
            {
                UserId = request.UserId,
                Title = "Result for: " + user.ResultObj.UserName,
                ResultDate = DateTime.UtcNow,
                Description = gptResult,
                Status = Data.Enums.ResultStatus.InProgress,
                IsSended = false

            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }


        private const string ResultApiBaseUrl = "https://erscus.azurewebsites.net/api/UserDetail";
        private const string ProductApiBaseUrl = "https://erscus.azurewebsites.net/api/Products/getProductForAI";
        private const string LinkProduct = "https://erscus.azurewebsites.net/api/Products/";
        public static async Task<string> GetResultByUserIdAsync(Guid userId)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ResultApiBaseUrl}/{userId}";
                string productApi = $"{ProductApiBaseUrl}";

                var response = await client.GetAsync(apiUrl);
                var productResponse = await client.GetAsync(productApi);

                if (response.IsSuccessStatusCode)
                {
                    string resultContent = await response.Content.ReadAsStringAsync();
                    string ProductResultContent = await productResponse.Content.ReadAsStringAsync();

                    var resultData = JsonConvert.DeserializeObject<UserDetailVm>(resultContent);
                    // var ProductResultData = JsonConvert.DeserializeObject<ProductVm>(ProductResultContent);

                    string result = "Gender: " + resultData.Gender.ToString() +
                        ", Age Range is " + resultData.AgeRange.ToString() +
                        ", Goal is " + resultData.Goal.ToString() +
                        ", Body type is " + resultData.BodyType.ToString() +
                        ", Body goal is " + resultData.BodyGoal.ToString() +
                        ", Taget zone is " + resultData.TagetZone.ToString() +
                        ", Time spend working is" + resultData.TimeSpend.ToString() +
                        ", The last perfect weight is " + resultData.LastPerfectWeight.ToString() +
                        ", The user is " + resultData.DoWorkout.ToString() + "workout" +
                        ", The user is " + resultData.FeelTired.ToString() + "feel tired" +
                        ", Height of user is " + resultData.Height.ToString() + "Centimeter " +
                        ", Current weight is " + resultData.CurrentWeight.ToString() + "Kilogram " +
                        ", Taget weight is " + resultData.GoalWeight.ToString() + "Kilogram " +
                        ", The user's sleep frequence is " + resultData.TimeSleep.ToString() +
                        ", Drink water frequence is " + resultData.WaterDrink.ToString() +
                        ", The target diet that user want is " + resultData.Diet.ToString() +
                        ", List of allergies keyword that user avoid is " + resultData.ProductAllergies +
                        ". Read and analys all of the information above. " +
                        " If the information is invalid or there is no sultable product for user, then don't provide any product and explain the reason" +
                        " If the information is valid, then" +
                        " this the type of person that want some product suitable for them" +
                        " .Choose a list contain multiple of suitable product for this person," +
                        " remember to exclude all the product that have allergies keyword in it's description, " +
                        " then choose the product description, product stock and the product name must be in this list: " + ProductResultContent +
                        " . Then for every product you recommend, also provide a link to that product that have a type like this: " + LinkProduct + "{#productid#}" +
                        " and give me the reason why you choose those products for this person. " +
                        ". Finally, generate a list of work out exercise that suitable for this person and then give me a best advice for this person base on their stats.";
                    return result;
                }
                else
                {
                    // Xử lý lỗi nếu cần
                    return null;
                }
            }
        }

        public async Task<ApiResult<string>> GetResultEmail(string email, int id)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiErrorResult<string>("Email not found");
            }
            var result = await GetById(id);
            if (result == null)
            {
                return new ApiErrorResult<string>("Result not found");
            }
            if (email != result.Email)
            {
                return new ApiErrorResult<string>("Result information not match");
            }
            var resultEntity = await _context.Results.FindAsync(result.Id);
            var subject = "ERS health care result";
            var descriptionFormat = ExtractDescriptionFromCKEditorFormat(result.Description);
            var body = $@"
            <p style='color: black;'>This is your health care result in ERS system.</p>
            <table border='1' style='border-collapse: collapse;'>          
            <tr>
            <td style='color: black;'><strong>Title</strong></td>
            <td style='color: black;'>Result for user {user.UserName}</td>
            </tr>
            <tr>
            <td style='color: black;'><strong>Email</strong></td>
            <td style='color: black;'>{user.Email}</td>
            </tr>
            <tr>
            <td style='color: black;'><strong>Description</strong></td>
            <td style='color: black;'>{descriptionFormat}</td>
            </tr>
            <tr>
            <td style='color: black;'><strong>Result Date</strong></td>
            <td style='color: black;'>{result.ResultDate}</td>
            </tr>            
            </table>";

            
            try
            {
                await _emailService.SendPasswordResetEmailAsync(email, subject, body);
                resultEntity.IsSended = true;
                await _context.SaveChangesAsync();
                return new ApiSuccessMessage<string>(" Result email sent");
            }
            catch
            {
                // Handle the exception as needed
                return new ApiErrorResult<string>("Error sending result email");
            }
        }

        // Phương thức để trích xuất nội dung từ định dạng CKEditor
        private string ExtractDescriptionFromCKEditorFormat(string ckEditorContent)
        {
            // Xử lý các thẻ HTML cần thiết để trích xuất nội dung
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(ckEditorContent);

            // Trích xuất nội dung từ các thẻ HTML trong định dạng CKEditor
            var extractedContent = new StringBuilder();
            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                if (node.Name == "h2" || node.Name == "p" || node.Name == "ol" || node.Name == "ul")
                {
                    extractedContent.Append(node.OuterHtml);
                }
                else if (node.Name == "strong" || node.Name == "i" || node.Name == "br")
                {
                    extractedContent.Append(node.OuterHtml);
                }
                else if (node.Name == "#text")
                {
                    extractedContent.Append(node.InnerText);
                }
            }

            return extractedContent.ToString();
        }

        public async Task<int> UpdateIsSend(int id, UpdateIsSendRequest request)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) throw new EShopException($"Cannot find a result with id: {id}");
            result.IsSended = request.IsSended;
            return await _context.SaveChangesAsync();
        }
    }
}
