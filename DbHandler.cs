using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse8
{
    public class DbHandler
    {
        private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["Pulse8ConnStr"].ConnectionString;

        private const string SELECT_MEMBER = @"
                        ;WITH MemberCategory_CTE (MemberID, DiagnosisID, DiagnosisDescription, CategoryDescription, CategoryScore) AS 
                        (
                           SELECT
                              md.MemberID,
                              d.DiagnosisID,
                              d.DiagnosisDescription,
                              dc.CategoryDescription,
                              dc.CategoryScore 
                           FROM
                              MemberDiagnosis md 
                              INNER JOIN
                                 dbo.Diagnosis d 
                                 ON md.Diagnosisid = d.Diagnosisid 
                              INNER JOIN
                                 dbo.Diagnosiscategorymap dcm 
                                 ON d.Diagnosisid = dcm.Diagnosisid 
                              INNER JOIN
                                 dbo.Diagnosiscategory dc 
                                 ON dcm.Diagnosiscategoryid = dc.Diagnosiscategoryid 
                           WHERE
                              md.MemberID = @memberId 
                        )
                        SELECT
                           m.MemberID,
                           m.FirstName,
                           m.LastName,
                           temp.DiagnosisID,
                           temp.DiagnosisDescription,
                           temp.CategoryDescription,
                           temp.CategoryScore,
                           CASE
                              WHEN
                                 temp.CategoryScore = 
                                 (
                                    SELECT
                                       MIN(MemberCategory_CTE.CategoryScore) 
                                    FROM
                                       MemberCategory_CTE 
                                 )
                              THEN
                                 1 
                              ELSE
                                 0 
                           END
                           AS [IsMostSevereCategory] 
                        FROM
                           dbo.Member m 
                           INNER JOIN
                              MemberCategory_CTE temp 
                              ON m.MemberID = temp.MemberID
";

        public List<MemberDto> GetMember(int memberId)
        {
            var memberCategories = new List<MemberDto>();

            using (var sqlConnection = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(SELECT_MEMBER, sqlConnection))
            {
                sqlConnection.Open();
                cmd.Parameters.AddWithValue("@memberId", memberId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        memberCategories.Add(new MemberDto
                        {
                            CategoryDescription = reader["CategoryDescription"].ToString(),
                            CategoryScore = Convert.ToInt32(reader["CategoryScore"]),
                            DiagnosisDescription = reader["DiagnosisDescription"].ToString(),
                            DiagnosisId = Convert.ToInt32(reader["DiagnosisID"]),
                            FirstName = reader["FirstName"].ToString(),
                            IsMostSevereCategory = Convert.ToBoolean(reader["IsMostSevereCategory"]),
                            LastName = reader["LastName"].ToString(),
                            MemberId = Convert.ToInt32(reader["MemberID"]),
                        });
                    }
                    if (memberCategories.Count == 0)
                        throw new Exception("Member not found");
                }
            }
            return memberCategories;
        }
    }
}
