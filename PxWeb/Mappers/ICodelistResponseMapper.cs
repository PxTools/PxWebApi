using Px.Abstractions;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface ICodelistResponseMapper
    {
        CodelistResponse Map(Codelist codelist, string language);
    }
}
