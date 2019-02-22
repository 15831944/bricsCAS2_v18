using Bricscad.Runtime;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.Runtime;
using _Ac = Bricscad;
using _AcAp = Bricscad.ApplicationServices;

[assembly: CommandClass(typeof(ShowProgressBar.MyCadCommands))]

namespace ShowProgressBar
{
    public class MyCadCommands
    {
        [CommandMethod("LongWork1")]
        public static void RunLongWork_1()
        {
            _AcAp.Document dwg = _AcAp.Application.DocumentManager.MdiActiveDocument;
            Editor ed = dwg.Editor;

            try
            {
                MyCadDataHandler dataHandler = new MyCadDataHandler(dwg);
                dataHandler.DoWorkWithKnownLoopCount();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(
                    "\nError: {0}\n{1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                _Ac.Internal.Utils.PostCommandPrompt();
            }
        }

        [CommandMethod("LongWork2")]
        public static void RunLongWork_2()
        {
            Document dwg = _AcAp.Application.DocumentManager.MdiActiveDocument;
            Editor ed = dwg.Editor;

            try
            {
                MyCadDataHandler dataHandler = new MyCadDataHandler(dwg);
                dataHandler.DoWorkWithUnknownLoopCount();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(
                    "\nError: {0}\n{1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                _Ac.Internal.Utils.PostCommandPrompt();
            }
        }
    }
}
