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
    
    public partial class View_research_member
    {
        public int member_id { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string lname { get; set; }
        public int faculty_id { get; set; }
        public string permission { get; set; }
        public string faculty_name { get; set; }
        public int branch_id { get; set; }
        public string branch_name { get; set; }
        public int research_id { get; set; }
        public string research_name { get; set; }
        public int teacher_id { get; set; }
        public bool approve { get; set; }
    }
}
