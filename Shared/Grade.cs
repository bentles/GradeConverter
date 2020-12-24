using System.Text.Json.Serialization;

namespace GradeConverter.Shared
{
    public class Grade
    {
        public string label { get; set; }
        public int[] score { get; set; }


        // lol just gonna assume these wont fail
        public int min => score == null ? 0 : score[1];

        public int max => score == null ? 0 : score[2];

        public int mid => score == null ? 0 : score[0];
    }
}