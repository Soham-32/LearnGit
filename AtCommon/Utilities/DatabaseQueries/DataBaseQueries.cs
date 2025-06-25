using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AtCommon.DatabaseQueries
{
    public class DataBaseQueries
    {
        //Enterprise Agility Dashboard Widget 1 : Total Active Teams
        public static List<int> ActiveTeams(int companyId, string databaseConnection)
        {
            var teamIds = new List<int>();

            using var conn = new SqlConnection { ConnectionString = databaseConnection };

            conn.Open();

            var command = new SqlCommand
            {
                CommandText = "SELECT DISTINCT tm.TeamId " +
                              "FROM TeamMember tm " +
                              "WHERE tm.DeletedAt IS NULL " +
                              "AND EXISTS ( " +
                              "    SELECT 1 " +
                              "    FROM Team t1 " +
                              "    WHERE t1.CompanyId = @companyId " +
                              "    AND t1.Type = 'Team' " +
                              "    AND t1.DeletedAt IS NULL " +
                              "    AND t1.TeamArchiveStatusId = 1 " +
                              "    AND NOT EXISTS ( " +
                              "        SELECT 1 " +
                              "        FROM TeamTag tt " +
                              "        JOIN CompanyTeamTag ctt ON tt.CompanyTeamTagId = ctt.Id " +
                              "        WHERE tt.TeamId = t1.TeamId " +
                              "        AND ctt.Name = 'Group Of Individuals' " +
                              "    ) " +
                              "    AND t1.TeamId = tm.TeamId " +
                              ");",
                CommandType = CommandType.Text,
                Connection = conn
            };
            command.Parameters.Add("@companyId", SqlDbType.Int).Value = companyId;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    teamIds.Add(reader.GetInt32(0)); // Assuming TeamId is of type int
                }
            }


            return teamIds;
        }
    }
}
