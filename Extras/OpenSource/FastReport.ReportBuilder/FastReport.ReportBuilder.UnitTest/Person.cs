using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastReport.ReportBuilder.UnitTest
{
    public class Person
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }

        [DisplayFormat(DataFormatString = "P")]
        public int Level { get; set; }

        public List<Person> GetData()
        {
            return new List<Person> {
                new Person { FirstName = "Jon", LastName = "Snow", BirthDate = new DateTime(1987, 1, 15), IsActive = true, Level = 25 },
                new Person { FirstName = "Arya", LastName = "Stark", BirthDate = new DateTime(1987, 1, 15), Level = 10},
                new Person { FirstName = "Sansa", LastName = "Stark", BirthDate = new DateTime(1987, 1, 15), Level = 10},
                new Person { FirstName = "Oberyn", LastName = "Martell", BirthDate = new DateTime(1987, 1, 15) },
            };
        }

    }
}
