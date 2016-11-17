//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PokerViewer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class hand_action
    {
        public long HandID { get; set; }
        public int ActionID { get; set; }
        public string PlayerName { get; set; }
        public string ActionName { get; set; }
        public string Street { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsPFR { get; set; }
        public Nullable<bool> IsVPIP { get; set; }
        public Nullable<bool> Is3Bet { get; set; }
        public Nullable<bool> Is4Bet { get; set; }
    
        public virtual hand hand { get; set; }
        public virtual player player { get; set; }
    }
}
