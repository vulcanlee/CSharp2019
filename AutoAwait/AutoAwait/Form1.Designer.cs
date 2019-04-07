namespace AutoAwait
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnImput = new System.Windows.Forms.Button();
            this.btnCallWebAPI = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.AddValue1 = new System.Windows.Forms.TextBox();
            this.AddValue2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AddValueSum = new System.Windows.Forms.Label();
            this.Message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(267, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "+";
            // 
            // btnImput
            // 
            this.btnImput.Location = new System.Drawing.Point(95, 208);
            this.btnImput.Name = "btnImput";
            this.btnImput.Size = new System.Drawing.Size(75, 23);
            this.btnImput.TabIndex = 0;
            this.btnImput.Text = "輸入";
            this.btnImput.UseVisualStyleBackColor = true;
            this.btnImput.Click += new System.EventHandler(this.btnImput_Click);
            // 
            // btnCallWebAPI
            // 
            this.btnCallWebAPI.Location = new System.Drawing.Point(95, 255);
            this.btnCallWebAPI.Name = "btnCallWebAPI";
            this.btnCallWebAPI.Size = new System.Drawing.Size(75, 23);
            this.btnCallWebAPI.TabIndex = 1;
            this.btnCallWebAPI.Text = "呼叫 Web API";
            this.btnCallWebAPI.UseVisualStyleBackColor = true;
            this.btnCallWebAPI.Click += new System.EventHandler(this.btnCallWebAPI_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(95, 307);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(212, 380);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(456, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 4;
            // 
            // AddValue1
            // 
            this.AddValue1.Location = new System.Drawing.Point(151, 84);
            this.AddValue1.Name = "AddValue1";
            this.AddValue1.Size = new System.Drawing.Size(100, 22);
            this.AddValue1.TabIndex = 1;
            // 
            // AddValue2
            // 
            this.AddValue2.Location = new System.Drawing.Point(294, 84);
            this.AddValue2.Name = "AddValue2";
            this.AddValue2.Size = new System.Drawing.Size(100, 22);
            this.AddValue2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(416, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "=";
            // 
            // AddValueSum
            // 
            this.AddValueSum.AutoSize = true;
            this.AddValueSum.Font = new System.Drawing.Font("新細明體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.AddValueSum.Location = new System.Drawing.Point(456, 82);
            this.AddValueSum.Name = "AddValueSum";
            this.AddValueSum.Size = new System.Drawing.Size(110, 24);
            this.AddValueSum.TabIndex = 8;
            this.AddValueSum.Text = "計算結果";
            // 
            // Message
            // 
            this.Message.AutoSize = true;
            this.Message.Font = new System.Drawing.Font("新細明體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Message.Location = new System.Drawing.Point(168, 144);
            this.Message.Name = "Message";
            this.Message.Size = new System.Drawing.Size(110, 24);
            this.Message.TabIndex = 9;
            this.Message.Text = "系統訊息";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.AddValue1);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.AddValueSum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AddValue2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnCallWebAPI);
            this.Controls.Add(this.btnImput);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnImput;
        private System.Windows.Forms.Button btnCallWebAPI;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox AddValue1;
        private System.Windows.Forms.TextBox AddValue2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label AddValueSum;
        private System.Windows.Forms.Label Message;
    }
}

