using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Mi_Diario_de_Bienestar.Models;
using Microsoft.Maui.Storage;

namespace Mi_Diario_de_Bienestar.Data
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService(string? databaseFileName = null)
        {
            // Ubicación de la base de datos
            var fileName = string.IsNullOrWhiteSpace(databaseFileName)
                ? "bienestar.db"
                : databaseFileName;

            _dbPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        }

        /// Crea la BD y la tabla si no existen
        public async Task InitializeAsync()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);

            await Task.Run(() =>
            {
                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Registros (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Fecha TEXT NOT NULL,
                        Descripcion TEXT NOT NULL,
                        ActividadFisica INTEGER NOT NULL,
                        Energia INTEGER NOT NULL
                    );";
                cmd.ExecuteNonQuery();

                conn.Close();
            });
        }

        /// Devuelve todos los registros ordenados por fecha descendente
        public async Task<List<RegistroDiario>> GetAllAsync()
        {
            var list = new List<RegistroDiario>();

            await Task.Run(() =>
            {
                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Id, Fecha, Descripcion, ActividadFisica, Energia
                    FROM Registros
                    ORDER BY Fecha DESC, Id DESC;";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new RegistroDiario
                    {
                        Id = reader.GetInt32(0),
                        Fecha = DateTime.Parse(reader.GetString(1)),
                        Descripcion = reader.GetString(2),
                        ActividadFisica = reader.GetInt32(3),
                        Energia = reader.GetInt32(4)
                    });
                }

                conn.Close();
            });

            return list;
        }

        /// Inserta un registro y devuelve su ID
        public async Task<int> InsertAsync(RegistroDiario r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));

            return await Task.Run(() =>
            {
                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Registros (Fecha, Descripcion, ActividadFisica, Energia)
                    VALUES (@fecha, @desc, @actividad, @energia);
                    SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@fecha", r.Fecha.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@desc", r.Descripcion);
                cmd.Parameters.AddWithValue("@actividad", r.ActividadFisica);
                cmd.Parameters.AddWithValue("@energia", r.Energia);

                var result = cmd.ExecuteScalar();
                conn.Close();

                return Convert.ToInt32(result);
            });
        }

        /// Elimina un registro por ID
        public async Task<bool> DeleteAsync(int id)
        {
            return await Task.Run(() =>
            {
                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Registros WHERE Id = @id;";
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();
                conn.Close();

                return rows > 0;
            });
        }

        /// Calcula promedio de actividad física y energía de los últimos 7 días
        public async Task<(double avgActividad, double avgEnergia)?> GetStatisticsLast7DaysAsync()
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            string desde = DateTime.Today.AddDays(-6).ToString("yyyy-MM-dd");

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
        SELECT AVG(ActividadFisica), AVG(Energia)
        FROM Registros
        WHERE Fecha >= @desde;";
            cmd.Parameters.AddWithValue("@desde", desde);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync() &&
                !reader.IsDBNull(0) &&
                !reader.IsDBNull(1))
            {
                double act = reader.GetDouble(0);
                double ene = reader.GetDouble(1);

                return (act, ene);
            }

            return null;
        }
    }
}
