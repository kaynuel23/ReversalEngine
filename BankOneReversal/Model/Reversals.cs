using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOne.ReversalEngine.Model
{
    public class Reversals : Entity
    {
        public virtual string MFBCode { get; set; }
        public virtual string UniqueIdentifier { get; set; }
        public virtual ReversalStatus ReversalStatus { get; set; }
        public virtual string ErrorMessage { get; set; }
        public virtual int RetryCount { get; set; }
    }
}

namespace BankOne.ReversalEngine.Model
{
    public enum ReversalStatus
    {
        Pending,
        Processing,
        Successful,
        Failed
    }
}