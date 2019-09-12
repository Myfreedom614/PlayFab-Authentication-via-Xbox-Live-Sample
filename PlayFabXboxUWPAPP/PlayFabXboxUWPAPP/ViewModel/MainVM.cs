using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayFabXboxTestAPP1.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private string xblOutput;
        private bool xblSignedIn;
        private bool hasXblToken;
        private string xblTokenOutput;
        private string xboxToken;
        private string pfOutput;
        private string gameTag;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MainVM()
        {
            this.XblOutput = string.Empty;
            this.XblSignedIn = false;
            this.HasXblToken = false;
            this.XblTokenOutput = string.Empty;
            this.XboxToken = string.Empty;
            this.PFOutput = string.Empty;
        }

        public string XblOutput
        {
            get { return this.xblOutput; }
            set
            {
                this.xblOutput = value;
                this.OnPropertyChanged();
            }
        }

        public bool XblSignedIn
        {
            get { return xblSignedIn; }
            set
            {
                xblSignedIn = value;
                this.OnPropertyChanged();
            }
        }

        public bool HasXblToken
        {
            get { return hasXblToken; }
            set
            {
                hasXblToken = value;
                this.OnPropertyChanged();
            }
        }

        public string XblTokenOutput
        {
            get { return xblTokenOutput; }
            set
            {
                xblTokenOutput = value;
                this.OnPropertyChanged();
            }
        }

        public string XboxToken
        {
            get { return xboxToken; }
            set
            {
                xboxToken = value;
                this.OnPropertyChanged();
            }
        }

        public string PFOutput
        {
            get { return pfOutput; }
            set
            {
                pfOutput = value;
                this.OnPropertyChanged();
            }
        }

        public string GameTag
        {
            get { return gameTag; }
            set
            {
                gameTag = value;
                this.OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
