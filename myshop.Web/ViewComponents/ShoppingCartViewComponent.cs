﻿using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;

namespace myshop.Web.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            if (HttpContext.Session.GetInt32(SD.SessionKey) != null)
            {
                return View(HttpContext.Session.GetInt32(SD.SessionKey));
            }
            else
            {
                var count = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SessionKey, count);
                return View(HttpContext.Session.GetInt32(SD.SessionKey));
            }
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}
