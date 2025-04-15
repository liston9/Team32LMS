using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {

            var query = from g in db.Grades
                join s in db.Students on g.StudentId equals s.UId
                join courses in db.Courses on subject equals courses.DId
                join classes in db.Classes on season equals classes.Season
                where num == classes.CourseId && year == classes.Year
                    select new
                    {
                        fname = s.FirstName,
                        lname = s.LastName,
                        uid = s.UId,
                        dob = s.Dob,
                        grade = g.Grade1
                    }
            ;
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var query = from Courses in db.Courses
                join classes in db.Classes on Courses.CourseId equals classes.CourseId
                join cat in db.AssignmentCategories on classes.ClassId equals cat.ClassId
                join assign in db.Assignments on cat.CategoryId equals assign.CategoryId
                where Courses.DId == subject && Courses.Number == num && classes.Season == season &&
                      classes.Year == year && (cat.Name == category || category == null)

                select new
                {
                    aname = assign.Name,
                    cname = cat.Name,
                    due = assign.DueDate,
                    submissions = assign.Submissions.Count
                };
            
            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the following fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from Courses in db.Courses
                join classes in db.Classes on Courses.CourseId equals classes.CourseId
                join cat in db.AssignmentCategories on classes.ClassId equals cat.ClassId
                where Courses.DId == subject && Courses.Number == num && classes.Season == season && classes.Year == year
                select new
                {
                    name = cat.Name,
                    weight = cat.GradeWeight
                };
            
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var query = from Courses in db.Courses join classes in db.Classes on Courses.CourseId equals classes.CourseId
                where Courses.DId == subject && Courses.Number == num && classes.Season == season && classes.Year == year
                select classes.ClassId;
            var duplicateQuery = from AssignmentCategory in db.AssignmentCategories
                where AssignmentCategory.ClassId == query.First() && AssignmentCategory.Name == category
                    select AssignmentCategory;
            
            if (duplicateQuery.Any())
                return Json(new { success = false });
            
            var newCat = new AssignmentCategory()
            {
                Name = category,
                GradeWeight = (byte)catweight,
                ClassId = query.First(),
            };
            db.AssignmentCategories.Add(newCat);
            db.SaveChanges();
            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var query = from Courses in db.Courses
                join classes in db.Classes on Courses.CourseId equals classes.CourseId
                join cat in db.AssignmentCategories on classes.ClassId equals cat.ClassId
                where Courses.DId == subject && Courses.Number == num && classes.Season == season && classes.Year == year && cat.Name == category
                select cat.CategoryId;

            var duplicateAssignment = from Assignment in db.Assignments
                where Assignment.CategoryId == query.First() && Assignment.Name == asgname
                select Assignment;
            
            if (duplicateAssignment.Any())
                return Json(new { success = false });
            
            var assignment = new Assignment()
            {
                Name = asgname,
                MaxPoints = (uint)asgpoints,
                Contents = asgcontents,
                DueDate = asgdue,
                CategoryId = query.First(),
            };
            
            db.Assignments.Add(assignment);
            db.SaveChanges();

            var grades = from Courses in db.Courses
                join classes in db.Classes on Courses.CourseId equals classes.CourseId
                join grade in db.Grades on classes.ClassId equals grade.ClassId
                where Courses.DId == subject && Courses.Number == num && classes.Season == season &&
                      classes.Year == year
                select grade;
            
            List<Grade> g = grades.ToList();
            foreach (var grade in g)
                updateStudentGrade(grade.StudentId, grade.ClassId);
            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            return Json(null);
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            return Json(new { success = false });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from courses in db.Courses
                join classes in db.Classes on courses.CourseId equals classes.CourseId
                where uid == classes.ProfessorId
                select new
                {
                    subject = courses.DId,
                    number = courses.Number,
                    name = courses.Name,
                    season = classes.Season,
                    year = classes.Year,
                };
            return Json(query.ToArray());
        }

        private void updateStudentGrade(string uid, uint classID)
        {
            Dictionary<uint, int> categoryWeights = new();
            Dictionary<uint, long> studentScoreInCat = new();
            
            var cats = from AssignmentCategory in db.AssignmentCategories
                where AssignmentCategory.ClassId == classID
                    select AssignmentCategory;

            List<AssignmentCategory> c = cats.ToList();
            foreach (var category in c)
            {
                var classAssigns = from Assignment in db.Assignments
                    where Assignment.CategoryId == category.CategoryId
                    select Assignment.MaxPoints;

                long catPoints = 0;
                foreach (var points in classAssigns)
                    catPoints += points;

                var studentQuery = from Submission in db.Submissions
                    join Assignment in db.Assignments on Submission.AssignmentId equals Assignment.AssignmentId
                    where Submission.StudentId == uid && Assignment.CategoryId == category.CategoryId
                    select Submission.Score;
                
                long studentPoints = 0;
                List<uint> students = studentQuery.ToList();
                foreach (var points in students)
                    studentPoints += points;
                
                if (catPoints != 0) {
                    categoryWeights.Add(category.CategoryId, category.GradeWeight);
                    studentScoreInCat.Add(category.CategoryId, studentPoints/catPoints);
                }
            }

            double factor = 100.0/categoryWeights.Values.Sum();
            double grade = 0;
            foreach (var studentScore in studentScoreInCat)
                grade += studentScore.Value * categoryWeights[studentScore.Key];
            grade *= factor;
            string letterGrade;
            
            if (grade >= 93)
                letterGrade = "A";
            else if (grade >= 90)
                letterGrade = "A-";
            else if (grade >= 87)
                letterGrade = "B+";
            else if (grade >= 83)
                letterGrade = "B";
            else if (grade >= 80)
                letterGrade = "B-";
            else if (grade >= 77)
                letterGrade = "C+";
            else if (grade >= 73)
                letterGrade = "C";
            else if (grade >= 70)
                letterGrade = "C-";
            else if (grade >= 67)
                letterGrade = "D+";
            else if (grade >= 63)
                letterGrade = "D";
            else if (grade >= 60)
                letterGrade = "D-";
            else
                letterGrade = "E";
            
            var changeGrade = from Grade in db.Grades
                where Grade.StudentId == uid && Grade.ClassId == classID
                    select Grade;
            changeGrade.First().Grade1 = letterGrade;
            db.SaveChanges();
        }
        
        /*******End code to modify********/
    }
}

