using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOneReversal
{
    public class Mfb
    {
        public string Address { get; set; }

        public bool? AllowPostPaidBilling { get; set; }

        public string CBNCode { get; set; }

        public string Code { get; set; }

        public int CommercialBank { get; set; }
        public string Contact_Email { get; set; }

        public string Contact_FirstName { get; set; }
        public string Contact_LastName { get; set; }

        public string Contact_Phone { get; set; }
        public string CRMAccountID { get; set; }

        public bool? DisallowPostPaidBilling { get; set; }

        public bool? DoSystemAccessCheck { get; set; }

        public string Email { get; set; }
        public bool? EnableAccountOfficer2FA { get; set; }
        public string Footer { get; set; }

        public long ID { get; set; }

        public string IdentityCode { get; set; }

        public string ImageUrl { get; set; }

        public int? InstitutionCode { get; set; }

        public int? InstitutionType { get; set; }

        public string LocalConnectionString { get; set; }
        public long? Max_Cards { get; set; }

        public long? Max_Users { get; set; }

        public string Name { get; set; }
    

        public long? PaymentServicesBankID { get; set; }
        public string Phone { get; set; }
        public string RemoteConnectionString { get; set; }

        public string ShortName { get; set; }

        public string SMSAcctPassword { get; set; }
        public string SMSAcctUsername { get; set; }

        public int State { get; set; }

        public long? StateID { get; set; }
        public long? Status { get; set; }
        public int? CountryID { get; set; }
    }
}
