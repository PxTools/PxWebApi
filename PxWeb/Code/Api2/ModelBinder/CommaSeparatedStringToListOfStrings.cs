namespace PxWeb.Code.Api2.ModelBinder
{
    public class CommaSeparatedStringToListOfStrings : GenericListModelBinder<string>
    {
        public CommaSeparatedStringToListOfStrings() : base(x => x)
        {
        }
    }
}
