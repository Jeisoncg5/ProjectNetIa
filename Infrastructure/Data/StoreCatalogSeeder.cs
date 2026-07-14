using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Domain.Entities;
using ProjectNetIa.Infrastructure.Search;

namespace ProjectNetIa.Infrastructure.Data;

public static class StoreCatalogSeeder
{
    private const string ImgQs = "?auto=format&fit=crop&w=800&q=80";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("PendingModelChangesWarning", StringComparison.Ordinal))
        {
            // Schema already applied; continue seeding catalog data.
        }

        var existingNames = await context.Products
            .Select(product => product.Name)
            .ToListAsync();

        var categoryNames = await context.ProductCategories
            .ToDictionaryAsync(category => category.Id, category => category.Name);

        var colorIds = await context.Colors
            .ToDictionaryAsync(color => color.Name, color => color.Id);

        var sizeIds = await context.Sizes
            .ToDictionaryAsync(size => size.Name, size => size.Id);

        var catalog = BuildCatalog(colorIds, sizeIds);

        foreach (var seedProduct in catalog)
        {
            if (existingNames.Contains(seedProduct.Name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var product = new Product
            {
                ProductCategoryId = seedProduct.CategoryId,
                Name = seedProduct.Name,
                Description = seedProduct.Description,
                ImageUrl = seedProduct.ImageUrl,
                Price = seedProduct.Price,
                IsActive = true,
                Embedding = ProductEmbeddingGenerator.CreateForProduct(
                    categoryNames[seedProduct.CategoryId],
                    seedProduct.Name,
                    seedProduct.Description),
                CreatedAt = DateTime.UtcNow,
            };

            foreach (var seedVariant in seedProduct.Variants)
            {
                var variant = new ProductVariant
                {
                    Product = product,
                    SizeId = seedVariant.SizeId,
                    ColorId = seedVariant.ColorId,
                    Sku = seedVariant.Sku,
                    IsActive = true,
                };

                var inventory = new Inventory
                {
                    ProductVariant = variant,
                    Quantity = seedVariant.Quantity,
                    MinimumQuantity = seedVariant.MinimumQuantity,
                    UpdatedAt = DateTime.UtcNow,
                };

                product.ProductVariants.Add(variant);
                context.Inventories.Add(inventory);
            }

            context.Products.Add(product);
        }

        await SyncProductImagesAsync(context, catalog);
        await SyncProductEmbeddingsAsync(context, categoryNames);
        await context.SaveChangesAsync();
    }

    private static async Task SyncProductImagesAsync(
        ApplicationDbContext context,
        IReadOnlyList<SeedProduct> catalog)
    {
        var imagesByName = catalog.ToDictionary(
            product => product.Name,
            product => product.ImageUrl,
            StringComparer.OrdinalIgnoreCase);

        var products = await context.Products.ToListAsync();
        foreach (var product in products)
        {
            if (!imagesByName.TryGetValue(product.Name, out var imageUrl))
            {
                continue;
            }

            if (!string.Equals(product.ImageUrl, imageUrl, StringComparison.Ordinal))
            {
                product.ImageUrl = imageUrl;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    private static async Task SyncProductEmbeddingsAsync(
        ApplicationDbContext context,
        IReadOnlyDictionary<int, string> categoryNames)
    {
        var productsToSync = (await context.Products
        .Include(product => product.ProductCategory)
        .ToListAsync())
    .Where(product =>
        product.Embedding == null ||
        product.Embedding.ToArray().Length != ProductEmbeddingGenerator.Dimension)
    .ToList();

        foreach (var product in productsToSync)
        {
            var categoryName = product.ProductCategory?.Name
                ?? categoryNames.GetValueOrDefault(product.ProductCategoryId, string.Empty);

            product.Embedding = ProductEmbeddingGenerator.CreateForProduct(
                categoryName,
                product.Name,
                product.Description);
        }
    }

    private static List<SeedProduct> BuildCatalog(
        IReadOnlyDictionary<string, int> colorIds,
        IReadOnlyDictionary<string, int> sizeIds)
    {
        return
        [
            new SeedProduct(
                1,
                "Baby tee viral",
                "Camiseta ajustada de algodon suave, inspirada en tendencias urbanas.",
                39000m,
                $"https://images.unsplash.com/photo-1503342217505-b0a15ec3261c{ImgQs}",
                [
                    SeedVariant.Of("BAB-TEE-BLA-S", "Blanco", "S", 18, 4, colorIds, sizeIds),
                    SeedVariant.Of("BAB-TEE-BLA-M", "Blanco", "M", 16, 4, colorIds, sizeIds),
                    SeedVariant.Of("BAB-TEE-NEG-S", "Negro", "S", 14, 4, colorIds, sizeIds),
                    SeedVariant.Of("BAB-TEE-NEG-M", "Negro", "M", 13, 4, colorIds, sizeIds),
                ]),
            new SeedProduct(
                1,
                "Top mesh midnight",
                "Top semitransparente con vibe nocturna para capas y looks de salida.",
                52000m,
                $"https://images.unsplash.com/photo-1515886657613-9f3515b0c78f{ImgQs}",
                [
                    SeedVariant.Of("TOP-MES-NEG-S", "Negro", "S", 12, 3, colorIds, sizeIds),
                    SeedVariant.Of("TOP-MES-NEG-M", "Negro", "M", 12, 3, colorIds, sizeIds),
                    SeedVariant.Of("TOP-MES-ROJ-S", "Rojo", "S", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("TOP-MES-ROJ-M", "Rojo", "M", 8, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                2,
                "Camisa oversize de lino",
                "Camisa amplia con acabado fresco para outfits casuales y elegantes.",
                89000m,
                $"https://images.unsplash.com/photo-1596755094514-f87e34085b2c{ImgQs}",
                [
                    SeedVariant.Of("CAM-LIN-BEI-M", "Beige", "M", 10, 2, colorIds, sizeIds),
                    SeedVariant.Of("CAM-LIN-BEI-L", "Beige", "L", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("CAM-LIN-BLA-M", "Blanco", "M", 9, 2, colorIds, sizeIds),
                    SeedVariant.Of("CAM-LIN-BLA-L", "Blanco", "L", 7, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                2,
                "Blusa satin tie front",
                "Blusa liviana con textura satinada y nudo frontal para looks mas elevados.",
                76000m,
                $"https://images.unsplash.com/photo-1564257631407-4deb1f99d992{ImgQs}",
                [
                    SeedVariant.Of("BLU-SAT-BEI-S", "Beige", "S", 10, 2, colorIds, sizeIds),
                    SeedVariant.Of("BLU-SAT-BEI-M", "Beige", "M", 10, 2, colorIds, sizeIds),
                    SeedVariant.Of("BLU-SAT-BLA-S", "Blanco", "S", 9, 2, colorIds, sizeIds),
                    SeedVariant.Of("BLU-SAT-BLA-M", "Blanco", "M", 9, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                3,
                "Cargo street utility",
                "Pantalon cargo relaxed fit con bolsillos laterales y look urbano.",
                119000m,
                $"https://images.unsplash.com/photo-1473966968600-fa801b869a1a{ImgQs}",
                [
                    SeedVariant.Of("CAR-STR-GRI-S", "Gris", "S", 11, 3, colorIds, sizeIds),
                    SeedVariant.Of("CAR-STR-GRI-M", "Gris", "M", 9, 3, colorIds, sizeIds),
                    SeedVariant.Of("CAR-STR-NEG-M", "Negro", "M", 12, 3, colorIds, sizeIds),
                    SeedVariant.Of("CAR-STR-NEG-L", "Negro", "L", 8, 3, colorIds, sizeIds),
                ]),
            new SeedProduct(
                3,
                "Falda cargo mini pop",
                "Mini falda con bolsillos utilitarios y silueta pensada para outfits trend.",
                84000m,
                $"https://images.unsplash.com/photo-1583491472933-3e711d939cbd{ImgQs}",
                [
                    SeedVariant.Of("FAL-CAR-NEG-S", "Negro", "S", 11, 3, colorIds, sizeIds),
                    SeedVariant.Of("FAL-CAR-NEG-M", "Negro", "M", 10, 3, colorIds, sizeIds),
                    SeedVariant.Of("FAL-CAR-VER-S", "Verde", "S", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("FAL-CAR-VER-M", "Verde", "M", 8, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                4,
                "Jean wide leg noventero",
                "Jean de tiro alto y silueta amplia con lavado azul clasico.",
                135000m,
                $"https://images.unsplash.com/photo-1541099649105-f69ad21f3246{ImgQs}",
                [
                    SeedVariant.Of("JEA-WID-AZU-S", "Azul", "S", 9, 2, colorIds, sizeIds),
                    SeedVariant.Of("JEA-WID-AZU-M", "Azul", "M", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("JEA-WID-AZU-L", "Azul", "L", 7, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                4,
                "Jean mom acid wash",
                "Jean rigido de lavado acid con fit relajado para combinar con tops y blazers.",
                129000m,
                $"https://images.unsplash.com/photo-1582418702059-97ebafb35d09{ImgQs}",
                [
                    SeedVariant.Of("JEA-MOM-AZU-S", "Azul", "S", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("JEA-MOM-AZU-M", "Azul", "M", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("JEA-MOM-GRI-M", "Gris", "M", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("JEA-MOM-GRI-L", "Gris", "L", 6, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                5,
                "Bomber varsity club",
                "Chaqueta bomber ligera con vibra universitaria y acabados premium.",
                179000m,
                $"https://images.unsplash.com/photo-1551028719-00167b16eac5{ImgQs}",
                [
                    SeedVariant.Of("BOM-VAR-VER-M", "Verde", "M", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("BOM-VAR-VER-L", "Verde", "L", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("BOM-VAR-NEG-L", "Negro", "L", 5, 2, colorIds, sizeIds),
                    SeedVariant.Of("BOM-VAR-NEG-XL", "Negro", "XL", 4, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                5,
                "Blazer cropped city muse",
                "Blazer corto estructurado que eleva outfits casuales con toque elegante.",
                168000m,
                $"https://images.unsplash.com/photo-1594938298603-c8148c4dae35{ImgQs}",
                [
                    SeedVariant.Of("BLA-CRO-GRI-M", "Gris", "M", 7, 2, colorIds, sizeIds),
                    SeedVariant.Of("BLA-CRO-GRI-L", "Gris", "L", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("BLA-CRO-NEG-M", "Negro", "M", 7, 2, colorIds, sizeIds),
                    SeedVariant.Of("BLA-CRO-NEG-L", "Negro", "L", 6, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                6,
                "Vestido slip satinado",
                "Vestido ligero con brillo sutil para salidas nocturnas y eventos.",
                149000m,
                $"https://images.unsplash.com/photo-1566174053879-31528523f8ae{ImgQs}",
                [
                    SeedVariant.Of("VES-SLI-ROJ-S", "Rojo", "S", 5, 1, colorIds, sizeIds),
                    SeedVariant.Of("VES-SLI-ROJ-M", "Rojo", "M", 5, 1, colorIds, sizeIds),
                    SeedVariant.Of("VES-SLI-NEG-M", "Negro", "M", 4, 1, colorIds, sizeIds),
                ]),
            new SeedProduct(
                6,
                "Vestido floral soft girl",
                "Vestido fresco estampado de vibra romantica para dias soleados y planes casuales.",
                112000m,
                $"https://images.unsplash.com/photo-1496747611176-843222e1e57c{ImgQs}",
                [
                    SeedVariant.Of("VES-FLO-BLA-S", "Blanco", "S", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("VES-FLO-BLA-M", "Blanco", "M", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("VES-FLO-ROJ-S", "Rojo", "S", 5, 2, colorIds, sizeIds),
                    SeedVariant.Of("VES-FLO-ROJ-M", "Rojo", "M", 5, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                7,
                "Tenis chunky nube",
                "Tenis comodos con suela robusta y estilo chunky para diario.",
                199000m,
                $"https://images.unsplash.com/photo-1542291026-7eec264c27ff{ImgQs}",
                [
                    SeedVariant.Of("TEN-CHU-BLA-38", "Blanco", "38", 7, 2, colorIds, sizeIds),
                    SeedVariant.Of("TEN-CHU-BLA-39", "Blanco", "39", 7, 2, colorIds, sizeIds),
                    SeedVariant.Of("TEN-CHU-BEI-40", "Beige", "40", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("TEN-CHU-BEI-41", "Beige", "41", 5, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                7,
                "Sandalias platform sunset",
                "Sandalias de plataforma ligera para elevar outfits relajados sin perder comodidad.",
                145000m,
                $"https://images.unsplash.com/photo-1543163521-1bf539c55dd2{ImgQs}",
                [
                    SeedVariant.Of("SAN-PLA-NEG-38", "Negro", "38", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("SAN-PLA-NEG-39", "Negro", "39", 6, 2, colorIds, sizeIds),
                    SeedVariant.Of("SAN-PLA-BEI-40", "Beige", "40", 5, 2, colorIds, sizeIds),
                    SeedVariant.Of("SAN-PLA-BEI-41", "Beige", "41", 5, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                8,
                "Bolso mini baguette",
                "Bolso compacto para looks de salida con acabado suave y herrajes brillantes.",
                95000m,
                $"https://images.unsplash.com/photo-1584917865442-de89df76afd3{ImgQs}",
                [
                    SeedVariant.Of("BOL-BAG-NEG-XS", "Negro", "XS", 10, 2, colorIds, sizeIds),
                    SeedVariant.Of("BOL-BAG-ROJ-XS", "Rojo", "XS", 8, 2, colorIds, sizeIds),
                    SeedVariant.Of("BOL-BAG-BEI-XS", "Beige", "XS", 7, 2, colorIds, sizeIds),
                ]),
            new SeedProduct(
                8,
                "Gorra washed club",
                "Gorra curva con acabado desgastado para completar looks street sin esfuerzo.",
                48000m,
                $"https://images.unsplash.com/photo-1588850561407-ed78c282e89b{ImgQs}",
                [
                    SeedVariant.Of("GOR-WAS-NEG-XS", "Negro", "XS", 12, 3, colorIds, sizeIds),
                    SeedVariant.Of("GOR-WAS-AZU-XS", "Azul", "XS", 10, 3, colorIds, sizeIds),
                    SeedVariant.Of("GOR-WAS-BEI-XS", "Beige", "XS", 10, 3, colorIds, sizeIds),
                ]),
        ];
    }

    private sealed record SeedProduct(
        int CategoryId,
        string Name,
        string Description,
        decimal Price,
        string ImageUrl,
        IReadOnlyList<SeedVariant> Variants);

    private sealed record SeedVariant(
        string Sku,
        int ColorId,
        int SizeId,
        int Quantity,
        int MinimumQuantity)
    {
        public static SeedVariant Of(
            string sku,
            string color,
            string size,
            int quantity,
            int minimumQuantity,
            IReadOnlyDictionary<string, int> colorIds,
            IReadOnlyDictionary<string, int> sizeIds)
        {
            return new SeedVariant(
                sku,
                colorIds[color],
                sizeIds[size],
                quantity,
                minimumQuantity);
        }
    }
}
