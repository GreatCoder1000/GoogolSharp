/*
 *  Copyright 2025 @GreatCoder1000
 *  This file is part of GoogolSharp.
 *
 *  GoogolSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  GoogolSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with GoogolSharp.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace GoogolSharp.Database
{
    /// <summary>
    /// Provides read‑only access to the GoogolSharp SQLite database hosted on GitHub Pages.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class automatically downloads a cached local copy of the database from:
    /// https://greatcoder1000.github.io/googoldb.sqlite
    /// </para>
    /// <para>
    /// The database is stored under the user's LocalApplicationData folder and opened
    /// in read‑only mode for safe multi‑access usage. Since SQLite cannot operate directly
    /// over HTTP/HTTPS, the file is downloaded once and reused locally.
    /// </para>
    /// <para>
    /// Implements <see cref="IGoogolDB"/> and exposes high‑level lookup methods for
    /// number names, definitions, and inventors as defined in the GoogolSharp schema.
    /// </para>
    /// </remarks>
    public class CloudGoogolDB : IGoogolDB
    {
        private const string DbUrl = "https://greatcoder1000.github.io/googoldb.sqlite";
        private readonly string _localPath;

        public CloudGoogolDB()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GoogolSharp");

            Directory.CreateDirectory(folder);

            _localPath = Path.Combine(folder, "googoldb.sqlite");

            EnsureDatabaseDownloaded().Wait();
        }

        private async Task EnsureDatabaseDownloaded()
        {
            if (File.Exists(_localPath))
                return;

            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(DbUrl);
            await File.WriteAllBytesAsync(_localPath, bytes);
        }

        private SqliteConnection Open()
        {
            var conn = new SqliteConnection($"Data Source={_localPath};Mode=ReadOnly;");
            conn.Open();
            return conn;
        }

        // ---------------------------------------------------------
        //  IMPLEMENTATION OF IGoogolDB
        // ---------------------------------------------------------

        /// <summary>
        /// Returns the inventor of a number given both its name and its definition.
        /// </summary>
        /// <param name="name">The textual name of the number (from NumberNames.name).</param>
        /// <param name="definition">The textual definition of the number (from Definitions.name).</param>
        /// <returns>
        /// The inventor's name if found; otherwise an empty string.
        /// </returns>
        public string InventorOfNumberWithNameAndDefinition(string name, string definition)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT g.name
            FROM Numbers n
            JOIN NumberNames nn ON n.name_id = nn.id
            JOIN Definitions d ON n.definition = d.id
            LEFT JOIN Googologists g ON n.inventor = g.id
            WHERE nn.name = $name AND d.name = $def
            LIMIT 1;
        ";

            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$def", definition);

            return cmd.ExecuteScalar() as string ?? "";
        }

        /// <summary>
        /// Returns all number names invented by the specified googologist.
        /// </summary>
        /// <param name="inventor">The name of the googologist (from Googologists.name).</param>
        /// <returns>
        /// An array of number names invented by the given person.
        /// Returns an empty array if none exist.
        /// </returns>
        public string[] NumberNamesFromInventor(string inventor)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT DISTINCT nn.name
            FROM Numbers n
            JOIN NumberNames nn ON n.name_id = nn.id
            JOIN Googologists g ON n.inventor = g.id
            WHERE g.name = $inventor;
        ";

            cmd.Parameters.AddWithValue("$inventor", inventor);

            var list = new System.Collections.Generic.List<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(reader.GetString(0));

            return [.. list];
        }

        /// <summary>
        /// Returns all number definitions invented by the specified googologist.
        /// </summary>
        /// <param name="inventor">The name of the googologist (from Googologists.name).</param>
        /// <returns>
        /// An array of definitions associated with numbers invented by the given person.
        /// Returns an empty array if none exist.
        /// </returns>
        public string[] NumberDefinitionsFromInventor(string inventor)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT DISTINCT d.name
            FROM Numbers n
            JOIN Definitions d ON n.definition = d.id
            JOIN Googologists g ON n.inventor = g.id
            WHERE g.name = $inventor;
        ";

            cmd.Parameters.AddWithValue("$inventor", inventor);

            var list = new System.Collections.Generic.List<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(reader.GetString(0));

            return [.. list];
        }

        /// <summary>
        /// Returns all definitions associated with a given number name.
        /// </summary>
        /// <param name="name">The textual name of the number (from NumberNames.name).</param>
        /// <returns>
        /// An array of definitions for the specified number name.
        /// Returns an empty array if the name is not found or has no definitions.
        /// </returns>
        public string[] NumberDefinitionsFromNumberName(string name)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT DISTINCT d.name
            FROM Numbers n
            JOIN NumberNames nn ON n.name_id = nn.id
            JOIN Definitions d ON n.definition = d.id
            WHERE nn.name = $name;
        ";

            cmd.Parameters.AddWithValue("$name", name);

            var list = new System.Collections.Generic.List<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(reader.GetString(0));

            return [.. list];
        }
    }
}