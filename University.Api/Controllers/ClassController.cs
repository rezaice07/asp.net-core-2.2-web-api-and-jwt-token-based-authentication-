using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using University.Api.Infrastructures.Controller;
using University.Api.ViewModels.Classes;
using University.Core.Filters;
using University.Data.Models;
using University.Service.Classes;
using Microsoft.Extensions.Configuration;

namespace University.Api.Controllers
{
    [Route("api/[controller]")]
    public class ClassController : CoreController
    {
        #region Private Methods

        private readonly IMapper _mapper;
        private readonly IClassService _classService;
        private readonly IConfiguration _configuration;

        #endregion

        #region ctor

        public ClassController(IMapper mapper,
            IConfiguration configuration,
            IClassService classService
            )
        {
            _mapper = mapper;
            _classService = classService;
            _configuration = configuration;
        }

        #endregion

        #region List

        //POST api/class/getclassbyfilter 
        [Route("getclassbyfilter")]
        [HttpPost]
        public async Task<IActionResult> GetClassByFilter(ClassSearchFilter filter)
        {
            var classes = await _classService.GetListByFilter(filter);

            var model = new ClassListViewModel
            {
                Classes = classes,
                SearchFilter = filter
            };

            return Ok(new
            {
                model = model
            });
        }

        #endregion

        #region Get Details By Id

        // GET api/class/details/4
        [HttpGet]        
        [Route("details/{classId:int}")]
        public async Task<IActionResult> GetClassDetails(int classId)
        {
            var classDetails = await _classService.GetDetailsById(classId);

            if (classDetails == null)
            {
                var message = "Class not found, Please try another.";
                return Ok(new { IsSuccess = false, Message = message });
            }
            return Ok(classDetails);
        }

        #endregion

        #region Register Class

        [Route("registerclass")]
        [HttpPost]
        public async Task<ActionResult> RegisterClass([FromBody] ClassEnrollViewModel model)
        {
            var currentUser = CurrentLoginUser;

            if (currentUser == null)
                return Ok(new { IsSuccess = false, Message = "There was an error while trying to register the class" });

            //validate available seats
            var isValidClassSeatCapacity = await _classService.ValidateClassSeatCapacity(model.ClassId);

            if (isValidClassSeatCapacity)
                return Ok(new { IsSuccess = false, Message = "You do not have available seat for this class. Please try another!" });

            //validate student class enrollment
            var isValidStudentClassEnrollment = await _classService.ValidateStudentClassEnrollment(model.ClassId, model.UserId);

            if (isValidStudentClassEnrollment)
                return Ok(new { IsSuccess = false, Message = "You already registered this class. Please try another!" });

            var newStudentEnrolledClass = _mapper.Map<StudentEnrolledClass>(model);
            newStudentEnrolledClass.UserId = currentUser.Id;

            //let's register the class
            var isRegistered = await _classService.RegisterClass(newStudentEnrolledClass);

            if (isRegistered)
                return Ok(new { IsSuccess = false, Message = "There was an error while trying to register the class!" });

            return Ok(new { IsSuccess = true, Message = "You have registered successfully" });
        }

        #endregion

        #region Drop Registered Class

        [Route("dropregisteredclass")]
        [HttpGet]
        public async Task<ActionResult> DropRegisteredClass(int studentEnrolledClassId, int classId)
        {
            var currentUser = CurrentLoginUser;

            if (currentUser == null)
                return Ok(new { IsSuccess = false, Message = "There was an error while trying to register the class" });

            var dropStudentEnrolledClass = new StudentEnrolledClass
            {
                ClassId = classId,
                UserId = currentUser.Id,
                Id = studentEnrolledClassId
            };

            //let's drop this registered class
            var isDropedRegisteredClass = await _classService.DropRegisteredClass(dropStudentEnrolledClass);

            if (isDropedRegisteredClass)
                return Ok(new { IsSuccess = false, Message = "There was an error while trying to drop the class!" });

            return Ok(new { IsSuccess = true, Message = "You have drop your registered class successfully" });
        }

        #endregion
    }
}
