using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<PlantCareDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddOData(options => 
    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(50));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlantCareDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapGet("/api/plants", async (
    PlantCareDbContext db,
    string? status,
    string? location,
    string? species,
    DateTime? createdAfter,
    DateTime? createdBefore,
    string? sortBy,
    bool descending = false,
    int page = 1,
    int pageSize = 20) =>
{
    var query = db.Plants.AsQueryable();
    
    if (!string.IsNullOrEmpty(status))
        query = query.Where(p => p.Status == status);
    
    if (!string.IsNullOrEmpty(location))
        query = query.Where(p => p.Location.Contains(location));
    
    if (!string.IsNullOrEmpty(species))
        query = query.Where(p => p.Species.Contains(species));
    
    if (createdAfter.HasValue)
        query = query.Where(p => p.CreatedAt >= createdAfter.Value);
    
    if (createdBefore.HasValue)
        query = query.Where(p => p.CreatedAt <= createdBefore.Value);
    
    // Sorting
    query = sortBy?.ToLower() switch
    {
        "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
        "species" => descending ? query.OrderByDescending(p => p.Species) : query.OrderBy(p => p.Species),
        "createdat" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
        "location" => descending ? query.OrderByDescending(p => p.Location) : query.OrderBy(p => p.Location),
        _ => query.OrderByDescending(p => p.CreatedAt)
    };
    
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return Results.Ok(new
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        Items = items
    });
});

app.MapGet("/api/plants/{id:int}", async (int id, PlantCareDbContext db) =>
{
    var plant = await db.Plants
        .Include(p => p.Observations)
        .Include(p => p.CareActions)
        .FirstOrDefaultAsync(p => p.Id == id);
    
    return plant is not null ? Results.Ok(plant) : Results.NotFound();
});

app.MapPost("/api/plants", async (Plant plant, PlantCareDbContext db) =>
{
    plant.CreatedAt = DateTime.UtcNow;
    plant.Status = "Active";
    
    db.Plants.Add(plant);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/plants/{plant.Id}", plant);
});

app.MapPut("/api/plants/{id:int}", async (int id, Plant updatedPlant, PlantCareDbContext db) =>
{
    var plant = await db.Plants.FindAsync(id);
    if (plant is null) return Results.NotFound();
    
    plant.Name = updatedPlant.Name;
    plant.Species = updatedPlant.Species;
    plant.Location = updatedPlant.Location;
    plant.Status = updatedPlant.Status;
    
    await db.SaveChangesAsync();
    return Results.Ok(plant);
});

app.MapDelete("/api/plants/{id:int}", async (int id, PlantCareDbContext db) =>
{
    var plant = await db.Plants.FindAsync(id);
    if (plant is null) return Results.NotFound();
    
    db.Plants.Remove(plant);
    await db.SaveChangesAsync();
    
    return Results.NoContent();
});

// Observations endpoints
app.MapGet("/api/plants/{plantId:int}/observations", async (int plantId, PlantCareDbContext db) =>
{
    return await db.Observations
        .Where(o => o.PlantId == plantId)
        .Include(o => o.HealthAssessment)
        .OrderByDescending(o => o.Timestamp)
        .ToListAsync();
});

app.MapPost("/api/observations", async (Observation observation, PlantCareDbContext db) =>
{
    observation.Timestamp = DateTime.UtcNow;
    
    db.Observations.Add(observation);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/observations/{observation.Id}", observation);
});

// Care Actions endpoints
app.MapGet("/api/plants/{plantId:int}/care-actions", async (int plantId, PlantCareDbContext db) =>
{
    return await db.CareActions
        .Where(c => c.PlantId == plantId)
        .Include(c => c.CareActionType)
        .OrderByDescending(c => c.ExecutedAt)
        .ToListAsync();
});

app.MapPost("/api/care-actions", async (CareAction careAction, PlantCareDbContext db) =>
{
    careAction.ExecutedAt = DateTime.UtcNow;
    
    db.CareActions.Add(careAction);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/care-actions/{careAction.Id}", careAction);
});

// Care Action Types endpoints
app.MapGet("/api/care-action-types", async (PlantCareDbContext db) => await db.CareActionTypes.ToListAsync());

app.MapPost("/api/care-action-types", async (CareActionType type, PlantCareDbContext db) =>
{
    db.CareActionTypes.Add(type);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/care-action-types/{type.Id}", type);
});

// Health Assessment endpoint
app.MapPost("/api/health-assessments", async (HealthAssessment assessment, PlantCareDbContext db) =>
{
    db.HealthAssessments.Add(assessment);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/health-assessments/{assessment.Id}", assessment);
});

app.Run();