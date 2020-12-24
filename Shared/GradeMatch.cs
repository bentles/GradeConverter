namespace GradeConverter.Models
{
    public class GradeMatch
    {
        public string FromGradeSystemName { get; set; }
        public string FromGrade { get; set; }
        public string ToGrade { get; set; } //the one converted to
        public string ToGradeSystem { get; internal set; }
    }
}