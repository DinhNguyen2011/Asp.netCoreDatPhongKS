using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Asp.netCoreDatPhongKS.Filters
{
   
    public class RestrictToAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTroId = context.HttpContext.Session.GetString("VaiTroId");
            if (string.IsNullOrEmpty(vaiTroId) || (vaiTroId != "1" && vaiTroId != "2"))
            {
                context.Result = new RedirectToActionResult("Error", "Home", null);
            }
            base.OnActionExecuting(context);
        }
    }
}