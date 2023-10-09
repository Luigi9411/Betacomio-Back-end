using System;
using System.Collections.Generic;

namespace Betacomio.Models;

public partial class VProductDescriptionPrice
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public decimal ListPrice { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte[]? ThumbNailPhoto { get; set; }

    public string Culture { get; set; } = null!;

    public string? ThumbnailPhotoFileName { get; set; }
}
