using PxWeb.Api2.Server.Models;

namespace PxWeb.Helper.Api2
{
    public static class ProblemUtility
    {
        public static Problem NonExistentVariable()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Non-existent variable";
            return p;
        }

        public static Problem NonExistentCodelist()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Non-existent codelist";
            return p;
        }

        public static Problem NonExistentValue()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Non-existent value";
            return p;
        }

        public static Problem MissingSelection()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Missing selection for mandantory variable";
            return p;
        }
        public static Problem IllegalSelection()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Illegal selection for mandantory variable";
            return p;
        }

        public static Problem IllegalSelectionExpression()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Illegal selection expression";
            return p;
        }

        public static Problem IllegalPlacementSelection()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 400;
            p.Title = "Illegal placment";
            return p;
        }

        public static Problem TooManyCellsSelected()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Detail = "Too many cells selected";
            p.Status = 400;
            p.Title = "Too many cells selected";
            return p;
        }

        public static Problem NonExistentTable()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Status = 404;
            p.Title = "Non-existent table";
            return p;
        }


        public static Problem OutOfRange()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Detail = "Non-existent page";
            p.Status = 404;
            p.Title = "Non-existent page";
            return p;
        }

        public static Problem UnsupportedOutputFormat()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Detail = "Unsupported output format";
            p.Status = 400;
            p.Title = "Unsupported output format";
            return p;
        }

        public static Problem NonExistentSavedQuery()
        {
            Problem p = new Problem();
            p.Type = "Parameter error";
            p.Detail = "Saved query not found";
            p.Status = 404;
            p.Title = "Saved query not found";
            return p;
        }

        public static Problem InternalErrorCreateSavedQuery()
        {
            Problem p = new Problem();
            p.Type = "Create saved query";
            p.Detail = "Internal error! Could not create saved query";
            p.Status = 400;
            p.Title = "Internal error! Could not create saved query";
            return p;
        }

        public static Problem NoQuerySpecified()
        {
            Problem p = new Problem();
            p.Type = "Create saved query";
            p.Detail = "No query specified";
            p.Status = 400;
            p.Title = "No query specified";
            return p;
        }
    }
}
