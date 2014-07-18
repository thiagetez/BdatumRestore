using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BdatumRestore.Model
{
    public class FormModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //[JsonProperty(PropertyName = "mime")]
        //public string MimeType { get; private set; }

        //[JsonProperty(PropertyName = "metadata")]
        //public List<string> Metadata { get; private set; }

        //[JsonProperty(PropertyName = "version")]
        //public int Version { get; private set; }

        //[JsonProperty(PropertyName = "size")]
        //public long? Size { get; private set; }

        //[JsonProperty(PropertyName = "extension")]
        //public string Extension { get; private set; }

        public FormModel(string name)
        {
            Name = name; 
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
           if(handler!=null)
           {
               handler(this, new PropertyChangedEventArgs(propertyName));
           }
        }

        

      

    }
}
