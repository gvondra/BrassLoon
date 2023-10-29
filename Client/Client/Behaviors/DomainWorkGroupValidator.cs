using BrassLoon.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Client.Behaviors
{
    public class DomainWorkGroupValidator
    {
        private readonly WorkGroupVM _workGroupVM;

        public DomainWorkGroupValidator(WorkGroupVM workGroupVM)
        {
            _workGroupVM = workGroupVM;
            workGroupVM.PropertyChanged += WorkGroupVM_PropertyChanged;
        }

        private void WorkGroupVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_workGroupVM[e.PropertyName] != null)
                _workGroupVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(WorkGroupVM.Title):
                    RequiredTextField(e.PropertyName, _workGroupVM.Title, _workGroupVM);
                    break;
            }
        }

        private static void RequiredTextField(string propertyName, string value, ViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(value))
                viewModel[propertyName] = "Is required";
        }
    }
}
