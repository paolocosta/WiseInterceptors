using System;
using System.Runtime.Caching;
using System.Windows.Forms;
using WiseInterceptors.Interceptors.Cache;

namespace CacheDemo
{
    public partial class Form1 : Form
    {
        TimeGetter _timeGetter;
        public Form1(TimeGetter timeGetter)
        {
            _timeGetter = timeGetter;
            InitializeComponent();
        }

        private void StartSimulation_Click(object sender, EventArgs e)
        {
            Settings.Enabled = false;
            Simulation.Enabled = true;
        }

        private void StopSimulation_Click(object sender, EventArgs e)
        {
            Settings.Enabled = true;
            Simulation.Enabled = false;
            Times.Items.Clear();
            foreach (var element in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(element.Key);
            }
        }

        public CacheSettings GetCacheSettings()
        {
            //I'm testing one method so I hard code the key. Please don't do this in real code!!
            return new CacheSettings
            {
                Duration = (int)Duration.Value,
                Priority = PriorityEnum.Normal,
                UseCache = UseCache.Checked,                
                Key = "Key"
            };
        }

        public FaultToleranceEnum GetFaultTolerance()
        {
            foreach(RadioButton c in FaultTolerance.Controls)
            {
                if (c.Checked)
                {
                    return (FaultToleranceEnum)Enum.Parse(typeof(FaultToleranceEnum), c.Name);
                }
            }
            //I'll never get here but the compiler doesn't know it
            return FaultToleranceEnum.AlwaysUsePersistentCache;
        }

        private void btnWriteUctNow_Click(object sender, EventArgs e)
        {
            Times.Items.Insert(0, string.Format("{0} {1}", DateTime.Now.ToLongTimeString(), _timeGetter.Now(false).ToLongTimeString()));
        }

        private void GenerateException_Click(object sender, EventArgs e)
        {
            try
            {
                Times.Items.Insert(0, string.Format("{0} {1}", DateTime.Now.ToLongTimeString(), _timeGetter.Now(true).ToLongTimeString()));
            }
            catch (ApplicationException)
            {
                Times.Items.Insert(0, string.Format("{0} exception", DateTime.Now.ToLongTimeString()));
            }
        }
    }
}
