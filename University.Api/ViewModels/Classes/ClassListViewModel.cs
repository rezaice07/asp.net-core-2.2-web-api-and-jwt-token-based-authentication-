using System.Collections.Generic;
using University.Core.Filters;
using University.Data.Models;

namespace University.Api.ViewModels.Classes
{
    public class ClassListViewModel
    {
        public IEnumerable<Class> Classes { get; set; }
        public ClassSearchFilter SearchFilter { get; set; }
    }
}
