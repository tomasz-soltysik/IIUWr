﻿using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModelInterfaces.Fereol;
using LionCub.Patterns.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IIUWr.ViewModels.Fereol
{
    public class SemesterViewModel : ISemesterViewModel
    {
        private readonly ICoursesService _coursesService;

        public SemesterViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ICourseViewModel> _courses = new ObservableCollection<ICourseViewModel>();
        public ObservableCollection<ICourseViewModel> Courses
        {
            get { return _courses; }
            private set
            {
                if (_courses != value)
                {
                    _courses = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private Semester _semester;
        public Semester Semester
        {
            get { return _semester; }
            set
            {
                if (_semester != value)
                {
                    _semester = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            private set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged.Notify(this);
                }
            }
        }
        
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    if (value)
                    {
                        Refresh();
                    }
                    else
                    {
                        Courses.Clear();
                    }
                    _isExpanded = value;
                    PropertyChanged.Notify(this);
                }
            }
        }
        
        public async void Refresh()
        {
            IsRefreshing = true;
            var result = await _coursesService.GetCourses(Semester);
            if (result != null)
            {
                foreach (Course course in result)
                {
                    var courseVM = Courses.FirstOrDefault(vm => vm.Course == course);
                    if (courseVM == null)
                    {
                        courseVM = IoC.Get<ICourseViewModel>();
                        Courses.Add(courseVM);
                    }
                    courseVM.Course = course;
                }
            }
            IsRefreshing = false;
        }
    }
}
