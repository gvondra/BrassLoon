using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class WorkGroupVM : ViewModelBase
    {
        private readonly WorkGroup _workGroup;
        private readonly WorkGroupsVM _workGroupsVM;
        private ICommand _save;

        public WorkGroupVM(WorkGroup workGroup, WorkGroupsVM workGroupsVM)
        {
            _workGroup = workGroup;
            _workGroupsVM = workGroupsVM;
        }

        internal WorkGroup InnerWorkGroup => _workGroup;

        internal WorkGroupsVM WorkGroupsVM => _workGroupsVM;

        public Guid? WorkGroupId => _workGroup.WorkGroupId;

        public Guid? DomainId => _workGroup.DomainId;

        public string Title
        { 
            get => _workGroup.Title;
            set
            {
                if (_workGroup.Title != value)
                {
                    _workGroup.Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _workGroup.Description;
            set
            {
                if (_workGroup.Description != value)
                {
                    _workGroup.Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? CreateTimestamp => _workGroup.CreateTimestamp;

        public DateTime? UpdateTimestamp => _workGroup.UpdateTimestamp;

        //public List<string> MemberUserIds { get; set; }

        //public List<Guid> WorkTaskTypeIds { get; set; }

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
