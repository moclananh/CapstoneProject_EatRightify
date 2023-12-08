using Component.Data.EF;
using Component.Utilities.Exceptions;
using Component.ViewModels.AI;
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
    }
}
