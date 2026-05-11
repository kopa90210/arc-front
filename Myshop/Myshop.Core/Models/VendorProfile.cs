using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations.Schema;

namespace Myshop.Core.Models
{

public class VendorProfile
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public string? StoreName        { get; set; }
    public string? StoreLogo        { get; set; }
    public string? CoverImage       { get; set; }   // hero banner
    public string? Tagline          { get; set; }   // subtitle under store name
    public string? StoreDescription { get; set; }
    public string? Address          { get; set; }   // "City, Country"
    public bool    IsVerified       { get; set; } = false;

    public decimal Balance    { get; set; } = 0m;   // pending earnings
    public decimal TotalSales { get; set; } = 0m;   // lifetime sales value

    
}

}