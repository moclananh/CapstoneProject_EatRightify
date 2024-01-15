using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.AI;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.System.Users;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public AIService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateResultRequest> Update(int id, UpdateResultRequest request)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) throw new EShopException($"Cannot find a result with id: {id}");
            result.Title = request.Title;
            result.Description = request.Description;
            result.IsSended = request.IsSend;
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

        public async Task<List<ResultVM>> GetAll(string keyword)
        {
            var query = from r in _context.Results
                        join u in _context.AppUsers on r.UserId equals u.Id
                        select new { r, u };

            //filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.r.Title.Contains(keyword));

            return await query.Select(x => new ResultVM()
            {
              Id = x.r.ResultId,
              Title = x.r.Title,
              Email = x.u.Email,
              Description = x.r.Description,
              ResultDate = x.r.ResultDate,
              Status= x.r.Status,
              IsSend = x.r.IsSended
            }).Distinct().ToListAsync();
        

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

        public async Task<ResultVM> GetById(Guid userId)
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
            string userDetail = await GetResultByUserIdAsync(request.UserId, request.LanguageId);
            string gptResult = await ChatGPTService.GetGPTResult(userDetail);
            var result = new Result()
            {
                UserId = request.UserId,
                Title = "Result for " + request.UserId + " On "+ DateTime.UtcNow,
                ResultDate = DateTime.UtcNow,
                Description = gptResult,
                Status = Data.Enums.ResultStatus.InProgress,
                IsSended = false

            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }


        private const string ResultApiBaseUrl = "https://eatright2.azurewebsites.net/api/UserDetail";
        private const string ProductApiBaseUrl = "https://eatright2.azurewebsites.net/api/Products/getAll?LanguageId";
        private const string LinkProduct = "https://eatright2.azurewebsites.net/api/Products/";

        public static async Task<string> GetResultByUserIdAsync(Guid userId, string languageId)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ResultApiBaseUrl}/{userId}";
                string productApi = $"{ProductApiBaseUrl}={languageId}";

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
                        " If the information is valid, then"+
                        " this the type of person that want some product suitable for them" +
                        " .Choose a list contain multiple of suitable product for this person," +
                        " remember to exclude all the product that have allergies keyword in it's description, " +
                        " then choose the product description, product stock and the product name must be in this list: " + ProductResultContent +
                        " . Then for every product you recommend, also provide a link to that product that have a type like this: " + LinkProduct + "{#productid#}"+ "/{#languageid#}" +
                        " and give me the reason why you choose those products for this person. " +
                        ". Finally, generate a list of work out exercise that suitable for this person and then give me a best advice for this person base on their stats." ;
                    return result;
                }
                else
                {
                    // Xử lý lỗi nếu cần
                    return null;
                }
            }
        }
    }
}
