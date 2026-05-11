using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;
using System.Text.Json;

namespace Myshop.Infrastructure.Context
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Ensure Roles
            string[] roles = { "Admin", "Vendor", "Creator", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admin
            var adminEmail = "admin@myshop.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 3. Seed Vendors
            var vendorsInfo = new[]
            {
                new { Email = "vendor1@test.com", Name = "Maison Noir", Desc = "High-end luxury streetwear and accessories.", Profile = "https://images.unsplash.com/photo-1594913785162-e6785b4ad310?q=80&w=200&h=200&auto=format&fit=crop", Logo = "https://images.unsplash.com/photo-1541140134513-85a161dc4a00?q=80&w=150&h=150&auto=format&fit=crop" },
                new { Email = "vendor2@test.com", Name = "Urban Edge", Desc = "Modern urban wear for the digital generation.", Profile = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=200&h=200&auto=format&fit=crop", Logo = "https://images.unsplash.com/photo-1531297484001-80022131f5a1?q=80&w=150&h=150&auto=format&fit=crop" },
                new { Email = "vendor3@test.com", Name = "Aethelwear", Desc = "Sustainable fashion for the conscious mind.", Profile = "https://images.unsplash.com/photo-1528698827591-e19ccd7bc23d?q=80&w=200&h=200&auto=format&fit=crop", Logo = "https://images.unsplash.com/photo-1516280440502-628d09cb8d9e?q=80&w=150&h=150&auto=format&fit=crop" },
                new { Email = "vendor4@test.com", Name = "Lumina", Desc = "Bright, vibrant, and elegant designs.", Profile = "https://images.unsplash.com/photo-1490481651871-ab68de25d43d?q=80&w=200&h=200&auto=format&fit=crop", Logo = "https://images.unsplash.com/photo-1445205170230-053b83016050?q=80&w=150&h=150&auto=format&fit=crop" }
            };

            foreach (var vInfo in vendorsInfo)
            {
                if (await userManager.FindByEmailAsync(vInfo.Email) == null)
                {
                    var vendorUser = new AppUser
                    {
                        UserName = vInfo.Email,
                        Email = vInfo.Email,
                        FullName = vInfo.Name + " Store",
                        ProfileImage = vInfo.Profile,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(vendorUser, "Vendor@123");
                    await userManager.AddToRoleAsync(vendorUser, "Vendor");

                    context.VendorProfiles.Add(new VendorProfile
                    {
                        UserId = vendorUser.Id,
                        StoreName = vInfo.Name,
                        StoreDescription = vInfo.Desc,
                        StoreLogo = vInfo.Logo,
                        Address = "Fashion District, Global",
                        Balance = 2500.00m,
                        TotalSales = 5000.00m
                    });
                }
            }
            await context.SaveChangesAsync();

            // 4. Seed Products
            if (!context.Products.Any() || context.Products.Count() < 10)
            {
                var v1Id = (await userManager.FindByEmailAsync(vendorsInfo[0].Email)).Id;
                var v2Id = (await userManager.FindByEmailAsync(vendorsInfo[1].Email)).Id;
                var v3Id = (await userManager.FindByEmailAsync(vendorsInfo[2].Email)).Id;
                var v4Id = (await userManager.FindByEmailAsync(vendorsInfo[3].Email)).Id;

                var products = new List<Product>
                {
                    // Maison Noir
                    new Product { Name = "Noir Oversized Hoodie", Description = "Premium heavy-weight cotton oversized hoodie.", Price = 120.00m, Category = "Tops", Brand = "Maison Noir", Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f95609a7?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v1Id, Rating = 4.8m, RatingCount = 12, IsNew = true },
                    new Product { Name = "Shadow Cargo Pants", Description = "Multi-pocket technical cargo pants.", Price = 150.00m, Category = "Pants", Brand = "Maison Noir", Stock = 30, ImageUrl = "https://images.unsplash.com/photo-1594633312681-425c7b97ccd1?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v1Id, Rating = 4.5m, RatingCount = 8 },
                    new Product { Name = "Minimalist Tote Bag", Description = "Vegan leather sleek tote bag.", Price = 85.00m, Category = "Accessories", Brand = "Maison Noir", Stock = 40, ImageUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v1Id, Rating = 4.7m, RatingCount = 15 },
                    new Product { Name = "Silk Evening Dress", Description = "Elegant silk slip dress.", Price = 250.00m, Category = "Dresses", Brand = "Maison Noir", Stock = 15, ImageUrl = "https://images.unsplash.com/photo-1566150905458-1bf1fc113f0d?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v1Id, Rating = 4.9m, RatingCount = 20 },
                    
                    // Urban Edge
                    new Product { Name = "Cyberpunk Tee", Description = "Neon print oversized tee.", Price = 45.00m, Category = "Tops", Brand = "Urban Edge", Stock = 100, ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v2Id, Rating = 4.2m, RatingCount = 25, IsNew = true },
                    new Product { Name = "Vaporwave Sneakers", Description = "Limited edition colorway sneakers.", Price = 210.00m, Category = "Shoes", Brand = "Urban Edge", Stock = 15, ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v2Id, Rating = 4.9m, RatingCount = 5, SalePrice = 189.00m },
                    new Product { Name = "Distressed Denim Jacket", Description = "Vintage wash distressed jacket.", Price = 110.00m, Category = "Outerwear", Brand = "Urban Edge", Stock = 45, ImageUrl = "https://images.unsplash.com/photo-1576871337622-98d48d1cf531?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v2Id, Rating = 4.4m, RatingCount = 31 },

                    // Aethelwear
                    new Product { Name = "Organic Linen Shirt", Description = "Breathable 100% organic linen.", Price = 75.00m, Category = "Tops", Brand = "Aethelwear", Stock = 60, ImageUrl = "https://images.unsplash.com/photo-1598033129183-c4f50c736f10?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v3Id, Rating = 4.6m, RatingCount = 19 },
                    new Product { Name = "Earth Tone Chinos", Description = "Comfortable everyday chinos.", Price = 85.00m, Category = "Pants", Brand = "Aethelwear", Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v3Id, Rating = 4.3m, RatingCount = 14 },
                    new Product { Name = "Recycled Wool Coat", Description = "Warm and sustainable winter coat.", Price = 195.00m, Category = "Outerwear", Brand = "Aethelwear", Stock = 20, ImageUrl = "https://images.unsplash.com/photo-1539533113208-f6df8cc8b543?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v3Id, Rating = 4.8m, RatingCount = 7 },

                    // Lumina
                    new Product { Name = "Golden Hour Maxi Dress", Description = "Flowy summer maxi dress.", Price = 140.00m, Category = "Dresses", Brand = "Lumina", Stock = 25, ImageUrl = "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v4Id, Rating = 4.7m, RatingCount = 33 },
                    new Product { Name = "Crystal Drop Earrings", Description = "Elegant evening accessories.", Price = 55.00m, Category = "Accessories", Brand = "Lumina", Stock = 80, ImageUrl = "https://images.unsplash.com/photo-1535632066927-ab7c9ab60908?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v4Id, Rating = 4.5m, RatingCount = 42 },
                    new Product { Name = "Satin Slip Skirt", Description = "Versatile champagne satin skirt.", Price = 89.00m, Category = "Pants", Brand = "Lumina", Stock = 35, ImageUrl = "https://images.unsplash.com/photo-1582142306909-195724d33ffc?q=80&w=400&h=500&auto=format&fit=crop", VendorId = v4Id, Rating = 4.6m, RatingCount = 11 }
                };

                // Only add products that don't already exist by name
                foreach(var p in products)
                {
                    if(!context.Products.Any(cp => cp.Name == p.Name))
                    {
                        context.Products.Add(p);
                    }
                }
                await context.SaveChangesAsync();
            }

            var allProducts = await context.Products.ToListAsync();

            // 5. Seed Creators
            var creatorsInfo = new[]
            {
                new { Email = "creator1@test.com", Name = "Elena Styles", Profile = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?q=80&w=200&h=200&auto=format&fit=crop", Bio = "Digital stylist and fashion minimalist. Sharing my daily looks.", Tags = new[] { "minimalist", "chic", "luxury" } },
                new { Email = "creator2@test.com", Name = "Marcus Trend", Profile = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?q=80&w=200&h=200&auto=format&fit=crop", Bio = "Streetwear aficionado. Exploring urban fashion.", Tags = new[] { "streetwear", "urban", "sneakers" } },
                new { Email = "creator3@test.com", Name = "Sophia Vintage", Profile = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=200&h=200&auto=format&fit=crop", Bio = "Thrift queen and vintage lover.", Tags = new[] { "vintage", "sustainable", "retro" } }
            };

            foreach (var cInfo in creatorsInfo)
            {
                var user = await userManager.FindByEmailAsync(cInfo.Email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = cInfo.Email,
                        Email = cInfo.Email,
                        FullName = cInfo.Name,
                        ProfileImage = cInfo.Profile,
                        CoverImage = "https://images.unsplash.com/photo-1441986300917-64674bd600d8?q=80&w=800&h=300&auto=format&fit=crop",
                        Bio = cInfo.Bio,
                        Location = "Global",
                        StyleTagsJson = JsonSerializer.Serialize(cInfo.Tags),
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "Creator@123");
                    await userManager.AddToRoleAsync(user, "Creator");

                    // Seed Outfits & Selfies for this creator
                    for(int i = 0; i < 3; i++)
                    {
                        var sampleProduct1 = allProducts.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                        var sampleProduct2 = allProducts.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        if (sampleProduct1 != null)
                        {
                            var outfit = new Outfit
                            {
                                UserId = user.Id,
                                Name = $"{cInfo.Tags[0]} Look {i + 1}",
                                Description = $"Curated {cInfo.Tags[0]} style featuring premium pieces.",
                                Tags = string.Join(",", cInfo.Tags),
                                CreatedAt = DateTime.UtcNow.AddDays(-i),
                                IsPublic = true
                            };
                            context.Outfits.Add(outfit);
                            await context.SaveChangesAsync();

                            context.OutfitItems.Add(new OutfitItem
                            {
                                OutfitId = outfit.Id,
                                ItemId = sampleProduct1.Id,
                                ItemType = "Product",
                                PositionX = 50,
                                PositionY = 100
                            });
                            
                            if (sampleProduct2 != null)
                            {
                                context.OutfitItems.Add(new OutfitItem
                                {
                                    OutfitId = outfit.Id,
                                    ItemId = sampleProduct2.Id,
                                    ItemType = "Product",
                                    PositionX = 150,
                                    PositionY = 150
                                });
                            }
                        }

                        // Seed a Selfie
                        context.Selfies.Add(new Selfie
                        {
                            UserId = user.Id,
                            Caption = $"Loving the {cInfo.Tags[0]} vibes today ✨",
                            ImageUrl = i % 2 == 0 ? "https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?q=80&w=400&h=600&auto=format&fit=crop" : "https://images.unsplash.com/photo-1483985988355-763728e1935b?q=80&w=400&h=600&auto=format&fit=crop",
                            IsPublic = true,
                            CreatedAt = DateTime.UtcNow.AddDays(-i)
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
