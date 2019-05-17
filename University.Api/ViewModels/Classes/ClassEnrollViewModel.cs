using System;

namespace University.Api.ViewModels.Classes
{
    public class ClassEnrollViewModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
