using DataDesigner.Core.Data;
using DataDesigner.Core.Generator;
using Microsoft.AspNetCore.Mvc;

namespace DataDesigner.Controllers.Test
{
    /// <summary>
    /// GeneratorController.
    /// </summary>
    public class GeneratorController : Controller
    {
        private readonly ILogger<GeneratorController> _logger;

        private readonly CodeGenerator _codeGenerator;

        public GeneratorController(IServiceProvider serviceProvider) 
        {
            _logger = serviceProvider.GetRequiredService<ILogger<GeneratorController>>();

            _codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
        }

        [HttpGet]
        [Route("test/generate-enum")]
        public string GenerateEnum()
        {
            ////////////////////////////////////////
            /// Player types.
            var playerTypes = new List<EnumMember>();

            playerTypes.Add(new EnumMember
            {
                Name = "None",
                Value = 0,
                Comment = "None player"
            });

            playerTypes.Add(new EnumMember
            {
                Name = "Normal",
                Value = 1,
                Comment = "Normal player"
            });

            playerTypes.Add(new EnumMember
            {
                Name = "Paid",
                Value = 2,
                Comment = "Paid player"
            });

            playerTypes.Add(new EnumMember
            {
                Name = "Platinum",
                Value = 3,
                Comment = "Platinum player"
            });

            // Add enums.
            _codeGenerator.AddEnum("TestCode", "Type_Player", playerTypes);

            //////////////////////////////////////////////////////////
            /// Item types.
            var itemTypes = new List<EnumMember>();

            itemTypes.Add(new EnumMember
            {
                Name = "None",
                Value = 0,
                Comment = "None"
            });

            itemTypes.Add(new EnumMember
            {
                Name = "Waepon",
                Value = 1,
                Comment = "Waepon"
            });

            itemTypes.Add(new EnumMember
            {
                Name = "Consume",
                Value = 2,
                Comment = "Consume"
            });

            itemTypes.Add(new EnumMember
            {
                Name = "Material",
                Value = 3,
                Comment = "Material"
            });

            _codeGenerator.AddEnum("TestCode", "Type_Item", itemTypes);

            return "GenerateEnum OK.";
        }

        [HttpGet]
        [Route("test/generate-class")]
        public string GenerateClass()
        {
            /// Item data members.
            var itemMembers = new List<ClassMember>();

            // int.
            itemMembers.Add(new ClassMember
            {
                Name = "Id",
                Type = "int",
                Comment = "RefId"
            });

            // string.
            itemMembers.Add(new ClassMember
            {
                Name = "NameKey",
                Type = "string",
                Comment = "Name key"
            });

            // double.
            itemMembers.Add(new ClassMember
            {
                Name = "Gold",
                Type = "double",
                Comment = "Gold"
            });

            // bool.
            itemMembers.Add(new ClassMember
            {
                Name = "IsLock",
                Type = "bool",
                Comment = "Lock status"
            });

            // Type_Item.
            itemMembers.Add(new ClassMember
            {
                Name = "ItemType",
                Type = "Type_Item",
                Comment = "Type_Item"
            });

            _codeGenerator.AddClass("TestCode", "TB_Item", itemMembers);

            _codeGenerator.CreateFiles(AppDomain.CurrentDomain.BaseDirectory);

            return "GenerateClass OK.";
        }
    }
}
