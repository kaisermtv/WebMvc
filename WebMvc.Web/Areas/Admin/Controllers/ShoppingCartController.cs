﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Domain.Constants;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Web.Application;
using WebMvc.Web.Areas.Admin.ViewModels;

namespace WebMvc.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class ShoppingCartController : BaseAdminController
    {
        public readonly IShoppingCartProductService _shoppingCartProductService;
        public readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController() : base()
        {
            _shoppingCartProductService = ServiceFactory.Get<IShoppingCartProductService>();
            _shoppingCartService = ServiceFactory.Get<IShoppingCartService>();
        }

        public ShoppingCartController(IShoppingCartService shoppingCartService,IShoppingCartProductService shoppingCartProductService,ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ISettingsService settingsService, ILocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _shoppingCartService = shoppingCartService;
            _shoppingCartProductService = shoppingCartProductService;
        }

        // GET: Admin/ShoppingCart
        public ActionResult Index(int? p)
        {
            int limit = 20;
            var count = _shoppingCartService.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new ShoppingCartViewModel
            {
                Paging = Paging,
                shoppingCarts = _shoppingCartService.GetList(limit, Paging.Page),
            };

            return View(viewModel);
        }

        public ActionResult Edit(Guid guid)
        {
            var cart = _shoppingCartService.Get(guid);
            if (cart == null) return RedirectToAction("Index", "ShoppingCart");

            var viewModel = new ShoppingCartEditViewModel
            {
                Id = cart.Id,
                Name = cart.Name,
                Email = cart.Email,
                Phone = cart.Phone,
                Addren = cart.Addren,
                ShipName = cart.ShipName,
                ShipPhone = cart.ShipPhone,
                ShipAddren = cart.ShipAddren,
                ShipNote = cart.ShipNote,
                TotalMoney = cart.TotalMoney,
                Note = cart.Note,
                Status = cart.Status,
                CreateDate = cart.CreateDate
            };

            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShoppingCartEditViewModel viewModel)
        {
            var cart = _shoppingCartService.Get(viewModel.Id);
            if (cart == null) return RedirectToAction("Index", "ShoppingCart");

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {



                }
            }

            return View(viewModel);
        }
    }
}