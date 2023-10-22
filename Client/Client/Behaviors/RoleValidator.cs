using BrassLoon.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Client.Behaviors
{
    public class RoleValidator
    {
        private readonly RoleVM _roleVM;

        public RoleValidator(RoleVM roleVM)
        {
            _roleVM = roleVM;
            roleVM.PropertyChanged += RoleVM_PropertyChanged;
        }

        private void RoleVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_roleVM[e.PropertyName] != null)
                _roleVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(RoleVM.Name):
                    RequiredTextField(e.PropertyName, _roleVM.Name, _roleVM);
                    break;
                case nameof(RoleVM.PolicyName):
                    RequiredTextField(e.PropertyName, _roleVM.PolicyName, _roleVM);
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
