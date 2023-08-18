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

//Clientes

app.MapGet("/Clientes", async (ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.Clientes.ToListAsync());

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
    if (await ClienteDb.Clientes.FindAsync(id) is Cliente cliente)
    {
        ClienteDb.Clientes.Remove(cliente);
        await ClienteDb.SaveChangesAsync();
        return Results.Ok(cliente);
    }

    return Results.NotFound();
});

//TipoDocumento 

app.MapGet("/TiposDocumento", async (ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.TiposDocumento.ToListAsync());

app.MapGet("/TiposDocumentoContador", async (ClientesPruebaTecnicaContext ClienteDb) =>
{
    var tiposDocumento = await ClienteDb.TiposDocumento.ToListAsync();

    var documentCounts = await ClienteDb.GetDocumentCountsForTiposDocumentoAsync();

    var response = tiposDocumento.Select(td => new
    {
        TipoDocumento = td,
        DocumentCount = documentCounts.ContainsKey(td.TipoDocumentoNombre) ? documentCounts[td.TipoDocumentoNombre] : 0
    });

    return Results.Ok(response);
});

app.MapGet("/TiposDocumento/{id}", async (int id, ClientesPruebaTecnicaContext ClienteDb) =>
   await ClienteDb.TiposDocumento.FindAsync(id)
       is TiposDocumento documento
           ? Results.Ok(documento)
           : Results.NotFound());

app.MapPost("/TiposDocumento", async (TiposDocumento documento, ClientesPruebaTecnicaContext ClienteDb) =>
{
    ClienteDb.TiposDocumento.Add(documento);
    await ClienteDb.SaveChangesAsync();
    return Results.Created($"/Clientes/{documento.TipoDocumentoId}", documento);
});

app.MapDelete("/TiposDocumento/{id}", async (int id, ClientesPruebaTecnicaContext ClienteDb) =>
{
    if (await ClienteDb.TiposDocumento.FindAsync(id) is TiposDocumento documento)
    {
        ClienteDb.TiposDocumento.Remove(documento);
        await ClienteDb.SaveChangesAsync();
        return Results.Ok(documento);
    }

    return Results.NotFound();
});

app.MapPut("/TiposDocumento/{id}", async (int id, TiposDocumento inputDocumento, ClientesPruebaTecnicaContext ClienteDb) =>
{
    var documento = await ClienteDb.TiposDocumento.FindAsync(id);

    if (documento == null) return Results.NotFound();

    documento.TipoDocumentoNombre = inputDocumento.TipoDocumentoNombre;

    await ClienteDb.SaveChangesAsync();
    return Results.NoContent();
});

//Cliente con TipoDocumento 

app.MapGet("/ClientesDocumentos", async (ClientesPruebaTecnicaContext ClienteDb) =>
    await ClienteDb.GetClientesConTipoDocumento(ClienteDb).ToListAsync());





app.UseCors("NuevaPolitica");

app.Run();
