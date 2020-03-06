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
                                SELECT TOP(1)
                                   m.MemberID,
                                   m.FirstName,
                                   m.LastName,
                                   d.DiagnosisID,
                                   d.DiagnosisDescription,
                                   dc.CategoryDescription,
                                   dc.CategoryScore,
                                   CASE
                                      WHEN
                                         dc.Diagnosiscategoryid = 1 OR dc.DiagnosisCategoryID IS NULL
                                      THEN
                                         1 
                                      ELSE
                                         0 
                                   END
                                   AS [IsMostSevereCategory] 
                                FROM
                                   dbo.Member m 
                                   INNER JOIN
                                      dbo.Memberdiagnosis md 
                                      ON m.Memberid = md.Memberid 
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
                                   m.MemberID = @memberId
                                ORDER BY
                                   d.DiagnosisID
";

        public MemberDto GetMember(int memberId)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(SELECT_MEMBER, sqlConnection))
            {
                sqlConnection.Open();
                cmd.Parameters.AddWithValue("@memberId", memberId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception("Member not found");

                    var member = new MemberDto
                    {
                        CategoryDescription = reader["CategoryDescription"].ToString(),
                        CategoryScore = Convert.ToInt32(reader["CategoryScore"]),
                        DiagnosisDescription = reader["DiagnosisDescription"].ToString(),
                        DiagnosisId = Convert.ToInt32(reader["DiagnosisID"]),
                        FirstName = reader["FirstName"].ToString(),
                        IsMostSevereCategory = Convert.ToBoolean(reader["IsMostSevereCategory"]),
                        LastName = reader["LastName"].ToString(),
                        MemberId = Convert.ToInt32(reader["MemberID"]),
                    };

                    return member;
                }
            }
        }
    }
}
