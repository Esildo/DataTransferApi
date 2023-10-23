using Azure.Core;
using DataTransferApi.Db;
using DataTransferApi.Entities;
using DataTransferApi.Heppers;
using DataTransferApi.HeppersService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;

namespace DataTransferApi.Services
{
    public class OneTokenStorageService : IOneTokenService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICommandDBService _efHelper;
        private readonly string _token;

        public OneTokenStorageService(AppDbContext appDbContext, ICommandDBService efHelper)
        { 
            _efHelper = efHelper;
            _appDbContext = appDbContext;
            _token = Guid.NewGuid().ToString();
        }

        //Create One time token for one file
        public async Task<string> CreateOneFileTokenAsync(string groupName, string userName,string userId)
        {
            var tokenLink = new TokenLink
            {
                Token = _token,
                Path = await _efHelper.GetFilePathByNameAsync(groupName,userName,userId),
                ExpireDate = DateTime.Now.AddMinutes(1)
            };
            _appDbContext.TokenLinks.Add(tokenLink);
            _appDbContext.SaveChanges();

            return _token;
        }

        //Create one time token for group of files
        public async Task<string> CreateOneGroupTokenAsync(string groupName, string userId)
        {
            IEnumerable<string> fileGroupPaths = await _efHelper.GetFileNameGroupAsync(groupName, userId);
            foreach (string path in fileGroupPaths)
            {
                var tokenLink = new TokenLink
                {
                    Token = _token,
                    Path = await _efHelper.GetFilePathByNameAsync(groupName, path, userId),
                    ExpireDate = DateTime.Now.AddMinutes(1)
                };
                _appDbContext.TokenLinks.Add(tokenLink);
                _appDbContext.SaveChanges();
            }
            return _token;

        }
        //Download single file using a one-time link
        public async Task<(byte[],string,string)> DownloadFileToken(string token)
        {

            string path = await _appDbContext.TokenLinks.Where(t => t.Token == token).Select(p => p.Path).SingleOrDefaultAsync();
            DeleteToken(token);
            return await _efHelper.ReturnFileTupleAsync(path);
        }

        //Download group of files using a one-time link
        public async Task<IEnumerable<(byte[],string,string)>> DownLoadFileGroupAsync(string token)
        {
            IEnumerable<string> filesPath = await _appDbContext.TokenLinks.Where(t => t.Token == token)
                                                                            .Select(p => p.Path).ToArrayAsync();

            DeleteToken(token);

            List<(byte[], string, string)> groupTuple = new List<(byte[], string, string)>();
            foreach (var path in filesPath)
            {
                var fileTuple = await _efHelper.ReturnFileTupleAsync(path);
                groupTuple.Add((fileTuple.Item1,fileTuple.Item2,fileTuple.Item3));
            }
            return groupTuple;
        }


        //Check tokens
        public async Task<int> TokenCheck(string tokenCheck)
        {
            int CountToken;
            bool timeValid;

            CountToken = await GetTokenCountAsync(tokenCheck);
            if (CountToken == 0)
            {
                //handle
                throw new Exception("No token there");
            }
            timeValid =  CheckDataAsync(tokenCheck);
            if(!timeValid)
            {
                DeleteTimout();
                //handle
                throw new Exception("Time out Token");
            }
            return CountToken;
        }

        //Delete all tokens with this key
        private void DeleteToken(string token)
        {
            var linksToDelete = _appDbContext.TokenLinks.Where(t => t.Token == token);
            _appDbContext.TokenLinks.RemoveRange(linksToDelete);
            _appDbContext.SaveChanges();
        }

        //Count all tokenst with this key
        private async Task<int> GetTokenCountAsync(string token)
        {
            int fileCount = await _appDbContext.TokenLinks.CountAsync(t => t.Token == token);

            return fileCount;
        }


        //Check if token time out 
        public bool CheckDataAsync(string token)
        {
            var tokenTime = _appDbContext.TokenLinks.Where(t => t.Token == token).Select(p => p.ExpireDate).FirstOrDefault();
            if (DateTime.Now <= tokenTime)
            {
                return true;
            }
            else
            {
                return false;
            }
            return true;
        }

        //delete all time out tokens
        private void DeleteTimout()
        {
            var linksToDelete = _appDbContext.TokenLinks.Where(t => t.ExpireDate < DateTime.Now);
            _appDbContext.TokenLinks.RemoveRange(linksToDelete);
            _appDbContext.SaveChanges();
        }

    }
}
