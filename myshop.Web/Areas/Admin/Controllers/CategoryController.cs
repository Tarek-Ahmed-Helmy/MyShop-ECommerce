using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;

namespace myshop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private IUnitOfWork _unitOfWork;
    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var categories = _unitOfWork.Category.GetAll();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(category);
            _unitOfWork.Complete();
            TempData["CreateMsg"] = "Item has created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null | id == 0)
        {
            NotFound();
        }
        var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(category);
            _unitOfWork.Complete();
            TempData["UpdateMsg"] = "Item has updated successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [HttpGet]
    public IActionResult Delete(int? id)
    {
        if (id == null | id == 0)
        {
            NotFound();
        }
        var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteCat(int? id)
    {
        var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
        if (category == null)
        {
            NotFound();
        }
        _unitOfWork.Category.Remove(category);
        _unitOfWork.Complete();
        TempData["DeleteMsg"] = "Item has deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}