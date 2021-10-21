using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

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
                await GetMinions(connection);
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
        //03. Add Minion
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

        private static async Task ChangeTownsNameCasing(SqlConnection connection)
        {
            string country = Console.ReadLine();

            SqlCommand changeTownsCasing = new SqlCommand(Queries.SetTownsToUpper, connection);
            changeTownsCasing.Parameters.AddWithValue("@countryName", country);
            int rowsAffected = await changeTownsCasing.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                Console.WriteLine("No town names were affected.");
                return;
            }

            Console.WriteLine($"{rowsAffected} town names were affected.");

            SqlCommand getTowns = new SqlCommand(Queries.GetAffectedTowns, connection);
            getTowns.Parameters.AddWithValue("@countryName", country);
            SqlDataReader reader = await getTowns.ExecuteReaderAsync();

            await using (reader)
            {
                List<string> townNames = new List<string>();

                while (reader.Read())
                {
                    townNames.Add((string)reader["Name"]);
                }

                Console.WriteLine($"[{string.Join(", ", townNames)}]");
            }

        }
        //05. Remove villain
        private static async Task DeleteVillain(SqlConnection connection)
        {
            int villainId = int.Parse(Console.ReadLine());

            SqlCommand getVillainNameFromId = new SqlCommand(Queries.GetVillainNameFromId, connection);
            getVillainNameFromId.Parameters.AddWithValue("@villainId", villainId);

            object villainName = await getVillainNameFromId.ExecuteScalarAsync();

            if (villainName==null)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            SqlCommand getMinionCount = new SqlCommand(Queries.GetReleasedMinionsCount, connection);
            getMinionCount.Parameters.AddWithValue("villainId", villainId);
            object minionCount = await getMinionCount.ExecuteScalarAsync();

            SqlCommand removeMapping = new SqlCommand(Queries.RemoveVillainMinionMapping, connection);
            removeMapping.Parameters.AddWithValue("@villainId", villainId);
            await removeMapping.ExecuteNonQueryAsync();

            SqlCommand deleteVillain = new SqlCommand(Queries.DeleteVillainWithId, connection);
            deleteVillain.Parameters.AddWithValue("villainId", villainId);
            await deleteVillain.ExecuteNonQueryAsync();

            Console.WriteLine($"{villainName} was deleted.");
            Console.WriteLine($"{minionCount} minions were released.");
        }
        //6. Print All Minion Names
        private static async Task GetMinions(SqlConnection connection)
        {
            SqlCommand getMinions = new SqlCommand(Queries.GetAllMinions, connection);

            SqlDataReader reader = await getMinions.ExecuteReaderAsync();
            List<string> minionNames = new List<string>();

            using (reader)
            {
                while (reader.Read())
                {
                    minionNames.Add((string)reader["Name"]);
                }
            }

            int count = 0;

            for (int i = 0; i < minionNames.Count; i++)
            {
                Console.WriteLine($"{minionNames[i]}");
                count++;

                if (count>=minionNames.Count)
                {
                    break;
                }

                Console.WriteLine($"{minionNames[minionNames.Count-i-1]}");
                count++;
            }
        }
    }
}
