using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse8
{
    public class MemberDto
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DiagnosisId { get; set; }
        public string DiagnosisDescription { get; set; }
        public string CategoryDescription { get; set; }
        public int CategoryScore { get; set; }
        public bool IsMostSevereCategory { get; set; }
    }
}
