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
                await AddMinion(connection);
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

        //02. Minion Names
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

        private static async Task AddMinion(SqlConnection connection)
        {
            //Minion: Bob 14 Berlin
            //Villain: Gru
            string[] minionData = Console.ReadLine()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string villainName = Console.ReadLine()
                .Substring(9);

            string minionName = minionData[1];
            int minionAge = int.Parse(minionData[2]);
            string minionTown = minionData[3];

            SqlCommand getTownByName = new SqlCommand(Queries.GetTownFromTownName, connection);
            getTownByName.Parameters.AddWithValue("@townName", minionTown);
            object townId = await getTownByName.ExecuteScalarAsync();

            if (townId == null)
            {
                SqlCommand createTown = new SqlCommand(Queries.CreateTown, connection);
                createTown.Parameters.AddWithValue("@townName", minionTown);
                await createTown.ExecuteNonQueryAsync();

                Console.WriteLine($"Town {minionTown} was added to the database.");
                townId = await getTownByName.ExecuteScalarAsync();
            }


            SqlCommand createMinion = new SqlCommand(Queries.CreateMinion, connection);
            createMinion.Parameters.AddWithValue("@nam", minionName);
            createMinion.Parameters.AddWithValue("@age", minionAge);
            createMinion.Parameters.AddWithValue("@townId", townId);
            await createMinion.ExecuteNonQueryAsync();

            SqlCommand getMinionId = new SqlCommand(Queries.GetMinionIdFromName, connection);
            getMinionId.Parameters.AddWithValue("@Name", minionName);
            object minionId = await getMinionId.ExecuteScalarAsync();

            SqlCommand getVillain = new SqlCommand(Queries.GetVillianIdFromName, connection);
            getVillain.Parameters.AddWithValue("@Name", villainName);
            object villain = await getVillain.ExecuteScalarAsync();

            if (villain == null)
            {
                SqlCommand createVillain = new SqlCommand(Queries.CreateVillain, connection);
                createVillain.Parameters.AddWithValue("@villainName", villainName);
                await createVillain.ExecuteNonQueryAsync();

                Console.WriteLine($"Villain {villainName} was added to the database.");
                villain = await getVillain.ExecuteScalarAsync();
            }

            SqlCommand mapMinion = new SqlCommand(Queries.MapMinionToVillain, connection);
            mapMinion.Parameters.AddWithValue("@minionId", minionId);
            mapMinion.Parameters.AddWithValue("@villainId", villain);

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }
    }
}
