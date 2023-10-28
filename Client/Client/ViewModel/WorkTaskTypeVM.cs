using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.ObjectModel;

namespace BrassLoon.Client.ViewModel
{
    public class WorkTaskTypeVM : ViewModelBase
    {
        private readonly WorkTaskType _taskType;
        private readonly WorkTaskTypesVM _taskTypesVM;

        public WorkTaskTypeVM(WorkTaskType taskType, WorkTaskTypesVM taskTypesVM)
        {
            _taskType = taskType;
            _taskTypesVM = taskTypesVM;
        }

        internal WorkTaskType InnerTaskType => _taskType;

        internal WorkTaskTypesVM TaskTypesVM => _taskTypesVM;

        public ObservableCollection<WorkTaskStatusVM> Statuses { get; } = new ObservableCollection<WorkTaskStatusVM>();

        public Guid? WorkTaskTypeId => _taskType.WorkTaskTypeId;

        public Guid? DomainId => _taskType.DomainId;

        public int WorkTaskCount => _taskType.WorkTaskCount ?? 0;

        public string Code
        {
            get => _taskType.Code;
            set
            {
                if (_taskType.Code != value)
                {
                    _taskType.Code = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _taskType.Title;
            set
            {
                if (_taskType.Title != value)
                {
                    _taskType.Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _taskType.Description;
            set
            {
                if (_taskType.Description != value)
                {
                    _taskType.Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public short? PurgePeriod
        {
            get => _taskType.PurgePeriod;
            set
            {
                if (_taskType.PurgePeriod.HasValue != value.HasValue
                    || (value.HasValue && _taskType.PurgePeriod.Value != value.Value))
                {
                    _taskType.PurgePeriod = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
