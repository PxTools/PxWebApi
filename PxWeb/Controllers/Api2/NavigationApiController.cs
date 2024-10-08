/*
 * PxApi
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 2.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using PCAxis.Menu;

using Px.Abstractions.Interfaces;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;
using PxWeb.Mappers;

namespace PxWeb.Controllers.Api2
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class NavigationApiController : PxWeb.Api2.Server.Controllers.NavigationApiController
    {
        private readonly IDataSource _dataSource;
        private readonly ILanguageHelper _languageHelper;
        private readonly IFolderResponseMapper _folderResponseMapper;

        public NavigationApiController(IDataSource dataSource, ILanguageHelper languageHelper, IFolderResponseMapper folderMapper)
        {
            _dataSource = dataSource;
            _languageHelper = languageHelper;
            _folderResponseMapper = folderMapper;
        }

        /// <summary>
        /// Gets navigation item with the given id.
        /// HttpGet
        /// Route /navigation/{id}
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="lang">The language if the default is not what you want.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        /// <response code="404">Error respsone for 404</response>
        /// <response code="429">Error respsone for 429</response>
        public override IActionResult GetNavigationById([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang)
        {
            return GetFolder(id, lang, false);
        }

        /// <summary>
        /// Browse the database structure
        /// HttpGet
        /// Route /navigation
        /// </summary>
        /// <param name="lang">The language if the default is not what you want.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        /// <response code="404">Error respsone for 404</response>
        /// <response code="429">Error respsone for 429</response>

        public override IActionResult GetNavigationRoot([FromQuery(Name = "lang")] string? lang)
        {
            return GetFolder("", lang, true);
        }

        private IActionResult GetFolder(string id, string? lang, bool isRoot)
        {
            bool selectionExists = true;

            lang = _languageHelper.HandleLanguage(lang);

            Item? item = _dataSource.CreateMenu(id, lang, out selectionExists);

            if (item == null)
            {
                return new BadRequestObjectResult(ErrorReadingNodeData());
            }

            FolderResponse folder = _folderResponseMapper.GetFolder((PxMenuItem)item, lang, isRoot);

            return new ObjectResult(folder);
        }

        private Problem NonExistentNode()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 404;
            p.Title = "Non-existent node";
            return p;
        }

        private Problem ErrorReadingNodeData()
        {
            Problem p = new Problem();
            p.Type = "Data error";
            p.Status = 400;
            p.Title = "Error reading node";
            return p;
        }
    }
}
