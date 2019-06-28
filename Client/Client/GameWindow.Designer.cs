namespace Client
{
    partial class GameWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            this.GameSurface = new Urho.Extensions.WinForms.UrhoSurface();
            this.SuspendLayout();
            // 
            // GameSurface
            // 
            this.GameSurface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameSurface.ForceFocus = false;
            this.GameSurface.FpsLimit = 60;
            this.GameSurface.Location = new System.Drawing.Point(0, 0);
            this.GameSurface.Name = "GameSurface";
            this.GameSurface.Paused = false;
            this.GameSurface.Size = new System.Drawing.Size(800, 450);
            this.GameSurface.TabIndex = 0;
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GameSurface);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameWindow";
            this.Text = "GPx0F";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameWindow_FormClosed);
            this.Load += new System.EventHandler(this.GameWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Urho.Extensions.WinForms.UrhoSurface GameSurface;
    }
}

