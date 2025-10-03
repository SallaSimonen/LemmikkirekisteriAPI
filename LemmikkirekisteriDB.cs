namespace LemmikkirekisteriAPI;

using System.ComponentModel;
using System.Transactions;
using Microsoft.Data.Sqlite;


public class LemmikkirekisteriDB
{
    private string _connectionString = "Data Source = lemmikkirekisteri.db";

    public LemmikkirekisteriDB()
    {
        // Luodaan yhteys tietokantaan.
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            // Luodaan taulu Omistajat sarakkeet id, nimi, puhelinnumero.
            var command1 = connection.CreateCommand();
            command1.CommandText = @"CREATE TABLE IF NOT EXISTS Omistajat (
                id INTEGER PRIMARY KEY,
                nimi TEXT,
                puhelinnumero VARCHAR(20)
            )";

            command1.ExecuteNonQuery();

            // Luodaan taulu Lemmikit sarakkeet id, omistaja_id, nimi, laji.
            var command2 = connection.CreateCommand();
            command2.CommandText = @"CREATE TABLE IF NOT EXISTS Lemmikit (
                id INTEGER PRIMARY KEY,
                omistaja_id INTEGER,
                nimi TEXT, 
                laji TEXT,
                FOREIGN KEY (omistaja_id) REFERENCES Omistajat(id)
            )";

            command2.ExecuteNonQuery();
        }
    }

    public void LisaaOmistaja(int id, string nimi, string puh)
    {
        // Luodaan yhteys tietokantaan.
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            // Lisätään omistaja tietokantaan.
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Omistajat (id, nimi, puhelinnumero) VALUES (@Id, @Nimi, @Puh)";
            command.Parameters.AddWithValue("Id", id);
            command.Parameters.AddWithValue("Nimi", nimi);
            command.Parameters.AddWithValue("Puh", puh);
            command.ExecuteNonQuery();
        }
    }

    public void LisaaLemmikki(int id, string omistajanNimi, string lemmikinNimi, string laji)
    {
        var nimiLower = omistajanNimi.ToLower();

        // Luodaan yhteys tietokantaan.
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                // Haetaan tietokannasta omistajan id nimen perusteella.
                var command1 = connection.CreateCommand();
                command1.CommandText = "SELECT id FROM Omistajat WHERE LOWER(nimi) = @Nimi";
                command1.Parameters.AddWithValue("Nimi", nimiLower);
                object? omistajaId = command1.ExecuteScalar();



                // Lisätään lemmikki tietokantaan.
                var command2 = connection.CreateCommand();
                command2.CommandText = "INSERT INTO Lemmikit (id, omistaja_id, nimi, laji) VALUES (@Id, @OmistajaId, @Nimi, @Laji)";
                command2.Parameters.AddWithValue("Id", id);
                command2.Parameters.AddWithValue("OmistajaId", omistajaId);
                command2.Parameters.AddWithValue("Nimi", lemmikinNimi);
                command2.Parameters.AddWithValue("Laji", laji);
                command2.ExecuteNonQuery();

                transaction.Commit();
            }

        }
    }

    public void PaivitaPuhelinnumero(string nimi, string puh)
    {
        string lowerNimi = nimi.ToLower();
        // Luodaan yhteys tietokantaan.
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();


            var command2 = connection.CreateCommand();
            command2.CommandText = "UPDATE Omistajat SET puhelinnumero = @Puh WHERE LOWER(nimi) = @Nimi";
            command2.Parameters.AddWithValue("Puh", puh);
            command2.Parameters.AddWithValue("Nimi", lowerNimi);
            command2.ExecuteNonQuery();
        }

    }

    public string EtsiPuhelinnumero(string nimi)
    {
        string nimiLower = nimi.ToLower();
        // Luodaan yhteys tietokantaan.
        using (var connection = new SqliteConnection(_connectionString))
        {


            connection.Open();

            // Lisätään omistaja tietokantaan.
            var command1 = connection.CreateCommand();
            command1.CommandText = "SELECT puhelinnumero FROM Omistajat WHERE id = (SELECT omistaja_id FROM Lemmikit WHERE LOWER(nimi) = @Nimi)";
            command1.Parameters.AddWithValue("Nimi", nimiLower);
            object? puh = command1.ExecuteScalar();

            nimi = nimi[0].ToString().ToUpper() + nimi.Substring(1);

            return $"Lemmikin {nimi} omistajan puhelinnumero on: {puh}";

        }
    }
}