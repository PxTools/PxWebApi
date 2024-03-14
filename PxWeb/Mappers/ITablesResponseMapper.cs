using Px.Search;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface ITablesResponseMapper
    {
        TablesResponse Map(SearchResultContainer searchResultContainer, string lang, string? query);
    }
}
