using LemmikkirekisteriAPI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

LemmikkirekisteriDB lemmikkirekisteriDB = new LemmikkirekisteriDB();


app.MapPost("/omistajat", (Omistaja omistaja) =>
{
    lemmikkirekisteriDB.LisaaOmistaja(omistaja.Id, omistaja.Nimi, omistaja.Puhelinnumero);
    return Results.Ok(omistaja);

});

app.MapPost("/lemmikit", (Lemmikki lemmikki) =>
{
   lemmikkirekisteriDB.LisaaLemmikki(lemmikki.Id, lemmikki.OmistajanNimi, lemmikki.Nimi, lemmikki.Laji);
    return Results.Ok(lemmikki);
});

app.MapPut("/omistajat", (Omistaja omistaja) =>
{
    lemmikkirekisteriDB.PaivitaPuhelinnumero(omistaja.Nimi, omistaja.Puhelinnumero);
    return Results.Ok(omistaja);
});

app.MapGet("/omistajat", ([FromBody] Lemmikki lemmikki) =>
{
    return lemmikkirekisteriDB.EtsiPuhelinnumero(lemmikki.Nimi);
    
});


app.Run();
