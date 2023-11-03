using BrassLoon.Interface.Authorization.Models;

namespace BrassLoon.Client.ViewModel
{
    public class AppliedRoleVM : ViewModelBase
    {
        public readonly AppliedRole _appliedRole;
        private bool _isApplied;

        public AppliedRoleVM(AppliedRole appliedRole)
        {
            _appliedRole = appliedRole;
        }

        internal AppliedRole InnerAppliedRole => _appliedRole;

        public string Name => _appliedRole.Name;

        public string PolicyName => _appliedRole.PolicyName;

        public bool IsApplied
        {
            get => _isApplied;
            set
            {
                if (_isApplied != value)
                {
                    _isApplied = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
