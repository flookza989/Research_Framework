//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Research_Framework.ApplicationDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class research
    {
        public int research_id { get; set; }
        public string research_name { get; set; }
        public int teacher_id { get; set; }
        public bool approve { get; set; }
    }
}