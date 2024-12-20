﻿using Component.Application.Common;
using Component.Application.System.Users;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.Categories;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        public BlogService(ApplicationDbContext context, IUserService userService, IStorageService storageService, IConfiguration configuration)
        {
            _context = context;
            _storageService = storageService;
            _configuration = configuration;
        }

        public async Task<Blog> Create(BlogCreateRequest request)
        {
            var blogs = new Blog()
            {
                Title = request.Title,
                Description = request.Description,
                Url = request.Url,                
                SortOrder = request.SortOrder,
                DateCreate= DateTime.Now,
                Status= Data.Enums.Status.Active,
                CreatedBy = request.CreatedBy,

            };
            if (!string.IsNullOrWhiteSpace(request.Image) && IsBase64String(request.Image))
            {
                blogs.Image = await _storageService.SaveImageAsync(request.Image);
            }
            else
            {
                blogs.Image = _configuration["NoImage"];
            }

            _context.Blogs.Add(blogs);
            await _context.SaveChangesAsync();
            return blogs;
        }
        private bool IsBase64String(string s)
        {
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> Delete(int blogId)
        {
            var check = await GetById(blogId);
            var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == blogId);
            if (blog == null)
            {
                throw new EShopException($"Cannot find a location: {blogId}");
            }
            if (check.Id != blog.Id)
            {
                throw new EShopException($"Error to find location: {blogId}");
            }
            _context.Blogs.Remove(blog);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<BlogVm>> GetAll()
        {
            var query = from b in _context.Blogs
                        join u in _context.AppUsers on b.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        select new { b, u };
            query = query.OrderByDescending(x => x.b.DateCreate);
            return await query.Select(x => new BlogVm()
            {
                Id = x.b.Id,
                Title = x.b.Title, Description = x.b.Description,
                Url = x.b.Url,
                Image = x.b.Image,
                SortOrder= x.b.SortOrder,
                DateCreate= x.b.DateCreate,
                Status = x.b.Status,
                CreatedBy = x.u.UserName,
                ViewCount = x.b.ViewCount,
            }).ToListAsync();
        }

        public async Task<PagedResult<BlogVm>> GetAllPaging(GetBlogPagingRequest request)
        {
            //1. Select join
            var query = from b in _context.Blogs
                        join u in _context.AppUsers on b.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        select new BlogVm()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Description = b.Description,
                            Url = b.Url,
                            Image = b.Image,
                            SortOrder = b.SortOrder,
                            DateCreate = b.DateCreate,
                            Status = b.Status,
                            CreatedBy = u.UserName,
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
                        join u in _context.AppUsers on b.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        where b.Id == id
                        select new { b, u};
            return await query.Select(x => new BlogVm()
            {
                Id = x.b.Id,
                Title = x.b.Title,
                Description = x.b.Description,
                Url = x.b.Url,
                Image = x.b.Image,
                SortOrder = x.b.SortOrder,
                DateCreate = x.b.DateCreate,
                Status = x.b.Status,
                CreatedBy = x.u.UserName,
                UserAvatar = x.u.Avatar,
                ViewCount = x.b.ViewCount,
            }).FirstOrDefaultAsync();
        }

        public async Task<int> Update(BlogUpdateRequest request)
        {
            var blog = await _context.Blogs.FindAsync(request.Id);
          
            if (blog == null) throw new EShopException($"Cannot find a blogs with id: {request.Id}");
            blog.Title = request.Title;
            blog.Description = request.Description;
            blog.Url = request.Url;
            blog.SortOrder = request.SortOrder;
            blog.Status = request.Status;
            if (!string.IsNullOrWhiteSpace(request.Image) && IsBase64String(request.Image))
            {
                blog.Image = await _storageService.SaveImageAsync(request.Image);
            }
            else
            {
                request.Image = blog.Image;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task AddViewcount(int blogId)
        {
            var blog = await _context.Blogs.FindAsync(blogId);
            blog.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> TotalView()
        {
            var totalViewCount = await (from b in _context.Blogs
                                        join u in _context.AppUsers on b.CreatedBy equals u.Id into bu
                                        from u in bu.DefaultIfEmpty()
                                        select b.ViewCount).SumAsync();

            return totalViewCount;
        }

        public async Task<List<BlogVm>> GetAllBlogActive()
        {
            var query = from b in _context.Blogs
                        join u in _context.AppUsers on b.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        where b.Status == Data.Enums.Status.Active
                        select new { b, u };
            query = query.OrderByDescending(x => x.b.DateCreate);
            return await query.Select(x => new BlogVm()
            {
                Id = x.b.Id,
                Title = x.b.Title,
                Description = x.b.Description,
                Url = x.b.Url,
                Image = x.b.Image,
                SortOrder = x.b.SortOrder,
                DateCreate = x.b.DateCreate,
                Status = x.b.Status,
                CreatedBy = x.u.UserName,
                ViewCount = x.b.ViewCount,
            }).ToListAsync();
        }
    }
}
