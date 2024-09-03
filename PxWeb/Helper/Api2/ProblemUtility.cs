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
    }
}
