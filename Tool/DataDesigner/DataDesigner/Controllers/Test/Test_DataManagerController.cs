
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataDesigner.Controllers.Test
{
    using DataDesigner.Core.Data;
    using DataDesigner.Core.Services.Interfaces;
    using DataDesigner.Core.TypeManager;

    /// <summary>
    /// Test_DataMapperController.
    /// </summary>
    public class Test_DataManagerController : Controller
    {
        private readonly ILogger<Test_DataManagerController> _logger;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly ClassManager _classManaer;
        private readonly EnumManager _enumManager;

        public Test_DataManagerController(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<Test_DataManagerController>>();

            _jsonSerializer = serviceProvider.GetService<IJsonSerializer>();
             
            _enumManager = serviceProvider.GetRequiredService<EnumManager>();
            _classManaer = serviceProvider.GetRequiredService<ClassManager>();
        }

        [HttpGet]
        [Route("test/data-manager/class")]
        public string Test_Class()
        {
            // Manage class.
            //_classManaer.UpdateSchemaInfo(new ClassSchemaInfo
            //{
            //    Name = "TB_Dungeon",
            //    Description = "Dungeon Table."
            //});

            var classMembers = new List<ClassMember>();

            // Id.
            classMembers.Add(new ClassMember
            {
                Name = "Id",
                Type = "int",
                Comment = "Primary key",
                IsIndexed = true,
            });

            // GroupId.
            classMembers.Add(new ClassMember
            {
                Name = "GroupId",
                Type = "int",
                Comment = "groud id",
                IsIndexed = true,
            });

            // IsLocked.
            classMembers.Add(new ClassMember
            {
                Name = "IsLocked",
                Type = "bool",
                Comment = "lock status",
                IsIndexed = false,
            });

            // Summary.
            classMembers.Add(new ClassMember
            {
                Name = "Summary",
                Type = "string",
                Comment = "summmary for this table",
                IsIndexed = false,
            });

            var classMemberJson = _jsonSerializer.Serialize(classMembers);

            //_classManaer.U("TB_Dungeon", classMemberJson);

            return "Result ok.";
        }

        [HttpGet]
        [Route("test/data-manager/enum")]
        public string Test_Enum()
        {
            // Manage class.
            //_enumManager.UpdateSchemaInfo(new EnumSchemaColumn
            //{
            //    Name = "Type_Dungeon",
            //    Category = "All",
            //    Description = "Dungeon Type."
            //});

            var enumMembers = new List<EnumMember>();

            // None.
            enumMembers.Add(new EnumMember
            {
                Name = "None",
                Value = 0,
                Comment = "None type",
            });

            // Normal.
            enumMembers.Add(new EnumMember
            {
                Name = "Normal",
                Value = 1,
                Comment = "Instance",
            });

            // Instance.
            enumMembers.Add(new EnumMember
            {
                Name = "Instance",
                Value = 2,
                Comment = "Instance",
            });

            // Raid.
            enumMembers.Add(new EnumMember
            {
                Name = "Raid",
                Value = 3,
                Comment = "Raid",
            });

            _enumManager.UpdateMembers("Type_Dungeon", enumMembers);

            return "Result ok.";
        }
    }
}
