using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using eShopLegacyMVC.Models;
using eShopLegacyMVC.Services;

namespace eShopLegacyMVC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly ICatalogService _service;
        private readonly IConfiguration _configuration;

        public CatalogController(ICatalogService service, ILogger<CatalogController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index(int pageSize = 10, int pageIndex = 0)
        {
            _logger.LogInformation($"Now loading... /Catalog/Index?pageSize={pageSize}&pageIndex={pageIndex}");
            var paginatedItems = _service.GetCatalogItemsPaginated(pageSize, pageIndex);
            ChangeUriPlaceholder(paginatedItems.Data);
            return View(paginatedItems);
        }

        public IActionResult Details(int? id)
        {
            _logger.LogInformation($"Now loading... /Catalog/Details?id={id}");
            if (id == null)
            {
                return BadRequest();
            }
            CatalogItem catalogItem = _service.FindCatalogItem(id.Value);
            if (catalogItem == null)
            {
                return NotFound();
            }
            AddUriPlaceHolder(catalogItem);

            return View(catalogItem);
        }

        public IActionResult Create()
        {
            _logger.LogInformation($"Now loading... /Catalog/Create");
            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand");
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type");
            return View(new CatalogItem());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Description,Price,PictureFileName,CatalogTypeId,CatalogBrandId,AvailableStock,RestockThreshold,MaxStockThreshold,OnReorder")] CatalogItem catalogItem)
        {
            _logger.LogInformation($"Now processing... /Catalog/Create?catalogItemName={catalogItem.Name}");
            if (ModelState.IsValid)
            {
                _service.CreateCatalogItem(catalogItem);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand", catalogItem.CatalogBrandId);
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type", catalogItem.CatalogTypeId);
            return View(catalogItem);
        }

        public IActionResult Edit(int? id)
        {
            _logger.LogInformation($"Now loading... /Catalog/Edit?id={id}");
            if (id == null)
            {
                return BadRequest();
            }
            CatalogItem catalogItem = _service.FindCatalogItem(id.Value);
            if (catalogItem == null)
            {
                return NotFound();
            }
            AddUriPlaceHolder(catalogItem);
            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand", catalogItem.CatalogBrandId);
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type", catalogItem.CatalogTypeId);
            return View(catalogItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Name,Description,Price,PictureFileName,CatalogTypeId,CatalogBrandId,AvailableStock,RestockThreshold,MaxStockThreshold,OnReorder")] CatalogItem catalogItem)
        {
            _logger.LogInformation($"Now processing... /Catalog/Edit?id={catalogItem.Id}");
            if (ModelState.IsValid)
            {
                _service.UpdateCatalogItem(catalogItem);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand", catalogItem.CatalogBrandId);
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type", catalogItem.CatalogTypeId);
            return View(catalogItem);
        }

        public IActionResult Delete(int? id)
        {
            _logger.LogInformation($"Now loading... /Catalog/Delete?id={id}");
            if (id == null)
            {
                return BadRequest();
            }
            CatalogItem catalogItem = _service.FindCatalogItem(id.Value);
            if (catalogItem == null)
            {
                return NotFound();
            }
            AddUriPlaceHolder(catalogItem);

            return View(catalogItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Now processing... /Catalog/DeleteConfirmed?id={id}");
            CatalogItem catalogItem = _service.FindCatalogItem(id);
            _service.RemoveCatalogItem(catalogItem);
            return RedirectToAction(nameof(Index));
        }

        private void ChangeUriPlaceholder(IEnumerable<CatalogItem> items)
        {
            foreach (var catalogItem in items)
            {
                AddUriPlaceHolder(catalogItem);
            }
        }

        private void AddUriPlaceHolder(CatalogItem item)
        {
            item.PictureUri = Url.RouteUrl("GetPicture", new { catalogItemId = item.Id }, Request.Scheme);
        }
    }
}
