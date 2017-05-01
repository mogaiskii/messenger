namespace messengerKiller
{
    partial class Form1
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.chatTextBox = new System.Windows.Forms.RichTextBox();
            this.messageBox = new System.Windows.Forms.RichTextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.friendsList = new System.Windows.Forms.ListView();
            this.FriendLabel = new System.Windows.Forms.Label();
            this.friendTextBox = new System.Windows.Forms.TextBox();
            this.friendAddButton = new System.Windows.Forms.Button();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(18, 15);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(75, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Введите имя:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(116, 12);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(134, 20);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.Text = "Noname";
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(339, 12);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 20);
            this.loginButton.TabIndex = 5;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // chatTextBox
            // 
            this.chatTextBox.BackColor = System.Drawing.Color.LightGreen;
            this.chatTextBox.Location = new System.Drawing.Point(15, 72);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.ReadOnly = true;
            this.chatTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.chatTextBox.Size = new System.Drawing.Size(399, 251);
            this.chatTextBox.TabIndex = 3;
            this.chatTextBox.Text = "";
            // 
            // messageBox
            // 
            this.messageBox.Location = new System.Drawing.Point(13, 330);
            this.messageBox.Name = "messageBox";
            this.messageBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.messageBox.Size = new System.Drawing.Size(318, 54);
            this.messageBox.TabIndex = 1;
            this.messageBox.Text = "";
            // 
            // sendButton
            // 
            this.sendButton.Enabled = false;
            this.sendButton.Location = new System.Drawing.Point(338, 330);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 54);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // friendsList
            // 
            this.friendsList.Location = new System.Drawing.Point(429, 39);
            this.friendsList.Name = "friendsList";
            this.friendsList.Size = new System.Drawing.Size(163, 284);
            this.friendsList.TabIndex = 6;
            this.friendsList.UseCompatibleStateImageBehavior = false;
            this.friendsList.SelectedIndexChanged += new System.EventHandler(this.friendsList_SelectedIndexChanged);
            // 
            // FriendLabel
            // 
            this.FriendLabel.AutoSize = true;
            this.FriendLabel.Location = new System.Drawing.Point(435, 326);
            this.FriendLabel.Name = "FriendLabel";
            this.FriendLabel.Size = new System.Drawing.Size(106, 13);
            this.FriendLabel.TabIndex = 7;
            this.FriendLabel.Text = "Введите имя друга:";
            // 
            // friendTextBox
            // 
            this.friendTextBox.Location = new System.Drawing.Point(429, 342);
            this.friendTextBox.Name = "friendTextBox";
            this.friendTextBox.Size = new System.Drawing.Size(163, 20);
            this.friendTextBox.TabIndex = 8;
            // 
            // friendAddButton
            // 
            this.friendAddButton.Location = new System.Drawing.Point(429, 360);
            this.friendAddButton.Name = "friendAddButton";
            this.friendAddButton.Size = new System.Drawing.Size(163, 23);
            this.friendAddButton.TabIndex = 9;
            this.friendAddButton.Text = "Добавить";
            this.friendAddButton.UseVisualStyleBackColor = true;
            this.friendAddButton.Click += new System.EventHandler(this.friendAddButton_Click);
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(19, 42);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(91, 13);
            this.passwordLabel.TabIndex = 10;
            this.passwordLabel.Text = "Введите пароль:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(116, 39);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '-';
            this.passwordTextBox.Size = new System.Drawing.Size(134, 20);
            this.passwordTextBox.TabIndex = 11;
            // 
            // registerButton
            // 
            this.registerButton.Location = new System.Drawing.Point(338, 37);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(75, 23);
            this.registerButton.TabIndex = 12;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 396);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.friendAddButton);
            this.Controls.Add(this.friendTextBox);
            this.Controls.Add(this.FriendLabel);
            this.Controls.Add(this.friendsList);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.Name = "Form1";
            this.Text = "Messenger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.RichTextBox chatTextBox;
        private System.Windows.Forms.RichTextBox messageBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ListView friendsList;
        private System.Windows.Forms.Label FriendLabel;
        private System.Windows.Forms.TextBox friendTextBox;
        private System.Windows.Forms.Button friendAddButton;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button registerButton;
    }
}

