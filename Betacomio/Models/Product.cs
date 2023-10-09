using System;
using System.Collections.Generic;

namespace Betacomio.Models;

/// <summary>
/// Products sold or used in the manfacturing of sold products.
/// </summary>
public partial class Product
{
    /// <summary>
    /// Primary key for Product records.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Name of the product.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Unique product identification number.
    /// </summary>
    public string ProductNumber { get; set; } = null!;

    /// <summary>
    /// Product color.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Standard cost of the product.
    /// </summary>
    public decimal StandardCost { get; set; }

    /// <summary>
    /// Selling price.
    /// </summary>
    public decimal ListPrice { get; set; }

    /// <summary>
    /// Product size.
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// Product weight.
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Product is a member of this product category. Foreign key to ProductCategory.ProductCategoryID. 
    /// </summary>
    public int? ProductCategoryId { get; set; }

    /// <summary>
    /// Product is a member of this product model. Foreign key to ProductModel.ProductModelID.
    /// </summary>
    public int? ProductModelId { get; set; }

    /// <summary>
    /// Date the product was available for sale.
    /// </summary>
    public DateTime SellStartDate { get; set; }

    /// <summary>
    /// Date the product was no longer available for sale.
    /// </summary>
    public DateTime? SellEndDate { get; set; }

    /// <summary>
    /// Date the product was discontinued.
    /// </summary>
    public DateTime? DiscontinuedDate { get; set; }

    /// <summary>
    /// Small image of the product.
    /// </summary>
    public byte[]? ThumbNailPhoto { get; set; }

    /// <summary>
    /// Small image file name.
    /// </summary>
    public string? ThumbnailPhotoFileName { get; set; }

    /// <summary>
    /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
    /// </summary>
    public Guid Rowguid { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public virtual ProductCategory? ProductCategory { get; set; }

    public virtual ProductModel? ProductModel { get; set; }

    public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();

    public Product() { }

    public Product(string init)
    {
        if (init.Equals("init"))
        {

            string guidString = Guid.NewGuid().ToString();
            string limitedString = guidString.Substring(0, Math.Min(guidString.Length, 25));

            ProductId = 0;
            Name = Guid.NewGuid().ToString();
            ProductNumber = limitedString;
            Color = "Black";
            StandardCost = 0;
            ListPrice = 0;
            Size = "";
            Weight = 1;
            ProductCategoryId = 18;
            ProductModelId = 6;
            SellStartDate = DateTime.Now;
        SellEndDate = null;
            DiscontinuedDate = null;
            ThumbNailPhoto = null;
            ThumbnailPhotoFileName = "no_image_available_small.gif";
            Rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
            ProductCategory = null;
            ProductModel = null;
        }

    }
}
