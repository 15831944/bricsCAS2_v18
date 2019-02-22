using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bricscad.EditorInput;
using Bricscad.ApplicationServices;
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using _AcAp = Bricscad.ApplicationServices;


using CAS.myUtilities;
using CAS.myUtilities.myString;

namespace CAS.myFunctions
{
    public class SetPoint
    {
        Editor m_ed = null;
        readonly myCAD.MyUtilities objUtil = new myCAD.MyUtilities();
        MyConfig _config = new MyConfig();

        private static string PNrZähler = "0";

        //Methods
        public void Start()
        {
            string _block = _config.GetAppSettingString("Block");
            string _Basislayer = _config.GetAppSettingString("Basislayer");
            m_ed =  Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointOptions prPtOpt = new PromptPointOptions("Bitte Position wählen:");
            bool cont = true;

            while(cont)
            {
                PromptPointResult prPtRes = m_ed.GetPoint("Position wählen:");

                if (prPtRes.Status == PromptStatus.OK)
                {
                    PNrZähler = MyString.Increment(PNrZähler, Mode.AlphaNumeric);
                    PromptStringOptions prStringOpt = new PromptStringOptions("Nr: " + PNrZähler)
                    { AllowSpaces = false };

                    PromptResult prRes = m_ed.GetString(prStringOpt);

                    if (prRes.StringResult != "")
                        PNrZähler = prRes.StringResult;

                    Point2d pt2d = new Point2d(prPtRes.Value.X, prPtRes.Value.Y);
                    CAS.myCAD.Messpunkt MP = new myCAD.Messpunkt(PNrZähler, pt2d);
                    MP.Draw(_block, _Basislayer);
                }
                else
                    cont = false;
            }
        }
    }
}
