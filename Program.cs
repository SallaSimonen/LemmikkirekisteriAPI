using LemmikkirekisteriAPI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.WebHost.UseUrls("http://0.0.0.0:80");

LemmikkirekisteriDB lemmikkirekisteriDB = new LemmikkirekisteriDB();


app.MapPost("/omistajat", (Omistaja omistaja) =>
{
    lemmikkirekisteriDB.LisaaOmistaja(omistaja.Nimi, omistaja.Puhelinnumero);
    return Results.Ok(omistaja);

});

app.MapPost("/lemmikit", (Lemmikki lemmikki) =>
{
   lemmikkirekisteriDB.LisaaLemmikki(lemmikki.OmistajanNimi, lemmikki.Nimi, lemmikki.Laji);
    return Results.Ok(lemmikki);
});

app.MapPut("/omistajat", (Omistaja omistaja) =>
{
    lemmikkirekisteriDB.PaivitaPuhelinnumero(omistaja.Nimi, omistaja.Puhelinnumero);
    return Results.Ok(omistaja);
});

app.MapPost("/puhelin", (Lemmikki lemmikki) =>
{
    return lemmikkirekisteriDB.EtsiPuhelinnumero(lemmikki.Nimi);

});

app.Run();
