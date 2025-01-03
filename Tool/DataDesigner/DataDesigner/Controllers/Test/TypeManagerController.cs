
using Microsoft.AspNetCore.Mvc;

namespace DataDesigner.Controllers.Test
{
    using DataDesigner.Core.Data;
    using DataDesigner.Core.Services.Interfaces;
    using DataDesigner.Core.TypeManager;

    /// <summary>
    /// Test_DataMapperController.
    /// </summary>
    public class TypeManagerController : Controller
    {
        private readonly ILogger<TypeManagerController> _logger;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly ClassManager _classManaer;
        private readonly EnumManager _enumManager;

        public TypeManagerController(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<TypeManagerController>>();

            _jsonSerializer = serviceProvider.GetService<IJsonSerializer>();
             
            _enumManager = serviceProvider.GetRequiredService<EnumManager>();
            _classManaer = serviceProvider.GetRequiredService<ClassManager>();
        }

        [HttpGet]
        [Route("test/data-manager/class/add-column")]
        public string ClassManager_AddColumn()
        {


            return "ClassManager_AddColumn ok.";
        }

        [HttpGet]
        [Route("test/data-manager/class/delete-column")]
        public string ClassManager_DeleteColumn()
        {


            return "ClassManager_DeleteColumn ok.";
        }

        [HttpGet]
        [Route("test/data-manager/class/update-column")]
        public string ClassManager_UpdateColumn()
        {


            return "ClassManager_UpdateColumn ok.";
        }

        [HttpGet]
        [Route("test/data-manager/class/add-row")]
        public string ClassManager_AddRow()
        {


            return "ClassManager_AddRow ok.";
        }

        [HttpGet]
        [Route("test/data-manager/class/delete-row")]
        public string ClassManager_DeleteRow()
        {


            return "ClassManager_DeleteRow ok.";
        }

        [HttpGet]
        [Route("test/data-manager/class/update-row")]
        public string ClassManager_UpdateRow()
        {


            return "ClassManager_UpdateRow ok.";
        }
    }
}
