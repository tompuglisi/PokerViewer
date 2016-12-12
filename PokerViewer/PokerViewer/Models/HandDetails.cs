using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerViewer.Models
{
    public class HandDetails
    {
        public hand Hand { get; set; }
        public List<hand_action> ActionList { get; set; }
    }
}