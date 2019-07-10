using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels
{
    public class TrainingProgramEditViewModel
    {
        public TrainingProgram trainingProgram { get; set; }
        //this selected Employee list will hold all of the Ids of the Employee that were selected.
        public List<int> SelectedEmployee { get; set; } = new List<int>();

        //a list of all available employee
        public List<Employee> AvailableEmployee { get; set; }

        public List<SelectListItem> AvailableEmployeeSelectList
        {
            get
            {
                if (AvailableEmployee == null)
                {
                    return null;
                }
                return AvailableEmployee
                       .Select(e => new SelectListItem(e.FirstName, e.Id.ToString()))
                       .ToList();
            }

        }
    }
}
