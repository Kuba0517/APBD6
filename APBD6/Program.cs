using System.Data.SqlClient;
using APBD6.DTOs;
using APBD6.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAnimalRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateAnimalRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("animals", (IConfiguration configuration, string orderBy = "") =>
{
    var animals = new List<GetAllAnimalsResponse>();
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand = orderBy switch
        {
            "IdAnimal" => new SqlCommand("SELECT * FROM Animal ORDER BY IdAnimal", sqlConnection),
            "Name" => new SqlCommand("SELECT * FROM Animal ORDER BY Name", sqlConnection),
            "Area" => new SqlCommand("SELECT * FROM Animal ORDER BY Area", sqlConnection),
            "Description" => new SqlCommand("SELECT * FROM Animal ORDER BY Description", sqlConnection),
            "Category" => new SqlCommand("SELECT * FROM Animal ORDER BY Category", sqlConnection),
            _ => new SqlCommand("SELECT * FROM Animal", sqlConnection)
        };

        sqlCommand.Connection.Open();
        var reader = sqlCommand.ExecuteReader();
        
        while (reader.Read())
        {
            animals.Add(new GetAllAnimalsResponse(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)
            ));
        }
    }

    return Results.Ok(animals);
});

app.MapGet("animals/{id:int}", (int id, IConfiguration configuration) =>
{
    var animals = new List<GetAllAnimalsResponse>();
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand = new SqlCommand($"SELECT * FROM Animal WHERE IdAnimal = @id", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@id", id);
        sqlCommand.Connection.Open();
        var reader = sqlCommand.ExecuteReader();
        
        while (reader.Read())
        {
            animals.Add(new GetAllAnimalsResponse(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)
            ));
        }
    }

    return animals.Count == 0 ? Results.NotFound() : Results.Ok(animals);
});

app.MapPost(("/animals"),
    (IConfiguration configuration, CreateAnimalRequest request, IValidator<CreateAnimalRequest> validator) =>
    {
        var validation = validator.Validate(request);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var sqlCommand =
                new SqlCommand("INSERT INTO ANIMAL (Name, Description, Category, Area) VALUES (@n, @d, @c, @a) ",
                    sqlConnection);
            sqlCommand.Parameters.AddWithValue("@n", request.Name);
            sqlCommand.Parameters.AddWithValue("@d", request.Description);
            sqlCommand.Parameters.AddWithValue("@c", request.Category);
            sqlCommand.Parameters.AddWithValue("@a", request.Area);
            sqlConnection.Open();

            sqlCommand.ExecuteNonQuery();
        }

        return Results.Created();
    });

app.MapPut(("/animals/{id:int}"), (int id, IConfiguration configuration, UpdateAnimalRequest request, IValidator<UpdateAnimalRequest> validator) =>
{
    var validation = validator.Validate(request);
    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand =
            new SqlCommand("UPDATE Animal SET Name = @n, Description = @d, Category = @c, Area = @a WHERE IdAnimal = @id",
                sqlConnection);
        sqlCommand.Parameters.AddWithValue("@n", request.Name);
        sqlCommand.Parameters.AddWithValue("@d", request.Description);
        sqlCommand.Parameters.AddWithValue("@c", request.Category);
        sqlCommand.Parameters.AddWithValue("@a", request.Area);
        sqlCommand.Parameters.AddWithValue("@id", id);
        
        sqlConnection.Open();

        sqlCommand.ExecuteNonQuery();
    }

    return Results.Ok();
});

app.MapDelete(("/animals/{id:int}"), (int id, IConfiguration configuration) =>
{
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand =
            new SqlCommand("DELETE ANIMAL WHERE IdAnimal = @id",
                sqlConnection);
        sqlCommand.Parameters.AddWithValue("@id", id);
        
        sqlConnection.Open();

        sqlCommand.ExecuteNonQuery();
    }

    return Results.Ok();
});


app.Run();