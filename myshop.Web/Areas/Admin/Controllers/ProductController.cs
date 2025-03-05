using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;

namespace myshop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        //var products = _unitOfWork.Product.GetAll();
        //return View(products);
        // It will be viewed by datatables using jquery
        return View();
    }

    [HttpGet]
    public IActionResult GetData()
    {
        var products = _unitOfWork.Product.GetAll(includeEntity:"Category");
        return Json(new { data = products });
    }

    [HttpGet]
    public IActionResult Create()
    {
        ProductVM productVM = new ProductVM()
        {
            CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            })
        };
        return View(productVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductVM productVM, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(rootPath, @"Images/Products");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVM.ImgPath = @"Images/Products/"+ fileName + extension;
            }
            var newProduct = new Product { 
                ProductName = productVM.ProductName,
                ProductDescription = productVM.ProductDescription,
                ImgPath = productVM.ImgPath,
                Price = productVM.Price,
                CategoryId = productVM.CategoryId
            };
            _unitOfWork.Product.Add(newProduct);
            _unitOfWork.Complete();
            TempData["CreateMsg"] = "Item has created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(productVM);
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null | id == 0)
        {
            return NotFound();
        }

        Product product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);

        ProductVM productVM = new ProductVM()
        {
            Id = product.Id,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ImgPath = product.ImgPath,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            })
        };
        return View(productVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(ProductVM productVM, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(rootPath, @"Images/Products");
                var extension = Path.GetExtension(file.FileName);

                if (productVM.ImgPath != null)
                {
                    var oldImg = Path.Combine(rootPath, productVM.ImgPath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImg))
                    {
                        System.IO.File.Delete(oldImg);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVM.ImgPath = @"Images/Products/" + fileName + extension;
            }
            var newProduct = new Product
            {
                ProductName = productVM.ProductName,
                ProductDescription = productVM.ProductDescription,
                ImgPath = productVM.ImgPath,
                Price = productVM.Price,
                CategoryId = productVM.CategoryId
            };
            _unitOfWork.Product.Update(newProduct);
            _unitOfWork.Complete();
            TempData["UpdateMsg"] = "Item has updated successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(productVM);
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
        string rootPath = _webHostEnvironment.WebRootPath;
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        _unitOfWork.Product.Remove(product);
        var oldImg = Path.Combine(rootPath, product.ImgPath.TrimStart('\\'));
        if (System.IO.File.Exists(oldImg))
        {
            System.IO.File.Delete(oldImg);
        }
        _unitOfWork.Complete();
        return Json(new { success = true, message = "Item has deleted successfully" });
    }
}