using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mono.Data.Sqlite;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace EDBR.DB
{
    public static class Factions
    {
        const string path = "URI=file:Assets/Resources/Databases/factions.db";

        public static string[] FindPartialMatches(string input)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            SqliteConnection conn = new SqliteConnection(path);
            conn.Open();

            List<string> results = new List<string>();

            string query = $"SELECT * FROM factions WHERE name LIKE @input LIMIT 12";

            using (var command = new SqliteCommand(query, conn))
            {
                command.Parameters.AddWithValue("@input", "%" + input + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(reader.GetString(reader.GetOrdinal("name")));
                    }
                }
            }

            conn.Close();
            stopwatch.Stop();
            Debug.Log($"FINISHED FindPartialMatches({input}) IN {stopwatch.ElapsedMilliseconds} milliseconds");
            return results.ToArray();
        }
    }
}