using System;
using System.Collections.Generic;
using System.Text;

namespace AllVillainNames
{
    public class Queries
    {
        public const string GetVilliansWithMoreThan3Minions =
            @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
               FROM Villains AS v
               JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
           GROUP BY v.Id, v.Name
             HAVING COUNT(mv.VillainId) > 3 
           ORDER BY COUNT(mv.VillainId)";

        public const string GetVillainForId =
            @"SELECT Name FROM Villains WHERE Id = @Id";

        public const string GetMinionNamesForVillain =
            @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                FROM MinionsVillains AS mv
                JOIN Minions As m ON mv.MinionId = m.Id
               WHERE mv.VillainId = @Id
               ORDER BY m.Name";
    }
}
