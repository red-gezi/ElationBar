namespace 语音下载
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Text_Web = new TextBox();
            Btn_Filiter = new Button();
            Btn_Download = new Button();
            Btn_OpenWeb = new Button();
            OpenDire = new Button();
            CharaTag = new TextBox();
            VoiceItem = new CheckedListBox();
            SuspendLayout();
            // 
            // Text_Web
            // 
            Text_Web.Location = new Point(12, 137);
            Text_Web.Multiline = true;
            Text_Web.Name = "Text_Web";
            Text_Web.Size = new Size(648, 330);
            Text_Web.TabIndex = 0;
            Text_Web.Text = resources.GetString("Text_Web.Text");
            // 
            // Btn_Filiter
            // 
            Btn_Filiter.Location = new Point(204, 473);
            Btn_Filiter.Name = "Btn_Filiter";
            Btn_Filiter.Size = new Size(200, 52);
            Btn_Filiter.TabIndex = 1;
            Btn_Filiter.Text = "提取";
            Btn_Filiter.UseVisualStyleBackColor = true;
            Btn_Filiter.Click += Btn_Filiter_Click;
            // 
            // Btn_Download
            // 
            Btn_Download.Location = new Point(12, 886);
            Btn_Download.Name = "Btn_Download";
            Btn_Download.Size = new Size(200, 56);
            Btn_Download.TabIndex = 3;
            Btn_Download.Text = "下载";
            Btn_Download.UseVisualStyleBackColor = true;
            Btn_Download.Click += Btn_Download_Click;
            // 
            // Btn_OpenWeb
            // 
            Btn_OpenWeb.Location = new Point(58, 57);
            Btn_OpenWeb.Name = "Btn_OpenWeb";
            Btn_OpenWeb.Size = new Size(190, 65);
            Btn_OpenWeb.TabIndex = 4;
            Btn_OpenWeb.Text = "打开人物WIki";
            Btn_OpenWeb.UseVisualStyleBackColor = true;
            Btn_OpenWeb.Click += Btn_OpenWeb_Click;
            // 
            // OpenDire
            // 
            OpenDire.Location = new Point(460, 886);
            OpenDire.Name = "OpenDire";
            OpenDire.Size = new Size(200, 60);
            OpenDire.TabIndex = 5;
            OpenDire.Text = "打开下载文件夹";
            OpenDire.UseVisualStyleBackColor = true;
            OpenDire.Click += OpenDire_Click;
            // 
            // CharaTag
            // 
            CharaTag.Location = new Point(374, 74);
            CharaTag.Name = "CharaTag";
            CharaTag.Size = new Size(150, 30);
            CharaTag.TabIndex = 6;
            CharaTag.Text = "开拓者";
            // 
            // VoiceItem
            // 
            VoiceItem.FormattingEnabled = true;
            VoiceItem.Location = new Point(12, 558);
            VoiceItem.Name = "VoiceItem";
            VoiceItem.Size = new Size(651, 274);
            VoiceItem.TabIndex = 7;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(702, 981);
            Controls.Add(VoiceItem);
            Controls.Add(CharaTag);
            Controls.Add(OpenDire);
            Controls.Add(Btn_OpenWeb);
            Controls.Add(Btn_Download);
            Controls.Add(Btn_Filiter);
            Controls.Add(Text_Web);
            Name = "Form1";
            Text = "米游社人物语音下载";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Text_Web;
        private Button Btn_Filiter;
        private Button Btn_Download;
        private Button Btn_OpenWeb;
        private Button OpenDire;
        private TextBox CharaTag;
        private CheckedListBox VoiceItem;
    }
}
