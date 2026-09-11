using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2;

namespace PxWebApi_Mvc.Tests.Wrappers
{
    internal sealed class CancellationAwareDataWorkflow : IDataWorkflow
    {
        public PXModel? Run(string tableId, string language, VariablesSelection? variablesSelection, out Problem? problem, CancellationToken token)
        {
            problem = null;
            WaitForCancellation(token);
            return null;
        }

        public PXModel? Run(string tableId, string language, out Problem? problem, CancellationToken token)
        {
            problem = null;
            WaitForCancellation(token);
            return null;
        }

        private static void WaitForCancellation(CancellationToken token)
        {
            if (!token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
            {
                throw new OperationCanceledException(token);
            }

            token.ThrowIfCancellationRequested();
        }
    }
}
