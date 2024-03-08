using Component.Data.EF;
using Component.Data.Entities;
using Component.Data.Enums;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Orders;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Comments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Comments
{
    public class CommentService : ICommentService
    {

        private readonly ApplicationDbContext _context;
        public CommentService(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<Comment> Create(CommentCreateRequest request)
        {
            var comments = new Comment()
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                Content = request.Content,
                CreatedAt = DateTime.Now,
                Status = Data.Enums.Status.Active,
                Grade= request.Grade,
            };

            _context.Comments.Add(comments);
            await _context.SaveChangesAsync();
            return comments;
        }

        public async Task<int> Delete(int commentId)
        {
            var comments = await _context.Comments.FindAsync(commentId);
            if (comments == null) throw new EShopException($"Cannot find a comments: {commentId}");

            _context.Comments.Remove(comments);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<CommentVm>> GetAll(int productId)
        {
            var query = from c in _context.Comments
                        join u in _context.AppUsers on c.UserId equals u.Id
                        where c.ProductId == productId
                        select new {c, u };
            query = query.OrderByDescending(x => x.c.CreatedAt);
            return await query.Select(x => new CommentVm()
            {
                Id = x.c.Id,
                UserId= x.c.UserId,
                UserName = x.u.UserName,
                ProductId = x.c.ProductId,
                Content= x.c.Content,
                CreatedAt = x.c.CreatedAt,
                Status = x.c.Status,
                Grade= x.c.Grade,
                UserAvatar = x.u.Avatar,
                ModifieddAt = x.c.ModifiedAt,
            }).ToListAsync();
        }

        public async Task<PagedResult<CommentVm>> GetAllCommentByProductIdPaging(GetCommentPagingRequest request)
        {
            var query = from c in _context.Comments
                        join u in _context.AppUsers on c.UserId equals u.Id
                        where c.ProductId == request.ProductId
                        select new CommentVm()
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            UserName = u.UserName,
                            ProductId = c.ProductId,
                            Content = c.Content,
                            CreatedAt = c.CreatedAt,
                            Status = c.Status,
                            Grade= c.Grade,
                            UserAvatar = u.Avatar
                        };
            int totalRow = query.Count();

            var data = query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<CommentVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }


        public async Task<CommentVm> GetById(int id)
        {

            var query = from c in _context.Comments
                        join u in _context.AppUsers on c.UserId equals u.Id
                        where c.Id == id
                        select new { c, u };
            return await query.Select(x => new CommentVm()
            {
                Id = x.c.Id,
                UserId = x.c.UserId,
                UserName = x.u.UserName,
                ProductId = x.c.ProductId,
                Content = x.c.Content,
                CreatedAt = x.c.CreatedAt,
                Status = x.c.Status,
                Grade = x.c.Grade,
                UserAvatar = x.u.Avatar,
                ModifieddAt = x.c.ModifiedAt,
            }).FirstOrDefaultAsync();
        }

        public async Task<int> Update(CommentUpdateRequest request)
        {
            var comments = await _context.Comments.FindAsync(request.Id);

            if (comments == null) throw new EShopException($"Cannot find a comments with id: {request.Id}");
            comments.Content = request.Content;
            comments.Grade = request.Grade;
            comments.ModifiedAt = DateTime.Now;
            return await _context.SaveChangesAsync();
        }

     
    }
}
