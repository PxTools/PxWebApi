using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Px.Abstractions;
using Px.Abstractions.Interfaces;

using PxWeb.Code;
using PxWeb.Helper.Api2;
using PxWeb.Mappers;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class CodelistsApiController : PxWeb.Api2.Server.Controllers.CodelistsApiController
    {
        private readonly IDataSource _dataSource;
        private readonly ILanguageHelper _languageHelper;
        private readonly ICodelistResponseMapper _codelistResponseMapper;
        private readonly ILogger<CodelistsApiController> _logger;

        public CodelistsApiController(ILogger<CodelistsApiController> logger, IDataSource dataSource, ILanguageHelper languageHelper, ICodelistResponseMapper codelistResponseMapper)
        {
            _logger = logger;
            _dataSource = dataSource;
            _languageHelper = languageHelper;
            _codelistResponseMapper = codelistResponseMapper;
        }

        public override IActionResult GetCodelistById([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang)
        {
            lang = _languageHelper.HandleLanguage(lang);
            Codelist? codelist = _dataSource.GetCodelist(id, lang);

            if (codelist != null)
            {
                return Ok(_codelistResponseMapper.Map(codelist, lang));
            }
            else
            {
                _logger.LogNoCodelistWithGivenId();
                return NotFound(ProblemUtility.NonExistentCodelist());
            }
        }
    }
}
