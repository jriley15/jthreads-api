using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.Application.Services;
using JThreads.Data.Dto.Namespace;
using JThreads.Data.Dto.Threads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JThreads.API.Extensions;

namespace JThreads.API.Controllers
{
    [Authorize]
    public class NamespaceController : BaseController
    {
        private readonly NamespaceService _namespaceService;

        public NamespaceController(NamespaceService namespaceService)
        {
            _namespaceService = namespaceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNamespaceDto createNamespaceDto)
        {
            return this.GenerateResponse(await _namespaceService.Create(createNamespaceDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return this.GenerateResponse(await _namespaceService.GetAll());
        }

    }
}
