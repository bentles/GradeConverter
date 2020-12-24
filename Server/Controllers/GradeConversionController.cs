using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Linq;
using GradeConverter.Shared;

namespace GradeConverter.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradeConversionController : ControllerBase
    {
        private const string Sport = "Free";
        private const string Boulder = "Boulder";
        static GradeSystem[] gradeSystems;

        static GradeConversionController()
        {
            var text = File.ReadAllText("Assets/grades.json");
            gradeSystems = JsonSerializer.Deserialize<GradeSystem[]>(text);
        }

        [HttpGet("[action]")]
        public GradeMatch[] GetSouthAfricanRouteGrade(string grade)
        {
            return ConvertGradeToSystem(grade, "South African", Sport);
        }

        [HttpGet("[action]")]
        public GradeMatch[] GetSouthAfricanBoulderGrade(string grade)
        {
            return ConvertGradeToSystem(grade, "Fontainebleau", Boulder);
        }

        private GradeMatch[] ConvertGradeToSystem(string grade, string systemName, string systemType)
        {
            var matchingGrades = (from gradeSystem in MatchingGradeSystems(grade, systemType)
                                  from gradeName in gradeSystem.grade
                                  where string.Equals(gradeName.label, grade, System.StringComparison.InvariantCultureIgnoreCase)
                                  select new { SystemName = gradeSystem.abbreviatedName, GradeName = gradeName.label, Value = gradeName.mid });

            var SAGrades = gradeSystems.First(g => g.abbreviatedName == systemName).grade;

            return (from gradeValues in matchingGrades
                    let SAGrade = SAGrades.FirstOrDefault(sa => gradeValues.Value >= sa.min && gradeValues.Value <= sa.max)
                    select new GradeMatch()
                    {
                        FromGrade = gradeValues.GradeName,
                        FromGradeSystemName = gradeValues.SystemName,
                        ToGrade = SAGrade?.label ?? "Could not find conversion :(",
                        ToGradeSystem = systemName
                    }).ToArray();
        }

        [HttpGet("[action]")]
        public string[] GetMatchingRouteGradeSystems(string grade)
        {
            return (from gradeSystem in MatchingGradeSystems(grade, Sport)
                    select gradeSystem.abbreviatedName).ToArray();
        }

        [HttpGet("[action]")]
        public string[] GetMatchingBoulderGradeSystems(string grade)
        {
            return (from gradeSystem in MatchingGradeSystems(grade, Boulder)
                    select gradeSystem.abbreviatedName).ToArray();
        }

        private GradeSystem[] MatchingGradeSystems(string grade, string type)
        {
            return (from gradeSystem in gradeSystems
                    where gradeSystem.type == type &&
                    gradeSystem.IsGradeLegal(grade)
                    select gradeSystem).ToArray();
        }


        [HttpGet]
        public string Get()
        {
            return SanityCheck();
        }


        private string SanityCheck()
        {
            var gradeSanities = (from gradeSystem in gradeSystems
                                 from grade in gradeSystem.grade
                                 select new
                                 {
                                     Sane = grade.mid == grade.max - ((grade.max - grade.min) / 2),
                                     System = gradeSystem.abbreviatedName,
                                     Grade = grade.label
                                 }).ToList();

            return (!gradeSanities.Any(g => !g.Sane)).ToString();
        }
    }
}