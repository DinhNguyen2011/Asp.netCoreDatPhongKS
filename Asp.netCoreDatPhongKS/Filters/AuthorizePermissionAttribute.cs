using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Asp.netCoreDatPhongKS.Filters
{
    public class AuthorizePermissionAttribute : ActionFilterAttribute
    {
        private readonly string[] _requiredPermissions;

        public AuthorizePermissionAttribute(params string[] requiredPermissions)
        {
            _requiredPermissions = requiredPermissions;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var taiKhoanId = session.GetString("TaiKhoanId");
            var vaiTroId = session.GetString("VaiTroId");
            var quyenJson = session.GetString("Quyen");

            if (string.IsNullOrEmpty(taiKhoanId) || string.IsNullOrEmpty(quyenJson))
            {
                context.Result = new RedirectToActionResult("Login", "TaiKhoan", null);
                return;
            }

            // Quản lý (VaiTroId = 1) có tất cả quyền
            if (vaiTroId == "1")
            {
                base.OnActionExecuting(context);
                return;
            }

            var quyen = JsonSerializer.Deserialize<List<string>>(quyenJson);
            if (!_requiredPermissions.All(p => quyen.Contains(p)))
            {
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new ContentResult
                {
                    Content = "<script>toastr.error('Bạn không có quyền truy cập này'); window.location.href='/admin/index';</script>",
                    ContentType = "text/html"
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}