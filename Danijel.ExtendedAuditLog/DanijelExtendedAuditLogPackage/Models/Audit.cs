using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danijel.ExtendedAuditLog.DanijelExtendedAuditLogPackage.Models
{
    public class Audit
    {
        public string Message { get; set; }
        public string UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }

    }
}
