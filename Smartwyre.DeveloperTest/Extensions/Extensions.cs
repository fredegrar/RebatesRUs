using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Extensions
{
    public static class Extensions
    {
        #region CalculateRebateResult extensions
        public static string ToDisplayString(this CalculateRebateResult r)
        {
            string result = "bad result";
            if (r != null)
            {
                result = r.Success ? "rebate processed!" : "rebate is not valid for the given input";
            }

            return result;
        }
        #endregion
    }
}
