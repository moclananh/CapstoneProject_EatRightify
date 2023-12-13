using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.AI;
using Component.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if(result == null) throw new EShopException($"Cannot find a result with id: {id}");
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
            string gptResult = await ChatGPTService.GetGPTResult(request.Description);
            var result = new Result()
            {
                UserId = request.UserId,
                Title = "Result for " + DateTime.UtcNow,
                ResultDate = DateTime.UtcNow,
                Description = gptResult,
                Status = Data.Enums.ResultStatus.InProgress,
                IsSended = false

            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }
        
    }
}
