using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ClientesPruebaTecnicaContext>(Options =>
    Options.UseMySql(builder.Configuration.GetConnectionString("conexion"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapGet("/Clientes", async (ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.Clientes.ToListAsync());

app.MapGet("/ClientesDocumentos", async (ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.GetClientesConTipoDocumento(ClienteDb).ToListAsync());

app.MapGet("/TiposDocumento", async (ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.TiposDocumento.ToListAsync());

app.MapGet("/Clientes/{id}", async (int id, ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.Clientes.FindAsync(id)
        is Cliente cliente
            ? Results.Ok(cliente)
            : Results.NotFound());

app.MapPost("/Clientes", async (Cliente cliente, ClientesPruebaTecnicaContext ClienteDb) =>
{
    ClienteDb.Clientes.Add(cliente);
    await ClienteDb.SaveChangesAsync();
    return Results.Created($"/Clientes/{cliente.ClienteId}", cliente);
});

app.MapPut("/Clientes/{id}", async (int id, Cliente inputCliente, ClientesPruebaTecnicaContext ClienteDb) =>
{
    var cliente = await ClienteDb.Clientes.FindAsync(id);

    if (cliente == null) return Results.NotFound();

    cliente.Nombre = inputCliente.Nombre;
    cliente.Apellidos = inputCliente.Apellidos;
    cliente.TipoDocumentoId = inputCliente.TipoDocumentoId;
    cliente.NumeroDocumento = inputCliente.NumeroDocumento;
    cliente.Pais = inputCliente.Pais;
    cliente.Departamento = inputCliente.Departamento;
    cliente.NumeroTelefono = inputCliente.NumeroTelefono;
    cliente.CorreoElectronico = inputCliente.CorreoElectronico;
    cliente.FechaNacimiento = inputCliente.FechaNacimiento;


    await ClienteDb.SaveChangesAsync();
    return Results.NoContent();
});



app.MapDelete("/Clientes/{id}", async (int id, ClientesPruebaTecnicaContext ClienteDb) =>
{
    if(await ClienteDb.Clientes.FindAsync(id) is Cliente cliente)
    {
        ClienteDb.Clientes.Remove(cliente);
        await ClienteDb.SaveChangesAsync();
        return Results.Ok(cliente);
    }

    return Results.NotFound();
});

app.UseCors("NuevaPolitica");

app.Run();
