using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class WorkTaskStatusVM : ViewModelBase
    {
        private readonly WorkTaskStatus _taskStatus;
        private readonly WorkTaskTypeVM _taskTypeVM;
        private ICommand _save;

        public WorkTaskStatusVM(WorkTaskStatus taskStatus, WorkTaskTypeVM taskTypeVM)
        {
            _taskStatus = taskStatus;
            _taskTypeVM = taskTypeVM;
        }

        internal WorkTaskStatus InnerTaskStatus => _taskStatus;

        internal WorkTaskTypeVM TaskTypeVM => _taskTypeVM;

        public Guid? WorkTaskStatusId => _taskStatus.WorkTaskStatusId;

        public Guid? WorkTaskTypeId => _taskStatus.WorkTaskTypeId;

        public Guid? DomainId => _taskStatus.DomainId;

        public int WorkTaskCount => _taskStatus.WorkTaskCount ?? 0;

        public bool IsNotNew => WorkTaskStatusId.HasValue;

        public string Code
        {
            get => _taskStatus.Code;
            set
            {
                if (_taskStatus.Code != value)
                {
                    _taskStatus.Code = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _taskStatus.Name;
            set
            {
                if (_taskStatus.Name != value)
                {
                    _taskStatus.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _taskStatus.Description;
            set
            {
                if (_taskStatus.Description != value)
                {
                    _taskStatus.Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsDefaultStatus
        {
            get => _taskStatus.IsDefaultStatus ?? false;
            set
            {
                if (!_taskStatus.IsDefaultStatus.HasValue || _taskStatus.IsDefaultStatus.Value != value)
                {
                    _taskStatus.IsDefaultStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsClosedStatus
        {
            get => _taskStatus.IsClosedStatus ?? false;
            set
            {
                if (!_taskStatus.IsClosedStatus.HasValue || _taskStatus.IsClosedStatus.Value != value)
                {
                    _taskStatus.IsClosedStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Save
        {
            get => _save;
            set
            {
                if (_save != value)
                {
                    _save = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
