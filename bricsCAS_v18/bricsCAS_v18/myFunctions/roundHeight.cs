using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bricscad.Runtime;
using Bricscad.EditorInput;
using Teigha.Runtime;
using Teigha.Geometry;
using _AcAp = Bricscad.ApplicationServices;
using CAS.myCAD;

namespace CAS.myFunctions
{
    public class RoundHeight
    {
        Editor _ed = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
        readonly myCAD.MyUtilities objUtil = new myCAD.MyUtilities();
        List<Messpunkt> _lsMP = new List<Messpunkt>();
        int _digits, counter = 0;

        //methods
        public void Start()
        {
            Blöcke.Instance.Init();
            ErrorStatus es = Blöcke.Instance.SelectWindow();
            _lsMP = Blöcke.Instance.LsMP;
            //Blöcke.Instance.Dispose();

            if (es == ErrorStatus.OK)
            {
                PromptIntegerResult resInt = _ed.GetInteger("Kommastellen: ");

                if (resInt.Status == PromptStatus.OK)
                {
                    _digits = resInt.Value;

                    foreach (Messpunkt mp in _lsMP)
                    {
                        if (mp.RoundHeight(_digits) == ErrorStatus.OK)
                            counter++;
                    }
                    MessageBox.Show(counter + " Punkte gerundet");
                }
            }
        }
    }
}
