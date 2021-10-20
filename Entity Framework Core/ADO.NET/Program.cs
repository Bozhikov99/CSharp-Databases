using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace AllVillainNames
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(Configuration.ConnectionString);
            await connection.OpenAsync();

            await using (connection)
            {
                await MinionNames(connection, 1);
            }
        }

        //01. Villain Names
        private static async Task VillainNamesAsync(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand(Queries.GetVilliansWithMoreThan3Minions, connection);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            await using (reader)
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
                }
            }
        }

        private static async Task MinionNames(SqlConnection connection, int Id)
        {
            SqlCommand getVillainFromId = new SqlCommand(Queries.GetVillainForId, connection);
            getVillainFromId.Parameters.AddWithValue("Id", Id);

            object villain = await getVillainFromId.ExecuteScalarAsync();

            if (villain == null)
            {
                Console.WriteLine($"No villain with ID {Id} exists in the database.");
                return;
            }

            Console.WriteLine($"Villain name: {villain}");

            SqlCommand getMinionsForVillain = new SqlCommand(Queries.GetMinionNamesForVillain, connection);
            getMinionsForVillain.Parameters.AddWithValue("Id", Id);

            SqlDataReader reader = await getMinionsForVillain.ExecuteReaderAsync();

            await using (reader)
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["RowNum"]}. {reader["Name"]} {reader["Age"]}");
                }
            }
        }
    }
}
