using IntegrationTool.ApplicationCore;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ProjectDesigner
{
    public class InitOperation
    {
        bool completed = false;
        string errorDescription;
        /// <summary>
        /// the operation is either completed successfully (true) or
        /// aborted due to exceptions (false).
        /// </summary>
        public bool Completed
        {
            get { return completed; }
        }
        /// <summary>
        /// the error text to be displayed in GUI in case of error
        /// (i.e. completed==false)
        /// </summary>
        public string ErrorDescription
        {
            get { return errorDescription; }
        }

        public InitOperation()
        {
            completed = false;
            errorDescription = String.Empty;
        }

        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(e);
        }
        /// <summary>
        /// Upon operation completion, fire a Complete event so that the 
        /// observer can receive and act on it.
        /// </summary>
        protected virtual void OnComplete(EventArgs e)
        {
            if (this.Complete != null)
            {
                this.Complete(this, e);
            }
        }
        /// <summary>
        /// The long-running operation is performed here.
        /// </summary>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ApplicationInitializer applicationInitializer = new ApplicationInitializer();
                e.Result = new object[] { applicationInitializer };

                foreach(ModuleDescription moduleDescription in applicationInitializer.ModuleLoader.Modules)
                {

                }
                System.Threading.Thread.Sleep(1500);
                
                // long-running operation goes here.
                
                completed = true;
            }
            catch (Exception ex)
            {
                completed = false;
                errorDescription = ex.Message + "\n" + ex.StackTrace;
            }
        }
        /// <summary>
        /// the event to be fired upon operation completion
        /// </summary>
        public event EventHandler Complete;
    }


}
