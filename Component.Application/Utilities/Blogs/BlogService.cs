﻿using Component.Application.Common;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.Categories;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Blogs
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        public BlogService(ApplicationDbContext context)
        {
            _context = context;
          
        }

        public async Task<Blog> Create(BlogCreateRequest request)
        {
            var blogs = new Blog()
            {
                Title = request.Title,
                Description = request.Description,
                Url = request.Url,
                Image = request.Image,
                SortOrder= request.SortOrder,
                DateCreate= DateTime.Now,
                Status= Data.Enums.Status.Active

            };

            _context.Blogs.Add(blogs);
            await _context.SaveChangesAsync();
            return blogs;
        }

        public async Task<int> Delete(int blogId)
        {
            var blog = await _context.Blogs.FindAsync(blogId);
            if (blog == null) throw new EShopException($"Cannot find a blog: {blogId}");

            _context.Blogs.Remove(blog);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<BlogVm>> GetAll()
        {
            var query = from b in _context.Blogs
                        select new { b };
            return await query.Select(x => new BlogVm()
            {
                Id = x.b.Id,
                Title = x.b.Title, Description = x.b.Description,
                Url = x.b.Url,
                Image = x.b.Image,
                SortOrder= x.b.SortOrder,
                DateCreate= x.b.DateCreate,
                Status = x.b.Status
            }).ToListAsync();
        }

        public async Task<PagedResult<BlogVm>> GetAllPaging(GetBlogPagingRequest request)
        {
            //1. Select join
            var query = from b in _context.Blogs
                        select new BlogVm()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Description = b.Description,
                            Url = b.Url,
                            Image = b.Image,
                            SortOrder = b.SortOrder,
                            DateCreate = b.DateCreate,
                            Status = b.Status
                        };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.Title.Contains(request.Keyword));

          
            int totalRow = query.Count();

            var data = query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<BlogVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<BlogVm> GetById(int id)
        {
            var query = from b in _context.Blogs
                        where b.Id == id
                        select new { b };
            return await query.Select(x => new BlogVm()
            {
                Id = x.b.Id,
                Title = x.b.Title,
                Description = x.b.Description,
                Url = x.b.Url,
                Image = x.b.Image,
                SortOrder = x.b.SortOrder,
                DateCreate = x.b.DateCreate,
                Status = x.b.Status
            }).FirstOrDefaultAsync();
        }

        public async Task<int> Update(BlogUpdateRequest request)
        {
            var blog = await _context.Blogs.FindAsync(request.Id);
          
            if (blog == null) throw new EShopException($"Cannot find a blogs with id: {request.Id}");
            blog.Title = request.Title;
            blog.Description = request.Description;
            blog.Url = request.Url;
            blog.Image = request.Image;
            blog.SortOrder = request.SortOrder;
            blog.Status = request.Status;
            return await _context.SaveChangesAsync();
        }
    }
}