using System.ComponentModel;
using System.Windows.Forms;

namespace CacheDemo
{
    partial class Form1
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnWriteUctNow = new System.Windows.Forms.Button();
            this.Settings = new System.Windows.Forms.GroupBox();
            this.StartSimulation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Duration = new System.Windows.Forms.NumericUpDown();
            this.UseCache = new System.Windows.Forms.CheckBox();
            this.FaultTolerance = new System.Windows.Forms.GroupBox();
            this.AlwaysUsePersistentCache = new System.Windows.Forms.RadioButton();
            this.UsePersistentCacheOnlyInCaseOfError = new System.Windows.Forms.RadioButton();
            this.FailFastWithNoRecovery = new System.Windows.Forms.RadioButton();
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors = new System.Windows.Forms.RadioButton();
            this.Simulation = new System.Windows.Forms.GroupBox();
            this.StopSimulation = new System.Windows.Forms.Button();
            this.Times = new System.Windows.Forms.ListBox();
            this.GenerateException = new System.Windows.Forms.Button();
            this.Settings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Duration)).BeginInit();
            this.FaultTolerance.SuspendLayout();
            this.Simulation.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnWriteUctNow
            // 
            this.btnWriteUctNow.Location = new System.Drawing.Point(6, 29);
            this.btnWriteUctNow.Name = "btnWriteUctNow";
            this.btnWriteUctNow.Size = new System.Drawing.Size(156, 33);
            this.btnWriteUctNow.TabIndex = 0;
            this.btnWriteUctNow.Text = "Write UCTNow";
            this.btnWriteUctNow.UseVisualStyleBackColor = true;
            this.btnWriteUctNow.Click += new System.EventHandler(this.btnWriteUctNow_Click);
            // 
            // Settings
            // 
            this.Settings.Controls.Add(this.StartSimulation);
            this.Settings.Controls.Add(this.label1);
            this.Settings.Controls.Add(this.Duration);
            this.Settings.Controls.Add(this.UseCache);
            this.Settings.Controls.Add(this.FaultTolerance);
            this.Settings.Dock = System.Windows.Forms.DockStyle.Top;
            this.Settings.Location = new System.Drawing.Point(0, 0);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(610, 194);
            this.Settings.TabIndex = 2;
            this.Settings.TabStop = false;
            this.Settings.Text = "Settings";
            // 
            // StartSimulation
            // 
            this.StartSimulation.Location = new System.Drawing.Point(288, 108);
            this.StartSimulation.Name = "StartSimulation";
            this.StartSimulation.Size = new System.Drawing.Size(207, 23);
            this.StartSimulation.TabIndex = 6;
            this.StartSimulation.Text = "Start simulation";
            this.StartSimulation.UseVisualStyleBackColor = true;
            this.StartSimulation.Click += new System.EventHandler(this.StartSimulation_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(285, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Duration (sec)";
            // 
            // Duration
            // 
            this.Duration.Location = new System.Drawing.Point(375, 59);
            this.Duration.Name = "Duration";
            this.Duration.Size = new System.Drawing.Size(120, 20);
            this.Duration.TabIndex = 4;
            this.Duration.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // UseCache
            // 
            this.UseCache.AutoSize = true;
            this.UseCache.Location = new System.Drawing.Point(288, 29);
            this.UseCache.Name = "UseCache";
            this.UseCache.Size = new System.Drawing.Size(76, 17);
            this.UseCache.TabIndex = 3;
            this.UseCache.Text = "UseCache";
            this.UseCache.UseVisualStyleBackColor = true;
            // 
            // FaultTolerance
            // 
            this.FaultTolerance.Controls.Add(this.AlwaysUsePersistentCache);
            this.FaultTolerance.Controls.Add(this.UsePersistentCacheOnlyInCaseOfError);
            this.FaultTolerance.Controls.Add(this.FailFastWithNoRecovery);
            this.FaultTolerance.Controls.Add(this.ConsiderSoftlyExpiredValuesInCaseOfErrors);
            this.FaultTolerance.Location = new System.Drawing.Point(7, 20);
            this.FaultTolerance.Name = "FaultTolerance";
            this.FaultTolerance.Size = new System.Drawing.Size(241, 140);
            this.FaultTolerance.TabIndex = 2;
            this.FaultTolerance.TabStop = false;
            this.FaultTolerance.Text = "Fault tolerance";
            // 
            // AlwaysUsePersistentCache
            // 
            this.AlwaysUsePersistentCache.AutoSize = true;
            this.AlwaysUsePersistentCache.Location = new System.Drawing.Point(16, 88);
            this.AlwaysUsePersistentCache.Name = "AlwaysUsePersistentCache";
            this.AlwaysUsePersistentCache.Size = new System.Drawing.Size(154, 17);
            this.AlwaysUsePersistentCache.TabIndex = 3;
            this.AlwaysUsePersistentCache.TabStop = true;
            this.AlwaysUsePersistentCache.Text = "AlwaysUsePersistentCache";
            this.AlwaysUsePersistentCache.UseVisualStyleBackColor = true;
            // 
            // UsePersistentCacheOnlyInCaseOfError
            // 
            this.UsePersistentCacheOnlyInCaseOfError.AutoSize = true;
            this.UsePersistentCacheOnlyInCaseOfError.Location = new System.Drawing.Point(16, 65);
            this.UsePersistentCacheOnlyInCaseOfError.Name = "UsePersistentCacheOnlyInCaseOfError";
            this.UsePersistentCacheOnlyInCaseOfError.Size = new System.Drawing.Size(208, 17);
            this.UsePersistentCacheOnlyInCaseOfError.TabIndex = 2;
            this.UsePersistentCacheOnlyInCaseOfError.TabStop = true;
            this.UsePersistentCacheOnlyInCaseOfError.Text = "UsePersistentCacheOnlyInCaseOfError";
            this.UsePersistentCacheOnlyInCaseOfError.UseVisualStyleBackColor = true;
            // 
            // FailFastWithNoRecovery
            // 
            this.FailFastWithNoRecovery.AutoSize = true;
            this.FailFastWithNoRecovery.Location = new System.Drawing.Point(16, 19);
            this.FailFastWithNoRecovery.Name = "FailFastWithNoRecovery";
            this.FailFastWithNoRecovery.Size = new System.Drawing.Size(143, 17);
            this.FailFastWithNoRecovery.TabIndex = 0;
            this.FailFastWithNoRecovery.TabStop = true;
            this.FailFastWithNoRecovery.Text = "FailFastWithNoRecovery";
            this.FailFastWithNoRecovery.UseVisualStyleBackColor = true;
            // 
            // ConsiderSoftlyExpiredValuesInCaseOfErrors
            // 
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.AutoSize = true;
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.Location = new System.Drawing.Point(16, 42);
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.Name = "ConsiderSoftlyExpiredValuesInCaseOfErrors";
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.Size = new System.Drawing.Size(230, 17);
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.TabIndex = 1;
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.TabStop = true;
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.Text = "ConsiderSoftlyExpiredValuesInCaseOfErrors";
            this.ConsiderSoftlyExpiredValuesInCaseOfErrors.UseVisualStyleBackColor = true;
            // 
            // Simulation
            // 
            this.Simulation.Controls.Add(this.GenerateException);
            this.Simulation.Controls.Add(this.Times);
            this.Simulation.Controls.Add(this.StopSimulation);
            this.Simulation.Controls.Add(this.btnWriteUctNow);
            this.Simulation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Simulation.Enabled = false;
            this.Simulation.Location = new System.Drawing.Point(0, 200);
            this.Simulation.Name = "Simulation";
            this.Simulation.Size = new System.Drawing.Size(610, 238);
            this.Simulation.TabIndex = 3;
            this.Simulation.TabStop = false;
            this.Simulation.Text = "Simulation";
            // 
            // StopSimulation
            // 
            this.StopSimulation.Location = new System.Drawing.Point(6, 203);
            this.StopSimulation.Name = "StopSimulation";
            this.StopSimulation.Size = new System.Drawing.Size(171, 23);
            this.StopSimulation.TabIndex = 2;
            this.StopSimulation.Text = "Stop simulation";
            this.StopSimulation.UseVisualStyleBackColor = true;
            this.StopSimulation.Click += new System.EventHandler(this.StopSimulation_Click);
            // 
            // Times
            // 
            this.Times.FormattingEnabled = true;
            this.Times.Location = new System.Drawing.Point(246, 29);
            this.Times.Name = "Times";
            this.Times.Size = new System.Drawing.Size(358, 199);
            this.Times.TabIndex = 3;
            // 
            // GenerateException
            // 
            this.GenerateException.Location = new System.Drawing.Point(6, 78);
            this.GenerateException.Name = "GenerateException";
            this.GenerateException.Size = new System.Drawing.Size(156, 33);
            this.GenerateException.TabIndex = 4;
            this.GenerateException.Text = "Generate exception";
            this.GenerateException.UseVisualStyleBackColor = true;
            this.GenerateException.Click += new System.EventHandler(this.GenerateException_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 438);
            this.Controls.Add(this.Simulation);
            this.Controls.Add(this.Settings);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Settings.ResumeLayout(false);
            this.Settings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Duration)).EndInit();
            this.FaultTolerance.ResumeLayout(false);
            this.FaultTolerance.PerformLayout();
            this.Simulation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnWriteUctNow;
        private GroupBox Settings;
        private GroupBox FaultTolerance;
        private RadioButton FailFastWithNoRecovery;
        private RadioButton ConsiderSoftlyExpiredValuesInCaseOfErrors;
        private RadioButton UsePersistentCacheOnlyInCaseOfError;
        private RadioButton AlwaysUsePersistentCache;
        private Label label1;
        private NumericUpDown Duration;
        private CheckBox UseCache;
        private GroupBox Simulation;
        private Button StartSimulation;
        private Button StopSimulation;
        private ListBox Times;
        private Button GenerateException;
    }
}

