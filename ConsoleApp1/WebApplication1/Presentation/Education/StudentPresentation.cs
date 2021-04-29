﻿using AutoMapper;
using Microsoft.AspNetCore.Routing;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.EfStuff.Model;
using WebApplication1.EfStuff.Repositoryies;
using WebApplication1.Models;

namespace WebApplication1.Presentation
{
    public class StudentPresentation
    {
        private StudentRepository _studentRepository;
        private UniversityRepository _universityRepository;
        private IMapper Mapper { get; set; }

        public StudentPresentation(StudentRepository studentRepository, UniversityRepository universityRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _universityRepository = universityRepository;
            Mapper = mapper;            
        }

        public PagingList<StudentViewModel> GetStudentList(int page)
        {
            var students = _studentRepository
               .GetAll()
               .Select(x => Mapper.Map<StudentViewModel>(x))
               .ToList();
            var model = PagingList.Create(students, 3, page);

            model.Action = "StudentListAndSearch";
            return model;
        }

        public PagingList<StudentViewModel> GetStudentListAndSearch(string searchBy, string searchStudent, int page = 1)
        {
            var query = from x in _studentRepository.GetAll() select x;
            if (!String.IsNullOrEmpty(searchStudent))
            {
                if (searchBy == "iin")
                {
                    query = query.Where(x => x.IIN.Equals(searchStudent));
                }
                if (searchBy == "name")
                {
                    query = query.Where(x => x.Name.Contains(searchStudent));
                }
                if (searchBy == "courseYear")
                {
                    query = query.Where(x => x.CourseYear == int.Parse(searchStudent)).OrderBy(s => s.UniversityId);
                }
                if (searchBy == "universityID")
                {
                    query = query.Where(x => x.UniversityId == int.Parse(searchStudent)).OrderBy(s => s.CourseYear);
                }
            }

            List<StudentViewModel> studentViewModels = new List<StudentViewModel>();

            foreach (var item in query)
            {
                studentViewModels.Add(Mapper.Map<StudentViewModel>(item));
            }

            var model = PagingList.Create(studentViewModels, 3, page);
            model.RouteValue = new RouteValueDictionary {
                                    {"searchBy", searchBy},
                                    {"searchStudent", searchStudent} };

            model.Action = "StudentListAndSearch";
            return model;
        }

        public StudentViewModel GetStudentById(long studentId)
        {
            var student = _studentRepository.Get(studentId);
            var studentViewModel = Mapper.Map<StudentViewModel>(student);
            return studentViewModel;
        }

        public void GetStudentGrantByGpa(string select, double minGpaValue)
        {
            if (select == "issueGrant")
            {
                var studentsNoGrant = _studentRepository.GetAll()
                     .Where(x => x.OnGrant == false)
                     .Where(c => c.CourseYear > 1)
                     .Where(student => student.Gpa >= minGpaValue)
                     .ToList();
                foreach (var student in studentsNoGrant)
                {
                    _studentRepository.UpdateStudentGrantData(student.Id, true);
                }
            }
            else
            {
                var studentsYesGrant = _studentRepository.GetAll().Where(x => x.OnGrant == true).Where(student => student.Gpa <= minGpaValue).ToList();
                foreach (var student in studentsYesGrant)
                {
                    _studentRepository.UpdateStudentGrantData(student.Id, false);
                }
            }
        }

        public void GetStudentGrantIndividual(long id, bool onGrant)
        {
            _studentRepository.UpdateStudentGrantData(id, onGrant);
        }

        public void GetAddNewOrEditStudent(StudentViewModel studentViewModel)
        {
            var student = Mapper.Map<Student>(studentViewModel);
            _studentRepository.Save(student);
        }

        public List<University> GetUniversityList()
        {
            return _universityRepository.GetAll();
        }

        public University GetUniversityByUniversityName(string universityName)
        {
            return _universityRepository.GetUniversityByName(universityName);
        }

        public List<string> GetListOfUniversityNames()
        {
            var all = GetUniversityList();
            List<string> universityNames = new List<string>();
            foreach (var university in all)
            {
                universityNames.Add(university.Name);
            }

            return universityNames;
        }

        public List<long> GetListOfUniversityIds()
        {
            var all = GetUniversityList();
            List<long> universityIds = new List<long>();
            foreach (var university in all)
            {
                universityIds.Add(university.Id);
            }

            return universityIds;
        }

        public void EndStudyYearForUniversity()
        {
            List<Student> students = _studentRepository.GetAll();
            int fourthCourseStudentsCount = 0;
            foreach (Student student in students)
            {
                if (student.CourseYear != 4)
                {
                    student.CourseYear = student.CourseYear + 1;
                }
                else
                {
                    student.CourseYear = null;
                    student.GraduatedYear = DateTime.Now;
                    // Certificate
                    fourthCourseStudentsCount++;
                }
                _studentRepository.Save(student);
            }            
        }
    }
}
