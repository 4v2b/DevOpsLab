using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

builder.Services.AddDbContext<PlantCareDbContext>(options =>
    options.UseSqlite("Data Source=plantjournal.db"));

builder.Services.AddControllers().AddOData(options => 
    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(50));

builder.Services.AddEndpointsApiExplorer();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Plants endpoints
app.MapGet("/api/plants", (PlantCareDbContext db, ODataQueryOptions<Plant> options) =>
    {
        IQueryable<Plant> query = db.Plants;
    
        var result = options.ApplyTo(query);
        return Task.FromResult(Results.Ok(result));
    })
    .WithName("GetPlants");

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