using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Common
{
    public interface IStorageService
    {
        string GetFileUrl(string fileName);

        /*  Task SaveFileAsync(Stream mediaBinaryStream, string fileName);*/
        Task<string> SaveImageAsync(IFormFile image);

        Task DeleteFileAsync(string fileName);
    }
}
