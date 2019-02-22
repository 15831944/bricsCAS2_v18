using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowProgressBar
{
    public class ProcessingProgressBar : IDisposable
    {
        private dlgProgress _dlg = null;
        private readonly ILongProcessingObject _executingObject;

        public ProcessingProgressBar(ILongProcessingObject executingObj)
        {
            _executingObject = executingObj;
        }

        public void Start()
        {
            _dlg = new dlgProgress(_executingObject);
            _dlg.ShowDialog();
        }

        public void Dispose()
        {
            if (_dlg!=null)
            {
                _dlg.Dispose();
            }
        }
    }
}
